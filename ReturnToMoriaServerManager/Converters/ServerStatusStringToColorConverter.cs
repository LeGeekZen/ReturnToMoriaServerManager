/*
    Fichier : ServerStatusStringToColorConverter.cs
    Emplacement : ReturnToMoriaServerManager/Converters/ServerStatusStringToColorConverter.cs
    Auteur : Le Geek Zen
    Description : Convertisseur pour colorer le texte du statut serveur selon la valeur
*/

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReturnToMoriaServerManager.Converters
{
    /// <summary>
    /// Convertisseur qui transforme une chaîne de statut du serveur en couleur pour l'interface utilisateur.
    /// </summary>
    public class ServerStatusStringToColorConverter : IValueConverter
    {
        /// <summary>
        /// Convertit une chaîne de statut du serveur en couleur.
        /// </summary>
        /// <param name="value">Chaîne de statut du serveur</param>
        /// <param name="targetType">Type cible (Brush)</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé)</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>Brush coloré selon le statut du serveur</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLowerInvariant() switch
                {
                    "running" => new SolidColorBrush(Colors.LightGreen),
                    "stopping" or "preparing" => new SolidColorBrush(Colors.Orange),
                    "not running" or "stopped" => new SolidColorBrush(Colors.LightCoral),
                    _ => new SolidColorBrush(Colors.LightCoral)
                };
            }
            return new SolidColorBrush(Colors.LightCoral);
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