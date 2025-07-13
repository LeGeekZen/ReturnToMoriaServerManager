/*
    Fichier : ServerConfigViewModel.cs
    Emplacement : ReturnToMoriaServerManager/ViewModels/ServerConfigViewModel.cs
    Auteur : Le Geek Zen
    Description : ViewModel pour la configuration avancée du serveur Return to Moria
*/

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using ReturnToMoriaServerManager.Models;
using ReturnToMoriaServerManager.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReturnToMoriaServerManager.ViewModels
{
    public class ServerConfigViewModel : INotifyPropertyChanged
    {
        private readonly ILogger<ServerConfigViewModel> _logger;
        private readonly IServerIniConfigService _serverIniConfigService;
        private readonly IFileService _fileService;
        private readonly ServerConfiguration _serverConfiguration;

        private ServerIniConfiguration _config;
        private bool _isBusy;
        private string _statusMessage = "Prêt";
        private bool _isConfigLoaded;

        public ServerConfigViewModel(
            ILogger<ServerConfigViewModel> logger,
            IServerIniConfigService serverIniConfigService,
            IFileService fileService,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _serverIniConfigService = serverIniConfigService;
            _fileService = fileService;
            _serverConfiguration = configurationService.LoadConfiguration();
            _config = new ServerIniConfiguration();

            // Commandes
            LoadConfigCommand = new RelayCommand(async () => await LoadConfigAsync(), () => !IsBusy);
            SaveConfigCommand = new RelayCommand(async () => await SaveConfigAsync(), () => !IsBusy && IsConfigLoaded);
            ResetToDefaultsCommand = new RelayCommand(async () => await ResetToDefaultsAsync(), () => !IsBusy);
            CreateNewConfigCommand = new RelayCommand(async () => await CreateNewConfigAsync(), () => !IsBusy);

            // Charger la configuration au démarrage
            _ = LoadConfigAsync();
        }

        #region Propriétés

        public ServerIniConfiguration Config
        {
            get => _config;
            set 
            {
                if (SetProperty(ref _config, value))
                {
                    _logger.LogInformation("Config mise à jour. DifficultyPreset = {DifficultyPreset}", _config.DifficultyPreset);
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsConfigLoaded
        {
            get => _isConfigLoaded;
            set => SetProperty(ref _isConfigLoaded, value);
        }

        // Propriété directe pour faciliter le binding
        public string DifficultyPreset
        {
            get => _config.DifficultyPreset;
            set
            {
                if (_config.DifficultyPreset != value)
                {
                    _config.DifficultyPreset = value;
                    _logger.LogInformation("DifficultyPreset changé vers: {Value}", value);
                    OnPropertyChanged(nameof(DifficultyPreset));
                    OnPropertyChanged(nameof(Config));
                }
            }
        }

        #endregion

        #region Commandes

        public ICommand LoadConfigCommand { get; }
        public ICommand SaveConfigCommand { get; }
        public ICommand ResetToDefaultsCommand { get; }
        public ICommand CreateNewConfigCommand { get; }

        #endregion

        #region Méthodes privées

        private Task LoadConfigAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Chargement de la configuration...";

                if (!_fileService.DirectoryExists(_serverConfiguration.ServerPath))
                {
                    StatusMessage = "Le dossier du serveur n'existe pas";
                    return Task.CompletedTask;
                }

                var config = _serverIniConfigService.LoadServerIniConfig(_serverConfiguration.ServerPath);
                if (config != null)
                {
                    Config = config;
                    IsConfigLoaded = true;
                    StatusMessage = "Configuration chargée avec succès";
                    _logger.LogInformation("Configuration du serveur chargée");
                }
                else
                {
                    StatusMessage = "Aucun fichier de configuration trouvé";
                    IsConfigLoaded = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du chargement de la configuration");
                StatusMessage = "Erreur lors du chargement de la configuration";
                IsConfigLoaded = false;
            }
            finally
            {
                IsBusy = false;
            }
            
            return Task.CompletedTask;
        }

        private Task SaveConfigAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Sauvegarde de la configuration...";

                if (!_fileService.DirectoryExists(_serverConfiguration.ServerPath))
                {
                    StatusMessage = "Le dossier du serveur n'existe pas";
                    return Task.CompletedTask;
                }

                _serverIniConfigService.SaveServerIniConfig(_serverConfiguration.ServerPath, Config);
                StatusMessage = "Configuration sauvegardée avec succès";
                _logger.LogInformation("Configuration du serveur sauvegardée");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la sauvegarde de la configuration");
                StatusMessage = "Erreur lors de la sauvegarde de la configuration";
            }
            finally
            {
                IsBusy = false;
            }
            
            return Task.CompletedTask;
        }

        private Task ResetToDefaultsAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Réinitialisation aux valeurs par défaut...";

                Config = new ServerIniConfiguration();
                IsConfigLoaded = true;
                StatusMessage = "Configuration réinitialisée aux valeurs par défaut";
                _logger.LogInformation("Configuration réinitialisée aux valeurs par défaut");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la réinitialisation de la configuration");
                StatusMessage = "Erreur lors de la réinitialisation de la configuration";
            }
            finally
            {
                IsBusy = false;
            }
            
            return Task.CompletedTask;
        }

        private Task CreateNewConfigAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Création d'une nouvelle configuration...";

                if (!_fileService.DirectoryExists(_serverConfiguration.ServerPath))
                {
                    StatusMessage = "Le dossier du serveur n'existe pas";
                    return Task.CompletedTask;
                }

                var newConfig = new ServerIniConfiguration();
                _serverIniConfigService.SaveServerIniConfig(_serverConfiguration.ServerPath, newConfig);
                
                Config = newConfig;
                IsConfigLoaded = true;
                StatusMessage = "Nouvelle configuration créée avec succès";
                _logger.LogInformation("Nouvelle configuration du serveur créée");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la configuration");
                StatusMessage = "Erreur lors de la création de la configuration";
            }
            finally
            {
                IsBusy = false;
            }
            
            return Task.CompletedTask;
        }

        #endregion

        #region INotifyPropertyChanged

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

        #endregion
    }
} 