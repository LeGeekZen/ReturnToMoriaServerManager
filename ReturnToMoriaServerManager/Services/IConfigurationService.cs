/*
    Fichier : IConfigurationService.cs
    Emplacement : ReturnToMoriaServerManager/Services/IConfigurationService.cs
    Auteur : Le Geek Zen
    Description : Interface pour la gestion de la configuration de l'application
*/

using ReturnToMoriaServerManager.Models;

namespace ReturnToMoriaServerManager.Services
{
    public interface IConfigurationService
    {
        ServerConfiguration LoadConfiguration();
        void SaveConfiguration(ServerConfiguration configuration);
    }
} 