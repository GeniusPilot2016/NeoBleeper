# Guide de dépannage de NeoBleeper

Ce guide propose des solutions aux problèmes courants rencontrés lors de l'utilisation de NeoBleeper, notamment ceux liés au comportement des haut-parleurs système, à la sortie audio, à la compatibilité matérielle et aux bips système persistants.

---

## 1. Son bloqué dans le haut-parleur système après un plantage ou une fermeture forcée

**Problème** :
Si NeoBleeper plante ou est fermé de force alors que le son est diffusé par le haut-parleur système (PC), le son peut se bloquer et produire des bips ou des bourdonnements continus.

**Raison :**
Le haut-parleur système est contrôlé à un niveau matériel/logiciel bas. Si l'application ne libère pas ou ne réinitialise pas correctement le haut-parleur à la fermeture, le bip peut persister.

**Solutions** :
- **Utiliser l'utilitaire Arrêt du bip NeoBleeper :**
  NeoBleeper est fourni avec un outil appelé « Arrêt du bip NeoBleeper » dans le dossier du programme.

  ![image4](https://github.com/user-attachments/assets/fbf24bb4-5490-412e-bd03-d90acd11d483)
  
  - Lancez cet outil et appuyez sur le bouton **Arrêter le bip** pour arrêter le bip sonore bloqué du haut-parleur système.
  - N'utilisez cet utilitaire que si le bip persiste après un plantage ou un arrêt forcé.

  #### Messages de l'utilitaire Beep Stopper et leur signification

  Lorsque vous utilisez l'utilitaire Beep Stopper, les messages suivants peuvent s'afficher :

  ![image1](https://github.com/user-attachments/assets/5a80254f-abba-4072-a70e-6c625f87601a)
    
    **Le haut-parleur système n'émet pas de bip ou son bip est émis différemment. Aucune action n'a été effectuée.**  
    Ce message apparaît lorsque l'utilitaire vérifie le haut-parleur système et constate qu'il n'émet aucun bip, ou qu'il émet des bips d'une manière qui ne peut être contrôlée par l'outil. Dans ce cas, le bip stopper n'intervient pas.
    - *Conseil:* Si vous entendez toujours un bip persistant, essayez de redémarrer votre ordinateur.

  ![image2](https://github.com/user-attachments/assets/01611036-50ec-4730-b724-5f0ece8a9ce4)
    
    **Le bip sonore a été arrêté!**  
    Ce message confirme que l'utilitaire Beep Stopper a détecté un bip bloqué et a réussi à l'arrêter. Aucune autre action n'est requise.

  ![image3](https://github.com/user-attachments/assets/00107703-8527-4a26-ab02-1c530492a083)
  
    **La sortie des haut-parleurs système est absente ou présente une sortie non standard. L'arrêt du bip sonore peut entraîner une instabilité ou des comportements indésirables. Voulez-vous continuer ?**  
    Ce message apparaît lorsque l'utilitaire Beep Stopper est lancé et détecte que votre système ne dispose pas de haut-parleur standard (PC) ou que la sortie de ce dernier est « non standard ». Dans ce cas, l'utilitaire vous avertit que l'utilisation de Beep Stopper pourrait ne pas fonctionner comme prévu et entraîner un comportement inattendu ou une instabilité.

    Si vous continuez, l'outil tentera d'arrêter le bip, mais cela pourrait être inefficace ou avoir des effets secondaires si votre matériel n'est pas pris en charge ou non standard.
    Si vous choisissez d'abandonner, l'outil se fermera sans effectuer de modifications.
    - *Conseil:* Si vous recevez ce message, cela signifie que votre ordinateur ne dispose pas de haut-parleur système compatible ou que sa sortie ne peut pas être contrôlée de manière fiable. Tout bip ou bourdonnement que vous entendez provient probablement d'un autre périphérique audio (comme vos haut-parleurs principaux ou votre casque). Utilisez les paramètres audio standard de votre périphérique pour résoudre les problèmes de son et fermez toutes les applications susceptibles de produire du son indésirable. Si le problème persiste, essayez de redémarrer votre ordinateur ou de vérifier les paramètres audio de votre appareil.
  
- **Redémarrez votre ordinateur** :
  Si le bip stop ne résout pas le problème, un redémarrage du système réinitialisera le matériel des haut-parleurs.

- **Prévention** :
  Fermez toujours NeoBleeper normalement. Évitez de forcer sa fermeture via le Gestionnaire des tâches ou un outil similaire pendant la lecture du son.

---

## 2. Détection et compatibilité des haut-parleurs système

NeoBleeper inclut une logique de détection permettant de vérifier si votre système dispose d'une sortie haut-parleur PC standard, ainsi que la prise en charge des sorties haut-parleurs système « cachées » (comme celles n'utilisant pas l'identifiant PNP0800). Si votre matériel ne prend pas en charge un haut-parleur système standard ou caché, ou si la sortie est non standard et inutilisable, vous pourriez recevoir des messages d'avertissement ou devoir utiliser votre périphérique audio habituel pour les bips. Cependant, depuis les versions récentes, NeoBleeper ne vous oblige plus à utiliser exclusivement le périphérique audio en l'absence de haut-parleur standard ; il permet désormais l'utilisation des sorties haut-parleurs système cachées/non PNP0800, le cas échéant.

### Exemple d'avertissement (Image 1) :

  ![image1](https://github.com/user-attachments/assets/b771b3dd-17f5-4ea2-8608-094a158b9b40)
  
  > **Explication :**
  > La carte mère de votre ordinateur ne dispose pas de sortie haut-parleur standard, ou la sortie est non standard. NeoBleeper tentera de détecter et de proposer l'utilisation de sorties haut-parleurs système « cachées » non identifiées comme PNP0800. Si une telle sortie est disponible, vous pouvez utiliser le haut-parleur système même si cet avertissement s'affiche. Sinon, NeoBleeper utilisera votre périphérique audio habituel (comme des enceintes ou un casque).

### Boîtes de dialogue Paramètres (Images 2 et 3) :

  ![image2](https://github.com/user-attachments/assets/f232fc11-bbc8-4570-a33b-ef30f9cdc6cc)
  
  ![image3](https://github.com/user-attachments/assets/6796e215-5cf6-4b9d-89cf-113f3b52db19)

- **Disponibilité du bouton « Tester le haut-parleur système »** :

  Cette option est activée si NeoBleeper détecte une sortie de haut-parleur système utilisable, y compris les sorties masquées ou non PNP0800.

- **Paramètre « Utiliser un périphérique audio pour créer un bip »** :

  Vous pouvez désormais désactiver cette fonctionnalité si une sortie de haut-parleur système masquée ou non standard est détectée.

#### Que signifie « sortie de haut-parleur système non standard » ? 
  Certains ordinateurs, ordinateurs portables ou machines virtuelles modernes ne disposent pas de véritable haut-parleur PC, ou le routage du signal est non standard. NeoBleeper tente désormais de détecter et d'utiliser ces sorties de haut-parleurs système cachées (non identifiées comme périphériques PNP0800), mais ne peut activer l'option haut-parleur système que si elle est accessible au niveau matériel. Si aucune sortie utilisable n'est trouvée, vous devrez utiliser votre périphérique audio habituel.

---

## 3. Comment vérifier la présence d'un haut-parleur système

- **Ordinateurs de bureau** : La plupart des anciens ordinateurs de bureau sont équipés d'un connecteur pour haut-parleur PC sur la carte mère. Les systèmes plus récents peuvent omettre cette fonctionnalité ou afficher la sortie sous une forme masquée/non PNP0800 que NeoBleeper peut désormais utiliser.
- **Ordinateurs portables** : La plupart des ordinateurs portables ne disposent pas de haut-parleur système séparé ; tout le son est acheminé via le système audio principal.
- **Machines virtuelles** : L'émulation des haut-parleurs système est souvent absente ou peu fiable ; les sorties non PNP0800 peuvent être indisponibles.
- **Comment le savoir** : Si vous voyez les avertissements ci-dessus, mais que vous parvenez à activer et tester le haut-parleur système dans NeoBleeper, votre ordinateur dispose probablement d'une sortie masquée ou non standard.
  
---

## 2.1 Test de sortie des haut-parleurs système (détection des fréquences ultrasoniques)

  NeoBleeper intègre désormais un nouveau test matériel avancé permettant de détecter la sortie des haut-parleurs système (également appelés haut-parleurs PC), même si le périphérique n'est pas signalé par Windows (avec certains identifiants comme PNP0C02 au lieu de PNP0800). Ce test utilise des fréquences ultrasoniques (généralement 30 à 38 kHz, inaudibles) et analyse le retour électrique sur le port du haut-parleur système.

- **Fonctionnement** :

  Au démarrage, NeoBleeper effectue une deuxième étape après la vérification habituelle de l'identifiant du périphérique. Il envoie des signaux ultrasoniques au port du haut-parleur système et surveille le retour matériel afin de détecter la présence d'une sortie de haut-parleur fonctionnelle, même masquée ou non standard.

- **Remarques possibles** :

  Sur certains systèmes, notamment ceux équipés de buzzers piézoélectriques, vous pouvez entendre de légers clics pendant cette étape. Ce phénomène est normal et indique que le test matériel est en cours.

  ![image4](https://github.com/user-attachments/assets/62e09438-ed58-4ce3-915f-95e095b293de)
  
  *Vérification de la présence du haut-parleur système (Haut-parleur PC) à l'étape 2/2... (Vous pourriez entendre des clics)*

- **Pourquoi ce test ?**

  De nombreux systèmes modernes ne sont pas équipés d'un haut-parleur PNP0800, mais disposent tout de même d'une sortie haut-parleur utilisable (masquée). NeoBleeper utilise cette méthode avancée pour activer les fonctions de bip sur davantage de matériel.

## 4. Je n'entends aucun son !

- **Vérifiez les paramètres de NeoBleeper :**
  Si le haut-parleur de votre système n'est pas disponible, assurez-vous que votre périphérique audio (haut-parleurs/casque) est correctement sélectionné et fonctionnel.
- **Vérifiez le mixeur de volume Windows :**
  Assurez-vous que NeoBleeper n'est pas coupé dans le mixeur de volume système.
- **Essayez le bouton « Tester le haut-parleur système » :**
  Utilisez-le pour tester le haut-parleur de votre PC.
- **Lisez les messages d'avertissement :**
  NeoBleeper vous fournira des instructions spécifiques s'il ne peut pas accéder au haut-parleur de votre système.

---

## 5. Foire aux questions

### Q : Puis-je utiliser le haut-parleur système si mon matériel ne possède pas de périphérique PNP0800 ?
**R :** Oui ! NeoBleeper tente désormais de détecter et d’utiliser les sorties de haut-parleur système masquées ou non PNP0800, lorsque cela est possible. En cas de succès, vous pouvez utiliser le haut-parleur système même si Windows ne signale pas de périphérique standard.

### Q : Pourquoi le paramètre « Utiliser un périphérique audio pour créer un bip » devient-il parfois permanent (dans les anciennes versions) ?
**R :** Lorsqu’aucune sortie de haut-parleur système standard n’est détectée (dans les anciennes versions), NeoBleeper applique ce paramètre pour garantir que la sortie audio reste possible.

### Q : Existe-t-il une solution de contournement pour l’absence de haut-parleur système ?
**R :** Vous devez utiliser votre périphérique audio habituel (haut-parleurs/casque) si aucune sortie de haut-parleur système standard n’est trouvée (dans les anciennes versions).

### Q : Que faire si l’outil Stopper de bip ne supprime pas le bip bloqué ? **R :** Redémarrez votre ordinateur pour réinitialiser les haut-parleurs si l'utilitaire Beep Stopper échoue.

### Q : Pourquoi entends-je des clics au démarrage ?
**R :** Lors du test avancé des sorties des haut-parleurs système (étape 2), NeoBleeper envoie des signaux ultrasoniques au matériel pour détecter les sorties de haut-parleurs masquées ou non standard. Sur certains systèmes (notamment ceux équipés de buzzers piézoélectriques), cela peut provoquer de légers clics. Ce phénomène est normal et n'indique aucun problème ; cela signifie simplement que le test matériel est en cours d'exécution.

### Q : Le test matériel par ultrasons (étape 2) peut-il détecter des haut-parleurs système défectueux (circuit ouvert) ou déconnectés ?
**R :** Ce test n'a pas encore été testé et est inconnu. Bien que le test vérifie la rétroaction électrique et l'activité des ports, il peut ne pas faire la distinction entre un haut-parleur physiquement présent mais défectueux (circuit ouvert) ou déconnecté et un haut-parleur absent. Si le haut-parleur est complètement défectueux ou déconnecté (circuit ouvert), le test peut renvoyer un résultat erroné, indiquant qu'aucune sortie fonctionnelle n'est détectée. Cependant, ce comportement n'est pas garanti et peut dépendre du matériel et du mode de défaillance. Si vous pensez que votre haut-parleur système ne fonctionne pas, une inspection physique ou l'utilisation d'un multimètre est recommandée.

**Mises à jour potentielles** :
Si des tests ou développements futurs permettent à NeoBleeper de détecter de manière fiable les haut-parleurs système défectueux ou déconnectés grâce au test matériel par ultrasons, cette FAQ et la logique de détection seront mises à jour pour refléter ces améliorations. Consultez les journaux des modifications ou les nouvelles versions pour plus de détails.

---

## 6. Obtenir de l'aide

- **Fournir des informations sur l'ordinateur et son environnement** : Lorsque vous signalez des problèmes de détection de matériel ou de son, veuillez inclure des informations sur votre ordinateur (ordinateur de bureau/portable, fabricant/modèle, système d'exploitation) et tout matériel concerné.
- **Joindre des captures d'écran ou des messages d'erreur** : Les captures d'écran des messages d'erreur ou d'avertissement sont très utiles. Précisez précisément quand le problème survient.
- **Inclure le fichier journal** : À partir des versions plus récentes, NeoBleeper crée un fichier journal détaillé appelé « DebugLog.txt » dans le dossier du programme. Veuillez joindre ce fichier lorsque vous demandez de l'aide, car il contient des informations de diagnostic utiles.
- **Décrire les étapes pour reproduire le problème** : Décrivez clairement ce que vous faisiez lorsque le problème est survenu.
- **Ouvrir un ticket sur GitHub** : Pour obtenir de l'aide, ouvrez un ticket sur GitHub et fournissez tous les détails ci-dessus pour une assistance optimale.

_Ce guide est mis à jour au fur et à mesure que de nouveaux problèmes et solutions sont découverts. Pour obtenir de l'aide, veuillez ouvrir un problème sur GitHub avec des informations détaillées sur votre configuration et le problème rencontré._
