/*
    Fichier : NavigationViewModel.cs
    Emplacement : ReturnToMoriaServerManager/ViewModels/NavigationViewModel.cs
    Auteur : Le Geek Zen
    Description : ViewModel pour la navigation entre les pages de l'application
*/

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using ReturnToMoriaServerManager.Views;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ReturnToMoriaServerManager.ViewModels;

namespace ReturnToMoriaServerManager.ViewModels
{
    public class NavigationViewModel : INotifyPropertyChanged
    {
        private readonly ILogger<NavigationViewModel> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Page _currentPage = null!;
        private bool _isServerInfosSelected = true;
        private bool _isServerConfigSelected;

        public NavigationViewModel(ILogger<NavigationViewModel> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            
            _logger.LogInformation("Initialisation du NavigationViewModel");
            
            // Créer la page initiale
            try
            {
                var serverInfosViewModel = _serviceProvider.GetRequiredService<ServerInfosViewModel>();
                _currentPage = new ServerInfosPage(serverInfosViewModel);
                _logger.LogInformation("Page d'informations du serveur créée avec succès");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la page d'informations");
            }

            // Commandes de navigation
            NavigateToServerInfosCommand = new SimpleRelayCommand(NavigateToServerInfos);
            NavigateToServerConfigCommand = new SimpleRelayCommand(NavigateToServerConfig);
            
            _logger.LogInformation("Commandes de navigation créées");
        }

        public Page CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public bool IsServerInfosSelected
        {
            get => _isServerInfosSelected;
            set => SetProperty(ref _isServerInfosSelected, value);
        }

        public bool IsServerConfigSelected
        {
            get => _isServerConfigSelected;
            set => SetProperty(ref _isServerConfigSelected, value);
        }

        public ICommand NavigateToServerInfosCommand { get; }
        public ICommand NavigateToServerConfigCommand { get; }

        private void NavigateToServerInfos()
        {
            _logger.LogInformation("Tentative de navigation vers la page d'informations");
            try
            {
                var serverInfosViewModel = _serviceProvider.GetRequiredService<ServerInfosViewModel>();
                CurrentPage = new ServerInfosPage(serverInfosViewModel);
                IsServerInfosSelected = true;
                IsServerConfigSelected = false;
                _logger.LogInformation("Navigation vers la page d'informations du serveur réussie");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la navigation vers la page d'informations");
            }
        }

        private void NavigateToServerConfig()
        {
            _logger.LogInformation("Tentative de navigation vers la page de configuration");
            try
            {
                var serverConfigViewModel = _serviceProvider.GetRequiredService<ServerConfigViewModel>();
                _logger.LogInformation("ServerConfigViewModel récupéré avec succès");
                
                var serverConfigPage = new ServerConfigPage(serverConfigViewModel);
                _logger.LogInformation("ServerConfigPage créée avec succès");
                
                CurrentPage = serverConfigPage;
                IsServerInfosSelected = false;
                IsServerConfigSelected = true;
                _logger.LogInformation("Navigation vers la page de configuration avancée réussie");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la navigation vers la page de configuration");
            }
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
    /// Commande simple pour les actions synchrones
    /// </summary>
    public class SimpleRelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public SimpleRelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                _execute();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
} 