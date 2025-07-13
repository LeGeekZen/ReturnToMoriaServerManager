/*
    Fichier : BoolToColorConverter.cs
    Emplacement : ReturnToMoriaServerManager/Converters/BoolToColorConverter.cs
    Auteur : Le Geek Zen
    Description : Convertisseur pour transformer un booléen en couleur WPF
*/

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ReturnToMoriaServerManager.Converters
{
    /// <summary>
    /// Convertisseur qui transforme une valeur booléenne en couleur pour l'interface utilisateur.
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en couleur.
        /// </summary>
        /// <param name="value">Valeur booléenne à convertir</param>
        /// <param name="targetType">Type cible (Brush)</param>
        /// <param name="parameter">Paramètre optionnel pour personnaliser le comportement</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>Brush coloré selon la valeur booléenne</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // Si un paramètre est fourni, utiliser pour la navigation
                if (parameter != null && parameter.ToString() == "Navigation")
                {
                    return boolValue ? new SolidColorBrush(Color.FromRgb(52, 152, 219)) : new SolidColorBrush(Color.FromRgb(45, 45, 45));
                }
                
                // Comportement par défaut pour les statuts d'installation
                return boolValue ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }
            return new SolidColorBrush(Colors.Gray);
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