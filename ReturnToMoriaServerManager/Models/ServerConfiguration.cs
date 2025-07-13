/*
    Fichier : ServerConfiguration.cs
    Emplacement : ReturnToMoriaServerManager/Models/ServerConfiguration.cs
    Auteur : Le Geek Zen
    Description : Modèles de configuration du serveur Return to Moria avec gestion des propriétés
*/

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.IO;

namespace ReturnToMoriaServerManager.Models
{
    /// <summary>
    /// Configuration de base du serveur avec chemins d'installation.
    /// </summary>
    public class ServerConfiguration : INotifyPropertyChanged
    {
        private string _steamCmdPath = "";
        private string _serverPath = "";

        /// <summary>
        /// Chemin d'installation de SteamCMD.
        /// </summary>
        public string SteamCmdPath
        {
            get => _steamCmdPath;
            set
            {
                if (SetProperty(ref _steamCmdPath, value))
                {
                    // Mettre à jour automatiquement le ServerPath
                    var newServerPath = Path.Combine(value, "steamapps", "common", "Return to Moria Dedicated Server");
                    if (ServerPath != newServerPath)
                    {
                        ServerPath = newServerPath;
                    }
                }
            }
        }

        /// <summary>
        /// Chemin d'installation du serveur Return to Moria.
        /// </summary>
        public string ServerPath
        {
            get => _serverPath;
            set => SetProperty(ref _serverPath, value);
        }

        /// <summary>
        /// Événement déclenché lors du changement d'une propriété.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Déclenche l'événement PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Met à jour une propriété et déclenche l'événement PropertyChanged si la valeur a changé.
        /// </summary>
        /// <typeparam name="T">Type de la propriété</typeparam>
        /// <param name="field">Champ privé de la propriété</param>
        /// <param name="value">Nouvelle valeur</param>
        /// <param name="propertyName">Nom de la propriété</param>
        /// <returns>True si la valeur a changé</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    /// <summary>
    /// Configuration complète du serveur Return to Moria avec tous les paramètres INI.
    /// </summary>
    public class MoriaServerConfiguration : INotifyPropertyChanged
    {
        // Section [Main]
        private string _optionalPassword = "";
        /// <summary>
        /// Mot de passe optionnel pour rejoindre le serveur.
        /// </summary>
        public string OptionalPassword
        {
            get => _optionalPassword;
            set => SetProperty(ref _optionalPassword, value);
        }

        // Section [World]
        private string _worldName = "Dedicated Server World";
        /// <summary>
        /// Nom du monde du serveur.
        /// </summary>
        public string WorldName
        {
            get => _worldName;
            set => SetProperty(ref _worldName, value);
        }

        private string _optionalWorldFilename = "";
        /// <summary>
        /// Nom de fichier optionnel du monde.
        /// </summary>
        public string OptionalWorldFilename
        {
            get => _optionalWorldFilename;
            set => SetProperty(ref _optionalWorldFilename, value);
        }

        // Section [World.Create]
        private WorldType _worldType = WorldType.Campaign;
        /// <summary>
        /// Type de monde à créer.
        /// </summary>
        public WorldType WorldType
        {
            get => _worldType;
            set => SetProperty(ref _worldType, value);
        }

        private string _seed = "random";
        /// <summary>
        /// Graine pour la génération du monde.
        /// </summary>
        public string Seed
        {
            get => _seed;
            set => SetProperty(ref _seed, value);
        }

        private DifficultyPreset _difficultyPreset = DifficultyPreset.Normal;
        /// <summary>
        /// Préréglage de difficulté du monde.
        /// </summary>
        public DifficultyPreset DifficultyPreset
        {
            get => _difficultyPreset;
            set => SetProperty(ref _difficultyPreset, value);
        }

        // Custom Difficulty Settings
        private DifficultyLevel _combatDifficulty = DifficultyLevel.Default;
        /// <summary>
        /// Niveau de difficulté des combats.
        /// </summary>
        public DifficultyLevel CombatDifficulty
        {
            get => _combatDifficulty;
            set => SetProperty(ref _combatDifficulty, value);
        }

        private DifficultyLevel _enemyAggression = DifficultyLevel.High;
        /// <summary>
        /// Niveau d'agressivité des ennemis.
        /// </summary>
        public DifficultyLevel EnemyAggression
        {
            get => _enemyAggression;
            set => SetProperty(ref _enemyAggression, value);
        }

        private DifficultyLevel _survivalDifficulty = DifficultyLevel.Default;
        /// <summary>
        /// Niveau de difficulté de survie.
        /// </summary>
        public DifficultyLevel SurvivalDifficulty
        {
            get => _survivalDifficulty;
            set => SetProperty(ref _survivalDifficulty, value);
        }

        private DifficultyLevel _miningDrops = DifficultyLevel.Default;
        /// <summary>
        /// Niveau de difficulté des récompenses de minage.
        /// </summary>
        public DifficultyLevel MiningDrops
        {
            get => _miningDrops;
            set => SetProperty(ref _miningDrops, value);
        }

        private DifficultyLevel _worldDrops = DifficultyLevel.Default;
        /// <summary>
        /// Niveau de difficulté des récompenses du monde.
        /// </summary>
        public DifficultyLevel WorldDrops
        {
            get => _worldDrops;
            set => SetProperty(ref _worldDrops, value);
        }

        private DifficultyLevel _hordeFrequency = DifficultyLevel.Default;
        /// <summary>
        /// Fréquence des hordes d'ennemis.
        /// </summary>
        public DifficultyLevel HordeFrequency
        {
            get => _hordeFrequency;
            set => SetProperty(ref _hordeFrequency, value);
        }

        private DifficultyLevel _siegeFrequency = DifficultyLevel.Default;
        /// <summary>
        /// Fréquence des sièges.
        /// </summary>
        public DifficultyLevel SiegeFrequency
        {
            get => _siegeFrequency;
            set => SetProperty(ref _siegeFrequency, value);
        }

        private DifficultyLevel _patrolFrequency = DifficultyLevel.Default;
        /// <summary>
        /// Fréquence des patrouilles d'ennemis.
        /// </summary>
        public DifficultyLevel PatrolFrequency
        {
            get => _patrolFrequency;
            set => SetProperty(ref _patrolFrequency, value);
        }

        // Section [Host]
        private string _listenAddress = "";
        /// <summary>
        /// Adresse d'écoute du serveur.
        /// </summary>
        public string ListenAddress
        {
            get => _listenAddress;
            set => SetProperty(ref _listenAddress, value);
        }

        private int _listenPort = 7777;
        /// <summary>
        /// Port d'écoute du serveur.
        /// </summary>
        public int ListenPort
        {
            get => _listenPort;
            set => SetProperty(ref _listenPort, value);
        }

        private string _advertiseAddress = "auto";
        /// <summary>
        /// Adresse de publication du serveur.
        /// </summary>
        public string AdvertiseAddress
        {
            get => _advertiseAddress;
            set => SetProperty(ref _advertiseAddress, value);
        }

        private string _advertisePort = "";
        /// <summary>
        /// Port de publication du serveur.
        /// </summary>
        public string AdvertisePort
        {
            get => _advertisePort;
            set => SetProperty(ref _advertisePort, value);
        }

        private int _initialConnectionRetryTime = 120;
        /// <summary>
        /// Temps de retry initial pour les connexions (en secondes).
        /// </summary>
        public int InitialConnectionRetryTime
        {
            get => _initialConnectionRetryTime;
            set => SetProperty(ref _initialConnectionRetryTime, value);
        }

        private int _afterDisconnectionRetryTime = 600;
        public int AfterDisconnectionRetryTime
        {
            get => _afterDisconnectionRetryTime;
            set => SetProperty(ref _afterDisconnectionRetryTime, value);
        }

        // Section [Console]
        private bool _consoleEnabled = true;
        public bool ConsoleEnabled
        {
            get => _consoleEnabled;
            set => SetProperty(ref _consoleEnabled, value);
        }

        // Section [Performance]
        private int _serverFPS = 60;
        public int ServerFPS
        {
            get => _serverFPS;
            set => SetProperty(ref _serverFPS, value);
        }

        private int _loadedAreaLimit = 12;
        public int LoadedAreaLimit
        {
            get => _loadedAreaLimit;
            set => SetProperty(ref _loadedAreaLimit, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    /// <summary>
    /// Types de monde disponibles
    /// </summary>
    public enum WorldType
    {
        Campaign,
        Sandbox
    }

    /// <summary>
    /// Préréglages de difficulté
    /// </summary>
    public enum DifficultyPreset
    {
        Story,
        Solo,
        Normal,
        Hard,
        Custom
    }

    /// <summary>
    /// Niveaux de difficulté personnalisés
    /// </summary>
    public enum DifficultyLevel
    {
        VeryLow,
        Low,
        Default,
        High,
        VeryHigh
    }
} 