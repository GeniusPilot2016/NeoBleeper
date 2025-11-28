# 🤝 Beitrag zu NeoBleeper

Zunächst einmal vielen Dank, dass Sie eine Mitarbeit bei NeoBleeper in Erwägung ziehen! Ihre Beiträge sind für den Erfolg dieses Projekts unerlässlich. Ob Sie einen Fehler melden, eine Funktion vorschlagen, die Dokumentation verbessern, eine ältere BMM- oder NBPML-Datei hochladen oder Code einreichen – Ihre Unterstützung ist uns sehr wichtig.

## 📑 Inhaltsverzeichnis
1. [Verhaltenskodex](#code-of-conduct)
2. [Wie kann ich mitwirken?](#how-can-i-contribute)

- [Fehlerberichte](#bug-reports)

- [Funktionsanfragen](#feature-requests)

- [Codebeiträge](#code-contributions)

- [Dokumentation](#documentation)

- [Beiträge zu BMM- und NBPML-Dateien](#bmm-and-nbpml-file-contributions)
3. [Pull-Request-Prozess](#pull-request-process)
4. [Styleguides](#style-guides)

- [Codestil](#code-style)

- [C#-spezifische Hinweise](#c-sharp-specific-notes)
5. [Community-Support](#community-support)

## 🌟 Verhaltenskodex
Durch Ihre Teilnahme an dieser Community erklären Sie sich mit den folgenden Bedingungen einverstanden: Mit der Teilnahme an diesem Projekt erklären Sie sich mit dem Verhaltenskodex einverstanden. Bitte verhalten Sie sich respektvoll und rücksichtsvoll gegenüber anderen Mitgliedern der Community. Details finden Sie in der Datei `CODE_OF_CONDUCT.md`.

## 🤝🙋‍♂️ Wie kann ich mitwirken?

### 🪲 Fehlerberichte
Wenn Sie einen Fehler in NeoBleeper gefunden haben, erstellen Sie bitte ein Issue und geben Sie die folgenden Details an:

- Einen klaren und aussagekräftigen Titel.

- Die Version von NeoBleeper oder den Commit-Hash (falls zutreffend).

- Schritte zur Reproduktion des Fehlers oder einen Codeausschnitt.

- Erwartetes und tatsächliches Verhalten.

- Alle weiteren relevanten Details, einschließlich Screenshots oder Crash-Logs.

### 💭 Funktionswünsche
Wir freuen uns über Ihre Ideen! So schlagen Sie eine Funktion vor:

1. Prüfen Sie die Issues, ob diese Funktion bereits vorgeschlagen wurde.

2. Falls nicht, erstellen Sie ein neues Ticket und beschreiben Sie es detailliert. Geben Sie dabei Folgendes an:

Hintergrund des Anliegens.

Warum es wertvoll ist.

Mögliche Auswirkungen, Risiken oder zu berücksichtigende Aspekte.

### 👩‍💻 Codebeiträge
1. Forken Sie das Repository und erstellen Sie einen neuen Branch von `main`. Benennen Sie Ihren Branch aussagekräftig, z. B. `feature/add-tune-filter`.

2. Öffnen Sie den Repository-Ordner in Visual Studio:

Stellen Sie sicher, dass Sie [Visual Studio](https://visualstudio.microsoft.com/) mit den erforderlichen Workloads (z. B. „.NET-Desktopentwicklung“ für NeoBleeper) installiert haben.

Klonen Sie Ihren Fork des Repositorys auf Ihren lokalen Rechner (Sie können die in Visual Studio integrierten Git-Tools oder die Git-Befehlszeilenschnittstelle verwenden).

Öffnen Sie nach dem Klonen die Projektmappendatei (`.sln`) in Visual Studio.

3. NuGet-Pakete installieren:

- Stellen Sie alle erforderlichen Abhängigkeiten wieder her, indem Sie in der oberen Leiste auf „NuGet-Pakete wiederherstellen“ klicken oder im Terminal „dotnet restore“ ausführen.

4. Änderungen hinzufügen:

- Nutzen Sie die Funktionen von Visual Studio wie IntelliSense, Debugging und Codeformatierung, um effektiv mitzuwirken.

- Stellen Sie sicher, dass alle erforderlichen Tests enthalten sind und erfolgreich durchlaufen werden.

- Achten Sie darauf, dass Ihr Code dem Styleguide entspricht.

5. Namen oder Spitznamen zur Infoseite hinzufügen:

- Öffnen Sie die Datei „about_neobleeper.cs“ und suchen Sie die Komponente „listView1“.

- Wählen Sie die Komponente „listView1“ im Visual Studio-Designer aus.

- Klicken Sie auf den kleinen Pfeil in der oberen rechten Ecke der Komponente, um das Dropdown-Menü zu öffnen.

- Wählen Sie „Elemente bearbeiten“, um den Editor für die ListView-Elemente zu öffnen.

- Fügen Sie ein neues „ListViewItem“ hinzu:

- Geben Sie Ihren Namen oder Spitznamen in die Eigenschaft „Text“ ein.

- Für Ihre Beiträge/Aufgaben:

Suchen Sie die **SubItems**-Eigenschaft.

Klicken Sie auf die drei Punkte (`...`) rechts neben dem Feld `(Collection)`.

Fügen Sie ein **SubItem** mit einer kurzen Beschreibung Ihrer Aufgaben hinzu oder bearbeiten Sie es.

Wenn Sie Ihren Namen bereits hinzugefügt haben, bearbeiten Sie das SubItem oder aktualisieren Sie Ihren bestehenden Eintrag, bevor Sie Ihre Änderungen speichern.

6. Testen Sie Ihren Code:

Führen Sie die Tests mit dem Test-Explorer von Visual Studio aus.

Beheben Sie alle fehlgeschlagenen Tests und validieren Sie Ihre Änderungen.

7. Speichern Sie Ihre Änderungen mit klaren und prägnanten Nachrichten.

Verwenden Sie die in Visual Studio integrierten Git-Tools, um Ihre Änderungen bereitzustellen und zu speichern.

8. Pushen Sie Ihren Branch und öffnen Sie einen Pull Request im Repository.

9. Seien Sie bereit, mit Reviewern zusammenzuarbeiten und Ihren Code bei Bedarf zu überarbeiten.

### 🧾 Dokumentation
Die Verbesserung unserer Dokumentation ist eine der einfachsten Möglichkeiten, beizutragen! Fügen Sie gerne Beispiele hinzu oder aktualisieren Sie diese, präzisieren Sie Abschnitte oder verbessern Sie die allgemeine Lesbarkeit.

### 🎼 Beiträge zu BMM- und NBPML-Dateien
NeoBleeper unterstützt ältere BMM- (Bleeper Music Maker) und NBPML-Dateien (NeoBleeper Project Markup Language). Wenn Sie zu diesen Dateitypen beitragen oder mit ihnen arbeiten, beachten Sie bitte Folgendes:

- Stellen Sie sicher, dass BMM-Dateien korrekt geparst und in NeoBleeper wie erwartet dargestellt werden.

- Testen Sie die Kompatibilität mit älteren Formaten und der aktuellen Implementierung.

- Halten Sie sich bei NBPML-Dateien an die aktuellen Spezifikationen der NeoBleeper Project Markup Language.

Sollten Sie Probleme mit diesen Dateiformaten feststellen, folgen Sie bitte den Richtlinien im Abschnitt „Fehlerberichte“. Funktionswünsche zur Verbesserung der Unterstützung von BMM- und NBPML-Dateien sind ebenfalls willkommen!

## ⬇️ Pull-Request-Prozess
Alle Beiträge erfolgen über Pull-Requests. So funktioniert es:
1. Füllen Sie die Pull-Request-Vorlage aus.

2. Stellen Sie sicher, dass Ihr Pull-Request keine bereits vorhandenen Pull-Requests dupliziert.

3. Beschreiben Sie Ihre Änderungen detailliert und verweisen Sie nach Möglichkeit auf verwandte Issues.

4. Berücksichtigen Sie alle Kommentare und Änderungswünsche der Reviewer.

5. Pull-Requests müssen alle CI/CD-Prüfungen bestehen, einschließlich Tests und Codequalitätsprüfungen.

## 📖 Styleguides
### ✨ Programmierstil
Befolgen Sie die [.NET-Codierungskonventionen](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions). Wichtige Punkte:

- Verwenden Sie automatische Eigenschaften anstelle von öffentlichen Feldern.

- Verwenden Sie `var` für lokale Variablen, wenn der Typ eindeutig ist.

- Vermeiden Sie magische Zeichenketten und Zahlen. Verwenden Sie Konstanten oder Enumerationen.

### 📒 C#-spezifische Hinweise

- Platzieren Sie `{` in derselben Zeile wie den vorhergehenden Code.

- Verwenden Sie PascalCase für Klassen- und Methodennamen und camelCase für lokale Variablen.

- Beachten Sie die [Microsoft-Namensrichtlinien](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines).

## 👨‍👩‍👧‍👦 Community-Support
Bei Fragen können Sie gerne eine GitHub-Diskussion eröffnen oder uns über die Issues kontaktieren. Wir ermutigen alle, ihr Wissen zu teilen und anderen Mitwirkenden zu helfen.

Vielen Dank für Ihren Beitrag zu NeoBleeper und Ihre Unterstützung beim Aufbau von etwas Großartigem!
