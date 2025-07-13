/*
    Fichier : MoriaServerConfigService.cs
    Emplacement : ReturnToMoriaServerManager/Services/MoriaServerConfigService.cs
    Auteur : Le Geek Zen
    Description : Implémentation du service de gestion de la configuration complète du serveur Moria
*/

using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public class MoriaServerConfigService(ILogger<MoriaServerConfigService> logger) : IMoriaServerConfigService
    {
        private const string ConfigFileName = "MoriaServerConfig.ini";

        /// <summary>
        /// Vérifie si le fichier de configuration Moria existe dans le dossier du serveur.
        /// </summary>
        /// <param name="serverPath">Chemin du dossier du serveur</param>
        /// <returns>True si le fichier de configuration existe</returns>
        public bool IsMoriaServerConfigPresent(string serverPath)
        {
            var configPath = Path.Combine(serverPath, ConfigFileName);
            var exists = File.Exists(configPath);
            logger.LogDebug("Vérification fichier config Moria: {Path} = {Exists}", configPath, exists);
            return exists;
        }

        /// <summary>
        /// Charge la configuration complète du serveur Moria depuis le fichier INI.
        /// </summary>
        /// <param name="serverPath">Chemin du dossier du serveur</param>
        /// <returns>Configuration du serveur ou null si le fichier n'existe pas</returns>
        public MoriaServerConfiguration? LoadMoriaServerConfig(string serverPath)
        {
            try
            {
                var configPath = Path.Combine(serverPath, ConfigFileName);
                if (!File.Exists(configPath))
                {
                    logger.LogWarning("Fichier de configuration Moria non trouvé: {Path}", configPath);
                    return null;
                }

                var config = new MoriaServerConfiguration();
                var lines = File.ReadAllLines(configPath);
                var currentSection = "";

                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(';'))
                        continue;

                    // Détecter les sections [Section]
                    if (trimmedLine.StartsWith('[') && trimmedLine.EndsWith(']'))
                    {
                        currentSection = trimmedLine[1..^1].ToLower();
                        continue;
                    }

                    var parts = trimmedLine.Split('=', 2);
                    if (parts.Length != 2)
                        continue;

                    var key = parts[0].Trim().ToLower();
                    var value = parts[1].Trim();

                    ParseConfigValue(config, currentSection, key, value);
                }

                logger.LogDebug("Configuration Moria chargée depuis: {Path}", configPath);
                return config;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors du chargement de la configuration Moria");
                return null;
            }
        }

        /// <summary>
        /// Sauvegarde la configuration complète du serveur Moria dans le fichier INI.
        /// </summary>
        /// <param name="serverPath">Chemin du dossier du serveur</param>
        /// <param name="config">Configuration à sauvegarder</param>
        public void SaveMoriaServerConfig(string serverPath, MoriaServerConfiguration config)
        {
            try
            {
                var configPath = Path.Combine(serverPath, ConfigFileName);
                var content = GenerateConfigContent(config);
                File.WriteAllText(configPath, content, Encoding.UTF8);
                logger.LogInformation("Configuration Moria sauvegardée: {Path}", configPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la sauvegarde de la configuration Moria");
                throw;
            }
        }

        /// <summary>
        /// Crée une configuration par défaut pour le serveur Moria.
        /// </summary>
        /// <returns>Configuration par défaut</returns>
        public MoriaServerConfiguration CreateDefaultConfig()
        {
            return new MoriaServerConfiguration();
        }

        private static void ParseConfigValue(MoriaServerConfiguration config, string section, string key, string value)
        {
            switch (section)
            {
                case "main":
                    switch (key)
                    {
                        case "optionalpassword":
                            config.OptionalPassword = value;
                            break;
                    }
                    break;

                case "world":
                    switch (key)
                    {
                        case "name":
                            config.WorldName = value.Trim('"');
                            break;
                        case "optionalworldfilename":
                            config.OptionalWorldFilename = value;
                            break;
                    }
                    break;

                case "world.create":
                    switch (key)
                    {
                        case "type":
                            if (Enum.TryParse<WorldType>(value, true, out var worldType))
                                config.WorldType = worldType;
                            break;
                        case "seed":
                            config.Seed = value;
                            break;
                        case "difficulty.preset":
                            if (Enum.TryParse<DifficultyPreset>(value, true, out var difficultyPreset))
                                config.DifficultyPreset = difficultyPreset;
                            break;
                        case "difficulty.custom.combatdifficulty":
                            if (Enum.TryParse<DifficultyLevel>(value, true, out var combatDifficulty))
                                config.CombatDifficulty = combatDifficulty;
                            break;
                        case "difficulty.custom.enemyaggression":
                            if (Enum.TryParse<DifficultyLevel>(value, true, out var enemyAggression))
                                config.EnemyAggression = enemyAggression;
                            break;
                        case "difficulty.custom.survivaldifficulty":
                            if (Enum.TryParse<DifficultyLevel>(value, true, out var survivalDifficulty))
                                config.SurvivalDifficulty = survivalDifficulty;
                            break;
                        case "difficulty.custom.miningdrops":
                            if (Enum.TryParse<DifficultyLevel>(value, true, out var miningDrops))
                                config.MiningDrops = miningDrops;
                            break;
                        case "difficulty.custom.worlddrops":
                            if (Enum.TryParse<DifficultyLevel>(value, true, out var worldDrops))
                                config.WorldDrops = worldDrops;
                            break;
                        case "difficulty.custom.hordefrequency":
                            if (Enum.TryParse<DifficultyLevel>(value, true, out var hordeFrequency))
                                config.HordeFrequency = hordeFrequency;
                            break;
                        case "difficulty.custom.siegfrequency":
                            if (Enum.TryParse<DifficultyLevel>(value, true, out var siegeFrequency))
                                config.SiegeFrequency = siegeFrequency;
                            break;
                        case "difficulty.custom.patrolfrequency":
                            if (Enum.TryParse<DifficultyLevel>(value, true, out var patrolFrequency))
                                config.PatrolFrequency = patrolFrequency;
                            break;
                    }
                    break;

                case "host":
                    switch (key)
                    {
                        case "listenaddress":
                            config.ListenAddress = value;
                            break;
                        case "listenport":
                            if (int.TryParse(value, out var listenPort))
                                config.ListenPort = listenPort;
                            break;
                        case "advertiseaddress":
                            config.AdvertiseAddress = value;
                            break;
                        case "advertiseport":
                            config.AdvertisePort = value;
                            break;
                        case "initialconnectionretrytime":
                            if (int.TryParse(value, out var initialRetryTime))
                                config.InitialConnectionRetryTime = initialRetryTime;
                            break;
                        case "afterdisconnectionretrytime":
                            if (int.TryParse(value, out var afterRetryTime))
                                config.AfterDisconnectionRetryTime = afterRetryTime;
                            break;
                    }
                    break;

                case "console":
                    switch (key)
                    {
                        case "enabled":
                            if (bool.TryParse(value, out var consoleEnabled))
                                config.ConsoleEnabled = consoleEnabled;
                            break;
                    }
                    break;

                case "performance":
                    switch (key)
                    {
                        case "serverfps":
                            if (int.TryParse(value, out var serverFPS))
                                config.ServerFPS = serverFPS;
                            break;
                        case "loadedarealimit":
                            if (int.TryParse(value, out var loadedAreaLimit))
                                config.LoadedAreaLimit = loadedAreaLimit;
                            break;
                    }
                    break;
            }
        }

        private static string GenerateConfigContent(MoriaServerConfiguration config)
        {
            var sb = new StringBuilder();
            
            // En-tête
            sb.AppendLine("; Configuration file for the dedicated server.");
            sb.AppendLine("; Only edit this configuration when the server is offline.");
            sb.AppendLine("; This list may be overwritten when changes are made in-game.");
            sb.AppendLine();
            sb.AppendLine();

            // Section [Main]
            sb.AppendLine("[Main]");
            sb.AppendLine("; If a password is specified, players will need to enter a password to join the server.");
            sb.AppendLine("; Case-sensitive.");
            sb.AppendLine($"OptionalPassword={config.OptionalPassword}");
            sb.AppendLine();
            sb.AppendLine();

            // Section [World]
            sb.AppendLine("[World]");
            sb.AppendLine("; Name of the saved world to load. If it doesn't exist, create it");
            sb.AppendLine("; with the properties defined in the [World.Create] section.");
            sb.AppendLine($"Name=\"{config.WorldName}\"");
            sb.AppendLine();
            sb.AppendLine("; The file name of the world to load. Helpful if you have multiple worlds with the same name.");
            sb.AppendLine("; Note that renaming world files is not currently supported.");
            sb.AppendLine($"OptionalWorldFilename={config.OptionalWorldFilename}");
            sb.AppendLine();
            sb.AppendLine();

            // Section [World.Create]
            sb.AppendLine("[World.Create]");
            sb.AppendLine("; World generation parameters, only for newly created worlds.");
            sb.AppendLine();
            sb.AppendLine("; Type of world to create:");
            sb.AppendLine(";  * campaign");
            sb.AppendLine(";  * sandbox");
            sb.AppendLine($"Type={config.WorldType.ToString().ToLower()}");
            sb.AppendLine();
            sb.AppendLine("; Seed for world generation:");
            sb.AppendLine(";  * random - Generates a random seed.");
            sb.AppendLine(";  * <integer> - Use the given number.");
            sb.AppendLine($"Seed={config.Seed}");
            sb.AppendLine();
            sb.AppendLine("; Specify world difficulty:");
            sb.AppendLine(";  * story: Ideal for players who want to experience the story without much danger.");
            sb.AppendLine(";  * solo: Ideal for a single dwarf.");
            sb.AppendLine(";  * normal: Ideal for a small company of dwarves.");
            sb.AppendLine(";  * hard: Ideal for a large company of dwarves.");
            sb.AppendLine(";  * custom: Use customizations for each category of difficulty.");
            sb.AppendLine($"Difficulty.Preset={config.DifficultyPreset.ToString().ToLower()}");
            sb.AppendLine();
            sb.AppendLine("; Custom difficulty properties, used only if the \"Difficulty.Preset\" property is set to custom.");
            sb.AppendLine("; Acceptable values are verylow, low, default, high and veryhigh.");
            sb.AppendLine("; The base enemy damage and hit points.");
            sb.AppendLine($"Difficulty.Custom.CombatDifficulty={config.CombatDifficulty.ToString().ToLower()}");
            sb.AppendLine("; How often enemies attack and how many will attack at once.");
            sb.AppendLine($"Difficulty.Custom.EnemyAggression={config.EnemyAggression.ToString().ToLower()}");
            sb.AppendLine("; The strength of various buffs, speed a dwarf succumbs to despair and");
            sb.AppendLine("; the decay rates of stamina, energy and hunger.");
            sb.AppendLine($"Difficulty.Custom.SurvivalDifficulty={config.SurvivalDifficulty.ToString().ToLower()}");
            sb.AppendLine("; Volume of ore that drops from each vein.");
            sb.AppendLine($"Difficulty.Custom.MiningDrops={config.MiningDrops.ToString().ToLower()}");
            sb.AppendLine("; The drop rates of rewards for defeating orcs and enemies.");
            sb.AppendLine($"Difficulty.Custom.WorldDrops={config.WorldDrops.ToString().ToLower()}");
            sb.AppendLine("; How often noisy actions will trigger a horde of orcs.");
            sb.AppendLine($"Difficulty.Custom.HordeFrequency={config.HordeFrequency.ToString().ToLower()}");
            sb.AppendLine("; How often orcs will target and attack a dwarf base.");
            sb.AppendLine($"Difficulty.Custom.SiegeFrequency={config.SiegeFrequency.ToString().ToLower()}");
            sb.AppendLine("; How often orc and enemy groups spawn.");
            sb.AppendLine($"Difficulty.Custom.PatrolFrequency={config.PatrolFrequency.ToString().ToLower()}");
            sb.AppendLine();
            sb.AppendLine();

            // Section [Host]
            sb.AppendLine("[Host]");
            sb.AppendLine();
            sb.AppendLine("; Local IP address to bind on the server.");
            sb.AppendLine("; Normally leave this empty.");
            sb.AppendLine("; Possible values:");
            sb.AppendLine("; * Empty value for default (bind all adapters.)");
            sb.AppendLine("; * <IPv4> or <IPv6> ... manually specify the IP address.");
            sb.AppendLine($"ListenAddress={config.ListenAddress}");
            sb.AppendLine();
            sb.AppendLine("; Port bound by the server for incoming connections.");
            sb.AppendLine("; You must allow TCP and UDP traffic on your firewall and may need to set up port forwarding.");
            sb.AppendLine("; Possible values:");
            sb.AppendLine(";  * -1 ... use the default engine port (7777)");
            sb.AppendLine(";  * <integer> ... manually specify the port.");
            sb.AppendLine($"ListenPort={config.ListenPort}");
            sb.AppendLine();
            sb.AppendLine("; Host reported to clients. Clients will try to connect it when joining a hosted session.");
            sb.AppendLine("; Normally set this to \"auto\".");
            sb.AppendLine("; If this machine and all of your friends are playing on a LAN, set this to \"local\" to avoid having to set up port forwarding.");
            sb.AppendLine("; If there is an issue with automatic detection, you can specify your server's IP address directly.");
            sb.AppendLine("; Possible values:");
            sb.AppendLine(";  * auto ... detect public IP address. For public servers or servers behind NAT, proxy,");
            sb.AppendLine(";             or in a container with properly configured port mapping or port forwarding.");
            sb.AppendLine(";  * local ... automatically detect local IP address. For LAN games and servers with a public IP address.");
            sb.AppendLine(";  * <IPv4> or <IPv6> ... manually specify the IP address clients should connect to.");
            sb.AppendLine($"AdvertiseAddress={config.AdvertiseAddress}");
            sb.AppendLine();
            sb.AppendLine("; Port reported to clients. Clients will try to connect it when joining a hosted session.");
            sb.AppendLine("; Normally leave this empty.");
            sb.AppendLine("; Possible values:");
            sb.AppendLine(";  * <empty> ... use the ListenPort. To be used when connecting directly or via port forwarding.");
            sb.AppendLine(";  * <integer> ... manually specify the port. To be used when the server");
            sb.AppendLine(";                  listen port is mapped to a different, exposed port. This is rare.");
            sb.AppendLine($"AdvertisePort={config.AdvertisePort}");
            sb.AppendLine();
            sb.AppendLine("; If you fail to host on launch, the maximum number of seconds to retry.");
            sb.AppendLine($"InitialConnectionRetryTime={config.InitialConnectionRetryTime}");
            sb.AppendLine();
            sb.AppendLine("; If your hosted session drops, the maximum number of seconds to try to rehost.");
            sb.AppendLine($"AfterDisconnectionRetryTime={config.AfterDisconnectionRetryTime}");
            sb.AppendLine();
            sb.AppendLine();

            // Section [Console]
            sb.AppendLine("[Console]");
            sb.AppendLine("; Open the console window so you can type commands. (true or false)");
            sb.AppendLine("; Note: To close the app, you will need to kill the process in Windows Task Manager.");
            sb.AppendLine($"Enabled={config.ConsoleEnabled.ToString().ToLower()}");
            sb.AppendLine();
            sb.AppendLine();

            // Section [Performance]
            sb.AppendLine("[Performance]");
            sb.AppendLine("; Frames per second to tick the server.");
            sb.AppendLine("; Typically leave this at 60. Higher values are unlikely to improve a dedicated server.");
            sb.AppendLine("; If your server uses too much CPU, you might try 30 fps instead.");
            sb.AppendLine($"ServerFPS={config.ServerFPS}");
            sb.AppendLine("; Maximum number of areas to keep loaded at once, a number between 4 and 32.");
            sb.AppendLine("; This number greatly impacts memory, CPU usage and bandwidth.");
            sb.AppendLine("; The default is 12 loaded areas, which generally supports 8 player sessions well.");
            sb.AppendLine("; If you wish to improve performance, reducing to 8 loaded areas supports 4 player sessions very well.");
            sb.AppendLine("; Increasing this number above 12 may decrease the number of loading walls you see,");
            sb.AppendLine("; at the cost of much higher memory and CPU usage, as more enemies will need to be simulated to fill those areas.");
            sb.AppendLine($"LoadedAreaLimit={config.LoadedAreaLimit}");

            return sb.ToString();
        }
    }
} 