# Manuale di NeoBleeper
- ## Come usare NeoBleeper?
  Le due aree principali sono la tastiera (area orizzontale nella parte superiore dello schermo) e l'elenco musicale (la vista elenco in basso a destra con sette colonne).
  La ​​tastiera è divisa in tre sezioni colorate, ciascuna delle quali rappresenta un'ottava.
  Per accedere a note al di fuori dell'intervallo corrente (più alte delle note verdi o più basse delle note rosse), regola il valore "Ottava" situato a destra e a sinistra della tastiera.
  Aumentando questo valore di un'unità, le note della tastiera si spostano di un'ottava più in alto.
  
  ![image](https://github.com/user-attachments/assets/34edbb36-bc79-4d49-8eae-3333337fee12)
  
  L'elenco musicale contiene righe di note. È possibile riprodurre fino a 4 note contemporaneamente.
  Le sette colonne dell'elenco musicale sono "Durata" (durata della nota o delle note sulla riga), le quattro colonne centrali per le quattro note, la sesta colonna "Mod" (modificatore di durata) e la settima colonna "Art" (articolazioni).
  Quando si clicca sulle note sulla tastiera, queste vengono riprodotte immediatamente tramite l'altoparlante di sistema e inserite nell'elenco musicale nella colonna "Nota 1" per impostazione predefinita.
  Questo comportamento può essere modificato per inserire le note in "Nota 1", "Nota 2", "Nota 3" o "Nota 4", consentendo agli utenti di identificare le note ascoltandole.
  Le note cliccate sulla tastiera vengono inserite sopra la riga attualmente selezionata nell'elenco musicale.
  Se non è selezionata alcuna riga, le note vengono aggiunte alla fine del brano.
  Per aggiungere note alla fine quando una riga è selezionata, fare clic su "Deseleziona riga" (pulsante turchese a destra dell'elenco musicale).

  ![image](https://github.com/user-attachments/assets/1ed5d301-421c-42aa-876b-fdea0e00bc2b)

  - ### Caselle di controllo multifunzione nell'elenco note

    NeoBleeper ora include caselle di controllo multifunzione nell'elenco note, aggiungendo potenti funzionalità di modifica e riproduzione con un'interfaccia semplice e intuitiva.

    ![image1](https://github.com/user-attachments/assets/58d14180-8d5a-43d1-b343-d45d9d7e1121)

    **Caratteristiche principali delle caselle di controllo multifunzione:**
    - **Riproduci battuta nelle linee selezionate (nessuna limitazione di numeri interi):**
    Quando la funzione "Riproduci battuta" o una modifica di battuta è abilitata, le battute vengono riprodotte solo nelle linee in cui la casella di controllo è selezionata. Questo consente pattern ritmici personalizzati indipendentemente dai limiti di battuta o dalle posizioni dei numeri interi: le battute possono essere riprodotte su qualsiasi linea selezionata, per una maggiore flessibilità creativa.
    - **Tagliare e copiare più note:**
    Per tagliare o copiare più linee contemporaneamente, selezionare le linee desiderate, quindi utilizzare le azioni "Taglia" o "Copia". Tutte le linee selezionate vengono incluse nell'operazione, consentendo un'efficiente modifica in batch.
    - **Cancellare più note:**
    Per cancellare più note contemporaneamente, selezionare le linee che si desidera rimuovere e utilizzare il pulsante "Cancella". Questo consente l'eliminazione rapida di più note, riducendo il lavoro ripetitivo e minimizzando gli errori.
    - **Riproduzione vocale sulle linee selezionate:**
    Quando si utilizza il sistema di sintesi vocale, è possibile limitare la riproduzione vocale alle sole linee selezionate. Questo semplifica l'evidenziazione di determinate frasi musicali o l'alternanza tra i segnali acustici tradizionali e la sintesi vocale all'interno di un brano.
      
   **Suggerimenti per l'utilizzo:**
    - È possibile selezionare o deselezionare più linee in rapida successione per configurare rapidamente quali parti della musica saranno interessate da ciascuna operazione.
    - Le caselle di controllo multifunzione funzionano in combinazione con altre funzioni di editing, consentendo operazioni batch avanzate senza dover cambiare modalità.
    - Queste caselle di controllo sono indipendenti dalla selezione della linea per l'inserimento delle note: le linee possono essere selezionate per le azioni batch anche se un'altra linea è selezionata per la modifica.
    
    Questa aggiunta semplifica sia la composizione che l'editing, offrendo un maggiore controllo sulla riproduzione, sugli effetti vocali e sulla manipolazione batch della musica.
    
  Per passare dall'inserimento di una nota alla sostituzione della nota attualmente selezionata e per scegliere in quale delle quattro colonne di note inserire o sostituire una nota, utilizzare la casella delle opzioni sotto il lato sinistro della tastiera intitolata "Quando si preme il tasto".
  Questa casella include anche l'opzione per suonare le note senza inserirle o sostituirle.

  ![image](https://github.com/user-attachments/assets/284b8f5f-02ee-47ec-b735-64608c7a8fbb)

  Per modificare la durata della nota inserita, regola il valore "Durata nota" situato al centro della tastiera.
  Questo menu a discesa consente di selezionare note intere, 1/2 (metà), 1/4 (quarto), 1/8, 1/16 o 1/32.

  ![immagine](https://github.com/user-attachments/assets/4aac418b-83ac-423a-a8c5-d1fa9ce13ff7)
  
  Ad esempio, per creare una minima puntata, inserisci una minima e poi una semiminima, oppure inserisci una minima e applica un modificatore di durata "Puntata".

  ![immagine](https://github.com/user-attachments/assets/db065d5c-51a0-4765-9907-f37274c27cec)
  
  Se una nota da 1/32 non è abbastanza breve, il tempo (BPM) del brano può essere impostato fino a 600 BPM, il doppio della capacità della maggior parte dei sequencer MIDI.
  Questo consente di aumentare il tempo e di compensare raddoppiando la durata di ogni nota.
  Il controllo del tempo si trova nella parte inferiore della tastiera, appena a destra del controllo dell'indicazione di tempo.
  Imposta il numero di battiti al minuto e include un pulsante "Metronomo" per rappresentare acusticamente la velocità del battito.

  ![immagine](https://github.com/user-attachments/assets/575eacea-955d-464c-9c3e-ff1713b67834)
  
  Dal file ZIP viene estratta una cartella denominata "Musica". Contiene diversi file di salvataggio (NBPML) per questo programma, che possono essere caricati come dimostrazioni. Per ulteriori informazioni, consultare la sezione "Apertura e salvataggio" di seguito.

  ![image](https://github.com/user-attachments/assets/7d69785d-5e00-45f2-b529-24a869810a98)

- ## Alternanza tra note
  Poiché l'altoparlante di sistema può riprodurre solo una nota alla volta, il programma alterna tra "Nota 1", "Nota 2", "Nota 3" e "Nota 4" se più colonne contengono una nota sulla stessa riga dell'elenco musicale.
  La velocità di questa alternanza è controllata dal valore immesso nella casella di testo "Passa da una nota all'altra ogni: ... mS", situata a sinistra dell'elenco musicale.
  Inoltre, l'ordine delle note alternate può essere determinato tramite i pulsanti di opzione nella casella di testo "Passa da una nota all'altra ogni: ... mS".

  ![image](https://github.com/user-attachments/assets/35f57f2e-b2d5-4c3f-87f6-26c51eb630a8)

- ## Rapporto Nota/Silenzio
  Questa impostazione definisce la proporzione di tempo in cui una riga dell'elenco musicale produce suono rispetto al silenzio. Regolando questo rapporto è possibile ridurre l'emissione di toni continui.

  ![image](https://github.com/user-attachments/assets/33cfd48f-806e-4f4b-8a87-c38d44c88706)
  
- ## Apertura e salvataggio di file
  Musica e impostazioni possono essere salvate e caricate utilizzando i pulsanti "Apri", "Salva" e "Salva con nome" nel pulsante "File" nella barra dei menu. L'operazione di salvataggio include l'elenco dei brani e la maggior parte delle opzioni di configurazione.
  
  I file salvati utilizzano l'estensione ".NBPML" e sono basati su testo XML. Questi file possono essere modificati con editor di testo come Blocco note.
  Inoltre, NeoBleeper può aprire file ".BMM", il formato di file di Bleeper Music Maker, ma non possono essere sovrascritti e devono essere salvati come file ".NBPML" separato.

  ![image](https://github.com/user-attachments/assets/0632588b-e6dd-44b0-8e86-4cde0a768f73)

 - ## Suggerimenti per la modifica della musica
  Sia i file NBPML che BMM sono basati su testo e compatibili con gli editor di testo standard. Le funzioni di copia e incolla e di ricerca e sostituzione possono essere utili per le attività ripetitive o per correggere errori.
  
  Per cancellare una riga, utilizzare il pulsante rosso "Cancella riga intera". Per cancellare solo una colonna di note, utilizzare i quattro pulsanti blu nella stessa area.

  ![immagine](https://github.com/user-attachments/assets/1e1adc29-93c5-4f1a-b505-becbb5fec8ba)
  
  Per sostituire la durata delle note, selezionare l'opzione "Sostituisci" e abilitare la sostituzione della durata. Quindi fare clic su "Riga vuota" per ogni riga per aggiornare la durata senza alterare le note.

  ![image](https://github.com/user-attachments/assets/68088290-2f1e-491c-acc1-7a81f9ee8917)

  ![image](https://github.com/user-attachments/assets/47f47a6f-f9d8-47b6-bedc-dc2bc2c62110)

- ## Riproduzione musicale
  Utilizza il pulsante verde in alto "Riproduci tutto" per riprodurre tutte le note nell'elenco musicale. La riproduzione riprende dall'inizio alla fine, se la casella di controllo sottostante al pulsante "Riga vuota" è selezionata. Il pulsante verde centrale riproduce dalla riga attualmente selezionata e torna indietro.
  Il pulsante verde in basso interrompe la riproduzione al termine della nota corrente.

  ![immagine](https://github.com/user-attachments/assets/3a4ec422-47ed-4dbb-bd76-9ace4a9fa616)
  
  Cliccando su una riga nell'elenco musicale, quella riga viene riprodotta per impostazione predefinita. Per modificare questo comportamento o limitare la riproduzione a una colonna di note, regola le caselle di controllo nella casella "Quando si clicca su una riga" sotto "Quali note nell'elenco riprodurre" in basso a sinistra nella finestra principale.
  Caselle di controllo simili sotto "Quando la musica viene riprodotta" controllano il comportamento della riproduzione durante la riproduzione automatica.

  ![image](https://github.com/user-attachments/assets/e2871f3d-45a8-4e67-a126-266fdea18c4e)

- ## Modificatori di lunghezza e articolazioni

  NeoBleeper supporta note puntate e terzine, nonché Staccato, Spiccato e Fermata. La colonna "Mod" nell'elenco musicale mostra "Pun" per le note puntate e "Ter" per le terzine, mentre la colonna "Art" nell'elenco musicale mostra "Sta" per Staccato,
  "Spi" per Spiccato e "Fer" per Fermata.

  ![image](https://github.com/user-attachments/assets/c40e3737-30d1-4181-8562-5b6a7cd3c971)
  
  Per applicare un modificatore puntato (1,5 volte la lunghezza originale), seleziona una riga e clicca sul pulsante "Puntato" sopra l'elenco musicale. Una nota puntata equivale alla lunghezza originale più la nota più breve successiva. Ad esempio, una semiminima puntata equivale a una semiminima più una croma.
  
  Per applicare un modificatore di terzina (1/3 della lunghezza originale), seleziona una riga e clicca sul pulsante "Terzina". Tre note di terzina della stessa lunghezza equivalgono a una nota della lunghezza originale. Una riga non può essere sia puntata che terzinata.
  
  Per applicare un modificatore di Staccato (metà della lunghezza originale, poi silenzio), seleziona una riga e clicca sul pulsante "Staccato".
  
  Per applicare un modificatore di Spiccato (0,25 volte la lunghezza originale, poi silenzio), seleziona una riga e clicca sul pulsante "Spiccato".
  
  Per applicare un modificatore di Fermata (il doppio della lunghezza originale), seleziona una riga e clicca sul pulsante "Fermata". Una riga non può essere Staccato, Spiccato e Fermata contemporaneamente.
  
  Per inserire note puntate, terzinate, Staccato, Spiccato o Fermata, premi il pulsante corrispondente e poi clicca sulle note sulla tastiera. Durante la riproduzione, i pulsanti "Puntato", "Terzina", "Staccato", "Spiccato" e "Fermata" si attivano automaticamente quando si incontrano tali modificatori e articolazioni.

  ![image](https://github.com/user-attachments/assets/71648b8c-57a9-463a-93e4-07daef7eb56b)

- ## Indicazione di Tempo e Visualizzazione della Posizione

  NeoBleeper fornisce l'impostazione "Indicazione di Tempo", situata a sinistra dell'impostazione BPM. Definisce il numero di battiti per battuta. Questa impostazione influisce sul suono del metronomo e sul comportamento della visualizzazione della posizione, ma non altera il suono in riproduzione.

  ![image](https://github.com/user-attachments/assets/4ef1ba4a-1eab-43cb-b38e-36fe4883053f)
  
  Tre indicatori di posizione nell'angolo in basso a destra mostrano la posizione corrente nella musica. Il display superiore mostra la battuta, quello centrale mostra il battito all'interno della battuta e quello inferiore mostra una rappresentazione tradizionale con note intere, minime (1/2), quarti (1/4), ottavi, sedicesimi o trentaduesimi.

  ![immagine](https://github.com/user-attachments/assets/f3e6b9a3-a1ba-4199-88e5-10e3977dd08a)
  
  Le indicazioni di tempo più basse comportano cambiamenti più rapidi nel display superiore. Il display centrale si reimposta su 1 all'inizio di ogni nuova battuta.
  
  Il display inferiore non può rappresentare posizioni con una precisione superiore a 1/32 di nota. Mostra "(Errore)" con un testo rosso per le posizioni non supportate, come quelle create da sedicesimi puntati (3/64). Una volta che la posizione diventa nuovamente divisibile per una nota di 1/32, il display riprende a funzionare normalmente.

  ![immagine](https://github.com/user-attachments/assets/0f56a8c0-e20f-4fc9-91e4-b8af0727f6fc)
  
  Anche le terzine influiscono sulla precisione della visualizzazione. Dopo aver inserito tre terzine della stessa lunghezza, la posizione diventa divisibile per una nota da 1/32, ripristinando la funzionalità di visualizzazione.
  
  La riproduzione di terzine verso la fine di una lunga lista musicale può richiedere notevoli risorse della CPU. In caso di problemi di prestazioni, abilitare la casella di controllo "Non aggiornare" sotto la visualizzazione della posizione per disabilitare gli aggiornamenti durante la riproduzione. Gli aggiornamenti in modalità di modifica rimangono attivi.
  
  I vecchi file BMM creati con versioni precedenti alla revisione 127 di Bleeper Music Maker utilizzano per impostazione predefinita un'indicazione di tempo pari a 4 quando vengono aperti in NeoBleeper. La modifica e il salvataggio dell'indicazione di tempo nei file .NBPML mantengono l'impostazione.

- ## Registrazione del Debug

  Dalla versione 0.18.0 Alpha, NeoBleeper utilizza la classe `Logger` per gestire tutti i log e la diagnostica. L'output del log viene salvato in un file denominato `DebugLog.txt` situato nella directory principale dell'applicazione.
  
  La classe `Logger` fornisce informazioni dettagliate sul runtime, inclusi errori, avvisi e messaggi di debug generali. Questo file di log viene creato e aggiornato automaticamente durante l'esecuzione dell'applicazione.
  
  Per il debug avanzato, è comunque possibile avviare NeoBleeper direttamente da Visual Studio per utilizzare i suoi strumenti integrati, come i punti di interruzione e la finestra di output. Tuttavia, il file `DebugLog.txt` garantisce che il log sia costantemente disponibile anche al di fuori dell'ambiente di sviluppo di Visual Studio.
  
  I file trigger esterni come `logenable` e i vecchi metodi di diagnostica non sono più supportati. Tutte le informazioni rilevanti sono ora centralizzate nel file `DebugLog.txt` per facilitarne l'accesso e la consultazione.
  
- ## Mod

  Il programma include diverse modifiche che ne alterano il comportamento rispetto al design originale. Queste modifiche sono elencate in basso a sinistra dello schermo, accanto all'elenco dei brani musicali. Ogni mod ha una casella di controllo per abilitarla o disabilitarla. Se una casella di controllo non può essere deselezionata, chiudendo la finestra della mod la mod verrà disabilitata e la casella di controllo verrà deselezionata.
  
  Clicca sul pulsante con il punto interrogativo accanto alla casella di controllo di una mod per visualizzare una breve descrizione della sua funzione (disponibile per la maggior parte delle mod).

  ![image](https://github.com/user-attachments/assets/d98b8ccd-e7f5-4810-b15b-d17e58052449)
  
  - ### Mod Riproduzione Sincronizzata

    ![immagine](https://github.com/user-attachments/assets/c98f60a5-7e4d-4244-8921-943ceddfe98c)
    
    Questa mod consente a NeoBleeper di avviare la riproduzione a un orario di sistema specificato. È progettata per sincronizzare più istanze di NeoBleeper, in particolare quando si utilizzano file NBPML o BMM separati per diverse sezioni di una composizione. Configurando ogni istanza in modo che si avvii alla stessa ora, è possibile ottenere una riproduzione sincronizzata tra le istanze.
    
    Attivando la mod si apre una finestra di configurazione. Questa finestra consente agli utenti di inserire un orario di inizio target (ora, minuto, secondo) in base all'orologio di sistema. L'ora di sistema corrente viene visualizzata come riferimento. Per impostazione predefinita, l'orario target è impostato su un minuto avanti rispetto all'ora corrente, ma questo valore può essere regolato manualmente. Gli utenti possono anche specificare se la riproduzione deve iniziare dall'inizio del brano o dalla riga attualmente selezionata nell'elenco dei brani. Il programma eseguirà il comando di riproduzione corrispondente ("Riproduci tutto" o "Riproduci dalla riga selezionata") al raggiungimento dell'orario di destinazione.
    
    È presente un pulsante di controllo per avviare lo stato di attesa. Una volta attivato, l'interfaccia indica che il programma è in attesa e l'etichetta del pulsante cambia in "Interrompi attesa". Se il programma non è in stato di attesa al raggiungimento dell'orario di destinazione, la riproduzione non verrà eseguita.
    
    La casella di controllo "Riproduzione sincronizzata" è deselezionata e la finestra è chiusa. Per riaprire la finestra, disabilitando questa opzione si annullerà qualsiasi stato di attesa attivo.
    
    La riproduzione viene interrotta automaticamente all'avvio dello stato di attesa per evitare problemi, a differenza di Bleeper Music Maker originale.
    
    La sincronizzazione tra più computer è possibile se tutti gli orologi di sistema sono allineati con precisione. Si consiglia di sincronizzare gli orologi di sistema prima di utilizzare questa funzione sui dispositivi.
    
  - ### Mod Suono Riproduci Beat

    ![immagine](https://github.com/user-attachments/assets/f6b9ef4c-49a0-4a54-ad9e-8fb4833fdf34)
    
    Questa modifica garantisce che l'altoparlante/dispositivo audio di sistema emetta un suono di beat a ogni beat o a ogni altro beat, a seconda della configurazione selezionata. Il suono ricorda un beat in stile techno a causa della natura elettronica dell'altoparlante/dispositivo audio di sistema. Quando la casella di controllo "Riproduci un suono di beat" è selezionata, viene visualizzata una finestra di configurazione. Questa finestra consente agli utenti di scegliere se il suono di beat venga riprodotto a ogni beat o a ogni beat dispari. Quest'ultima opzione dimezza di fatto il tempo dei suoni di beat.
    
    Per dimostrare la modifica del tempo, gli utenti possono avviare il programma, aggiungere quattro note da un quarto all'elenco musicale, abilitare l'opzione "Riproduci un suono di beat" e alternare tra le due impostazioni del suono di beat. La differenza di tempo dovrebbe essere udibile. La casella di controllo "Riproduci suono beat" è deselezionata quando la finestra di configurazione è chiusa.

   - ### Mod Portamento Bleeper

    ![immagine](https://github.com/user-attachments/assets/02bdca54-bebc-457b-859e-9d30bfdc816a)
  
    Questa modifica fa sì che il tono dell'altoparlante/dispositivo audio di sistema passi gradualmente dalla nota precedente a quella corrente. Quando la casella di controllo "Portamento Bleeper" è selezionata, viene visualizzata una finestra di impostazioni. Questa finestra consente agli utenti di regolare la velocità di transizione tra le note, da quasi istantanea a durate prolungate. Gli utenti possono anche configurare la durata della nota al clic o impostare la riproduzione della nota in modo che continui a essere riprodotta indefinitamente.
  
   - ### Usa la tastiera come mod per pianoforte

    ![immagine](https://github.com/user-attachments/assets/97c16e1c-3836-40d9-a8cd-8f54cace4689)

    Questa funzione mappa la tastiera del computer sulle note musicali, consentendo la riproduzione diretta tramite pressione dei tasti senza dispositivi di input MIDI. Ogni tasto corrisponde a una nota specifica sul pianoforte virtuale. La mappatura segue un layout predefinito, in genere allineato con le etichette visibili della tastiera.
    Se abilitata, la pressione di un tasto attiverà immediatamente la nota associata utilizzando il metodo di sintesi corrente.

  - ### Sistema vocale ("Interni vocali")

    NeoBleeper ora include un potente sistema di sintesi vocale, accessibile tramite la finestra "Interni vocali". Questo sistema consente un controllo avanzato sulle voci sintetizzate, inclusi formanti vocalici, rumore e sibilanza, consentendo di creare suoni vocali simili a quelli umani o sperimentali direttamente nelle composizioni.
  
    ![image](https://github.com/user-attachments/assets/92dade58-16b5-4ea6-9994-4d319ae8111a)

      - #### **Accesso al Sistema Vocale**

      Per aprire la finestra Voice Internals, cerca l'opzione "Voice System" o "Voice Internals" nel menu o nella selezione del dispositivo di output per ogni nota.
      Ogni colonna di note (Note 1–4) può ora essere indirizzata individualmente al sistema vocale, al segnale acustico tradizionale o ad altri dispositivi di output utilizzando i nuovi menu a discesa "Opzioni di output".
      
      - #### **Panoramica della finestra Voice Internals**
      
      La finestra Voice Internals è organizzata in sezioni, ognuna delle quali offre un controllo preciso su diversi aspetti della voce sintetizzata:

      - ##### **Controllo Formante**

        Sono presenti quattro cursori per le formanti, ognuno dei quali rappresenta una risonanza chiave del tratto vocale umano:
        - Regola il **Volume** e la **Frequenza (Hz)** per ciascuna formante.
        - I pulsanti preimpostati ("Vocale aperta", "Chiusa anteriore", ecc.) consentono una rapida selezione dei suoni vocalici tipici.
        
        - ##### **Sezione Oscillatore**
        
          I cursori **Saw Vol** e **Noise Vol** controllano il livello dell'oscillatore a dente di sega e della sorgente di rumore, che costituiscono la base del timbro vocale.
          Questi possono essere combinati con i filtri per le formanti per una varietà di effetti sintetici e vocali.
        
        - ##### **Sibillanza e Mascheramento della Sibilanza**
        
          Quattro controlli di mascheramento consentono di simulare effetti di sibilanza o consonanti modellando le componenti del rumore e mascherando le frequenze.
          Il cursore "Cutoff Hz" imposta una frequenza di taglio per il mascheramento del rumore.
          
        ##### **Variazioni casuali delle formanti**

          - I cursori di altezza e intervallo introducono sottili variazioni casuali alle frequenze delle formanti, aggiungendo realismo o effetti speciali.
        
        - ##### **Opzioni di output**

          - Assegna il motore sonoro utilizzato da ogni colonna di note:
            "Beep altoparlante/dispositivo audio di sistema"
            "Sistema vocale"
            ...e altri, se disponibili.
          
          Puoi riprodurre un mix di suoni sintetizzati dalla voce e di suoni altoparlante/dispositivo audio di sistema in un singolo brano.
        
        - ##### **Note sui tasti e sull'utilizzo**

          La finestra fornisce una legenda per la codifica a colori e le abbreviazioni dei parametri.
          Un menu a discesa consente di scegliere quando riprodurre la voce (tutte le linee, linee specifiche, ecc.).
        
      - #### **Come usare il sistema vocale**

        1. **Assegnare una nota al sistema vocale**
          Nelle "Opzioni di output" della finestra Voice Internals o dell'interfaccia principale, impostare una colonna di note (ad esempio, Nota 2) su "Sistema vocale".
        2. **Modificare le impostazioni di formanti e oscillatori**
          Utilizzare i cursori e i pulsanti preimpostati per modellare vocali, timbro e sibilanti.
        3. **Riproduzione**
          Quando si riproduce musica, le colonne di note selezionate utilizzeranno il sintetizzatore vocale in base alle impostazioni.
        4. **Sperimenta**
          Provare diverse combinazioni, intervalli di randomizzazione e mix di oscillatori per voci sintetiche robotiche, naturali o uniche.
      
      - #### **Suggerimenti**

        - Mix and match: assegna alcune note al sistema vocale e altre ai segnali acustici per ottenere colonne sonore ricche e stratificate.
        - Per risultati ottimali, regola le formanti in modo che corrispondano all'altezza delle note.
        - Utilizza i cursori di randomizzazione per ottenere irregolarità più "umane" o artefatti robotici.
        - Il sistema vocale può essere utilizzato per la progettazione di suoni sperimentali, non solo per le voci.
          
- ## Impostazioni
  La finestra Impostazioni di NeoBleeper è divisa in quattro schede principali, ciascuna dedicata a un aspetto diverso della configurazione dell'app.
  
  - ### Impostazioni generali
    Questa scheda si concentra sulle preferenze di base e sull'integrazione a livello di sistema:
    
    ![image](https://github.com/user-attachments/assets/5108bb51-fa53-414e-9ee1-50c545b5017c)

    - #### Lingua
      **Selettore Lingua:** Consente di scegliere la lingua di NeoBleeper tra inglese, tedesco, francese, italiano, spagnolo, turco, russo, ucraino e vietnamita.
    
    - #### Aspetto Generale
      **Selettore Tema:** Consente di scegliere tra i temi personalizzati di NeoBleeper o di utilizzare l'aspetto predefinito del sistema operativo.
    
      **Modalità Bleeper Classica:** Un'opzione legacy per gli utenti che preferiscono l'interfaccia o il comportamento originale.
    
    - #### Crea Musica con l'IA
      **Campo Chiave API Google Gemini™:** Input sicuro per abilitare le funzionalità musicali generate dall'IA.
    
      **Avviso di Sicurezza:** Consiglia agli utenti di non condividere la propria chiave API.
    
      **Pulsanti Aggiorna/Reimposta:** Gestisce il ciclo di vita della chiave API. Il pulsante Aggiorna è disabilitato, probabilmente in attesa di un input valido.
    
    - #### Altoparlante di Sistema di Test
      **Pulsante Test:** Emette un segnale acustico per confermare il funzionamento dell'altoparlante.
    
      **Messaggio di fallback:** suggerisce di utilizzare un dispositivo audio alternativo se non si sente alcun suono dall'altoparlante del sistema.
  
  - ### Creazione delle impostazioni audio

    Questa scheda è dedicata alla configurazione del modo in cui NeoBleeper genera segnali acustici utilizzando le funzionalità audio del sistema. Offre sia controllo tecnico che flessibilità creativa per modellare il tono e la consistenza dei suoni prodotti.

    ![image](https://github.com/user-attachments/assets/0b9169e4-0f08-475e-9c79-329613a6da58)
    
    - #### Usa dispositivo audio per creare un segnale acustico:
      Una casella di controllo che abilita o disabilita l'uso del dispositivo audio del sistema per la generazione di segnali acustici al posto dell'altoparlante di sistema. Se non selezionata, NeoBleeper utilizza l'altoparlante di sistema per la creazione del suono. Abilitando questa opzione è possibile ottenere una sintesi sonora più ricca e basata sulla forma d'onda.
    
    - #### Creazione di segnali acustici dalle impostazioni del dispositivo audio

      ##### Selezione della forma d'onda del tono
        **Scegli la forma d'onda utilizzata per generare i segnali acustici. Ogni opzione influisce sul timbro e sul carattere del suono:**
        
        **Quadrato (predefinito):** Produce un tono acuto e vibrante. Ideale per segnali acustici digitali classici e avvisi in stile retrò.
        
        **Sinusoidale:** Suono fluido e puro. Ottimo per notifiche discrete o applicazioni musicali.
        
        **Triangolare:** Più morbido del quadrato, con un suono leggermente cupo. Equilibrato tra nitidezza e fluidità.
        
        **Rumore:** Genera segnali a raffica casuali, utili per effetti sonori come interferenze, raffiche o texture simili a percussioni.

  - ### Impostazioni Dispositivi

    Questa scheda consente di configurare l'interazione di NeoBleeper con hardware MIDI esterno, strumenti virtuali e altri hardware esterni. Che si tratti di integrare input live o di indirizzare l'output a un synth, è qui che si definisce il flusso del segnale.
    
    ![image](https://github.com/user-attachments/assets/137c15a5-d410-42e8-b52e-9e75434e0ab4)

    - #### Dispositivi di ingresso MIDI
      **Usa ingresso MIDI live:** Abilita la ricezione del segnale MIDI in tempo reale da controller o software esterni. Se selezionato, NeoBleeper ascolta i messaggi MIDI in arrivo per attivare suoni o azioni.
      
      **Seleziona dispositivo di ingresso MIDI:** Un menu a discesa che elenca le sorgenti di ingresso MIDI disponibili. Scegli il dispositivo preferito per iniziare a ricevere dati MIDI.
      
      **Aggiorna:** Aggiorna l'elenco dei dispositivi di ingresso disponibili, utile quando si collega nuovo hardware o si avviano porte MIDI virtuali.
      
    - #### Dispositivi di uscita MIDI
      **Usa uscita MIDI:** Attiva la trasmissione MIDI da NeoBleeper a dispositivi esterni o strumenti virtuali.
      
      **Seleziona dispositivo di uscita MIDI:** Scegli dove NeoBleeper invia i suoi segnali MIDI. L'opzione predefinita è in genere un synth generico come Microsoft GS Wavetable Synth.
      
      **Canale:** Seleziona il canale MIDI (1/16) utilizzato per l'uscita. Questo consente il routing a strumenti o tracce specifici in configurazioni multicanale.
      
      **Strumento:** Definisce lo strumento General MIDI utilizzato per la riproduzione. Le opzioni spaziano da pianoforti e archi a sintetizzatori e percussioni, offrendoti il ​​controllo sul timbro dell'output.
      
      **Aggiorna:** Aggiorna l'elenco dei dispositivi di output disponibili, garantendo il riconoscimento dei nuovi dispositivi collegati.

    - #### Altri dispositivi e firmware del microcontrollore

      NeoBleeper supporta anche l'interazione con vari dispositivi hardware esterni, come buzzer, motori e microcontrollori, espandendo le sue capacità oltre i tradizionali dispositivi MIDI. Il gruppo **Altri dispositivi** all'interno della scheda Impostazioni dispositivi fornisce opzioni di configurazione e strumenti di generazione del firmware per questi componenti esterni.

      **Impostazioni per altri dispositivi:**
      - **Abilita dispositivo:**
      Casella di controllo per abilitare l'uso del motore o del buzzer (tramite Arduino, Raspberry Pi o ESP32). Questa casella deve essere selezionata per accedere ad altre opzioni del dispositivo.
      - **Tipo di dispositivo:**
      Pulsanti di opzione per selezionare tra:
      - **Motore passo-passo**
      - **Motore CC o Buzzer**
      - **Ottava motore passo-passo:**
      Controllo a scorrimento per regolare l'ottava di uscita del motore passo-passo, consentendo di adattare il movimento del motore alle gamme di tonalità musicali.
      - **Pulsante Ottieni firmware:**
      Cliccando su questo pulsante viene generato un firmware compatibile per il dispositivo selezionato. È necessario aggiornare questo firmware al microcontrollore prima di utilizzare la funzione. Se il microcontrollore non è aggiornato, la casella di controllo del dispositivo rimane disattivata.
      
      ![image1](https://github.com/user-attachments/assets/12a1f12d-d24b-4431-a058-9ea0a3940496)
      
      **Generatore di firmware per microcontrollori:**
      - Questa funzione consente di generare e copiare rapidamente firmware pronto all'uso per microcontrollori (come Arduino) direttamente da NeoBleeper.
      - Il firmware consente il controllo di hardware come buzzer e motori passo-passo, consentendo alle composizioni musicali di attivare azioni fisiche e suoni.
      - È possibile selezionare il tipo di microcontrollore (ad esempio, "Arduino (file ino)") dal menu a discesa.
      - La finestra del codice visualizza il firmware generato, personalizzato per il dispositivo selezionato.
      - Fare clic sul pulsante "Copia firmware negli appunti" per copiare facilmente il codice e caricarlo sul microcontrollore.

      **Esempio di caso d'uso:**
      - Con questa funzione, è possibile sincronizzare la riproduzione musicale con l'hardware, ad esempio attivando buzzer o pilotando motori passo-passo, utilizzando i segnali di uscita del sistema o il GCode esportato.
      - Il firmware Arduino generato include la gestione dei comandi seriali per l'identificazione del dispositivo e il controllo della velocità del motore, semplificando l'integrazione di NeoBleeper con la robotica o installazioni personalizzate.
      
      **Suggerimenti per l'integrazione:**
      - Combinare l'esportazione GCode di NeoBleeper con il firmware del microcontrollore per tradurre la musica in movimenti meccanici o output udibili. - Il gruppo "Altri dispositivi" semplifica la connessione del PC a hardware esterno, ampliando le possibilità creative per macchine basate sulla musica, performance cinetiche o arte sonora sperimentale.
      
      > Per ulteriori dettagli o per la risoluzione dei problemi, consultare i canali di supporto NeoBleeper o la documentazione del microcontrollore.

  - ### Impostazioni Aspetto
    Questa scheda ti offre il pieno controllo sull'identità visiva di NeoBleeper, consentendoti di personalizzare i colori degli elementi chiave dell'interfaccia per maggiore chiarezza, estetica o personalizzazione. È organizzata in sezioni per la visualizzazione di tastiera, pulsanti, indicatori ed eventi di testo.

    ![image5](https://github.com/user-attachments/assets/ed93aeb4-5380-4b76-825d-568464cd3745)

    - #### Colori della tastiera
      **Definisci lo schema di colori per le diverse ottave sulla tastiera virtuale:**
      
      **Colore della prima ottava:** Arancione chiaro
      
      **Colore della seconda ottava:** Azzurro chiaro
      
      **Colore della terza ottava:** Verde chiaro
      
      Queste impostazioni aiutano a distinguere visivamente le gamme di tonalità, agevolando sia l'esecuzione che la composizione.

    - #### Colori dei pulsanti e dei controlli
      **Personalizza l'aspetto degli elementi interattivi nell'interfaccia:**
      
      **Colore riga vuota:** Arancione chiaro
      
      **Colore note cancellate:** Blu
      
      **Colore linea deselezionata:** Ciano chiaro
      
      **Colore riga intera cancellata:** Rosso
      
      **Colore pulsanti di riproduzione:** Verde chiaro
      
      **Colore metronomo:** Azzurro chiaro
      
      **Colore markup tastiera:** Grigio chiaro
      
      Queste assegnazioni di colore migliorano l'usabilità rendendo azioni e stati visivamente intuitivi.
    
    - #### Colori degli indicatori
      **Imposta i colori per gli indicatori di feedback in tempo reale:**
      
      **Colore dell'indicatore acustico:** Rosso
      
      **Colore dell'indicatore di nota:** Rosso
      
      Questi indicatori lampeggiano o si illuminano durante la riproduzione o l'input, aiutandoti a monitorare l'attività a colpo d'occhio.

    - #### Impostazioni Testi/Eventi di Testo
      **Dimensioni Testi/Eventi di Testo:** Regola le dimensioni (in punti) dei testi o degli eventi di testo visualizzati durante la riproduzione di file MIDI o altre funzionalità basate su eventi.
    
      **Anteprima Impostazioni Testi/Eventi di Testo:** Utilizza questo pulsante per visualizzare in anteprima l'aspetto dei testi o degli eventi di testo, assicurandoti che la leggibilità e lo stile corrispondano alle tue preferenze.
    
    - #### Opzione di Reimpostazione
      **Ripristina le Impostazioni Aspetto ai Valori Predefiniti:** Un pulsante con un clic per ripristinare tutte le impostazioni di colore e aspetto ai valori predefiniti originali, perfetto per annullare esperimenti o ricominciare da capo.

  - ## Strumenti
    Questi strumenti compatti ma potenti nel menu `File` forniscono un rapido accesso a tre funzionalità principali di NeoBleeper, ciascuna progettata per semplificare il flusso di lavoro ed espandere le possibilità creative. Ogni opzione è abbinata a una scorciatoia da tastiera per un controllo rapido e pratico:
    
    ![image](https://github.com/user-attachments/assets/5305805f-c8b4-4a1d-9b66-d08b1245f711)

    - ### Riproduci file MIDI - `Ctrl + M`
      Carica e riproduci istantaneamente un file MIDI tramite altoparlante di sistema o dispositivo audio all'interno di NeoBleeper. Questa funzione è ideale per l'anteprima delle composizioni, per testare l'accuratezza della riproduzione o per integrare dati MIDI esterni nel tuo flusso di lavoro.

      ![image](https://github.com/user-attachments/assets/518d8923-103c-48d2-8e45-c235dafc7e7d)
      
      Scegli il file MIDI cliccando su "Sfoglia file" nella finestra "Impostazioni di riproduzione file MIDI". Il file MIDI selezionato appare nella casella di testo a sinistra del pulsante.
      
      Il tempo è visualizzato come "00:00.00" (minuti, secondi, centesimi di secondo). Si aggiorna solo quando il timer di riproduzione è attivo e i messaggi MIDI vengono riprodotti al momento corretto, a condizione che il tempo rimanga invariato.
      La percentuale indica la proporzione di messaggi MIDI elaborati. Ad esempio, se la prima metà contiene pochi messaggi e la seconda metà è più densa, la percentuale potrebbe non raggiungere il 50% fino a una fase avanzata della riproduzione. Una casella di controllo "Loop" consente al file MIDI di riavviarsi automaticamente al termine della riproduzione.
      I tre pulsanti sotto il cursore, da sinistra a destra, servono per il riavvolgimento (tornare all'inizio del file MIDI), la riproduzione (dalla posizione corrente) e l'arresto (senza riavvolgimento). Una casella di controllo sotto questi controlli consente la riproduzione in loop.
      
      In questa finestra, gli utenti possono selezionare canali specifici per l'input. I canali non selezionati verranno ignorati. Gli utenti possono selezionare o deselezionare le caselle di controllo e le modifiche avranno effetto immediato. Quando una casella di controllo è selezionata, le note del canale corrispondente verranno elaborate durante la riproduzione.
      
      Nella parte inferiore della finestra "Riproduci file MIDI", una griglia di rettangoli mostra le note tenute. Ogni rettangolo rappresenta una nota tenuta. È possibile visualizzare fino a 32 rettangoli contemporaneamente. Se vengono tenute più di 32 note, vengono visualizzate solo le prime 32.
      
      La modifica dell'impostazione "Cambia tra le note ogni ... mS" nella finestra "Riproduci file MIDI" influisce sulla velocità di riproduzione delle note ricevute dall'ingresso MIDI.
      
      Se la casella di controllo "Riproduci ogni nota solo una volta (non continuare ad alternare)" è selezionata, ogni nota viene riprodotta una volta per la durata specificata dall'impostazione "Cambia tra le note ogni ... mS". Questo produce un effetto più staccato.
      
      Se la casella di controllo "Prova a far durare ogni ciclo 30 mS (con tempo di alternanza massimo di 15 mS per nota)" è selezionata, la durata dell'alternanza viene regolata automaticamente per soddisfare questo comportamento temporale. Questo aiuta a mantenere una temporizzazione precisa quando più note vengono suonate in rapida successione.

      #### Visualizzazione di testi ed eventi testuali

      Il lettore di file MIDI di NeoBleeper include una funzione per visualizzare testi o eventi testuali incorporati nei file MIDI, fornendo un feedback visivo in tempo reale di linee vocali o spunti per applicazioni di karaoke e performance.
      
      ![image1](https://github.com/user-attachments/assets/4a4cf1b3-6117-4e3e-b4ea-c154db0d2a0f)
      
      Quando la casella di controllo "Mostra testi o eventi testuali" è abilitata nella finestra "Riproduci file MIDI", tutti i testi o gli eventi testuali incorporati nel file MIDI in riproduzione vengono visualizzati in modo evidente nella parte inferiore della finestra dell'applicazione. Questi eventi appaiono come sovrapposizioni di testo grandi e chiare, che si aggiornano in sincronia con la progressione del brano.
      
      Questa funzione è particolarmente utile per seguire le parti vocali, dare suggerimenti agli artisti dal vivo o semplicemente godersi una riproduzione in stile karaoke. Se il file MIDI non contiene testi o eventi testuali, la sovrapposizione rimane nascosta.
      
      La visualizzazione del testo/testo si aggiorna automaticamente man mano che si verificano nuovi eventi durante la riproduzione e scompare quando la riproduzione viene interrotta o quando viene caricato un nuovo file.
      
    - ### Crea musica con l'IA - `Ctrl + Alt + A`
      Sfrutta la potenza dell'IA per generare idee musicali. Che tu stia cercando ispirazione, colmando lacune o sperimentando nuovi stili, questo strumento offre suggerimenti intelligenti e contestuali per melodie, armonie e ritmi.

      ![image](https://github.com/user-attachments/assets/c548c21a-2b55-4d90-88cf-09fc2a5d1164)
      
      **Come funziona:**
      - Apri la finestra "Crea musica con l'IA" dal menu File o utilizzando la scorciatoia.
      - Scegli il **Modello di IA** desiderato (ad esempio, Gemini 2.5 Flash) dal menu a discesa.
      - Inserisci un suggerimento musicale nella casella **Prompt** (ad esempio, "Genera una melodia folk con chitarra acustica").
      - Fai clic su **Crea** per far generare la musica all'IA. Una barra di avanzamento indicherà quando la richiesta è in fase di elaborazione.
      - Un avviso ti ricorda che i risultati sono suggerimenti motivazionali e potrebbero contenere errori.
      - La funzionalità è basata su Google Gemini™.
      
      **Guida ai prompt e restrizioni dell'IA:**
      - Lo strumento di IA elaborerà solo i prompt relativi alla composizione musicale. Se il prompt non è correlato alla musica (ad esempio, "scrivi una barzelletta"), riceverai un errore:

        ![image](https://github.com/user-attachments/assets/20606e82-87af-468a-812a-de97aa14d4a4)
        
        *"Rilevato un prompt non musicale. Inserire un nome di brano, artista, compositore o qualsiasi termine correlato alla musica per una composizione. Suggerimenti per prompt validi: 'componi una canzone rock', 'Beethoven', "Yellow Submarine'."*
      - Non sono consentiti prompt con contenuti offensivi o inappropriati. Se rilevati, verrà visualizzato un errore:

        ![image3](https://github.com/user-attachments/assets/65912dc4-3b45-4aad-b7ae-f31c5b4861b5) 

        *"Profanita rilevata. Provare a richiedere una composizione musicale o musica relativa a un artista."*
      - I prompt validi devono essere specifici e incentrati sulla musica (ad esempio, "Genera una melodia jazz per pianoforte" o "Crea un pattern di batteria techno veloce").

     **Note:**
      - Se non viene scritto alcun prompt quando si clicca sul pulsante "Crea", l'IA utilizzerà il testo segnaposto del prompt nella casella di testo come prompt.
      - La musica generata dall'IA ha lo scopo di stimolare l'ispirazione e deve essere esaminata prima di essere utilizzata nelle composizioni finali.
      - L'IA non garantisce risultati perfetti o stilisticamente accurati.
      - Tutti i contenuti generati devono essere controllati per verificarne l'accuratezza e la musicalità prima dell'uso pubblico.
    
    **Integrazione con le opzioni di output:**
      - È possibile utilizzare la musica generata dall'IA con qualsiasi motore di output (segnale acustico di sistema, dispositivo audio o sistema vocale).
      - Assegnare la musica generata dall'IA a colonne di note specifiche e combinarla con le funzioni di sintesi vocale o tradizionale per risultati unici.
  
    - ### Converti in GCode - `Ctrl + Maiusc + G`

      Trasforma i dati musicali in GCode per buzzer o motori di macchine CNC o stampanti 3D. Questo colma il divario tra suono e movimento, consentendo rappresentazioni fisiche di sequenze musicali, perfette per l'arte sperimentale o strumenti didattici.

      ![immagine](https://github.com/user-attachments/assets/3a890e48-9527-4717-9106-98e8dfcb286b)
      
      Questa funzione converte le configurazioni di note musicali selezionate in istruzioni GCode da utilizzare con macchine CNC o stampanti 3D. È possibile definire fino a quattro note, ciascuna assegnata a un tipo di componente (motore M3/M4 e M300 per il buzzer). Le note possono essere attivate/disattivate singolarmente.
      
      L'ordine di riproduzione può essere configurato per alternare le note in sequenza o per parità di colonna (prima le colonne dispari, poi quelle pari).
      
      Una volta attivato, il sistema genera un GCode che attiva i componenti assegnati in base al pattern di note selezionato. Il timing e la modulazione sono determinati dalla logica di riproduzione.
      
      Utilizzare il pulsante "Esporta come GCode" per salvare l'output. Verificare la compatibilità con il computer di destinazione prima dell'esecuzione.

    - ### Converti in comando Beep per Linux - `Ctrl + Maiusc + B`

      Converti rapidamente le tue composizioni musicali in uno script di comandi beep compatibile con Linux per una facile riproduzione sui sistemi Linux.

      ![image1](https://github.com/user-attachments/assets/04cbf3c7-412c-4549-95d8-6a5277914563)
      
      **Panoramica delle funzionalità:**
      - NeoBleeper genera una sequenza di comandi beep che rappresentano la tua musica, formattati per l'utility `beep` di Linux.
      - Ogni nota e pausa viene tradotta in parametri beep appropriati (`-f` per la frequenza, `-l` per la lunghezza/durata, `-D` per il ritardo e `-n` per le note concatenate).
      - Il risultato è un singolo comando (o una serie di comandi) che può essere eseguito in un terminale Linux per riprodurre la tua musica utilizzando l'altoparlante di sistema.
      
      **Come si usa:**
      1. Componi la tua musica in NeoBleeper come di consueto.
      2. Apri lo strumento "Converti in comando Beep per Linux" dal menu File.
      3. La tua musica verrà convertita istantaneamente in uno script di comando Beep e visualizzata in un'area di testo.
      4. Utilizza il pulsante "Copia comando Beep negli appunti" per copiare il comando e utilizzarlo nel tuo terminale.
      5. In alternativa, salva il comando come file `.sh` cliccando su "Salva come file .sh" per eseguirlo in seguito su qualsiasi sistema Linux compatibile.
         
      **Esempio di output:**
        - Il comando potrebbe apparire così:
        ```
        beep -f 554 -l 195 -n -f 0 -l 0 -D 5 -n -f 523 -l 195 -n ...
        ```
        Ogni gruppo di parametri corrisponde a una nota musicale o a una pausa.
      
        **Integrazione e suggerimenti:**
        - Ideale per condividere musica con utenti Linux o per l'utilizzo in script shell.
        - Il comando è compatibile con l'utility standard `beep` di Linux (assicurarsi che sia installata e di disporre dei permessi per utilizzare l'altoparlante di sistema).
        - La modifica del comando generato consente di apportare rapide modifiche a tempo, tono o ritmo.
        
        Questa funzionalità semplifica il processo di importazione della musica in ambienti Linux e consente utilizzi creativi come notifiche musicali, sistemi di avviso o semplicemente l'ascolto delle composizioni al di fuori di NeoBleeper.
