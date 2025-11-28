# 🤝 Contribuer à NeoBleeper

Tout d'abord, merci de l'intérêt que vous portez à NeoBleeper ! Votre contribution est essentielle au succès de ce projet. Qu'il s'agisse de signaler un bug, de proposer une fonctionnalité, d'améliorer la documentation, de téléverser un fichier BMM ou NBPML existant, ou de soumettre du code, votre participation est précieuse.

## 📑 Sommaire

1. Code de conduite

2. Comment contribuer ?

- Signalement de bogues

- Demandes de fonctionnalités

- Contributions au code

- Documentation

- Contributions aux fichiers BMM et NBPML

3. Processus de demande de fusion

4. Guides de style

- Style de code

- Notes spécifiques à C#

5. Support communautaire

## 🌟 Code de conduite

En participant à ce projet, vous acceptez le Code de conduite. Veuillez respecter le Code de conduite. Soyez respectueux et attentionné envers les autres membres de la communauté. Consultez le fichier `CODE_OF_CONDUCT.md` pour plus de détails.

## 🤝🙋‍♂️ Comment contribuer ?

### 🪲 Signalement de bugs
Si vous avez trouvé un bug dans NeoBleeper, veuillez créer un ticket et inclure les informations suivantes :

- Un titre clair et descriptif.

- La version de NeoBleeper ou le hachage du commit, le cas échéant.

- Les étapes pour reproduire le problème, ou un extrait de code.

- Le comportement attendu et le comportement constaté.

- Toute autre information pertinente, y compris des captures d'écran ou des rapports de plantage.

### 💭 Suggestions de fonctionnalités
Vos idées sont les bienvenues ! Pour suggérer une fonctionnalité :

1. Vérifiez si quelqu'un d'autre l'a déjà demandée.

2. Si ce n'est pas le cas, ouvrez un nouveau ticket et fournissez une description détaillée incluant :

- Le contexte de votre demande. - Pourquoi c'est utile.

- Impacts, risques et points à prendre en compte.

### 👩‍💻 Contributions au code

1. Créez une copie du dépôt et une nouvelle branche à partir de `main`. Nommez votre branche de manière descriptive, par exemple `feature/add-tune-filter`.

2. Ouvrez le dossier du dépôt dans Visual Studio :

- Assurez-vous d'avoir installé [Visual Studio](https://visualstudio.microsoft.com/) avec les charges de travail requises (par exemple, « Développement .NET Desktop » pour NeoBleeper).

- Clonez votre copie du dépôt sur votre machine locale (vous pouvez utiliser les outils Git intégrés à Visual Studio ou l'interface de ligne de commande Git).

- Une fois le dépôt cloné, ouvrez le fichier de solution (`.sln`) dans Visual Studio.

3. Installez les packages NuGet :

- Restaurez les dépendances requises en cliquant sur « Restaurer les packages NuGet » dans la barre supérieure ou en exécutant `dotnet restore` dans le terminal. 4. Ajoutez vos modifications :

- Utilisez les fonctionnalités de Visual Studio telles qu'IntelliSense, le débogage et la mise en forme du code pour contribuer efficacement.

- Assurez-vous que les tests appropriés sont inclus et que tous les tests existants réussissent.

- Vérifiez que votre code respecte le guide de style.

5. Ajoutez votre nom ou pseudonyme à la page « À propos » :

- Ouvrez le fichier `about_neobleeper.cs` et localisez le composant `listView1`.

- Sélectionnez le composant `listView1` dans le concepteur de Visual Studio.

- Cliquez sur la petite flèche dans le coin supérieur droit du composant pour ouvrir le menu déroulant.

- Sélectionnez **Modifier les éléments** pour ouvrir l'éditeur de collection des éléments ListView.

- Ajoutez un nouvel élément `ListViewItem` :

- Saisissez votre nom ou pseudonyme dans la propriété **Texte**.

- Pour vos contributions/tâches :

- Localisez la propriété **SubItems**.

- Cliquez sur les trois points (« ... ») à droite du champ `(Collection)`. - Ajoutez ou modifiez le **SubItem** avec une brève description de vos tâches.

- Si vous avez déjà ajouté votre nom, modifiez le SubItem ou mettez à jour votre entrée existante avant de valider vos modifications.

6. Testez votre code :

- Exécutez les tests à l’aide de l’Explorateur de tests de Visual Studio.

- Corrigez les tests ayant échoué et validez vos modifications.

7. Validez vos modifications avec des messages clairs et concis.

- Utilisez les outils Git intégrés à Visual Studio pour indexer et valider vos modifications.

8. Envoyez votre branche et ouvrez une demande de fusion dans le dépôt.

9. Soyez prêt à collaborer avec les relecteurs et à apporter les corrections nécessaires.

### 🧾 Documentation

Améliorer notre documentation est l’une des manières les plus simples de contribuer ! N’hésitez pas à ajouter ou à mettre à jour des exemples, à clarifier des sections ou à améliorer la lisibilité générale.

### 🎼 Contributions aux fichiers BMM et NBPML
NeoBleeper prend en charge les anciens fichiers BMM (Bleeper Music Maker) et NBPML (NeoBleeper Project Markup Language). Si vous contribuez au projet ou travaillez avec ces types de fichiers, veuillez vous assurer des points suivants :

- Vérifiez que les fichiers BMM sont correctement analysés et rendus comme prévu dans NeoBleeper.

- Testez la compatibilité avec les anciens formats et l'implémentation actuelle.

- Pour les fichiers NBPML, assurez-vous de respecter les dernières spécifications du langage de balisage du projet NeoBleeper.

Si vous rencontrez des problèmes spécifiques à ces formats de fichiers, veuillez suivre les instructions de la section « Rapports de bogues ». Les demandes de fonctionnalités pour une meilleure prise en charge des fichiers BMM et NBPML sont également les bienvenues !

## ⬇️ Processus de demande de fusion

Toutes les soumissions doivent être effectuées via des demandes de fusion. Voici la procédure :

1. Remplissez le modèle de demande de fusion.

2. Assurez-vous que votre demande de fusion ne fait pas doublon avec les demandes existantes.

3. Ajoutez les détails de vos modifications dans la description, en faisant référence aux problèmes associés lorsque cela est possible.

4. Répondez à tous les commentaires ou demandes de modifications des relecteurs.

5. Les demandes de fusion doivent réussir tous les contrôles CI/CD, y compris les tests et les contrôles de qualité du code.

## 📖 Guides de style
### ✨ Style de code

Suivez les [Conventions de codage .NET](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). Points clés :

- Privilégiez les propriétés automatiques aux champs publics.

- Utilisez `var` pour les variables locales lorsque leur type est évident.

- Évitez les chaînes de caractères et les nombres magiques. Utilisez des constantes ou des énumérations.

### 📒 Remarques spécifiques à C#

- Placez les accolades `{` sur la même ligne que le code précédent.

- Utilisez la notation PascalCase pour les noms de classes et de méthodes, et la notation camelCase pour les variables locales.

- Suivez les [Consignes de nommage Microsoft](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines).

## 👨‍👩‍👧‍👦 Assistance communautaire

Pour toute question, n'hésitez pas à ouvrir une discussion sur GitHub ou à nous contacter via les issues. Nous encourageons le partage de connaissances et l'entraide entre collaborateurs.

Merci de contribuer à NeoBleeper et de participer à la création de ce projet exceptionnel !
