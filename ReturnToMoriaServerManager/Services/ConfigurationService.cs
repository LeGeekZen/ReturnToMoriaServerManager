/*
    Fichier : ConfigurationService.cs
    Emplacement : ReturnToMoriaServerManager/Services/ConfigurationService.cs
    Auteur : Le Geek Zen
    Description : Implémentation du service de gestion de la configuration de l'application
*/

using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public class ConfigurationService(ILogger<ConfigurationService> logger) : IConfigurationService
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        /// <summary>
        /// Charge la configuration depuis le fichier JSON ou crée une configuration par défaut.
        /// </summary>
        /// <returns>Configuration du serveur</returns>
        public ServerConfiguration LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    var jsonContent = File.ReadAllText(_configPath);
                    var config = JsonSerializer.Deserialize<ServerConfiguration>(jsonContent);
                    if (config != null)
                    {
                        return config;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors du chargement de la configuration");
            }

            // Configuration par défaut
            return new ServerConfiguration
            {
                SteamCmdPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SteamCMD"),
                ServerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SteamCMD", "steamapps", "common", "Return to Moria Dedicated Server")
            };
        }

        /// <summary>
        /// Sauvegarde la configuration dans le fichier JSON avec mise à jour automatique du ServerPath.
        /// </summary>
        /// <param name="configuration">Configuration à sauvegarder</param>
        public void SaveConfiguration(ServerConfiguration configuration)
        {
            try
            {
                // Mettre à jour automatiquement le ServerPath en fonction du SteamCmdPath
                if (!string.IsNullOrEmpty(configuration.SteamCmdPath))
                {
                    var expectedServerPath = Path.Combine(configuration.SteamCmdPath, "steamapps", "common", "Return to Moria Dedicated Server");
                    if (configuration.ServerPath != expectedServerPath)
                    {
                        logger.LogInformation("Mise à jour automatique du ServerPath: {OldPath} -> {NewPath}", 
                            configuration.ServerPath, expectedServerPath);
                        configuration.ServerPath = expectedServerPath;
                    }
                }

                var jsonContent = JsonSerializer.Serialize(configuration, _jsonOptions);
                File.WriteAllText(_configPath, jsonContent);
                logger.LogDebug("Configuration sauvegardée: {Path}", _configPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la sauvegarde de la configuration");
                throw;
            }
        }
    }
} 