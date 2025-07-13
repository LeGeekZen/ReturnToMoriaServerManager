/*
    Fichier : App.xaml.cs
    Emplacement : ReturnToMoriaServerManager/App.xaml.cs
    Auteur : Le Geek Zen
    Description : Code-behind de l'application avec configuration des services et injection de dépendances
*/

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;
using ReturnToMoriaServerManager.Services;
using ReturnToMoriaServerManager.ViewModels;
using System.Windows;
using ReturnToMoriaServerManager.Views;
using System;

namespace ReturnToMoriaServerManager
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        public static ServiceProvider? Services => ((App)Current)._serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Gestion globale des exceptions non gérées
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                MessageBox.Show($"Erreur critique : {ex?.Message}\n\n{ex?.StackTrace}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show($"Erreur non gérée : {args.Exception.Message}\n\n{args.Exception.StackTrace}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };

            // Configuration des services
            var services = new ServiceCollection();
            ConfigureServices(services);
            
            _serviceProvider = services.BuildServiceProvider();
            
            // Création de la fenêtre principale avec injection de dépendances
            var navigationViewModel = _serviceProvider.GetRequiredService<NavigationViewModel>();
            var mainWindow = new MainWindow(navigationViewModel);
            mainWindow.Show();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Configuration du logging
            services.AddLogging(builder =>
            {
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Services
            services.AddSingleton<ISteamCmdService, SteamCmdService>();
            services.AddSingleton<IServerManagerService, ServerManagerService>();
            services.AddHttpClient<IFileService, FileService>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<IServerIniConfigService, ServerIniConfigService>();
            services.AddSingleton<IServerStatusService, ServerStatusService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ServerInfosViewModel>();
            services.AddTransient<ServerConfigViewModel>();
            services.AddTransient<NavigationViewModel>();

            // Vues
            services.AddTransient<MainWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
} 