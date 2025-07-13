/*
    Fichier : IServerIniConfigService.cs
    Emplacement : ReturnToMoriaServerManager/Services/IServerIniConfigService.cs
    Auteur : Le Geek Zen
    Description : Interface pour la gestion du fichier INI de configuration du serveur
*/

using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public interface IServerIniConfigService
    {
        /// <summary>
        /// Vérifie si le fichier INI du serveur existe.
        /// </summary>
        bool IsServerIniPresent(string serverPath);
        /// <summary>
        /// Charge la configuration INI du serveur.
        /// </summary>
        ServerIniConfiguration? LoadServerIniConfig(string serverPath);
        /// <summary>
        /// Sauvegarde la configuration INI du serveur.
        /// </summary>
        void SaveServerIniConfig(string serverPath, ServerIniConfiguration config);
        /// <summary>
        /// Crée un fichier INI de configuration pour le serveur.
        /// </summary>
        void CreateServerIniConfig(string serverPath, ServerIniConfiguration config);
    }
} 