/*
    Fichier : ISteamCmdService.cs
    Emplacement : ReturnToMoriaServerManager/Services/ISteamCmdService.cs
    Auteur : Le Geek Zen
    Description : Interface pour la gestion des opérations SteamCMD et du serveur Return to Moria
*/

using System;
using System.Threading.Tasks;

namespace ReturnToMoriaServerManager.Services
{
    public interface ISteamCmdService
    {
        /// <summary>
        /// URL officielle de téléchargement de SteamCMD.
        /// </summary>
        string SteamCmdDownloadUrl { get; }
        /// <summary>
        /// AppID Steam du jeu Return to Moria.
        /// </summary>
        string ReturnToMoriaAppId { get; }
        /// <summary>
        /// Installe SteamCMD dans le dossier spécifié.
        /// </summary>
        Task<bool> InstallSteamCmdAsync(string installPath, IProgress<int>? progressCallback = null, Action<string>? outputCallback = null);
        /// <summary>
        /// Vérifie si SteamCMD est installé dans le dossier donné.
        /// </summary>
        bool IsSteamCmdInstalled(string steamCmdPath);
        /// <summary>
        /// Exécute une commande SteamCMD avec les arguments donnés.
        /// </summary>
        Task<bool> ExecuteSteamCmdCommandAsync(string steamCmdPath, string[] commands, Action<string>? outputCallback = null);
        /// <summary>
        /// Installe ou met à jour le serveur Return to Moria via SteamCMD.
        /// </summary>
        Task<bool> InstallOrUpdateServerAsync(string steamCmdPath, string installPath, IProgress<int>? progressCallback = null, Action<string>? outputCallback = null);
        /// <summary>
        /// Démarre le serveur Return to Moria.
        /// </summary>
        Task<bool> StartServerAsync(string serverPath, Action<string>? outputCallback = null, Action? onProcessExited = null);
        /// <summary>
        /// Arrête le serveur Return to Moria.
        /// </summary>
        Task<bool> StopServerAsync(string serverPath, Action<string>? outputCallback = null);
    }
} 