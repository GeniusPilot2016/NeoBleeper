# Guida alla risoluzione dei problemi di NeoBleeper

Questa guida fornisce soluzioni ai problemi più comuni riscontrati durante l'utilizzo di NeoBleeper, in particolare quelli relativi al comportamento degli altoparlanti di sistema, all'uscita audio, alla compatibilità hardware e al segnale acustico persistente del sistema.

---

## 1. Suono bloccato nell'altoparlante di sistema dopo un arresto anomalo o una chiusura forzata

**Problema:**
Se NeoBleeper si blocca o viene chiuso forzatamente mentre l'audio è in riproduzione tramite l'altoparlante di sistema (PC), il suono potrebbe "bloccarsi", causando un segnale acustico o un ronzio continuo.

**Perché accade:**
L'altoparlante di sistema è controllato a un livello hardware/software basso. Se l'applicazione non rilascia o ripristina correttamente l'altoparlante all'uscita, il suono potrebbe persistere.

**Soluzioni:**
- **Utilizzare l'utilità NeoBleeper Beep Stopper (per la versione a 64 bit):**
  NeoBleeper include uno strumento chiamato "NeoBleeper Beep Stopper" nella cartella del programma.
  
  ![image4](https://github.com/user-attachments/assets/03a875b0-7ac3-4e53-a2c7-9a702d9dccb3)
  
  - Avviare questo strumento e premere il pulsante **Stop Beep** per interrompere il segnale acustico proveniente dall'altoparlante di sistema.
  - Utilizzare questa utilità solo quando il segnale acustico continua dopo un arresto anomalo o una chiusura forzata.
  
  #### Messaggi di Beep Stopper e loro significato
  
  Quando si utilizza l'utilità Beep Stopper, potrebbero essere visualizzati i seguenti messaggi:

  ![image1](https://github.com/user-attachments/assets/68ac83df-f644-4d0f-a759-b53d5a6e9653)
    
    **L'altoparlante di sistema non emette alcun segnale acustico oppure emette un segnale acustico diverso. Nessuna azione intrapresa.**  
    Questo messaggio appare quando l'utilità controlla l'altoparlante di sistema e rileva che non produce alcun segnale acustico o che il segnale acustico emesso non può essere controllato dallo strumento. In questo caso, Beep Stopper non intraprenderà ulteriori azioni.
    - *Suggerimento:* se il segnale acustico persiste, prova a riavviare il computer.

  ![image2](https://github.com/user-attachments/assets/12d6faa1-695e-42da-9586-829348db0b70)
    
    **Il segnale acustico è stato interrotto correttamente!**  
    Questo messaggio conferma che l'utilità Beep Stopper ha rilevato un segnale acustico bloccato ed è riuscita a interromperlo correttamente. Non sono necessarie ulteriori azioni.

  ![image3](https://github.com/user-attachments/assets/ad9c8e3e-28f8-488e-abb5-b9da2973f67b)
  
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

![image2](https://github.com/user-attachments/assets/68cd956b-0089-4ca8-93c8-3ccfbec541e0)

![image3](https://github.com/user-attachments/assets/ed586a9e-31c3-491f-b3ee-3dee2bc829a9)

- **Disponibilità del pulsante "Test altoparlante di sistema":**
Questa opzione è abilitata se NeoBleeper rileva un'uscita altoparlante di sistema utilizzabile, incluse uscite nascoste o non PNP0800.
- **Impostazione "Usa dispositivo audio per creare un segnale acustico":**
Ora è possibile disabilitare questa funzione se viene rilevata un'uscita altoparlante di sistema nascosta o non standard.

#### Cosa significa "uscita altoparlante di sistema non standard"? 
Alcuni computer, laptop o macchine virtuali moderni non dispongono di un vero altoparlante per PC, oppure il routing del segnale non è standard. NeoBleeper ora tenta di rilevare e utilizzare tali uscite nascoste degli altoparlanti di sistema (non identificate come dispositivi PNP0800), ma può abilitare l'opzione degli altoparlanti di sistema solo se è effettivamente accessibile a livello hardware. Se non viene trovata alcuna uscita utilizzabile, sarà necessario utilizzare il dispositivo audio standard.

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

---

## 3. Supporto e limitazioni di ARM64

**Dispositivi basati su ARM64:**
Sui sistemi Windows ARM64, il test "Altoparlante di sistema" e la casella di controllo "Usa dispositivo audio per creare segnali acustici" **non sono disponibili** in NeoBleeper. Tutti i segnali acustici e le uscite audio vengono sempre riprodotti tramite il dispositivo audio standard (altoparlanti o cuffie).

- Il pulsante "Test altoparlante di sistema" e le relative funzionalità di rilevamento **non** saranno visibili nelle impostazioni dei dispositivi ARM64.
- L'opzione "Usa dispositivo audio per creare segnali acustici" non è presente perché questo comportamento viene applicato automaticamente.
- Questa limitazione esiste perché l'accesso diretto all'hardware degli altoparlanti del PC/sistema non è disponibile sulle piattaforme Windows ARM64.
- Su ARM64, i segnali acustici verranno sempre riprodotti tramite il dispositivo di uscita audio standard.

**Se si utilizza un computer ARM64 e non si vedono le opzioni degli altoparlanti di sistema in NeoBleeper, si tratta di un problema previsto e non di un bug.**

---

## 4. Come verificare la presenza dell'altoparlante di sistema

- **Computer desktop:** La maggior parte dei desktop più vecchi ha un connettore per l'altoparlante del PC sulla scheda madre. I sistemi più recenti potrebbero non avere questa funzionalità o presentare l'uscita in un formato nascosto/non PNP0800 che NeoBleeper ora può utilizzare.
- **Laptop:** La maggior parte dei laptop non ha un altoparlante di sistema separato; tutto l'audio viene instradato attraverso il sistema audio principale.
- **Macchine virtuali:** L'emulazione dell'altoparlante di sistema è spesso assente o inaffidabile; le uscite non PNP0800 potrebbero non essere disponibili.
- **Come riconoscerlo:** Se visualizzi gli avvisi sopra riportati ma riesci ad abilitare e testare l'altoparlante di sistema in NeoBleeper, è probabile che il tuo computer abbia un'uscita nascosta o non standard.
  
---

## 5. Non sento alcun suono!

- **Controlla le impostazioni di NeoBleeper:**
  Se l'altoparlante di sistema non è disponibile, assicurati che il dispositivo audio (altoparlanti/cuffie) sia selezionato correttamente e funzionante.
- **Controlla il mixer del volume di Windows:**
  Assicurati che NeoBleeper non sia disattivato nel mixer del volume di sistema.
- **Prova il pulsante "Test altoparlante di sistema":**
  Usalo per testare l'altoparlante del PC.
- **Leggi i messaggi di avviso:**
  NeoBleeper fornirà istruzioni specifiche se non riesce ad accedere all'altoparlante di sistema.

---

## 6. Avvisi, errori e risoluzione dei problemi relativi all'API Google Gemini™ relativi all'IA

La funzione "Crea musica con l'IA" di NeoBleeper utilizza l'API Google Gemini™. Potresti visualizzare finestre di dialogo di errore o avvisi specifici relativi alla disponibilità dell'API, ai limiti di utilizzo o alle restrizioni nazionali.

### 6.1 Errori di quota o limite di velocità (429 RESOURCE_EXHAUSTED)

![image1](https://github.com/user-attachments/assets/be16a232-2eeb-4bcd-8384-289c89fe2af3)

**Messaggio di esempio:**
```
Si è verificato un errore: RESOURCE_EXHAUSTED (Codice: 429): You exceeded your current quota, please check your plan and billing details...
```

**Potenziali motivi:**
- **La quota API per il tuo account è esaurita.** Se utilizzi una chiave API gratuita, alcuni modelli (come `gemini-2.0-pro-exp`) potrebbero non essere disponibili o potrebbero avere limiti di utilizzo molto bassi/rigidi per gli account gratuiti.
- **Limitazioni del livello gratuito:** Alcuni modelli generativi più recenti (come Gemini Pro Exp) *non* sono disponibili per gli utenti del livello gratuito. Il tentativo di utilizzarli genera un errore di quota o di disponibilità.
- **Superamento dei limiti di frequenza:** Se invii troppe richieste troppo rapidamente, potresti raggiungere i limiti di frequenza dell'API anche con un piano a pagamento.

**Come risolvere:**
- **Controlla la quota e la fatturazione dell'API:** Accedi al tuo account Google Cloud/Gemini per verificare l'utilizzo e, se necessario, aggiorna il piano.
- **Utilizza solo modelli supportati:** Gli utenti del livello gratuito potrebbero essere limitati a determinati modelli. Consulta la documentazione per i modelli disponibili o passa a uno supportato.
- **Attendi e riprova più tardi:** A volte, attendere qualche istante può consentire l'aggiornamento temporaneo della quota, come indicato dal conto alla rovescia del messaggio. - **Consulta la [documentazione dell'API Gemini](https://ai.google.dev/gemini-api/docs/rate-limits) per policy di utilizzo e limiti di velocità aggiornati.**

---

### 6.2 Limitazioni regionali o nazionali

#### "L'API non è disponibile nel tuo Paese"

![image4](https://github.com/user-attachments/assets/7a80071c-7f0d-4080-9b97-c7129f4d2d92)

Alcune regioni non sono supportate dall'API di Google Gemini™ a causa di restrizioni regionali o legali.

**Potenziali motivi:**
- Il tuo Paese è un Paese in cui la disponibilità dell'API Gemini è limitata.
- La chiave API che stai utilizzando è registrata in una regione che non ha accesso.

**Come risolvere:**
- **Controlla i Paesi supportati dall'API di Google Gemini™** nella documentazione ufficiale.
- Se ti trovi in ​​un Paese con restrizioni, le funzionalità di IA non saranno utilizzabili.

#### Avviso specifico per regione (Pannello Impostazioni)

![image3](https://github.com/user-attachments/assets/fe7d7b2d-6554-4ee2-bb8a-31aad7c34d31)

Nello Spazio Economico Europeo, in Svizzera o nel Regno Unito, l'API Gemini™ potrebbe richiedere un account Google a pagamento (non gratuito).

- Se visualizzi questo avviso, assicurati di aver aggiornato il tuo piano API Gemini prima di provare a utilizzare le funzionalità di IA.

---

### 6.3 Consigli generali per l'API di IA

- Inserisci solo la tua chiave API; non condividerla per la tua sicurezza.
- NeoBleeper non trasmette la tua chiave API se non direttamente al servizio Gemini, se necessario per l'utilizzo delle funzionalità.
- Se riscontri errori ripetuti, prova a rimuovere e aggiungere nuovamente la tua chiave API e verifica che sia attiva.

---

## 7. Consigli per altoparlanti e audio di sistema per chipset specifici (incluso Intel B660)

### Se non si sente alcun suono, l'audio è danneggiato o l'altoparlante di sistema non è affidabile:

Alcuni chipset moderni, inclusi quelli della serie Intel B660 e successivi, potrebbero avere problemi con l'inizializzazione o la reinizializzazione dell'altoparlante di sistema (segnale acustico del PC), con conseguente silenzio o problemi audio.

**Consigli per gli utenti interessati:**

- **Provare a mettere il computer in modalità sospensione e a riattivarlo.**
Questo potrebbe aiutare a reinizializzare o ripristinare la porta hardware di basso livello responsabile dell'altoparlante di sistema e a ripristinare la funzionalità del segnale acustico.
- **Utilizzare la funzione "Usa dispositivo audio per creare un segnale acustico"** come soluzione alternativa se l'uscita dell'altoparlante di sistema non è affidabile.
- **Verificare la disponibilità di aggiornamenti del BIOS o del firmware:** Alcuni produttori di schede madri potrebbero rilasciare aggiornamenti che migliorano la compatibilità delle porte degli altoparlanti.
- **Specifico per desktop:** Se hai aggiunto, rimosso o ricollegato l'hardware degli altoparlanti di sistema, esegui un ciclo di accensione/spegnimento completo.

_Questa soluzione alternativa è evidenziata nelle impostazioni:_

![image2](https://github.com/user-attachments/assets/782dae29-9cc8-444e-b473-9513888df725)

> *Se non senti alcun suono o l'audio è corrotto, prova a mettere il computer in modalità sospensione e poi a riattivarlo. Questo può aiutare a reinizializzare l'altoparlante di sistema sui chipset interessati.*

---

*Per qualsiasi problema relativo all'audio o all'intelligenza artificiale non trattato qui, includi screenshot di errore, dettagli sull'hardware del tuo PC (in particolare marca e modello della scheda madre/chipset) e il tuo paese/regione quando richiedi supporto o apri un problema su GitHub.*

---

## 8. Domande frequenti

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

### D: Perché non vedo alcuna opzione relativa agli altoparlanti di sistema o ai segnali acustici sul mio dispositivo ARM64?
**R:** Sui sistemi Windows ARM64, NeoBleeper disabilita le impostazioni relative agli altoparlanti di sistema perché le piattaforme ARM64 non supportano l'accesso diretto all'hardware degli altoparlanti di sistema. Tutti i segnali acustici vengono riprodotti tramite il normale dispositivo di uscita audio (altoparlanti o cuffie) e le opzioni "Testa altoparlante di sistema" e "Usa dispositivo audio per creare un segnale acustico" vengono automaticamente nascoste. Questo comportamento è intenzionale e non si tratta di un errore.

### D: Cosa significa quando ricevo un avviso "è presente un'uscita per altoparlanti di sistema non standard"?
**R:** NeoBleeper ha rilevato hardware per altoparlanti non conforme agli standard tradizionali per gli altoparlanti per PC (ad esempio, non un dispositivo PNP0800). Potrebbe trattarsi di un'uscita per altoparlanti "nascosta" presente su desktop o macchine virtuali moderni. In questi casi, non tutte le funzionalità di segnalazione acustica potrebbero funzionare in modo affidabile, ma NeoBleeper tenterà di utilizzare qualsiasi uscita compatibile che riesca a rilevare.

### D: Perché il pulsante "Test altoparlante di sistema" è presente anche se Windows non elenca un dispositivo altoparlante per PC?
**R:** NeoBleeper include una logica di rilevamento per le uscite per altoparlanti di sistema nascoste o non standard. Se il pulsante viene visualizzato, significa che NeoBleeper ha trovato una potenziale porta hardware per l'uscita degli altoparlanti, anche se non è segnalata da Windows come dispositivo.

### D: Sto utilizzando l'API Google Gemini™ per le funzionalità di intelligenza artificiale e visualizzo il messaggio "Quota esaurita" o "API non disponibile nel tuo Paese". Cosa devo fare?
**R:** Consulta la sezione 6 di questa guida. Assicurati che la tua chiave API e la fatturazione/quota siano in regola e che il tuo utilizzo sia conforme alle restrizioni regionali di Google. Se ti trovi in ​​un'area geografica con restrizioni, purtroppo le funzionalità di intelligenza artificiale potrebbero non essere disponibili.

### D: Ho un sistema Intel B660 (o più recente) e l'altoparlante del mio PC a volte non funziona o si blocca. È normale?
**R:** Alcuni chipset più recenti presentano problemi di compatibilità noti con la reinizializzazione dell'altoparlante di sistema. Prova a mettere il computer in modalità sospensione e poi a riattivarlo, oppure utilizza il tuo dispositivo audio abituale. Verifica la presenza di aggiornamenti BIOS/firmware che potrebbero migliorare il supporto degli altoparlanti.

### D: Qual è il modo migliore per segnalare problemi audio o di intelligenza artificiale al supporto?
**R:** Fornisci sempre quante più informazioni possibili: marca/modello del computer, regione, screenshot delle finestre di dialogo di errore e il file `DebugLog.txt` dalla cartella NeoBleeper. Per problemi di intelligenza artificiale, includi il testo completo delle finestre di dialogo di errore e descrivi lo stato del tuo account API Gemini.

### D: Dopo un crash o una chiusura forzata, il Beep Stopper di NeoBleeper non ha interrotto il segnale acustico continuo. Esiste un altro modo per risolvere il problema?
**R:** Se il Beep Stopper non funziona, riavviando il computer si ripristinerà l'hardware degli altoparlanti di sistema e si interromperanno eventuali segnali acustici persistenti.

### D: È sicuro utilizzare l'utilità Beep Stopper se visualizzo un messaggio di avviso relativo a un'uscita audio non standard o mancante per gli altoparlanti di sistema?
**R:** Sì, ma tieni presente che l'utilità potrebbe non essere in grado di controllare l'hardware e, in rari casi, potrebbe causare instabilità o nessun effetto. In caso di dubbi, scegli di non procedere e riavvia il computer.

### D: Sulle macchine virtuali, non riesco a far funzionare l'altoparlante di sistema. È un bug?
**R:** Non necessariamente. Molte macchine virtuali non emulano correttamente l'altoparlante di un PC o presentano l'output in un modo che non può essere controllato a livello di programmazione. Utilizza il tuo dispositivo di output audio standard per ottenere risultati ottimali.

**Potenziali aggiornamenti futuri:**
Se futuri test o sviluppi consentiranno a NeoBleeper di rilevare in modo affidabile gli altoparlanti del sistema rotti o scollegati tramite il test hardware a ultrasuoni, queste FAQ e la logica di rilevamento verranno aggiornate per riflettere tali miglioramenti. Per maggiori dettagli, consultare i changelog o le nuove versioni.

---

## 9. Ottenere assistenza

- **Fornire dettagli sul computer e sull'ambiente:** Quando si segnalano problemi di rilevamento hardware o audio, si prega di includere dettagli sul computer (desktop/laptop, produttore/modello, sistema operativo) e su eventuali componenti hardware rilevanti.
- **Allegare screenshot o finestre di dialogo di errore:** Gli screenshot di finestre di dialogo di errore o avviso sono molto utili. Specificare esattamente quando si verifica il problema.
- **Includere il file di registro:** A partire dalle versioni più recenti, NeoBleeper crea un file di registro dettagliato denominato `DebugLog.txt` nella cartella del programma. Si prega di allegare questo file quando si richiede assistenza, poiché contiene utili informazioni diagnostiche.
- **Descrivere i passaggi per riprodurre il problema:** Descrivere chiaramente cosa si stava facendo quando si è verificato il problema.
- **Aprire un problema su GitHub:** Per ulteriore assistenza, aprire un problema su GitHub e includere tutti i dettagli sopra indicati per un supporto ottimale.

_Questa guida viene aggiornata man mano che vengono scoperti nuovi problemi e soluzioni. Per ulteriore assistenza, apri un problema su GitHub con informazioni dettagliate sulla tua configurazione e sul problema riscontrato.
