/*
    Fichier : SteamCmdService.cs
    Emplacement : ReturnToMoriaServerManager/Services/SteamCmdService.cs
    Auteur : Le Geek Zen
    Description : Implémentation du service de gestion des opérations SteamCMD et du serveur Return to Moria
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ReturnToMoriaServerManager.Services
{
    public class SteamCmdService : ISteamCmdService
    {
        private readonly ILogger<SteamCmdService> _logger;
        private readonly IFileService _fileService;

        /// <summary>
        /// URL officielle de téléchargement de SteamCMD.
        /// </summary>
        public string SteamCmdDownloadUrl => "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
        
        /// <summary>
        /// AppID Steam du jeu Return to Moria.
        /// </summary>
        public string ReturnToMoriaAppId => "3349480";

        public SteamCmdService(ILogger<SteamCmdService> logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        /// <summary>
        /// Installe SteamCMD dans le dossier spécifié avec téléchargement et extraction.
        /// </summary>
        /// <param name="installPath">Dossier d'installation de SteamCMD</param>
        /// <param name="progressCallback">Callback pour suivre la progression</param>
        /// <param name="outputCallback">Callback pour les messages de sortie</param>
        /// <returns>True si l'installation a réussi</returns>
        public async Task<bool> InstallSteamCmdAsync(string installPath, IProgress<int>? progressCallback = null, Action<string>? outputCallback = null)
        {
            try
            {
                _logger.LogInformation("Début de l'installation de SteamCMD dans {Path}", installPath);

                // Créer le dossier d'installation
                _fileService.CreateDirectoryIfNotExists(installPath);

                // Chemin temporaire pour le téléchargement
                var tempZipPath = Path.Combine(Path.GetTempPath(), "steamcmd.zip");

                // Télécharger SteamCMD
                var downloadProgress = new Progress<int>(percentage =>
                {
                    progressCallback?.Report(percentage / 2); // Téléchargement = 50% du total
                });

                var downloadSuccess = await _fileService.DownloadFileAsync(SteamCmdDownloadUrl, tempZipPath, downloadProgress);
                if (!downloadSuccess)
                {
                    _logger.LogError("Échec du téléchargement de SteamCMD");
                    outputCallback?.Invoke("Échec du téléchargement de SteamCMD");
                    return false;
                }

                // Extraire SteamCMD
                var extractProgress = new Progress<int>(percentage =>
                {
                    progressCallback?.Report(50 + percentage / 2); // Extraction = 50% du total
                });

                var extractSuccess = await _fileService.ExtractZipAsync(tempZipPath, installPath, extractProgress);
                if (!extractSuccess)
                {
                    _logger.LogError("Échec de l'extraction de SteamCMD");
                    outputCallback?.Invoke("Échec de l'extraction de SteamCMD");
                    return false;
                }

                // Nettoyer le fichier temporaire
                _fileService.DeleteFileIfExists(tempZipPath);

                // Exécuter steamcmd.exe une fois pour initialiser les dossiers (en mode fenêtre)
                var steamCmdExe = Path.Combine(installPath, "steamcmd.exe");
                if (_fileService.FileExists(steamCmdExe))
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = steamCmdExe,
                        Arguments = "+quit",
                        WorkingDirectory = installPath,
                        UseShellExecute = true, // Affiche la fenêtre
                        CreateNoWindow = false  // Affiche la fenêtre
                    };
                    using var process = Process.Start(startInfo);
                    if (process != null)
                        await process.WaitForExitAsync();
                }

                _logger.LogInformation("Installation de SteamCMD terminée avec succès");
                outputCallback?.Invoke("Installation de SteamCMD terminée avec succès");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'installation de SteamCMD");
                outputCallback?.Invoke($"Erreur lors de l'installation de SteamCMD : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Vérifie si SteamCMD est installé dans le dossier donné.
        /// </summary>
        /// <param name="steamCmdPath">Chemin du dossier SteamCMD</param>
        /// <returns>True si SteamCMD est installé</returns>
        public bool IsSteamCmdInstalled(string steamCmdPath)
        {
            var steamCmdExe = Path.Combine(steamCmdPath, "steamcmd.exe");
            var isInstalled = _fileService.FileExists(steamCmdExe);
            _logger.LogDebug("Vérification SteamCMD installé: {Path} = {IsInstalled}", steamCmdExe, isInstalled);
            return isInstalled;
        }

        /// <summary>
        /// Exécute une commande SteamCMD avec les arguments donnés.
        /// </summary>
        /// <param name="steamCmdPath">Chemin du dossier SteamCMD</param>
        /// <param name="commands">Tableau de commandes à exécuter</param>
        /// <param name="outputCallback">Callback pour les messages de sortie</param>
        /// <returns>True si l'exécution a réussi</returns>
        public async Task<bool> ExecuteSteamCmdCommandAsync(string steamCmdPath, string[] commands, Action<string>? outputCallback = null)
        {
            try
            {
                _logger.LogInformation("Exécution de commandes SteamCMD: {Commands}", string.Join(", ", commands));

                var steamCmdExe = Path.Combine(steamCmdPath, "steamcmd.exe");
                if (!_fileService.FileExists(steamCmdExe))
                {
                    _logger.LogError("SteamCMD non trouvé: {Path}", steamCmdExe);
                    return false;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = steamCmdExe,
                    Arguments = string.Join(" ", commands.Select(c => $"+{c}")),
                    WorkingDirectory = steamCmdPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = startInfo };

                // Gestion de la sortie
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _logger.LogDebug("SteamCMD Output: {Output}", e.Data);
                        outputCallback?.Invoke(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        _logger.LogWarning("SteamCMD Error: {Error}", e.Data);
                        outputCallback?.Invoke($"ERROR: {e.Data}");
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Attendre la fin de l'exécution
                await process.WaitForExitAsync();

                var success = process.ExitCode == 0;
                _logger.LogInformation("Exécution SteamCMD terminée avec le code: {ExitCode}", process.ExitCode);

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'exécution des commandes SteamCMD");
                return false;
            }
        }

        /// <summary>
        /// Installe ou met à jour le serveur Return to Moria via SteamCMD.
        /// </summary>
        /// <param name="steamCmdPath">Chemin du dossier SteamCMD</param>
        /// <param name="installPath">Dossier d'installation du serveur</param>
        /// <param name="progressCallback">Callback pour suivre la progression</param>
        /// <param name="outputCallback">Callback pour les messages de sortie</param>
        /// <returns>True si l'installation/mise à jour a réussi</returns>
        public async Task<bool> InstallOrUpdateServerAsync(string steamCmdPath, string installPath, IProgress<int>? progressCallback = null, Action<string>? outputCallback = null)
        {
            try
            {
                _logger.LogInformation("Début de l'installation/mise à jour du serveur Return to Moria");

                var commands = new[]
                {
                    "login anonymous",
                    $"app_update {ReturnToMoriaAppId} validate",
                    "quit"
                };

                // Créer le dossier d'installation
                _fileService.CreateDirectoryIfNotExists(installPath);

                // Lancer steamcmd.exe en mode fenêtre avec les commandes
                var steamCmdExe = Path.Combine(steamCmdPath, "steamcmd.exe");
                if (!_fileService.FileExists(steamCmdExe))
                {
                    _logger.LogError("SteamCMD non trouvé: {Path}", steamCmdExe);
                    outputCallback?.Invoke("SteamCMD non trouvé");
                    return false;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = steamCmdExe,
                    Arguments = string.Join(" ", commands.Select(c => $"+{c}")),
                    WorkingDirectory = steamCmdPath,
                    UseShellExecute = true, // Affiche la fenêtre
                    CreateNoWindow = false  // Affiche la fenêtre
                };
                using var process = Process.Start(startInfo);
                if (process != null)
                    await process.WaitForExitAsync();

                _logger.LogInformation("Installation/mise à jour du serveur terminée avec succès");
                outputCallback?.Invoke("Installation/mise à jour du serveur terminée avec succès");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'installation/mise à jour du serveur");
                outputCallback?.Invoke($"Erreur lors de l'installation/mise à jour du serveur : {ex.Message}");
                return false;
            }
        }

        public Task<bool> StartServerAsync(string serverPath, Action<string>? outputCallback = null, Action? onProcessExited = null)
        {
            try
            {
                _logger.LogInformation("Démarrage du serveur Return to Moria");

                // Essayer les deux noms possibles d'exécutable
                var possibleExeNames = new[] { "ReturnToMoriaServer.exe", "MoriaServer.exe" };
                string? serverExe = null;

                foreach (var exeName in possibleExeNames)
                {
                    var exePath = Path.Combine(serverPath, exeName);
                    if (_fileService.FileExists(exePath))
                    {
                        serverExe = exePath;
                        _logger.LogInformation("Exécutable trouvé: {Path}", exePath);
                        break;
                    }
                }

                if (serverExe == null)
                {
                    _logger.LogError("Aucun exécutable du serveur trouvé dans: {Path}", serverPath);
                    outputCallback?.Invoke("Aucun exécutable du serveur trouvé");
                    return Task.FromResult(false);
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = serverExe,
                    WorkingDirectory = serverPath,
                    UseShellExecute = true, // Affiche la fenêtre du serveur
                    CreateNoWindow = false  // Affiche la fenêtre du serveur
                };
                var process = Process.Start(startInfo);
                
                if (process != null && onProcessExited != null)
                {
                    // Surveiller la fin du processus en arrière-plan
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await process.WaitForExitAsync();
                            _logger.LogInformation("Processus serveur terminé avec le code: {ExitCode}", process.ExitCode);
                            onProcessExited?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Erreur lors de la surveillance du processus serveur");
                        }
                        finally
                        {
                            process.Dispose();
                        }
                    });
                }
                
                _logger.LogInformation("Serveur Return to Moria démarré avec succès");
                outputCallback?.Invoke("Serveur Return to Moria démarré avec succès");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du démarrage du serveur");
                outputCallback?.Invoke($"Erreur lors du démarrage du serveur : {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public async Task<bool> StopServerAsync(string serverPath, Action<string>? outputCallback = null)
        {
            try
            {
                _logger.LogInformation("Arrêt du serveur Return to Moria");

                // Chercher le processus du serveur
                var serverProcesses = Process.GetProcessesByName("ReturnToMoriaServer");
                if (serverProcesses.Length == 0)
                {
                    _logger.LogWarning("Aucun processus serveur trouvé");
                    outputCallback?.Invoke("Aucun processus serveur trouvé");
                    return true; // Considéré comme un succès si aucun processus n'est en cours
                }

                foreach (var process in serverProcesses)
                {
                    try
                    {
                        process.Kill();
                        await process.WaitForExitAsync();
                        _logger.LogInformation("Processus serveur arrêté: {ProcessId}", process.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erreur lors de l'arrêt du processus {ProcessId}", process.Id);
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }

                _logger.LogInformation("Serveur Return to Moria arrêté avec succès");
                outputCallback?.Invoke("Serveur Return to Moria arrêté avec succès");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'arrêt du serveur");
                outputCallback?.Invoke($"Erreur lors de l'arrêt du serveur : {ex.Message}");
                return false;
            }
        }
    }
} 