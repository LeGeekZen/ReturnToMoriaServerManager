/*
    Fichier : IFileService.cs
    Emplacement : ReturnToMoriaServerManager/Services/IFileService.cs
    Auteur : Le Geek Zen
    Description : Interface pour les opérations de gestion de fichiers et de dossiers
*/

using System;
using System.Threading.Tasks;

namespace ReturnToMoriaServerManager.Services
{
    public interface IFileService
    {
        /// <summary>
        /// Vérifie si un fichier existe à l'emplacement spécifié.
        /// </summary>
        bool FileExists(string path);
        /// <summary>
        /// Vérifie si un dossier existe à l'emplacement spécifié.
        /// </summary>
        bool DirectoryExists(string path);
        /// <summary>
        /// Crée le dossier s'il n'existe pas déjà.
        /// </summary>
        void CreateDirectoryIfNotExists(string path);
        /// <summary>
        /// Supprime le fichier s'il existe.
        /// </summary>
        void DeleteFileIfExists(string path);
        /// <summary>
        /// Télécharge un fichier depuis une URL vers un chemin de destination, avec suivi de progression optionnel.
        /// </summary>
        Task<bool> DownloadFileAsync(string url, string destinationPath, IProgress<int>? progress = null);
        /// <summary>
        /// Extrait une archive ZIP vers un dossier cible, avec suivi de progression optionnel.
        /// </summary>
        Task<bool> ExtractZipAsync(string zipPath, string extractPath, IProgress<int>? progress = null);
    }
} 