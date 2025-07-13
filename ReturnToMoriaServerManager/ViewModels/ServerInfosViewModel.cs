/*
    Fichier : ServerInfosViewModel.cs
    Emplacement : ReturnToMoriaServerManager/ViewModels/ServerInfosViewModel.cs
    Auteur : Le Geek Zen
    Description : ViewModel pour la page d'informations du serveur Return to Moria
*/

using Microsoft.Extensions.Logging;
using ReturnToMoriaServerManager.Services;

namespace ReturnToMoriaServerManager.ViewModels
{
    public class ServerInfosViewModel : MainViewModel
    {
        public ServerInfosViewModel(
            ILogger<MainViewModel> logger,
            ISteamCmdService steamCmdService,
            IServerManagerService serverManagerService,
            IFileService fileService,
            IConfigurationService configurationService) 
            : base(logger, steamCmdService, serverManagerService, fileService, configurationService)
        {
        }
    }
} 