/*
    Fichier : MainWindow.xaml.cs
    Emplacement : ReturnToMoriaServerManager/Views/MainWindow.xaml.cs
    Auteur : Le Geek Zen
    Description : Code-behind de la fenêtre principale avec navigation
*/

using System.Windows;
using ReturnToMoriaServerManager.ViewModels;

namespace ReturnToMoriaServerManager.Views
{
    /// <summary>
    /// Fenêtre principale de l'application avec système de navigation entre les pages.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initialise une nouvelle instance de la fenêtre principale.
        /// </summary>
        /// <param name="navigationViewModel">ViewModel de navigation pour gérer les pages</param>
        public MainWindow(NavigationViewModel navigationViewModel)
        {
            InitializeComponent();
            DataContext = navigationViewModel;
            
            // Gérer la taille de la fenêtre en fonction de l'écran disponible
            AdjustWindowSize();
            
            // S'assurer que la fenêtre reste toujours accessible
            this.Loaded += MainWindow_Loaded;
            
            // Gérer les changements de taille d'écran
            this.SizeChanged += MainWindow_SizeChanged;
        }
        
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Vérifier et ajuster la position de la fenêtre si nécessaire
            EnsureWindowIsVisible();
        }
        
        private void AdjustWindowSize()
        {
            // Obtenir les dimensions de l'écran de travail (sans la barre des tâches)
            var workArea = SystemParameters.WorkArea;
            
            // Définir la largeur souhaitée
            this.Width = 1000;
            
            // Calculer la hauteur maximale disponible (en laissant de l'espace pour la barre des tâches)
            double maxHeight = workArea.Height - 50; // 50 pixels de marge
            
            // Si la hauteur calculée est inférieure à la hauteur minimale, utiliser la hauteur minimale
            if (maxHeight < 600)
            {
                maxHeight = 600;
            }
            
            // Définir la hauteur maximale
            this.MaxHeight = maxHeight;
            
            // Si la hauteur actuelle dépasse la hauteur maximale, la réduire
            if (this.Height > maxHeight)
            {
                this.Height = maxHeight;
            }
        }
        
        private void EnsureWindowIsVisible()
        {
            // Obtenir les dimensions de l'écran de travail
            var workArea = SystemParameters.WorkArea;
            
            // Vérifier si la fenêtre est complètement visible
            if (this.Left + this.Width > workArea.Width)
            {
                this.Left = workArea.Width - this.Width;
            }
            
            if (this.Top + this.Height > workArea.Height)
            {
                this.Top = workArea.Height - this.Height;
            }
            
            // S'assurer que la fenêtre n'est pas en dehors de l'écran à gauche ou en haut
            if (this.Left < workArea.Left)
            {
                this.Left = workArea.Left;
            }
            
            if (this.Top < workArea.Top)
            {
                this.Top = workArea.Top;
            }
        }
        
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Ajuster la taille de la fenêtre si nécessaire lors du redimensionnement
            AdjustWindowSize();
            EnsureWindowIsVisible();
        }
    }
} 