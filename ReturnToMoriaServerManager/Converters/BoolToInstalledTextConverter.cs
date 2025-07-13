/*
    Fichier : BoolToInstalledTextConverter.cs
    Emplacement : ReturnToMoriaServerManager/Converters/BoolToInstalledTextConverter.cs
    Auteur : Le Geek Zen
    Description : Convertisseur pour convertir un booléen en texte d'état d'installation
*/

using System;
using System.Globalization;
using System.Windows.Data;

namespace ReturnToMoriaServerManager.Converters
{
    /// <summary>
    /// Convertisseur pour convertir un booléen en texte d'installation
    /// </summary>
    public class BoolToInstalledTextConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en texte d'état d'installation.
        /// </summary>
        /// <param name="value">Valeur booléenne à convertir</param>
        /// <param name="targetType">Type cible (string)</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé)</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>Texte indiquant l'état d'installation</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isInstalled)
            {
                return isInstalled ? "✓ Installé" : "✗ Non installé";
            }

            return "✗ Non installé";
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