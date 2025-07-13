/*
    Fichier : ServerInfosPage.xaml.cs
    Emplacement : ReturnToMoriaServerManager/Views/ServerInfosPage.xaml.cs
    Auteur : Le Geek Zen
    Description : Code-behind de la page d'informations du serveur Return to Moria
*/

using System.Windows.Controls;
using ReturnToMoriaServerManager.ViewModels;

namespace ReturnToMoriaServerManager.Views
{
    /// <summary>
    /// Page d'informations et de gestion du serveur Return to Moria.
    /// </summary>
    public partial class ServerInfosPage : Page
    {
        /// <summary>
        /// Initialise une nouvelle instance de la page d'informations.
        /// </summary>
        /// <param name="viewModel">ViewModel d'informations du serveur</param>
        public ServerInfosPage(ServerInfosViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
} 