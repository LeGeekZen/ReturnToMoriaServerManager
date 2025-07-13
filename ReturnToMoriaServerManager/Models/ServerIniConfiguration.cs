/*
    Fichier : ServerIniConfiguration.cs
    Emplacement : ReturnToMoriaServerManager/Models/ServerIniConfiguration.cs
    Auteur : Le Geek Zen
    Description : Modèle pour la configuration du fichier INI du serveur Return to Moria
*/

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace ReturnToMoriaServerManager.Models
{
    /// <summary>
    /// Configuration du serveur basée sur le fichier INI avec gestion des propriétés.
    /// </summary>
    public class ServerIniConfiguration : INotifyPropertyChanged
    {
        // [Main] Section
        private string _optionalPassword = string.Empty;

        // [World] Section
        private string _worldName = string.Empty;
        private string _optionalWorldFilename = string.Empty;

        // [World.Create] Section
        private string _worldType = "Campagne";
        private string _seed = "random";
        private string _difficultyPreset = "Normal";

        // [Difficulty.Custom] Section
        private string _combatDifficulty = "Par défaut";
        private string _enemyAggression = "Par défaut";
        private string _survivalDifficulty = "Par défaut";
        private string _miningDrops = "Par défaut";
        private string _worldDrops = "Par défaut";
        private string _hordeFrequency = "Par défaut";
        private string _siegeFrequency = "Par défaut";
        private string _patrolFrequency = "Par défaut";

        // [Host] Section
        private string _listenAddress = "0.0.0.0";
        private string _listenPort = "7777";
        private string _advertiseAddress = string.Empty;
        private string _advertisePort = "7777";
        private string _initialConnectionRetryTime = "5";
        private string _afterDisconnectionRetryTime = "10";

        // [Console] Section
        private bool _consoleEnabled = true;

        // [Performance] Section
        private string _serverFPS = "60";
        private string _loadedAreaLimit = "100";

        // [Main] Section
        /// <summary>
        /// Mot de passe optionnel pour rejoindre le serveur.
        /// </summary>
        public string OptionalPassword
        {
            get => _optionalPassword;
            set => SetProperty(ref _optionalPassword, value);
        }

        // [World] Section
        /// <summary>
        /// Nom du monde du serveur.
        /// </summary>
        public string WorldName
        {
            get => _worldName;
            set => SetProperty(ref _worldName, value);
        }

        /// <summary>
        /// Nom de fichier optionnel du monde.
        /// </summary>
        public string OptionalWorldFilename
        {
            get => _optionalWorldFilename;
            set => SetProperty(ref _optionalWorldFilename, value);
        }

        // [World.Create] Section
        /// <summary>
        /// Type de monde à créer (Campagne, Sandbox, etc.).
        /// </summary>
        public string WorldType
        {
            get => _worldType;
            set => SetProperty(ref _worldType, value);
        }

        /// <summary>
        /// Graine pour la génération du monde.
        /// </summary>
        public string Seed
        {
            get => _seed;
            set => SetProperty(ref _seed, value);
        }

        /// <summary>
        /// Préréglage de difficulté du monde.
        /// </summary>
        public string DifficultyPreset
        {
            get => _difficultyPreset;
            set => SetProperty(ref _difficultyPreset, value);
        }

        // [Difficulty.Custom] Section
        /// <summary>
        /// Niveau de difficulté des combats.
        /// </summary>
        public string CombatDifficulty
        {
            get => _combatDifficulty;
            set => SetProperty(ref _combatDifficulty, value);
        }

        /// <summary>
        /// Niveau d'agressivité des ennemis.
        /// </summary>
        public string EnemyAggression
        {
            get => _enemyAggression;
            set => SetProperty(ref _enemyAggression, value);
        }

        /// <summary>
        /// Niveau de difficulté de survie.
        /// </summary>
        public string SurvivalDifficulty
        {
            get => _survivalDifficulty;
            set => SetProperty(ref _survivalDifficulty, value);
        }

        /// <summary>
        /// Niveau de difficulté des récompenses de minage.
        /// </summary>
        public string MiningDrops
        {
            get => _miningDrops;
            set => SetProperty(ref _miningDrops, value);
        }

        /// <summary>
        /// Niveau de difficulté des récompenses du monde.
        /// </summary>
        public string WorldDrops
        {
            get => _worldDrops;
            set => SetProperty(ref _worldDrops, value);
        }

        /// <summary>
        /// Fréquence des hordes d'ennemis.
        /// </summary>
        public string HordeFrequency
        {
            get => _hordeFrequency;
            set => SetProperty(ref _hordeFrequency, value);
        }

        /// <summary>
        /// Fréquence des sièges.
        /// </summary>
        public string SiegeFrequency
        {
            get => _siegeFrequency;
            set => SetProperty(ref _siegeFrequency, value);
        }

        /// <summary>
        /// Fréquence des patrouilles d'ennemis.
        /// </summary>
        public string PatrolFrequency
        {
            get => _patrolFrequency;
            set => SetProperty(ref _patrolFrequency, value);
        }

        // [Host] Section
        /// <summary>
        /// Adresse d'écoute du serveur.
        /// </summary>
        public string ListenAddress
        {
            get => _listenAddress;
            set => SetProperty(ref _listenAddress, value);
        }

        /// <summary>
        /// Port d'écoute du serveur.
        /// </summary>
        public string ListenPort
        {
            get => _listenPort;
            set => SetProperty(ref _listenPort, value);
        }

        /// <summary>
        /// Adresse de publication du serveur.
        /// </summary>
        public string AdvertiseAddress
        {
            get => _advertiseAddress;
            set => SetProperty(ref _advertiseAddress, value);
        }

        /// <summary>
        /// Port de publication du serveur.
        /// </summary>
        public string AdvertisePort
        {
            get => _advertisePort;
            set => SetProperty(ref _advertisePort, value);
        }

        /// <summary>
        /// Temps de retry initial pour les connexions (en secondes).
        /// </summary>
        public string InitialConnectionRetryTime
        {
            get => _initialConnectionRetryTime;
            set => SetProperty(ref _initialConnectionRetryTime, value);
        }

        /// <summary>
        /// Temps de retry après déconnexion (en secondes).
        /// </summary>
        public string AfterDisconnectionRetryTime
        {
            get => _afterDisconnectionRetryTime;
            set => SetProperty(ref _afterDisconnectionRetryTime, value);
        }

        // [Console] Section
        /// <summary>
        /// Indique si la console est activée.
        /// </summary>
        public bool ConsoleEnabled
        {
            get => _consoleEnabled;
            set => SetProperty(ref _consoleEnabled, value);
        }

        // [Performance] Section
        /// <summary>
        /// FPS cible du serveur.
        /// </summary>
        public string ServerFPS
        {
            get => _serverFPS;
            set => SetProperty(ref _serverFPS, value);
        }

        /// <summary>
        /// Limite des zones chargées.
        /// </summary>
        public string LoadedAreaLimit
        {
            get => _loadedAreaLimit;
            set => SetProperty(ref _loadedAreaLimit, value);
        }

        // Propriétés héritées pour compatibilité
        /// <summary>
        /// Alias pour OptionalPassword (compatibilité).
        /// </summary>
        public string Password
        {
            get => OptionalPassword;
            set => OptionalPassword = value;
        }

        public string ServerName
        {
            get => _worldName;
            set => SetProperty(ref _worldName, value);
        }

        public int Port
        {
            get => int.TryParse(_listenPort, out int port) ? port : 7777;
            set => SetProperty(ref _listenPort, value.ToString());
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
} 