/*
    Fichier : IServerManagerService.cs
    Emplacement : ReturnToMoriaServerManager/Services/IServerManagerService.cs
    Auteur : Le Geek Zen
    Description : Interface pour la gestion du cycle de vie du serveur Return to Moria
*/

using System;
using System.Threading.Tasks;
using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public interface IServerManagerService
    {
        /// <summary>
        /// Événement déclenché lors d'un changement de statut du serveur.
        /// </summary>
        event EventHandler<ServerStatus> ServerStatusChanged;
        /// <summary>
        /// Événement déclenché lorsqu'un message est reçu du serveur.
        /// </summary>
        event EventHandler<string> ServerMessageReceived;
        /// <summary>
        /// Initialise le service avec la configuration du serveur.
        /// </summary>
        void Initialize(ServerConfiguration configuration);
        /// <summary>
        /// Met à jour la configuration du serveur.
        /// </summary>
        void UpdateConfiguration(ServerConfiguration configuration);
        /// <summary>
        /// Vérifie si le serveur est installé.
        /// </summary>
        bool IsServerInstalled();
        /// <summary>
        /// Vérifie de façon asynchrone le statut du serveur.
        /// </summary>
        Task<ServerStatus> CheckServerStatusAsync();
    }
} 