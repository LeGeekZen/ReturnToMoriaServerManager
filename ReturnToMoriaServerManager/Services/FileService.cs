/*
    Fichier : FileService.cs
    Emplacement : ReturnToMoriaServerManager/Services/FileService.cs
    Auteur : Le Geek Zen
    Description : Implémentation du service de gestion des fichiers et dossiers
*/

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace ReturnToMoriaServerManager.Services
{
    public class FileService(ILogger<FileService> logger, HttpClient httpClient) : IFileService
    {

        /// <summary>
        /// Vérifie si un fichier existe à l'emplacement spécifié.
        /// </summary>
        /// <param name="path">Chemin du fichier à vérifier</param>
        /// <returns>True si le fichier existe, false sinon</returns>
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Vérifie si un dossier existe à l'emplacement spécifié.
        /// </summary>
        /// <param name="path">Chemin du dossier à vérifier</param>
        /// <returns>True si le dossier existe, false sinon</returns>
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Crée le dossier s'il n'existe pas déjà.
        /// </summary>
        /// <param name="path">Chemin du dossier à créer</param>
        public void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Supprime le fichier s'il existe.
        /// </summary>
        /// <param name="path">Chemin du fichier à supprimer</param>
        public void DeleteFileIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Télécharge un fichier depuis une URL vers un chemin de destination, avec suivi de progression optionnel.
        /// </summary>
        /// <param name="url">URL du fichier à télécharger</param>
        /// <param name="destinationPath">Chemin de destination du fichier</param>
        /// <param name="progress">Callback pour suivre la progression du téléchargement</param>
        /// <returns>True si le téléchargement a réussi, false sinon</returns>
        public async Task<bool> DownloadFileAsync(string url, string destinationPath, IProgress<int>? progress = null)
        {
            try
            {
                using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

                var buffer = new byte[8192];
                var totalBytesRead = 0L;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    totalBytesRead += bytesRead;

                    if (totalBytes > 0 && progress != null)
                    {
                        var percentage = (int)((double)totalBytesRead / totalBytes * 100);
                        progress.Report(percentage);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors du téléchargement de {Url}", url);
                return false;
            }
        }

        /// <summary>
        /// Extrait une archive ZIP vers un dossier cible, avec suivi de progression optionnel.
        /// </summary>
        /// <param name="zipPath">Chemin de l'archive ZIP</param>
        /// <param name="extractPath">Dossier de destination pour l'extraction</param>
        /// <param name="progress">Callback pour suivre la progression de l'extraction</param>
        /// <returns>True si l'extraction a réussi, false sinon</returns>
        public Task<bool> ExtractZipAsync(string zipPath, string extractPath, IProgress<int>? progress = null)
        {
            try
            {
                if (!File.Exists(zipPath))
                {
                    logger.LogError("Fichier ZIP non trouvé: {Path}", zipPath);
                    return Task.FromResult(false);
                }

                CreateDirectoryIfNotExists(extractPath);

                using var archive = ZipFile.OpenRead(zipPath);
                var totalEntries = archive.Entries.Count;
                var processedEntries = 0;

                foreach (var entry in archive.Entries)
                {
                    var destinationPath = Path.Combine(extractPath, entry.FullName);
                    var destinationDir = Path.GetDirectoryName(destinationPath);

                    if (!string.IsNullOrEmpty(destinationDir))
                    {
                        CreateDirectoryIfNotExists(destinationDir);
                    }

                    if (!string.IsNullOrEmpty(entry.Name))
                    {
                        entry.ExtractToFile(destinationPath, true);
                    }

                    processedEntries++;
                    if (progress != null)
                    {
                        var percentage = (int)((double)processedEntries / totalEntries * 100);
                        progress.Report(percentage);
                    }
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'extraction du ZIP: {Path}", zipPath);
                return Task.FromResult(false);
            }
        }
    }
} 