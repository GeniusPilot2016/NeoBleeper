# Guida alla risoluzione dei problemi di NeoBleeper

Questa guida fornisce soluzioni ai problemi più comuni riscontrati durante l'utilizzo di NeoBleeper, in particolare quelli relativi al comportamento degli altoparlanti di sistema, all'uscita audio, alla compatibilità hardware e al segnale acustico persistente del sistema.

---

## 1. Suono bloccato nell'altoparlante di sistema dopo un arresto anomalo o una chiusura forzata

**Problema:**
Se NeoBleeper si blocca o viene chiuso forzatamente mentre l'audio è in riproduzione tramite l'altoparlante di sistema (PC), il suono potrebbe "bloccarsi", causando un segnale acustico o un ronzio continuo.

**Perché accade:**
L'altoparlante di sistema è controllato a un livello hardware/software basso. Se l'applicazione non rilascia o ripristina correttamente l'altoparlante all'uscita, il suono potrebbe persistere.

**Soluzioni:**
- **Utilizzare l'utilità NeoBleeper Beep Stopper:**
  NeoBleeper include uno strumento chiamato "NeoBleeper Beep Stopper" nella cartella del programma.
  
  ![image4](https://github.com/user-attachments/assets/03a875b0-7ac3-4e53-a2c7-9a702d9dccb3)
  
  - Avviare questo strumento e premere il pulsante **Stop Beep** per interrompere il segnale acustico proveniente dall'altoparlante di sistema.
  - Utilizzare questa utilità solo quando il segnale acustico continua dopo un arresto anomalo o una chiusura forzata.
  
  #### Messaggi di Beep Stopper e loro significato
  
  Quando si utilizza l'utilità Beep Stopper, potrebbero essere visualizzati i seguenti messaggi:

  ![image1](https://github.com/user-attachments/assets/d05d5a4c-1688-4a69-b9c0-234f880d8c95)
    
    **L'altoparlante di sistema non emette alcun segnale acustico oppure emette un segnale acustico diverso. Nessuna azione intrapresa.**  
    Questo messaggio appare quando l'utilità controlla l'altoparlante di sistema e rileva che non produce alcun segnale acustico o che il segnale acustico emesso non può essere controllato dallo strumento. In questo caso, Beep Stopper non intraprenderà ulteriori azioni.
    - *Suggerimento:* se il segnale acustico persiste, prova a riavviare il computer.
  
  ![image2](https://github.com/user-attachments/assets/6c22a450-597b-45a2-b55b-d3bdd97af712)
    
    **Il segnale acustico è stato interrotto correttamente!**  
    Questo messaggio conferma che l'utilità Beep Stopper ha rilevato un segnale acustico bloccato ed è riuscita a interromperlo correttamente. Non sono necessarie ulteriori azioni.
  
  ![image3](https://github.com/user-attachments/assets/9dde02ea-c904-4cf5-bc2f-b6d425aa911c)
  
    **L'uscita dell'altoparlante di sistema non è presente oppure è presente un'uscita dell'altoparlante di sistema non standard. Il blocco del segnale acustico potrebbe causare instabilità o comportamenti indesiderati. Vuoi continuare?**
    Questo messaggio viene visualizzato quando l'utilità Beep Stopper viene avviata e rileva che il sistema non dispone di un altoparlante di sistema standard (PC) oppure che l'uscita dell'altoparlante di sistema è "non standard". In questo caso, l'utilità avvisa che il tentativo di utilizzare Beep Stopper potrebbe non funzionare come previsto e potrebbe potenzialmente causare comportamenti imprevisti o instabilità.
    
    Se si procede, lo strumento tenterà di interrompere il segnale acustico, ma potrebbe essere inefficace o avere effetti collaterali se l'hardware non è supportato o non è standard.
    Se si sceglie di non continuare, lo strumento si chiuderà senza apportare modifiche.
    - *Suggerimento:* se si riceve questo messaggio, significa che il computer non dispone di un altoparlante di sistema compatibile oppure che la sua uscita non può essere controllata in modo affidabile. Qualsiasi segnale acustico o ronzio udito proviene probabilmente da un altro dispositivo audio (come gli altoparlanti principali o le cuffie). Utilizza le impostazioni audio standard del dispositivo per risolvere i problemi audio e chiudi tutte le applicazioni che potrebbero produrre audio indesiderato. Se il problema persiste, prova a riavviare il computer o a controllare le impostazioni audio del dispositivo.

- **Riavvia il computer:**
Se il Beep Stopper non risolve il problema, un riavvio del sistema ripristinerà l'hardware degli altoparlanti.

- **Prevenzione:**
Chiudi sempre NeoBleeper normalmente. Evita di forzarne la chiusura tramite Gestione Attività o strumenti simili mentre l'audio è in riproduzione.

---

## 2. Rilevamento e compatibilità degli altoparlanti di sistema

NeoBleeper include una logica di rilevamento per verificare se il sistema dispone di un'uscita standard per altoparlanti PC, nonché il supporto per uscite di altoparlanti di sistema "nascoste" (come quelle che non utilizzano l'ID PNP0800). Se l'hardware non supporta un altoparlante di sistema standard o nascosto, o l'uscita non è standard e non utilizzabile, potrebbero essere visualizzati messaggi di avviso o potrebbe essere necessario utilizzare il dispositivo audio standard per i segnali acustici. Tuttavia, a partire dalle versioni più recenti, NeoBleeper non obbliga più a utilizzare esclusivamente il dispositivo audio in assenza di un altoparlante standard: ora consente l'utilizzo di uscite di altoparlanti di sistema nascoste/non PNP0800, se presenti.

### Esempio di avviso (immagine 1):

![immagine1](https://github.com/user-attachments/assets/dd2a6d88-a4e3-46fd-bbce-a573be564c8e)

> **Spiegazione:**
> La scheda madre del computer non dispone di un'uscita standard per gli altoparlanti di sistema oppure l'uscita non è standard. NeoBleeper tenterà di rilevare e offrire l'utilizzo di uscite "nascoste" per gli altoparlanti di sistema non identificate come PNP0800. Se tale uscita è disponibile, è possibile utilizzare l'altoparlante di sistema anche se viene visualizzato questo avviso. In caso contrario, NeoBleeper utilizzerà il dispositivo audio standard (come altoparlanti o cuffie).

### Finestre di dialogo delle impostazioni (immagini 2 e 3):

![image2](https://github.com/user-attachments/assets/14878c30-d680-4b08-8100-933a8f2bcc6b)

![image3](https://github.com/user-attachments/assets/795e55a7-fa8d-4149-96e7-d52f4a213149)

- **Disponibilità del pulsante "Test altoparlante di sistema":**
Questa opzione è abilitata se NeoBleeper rileva un'uscita altoparlante di sistema utilizzabile, incluse uscite nascoste o non PNP0800.
- **Impostazione "Usa dispositivo audio per creare un segnale acustico":**
Ora è possibile disabilitare questa funzione se viene rilevata un'uscita altoparlante di sistema nascosta o non standard.

#### Cosa significa "uscita altoparlante di sistema non standard"? 
Alcuni computer, laptop o macchine virtuali moderni non dispongono di un vero altoparlante per PC, oppure il routing del segnale non è standard. NeoBleeper ora tenta di rilevare e utilizzare tali uscite nascoste degli altoparlanti di sistema (non identificate come dispositivi PNP0800), ma può abilitare l'opzione degli altoparlanti di sistema solo se è effettivamente accessibile a livello hardware. Se non viene trovata alcuna uscita utilizzabile, sarà necessario utilizzare il dispositivo audio standard.

---

## 3. Come verificare la presenza dell'altoparlante di sistema

- **Computer desktop:** La maggior parte dei desktop più vecchi ha un connettore per l'altoparlante del PC sulla scheda madre. I sistemi più recenti potrebbero non avere questa funzionalità o presentare l'uscita in un formato nascosto/non PNP0800 che NeoBleeper ora può utilizzare.
- **Laptop:** La maggior parte dei laptop non ha un altoparlante di sistema separato; tutto l'audio viene instradato attraverso il sistema audio principale.
- **Macchine virtuali:** L'emulazione dell'altoparlante di sistema è spesso assente o inaffidabile; le uscite non PNP0800 potrebbero non essere disponibili.
- **Come riconoscerlo:** Se visualizzi gli avvisi sopra riportati ma riesci ad abilitare e testare l'altoparlante di sistema in NeoBleeper, è probabile che il tuo computer abbia un'uscita nascosta o non standard.
  
---

## 2.1 Test di uscita degli altoparlanti di sistema (rilevamento della frequenza ultrasonica)

NeoBleeper ora include un nuovo test hardware avanzato per rilevare l'uscita degli altoparlanti di sistema (ovvero degli altoparlanti del PC), anche se il dispositivo non è segnalato da Windows (con determinati ID come PNP0C02 anziché PNP0800). Questo test utilizza frequenze ultrasoniche (in genere 30-38 kHz, non udibili) e analizza il feedback elettrico sulla porta degli altoparlanti di sistema.

- **Come funziona:**
  Durante l'avvio, NeoBleeper esegue una seconda fase dopo il consueto controllo dell'ID del dispositivo. Invia segnali ultrasonici alla porta degli altoparlanti di sistema e monitora il feedback hardware per rilevare la presenza di un'uscita altoparlante funzionante, anche se nascosta o non standard.

- **Cosa potresti notare:**
  Su alcuni sistemi, in particolare quelli con buzzer piezoelettrici, potresti udire lievi clic durante questa fase. Questo è normale e indica che il test hardware è in esecuzione.

  ![image4](https://github.com/user-attachments/assets/2805e881-8a83-4563-8985-784a04be5147)
  
  *Verifica della presenza dell'uscita dell'altoparlante di sistema (altoparlante del PC) nel passaggio 2/2... (potresti sentire dei clic)*

- **Perché questo test?**
  Molti sistemi moderni non dispongono di un dispositivo altoparlante di sistema PNP0800, ma dispongono comunque di un'uscita altoparlante utilizzabile (nascosta). NeoBleeper utilizza questo metodo avanzato per abilitare le funzionalità di segnale acustico su più hardware.

## 4. Non sento alcun suono!

- **Controlla le impostazioni di NeoBleeper:**
  Se l'altoparlante di sistema non è disponibile, assicurati che il dispositivo audio (altoparlanti/cuffie) sia selezionato correttamente e funzionante.
- **Controlla il mixer del volume di Windows:**
  Assicurati che NeoBleeper non sia disattivato nel mixer del volume di sistema.
- **Prova il pulsante "Test altoparlante di sistema":**
  Usalo per testare l'altoparlante del PC.
- **Leggi i messaggi di avviso:**
  NeoBleeper fornirà istruzioni specifiche se non riesce ad accedere all'altoparlante di sistema.

---

## 5. Domande frequenti

### D: Posso utilizzare l'altoparlante di sistema se il mio hardware non dispone di un dispositivo PNP0800?
**R:** Sì! NeoBleeper ora tenta di rilevare e utilizzare le uscite degli altoparlanti di sistema nascoste o non PNP0800, ove possibile. In caso di successo, è possibile utilizzare l'altoparlante di sistema anche se Windows non segnala un dispositivo standard.

### D: Perché l'impostazione "Usa dispositivo audio per creare un segnale acustico" a volte diventa permanente (nelle versioni precedenti)?
**R:** Quando non viene rilevata alcuna uscita standard per gli altoparlanti di sistema (nelle versioni precedenti), NeoBleeper applica questa impostazione per garantire che l'uscita audio sia ancora possibile.

### D: Esiste una soluzione alternativa per l'altoparlante di sistema mancante?
**R:** È necessario utilizzare il dispositivo audio abituale (altoparlanti/cuffie) se non viene rilevata un'uscita standard per gli altoparlanti di sistema (nelle versioni precedenti).

### D: Cosa succede se lo strumento Beep Stopper non interrompe il segnale acustico bloccato?
**R:** Riavvia il computer per ripristinare l'hardware degli altoparlanti se l'utilità Beep Stopper non funziona.

### D: Perché sento dei clic durante l'avvio?
**R:** Durante il test avanzato dell'uscita degli altoparlanti di sistema (fase 2), NeoBleeper invia segnali a ultrasuoni all'hardware per rilevare uscite nascoste o non standard degli altoparlanti. Su alcuni sistemi (in particolare quelli con buzzer piezoelettrici), ciò potrebbe causare lievi clic. Questo è normale e non indica un problema; significa semplicemente che il test hardware è in esecuzione.

### D: Il test hardware a ultrasuoni (fase 2) può rilevare altoparlanti di sistema rotti (circuito aperto) o scollegati?
**R:** Questa funzionalità non è attualmente testata e non è nota. Sebbene il test verifichi il feedback elettrico e l'attività delle porte, potrebbe non distinguere in modo affidabile tra un altoparlante fisicamente presente ma rotto (circuito aperto) o scollegato e un altoparlante mancante. Se l'altoparlante è completamente rotto o scollegato (circuito aperto), il test potrebbe restituire un risultato falso, indicando che non è stata rilevata alcuna uscita funzionale. Tuttavia, questo comportamento non è garantito e potrebbe dipendere dall'hardware specifico e dalla modalità di errore. Se si sospetta che l'altoparlante del sistema non funzioni, si consiglia un'ispezione fisica o l'utilizzo di un multimetro.

**Potenziali aggiornamenti futuri:**
Se futuri test o sviluppi consentiranno a NeoBleeper di rilevare in modo affidabile gli altoparlanti del sistema rotti o scollegati tramite il test hardware a ultrasuoni, queste FAQ e la logica di rilevamento verranno aggiornate per riflettere tali miglioramenti. Per maggiori dettagli, consultare i changelog o le nuove versioni.

---

## 6. Ottenere assistenza

- **Fornire dettagli sul computer e sull'ambiente:** Quando si segnalano problemi di rilevamento hardware o audio, si prega di includere dettagli sul computer (desktop/laptop, produttore/modello, sistema operativo) e su eventuali componenti hardware rilevanti.
- **Allegare screenshot o finestre di dialogo di errore:** Gli screenshot di finestre di dialogo di errore o avviso sono molto utili. Specificare esattamente quando si verifica il problema.
- **Includere il file di registro:** A partire dalle versioni più recenti, NeoBleeper crea un file di registro dettagliato denominato `DebugLog.txt` nella cartella del programma. Si prega di allegare questo file quando si richiede assistenza, poiché contiene utili informazioni diagnostiche.
- **Descrivere i passaggi per riprodurre il problema:** Descrivere chiaramente cosa si stava facendo quando si è verificato il problema.
- **Aprire un problema su GitHub:** Per ulteriore assistenza, aprire un problema su GitHub e includere tutti i dettagli sopra indicati per un supporto ottimale.

_Questa guida viene aggiornata man mano che vengono scoperti nuovi problemi e soluzioni. Per ulteriore assistenza, apri un problema su GitHub con informazioni dettagliate sulla tua configurazione e sul problema riscontrato.
