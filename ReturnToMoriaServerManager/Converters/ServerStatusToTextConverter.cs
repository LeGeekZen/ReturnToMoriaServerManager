/*
    Fichier : ServerStatusToTextConverter.cs
    Emplacement : ReturnToMoriaServerManager/Converters/ServerStatusToTextConverter.cs
    Auteur : Le Geek Zen
    Description : Convertisseur pour convertir le statut du serveur en texte descriptif
*/

using ReturnToMoriaServerManager.Models;
using System.Globalization;
using System.Windows.Data;
using System;

namespace ReturnToMoriaServerManager.Converters
{
    /// <summary>
    /// Convertisseur pour convertir le statut du serveur en texte
    /// </summary>
    public class ServerStatusToTextConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un statut de serveur en texte descriptif en français.
        /// </summary>
        /// <param name="value">Statut du serveur (ServerStatus)</param>
        /// <param name="targetType">Type cible (string)</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé)</param>
        /// <param name="culture">Culture pour la conversion</param>
        /// <returns>Texte descriptif du statut du serveur</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ServerStatus status)
            {
                return status switch
                {
                    ServerStatus.Error => "Erreur - Serveur non détecté",
                    ServerStatus.Stopped => "Arrêté",
                    ServerStatus.Starting => "Démarrage en cours...",
                    ServerStatus.Running => "Actif et fonctionnel",
                    ServerStatus.Stopping => "Arrêt en cours...",
                    _ => "Statut inconnu"
                };
            }

            return "Statut inconnu";
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