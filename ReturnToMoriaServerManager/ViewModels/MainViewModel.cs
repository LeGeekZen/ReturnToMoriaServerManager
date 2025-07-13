/*
    Fichier : MainViewModel.cs
    Emplacement : ReturnToMoriaServerManager/ViewModels/MainViewModel.cs
    Auteur : Le Geek Zen
    Description : ViewModel principal avec la logique métier pour la gestion du serveur
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using ReturnToMoriaServerManager.Models;
using ReturnToMoriaServerManager.Services;
using Microsoft.Extensions.DependencyInjection;
using ReturnToMoriaServerManager.Views;
using ReturnToMoriaServerManager.ViewModels;


namespace ReturnToMoriaServerManager.ViewModels
{
    /// <summary>
    /// ViewModel principal pour la gestion du serveur Return to Moria
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ILogger<MainViewModel> _logger;
        private readonly ISteamCmdService _steamCmdService;
        private readonly IServerManagerService _serverManagerService;
        private readonly IFileService _fileService;
        private readonly IConfigurationService _configurationService;
        private readonly IServerIniConfigService _serverIniConfigService;
        private readonly IServerStatusService _serverStatusService;
        private ServerConfiguration _configuration;
        private ServerStatus _serverStatus = ServerStatus.Error;
        private ServerStatusInfo _currentServerStatus;
        private bool _isBusy;
        private string _statusMessage = "Prêt";
        private int _progressValue;
        private bool _isProgressVisible;
        private ServerIniConfiguration? _serverIniConfig;
        private bool _isServerStatusAvailable;
        private bool _isWorldNameEmpty;
        public bool IsServerIniPresent { get; private set; }
        public ServerIniConfiguration? ServerIniConfig
        {
            get => _serverIniConfig;
            set { SetProperty(ref _serverIniConfig, value); }
        }

        public MainViewModel(
            ILogger<MainViewModel> logger,
            ISteamCmdService steamCmdService,
            IServerManagerService serverManagerService,
            IFileService fileService,
            IConfigurationService configurationService)
        {
            _logger = logger;
            _steamCmdService = steamCmdService;
            _serverManagerService = serverManagerService;
            _fileService = fileService;
            _configurationService = configurationService;
            _currentServerStatus = new ServerStatusInfo();
            if (App.Services == null)
                throw new InvalidOperationException("Le provider de services n'est pas initialisé.");
            _serverIniConfigService = App.Services.GetRequiredService<IServerIniConfigService>();
            _serverStatusService = App.Services.GetRequiredService<IServerStatusService>();

            // Charger la configuration
            _configuration = _configurationService.LoadConfiguration();

            // Sauvegarde automatique à chaque modification d'une propriété de configuration
            _configuration.PropertyChanged += (s, e) =>
            {
                try
                {
                    _configurationService.SaveConfiguration(_configuration);
                    _fileService.CreateDirectoryIfNotExists(_configuration.SteamCmdPath);
                    OnPropertyChanged(nameof(IsSteamCmdInstalled));
                    
                    // Si le SteamCmdPath change, vérifier si le serveur est installé dans le nouveau dossier
                    if (e.PropertyName == nameof(ServerConfiguration.SteamCmdPath))
                    {
                        _logger.LogInformation("Chemin SteamCMD modifié, vérification de l'installation du serveur...");
                        
                        // Mettre à jour le service de gestion du serveur avec la nouvelle configuration
                        _serverManagerService.UpdateConfiguration(_configuration);
                        
                        // Notifier les changements d'état
                        OnPropertyChanged(nameof(IsServerInstalled));
                        OnPropertyChanged(nameof(InstallOrUpdateServerButtonText));
                        
                        // Recharger la configuration serveur INI si elle existe
                        LoadServerIniConfig();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la sauvegarde automatique de la configuration");
                }
            };

            // Initialiser le service de gestion du serveur
            _serverManagerService.Initialize(_configuration);

            // S'abonner aux événements du serveur
            _serverManagerService.ServerStatusChanged += OnServerStatusChanged;
            _serverManagerService.ServerMessageReceived += OnServerMessageReceived;
            
            // S'abonner aux événements du service de statut
            _serverStatusService.StatusChanged += OnServerStatusInfoChanged;

            // Initialiser les commandes
            InstallSteamCmdCommand = new RelayCommand(InstallSteamCmdAsync, () => !IsBusy);
            InstallServerCommand = new RelayCommand(InstallServerAsync, () => !IsBusy && IsSteamCmdInstalled);
            BrowseSteamCmdPathCommand = new RelayCommand(BrowseSteamCmdPath);
            SaveConfigurationCommand = new RelayCommand(SaveConfigurationAsync);
            CreateServerIniCommand = new RelayCommand(CreateServerIni, () => !IsBusy && IsServerInstalled);
            SaveServerIniCommand = new RelayCommand(SaveServerIni, () => IsServerIniPresent);
            GenerateWorldCommand = new RelayCommand(GenerateWorldAsync, () => 
            {
                var canExecute = !IsBusy && IsServerIniPresent;
                _logger.LogDebug("GenerateWorldCommand CanExecute - IsBusy: {IsBusy}, IsServerIniPresent: {IsPresent}, CanExecute: {CanExecute}", 
                    IsBusy, IsServerIniPresent, canExecute);
                return canExecute;
            });
            StartServerCommand = new RelayCommand(StartServerAsync, () => !IsBusy && IsServerInstalled);
            StopServerCommand = new RelayCommand(StopServerAsync, () => !IsBusy);

            // Chargement de la config serveur INI
            LoadServerIniConfig();

            // Vérifier l'état initial
            CheckInitialState();
            
            // Démarrer la surveillance périodique du statut
            _ = Task.Run(async () => await StartStatusMonitoring());
            
            // Démarrer la surveillance du service de statut si le serveur est déjà en cours d'exécution
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000); // Attendre un peu que l'interface soit chargée
                if (IsServerIniPresent)
                {
                    _logger.LogInformation("Démarrage de la surveillance du statut au démarrage de l'application");
                    await _serverStatusService.StartMonitoringAsync(Configuration.ServerPath);
                }
            });
        }

        #region Propriétés

        public ServerConfiguration Configuration
        {
            get => _configuration;
            set
            {
                if (SetProperty(ref _configuration, value))
                {
                    try
                    {
                        // Sauvegarder automatiquement la configuration
                        _configurationService.SaveConfiguration(_configuration);
                        // Créer le dossier SteamCMD si besoin
                        _fileService.CreateDirectoryIfNotExists(_configuration.SteamCmdPath);
                        // Notifier l'état installé
                        OnPropertyChanged(nameof(IsSteamCmdInstalled));
                        // Rafraîchir les commandes qui dépendent de l'installation
                        ((RelayCommand)InstallServerCommand).RaiseCanExecuteChanged();
                        ((RelayCommand)CreateServerIniCommand).RaiseCanExecuteChanged();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erreur lors de la sauvegarde automatique de la configuration");
                    }
                }
            }
        }

        public ServerStatus ServerStatus
        {
            get => _serverStatus;
            set => SetProperty(ref _serverStatus, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                // Rafraîchir les commandes quand l'état occupé change
                ((RelayCommand)InstallSteamCmdCommand).RaiseCanExecuteChanged();
                ((RelayCommand)InstallServerCommand).RaiseCanExecuteChanged();
                ((RelayCommand)BrowseSteamCmdPathCommand).RaiseCanExecuteChanged();
                ((RelayCommand)SaveConfigurationCommand).RaiseCanExecuteChanged();
                ((RelayCommand)CreateServerIniCommand).RaiseCanExecuteChanged();
                ((RelayCommand)SaveServerIniCommand).RaiseCanExecuteChanged();
                ((RelayCommand)GenerateWorldCommand).RaiseCanExecuteChanged();
                ((RelayCommand)StartServerCommand).RaiseCanExecuteChanged();
                ((RelayCommand)StopServerCommand).RaiseCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public int ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        public bool IsProgressVisible
        {
            get => _isProgressVisible;
            set => SetProperty(ref _isProgressVisible, value);
        }

        public bool IsSteamCmdInstalled => _steamCmdService.IsSteamCmdInstalled(Configuration.SteamCmdPath);

        public string InstallOrUpdateServerButtonText => IsServerInstalled ? "Mettre à jour le serveur" : "Installer le serveur";

        public bool IsServerInstalled 
        { 
            get => _serverManagerService.IsServerInstalled(); 
        }

        public bool IsServerStatusAvailable
        {
            get => _isServerStatusAvailable;
            set => SetProperty(ref _isServerStatusAvailable, value);
        }

        public bool IsWorldNameEmpty
        {
            get => _isWorldNameEmpty;
            set => SetProperty(ref _isWorldNameEmpty, value);
        }

        public ServerStatusInfo CurrentServerStatus
        {
            get => _currentServerStatus;
            set => SetProperty(ref _currentServerStatus, value);
        }

        public ICommand CreateServerIniCommand { get; }
        public ICommand SaveServerIniCommand { get; }
        public ICommand GenerateWorldCommand { get; }
        public ICommand StartServerCommand { get; }
        public ICommand StopServerCommand { get; }

        #endregion

        #region Commandes

        public ICommand InstallSteamCmdCommand { get; }
        public ICommand InstallServerCommand { get; }
        public ICommand BrowseSteamCmdPathCommand { get; }
        public ICommand SaveConfigurationCommand { get; }

        #endregion

        #region Méthodes privées

        private void RefreshCommands()
        {
            ((RelayCommand)InstallSteamCmdCommand).RaiseCanExecuteChanged();
            ((RelayCommand)InstallServerCommand).RaiseCanExecuteChanged();
            ((RelayCommand)BrowseSteamCmdPathCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SaveConfigurationCommand).RaiseCanExecuteChanged();
            ((RelayCommand)CreateServerIniCommand).RaiseCanExecuteChanged();
            ((RelayCommand)SaveServerIniCommand).RaiseCanExecuteChanged();
            ((RelayCommand)GenerateWorldCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StartServerCommand).RaiseCanExecuteChanged();
            ((RelayCommand)StopServerCommand).RaiseCanExecuteChanged();
        }

        private async void CheckInitialState()
        {
            try
            {
                StatusMessage = "Vérification de l'état initial...";
                
                // Vérifier le statut du serveur
                var status = await _serverManagerService.CheckServerStatusAsync();
                ServerStatus = status;

                // Rafraîchir les commandes après vérification de l'état
                RefreshCommands();

                StatusMessage = "Prêt";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification de l'état initial");
                StatusMessage = "Erreur lors de l'initialisation";
            }
        }

        private async Task InstallSteamCmdAsync()
        {
            try
            {
                IsBusy = true;
                IsProgressVisible = true;
                StatusMessage = "Installation de SteamCMD...";

                var progress = new Progress<int>(percentage =>
                {
                    ProgressValue = percentage;
                });

                var outputCallback = new Action<string>(message =>
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Supprimer les commandes et méthodes inutilisées
                    }));
                });

                var success = await _steamCmdService.InstallSteamCmdAsync(Configuration.SteamCmdPath, progress, outputCallback);

                if (success)
                {
                    StatusMessage = "SteamCMD installé avec succès";
                    OnPropertyChanged(nameof(IsSteamCmdInstalled));
                    RefreshCommands();
                }
                else
                {
                    StatusMessage = "Échec de l'installation de SteamCMD";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'installation de SteamCMD");
                StatusMessage = "Erreur lors de l'installation de SteamCMD";
            }
            finally
            {
                IsBusy = false;
                IsProgressVisible = false;
                ProgressValue = 0;
            }
        }

        private async Task InstallServerAsync()
        {
            try
            {
                IsBusy = true;
                IsProgressVisible = true;
                StatusMessage = IsServerInstalled ? "Mise à jour du serveur Return to Moria..." : "Installation du serveur Return to Moria...";

                var progress = new Progress<int>(percentage =>
                {
                    ProgressValue = percentage;
                });

                var outputCallback = new Action<string>(message =>
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Supprimer les commandes et méthodes inutilisées
                    }));
                });

                var success = await _steamCmdService.InstallOrUpdateServerAsync(
                    Configuration.SteamCmdPath, 
                    Configuration.ServerPath, 
                    progress, 
                    outputCallback);

                if (success)
                {
                    StatusMessage = IsServerInstalled ? "Serveur mis à jour avec succès" : "Serveur installé avec succès";
                    OnPropertyChanged(nameof(IsServerInstalled));
                    OnPropertyChanged(nameof(InstallOrUpdateServerButtonText));
                    RefreshCommands();
                }
                else
                {
                    StatusMessage = "Échec de l'installation/mise à jour du serveur";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'installation/mise à jour du serveur");
                StatusMessage = "Erreur lors de l'installation/mise à jour du serveur";
            }
            finally
            {
                IsBusy = false;
                IsProgressVisible = false;
                ProgressValue = 0;
            }
        }

        private Task BrowseSteamCmdPath()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Sélectionner le dossier d'installation de SteamCMD",
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Sélectionner un dossier"
            };

            if (dialog.ShowDialog() == true)
            {
                var selectedPath = Path.GetDirectoryName(dialog.FileName);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    Configuration.SteamCmdPath = selectedPath;
                    OnPropertyChanged(nameof(Configuration));
                    OnPropertyChanged(nameof(IsSteamCmdInstalled));
                    RefreshCommands();
                }
            }
            
            return Task.CompletedTask;
        }

        private Task SaveConfigurationAsync()
        {
            try
            {
                _configurationService.SaveConfiguration(Configuration);
                StatusMessage = "Configuration sauvegardée avec succès";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la sauvegarde de la configuration");
                StatusMessage = "Erreur lors de la sauvegarde de la configuration";
            }
            
            return Task.CompletedTask;
        }

        private void OnServerStatusChanged(object? sender, ServerStatus status)
        {
            ServerStatus = status;
            StatusMessage = status switch
            {
                ServerStatus.Error => "Erreur du serveur",
                ServerStatus.Stopped => "Serveur arrêté",
                ServerStatus.Starting => "Démarrage du serveur",
                ServerStatus.Running => "Serveur actif",
                ServerStatus.Stopping => "Arrêt du serveur",
                _ => "Statut inconnu"
            };
        }

        private void OnServerMessageReceived(object? sender, string message)
        {
            // Supprimer les commandes et méthodes inutilisées
        }

        private void OnServerStatusInfoChanged(object? sender, ServerStatusInfo status)
        {
            _logger.LogInformation("Événement StatusChanged reçu du service de statut: Status={Status}, WorldName={WorldName}, InviteCode={InviteCode}", 
                status.Status, status.WorldName, status.InviteCode);
            
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _logger.LogDebug("Mise à jour du statut serveur: Status={Status}, WorldName={WorldName}, InviteCode={InviteCode}", 
                    status.Status, status.WorldName, status.InviteCode);
                
                CurrentServerStatus = status;
                CheckServerStatusAvailability();
                
                // Forcer la mise à jour de l'interface
                OnPropertyChanged(nameof(CurrentServerStatus));
                OnPropertyChanged(nameof(IsWorldNameEmpty));
                OnPropertyChanged(nameof(IsServerStatusAvailable));
                
                // Forcer la mise à jour des propriétés calculées de ServerStatusInfo
                OnPropertyChanged("CurrentServerStatus.Status");
                OnPropertyChanged("CurrentServerStatus.WorldName");
                OnPropertyChanged("CurrentServerStatus.InviteCode");
                OnPropertyChanged("CurrentServerStatus.AdvertisedAddressAndPort");
                OnPropertyChanged("CurrentServerStatus.Players");
                OnPropertyChanged("CurrentServerStatus.Version");
                OnPropertyChanged("CurrentServerStatus.IsRunning");
                OnPropertyChanged("CurrentServerStatus.IsStopping");
                OnPropertyChanged("CurrentServerStatus.IsStopped");
                
                ((RelayCommand)GenerateWorldCommand).RaiseCanExecuteChanged();
            }));
        }

        private void LoadServerIniConfig()
        {
            var iniPresent = _serverIniConfigService.IsServerIniPresent(Configuration.ServerPath);
            IsServerIniPresent = iniPresent;
            if (iniPresent)
            {
                ServerIniConfig = _serverIniConfigService.LoadServerIniConfig(Configuration.ServerPath);
            }
            else
            {
                ServerIniConfig = null;
            }
            OnPropertyChanged(nameof(IsServerIniPresent));
            ((RelayCommand)SaveServerIniCommand).RaiseCanExecuteChanged();
            
            // Vérifier aussi le statut du serveur
            CheckServerStatusAvailability();
            
            // Forcer la mise à jour de l'interface après un délai pour s'assurer que les fichiers sont bien créés
            if (iniPresent)
            {
                _ = Task.Delay(1000).ContinueWith(_ =>
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        CheckServerStatusAvailability();
                        OnPropertyChanged(nameof(IsWorldNameEmpty));
                        ((RelayCommand)GenerateWorldCommand).RaiseCanExecuteChanged();
                    }));
                });
            }
        }

        private void CheckServerStatusAvailability()
        {
            try
            {
                var statusPath = Path.Combine(Configuration.ServerPath, "Moria", "Saved", "Config", "Status.json");
                _logger.LogDebug("Vérification du statut serveur: {Path}", statusPath);
                
                if (_fileService.FileExists(statusPath))
                {
                    var statusContent = File.ReadAllText(statusPath);
                    _logger.LogDebug("Contenu du fichier Status.json: {Content}", statusContent);
                    
                    if (!string.IsNullOrEmpty(statusContent))
                    {
                        // Essayer de désérialiser pour vérifier si WorldName n'est pas vide
                        try
                        {
                            var statusInfo = System.Text.Json.JsonSerializer.Deserialize<ServerStatusInfo>(statusContent);
                            var worldNameEmpty = string.IsNullOrEmpty(statusInfo?.WorldName);
                            
                            // Si le serveur est en cours d'arrêt ou arrêté mais qu'un monde existe, on considère qu'il y a un monde
                            var hasWorld = !worldNameEmpty || 
                                          (statusInfo?.Status == "stopping") ||
                                          (statusInfo?.Status == "not running");
                            
                            IsServerStatusAvailable = hasWorld;
                            IsWorldNameEmpty = !hasWorld;
                            
                            // Mettre à jour CurrentServerStatus pour que l'interface se mette à jour
                            if (statusInfo != null)
                            {
                                CurrentServerStatus = statusInfo;
                                _logger.LogDebug("CurrentServerStatus mis à jour: Status={Status}, WorldName={WorldName}, InviteCode={InviteCode}", 
                                    statusInfo.Status, statusInfo.WorldName, statusInfo.InviteCode);
                            }
                            
                            _logger.LogDebug("Désérialisation réussie - WorldName: '{WorldName}', Status: '{Status}', IsWorldNameEmpty: {IsEmpty}, IsServerStatusAvailable: {IsAvailable}", 
                                statusInfo?.WorldName, statusInfo?.Status, IsWorldNameEmpty, IsServerStatusAvailable);
                        }
                        catch (Exception)
                        {
                            // Si la désérialisation échoue, vérifier manuellement
                            var worldNameEmpty = statusContent.Contains("\"WorldName\":") && 
                                                statusContent.Contains("\"WorldName\": \"\"");
                            var isStopping = statusContent.Contains("\"Status\": \"stopping\"");
                            var isNotRunning = statusContent.Contains("\"Status\": \"not running\"");
                            
                            // Si le serveur est en cours d'arrêt ou arrêté, on considère qu'il y a un monde
                            var hasWorld = !worldNameEmpty || isStopping || isNotRunning;
                            
                            IsServerStatusAvailable = hasWorld;
                            IsWorldNameEmpty = !hasWorld;
                            _logger.LogDebug("Désérialisation échouée, vérification manuelle - IsWorldNameEmpty: {IsEmpty}, IsStopping: {IsStopping}, IsNotRunning: {IsNotRunning}", 
                                IsWorldNameEmpty, isStopping, isNotRunning);
                        }
                    }
                    else
                    {
                        IsServerStatusAvailable = false;
                        IsWorldNameEmpty = true;
                        _logger.LogDebug("Fichier Status.json vide - IsWorldNameEmpty: true");
                    }
                }
                else
                {
                    IsServerStatusAvailable = false;
                    IsWorldNameEmpty = true;
                    _logger.LogDebug("Fichier Status.json non trouvé - IsWorldNameEmpty: true");
                }
                
                _logger.LogDebug("État final - IsWorldNameEmpty: {IsEmpty}, IsServerStatusAvailable: {IsAvailable}", IsWorldNameEmpty, IsServerStatusAvailable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la vérification de la disponibilité du statut serveur");
                IsServerStatusAvailable = false;
                IsWorldNameEmpty = true;
            }
        }

        private async Task StartStatusMonitoring()
        {
            // Surveiller le fichier Status.json toutes les 2 secondes
            while (true)
            {
                try
                {
                    await Task.Delay(2000);
                    
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (IsServerIniPresent)
                        {
                            CheckServerStatusAvailability();
                            OnPropertyChanged(nameof(IsWorldNameEmpty));
                            OnPropertyChanged(nameof(IsServerStatusAvailable));
                            OnPropertyChanged(nameof(CurrentServerStatus));
                            
                            // Forcer la mise à jour des propriétés individuelles de CurrentServerStatus
                            OnPropertyChanged("CurrentServerStatus.Status");
                            OnPropertyChanged("CurrentServerStatus.WorldName");
                            OnPropertyChanged("CurrentServerStatus.InviteCode");
                            OnPropertyChanged("CurrentServerStatus.AdvertisedAddressAndPort");
                            OnPropertyChanged("CurrentServerStatus.Players");
                            OnPropertyChanged("CurrentServerStatus.Version");
                            OnPropertyChanged("CurrentServerStatus.IsRunning");
                            OnPropertyChanged("CurrentServerStatus.IsStopping");
                            OnPropertyChanged("CurrentServerStatus.IsStopped");
                            
                            ((RelayCommand)GenerateWorldCommand).RaiseCanExecuteChanged();
                        }
                    }));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la surveillance du statut");
                }
            }
        }
        private async Task CreateServerIni()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Démarrage du serveur...";

                // Lancer directement le serveur sans créer de fichier de configuration
                var success = await _steamCmdService.StartServerAsync(Configuration.ServerPath, message =>
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        StatusMessage = message;
                    }));
                }, () =>
                {
                    // Callback appelé quand le processus serveur se termine
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _logger.LogInformation("Processus serveur terminé, rechargement de la configuration");
                        LoadServerIniConfig();
                        CheckServerStatusAvailability();
                        // Forcer la mise à jour de l'interface
                        OnPropertyChanged(nameof(IsWorldNameEmpty));
                        ((RelayCommand)CreateServerIniCommand).RaiseCanExecuteChanged();
                        ((RelayCommand)SaveServerIniCommand).RaiseCanExecuteChanged();
                        ((RelayCommand)GenerateWorldCommand).RaiseCanExecuteChanged();
                    }));
                });

                if (success)
                {
                    StatusMessage = "Serveur démarré avec succès";
                    // Démarrer la surveillance du statut
                    _logger.LogInformation("Démarrage de la surveillance du statut après création du serveur");
                    _ = _serverStatusService.StartMonitoringAsync(Configuration.ServerPath);
                }
                else
                {
                    StatusMessage = "Échec du démarrage du serveur";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du démarrage du serveur");
                StatusMessage = "Erreur lors du démarrage du serveur";
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task SaveServerIni()
        {
            if (ServerIniConfig != null)
            {
                _serverIniConfigService.SaveServerIniConfig(Configuration.ServerPath, ServerIniConfig);
                var oldStatus = StatusMessage;
                StatusMessage = "Configuration serveur sauvegardée !";
                await Task.Delay(2000);
                StatusMessage = oldStatus;
            }
        }

        private async Task GenerateWorldAsync()
        {
            _logger.LogInformation("Début de la génération du monde");
            try
            {
                IsBusy = true;
                StatusMessage = "Génération du monde...";

                // Lancer le serveur pour générer le monde
                var success = await _steamCmdService.StartServerAsync(Configuration.ServerPath, message =>
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        StatusMessage = message;
                    }));
                }, () =>
                {
                    // Callback appelé quand le processus serveur se termine
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _logger.LogInformation("Processus serveur terminé, vérification du monde généré");
                        CheckServerStatusAvailability();
                        ((RelayCommand)GenerateWorldCommand).RaiseCanExecuteChanged();
                    }));
                });

                if (success)
                {
                    StatusMessage = "Génération du monde en cours...";
                }
                else
                {
                    StatusMessage = "Échec de la génération du monde";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du monde");
                StatusMessage = "Erreur lors de la génération du monde";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task StartServerAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Démarrage du serveur...";

                var success = await _steamCmdService.StartServerAsync(Configuration.ServerPath, message =>
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        StatusMessage = message;
                    }));
                }, () =>
                {
                    // Callback appelé quand le processus serveur se termine
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _logger.LogInformation("Processus serveur terminé, rechargement de la configuration");
                        LoadServerIniConfig();
                        CheckServerStatusAvailability();
                        // Forcer la mise à jour de l'interface
                        OnPropertyChanged(nameof(IsWorldNameEmpty));
                        ((RelayCommand)CreateServerIniCommand).RaiseCanExecuteChanged();
                        ((RelayCommand)SaveServerIniCommand).RaiseCanExecuteChanged();
                        ((RelayCommand)GenerateWorldCommand).RaiseCanExecuteChanged();
                    }));
                });

                if (success)
                {
                    StatusMessage = "Serveur démarré avec succès";
                    // Démarrer la surveillance du statut
                    _logger.LogInformation("Démarrage de la surveillance du statut après démarrage du serveur");
                    _ = _serverStatusService.StartMonitoringAsync(Configuration.ServerPath);
                }
                else
                {
                    StatusMessage = "Échec du démarrage du serveur";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du démarrage du serveur");
                StatusMessage = "Erreur lors du démarrage du serveur";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task StopServerAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Arrêt du serveur...";

                var success = await _steamCmdService.StopServerAsync(Configuration.ServerPath, message =>
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        StatusMessage = message;
                    }));
                });

                if (success)
                {
                    StatusMessage = "Serveur arrêté avec succès";
                    // Arrêter la surveillance du statut
                    await _serverStatusService.StopMonitoringAsync();
                }
                else
                {
                    StatusMessage = "Échec de l'arrêt du serveur";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'arrêt du serveur");
                StatusMessage = "Erreur lors de l'arrêt du serveur";
            }
            finally
            {
                IsBusy = false;
            }
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

    /// <summary>
    /// Commande simple pour les actions asynchrones
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;

        public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        public async void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
} 