/*
    Fichier : IServerStatusService.cs
    Emplacement : ReturnToMoriaServerManager/Services/IServerStatusService.cs
    Auteur : Le Geek Zen
    Description : Interface pour le service de surveillance et de récupération du statut du serveur
*/

using System;
using System.Threading.Tasks;
using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public interface IServerStatusService
    {
        /// <summary>
        /// Événement déclenché lors d'un changement de statut du serveur.
        /// </summary>
        event EventHandler<ServerStatusInfo> StatusChanged;
        /// <summary>
        /// Statut courant du serveur.
        /// </summary>
        ServerStatusInfo CurrentStatus { get; }
        /// <summary>
        /// Indique si la surveillance du serveur est active.
        /// </summary>
        bool IsMonitoring { get; }
        /// <summary>
        /// Démarre la surveillance du statut du serveur.
        /// </summary>
        Task StartMonitoringAsync(string serverPath);
        /// <summary>
        /// Arrête la surveillance du statut du serveur.
        /// </summary>
        Task StopMonitoringAsync();
        /// <summary>
        /// Récupère le statut du serveur de façon asynchrone.
        /// </summary>
        Task<ServerStatusInfo> GetStatusAsync(string serverPath);
    }
} 