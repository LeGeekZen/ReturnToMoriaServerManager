/*
    Fichier : ServerStatusService.cs
    Emplacement : ReturnToMoriaServerManager/Services/ServerStatusService.cs
    Auteur : Le Geek Zen
    Description : Service pour lire et surveiller le statut du serveur Return to Moria
*/

using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public class ServerStatusService : IServerStatusService
    {
        private readonly ILogger<ServerStatusService> _logger;
        private CancellationTokenSource? _cancellationTokenSource;
        private ServerStatusInfo _currentStatus;
        private bool _isMonitoring;

        public ServerStatusService(ILogger<ServerStatusService> logger)
        {
            _logger = logger;
            _currentStatus = new ServerStatusInfo();
        }

        /// <summary>
        /// Événement déclenché lors d'un changement de statut du serveur.
        /// </summary>
        public event EventHandler<ServerStatusInfo>? StatusChanged;

        /// <summary>
        /// Statut courant du serveur.
        /// </summary>
        public ServerStatusInfo CurrentStatus => _currentStatus;
        
        /// <summary>
        /// Indique si la surveillance du serveur est active.
        /// </summary>
        public bool IsMonitoring => _isMonitoring;

        /// <summary>
        /// Démarre la surveillance du statut du serveur en arrière-plan.
        /// </summary>
        /// <param name="serverPath">Chemin du dossier du serveur</param>
        public async Task StartMonitoringAsync(string serverPath)
        {
            if (_isMonitoring)
                return;

            _logger.LogInformation("Démarrage de la surveillance du statut serveur: {Path}", serverPath);
            _cancellationTokenSource = new CancellationTokenSource();
            _isMonitoring = true;

            await Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var status = await GetStatusAsync(serverPath);
                        if (!status.Equals(_currentStatus))
                        {
                            _logger.LogDebug("Nouveau statut détecté: Status={Status}, WorldName={WorldName}, InviteCode={InviteCode}", 
                                status.Status, status.WorldName, status.InviteCode);
                            _currentStatus = status;
                            StatusChanged?.Invoke(this, status);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erreur lors de la surveillance du statut serveur");
                    }

                    await Task.Delay(2000, _cancellationTokenSource.Token); // Vérification toutes les 2 secondes
                }
            }, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Arrête la surveillance du statut du serveur.
        /// </summary>
        public Task StopMonitoringAsync()
        {
            _isMonitoring = false;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Récupère le statut actuel du serveur depuis le fichier Status.json.
        /// </summary>
        /// <param name="serverPath">Chemin du dossier du serveur</param>
        /// <returns>Informations de statut du serveur</returns>
        public async Task<ServerStatusInfo> GetStatusAsync(string serverPath)
        {
            var statusFilePath = Path.Combine(serverPath, "Moria", "Saved", "Config", "Status.json");
            _logger.LogDebug("Lecture du fichier de statut: {Path}", statusFilePath);
            
            if (!File.Exists(statusFilePath))
            {
                _logger.LogDebug("Fichier de statut non trouvé: {Path}", statusFilePath);
                return new ServerStatusInfo();
            }

            try
            {
                var jsonContent = await File.ReadAllTextAsync(statusFilePath);
                _logger.LogDebug("Contenu du fichier de statut: {Content}", jsonContent);
                var status = JsonSerializer.Deserialize<ServerStatusInfo>(jsonContent) ?? new ServerStatusInfo();
                _logger.LogDebug("Statut désérialisé: Status={Status}, WorldName={WorldName}, InviteCode={InviteCode}", 
                    status.Status, status.WorldName, status.InviteCode);
                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la lecture du fichier de statut: {Path}", statusFilePath);
                return new ServerStatusInfo();
            }
        }
    }
} 