# 🤝 Contribuire a NeoBleeper

Innanzitutto, grazie per aver preso in considerazione l'idea di contribuire a NeoBleeper! Il tuo contributo è fondamentale per il successo di questo progetto. Che tu stia segnalando un bug, proponendo una funzionalità, migliorando la documentazione, caricando un file BMM o NBPML legacy o inviando codice, il tuo coinvolgimento è molto apprezzato.

## 📑 Indice
1. [Codice di condotta](#-codice-codice-di-condotta)
2. [Come posso contribuire?](#%E2%80%8D%EF%B8%8F-come-posso-contribuire)
    - [Segnalazioni di bug](#-segnalazioni-di-bug)
    - [Richieste di funzionalità](#-richieste-di-funzionalit%C3%A0)
    - [Contributi al codice](#%E2%80%8D-contributi-al-codice)
    - [Documentazione](#-documentazione)
    - [Contributi ai file BMM e NBPML](#-contributi-per-file-bmm-e-nbpml)
3. [Processo di pull request](#%EF%B8%8F-procedura-per-le-pull-request)
4. [Guide di stile](#-guide-di-stile)
    - [Stile del codice](#-stile-del-codice)
    - [Note specifiche su C#](#-note-specifiche-su-c)
5. [Supporto della community](#%E2%80%8D%E2%80%8D%E2%80%8D-supporto-della-community)

## 🌟 Codice Codice di Condotta
Partecipando a questo progetto, accetti di rispettare il Codice di Condotta. Si prega di essere rispettosi e premurosi nei confronti degli altri membri della community. Consulta il file `CODE_OF_CONDUCT-it.md` per i dettagli.

## 🤝🙋‍♂️ Come posso contribuire?

### 🪲 Segnalazioni di Bug
Se hai trovato un bug in NeoBleeper, crea una segnalazione e includi i seguenti dettagli:
- Un titolo chiaro e descrittivo.
- La versione di NeoBleeper o l'hash del commit, se applicabile.
- I passaggi per riprodurre il problema o un frammento di codice.
- Comportamento previsto ed effettivo.
- Qualsiasi altro dettaglio rilevante, inclusi screenshot o registri di crash.

### 💭 Richieste di Funzionalità
Accogliamo volentieri le tue idee! Per richiedere una funzionalità:
1. Controlla le segnalazioni per vedere se qualcun altro l'ha già richiesta.
2. In caso contrario, apri una nuova richiesta e condividi una descrizione dettagliata che includa:
  - Contesto della richiesta.
  - Perché è utile.
  - Potenziali impatti, rischi o considerazioni.

### 👩‍💻 Contributi al codice
1. Crea un fork del repository e un nuovo branch da `main`. Assegna al branch un nome descrittivo, ad esempio `feature/add-tune-filter`.
2. Apri la cartella del repository in Visual Studio:
    - Assicurati di aver installato [Visual Studio](https://visualstudio.microsoft.com/) con i carichi di lavoro richiesti (ad esempio, ".NET desktop development" per NeoBleeper).
    - Clona il fork del repository sul computer locale (puoi utilizzare gli strumenti Git integrati di Visual Studio o la Git CLI).
    - Una volta clonato, apri il file della soluzione (`.sln`) in Visual Studio.
3. Installa i pacchetti NuGet:
    - Ripristina tutte le dipendenze necessarie cliccando su `Ripristina pacchetti NuGet` nella barra in alto o eseguendo `dotnet restore` dal terminale.
4. Aggiungi le modifiche:
    - Utilizza le funzionalità di Visual Studio come IntelliSense, debug e formattazione del codice per contribuire in modo efficace.
    - Assicurati che siano inclusi i test corretti e che tutti i test esistenti vengano superati.
    - Assicurati che il codice rispetti le linee guida di stile.
5. Aggiungi il tuo nome o nickname alla pagina Informazioni:
    - Apri il file `about_neobleeper.cs` e individua il componente `listView1`.
    - Seleziona il componente `listView1` nella finestra di progettazione di Visual Studio.
    - Fai clic sulla piccola freccia nell'angolo in alto a destra del componente per aprire il menu a discesa.
    - Seleziona **Modifica elementi** per aprire l'editor della raccolta di elementi ListView.
    - Aggiungi un nuovo `ListViewItem`:
      - Scrivi il tuo nome o nickname nella proprietà **Testo**. - Per i tuoi contributi/attività:
      - Individua la proprietà **SubItems**.
        - Fai clic sui tre punti (`...`) a destra del campo `(Collection)`.
        - Aggiungi o modifica il **SubItem** con una breve descrizione delle tue attività.
        - Se hai già aggiunto il tuo nome, modifica il SubItem o aggiorna la voce esistente prima di confermare le modifiche.
6. Testa il tuo codice:
    - Esegui i test utilizzando Test Explorer di Visual Studio.
    - Correggi eventuali test non riusciti e convalida le modifiche.
7. Esegui il commit delle modifiche con messaggi chiari e concisi.
    - Utilizza gli strumenti Git integrati di Visual Studio per lo staging e il commit delle modifiche.
8. Esegui il push del tuo branch e apri una pull request nel repository.
9. Sii pronto a collaborare con i revisori e a revisionare se necessario.

### 🧾 Documentazione
Migliorare la nostra documentazione è uno dei modi più semplici per contribuire! Sentiti libero di aggiungere o aggiornare esempi, chiarire sezioni o migliorare la leggibilità generale.

### 🎼 Contributi per file BMM e NBPML
NeoBleeper supporta i file legacy BMM (Bleeper Music Maker) e NBPML (NeoBleeper Project Markup Language). Se contribuisci o lavori con questi tipi di file, assicurati di quanto segue:
  - Verifica che i file BMM siano analizzati correttamente e visualizzati come previsto in NeoBleeper.
  - Verifica la compatibilità sia con i formati legacy che con l'implementazione corrente.
  - Per i file NBPML, mantieni la conformità alle specifiche più recenti del NeoBleeper Project Markup Language.

Se riscontri problemi specifici con questi formati di file, segui le linee guida nella sezione "Segnalazioni di bug". Sono benvenute anche richieste di funzionalità per un supporto avanzato per i file BMM e NBPML!

## ⬇️ Procedura per le Pull Request
Tutti gli invii devono essere effettuati tramite pull request. Ecco la procedura:
1. Compila il modello di pull request.
2. Assicurati che la tua pull request non duplichi quelle esistenti.
3. Aggiungi i dettagli sulle modifiche nella descrizione, facendo riferimento a problemi correlati ove possibile.
4. Gestisci tutti i commenti o le modifiche richieste dai revisori.
5. Le pull request devono superare tutti i controlli CI/CD, inclusi test e controlli di qualità del codice.

## 📖 Guide di stile
### ✨ Stile del codice
Segui le [.NET Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). I punti chiave includono:
  - Preferisci le proprietà automatiche ai campi pubblici.
  - Usa `var` per le variabili locali quando il tipo è ovvio.
  - Evita stringhe e numeri magici. Usa costanti o enumerazioni.

### 📒 Note specifiche su C#
  - Inserire `{` sulla stessa riga del codice precedente.
  - Utilizzare PascalCase per i nomi delle classi e dei metodi e camelCase per le variabili locali.
  - Seguire le [Linee guida Microsoft per la denominazione](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines).

## 👨‍👩‍👧‍👦 Supporto della community
Per qualsiasi domanda, non esitate ad aprire una discussione su GitHub o a contattarci tramite segnalazioni. Incoraggiamo tutti a condividere le proprie conoscenze e a dare una mano agli altri collaboratori.

Grazie per aver contribuito a NeoBleeper e per aver contribuito a creare qualcosa di incredibile!
