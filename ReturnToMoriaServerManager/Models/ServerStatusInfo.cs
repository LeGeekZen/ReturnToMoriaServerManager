/*
    Fichier : ServerStatusInfo.cs
    Emplacement : ReturnToMoriaServerManager/Models/ServerStatusInfo.cs
    Auteur : Le Geek Zen
    Description : Modèle pour représenter les informations de statut du serveur Return to Moria
*/

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReturnToMoriaServerManager.Models
{
    /// <summary>
    /// Informations détaillées sur le statut du serveur Return to Moria.
    /// </summary>
    public class ServerStatusInfo : INotifyPropertyChanged
    {
        private string _status = string.Empty;
        private string _inviteCode = string.Empty;
        private string _advertisedAddressAndPort = string.Empty;
        private string _worldName = string.Empty;
        private int _worldSeed;
        private string _players = string.Empty;
        private string _version = string.Empty;

        /// <summary>
        /// Statut actuel du serveur (running, stopped, etc.).
        /// </summary>
        public string Status 
        { 
            get => _status; 
            set => SetProperty(ref _status, value); 
        }
        
        /// <summary>
        /// Code d'invitation pour rejoindre le serveur.
        /// </summary>
        public string InviteCode 
        { 
            get => _inviteCode; 
            set => SetProperty(ref _inviteCode, value); 
        }
        
        /// <summary>
        /// Adresse et port publiés du serveur.
        /// </summary>
        public string AdvertisedAddressAndPort 
        { 
            get => _advertisedAddressAndPort; 
            set => SetProperty(ref _advertisedAddressAndPort, value); 
        }
        
        /// <summary>
        /// Nom du monde actuel du serveur.
        /// </summary>
        public string WorldName 
        { 
            get => _worldName; 
            set => SetProperty(ref _worldName, value); 
        }
        
        /// <summary>
        /// Graine du monde actuel.
        /// </summary>
        public int WorldSeed 
        { 
            get => _worldSeed; 
            set => SetProperty(ref _worldSeed, value); 
        }
        
        /// <summary>
        /// Informations sur les joueurs connectés.
        /// </summary>
        public string Players 
        { 
            get => _players; 
            set => SetProperty(ref _players, value); 
        }
        
        /// <summary>
        /// Version du serveur.
        /// </summary>
        public string Version 
        { 
            get => _version; 
            set => SetProperty(ref _version, value); 
        }

        /// <summary>
        /// Indique si le serveur est en cours d'exécution.
        /// </summary>
        public bool IsRunning => Status.Equals("running", StringComparison.OrdinalIgnoreCase);
        
        /// <summary>
        /// Indique si le serveur est en cours d'arrêt.
        /// </summary>
        public bool IsStopping => Status.Equals("stopping", StringComparison.OrdinalIgnoreCase);
        
        /// <summary>
        /// Indique si le serveur est arrêté.
        /// </summary>
        public bool IsStopped => Status.Equals("stopped", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(Status);

        /// <summary>
        /// Événement déclenché lors du changement d'une propriété.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Déclenche l'événement PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Met à jour une propriété et déclenche l'événement PropertyChanged si la valeur a changé.
        /// </summary>
        /// <typeparam name="T">Type de la propriété</typeparam>
        /// <param name="field">Champ privé de la propriété</param>
        /// <param name="value">Nouvelle valeur</param>
        /// <param name="propertyName">Nom de la propriété</param>
        /// <returns>True si la valeur a changé</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
} 