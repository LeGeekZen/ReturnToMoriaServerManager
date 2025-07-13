/*
    Fichier : BoolToVisibilityInverseConverter.cs
    Emplacement : ReturnToMoriaServerManager/Converters/BoolToVisibilityInverseConverter.cs
    Auteur : Le Geek Zen
    Description : Convertisseur pour inverser un booléen en visibilité WPF
*/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReturnToMoriaServerManager.Converters
{
    /// <summary>
    /// Convertisseur pour inverser un booléen en visibilité
    /// </summary>
    public class BoolToVisibilityInverseConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en valeur de visibilité WPF avec logique inversée.
        /// </summary>
        /// <param name="value">Valeur booléenne à convertir</param>
        /// <param name="targetType">Type cible (Visibility)</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé)</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>Visibility.Collapsed si true, Visibility.Visible sinon</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        /// <summary>
        /// Convertit une valeur de visibilité WPF en booléen avec logique inversée.
        /// </summary>
        /// <param name="value">Valeur de visibilité à convertir</param>
        /// <param name="targetType">Type cible (bool)</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé)</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>True si pas Visible, false sinon</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            return true;
        }
    }
} 