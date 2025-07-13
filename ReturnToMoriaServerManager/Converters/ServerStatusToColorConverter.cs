/*
    Fichier : ServerStatusToColorConverter.cs
    Emplacement : ReturnToMoriaServerManager/Converters/ServerStatusToColorConverter.cs
    Auteur : Le Geek Zen
    Description : Convertisseur pour convertir le statut du serveur en couleur d'indicateur
*/

using ReturnToMoriaServerManager.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System;

namespace ReturnToMoriaServerManager.Converters
{
    /// <summary>
    /// Convertisseur pour convertir le statut du serveur en couleur
    /// </summary>
    public class ServerStatusToColorConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un statut de serveur en couleur d'indicateur.
        /// </summary>
        /// <param name="value">Statut du serveur (ServerStatus)</param>
        /// <param name="targetType">Type cible (Color)</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé)</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>Couleur correspondant au statut du serveur</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ServerStatus status)
            {
                return status switch
                {
                    ServerStatus.Error => Colors.Red,
                    ServerStatus.Stopped => Colors.Yellow,
                    ServerStatus.Starting => Colors.Orange,
                    ServerStatus.Running => Colors.Green,
                    ServerStatus.Stopping => Colors.Orange,
                    _ => Colors.Gray
                };
            }

            return Colors.Gray;
        }

        /// <summary>
        /// Conversion inverse (non implémentée).
        /// </summary>
        /// <param name="value">Valeur à convertir</param>
        /// <param name="targetType">Type cible</param>
        /// <param name="parameter">Paramètre optionnel</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>Exception car non implémentée</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 