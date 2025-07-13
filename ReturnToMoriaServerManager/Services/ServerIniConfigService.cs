/*
    Fichier : ServerIniConfigService.cs
    Emplacement : ReturnToMoriaServerManager/Services/ServerIniConfigService.cs
    Auteur : Le Geek Zen
    Description : Implémentation du service de gestion du fichier INI de configuration du serveur
*/

using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public class ServerIniConfigService : IServerIniConfigService
    {
        private readonly ILogger<ServerIniConfigService> _logger;
        private const string IniFileName = "MoriaServerConfig.ini";

        public ServerIniConfigService(ILogger<ServerIniConfigService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Vérifie si le fichier INI du serveur existe dans le dossier spécifié.
        /// </summary>
        /// <param name="serverPath">Chemin du dossier du serveur</param>
        /// <returns>True si le fichier INI existe</returns>
        public bool IsServerIniPresent(string serverPath)
        {
            var iniPath = Path.Combine(serverPath, IniFileName);
            var exists = File.Exists(iniPath);
            _logger.LogDebug("Vérification fichier INI: {Path} = {Exists}", iniPath, exists);
            return exists;
        }

        /// <summary>
        /// Charge la configuration INI du serveur depuis le fichier.
        /// </summary>
        /// <param name="serverPath">Chemin du dossier du serveur</param>
        /// <returns>Configuration du serveur ou null si le fichier n'existe pas</returns>
        public ServerIniConfiguration? LoadServerIniConfig(string serverPath)
        {
            try
            {
                var iniPath = Path.Combine(serverPath, IniFileName);
                if (!File.Exists(iniPath))
                {
                    _logger.LogWarning("Fichier INI non trouvé: {Path}", iniPath);
                    return null;
                }

                var config = new ServerIniConfiguration();
                var lines = File.ReadAllLines(iniPath);
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

                    // Gérer les clés selon la section
                    switch (currentSection)
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
                                    // Enlever les guillemets si présents
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
                            config.WorldType = ConvertWorldTypeToFrench(value);
                            break;
                        case "seed":
                            config.Seed = value;
                            break;
                        case "difficulty.preset":
                            config.DifficultyPreset = ConvertDifficultyPresetToFrench(value);
                            break;
                            }
                            break;
                        case "difficulty.custom":
                            switch (key)
                            {
                                case "combatdifficulty":
                                    config.CombatDifficulty = ConvertDifficultyLevelToFrench(value);
                                    break;
                                case "enemyaggression":
                                    config.EnemyAggression = ConvertDifficultyLevelToFrench(value);
                                    break;
                                case "survivaldifficulty":
                                    config.SurvivalDifficulty = ConvertDifficultyLevelToFrench(value);
                                    break;
                                case "miningdrops":
                                    config.MiningDrops = ConvertDifficultyLevelToFrench(value);
                                    break;
                                case "worlddrops":
                                    config.WorldDrops = ConvertDifficultyLevelToFrench(value);
                                    break;
                                case "hordefrequency":
                                    config.HordeFrequency = ConvertDifficultyLevelToFrench(value);
                                    break;
                                case "siegefrequency":
                                    config.SiegeFrequency = ConvertDifficultyLevelToFrench(value);
                                    break;
                                case "patrolfrequency":
                                    config.PatrolFrequency = ConvertDifficultyLevelToFrench(value);
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
                                    config.ListenPort = value;
                                    break;
                                case "advertiseaddress":
                                    config.AdvertiseAddress = value;
                                    break;
                                case "advertiseport":
                                    config.AdvertisePort = value;
                                    break;
                                case "initialconnectionretrytime":
                                    config.InitialConnectionRetryTime = value;
                                    break;
                                case "afterdisconnectionretrytime":
                                    config.AfterDisconnectionRetryTime = value;
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
                                    config.ServerFPS = value;
                                    break;
                                case "loadedarealimit":
                                    config.LoadedAreaLimit = value;
                                    break;
                            }
                            break;
                        default:
                            // Gérer les clés sans section (pour compatibilité)
                            switch (key)
                            {
                                case "name":
                                    config.WorldName = value;
                                    break;
                                case "optionalpassword":
                                    config.OptionalPassword = value;
                                    break;
                                case "listenport":
                                    config.ListenPort = value;
                                    break;
                            }
                            break;
                    }
                }

                _logger.LogDebug("Configuration INI chargée depuis: {Path}", iniPath);
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du chargement de la configuration INI");
                return null;
            }
        }

        /// <summary>
        /// Sauvegarde la configuration INI du serveur dans le fichier.
        /// </summary>
        /// <param name="serverPath">Chemin du dossier du serveur</param>
        /// <param name="config">Configuration à sauvegarder</param>
        public void SaveServerIniConfig(string serverPath, ServerIniConfiguration config)
        {
            try
            {
                var iniPath = Path.Combine(serverPath, IniFileName);
                var content = GenerateIniContent(config);
                File.WriteAllText(iniPath, content, Encoding.UTF8);
                _logger.LogInformation("Configuration INI sauvegardée: {Path}", iniPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la sauvegarde de la configuration INI");
                throw;
            }
        }

        /// <summary>
        /// Crée un fichier INI de configuration pour le serveur.
        /// </summary>
        /// <param name="serverPath">Chemin du dossier du serveur</param>
        /// <param name="config">Configuration à sauvegarder</param>
        public void CreateServerIniConfig(string serverPath, ServerIniConfiguration config)
        {
            try
            {
                // Cette méthode n'est plus utilisée car le fichier se crée automatiquement
                // On garde juste la sauvegarde pour les modifications
                SaveServerIniConfig(serverPath, config);
                _logger.LogInformation("Configuration INI sauvegardée: {Path}", Path.Combine(serverPath, IniFileName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la sauvegarde de la configuration INI");
                throw;
            }
        }

        private static string GenerateIniContent(ServerIniConfiguration config)
        {
            var sb = new StringBuilder();
            sb.AppendLine("; Configuration file for the dedicated server.");
            sb.AppendLine("; Only edit this configuration when the server is offline.");
            sb.AppendLine("; This list may be overwritten when changes are made in-game.");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("[Main]");
            sb.AppendLine("; If a password is specified, players will need to enter a password to join the server.");
            sb.AppendLine("; Case-sensitive.");
            sb.AppendLine($"OptionalPassword={config.OptionalPassword}");
            sb.AppendLine();
            sb.AppendLine();
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
            sb.AppendLine("[World.Create]");
            sb.AppendLine("; World generation parameters, only for newly created worlds.");
            sb.AppendLine();
            sb.AppendLine("; Type of world to create:");
            sb.AppendLine(";  * campaign");
            sb.AppendLine(";  * sandbox");
            sb.AppendLine($"Type={ConvertWorldTypeToEnglish(config.WorldType)}");
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
            sb.AppendLine($"Difficulty.Preset={ConvertDifficultyPresetToEnglish(config.DifficultyPreset)}");
            sb.AppendLine();
            sb.AppendLine("; Custom difficulty properties, used only if the \"Difficulty.Preset\" property is set to custom.");
            sb.AppendLine("; Acceptable values are verylow, low, default, high and veryhigh.");
            sb.AppendLine("; The base enemy damage and hit points.");
            sb.AppendLine($"Difficulty.Custom.CombatDifficulty={ConvertDifficultyLevelToEnglish(config.CombatDifficulty)}");
            sb.AppendLine("; How often enemies attack and how many will attack at once.");
            sb.AppendLine($"Difficulty.Custom.EnemyAggression={ConvertDifficultyLevelToEnglish(config.EnemyAggression)}");
            sb.AppendLine("; The strength of various buffs, speed a dwarf succumbs to despair and");
            sb.AppendLine("; the decay rates of stamina, energy and hunger.");
            sb.AppendLine($"Difficulty.Custom.SurvivalDifficulty={ConvertDifficultyLevelToEnglish(config.SurvivalDifficulty)}");
            sb.AppendLine("; Volume of ore that drops from each vein.");
            sb.AppendLine($"Difficulty.Custom.MiningDrops={ConvertDifficultyLevelToEnglish(config.MiningDrops)}");
            sb.AppendLine("; The drop rates of rewards for defeating orcs and enemies.");
            sb.AppendLine($"Difficulty.Custom.WorldDrops={ConvertDifficultyLevelToEnglish(config.WorldDrops)}");
            sb.AppendLine("; How often noisy actions will trigger a horde of orcs.");
            sb.AppendLine($"Difficulty.Custom.HordeFrequency={ConvertDifficultyLevelToEnglish(config.HordeFrequency)}");
            sb.AppendLine("; How often orcs will target and attack a dwarf base.");
            sb.AppendLine($"Difficulty.Custom.SiegeFrequency={ConvertDifficultyLevelToEnglish(config.SiegeFrequency)}");
            sb.AppendLine("; How often orc and enemy groups spawn.");
            sb.AppendLine($"Difficulty.Custom.PatrolFrequency={ConvertDifficultyLevelToEnglish(config.PatrolFrequency)}");
            sb.AppendLine();
            sb.AppendLine();
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
            sb.AppendLine("[Console]");
            sb.AppendLine("; Open the console window so you can type commands. (true or false)");
            sb.AppendLine("; Note: To close the app, you will need to kill the process in Windows Task Manager.");
            sb.AppendLine($"Enabled={config.ConsoleEnabled.ToString().ToLower()}");
            sb.AppendLine();
            sb.AppendLine();
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

        /// <summary>
        /// Convertit le type de monde français vers l'anglais pour l'écriture dans le fichier .ini
        /// </summary>
        private static string ConvertWorldTypeToEnglish(string frenchValue)
        {
            return frenchValue.ToLower() switch
            {
                "campagne" => "campaign",
                "bac à sable" => "sandbox",
                _ => frenchValue.ToLower() // Si c'est déjà en anglais ou autre, on le laisse tel quel
            };
        }

        /// <summary>
        /// Convertit le préréglage de difficulté français vers l'anglais pour l'écriture dans le fichier .ini
        /// </summary>
        private static string ConvertDifficultyPresetToEnglish(string frenchValue)
        {
            return frenchValue.ToLower() switch
            {
                "histoire" => "story",
                "solo" => "solo",
                "normal" => "normal",
                "difficile" => "hard",
                "personnalisé" => "custom",
                _ => frenchValue.ToLower() // Si c'est déjà en anglais ou autre, on le laisse tel quel
            };
        }

        /// <summary>
        /// Convertit le niveau de difficulté français vers l'anglais pour l'écriture dans le fichier .ini
        /// </summary>
        private static string ConvertDifficultyLevelToEnglish(string frenchValue)
        {
            return frenchValue.ToLower() switch
            {
                "très faible" => "verylow",
                "faible" => "low",
                "par défaut" => "default",
                "élevé" => "high",
                "très élevé" => "veryhigh",
                _ => frenchValue.ToLower() // Si c'est déjà en anglais ou autre, on le laisse tel quel
            };
        }

        /// <summary>
        /// Convertit le type de monde anglais vers le français pour l'affichage
        /// </summary>
        private static string ConvertWorldTypeToFrench(string englishValue)
        {
            return englishValue.ToLower() switch
            {
                "campaign" => "Campagne",
                "sandbox" => "Bac à sable",
                _ => englishValue // Si c'est déjà en français ou autre, on le laisse tel quel
            };
        }

        /// <summary>
        /// Convertit le préréglage de difficulté anglais vers le français pour l'affichage
        /// </summary>
        private static string ConvertDifficultyPresetToFrench(string englishValue)
        {
            return englishValue.ToLower() switch
            {
                "story" => "Histoire",
                "solo" => "Solo",
                "normal" => "Normal",
                "hard" => "Difficile",
                "custom" => "Personnalisé",
                _ => englishValue // Si c'est déjà en français ou autre, on le laisse tel quel
            };
        }

        /// <summary>
        /// Convertit le niveau de difficulté anglais vers le français pour l'affichage
        /// </summary>
        private static string ConvertDifficultyLevelToFrench(string englishValue)
        {
            return englishValue.ToLower() switch
            {
                "verylow" => "Très faible",
                "low" => "Faible",
                "default" => "Par défaut",
                "high" => "Élevé",
                "veryhigh" => "Très élevé",
                _ => englishValue // Si c'est déjà en français ou autre, on le laisse tel quel
            };
        }
    }
} 