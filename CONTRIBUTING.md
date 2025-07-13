# Guide de Contribution

Merci de votre intérêt pour contribuer au projet Return to Moria Server Manager ! Ce document vous guidera dans le processus de contribution.

## 🚀 Comment Contribuer

### 1. Fork et Clone

1. Fork ce repository sur GitHub
2. Clone votre fork localement :
   ```bash
   git clone https://github.com/votre-username/ReturnToMoria_DedicatedServer.git
   cd ReturnToMoria_DedicatedServer
   ```

### 2. Configuration de l'Environnement

#### Prérequis
- Visual Studio 2022 ou Visual Studio Code
- .NET 8.0 SDK
- Windows 10/11 (pour le développement WPF)

#### Configuration
1. Ouvrez la solution `ReturnToMoriaServerManager.sln` dans Visual Studio
2. Restaurez les packages NuGet
3. Compilez le projet

### 3. Workflow de Développement

#### Branches
- `main` : Code stable et production
- `develop` : Branche de développement principale
- `feature/nom-de-la-fonctionnalite` : Nouvelles fonctionnalités
- `bugfix/description-du-bug` : Corrections de bugs
- `hotfix/description-urgente` : Corrections urgentes

#### Processus de Contribution

1. **Créer une branche** :
   ```bash
   git checkout -b feature/nouvelle-fonctionnalite
   ```

2. **Développer** :
   - Suivez les conventions de code (voir ci-dessous)
   - Testez vos modifications
   - Committez régulièrement avec des messages clairs

3. **Tester** :
   - Assurez-vous que le projet compile sans erreurs
   - Testez les fonctionnalités modifiées
   - Vérifiez que les tests existants passent

4. **Pousser** :
   ```bash
   git push origin feature/nouvelle-fonctionnalite
   ```

5. **Créer une Pull Request** :
   - Allez sur GitHub et créez une PR vers `develop`
   - Remplissez le template de PR
   - Attendez la review

## 📝 Conventions de Code

### C# et WPF

#### Structure des Fichiers
- Ajoutez un en-tête de commentaire standard à tous les fichiers source :
  ```csharp
  /*
   * Fichier : NomDuFichier.cs
   * Emplacement : ReturnToMoriaServerManager/Services/
   * Auteur : Votre Nom
   * Description : Description brève de la fonctionnalité
   */
  ```

#### Nommage
- **Classes** : PascalCase (`ServerManagerService`)
- **Méthodes** : PascalCase (`StartServer()`)
- **Variables** : camelCase (`serverStatus`)
- **Constantes** : UPPER_CASE (`DEFAULT_PORT`)
- **Interfaces** : Préfixe `I` (`IServerService`)

#### Architecture MVVM
- **Models** : Données et logique métier
- **ViewModels** : Logique de présentation
- **Views** : Interface utilisateur uniquement
- **Services** : Logique métier et accès aux données

#### XAML
- Utilisez des noms explicites pour les contrôles
- Organisez les propriétés dans un ordre logique
- Utilisez des styles et des ressources pour la cohérence

### Messages de Commit

Utilisez le format conventionnel :
```
type(scope): description

[body optionnel]

[footer optionnel]
```

Types :
- `feat` : Nouvelle fonctionnalité
- `fix` : Correction de bug
- `docs` : Documentation
- `style` : Formatage, espaces, etc.
- `refactor` : Refactoring
- `test` : Tests
- `chore` : Maintenance

Exemples :
```
feat(server): ajouter la surveillance en temps réel du statut

fix(config): corriger la sauvegarde des paramètres de difficulté

docs(readme): mettre à jour les instructions d'installation
```

## 🧪 Tests

### Tests Unitaires
- Créez des tests pour les nouvelles fonctionnalités
- Utilisez MSTest ou NUnit
- Placez les tests dans un projet séparé

### Tests d'Intégration
- Testez les interactions entre les services
- Vérifiez le comportement de l'interface utilisateur

## 📋 Template de Pull Request

```markdown
## Description
Description brève des modifications apportées.

## Type de Changement
- [ ] Bug fix (correction qui résout un problème)
- [ ] Nouvelle fonctionnalité (ajout qui ajoute une fonctionnalité)
- [ ] Breaking change (correction ou fonctionnalité qui casse la compatibilité)
- [ ] Documentation (mise à jour de la documentation)

## Tests
- [ ] J'ai ajouté des tests qui prouvent que ma correction fonctionne
- [ ] J'ai ajouté des tests qui prouvent que ma nouvelle fonctionnalité fonctionne
- [ ] Tous les tests existants passent

## Checklist
- [ ] Mon code suit les conventions de style du projet
- [ ] J'ai effectué une auto-review de mon propre code
- [ ] J'ai commenté mon code, particulièrement dans les zones difficiles à comprendre
- [ ] J'ai apporté les modifications correspondantes à la documentation
- [ ] Mes modifications ne génèrent pas de nouveaux avertissements
- [ ] J'ai ajouté des tests qui prouvent que ma correction fonctionne ou que ma fonctionnalité fonctionne
- [ ] Les tests unitaires et d'intégration passent localement avec mes modifications
- [ ] Toute modification dépendante a été documentée et mise à jour

## Screenshots (si applicable)
Ajoutez des captures d'écran pour les modifications de l'interface utilisateur.
```

## 🐛 Signaler un Bug

Utilisez le template d'issue pour les bugs :

```markdown
## Description du Bug
Description claire et concise du bug.

## Étapes pour Reproduire
1. Aller à '...'
2. Cliquer sur '...'
3. Faire défiler jusqu'à '...'
4. Voir l'erreur

## Comportement Attendu
Description claire et concise de ce que vous attendiez.

## Screenshots
Si applicable, ajoutez des captures d'écran pour expliquer votre problème.

## Environnement
- OS : [ex. Windows 10]
- Version : [ex. 1.0.0]
- .NET Version : [ex. 8.0]

## Informations Supplémentaires
Ajoutez tout autre contexte sur le problème ici.
```

## 💡 Proposer une Fonctionnalité

Utilisez le template d'issue pour les fonctionnalités :

```markdown
## Problème Résolu
Description claire et concise du problème que cette fonctionnalité résout.

## Solution Proposée
Description claire et concise de ce que vous voulez qu'il se passe.

## Alternatives Considérées
Description claire et concise de toutes les solutions alternatives ou fonctionnalités que vous avez considérées.

## Contexte Supplémentaire
Ajoutez tout autre contexte ou captures d'écran sur la demande de fonctionnalité ici.
```

## 📞 Support

Si vous avez des questions ou besoin d'aide :
- Créez une issue sur GitHub
- Consultez la documentation du projet
- Rejoignez la communauté

## 📄 Licence

En contribuant, vous acceptez que vos contributions soient sous la même licence que le projet (GPLv3).

Merci de contribuer à Return to Moria Server Manager ! 🎮 