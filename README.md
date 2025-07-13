# Return to Moria Server Manager

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/your-username/ReturnToMoria_DedicatedServer/actions)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License](https://img.shields.io/badge/License-GPLv3-green)](https://www.gnu.org/licenses/gpl-3.0)
[![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey)](https://www.microsoft.com/windows)

Un gestionnaire de serveur dédié moderne et intuitif pour Return to Moria, développé en C# avec WPF.

[![Return to Moria Server Manager](https://img.shields.io/badge/Download-Latest%20Release-blue)](https://github.com/your-username/ReturnToMoria_DedicatedServer/releases/latest)

## Fonctionnalités

### 🚀 Installation et Configuration
- **Installation automatique de SteamCMD** : Télécharge et installe SteamCMD automatiquement
- **Installation du serveur dédié** : Installe le serveur Return to Moria via Steam
- **Configuration de base** : Interface simple pour configurer le nom du monde, mot de passe et port

### ⚙️ Configuration Avancée du Serveur
- **Configuration complète** : Interface graphique pour modifier tous les paramètres du fichier `MoriaServerConfig.ini`
- **Paramètres de difficulté** : Configuration des préréglages et paramètres personnalisés
- **Configuration réseau** : Gestion des adresses IP, ports et paramètres de connexion
- **Paramètres de performance** : Configuration FPS et limite de zones chargées
- **Sauvegarde/Chargement** : Sauvegarde et chargement automatique des configurations

### 📊 Surveillance en Temps Réel
- **Statut du serveur** : Surveillance en temps réel du statut du serveur
- **Informations détaillées** : Version, code d'invitation, adresse IP, nombre de joueurs
- **Gestion des processus** : Démarrage et arrêt du serveur

### 🎮 Gestion du Monde
- **Création de monde** : Génération de nouveaux mondes avec paramètres personnalisables
- **Configuration du monde** : Nom, mot de passe, type (Campagne/Bac à sable)
- **Paramètres de difficulté** : 5 niveaux de difficulté prédéfinis + mode personnalisé

## Configuration Avancée

La page de configuration avancée permet de modifier tous les paramètres du serveur Return to Moria :

### Sections Disponibles

#### Configuration Principale
- **Mot de passe optionnel** : Protection du serveur par mot de passe

#### Configuration du Monde
- **Nom du monde** : Nom affiché aux joueurs
- **Nom de fichier optionnel** : Pour les mondes avec le même nom

#### Création du Monde
- **Type de monde** : Campagne ou Bac à sable
- **Seed** : Génération aléatoire ou seed spécifique
- **Préréglage de difficulté** : Histoire, Solo, Normal, Difficile, Personnalisé

#### Difficulté Personnalisée
- **Difficulté de combat** : Dégâts et points de vie des ennemis
- **Agressivité des ennemis** : Fréquence et nombre d'attaques
- **Difficulté de survie** : Buffs, désespoir, décroissance des stats
- **Drops de minage** : Volume de minerai par veine
- **Drops du monde** : Récompenses pour vaincre les ennemis
- **Fréquence des hordes** : Déclenchement par actions bruyantes
- **Fréquence des sièges** : Attaques de base des nains
- **Fréquence des patrouilles** : Apparition des groupes d'ennemis

#### Configuration Réseau
- **Adresse d'écoute** : IP locale du serveur
- **Port d'écoute** : Port pour les connexions entrantes
- **Adresse de publication** : IP annoncée aux clients
- **Port de publication** : Port annoncé aux clients
- **Temps de retry** : Paramètres de reconnexion

#### Console
- **Console activée** : Affichage de la fenêtre console

#### Performance
- **FPS du serveur** : Limite de FPS (recommandé 60)
- **Limite de zones chargées** : Nombre de zones simultanées (4-32)

## Installation

1. **Prérequis** : .NET 8.0 ou supérieur
2. **Compilation** : Ouvrir le projet dans Visual Studio et compiler
3. **Exécution** : Lancer l'application

## Utilisation

1. **Installation** : Utiliser l'interface pour installer SteamCMD et le serveur
2. **Configuration de base** : Configurer le nom du monde et les paramètres essentiels
3. **Configuration avancée** : Cliquer sur "Configuration Avancée" pour accéder à tous les paramètres
4. **Démarrage** : Lancer le serveur et surveiller son statut

## Structure du Projet

```
ReturnToMoriaServerManager/
├── Models/                 # Modèles de données
├── Services/              # Services métier
├── ViewModels/            # ViewModels MVVM
├── Views/                 # Vues WPF
├── Converters/            # Convertisseurs WPF
└── App.xaml.cs           # Point d'entrée et DI
```

## Technologies Utilisées

- **.NET 8.0** : Framework de développement
- **WPF** : Interface utilisateur
- **MVVM** : Pattern d'architecture
- **Dependency Injection** : Gestion des dépendances
- **Microsoft.Extensions.Logging** : Logging
- **Microsoft.Extensions.Http** : Client HTTP

## Licence

GPLv3 - Voir le fichier LICENSE pour plus de détails.

## 🤝 Contribution

Nous accueillons les contributions ! Consultez notre [Guide de Contribution](CONTRIBUTING.md) pour commencer.

### Comment Contribuer

1. **Fork** le projet
2. **Créez** une branche pour votre fonctionnalité (`git checkout -b feature/AmazingFeature`)
3. **Commitez** vos changements (`git commit -m 'Add some AmazingFeature'`)
4. **Poussez** vers la branche (`git push origin feature/AmazingFeature`)
5. **Ouvrez** une Pull Request

### Signaler un Bug

Si vous trouvez un bug, veuillez [créer une issue](https://github.com/your-username/ReturnToMoria_DedicatedServer/issues/new) avec le template de bug.

### Demander une Fonctionnalité

Si vous avez une idée de fonctionnalité, [créez une issue](https://github.com/your-username/ReturnToMoria_DedicatedServer/issues/new) avec le template de fonctionnalité.

## 📞 Support

Pour toute question ou problème :
- 📖 Consultez la [documentation officielle de Return to Moria](https://www.returntomoria.com/)
- 🐛 [Créez une issue](https://github.com/your-username/ReturnToMoria_DedicatedServer/issues) sur GitHub
- 💬 Rejoignez notre communauté Discord (lien à venir)

## 📊 Statistiques du Projet

![GitHub stars](https://img.shields.io/github/stars/your-username/ReturnToMoria_DedicatedServer?style=social)
![GitHub forks](https://img.shields.io/github/forks/your-username/ReturnToMoria_DedicatedServer?style=social)
![GitHub issues](https://img.shields.io/github/issues/your-username/ReturnToMoria_DedicatedServer)
![GitHub pull requests](https://img.shields.io/github/issues-pr/your-username/ReturnToMoria_DedicatedServer) 