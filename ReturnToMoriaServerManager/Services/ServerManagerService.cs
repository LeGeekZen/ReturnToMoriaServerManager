/*
    Fichier : ServerManagerService.cs
    Emplacement : ReturnToMoriaServerManager/Services/ServerManagerService.cs
    Auteur : Le Geek Zen
    Description : Implémentation du service de gestion du cycle de vie du serveur Return to Moria
*/

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public class ServerManagerService : IServerManagerService
    {
        private readonly ILogger<ServerManagerService> _logger;
        private ServerConfiguration? _configuration;

        public ServerManagerService(ILogger<ServerManagerService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initialise le service avec la configuration du serveur.
        /// </summary>
        /// <param name="configuration">Configuration du serveur</param>
        public void Initialize(ServerConfiguration configuration)
        {
            _configuration = configuration;
            _logger.LogInformation("Service de gestion du serveur initialisé avec le chemin: {ServerPath}", configuration.ServerPath);
        }

        /// <summary>
        /// Met à jour la configuration du serveur.
        /// </summary>
        /// <param name="configuration">Nouvelle configuration du serveur</param>
        public void UpdateConfiguration(ServerConfiguration configuration)
        {
            _configuration = configuration;
            _logger.LogInformation("Configuration du service de gestion du serveur mise à jour avec le chemin: {ServerPath}", configuration.ServerPath);
        }

        /// <summary>
        /// Vérifie si le serveur est installé en cherchant les exécutables possibles.
        /// </summary>
        /// <returns>True si le serveur est installé, false sinon</returns>
        public bool IsServerInstalled()
        {
            if (_configuration == null)
                return false;

            // Vérifier les deux noms possibles d'exécutable
            var possibleExeNames = new[] { "ReturnToMoriaServer.exe", "MoriaServer.exe" };
            
            foreach (var exeName in possibleExeNames)
            {
                var serverExe = Path.Combine(_configuration.ServerPath, exeName);
                if (File.Exists(serverExe))
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Vérifie de façon asynchrone le statut du serveur en analysant les processus et fichiers.
        /// </summary>
        /// <returns>Statut actuel du serveur</returns>
        public Task<ServerStatus> CheckServerStatusAsync()
        {
            try
            {
                if (_configuration == null)
                    return Task.FromResult(ServerStatus.Unknown);

                var statusFilePath = Path.Combine(_configuration.ServerPath, "Status.json");
                if (!File.Exists(statusFilePath))
                    return Task.FromResult(ServerStatus.Stopped);

                // Vérifier si le processus du serveur est en cours d'exécution
                var serverProcesses = System.Diagnostics.Process.GetProcessesByName("ReturnToMoriaServer");
                if (serverProcesses.Length == 0)
                    return Task.FromResult(ServerStatus.Stopped);

                return Task.FromResult(ServerStatus.Running);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification du statut du serveur");
                return Task.FromResult(ServerStatus.Error);
            }
        }

        // Événements requis par l'interface mais non utilisés
        #pragma warning disable CS0067
        /// <summary>
        /// Événement déclenché lors d'un changement de statut du serveur (non implémenté).
        /// </summary>
        public event EventHandler<ServerStatus>? ServerStatusChanged { add { } remove { } }
        /// <summary>
        /// Événement déclenché lorsqu'un message est reçu du serveur (non implémenté).
        /// </summary>
        public event EventHandler<string>? ServerMessageReceived { add { } remove { } }
        #pragma warning restore CS0067
    }
} 