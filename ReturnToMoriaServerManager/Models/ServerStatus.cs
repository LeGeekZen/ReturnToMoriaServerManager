/*
    Fichier : ServerStatus.cs
    Emplacement : ReturnToMoriaServerManager/Models/ServerStatus.cs
    Auteur : Le Geek Zen
    Description : Énumération pour représenter l'état du serveur Return to Moria
*/

namespace ReturnToMoriaServerManager.Models
{
    /// <summary>
    /// Énumération représentant les différents états possibles du serveur.
    /// </summary>
    public enum ServerStatus
    {
        /// <summary>
        /// État inconnu du serveur.
        /// </summary>
        Unknown,
        /// <summary>
        /// Le serveur est en cours d'exécution.
        /// </summary>
        Running,
        /// <summary>
        /// Le serveur est arrêté.
        /// </summary>
        Stopped,
        /// <summary>
        /// Le serveur est en cours de démarrage.
        /// </summary>
        Starting,
        /// <summary>
        /// Le serveur est en cours d'arrêt.
        /// </summary>
        Stopping,
        /// <summary>
        /// Le serveur est en erreur.
        /// </summary>
        Error
    }
} 