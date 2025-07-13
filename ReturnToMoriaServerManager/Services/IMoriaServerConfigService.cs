/*
    Fichier : IMoriaServerConfigService.cs
    Emplacement : ReturnToMoriaServerManager/Services/IMoriaServerConfigService.cs
    Auteur : Le Geek Zen
    Description : Interface du service de gestion de la configuration complète du serveur Moria
*/

using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public interface IMoriaServerConfigService
    {
        /// <summary>
        /// Charge la configuration complète du serveur depuis le fichier INI
        /// </summary>
        /// <param name="serverPath">Chemin du serveur</param>
        /// <returns>Configuration du serveur ou null si le fichier n'existe pas</returns>
        MoriaServerConfiguration? LoadMoriaServerConfig(string serverPath);

        /// <summary>
        /// Sauvegarde la configuration complète du serveur dans le fichier INI
        /// </summary>
        /// <param name="serverPath">Chemin du serveur</param>
        /// <param name="config">Configuration à sauvegarder</param>
        void SaveMoriaServerConfig(string serverPath, MoriaServerConfiguration config);

        /// <summary>
        /// Vérifie si le fichier de configuration existe
        /// </summary>
        /// <param name="serverPath">Chemin du serveur</param>
        /// <returns>True si le fichier existe</returns>
        bool IsMoriaServerConfigPresent(string serverPath);

        /// <summary>
        /// Crée une configuration par défaut
        /// </summary>
        /// <returns>Configuration par défaut</returns>
        MoriaServerConfiguration CreateDefaultConfig();
    }
} 