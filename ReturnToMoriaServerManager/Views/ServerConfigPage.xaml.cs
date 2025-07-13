/*
    Fichier : ServerConfigPage.xaml.cs
    Emplacement : ReturnToMoriaServerManager/Views/ServerConfigPage.xaml.cs
    Auteur : Le Geek Zen
    Description : Code-behind de la page de configuration avancée du serveur
*/

using System.Windows.Controls;
using ReturnToMoriaServerManager.ViewModels;

namespace ReturnToMoriaServerManager.Views
{
    /// <summary>
    /// Page de configuration avancée du serveur Return to Moria.
    /// </summary>
    public partial class ServerConfigPage : Page
    {
        /// <summary>
        /// Initialise une nouvelle instance de la page de configuration.
        /// </summary>
        /// <param name="viewModel">ViewModel de configuration du serveur</param>
        public ServerConfigPage(ServerConfigViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
} 