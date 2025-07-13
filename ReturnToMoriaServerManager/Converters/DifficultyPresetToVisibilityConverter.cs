/*
    Fichier : DifficultyPresetToVisibilityConverter.cs
    Emplacement : ReturnToMoriaServerManager/Converters/DifficultyPresetToVisibilityConverter.cs
    Auteur : Le Geek Zen
    Description : Convertisseur pour afficher/masquer la section de difficulté personnalisée selon le préréglage
*/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Microsoft.Extensions.Logging;

namespace ReturnToMoriaServerManager.Converters
{
    /// <summary>
    /// Convertisseur qui affiche ou masque la section de difficulté personnalisée selon le préréglage sélectionné.
    /// </summary>
    public class DifficultyPresetToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un préréglage de difficulté en visibilité pour la section personnalisée.
        /// </summary>
        /// <param name="value">Préréglage de difficulté (string)</param>
        /// <param name="targetType">Type cible (Visibility)</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé)</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>Visibility.Visible si "Personnalisé", Visibility.Collapsed sinon</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Log pour diagnostiquer
                System.Diagnostics.Debug.WriteLine($"DifficultyPresetToVisibilityConverter: value = {value}, type = {value?.GetType().Name}");
                
                if (value is string difficultyPreset)
                {
                    var isCustom = difficultyPreset.Equals("Personnalisé", StringComparison.OrdinalIgnoreCase);
                    System.Diagnostics.Debug.WriteLine($"DifficultyPresetToVisibilityConverter: isCustom = {isCustom}");
                    return isCustom ? Visibility.Visible : Visibility.Collapsed;
                }
                
                System.Diagnostics.Debug.WriteLine($"DifficultyPresetToVisibilityConverter: value is not string, returning Collapsed");
                return Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DifficultyPresetToVisibilityConverter error: {ex.Message}");
                return Visibility.Collapsed;
            }
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