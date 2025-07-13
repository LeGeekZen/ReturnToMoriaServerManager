/*
    Fichier : BoolToVisibilityConverter.cs
    Emplacement : ReturnToMoriaServerManager/Converters/BoolToVisibilityConverter.cs
    Auteur : Le Geek Zen
    Description : Convertisseur pour convertir un booléen en visibilité WPF
*/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReturnToMoriaServerManager.Converters
{
    /// <summary>
    /// Convertisseur pour convertir un booléen en visibilité
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en valeur de visibilité WPF.
        /// </summary>
        /// <param name="value">Valeur booléenne à convertir</param>
        /// <param name="targetType">Type cible (Visibility)</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé)</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>Visibility.Visible si true, Visibility.Collapsed sinon</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        /// <summary>
        /// Convertit une valeur de visibilité WPF en booléen.
        /// </summary>
        /// <param name="value">Valeur de visibilité à convertir</param>
        /// <param name="targetType">Type cible (bool)</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé)</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>True si Visible, false sinon</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }

            return false;
        }
    }
} 