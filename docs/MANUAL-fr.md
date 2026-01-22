# Manuel NeoBleeper
- ## Comment utiliser NeoBleeper ?
  Les deux zones principales sont le clavier (zone horizontale en haut de l'écran) et la liste musicale (la liste en bas à droite avec sept colonnes).
  Le clavier est divisé en trois sections colorées, chacune représentant une octave.
  Pour accéder aux notes hors de la tessiture actuelle (plus hautes que les notes vertes ou plus basses que les notes rouges), ajustez la valeur « Octave » située à droite et à gauche du clavier.
  Augmenter cette valeur d'une unité décale les notes du clavier d'une octave plus haut.
 
  ![image](https://github.com/user-attachments/assets/34edbb36-bc79-4d49-8eae-3333337fee12)
  
  La liste musicale contient des lignes de notes. Jusqu'à quatre notes peuvent être jouées simultanément.
  Les sept colonnes de la liste musicale sont : « Durée » (la ou les notes sur la ligne), les quatre colonnes du milieu pour les quatre notes, la sixième colonne « Mod » (modificateur de durée) et la septième colonne « Art » (articulations).
  Lorsque vous cliquez sur des notes au clavier, elles sont immédiatement jouées par le haut-parleur du système et enregistrées dans la liste musicale par défaut dans la colonne « Note 1 ».
  Ce comportement peut être modifié pour saisir des notes dans « Note 1 », « Note 2 », « Note 3 » ou « Note 4 », permettant ainsi aux utilisateurs d'identifier les notes à l'écoute.
  Les notes cliquées au clavier sont insérées au-dessus de la ligne sélectionnée dans la liste musicale.
  Si aucune ligne n'est sélectionnée, les notes sont ajoutées à la fin du morceau.
  Pour ajouter des notes à la fin d'une ligne sélectionnée, cliquez sur « Désélectionner la ligne » (bouton turquoise à droite de la liste musicale).

  ![image](https://github.com/user-attachments/assets/ebe16f41-e415-47e9-ac31-5e7557b3a025)

  - ### Cases à cocher polyvalentes dans la liste de notes

    NeoBleeper intègre désormais des cases à cocher polyvalentes dans la liste de notes, offrant ainsi de puissantes fonctionnalités d'édition et de lecture avec une interface simple et intuitive.

    ![image1](https://github.com/user-attachments/assets/04f1190d-2902-4085-8970-f6671eb5af38)

    **Fonctionnalités clés des cases à cocher polyvalentes** :
    - **Jouer le rythme sur les lignes cochées (sans limitation de nombre entier)** :
      Lorsque la fonction « Jouer le rythme » ou un modificateur de rythme est activée, les temps sont joués uniquement sur les lignes où la case est cochée. Cela permet de personnaliser les motifs rythmiques, indépendamment des limites de mesure ou des positions de nombre entier ; les temps peuvent être joués sur n'importe quelle ligne cochée pour une plus grande flexibilité créative.
    - **Couper et copier plusieurs notes** :
      Pour couper ou copier plusieurs lignes simultanément, cochez les lignes souhaitées, puis utilisez les actions « Couper » ou « Copier ». Toutes les lignes cochées sont incluses dans l'opération, ce qui permet une édition par lots efficace.
    - **Effacer plusieurs notes** :
      Pour effacer plusieurs notes simultanément, cochez les lignes à supprimer et utilisez le bouton « Effacer ». Cela permet de supprimer rapidement plusieurs notes, réduisant ainsi le travail répétitif et les erreurs.
    - **Lecture vocale sur les lignes cochées :**
      Lorsque vous utilisez la synthèse vocale, vous pouvez limiter la lecture vocale aux seules lignes cochées. Cela facilite la mise en évidence de phrases musicales ou l'alternance entre bips traditionnels et synthèse vocale au sein d'un morceau.
    
    - **Conseils d'utilisation :**
      - Vous pouvez sélectionner ou désélectionner plusieurs lignes successivement pour configurer rapidement les parties de votre musique affectées par chaque opération.
      - Les cases à cocher polyvalentes fonctionnent en conjonction avec d'autres fonctionnalités d'édition, permettant des opérations par lots avancées sans changer de mode.
      - Ces cases à cocher sont indépendantes de la sélection de ligne pour la saisie de notes : les lignes peuvent être cochées pour les actions par lots même si une autre ligne est sélectionnée pour l'édition.
      - Cet ajout simplifie la composition et l'édition, vous offrant un meilleur contrôle sur la lecture, les effets vocaux et la manipulation par lots de votre musique.
    
  Pour basculer entre l'insertion et le remplacement d'une note, et pour choisir dans quelle colonne de notes une note est insérée ou remplacée, utilisez la zone d'options située à gauche du clavier, intitulée « Lorsque la touche est cliquée ».
  Cette zone permet également de jouer des notes sans les insérer ni les remplacer.

  ![image](https://github.com/user-attachments/assets/14d28506-1b4e-4875-a479-02ea46542c29)

  Pour modifier la durée de la note insérée, réglez la valeur « Durée de la note » située au milieu du clavier.
  Ce menu déroulant permet de sélectionner les notes suivantes : ronde, demi-tons, quart, croche, double-seizième ou triple-double.

  ![image](https://github.com/user-attachments/assets/0ab1ef00-f075-41cd-8184-8266a3671e0d)
  
  Par exemple, pour créer une blanche pointée, insérez une blanche puis une noire, ou insérez une blanche et appliquez un modificateur de longueur « Pointé ».

  ![image](https://github.com/user-attachments/assets/af1d13f7-b43d-41a9-9616-a0b3eb65e561)

  Si une triple note n'est pas assez courte, le tempo (BPM) du morceau peut être réglé jusqu'à 600 BPM, soit le double de la capacité de la plupart des séquenceurs MIDI.
  Cela permet d'augmenter le tempo et de compenser en doublant la durée d'une note sur deux.
  Le contrôle du tempo est situé en bas du clavier, juste à droite du contrôle de signature rythmique.
  Il définit le nombre de battements par minute et comprend un bouton « Métronome » pour indiquer la vitesse du battement de manière audible.

  ![image](https://github.com/user-attachments/assets/5e01bbc9-dac6-4928-a6db-d3263b92da6e)

  Un dossier nommé « Musique » est extrait du fichier ZIP. Il contient plusieurs fichiers de sauvegarde (NBPML) pour ce programme, qui peuvent être chargés comme démonstrations. Consultez la section « Ouverture et sauvegarde » ci-dessous pour plus d'informations.

  ![image](https://github.com/user-attachments/assets/7d69785d-5e00-45f2-b529-24a869810a98)

- ## Alternance des notes
  Comme le haut-parleur du système ne peut jouer qu'une seule note à la fois, le programme alterne entre « Note 1 », « Note 2 », « Note 3 » et « Note 4 » si plusieurs colonnes de la liste musicale contiennent une note sur la même ligne.
  La vitesse de cette alternance est contrôlée par la valeur saisie dans la zone de texte « Basculer entre les notes
alternees toutes les: ... ms », située à gauche de la liste musicale.
  L'ordre d'alternance des notes peut également être déterminé à l'aide des boutons radio situés sous la zone de texte « Basculer entre les notes
alternees toutes les: ... ms ».
  
  ![image](https://github.com/user-attachments/assets/4eddf2a3-1e57-4119-922b-46a50fc04908)

- ## Rapport Note/Silence
- 
  Ce paramètre définit la proportion de temps pendant lequel une ligne de la liste musicale produit du son par rapport au silence. Ajuster ce ratio peut réduire la sortie de son continu.

  ![image](https://github.com/user-attachments/assets/a42c70bf-2bc1-41f4-ac6a-271b6b7b0458)
  
- ## Ouverture et enregistrement de fichiers
  
  La musique et les paramètres peuvent être enregistrés et chargés à l'aide des boutons « Ouvrir », « Enregistrer » et « Enregistrer sous » du menu « Fichier » de la barre de menu. L'enregistrement inclut la liste musicale et la plupart des options de configuration.
  
  Les fichiers enregistrés utilisent l'extension « .NBPML » et sont au format texte XML. Ils peuvent être modifiés avec des éditeurs de texte comme le Bloc-notes.
  
  NeoBleeper peut également ouvrir les fichiers « .BMM », qui sont le format de fichier de Bleeper Music Maker, mais ils ne peuvent pas être écrasés et doivent être enregistrés dans un fichier « .NBPML » distinct.

  ![image](https://github.com/user-attachments/assets/4cd8097e-a972-43c3-9da1-4d99bae645df)

- ## Conseils pour l'édition musicale
  Les fichiers NBPML et BMM sont au format texte et compatibles avec les éditeurs de texte standard. Les fonctions copier-coller et rechercher-remplacer peuvent faciliter les tâches répétitives ou la correction d'erreurs.
  
  Pour effacer une ligne, utilisez le bouton rouge « Effacer toute la ligne ». Pour effacer une seule colonne de notes, utilisez les quatre boutons bleus situés dans la même zone.

   ![image](https://github.com/user-attachments/assets/d25f1329-7954-4b6a-a0ac-27e81ca925b5)
  
   Pour remplacer la longueur des notes, sélectionnez l'option « Remplacer » et activez le remplacement de la longueur. Cliquez ensuite sur « Ligne vide » pour chaque ligne afin de mettre à jour la longueur sans modifier les notes.

   ![image](https://github.com/user-attachments/assets/b2b7d349-1bb9-4ea4-b7bd-c87f76b721c7)

   ![image](https://github.com/user-attachments/assets/71287c3d-2d38-453c-83ce-a6290a3b854a)

- ## Lecture de musique
  
  Utilisez le bouton vert « Tout lire » en haut pour lire toutes les notes de la liste musicale. La lecture reprend au début à la fin, si la case située sous le bouton « Ligne vide » est cochée. Le bouton vert du milieu lit la ligne sélectionnée et revient à celle-ci.
  Le bouton vert du bas arrête la lecture une fois la note terminée.
  
  ![image](https://github.com/user-attachments/assets/de4ccdf8-3ec6-4ac0-be95-c4bfe022bc61)
  
  Cliquer sur une ligne de la liste musicale la lit par défaut. Pour modifier ce comportement ou limiter la lecture à une colonne de notes, cochez les cases « Lorsqu'une ligne est cliquée » sous « Quelles notes de la liste jouer » en bas à gauche de la fenêtre principale.
  Des cases à cocher similaires sous « Lorsque la musique est jouée » contrôlent le comportement de la lecture automatique.
  
  ![image](https://github.com/user-attachments/assets/af89b111-17f3-4357-a01b-e318fa2cc785)

- ## Modificateurs de longueur et articulations

  NeoBleeper prend en charge les notes pointées et en triolets, ainsi que les staccato, spiccato et fermata. La colonne « Mod » de la liste musicale affiche « Pnt » pour les notes pointées et « Tri » pour les triolets, tandis que la colonne « Art » affiche « Sta » pour le staccato, « Spi » pour le spiccato et « Fer » pour le fermata.

  ![image](https://github.com/user-attachments/assets/a160fe10-7337-48e3-98ab-17db81adcf70)

  Pour appliquer un modificateur pointée (1,5 fois la durée originale), sélectionnez une ligne et cliquez sur le bouton « Pointée » au-dessus de la liste musicale. Une note pointée équivaut à la durée originale plus la note la plus courte suivante. Par exemple, une noire pointée équivaut à une noire plus une croche.
  
  Pour appliquer un modificateur de triolet (1/3 de la durée originale), sélectionnez une ligne et cliquez sur le bouton « Triolet ». Trois notes en triolet de même durée équivalent à une note de la durée originale. Une ligne ne peut pas être à la fois pointée et triolet.
  
  Pour appliquer un modificateur de Staccato (la moitié de la durée originale, puis silence), sélectionnez une ligne et cliquez sur le bouton « Staccato ».
  
  Pour appliquer un modificateur de Spiccato (0,25 fois la durée originale, puis silence), sélectionnez une ligne et cliquez sur le bouton « Spiccato ».
  
  Pour appliquer un modificateur de Fermata (le double de la durée originale), sélectionnez une ligne et cliquez sur le bouton « Fermata ». Une ligne ne peut pas être à la fois Staccato, Spiccato et Point d'orgue. Pour insérer des notes pointées, en triolet, staccato, spiccato ou en fermata, appuyez sur le bouton correspondant, puis cliquez sur les notes du clavier. Pendant la lecture, les boutons « Pointée », « Triolet », « Staccato », « Spiccato » et « Fermata » s'activent automatiquement lorsque ces modificateurs et articulations sont rencontrés.

  ![image](https://github.com/user-attachments/assets/88c15f44-46d9-42a1-af04-487fc840a72f)

- ## Affichage de la signature rythmique et de la position

  NeoBleeper propose un paramètre « Signature rythmique », situé à gauche du paramètre BPM. Il définit le nombre de temps par mesure. Ce paramètre affecte le son du métronome et l'affichage de la position, mais ne modifie pas le son de lecture.

  ![image](https://github.com/user-attachments/assets/15560a35-b1b1-4d8d-b46d-f3ae693cd2b6)
  
  Trois affichages de position, en bas à droite, indiquent la position actuelle dans la musique. L'affichage supérieur indique la mesure, celui du milieu le temps dans la mesure, et celui du bas affiche une représentation traditionnelle en rondes, blanches (1/2), noires (1/4), croches, doubles (1/16) ou triples (1/32).
  
  ![image](https://github.com/user-attachments/assets/73e5998b-2fc8-4ea0-ac1b-c614514bfdd4)

  Les signatures rythmiques plus basses entraînent des changements plus rapides dans l'affichage supérieur. L'affichage central revient à 1 au début de chaque nouvelle mesure.

  L'affichage inférieur ne peut pas représenter les positions plus précises que les triples croches. Il affiche « (Erreur) » en rouge pour les positions non prises en charge, telles que celles créées par des doubles croches pointées (3/64). Une fois la position divisible par une triple croche, l'affichage reprend son fonctionnement normal.
  
  ![image](https://github.com/user-attachments/assets/ce365675-31b9-4275-8397-508b1b2d1975)

  Les notes en triolet affectent également la précision de l'affichage. Après la saisie de trois notes en triolet de même durée, la position devient divisible par une triple croche, rétablissant ainsi la fonctionnalité d'affichage.

  La lecture de triolets vers la fin d'une longue liste musicale peut nécessiter des ressources processeur importantes. En cas de problèmes de performances, cochez la case « Ne pas mettre à jour » sous l'affichage des positions pour désactiver les mises à jour pendant la lecture. Les mises à jour du mode Édition restent actives.
  
  Les anciens fichiers BMM créés avec des versions antérieures à la révision 127 de Bleeper Music Maker utilisent par défaut une signature temporelle de 4 lorsqu'ils sont ouverts dans NeoBleeper. La modification et l'enregistrement de la signature temporelle dans les fichiers .NBPML préservent ce paramètre.

- ## Journalisation du débogage

  Depuis la version 0.18.0 Alpha, NeoBleeper utilise la classe « Logger » pour gérer l'ensemble de la journalisation et des diagnostics. La sortie de la journalisation est enregistrée dans un fichier nommé « DebugLog.txt » situé à la racine de l'application.
  
  La classe « Logger » fournit des informations d'exécution détaillées, notamment les erreurs, les avertissements et les messages de débogage généraux. Ce fichier journal est automatiquement créé et mis à jour pendant l'exécution de l'application.
  
  Pour un débogage avancé, vous pouvez toujours lancer NeoBleeper directement depuis Visual Studio afin d'utiliser ses outils intégrés, tels que les points d'arrêt et la fenêtre de sortie. Cependant, le fichier « DebugLog.txt » garantit la disponibilité constante de la journalisation, même en dehors de l'environnement de développement Visual Studio.
  
  Les fichiers de déclenchement externes tels que « logenable » et les anciennes méthodes de diagnostic ne sont plus pris en charge. Toutes les informations pertinentes sont désormais centralisées dans le fichier « DebugLog.txt » pour faciliter l'accès et la consultation.

- ## Mods

  Le programme inclut plusieurs modifications qui modifient son fonctionnement par rapport à la conception originale. Ces modifications sont listées en bas à gauche de l'écran, à côté de la liste des musiques. Chaque mod dispose d'une case à cocher permettant de l'activer ou de le désactiver. Si une case ne peut pas être décochée, la fermeture de la fenêtre du mod désactivera le mod et décochera la case.
  
  Cliquez sur le point d'interrogation à côté de la case à cocher d'un mod pour afficher une brève description de sa fonction (disponible pour la plupart des mods).
  
  ![image](https://github.com/user-attachments/assets/b1a88c3c-1004-49e8-bcc1-74bfedac0074)
  
  - ### Mod de lecture synchronisée

    ![image](https://github.com/user-attachments/assets/f69800b2-1ee2-467b-bba7-d816f9de93dc)
    
    Ce mod permet à NeoBleeper de démarrer la lecture à une heure système spécifiée. Il est conçu pour synchroniser plusieurs instances de NeoBleeper, notamment lors de l'utilisation de fichiers NBPML ou BMM distincts pour différentes sections d'une composition. En configurant chaque instance pour qu'elle démarre simultanément, la lecture est synchronisée entre elles.
    
    L'activation du mod ouvre une fenêtre de configuration. Cette fenêtre permet de saisir une heure de début cible (heure, minute, seconde) basée sur l'horloge système. L'heure système actuelle est affichée à titre de référence. Par défaut, l'heure cible est définie une minute avant l'heure actuelle, mais cette valeur peut être ajustée manuellement. Vous pouvez également spécifier si la lecture doit commencer au début de la musique ou à partir de la ligne sélectionnée dans la liste musicale. Le programme exécutera la commande de lecture correspondante (« Tout lire » ou « Lire à partir de la ligne sélectionnée ») lorsque le temps cible sera atteint.
    
    Un bouton de commande permet de lancer l'attente. Une fois activé, l'interface indique que le programme est en attente et le bouton devient « Arrêter l'attente ». Si le programme n'est pas en attente lorsque le temps cible est atteint, la lecture ne se fera pas.
    
    La case « Lecture synchronisée » est décochée lorsque la fenêtre est fermée. Pour rouvrir la fenêtre, désactivez cette option pour annuler tout état d'attente actif.
    
    La lecture s'arrête automatiquement lors du lancement de l'attente afin d'éviter tout problème, contrairement à Bleeper Music Maker original.
    
    La synchronisation entre plusieurs ordinateurs est possible si toutes les horloges système sont parfaitement alignées. Il est recommandé de synchroniser les horloges système avant d'utiliser cette fonctionnalité sur plusieurs appareils.
    
  - ### Mod Jouer un son de rythme

    ![image](https://github.com/user-attachments/assets/464645ad-591d-4f32-b07b-7202e81739d8)
    
    Cette modification permet au haut-parleur/dispositif sonore du système d'émettre un son de rythme à chaque temps ou tous les deux temps, selon la configuration sélectionnée. Le son ressemble à un rythme techno grâce à la nature électronique du haut-parleur/dispositif sonore. Lorsque la case « Jouer un son de rythme » est cochée, une fenêtre de configuration apparaît. Cette fenêtre permet de choisir si le son est joué à chaque temps ou à chaque temps impair. Cette dernière option divise par deux le tempo des sons de rythme.
    
    Pour visualiser le changement de tempo, lancez le programme, ajoutez quatre noires à la liste musicale, activez l'option « Jouer un son de rythme » et basculez entre les deux réglages de son de rythme. La différence de tempo devrait être audible. La case à cocher « Jouer le son du rythme » est décochée lorsque la fenêtre de configuration est fermée.
      
  - ### Mod Portamento de bip

    ![image](https://github.com/user-attachments/assets/66976096-a23b-41db-a4ef-13d1212b2ed1)
    
    Cette modification permet au haut-parleur/dispositif sonore de passer progressivement de la note précédente à la note actuelle. Lorsque la case « Portamento de bip » est cochée, une fenêtre de paramètres apparaît. Cette fenêtre permet d'ajuster la vitesse de transition entre les notes, allant d'une durée quasi instantanée à une durée prolongée. Vous pouvez également configurer la durée de la note lorsque vous cliquez dessus ou la laisser jouer indéfiniment.
  
  - ### Utiliser le clavier comme module de piano

    ![image](https://github.com/user-attachments/assets/2a5cb4ef-857c-4250-a202-15a1d1f10da9)

    Cette fonctionnalité associe le clavier de l'ordinateur aux notes de musique, permettant une lecture directe par simple pression des touches, sans périphérique d'entrée MIDI. Chaque touche correspond à une note spécifique du piano virtuel. L'association suit une disposition prédéfinie, généralement alignée sur les libellés visibles du clavier.

  Lorsqu'elle est activée, appuyer sur une touche déclenche immédiatement la note associée selon la méthode de synthèse active.

  - ### Système vocal (« Internes de la Vois »)

    NeoBleeper intègre désormais un puissant système de synthèse vocale, accessible via la fenêtre « Voice Internals ». Ce système permet un contrôle avancé des voix synthétisées, notamment les formants vocaliques, le bruit et la sybilance, vous permettant ainsi de créer des sons vocaux de type humain ou expérimentaux directement dans vos compositions.

    ![image](https://github.com/user-attachments/assets/75285d69-e146-4d53-a7ce-5029215b9b17)

      - #### **Accès au système vocal**

        Pour ouvrir la fenêtre « Internes vocaux », recherchez l'option « Système vocal » ou « Internes vocaux » dans le menu ou dans la sélection du périphérique de sortie pour chaque note.
      Chaque colonne de notes (Notes 1 à 4) peut désormais être acheminée individuellement vers le système vocal, le bip sonore traditionnel ou d'autres périphériques de sortie grâce aux nouvelles listes déroulantes « Options de sortie ».
      
      - #### **Présentation de la fenêtre « Internes vocaux »**
      
        La fenêtre « Internes vocaux » est organisée en sections, chacune vous permettant de contrôler précisément différents aspects de la voix synthétisée :

      - ##### **Contrôle des formants**

        Il existe quatre curseurs de formants, chacun représentant une résonance clé du conduit vocal humain :
          - Réglez le **Volume** et la **Fréquence (Hz)** pour chaque formant.
          - Les boutons de préréglage (« Ouvrir la voyelle », « Fermer le front », etc.) permettent une sélection rapide des voyelles typiques.
          
        - ##### **Section Oscillateur**

          Les curseurs **Volume de Scie** et **Volume de Bruit** contrôlent le niveau de l'oscillateur en dents de scie et de la source de bruit, constituant la base du timbre vocal.
          Ils peuvent être combinés avec les filtres à formants pour obtenir divers effets synthétiques et vocaux.
          
        - ##### **Sybillance et Masquage de Sybillance**
        
          Quatre commandes de masquage vous permettent de simuler des effets de sybilance ou de consonne en modelant les composantes du bruit et en masquant les fréquences.
          Le curseur « Hz de coupure » ​​définit une fréquence de coupure pour le masquage du bruit.
            
        ##### **Variations aléatoires des formants**

          - Les curseurs de hauteur et de plage introduisent de subtiles variations aléatoires dans les fréquences des formants, ajoutant du réalisme ou des effets spéciaux.
        
        - ##### **Options de sortie**

          - Affectez le moteur sonore à chaque colonne de notes :
            « Bip du haut-parleur système/du périphérique sonore »
            « Système vocal »
            ...et d'autres, si disponibles.
        
          Vous pouvez jouer un mélange de bips vocaux et de bips du haut-parleur système/du périphérique sonore dans un même morceau.
        
        - ##### **Notes sur la tonalité et l'utilisation**
        
          La fenêtre propose une légende pour le codage couleur et les abréviations des paramètres.
          Un menu déroulant vous permet de choisir quand jouer la voix (toutes les lignes, des lignes spécifiques, etc.).
        
        - #### **Comment utiliser le système vocal**
        
          1. **Affecter une note au système vocal**
          
            Dans les « Options de sortie » de la fenêtre « Internes vocaux » ou de l'interface principale, définissez une colonne de notes (par exemple, Note 2) sur « Système vocal ».
          2. **Modifier les formants et les paramètres de l'oscillateur**
          
            Utilisez les curseurs et les boutons de préréglage pour façonner la voyelle, le timbre et la sifflante.
          3. **Lecture**
          
            Lorsque vous écoutez de la musique, les colonnes de notes sélectionnées utiliseront le synthétiseur vocal en fonction de vos paramètres.
          4. **Expérimentation**
          
            Essayez différentes combinaisons, plages de randomisation et mixages d'oscillateurs pour obtenir des voix robotiques, naturelles ou synthétiques uniques.
        
        - #### **Conseils**
        
        - Mélangez et assortissez : attribuez certaines notes au système vocal et d'autres aux bips pour des bandes sonores riches et complexes.
        - Pour un résultat optimal, ajustez les formants à la hauteur de vos notes.
        - Utilisez les curseurs de randomisation pour des irrégularités plus « humaines » ou des artefacts robotiques.
        - Le système vocal peut être utilisé pour la conception sonore expérimentale, et pas seulement pour les voix.

- ## Paramètres
  La fenêtre Paramètres de NeoBleeper est divisée en quatre onglets principaux, chacun ciblant un aspect différent de la configuration de l'application.
  
  - ### Paramètres généraux
    Cet onglet se concentre sur les préférences fondamentales et l'intégration au niveau système :

    ![image](https://github.com/user-attachments/assets/26a990a8-943c-4a03-8952-dc9574b297a4)

    - #### Langue
      **Sélecteur de langue** : vous permet de choisir la langue de NeoBleeper parmi l'anglais, l'allemand, le français, l'italien, l'espagnol, le turc, le russe, l'ukrainien et le vietnamien.

    - #### Apparence générale
      **Sélecteur de thème** : vous permet de choisir entre les thèmes personnalisés de NeoBleeper ou d'utiliser l'apparence par défaut de votre système d'exploitation.
    
      **Mode Bleeper classique** : une option traditionnelle pour les utilisateurs qui préfèrent l'interface ou le comportement d'origine.
    
    - #### Créer de la musique avec l'IA
      **Champ de clé API Google Gemini™** : saisie sécurisée pour activer les fonctionnalités musicales générées par l'IA.
    
      **Avertissement de sécurité** : recommande aux utilisateurs de ne pas partager leur clé API.
    
      **Boutons de mise à jour/réinitialisation** : gèrent le cycle de vie de la clé API. Le bouton de mise à jour est désactivé, probablement en attente d'une saisie valide.

      - ##### **Conditions d'utilisation et vérification de l'âge de l'API Google Gemini™**
  
        Lors de la première saisie d'une clé API, une fenêtre d'acceptation des conditions d'utilisation de l'API Google Gemini™ s'affiche. Pour respecter les exigences de l'API et les restrictions d'âge, les utilisateurs doivent :
        
        - **Lire et accepter les conditions d'utilisation** avant d'utiliser les fonctionnalités de l'API Google Gemini™.
        
        - **Saisir leur date de naissance** pour confirmer qu'ils sont âgés de 18 ans ou plus. Les personnes de moins de 18 ans ne sont pas autorisées à utiliser les fonctionnalités de l'API Google Gemini™ dans NeoBleeper.
        
        - Si la clé API fournie n'est pas valide, si l'utilisateur est âgé de moins de 18 ans ou s'il n'accepte pas les conditions d'utilisation, la clé sera rejetée et les fonctionnalités de l'API Google Gemini™ ne seront pas activées.
        
        - **Votre date de naissance ne sera ni stockée ni partagée** par NeoBleeper ; elle est utilisée une seule fois à des fins de vérification d'âge et de conformité.
  
        ![image1](https://github.com/user-attachments/assets/73f5b45f-a719-4d01-bd3d-71bdcb7cd9ee)
        
        L'âge minimum requis et l'acceptation des conditions d'utilisation sont obligatoires lors de la première saisie d'une nouvelle clé API. Ceci garantit une utilisation responsable et la conformité légale des fonctionnalités musicales générées par l'IA.
  
    - #### Test du haut-parleur système

    NeoBleeper propose des outils performants pour vérifier la sortie audio de votre haut-parleur système et diagnostiquer les problèmes de connectivité ou de compatibilité. Dans l'onglet Général des Paramètres, sous « Test du haut-parleur système », vous trouverez deux fonctions de test distinctes :

    ![image](https://github.com/user-attachments/assets/e89017d3-46b4-4ba5-ac32-736874dd39d3)
    
    **Tester le haut-parleur système :**
    
    - Appuyez sur ce bouton pour afficher un menu déroulant avec deux options :
    
    - **Test de lecture d'un morceau standard :** Diffuse un morceau simple pour vérifier si le haut-parleur système produit le son attendu.
    
    - **Test avancé du haut-parleur système :** Exécute une série de tests de diagnostic pour évaluer en détail le fonctionnement de votre haut-parleur et la stabilité de ses pilotes (voir détails ci-dessous).
    
    **Fonctionnalités avancées de test des haut-parleurs système**
    
    Lorsque vous sélectionnez « Test avancé des haut-parleurs système » dans le menu déroulant, NeoBleeper effectue plusieurs vérifications de votre configuration matérielle et logicielle et affiche les résultats dans une fenêtre contextuelle :

    ![image1](https://github.com/user-attachments/assets/d56eb1c4-b626-400b-80ef-be5366e7aab6)
    
    Les tests suivants sont exécutés :
    
    - **Test de retour électrique :** Vérifie la réponse électrique attendue des haut-parleurs du système.
    
    - **Test de stabilité de l'état des ports :** Vérifie la stabilité de l'état des ports et des pilotes.
    
    - **Test de balayage de fréquence avancé :** Teste différentes fréquences pour garantir la reproduction d'une large gamme de tonalités.
    
    - **Résultat global :** Un résumé indique si tous les tests ont réussi ou si un problème spécifique a été détecté.
    
    Chaque vérification est accompagnée d'un statut (✔️ PASSÉ ou ❌ ÉCHEC).
    
    > Si vous n'entendez aucun son, vous devrez peut-être utiliser la fonction « Utiliser un périphérique audio pour émettre un bip ».
  
  - ### Création de paramètres sonores
    Cet onglet permet de configurer la manière dont NeoBleeper génère des bips audio grâce aux capacités sonores de votre système. Il offre à la fois un contrôle technique et une flexibilité créative pour façonner le ton et la texture des sons produits.

    ![image](https://github.com/user-attachments/assets/d285ab27-e04c-492f-9142-adab876426e9)
    
    - #### Utiliser le périphérique audio pour générer un bip

      Cette case à cocher active ou désactive l'utilisation du périphérique audio de votre système pour générer un bip au lieu du haut-parleur. Si elle n'est pas cochée, NeoBleeper utilise le haut-parleur pour générer le son. Activer cette option permet une synthèse sonore plus riche, basée sur des formes d'onde.

    - #### Création de bips depuis les paramètres du périphérique audio
      - ##### Sélection de la forme d'onde
        **Choisissez la forme d'onde utilisée pour générer les bips. Chaque option affecte le timbre et le caractère du son :**
        
        **Carré (par défaut) :** Produit un son aigu et bourdonnant. Idéal pour les bips numériques classiques et les alertes rétro.
        
        **Sinusoïdal :** Son doux et pur. Idéal pour les notifications discrètes ou les applications musicales.
        
        **Triangle :** Plus doux que le carré, avec un son légèrement creux. Équilibre entre acuité et fluidité.
        
        **Bruit :** Génère des rafales de signal aléatoires, utiles pour les effets sonores tels que les parasites, les rafales ou les textures de type percussion.

  - ### Paramètres des appareils

  Cet onglet vous permet de configurer l'interaction de NeoBleeper avec le matériel MIDI externe, les instruments virtuels et autres périphériques. Que vous intégriez une entrée en direct ou routiez une sortie vers un synthétiseur, c'est ici que vous définissez le flux du signal.

    ![image](https://github.com/user-attachments/assets/b863e209-628f-41bf-a0f9-ffaa4498c351)

    - #### Périphériques d'entrée MIDI
      **Utiliser l'entrée MIDI en direct** : Active la réception de signaux MIDI en temps réel depuis des contrôleurs ou logiciels externes. Lorsque cette option est cochée, NeoBleeper écoute les messages MIDI entrants pour déclencher des sons ou des actions.
      
      **Sélectionner le périphérique d'entrée MIDI** : Un menu déroulant répertorie les sources d'entrée MIDI disponibles. Choisissez votre périphérique préféré pour commencer à recevoir des données MIDI.
      
      **Actualiser** : Met à jour la liste des périphériques d'entrée disponibles, utile pour connecter un nouveau matériel ou lancer des ports MIDI virtuels.
      
    - #### Périphériques de sortie MIDI
      **Utiliser la sortie MIDI** : Active la transmission MIDI de NeoBleeper vers des périphériques externes ou des instruments virtuels.
      
      **Sélectionner le périphérique de sortie MIDI** : Choisissez où NeoBleeper envoie ses signaux MIDI. L'option par défaut est généralement un synthétiseur polyvalent comme Microsoft GS Wavetable Synth.
      
      **Canal** : Sélectionne le canal MIDI (1/16) utilisé pour la sortie. Cela permet le routage vers des instruments ou des pistes spécifiques dans les configurations multicanaux.
      
      **Instrument** : Définit l'instrument General MIDI utilisé pour la lecture. Les options vont des pianos et cordes aux synthés et percussions, vous permettant de contrôler le timbre de la sortie.
      
      **Actualiser** : Met à jour la liste des périphériques de sortie disponibles, garantissant ainsi la reconnaissance des nouveaux appareils connectés.
      
    - #### Autres périphériques et micrologiciel du microcontrôleur

      NeoBleeper prend également en charge l'interaction avec divers périphériques externes, tels que les buzzers, les moteurs et les microcontrôleurs, ce qui étend ses capacités au-delà des périphériques MIDI traditionnels. Le groupe **Autres périphériques** de l'onglet Paramètres des périphériques propose des options de configuration et des outils de génération de micrologiciel pour ces composants externes.

      **Paramètres pour les autres appareils** :
      - **Activer l'appareil** :
      
      Cochez la case pour activer l'utilisation d'un moteur ou d'un buzzer (via Arduino, Raspberry Pi ou ESP32). Cette case doit être cochée pour accéder aux autres options de l'appareil.
      - **Type d'appareil** :
      
      Choix entre les boutons radio :
      - **Moteur pas à pas**
      - **Moteur CC ou buzzer**
      - **Octave du moteur pas à pas** :
      
      Curseur permettant de régler l'octave de sortie du moteur pas à pas, vous permettant ainsi d'adapter le mouvement du moteur aux notes musicales.
      - **Bouton Obtenir le micrologiciel** :
      
      Cliquer sur ce bouton génère un micrologiciel compatible pour l'appareil sélectionné. Vous devez installer ce micrologiciel sur votre microcontrôleur avant d'utiliser cette fonctionnalité. Si le microcontrôleur n'est pas installé, la case à cocher de l'appareil reste grisée.
      
      ![image1](https://github.com/user-attachments/assets/0c30b469-a32c-43f4-8ecd-dbd3cc7ee462)
      
      **Générateur de firmware pour microcontrôleur** :
      - Cette fonctionnalité vous permet de générer et de copier rapidement un firmware prêt à l'emploi pour microcontrôleurs (comme Arduino) directement depuis NeoBleeper.
      - Le firmware permet de contrôler des composants matériels tels que des buzzers et des moteurs pas à pas, permettant ainsi à vos compositions musicales de déclencher des actions physiques et des sons.
      - Vous pouvez sélectionner votre type de microcontrôleur (par exemple, « Arduino (fichier ino) ») dans le menu déroulant.
      - La fenêtre de code affiche le firmware généré, adapté au périphérique sélectionné.
      - Cliquez sur le bouton « Copier le firmware dans le presse-papiers » pour copier facilement le code et le télécharger sur votre microcontrôleur.

      **Exemple d'utilisation :**
      - Cette fonctionnalité vous permet de synchroniser la lecture musicale avec du matériel, comme l'activation de buzzers ou le pilotage de moteurs pas à pas, en utilisant les signaux de sortie du système ou le G-Code exporté.
      - Le firmware Arduino généré inclut la gestion des commandes série pour l'identification des périphériques et le contrôle de la vitesse des moteurs, facilitant ainsi l'intégration de NeoBleeper à des systèmes robotiques ou des installations personnalisées.
      
      **Conseils d'intégration :**
      - Combinez l'exportation du G-Code de NeoBleeper avec le firmware du microcontrôleur pour convertir la musique en mouvements mécaniques ou en signaux sonores.
      Le groupe « Autres périphériques » simplifie la connexion de votre PC à du matériel externe, élargissant ainsi les possibilités créatives pour les machines musicales, les performances cinétiques ou l'art sonore expérimental.
      
      > Pour plus de détails ou pour résoudre un problème, consultez les canaux d'assistance NeoBleeper ou la documentation de votre microcontrôleur.
            
  - ### Paramètres d'apparence

  Cet onglet vous offre un contrôle total sur l'identité visuelle de NeoBleeper, vous permettant de personnaliser les couleurs des éléments clés de l'interface pour plus de clarté, d'esthétique ou de style. Il est organisé en sections : clavier, boutons, indicateurs et affichage des événements textuels.

  ![image5](https://github.com/user-attachments/assets/f6667bdb-0ce1-45d3-906b-dc6705b1e304)

    - #### Couleurs du clavier
      **Définissez le jeu de couleurs pour les différentes octaves du clavier virtuel :**
      
      **Couleur de la première octave :** Orange clair
      
      **Couleur de la deuxième octave :** Bleu clair
      
      **Couleur de la troisième octave :** Vert clair
    
    Ces paramètres permettent de distinguer visuellement les plages de hauteur, facilitant ainsi l'interprétation et la composition.
    
    - #### Couleurs des boutons et commandes
      **Personnalisez l'apparence des éléments interactifs de l'interface :**
      
      **Couleur de la ligne vide :** Orange clair
      
      **Couleur d'effacement des notes :** Bleu
      
      **Couleur de désélection de la ligne :** Cyan clair
      
      **Couleur d'effacement de la ligne entière :** Rouge
      
      **Couleur des boutons de lecture :** Vert clair
      
      **Couleur du métronome :** Bleu clair
      
      **Couleur du marquage du clavier :** Gris clair
      
    Ces attributions de couleurs améliorent l'ergonomie en rendant les actions et les états visuellement intuitifs.
    
    - #### Couleurs des indicateurs
      **Définir les couleurs des indicateurs de retour en temps réel** :
      
      **Couleur de l'indicateur sonore** : Rouge
      
      **Couleur de l'indicateur de note** : Rouge
      
    Ces indicateurs clignotent ou s'allument pendant la lecture ou la saisie, vous permettant de surveiller l'activité d'un coup d'œil.
    
    - #### Paramètres des événements paroles/texte
      **Taille des événements paroles/texte** : Ajustez la taille (en points) des événements paroles/texte affichés pendant la lecture de fichiers MIDI ou d'autres fonctionnalités pilotées par événements.
      
      **Aperçu des paramètres des événements paroles/texte** : Utilisez ce bouton pour prévisualiser l'apparence des événements paroles/texte, afin de garantir une lisibilité et un style adaptés à vos préférences.
      
      - #### Option de réinitialisation
      **Réinitialiser les paramètres d'apparence aux valeurs par défaut** : Un bouton permet de restaurer tous les paramètres de couleur et d'apparence à leurs valeurs par défaut, idéal pour annuler des expériences ou repartir de zéro.
        
  - ## Outils

    Ces outils compacts et puissants du menu « Fichier » offrent un accès rapide à trois fonctionnalités clés de NeoBleeper, chacune conçue pour optimiser votre flux de travail et élargir vos possibilités créatives. Chaque option est associée à un raccourci clavier pour un contrôle rapide et pratique :

    ![image](https://github.com/user-attachments/assets/eafc1e97-7189-4b5c-9f47-27c6a902f9dd)

    - ### Lecture d'un fichier MIDI - Ctrl + M
      
      Chargez et lisez instantanément un fichier MIDI via un haut-parleur système ou un périphérique audio dans NeoBleeper. Cette fonctionnalité est idéale pour prévisualiser vos compositions, tester la précision de la lecture ou intégrer des données MIDI externes à votre flux de travail.

      ![image](https://github.com/user-attachments/assets/4a600d9c-a987-437b-b698-7c6a8e20647f)

      Vous pouvez choisir le fichier MIDI en cliquant sur « Parcourir le fichier » dans la fenêtre « Paramètres de lecture du fichier MIDI ». Le fichier MIDI sélectionné apparaît dans la zone de texte à gauche du bouton.

      L'heure est affichée au format « 00:00.00 » (minutes, secondes, centièmes de seconde). Elle est mise à jour uniquement lorsque le minuteur de lecture est activé et que les messages MIDI sont lus aux bons moments, à condition que le tempo reste inchangé.
      Le pourcentage indique la proportion de messages MIDI traités. Par exemple, si la première moitié contient peu de messages et que la seconde est dense, le pourcentage pourrait n'atteindre 50 % qu'à un stade avancé de la lecture. Une case à cocher « Boucle » permet de redémarrer automatiquement le fichier MIDI une fois terminé.
      Les trois boutons situés sous le curseur, de gauche à droite, permettent de revenir au début du fichier MIDI, de lire (à partir de la position actuelle) et d'arrêter (sans revenir au début). Une case à cocher située sous ces commandes permet de lancer la lecture en boucle.
      
      Dans cette fenêtre, les utilisateurs peuvent sélectionner des canaux d'entrée spécifiques. Les canaux non sélectionnés seront ignorés. Les utilisateurs peuvent cocher ou décocher les cases ; les modifications prennent effet immédiatement. Lorsqu'une case est cochée, les notes du canal correspondant sont traitées pendant la lecture.
      
      En bas de la fenêtre « Jouer le fichier MIDI », une grille de rectangles affiche les notes tenues. Chaque rectangle représente une note tenue. Jusqu'à 32 rectangles peuvent être affichés simultanément. Si plus de 32 notes sont tenues, seules les 32 premières sont affichées.
      
      La modification du paramètre « Changer de note toutes les ... ms » dans la fenêtre « Jouer le fichier MIDI » affecte la vitesse de lecture des notes reçues par l'entrée MIDI.
      
      Si la case « Jouer chaque note une seule fois (sans alternance) » est cochée, chaque note est jouée une fois pendant la durée spécifiée par le paramètre « Changer de note toutes les ... ms ». Cela produit un effet plus staccato.
      
      Si la case « Essayer de faire durer chaque cycle 30 ms (avec une durée d'alternance maximale de 15 ms par note) » est cochée, la durée de l'alternance est automatiquement ajustée pour respecter ce comportement temporel. Cela permet de maintenir un timing précis lorsque plusieurs notes sont jouées en succession rapide.
            
      #### Affichage des paroles et des événements textuels

      Le lecteur de fichiers MIDI de NeoBleeper inclut une fonctionnalité permettant d'afficher les paroles et les événements textuels intégrés aux fichiers MIDI, offrant ainsi un retour visuel en temps réel des lignes vocales ou des signaux pour les applications de karaoké et de performance.

      ![image1](https://github.com/user-attachments/assets/89f3d937-32a1-4414-87a9-7280953bb820)

      Lorsque la case « Afficher les paroles ou les événements textuels » est cochée dans la fenêtre « Lire le fichier MIDI », tous les événements de paroles ou de texte intégrés au fichier MIDI en cours de lecture sont affichés en évidence en bas de la fenêtre de l'application. Ces événements apparaissent sous forme de superpositions de texte larges et claires, mises à jour au rythme de la progression du morceau.

      Cette fonctionnalité est particulièrement utile pour suivre les parties vocales, donner le signal aux artistes en direct ou simplement profiter d'une lecture de type karaoké. Si le fichier MIDI ne contient pas de paroles ou d'événements textuels, la superposition reste masquée.
      
      L'affichage des paroles et du texte se met à jour automatiquement à mesure que de nouveaux événements surviennent pendant la lecture et disparaît à l'arrêt de la lecture ou au chargement d'un nouveau fichier.

    - ### Créer de la musique avec l'IA - Ctrl + Alt + A

      Exploitez la puissance de l'IA pour générer des idées musicales. Que vous cherchiez l'inspiration, combliez des lacunes ou expérimentiez de nouveaux styles, cet outil propose des suggestions intelligentes et contextuelles de mélodies, d'harmonies et de rythmes.

      ![image](https://github.com/user-attachments/assets/bc406962-b21b-4473-be1f-1a252b613c46)

      **Fonctionnement** :
      - Ouvrez la fenêtre « Créer de la musique avec l'IA » depuis le menu Fichier ou en utilisant le raccourci.
      - Choisissez le **modèle d'IA** souhaité (par exemple, Gemini 2.5 Flash) dans le menu déroulant.
      - Saisissez une invite musicale dans le champ **Invite** (par exemple, « Générer un air folk avec guitare acoustique »).
      - Cliquez sur **Créer** pour que l'IA génère la musique. Une barre de progression indique le traitement de la requête.
      - Un avertissement vous rappelle que les résultats sont des suggestions inspirantes et peuvent contenir des erreurs.
      - Cette fonctionnalité est optimisée par Google Gemini™.

      **Conseils rapides et restrictions de l'IA:**
      - L'outil d'IA ne traitera que les invites liées à la composition musicale. Si votre invite n'est pas liée à la musique (par exemple, « écrire une blague »), vous recevrez une erreur :

        ![image](https://github.com/user-attachments/assets/b3572268-f419-4925-a6fb-fb451d4ccf1d)
        
        *"Détection d'une requête sans rapport avec la musique. Les requêtes doivent concerner la composition musicale. Veuillez essayer de demander une composition de chanson ou de la musique liée à un artiste."*
      - Les messages contenant du contenu offensant ou inapproprié ne sont pas autorisés. Si vous en détectez, une erreur s'affichera :

        ![image3](https://github.com/user-attachments/assets/2335a810-9e81-4308-ac3b-68820b1f7957) 

        *"Un langage offensant a été détecté dans l'invite. Veuillez reformuler votre demande sans utiliser de termes vulgaires ou explicites. Essayez de demander une composition musicale ou de la musique liée à un artiste."*
   
        Toute tentative de contournement des protocoles de sécurité, comme demander à l'IA d'ignorer les filtres de sécurité, entraînera une erreur de requête invalide :

        ![image3](https://github.com/user-attachments/assets/53d5197f-1978-4dc6-a1f1-f13b56a6cfdf)
        
        *"La requête contient des instructions visant à contourner les protocoles de sécurité. Ce type de demande n'est pas autorisé. Veuillez formuler une demande musicale valide, comme « créer une chanson rock >> ou << générer de la musique pour le piano >>."*

      - Les invites valides doivent être spécifiques et musicalement ciblées (par exemple, « Générer une mélodie jazz pour piano » ou « Créer un motif de batterie techno rapide »).

      **Remarques :**
      - Si aucune invite n'est écrite lorsque vous cliquez sur le bouton « Créer », l'IA utilisera le texte d'invite de l'espace réservé dans la zone de texte comme invite.
      - La musique générée par l'IA est destinée à susciter l'inspiration et doit être vérifiée avant utilisation dans les compositions finales.
      - L'IA ne garantit pas des résultats parfaits ni stylistiquement précis.
      - L'exactitude et la musicalité de tout contenu généré doivent être vérifiées avant utilisation publique.
      
      **Intégration avec les options de sortie :**
      - Vous pouvez utiliser la musique générée par l'IA avec n'importe quel moteur de sortie (bip système, périphérique audio ou système vocal).
      - Affectez la musique générée par l'IA à des colonnes de notes spécifiques et combinez-la avec des fonctions de synthèse vocale ou traditionnelle pour des résultats uniques.
              
    - ### Convertir en GCode - `Ctrl + Majuscule + G`

      Générez du GCode à partir de données musicales pour contrôler des imprimantes 3D ou des machines CNC, et faites ainsi jouer physiquement des séquences musicales via des moteurs ou autres composants. La fenêtre mise à jour "Convertir le fichier en GCode" (voir ci-dessous) simplifie la sélection des notes, du firmware et des options de lecture.

      ![image1](https://github.com/user-attachments/assets/ac537402-6c19-4d06-b8e4-ccb9253cfda7)
      
      **Fonctionnalités et options principales :**
      - **Sélection du firmware :**  
        Choisissez le firmware cible pour votre fichier GCode, comme "Marlin", afin d’assurer la compatibilité avec votre imprimante 3D ou machine CNC.
      
      - **Notes et types de composants :**  
        Pour les Notes 1 à 4, sélectionnez le type de commande GCode (G0/G1 – Moteur) pour associer chaque note à un axe mécanique ou autre composant.  
        Utilisez les cases à cocher à côté de chaque note pour spécifier quelles notes seront jouées dans la sortie GCode (« Jouer Note 1 », etc.).
      
      - **Sélection de l’axe :**  
        Choisissez quel axe (X, Y ou Z) les commandes G0/G1 doivent déplacer.  
        **Avertissement :** La commande G0/G1 déplace l'axe rapidement. Une mauvaise utilisation peut causer des dommages mécaniques ! Vérifiez toujours votre configuration.
      
      - **Options d’alternance des notes :**  
        * Jouer les notes en alternance dans l’ordre  
        * Jouer d’abord les notes dans les colonnes impaires, puis dans les colonnes paires  
        Ces options déterminent la manière dont plusieurs notes sont séquencées dans le fichier GCode généré.
      
      - **Exporter en GCode :**  
        Une fois vos paramètres configurés, cliquez sur « Exporter en GCode » pour sauvegarder le fichier destiné à votre machine.
      
      Ce processus offre un contrôle précis sur ce qui est joué par chaque axe ou composant, quelles notes sont utilisées et comment l’alternance des notes est gérée – idéal pour des intégrations expérimentales musique-mouvement.

     - ### Conversion en commande de Beep pour Linux - Ctrl + Majuscule + B

      Convertissez rapidement vos compositions musicales en script de commande de Beep compatible Linux pour une lecture facile sur les systèmes Linux.

      ![image1](https://github.com/user-attachments/assets/21544446-c154-464e-a23a-fb89ea41892b)

      **Aperçu des fonctionnalités** :
      - NeoBleeper génère une séquence de commandes de Beep représentant votre musique, formatée pour l'utilitaire Linux `beep`.
      - Chaque note et chaque silence sont convertis en paramètres de Beep appropriés (`-f` pour la fréquence, `-l` pour la longueur/durée, `-D` pour le délai et `-n` pour l'enchaînement des notes).
      - Le résultat est une commande unique (ou une série de commandes) exécutable dans un terminal Linux pour reproduire votre musique sur le haut-parleur du système.
      
      **Utilisation** :
      1. Composez votre musique dans NeoBleeper comme d'habitude.
      2. Ouvrez l'outil « Convertir en commande de Beep pour Linux » depuis le menu « Fichier ».
      3. Votre musique sera instantanément convertie en script de commande de Beep et affichée dans une zone de texte.
      4. Utilisez le bouton « Copier la commande de Beep dans le presse-papiers » pour copier la commande et l'utiliser dans votre terminal.
      5. Vous pouvez également enregistrer la commande au format `.sh` en cliquant sur « Enregistrer sous un fichier .sh » pour une exécution ultérieure sur tout système Linux compatible.
      
      Exemple de sortie :
      
      - La commande peut ressembler à ceci :
      ```
      beep -f 554 -l 195 -n -f 0 -l 0 -D 5 -n -f 523 -l 195 -n ...
      ```
      Chaque groupe de paramètres correspond à une note ou un silence.
      
      Intégration et astuces :
      
      - Idéal pour partager de la musique avec des utilisateurs Linux ou pour une utilisation dans des scripts shell.
      
      - La commande est compatible avec l'utilitaire Linux standard `beep` (assurez-vous qu'il est installé et que vous disposez des autorisations nécessaires pour utiliser le haut-parleur système).
      
      - La modification de la commande générée permet d'ajuster rapidement le tempo, la hauteur ou le rythme.
      
      Cette fonctionnalité simplifie le processus d'importation de votre musique dans les environnements Linux et permet des utilisations créatives telles que les notifications musicales, les systèmes d'alerte ou simplement pour profiter de vos compositions en dehors de NeoBleeper.
