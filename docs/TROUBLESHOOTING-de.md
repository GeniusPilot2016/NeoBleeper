# NeoBleeper Fehlerbehebung

Diese Anleitung bietet Lösungen für häufige Probleme bei der Verwendung von NeoBleeper, insbesondere im Zusammenhang mit dem Verhalten des Systemlautsprechers, der Tonausgabe, der Hardwarekompatibilität und anhaltendem Piepen.

---

## 1. Ton bleibt nach Absturz oder erzwungenem Schließen im Systemlautsprecher hängen

**Problem:**
Wenn NeoBleeper abstürzt oder erzwungen geschlossen wird, während die Audiowiedergabe über den Systemlautsprecher (PC-Lautsprecher) läuft, kann der Ton hängen bleiben, was zu einem anhaltenden Piepen oder Summen führt.

**Ursache:**
Der Systemlautsprecher wird auf niedriger Hardware-/Softwareebene gesteuert. Wenn die Anwendung den Lautsprecher beim Beenden nicht ordnungsgemäß freigibt oder zurücksetzt, kann der Ton bestehen bleiben.

**Lösungen:**
- **NeoBleeper Signalton-Stopper verwenden (für die 64-Bit-Version):**
  NeoBleeper enthält im Programmordner das Tool „NeoBleeper Signalton-Stopper“.
  
  ![image4](https://github.com/user-attachments/assets/b4f2bfce-ce16-4377-8884-5485ac04f9a8)
  
  - Starten Sie dieses Tool und drücken Sie die Schaltfläche **Signalton stoppen**, um den feststeckenden Piepton aus dem Systemlautsprecher zu stoppen.
  - Verwenden Sie dieses Tool nur, wenn der Piepton nach einem Absturz oder einem erzwungenen Beenden weiterhin ertönt.

  #### Signalton-Stopper-Meldungen und ihre Bedeutung

  Wenn Sie das Signalton-Stopper-Dienstprogramm verwenden, werden möglicherweise die folgenden Meldungen angezeigt:

  ![image1](https://github.com/user-attachments/assets/e810ada3-e70d-4920-97d5-a3906ca58e45)
    
    **Der Systemlautsprecher piept nicht oder anders. Keine Aktion ausgeführt.**  
    Diese Meldung erscheint, wenn das Dienstprogramm den Systemlautsprecher überprüft und feststellt, dass dieser entweder keinen Piepton erzeugt oder auf eine Art und Weise piept, die vom Tool nicht gesteuert werden kann. In diesem Fall ergreift der Beep Stopper keine weiteren Maßnahmen.
    - *Tipp:* Wenn der Piepton weiterhin anhält, starten Sie Ihren Computer neu.

  ![image2](https://github.com/user-attachments/assets/7b72bde9-739c-494d-919d-fd24615afdfc)
    
    **Signalton erfolgreich gestoppt!**  
    Diese Meldung bestätigt, dass das Dienstprogramm „Beep Stopper“ einen feststeckenden Piepton erkannt und erfolgreich gestoppt hat. Es sind keine weiteren Maßnahmen erforderlich.

  ![image3](https://github.com/user-attachments/assets/319f662d-feda-46f0-9cc4-a24474958e0f)
  
    **Es ist kein Systemlautsprecherausgang vorhanden oder es liegt ein nicht standardmäßiger Systemlautsprecherausgang vor. Der Signaltonstopper kann zu Instabilität oder unerwünschtem Verhalten führen. Möchten Sie fortfahren?**  
    Diese Meldung wird angezeigt, wenn das Dienstprogramm „Signalton-Stopper“ gestartet wird und feststellt, dass Ihr System entweder nicht über einen Standard-Systemlautsprecher (PC-lautsprecher) verfügt oder der Systemlautsprecherausgang nicht dem Standard entspricht. In diesem Fall warnt Sie das Dienstprogramm, dass die Verwendung von „Signalton-Stopper“ möglicherweise nicht wie erwartet funktioniert und zu unerwartetem Verhalten oder Instabilität führen kann.

    Wenn Sie fortfahren, versucht das Tool, den Piepton zu stoppen. Dies kann jedoch wirkungslos sein oder Nebenwirkungen haben, wenn Ihre Hardware nicht unterstützt wird oder nicht dem Standard entspricht.
    Wenn Sie nicht fortfahren, wird das Tool beendet, ohne Änderungen vorzunehmen.
    - *Tipp:* Wenn Sie diese Meldung erhalten, bedeutet dies, dass Ihr Computer nicht über einen kompatiblen Systemlautsprecher verfügt oder dessen Ausgabe nicht zuverlässig gesteuert werden kann. Piep- oder Brummgeräusche stammen wahrscheinlich von einem anderen Audiogerät (z. B. Ihren Hauptlautsprechern oder Kopfhörern). Beheben Sie Tonprobleme mit den Standardeinstellungen Ihres Audiogeräts und schließen Sie alle Anwendungen, die unerwünschten Ton erzeugen. Sollte das Problem weiterhin bestehen, starten Sie Ihren Computer neu oder überprüfen Sie die Toneinstellungen Ihres Geräts.
      
- **Starten Sie Ihren Computer neu:**
  Wenn der Signalton-Stopper das Problem nicht behebt, wird die Lautsprecherhardware durch einen Systemneustart zurückgesetzt.

- **Vorbeugung:**
  Schließen Sie NeoBleeper immer wie gewohnt. Vermeiden Sie das erzwungene Schließen über den Task-Manager oder ähnliche Tools, während der Ton abgespielt wird.
---

## 2. Systemlautsprechererkennung und -kompatibilität

NeoBleeper verfügt über eine Erkennungslogik, die prüft, ob Ihr System über einen Standard-PC-Lautsprecherausgang verfügt. Außerdem unterstützt es versteckte Systemlautsprecherausgänge (z. B. solche ohne PNP0800-ID). Wenn Ihre Hardware keinen Standard- oder versteckten Systemlautsprecher unterstützt oder der Ausgang nicht standardmäßig und nicht nutzbar ist, werden möglicherweise Warnmeldungen angezeigt oder Sie müssen für die Signaltöne auf Ihr normales Audiogerät zurückgreifen. Ab den neuesten Versionen zwingt NeoBleeper Sie jedoch nicht mehr dazu, ausschließlich das Audiogerät zu verwenden, wenn ein Standardlautsprecher fehlt. Es ermöglicht nun die Nutzung versteckter/nicht PNP0800-Systemlautsprecherausgänge, falls vorhanden.

### Beispielwarnung (Bild 1):

![image1](https://github.com/user-attachments/assets/d75c2792-dc52-4ffc-99c2-f3b88690fca6)

> **Erklärung:**
> Das Motherboard Ihres Computers verfügt entweder nicht über einen Standard-Systemlautsprecherausgang oder der Ausgang ist nicht standardmäßig. NeoBleeper versucht, versteckte Systemlautsprecherausgänge, die nicht als PNP0800 gekennzeichnet sind, zu erkennen und anzubieten. Ist ein solcher Ausgang verfügbar, können Sie den Systemlautsprecher auch bei dieser Warnung verwenden. Andernfalls greift NeoBleeper auf Ihr normales Audiogerät (z. B. Lautsprecher oder Kopfhörer) zurück.

### Einstellungsdialoge (Bilder 2 und 3):

![image2](https://github.com/user-attachments/assets/2638fee1-aec0-4cfd-a6db-7976b3ff6813)

![image3](https://github.com/user-attachments/assets/1df6c197-8e94-4eae-8fe7-cdfd9bab6ba5)


- **Verfügbarkeit der Schaltfläche „Systemlautsprecher testen“:**
  Diese Option wird aktiviert, wenn NeoBleeper einen nutzbaren Systemlautsprecherausgang erkennt, einschließlich versteckter oder nicht PNP0800-Ausgänge.
- **Einstellung „Audiogerät zum Erzeugen eines Signaltons verwenden“:**
  Sie können diese Funktion jetzt deaktivieren, wenn ein versteckter oder nicht standardmäßiger Systemlautsprecherausgang erkannt wird.

#### Was bedeutet „nicht standardmäßiger Systemlautsprecherausgang“?
Manche moderne Computer, Laptops oder virtuelle Maschinen verfügen nicht über einen echten PC-Lautsprecher, oder die Signalführung ist nicht standardmäßig. NeoBleeper versucht nun, solche versteckten Systemlautsprecherausgänge (nicht als PNP0800-Geräte identifiziert) zu erkennen und zu nutzen, kann die Systemlautsprecheroption jedoch nur aktivieren, wenn sie auf Hardwareebene tatsächlich zugänglich ist. Wenn kein nutzbarer Ausgang gefunden wird, müssen Sie Ihr normales Audiogerät verwenden.

---

## 3. ARM64-Unterstützung und Einschränkungen

**ARM64-basierte Geräte:**
Auf Windows-ARM64-Systemen sind der Test „Systemlautsprecher“ und das Kontrollkästchen „Audiogerät zur Erzeugung von Pieptönen verwenden“ in NeoBleeper **nicht verfügbar**. Stattdessen werden alle Pieptöne und Tonausgaben immer über Ihr Standard-Audiogerät (Lautsprecher oder Kopfhörer) ausgegeben.

- Die Schaltfläche „Systemlautsprecher testen“ und die zugehörigen Erkennungsfunktionen sind in den Einstellungen von ARM64-Geräten **nicht** sichtbar.

- Die Option „Audiogerät zur Erzeugung von Pieptönen verwenden“ ist nicht vorhanden, da dieses Verhalten automatisch erzwungen wird.

- Diese Einschränkung besteht, da auf ARM64-Windows-Plattformen kein direkter Zugriff auf die PC-/Systemlautsprecherhardware möglich ist.

- Auf ARM64 hören Sie Pieptöne immer über Ihr normales Audioausgabegerät.

**Wenn Sie einen ARM64-Rechner verwenden und die Systemlautsprecheroptionen in NeoBleeper nicht sehen, ist dies beabsichtigt und kein Fehler.**

---

## 4. So prüfen Sie, ob Systemlautsprecher vorhanden sind

- **Desktop-Computer:** Die meisten älteren Desktop-Computer verfügen über einen PC-Lautsprecheranschluss auf dem Motherboard. Neuere Systeme verfügen möglicherweise nicht über diese Funktion oder stellen den Ausgang in einer versteckten/nicht PNP0800-fähigen Form dar, die NeoBleeper nun nutzen kann.
- **Laptops:** Die meisten Laptops verfügen über keinen separaten Systemlautsprecher; der gesamte Ton wird über das Haupt-Audiosystem geleitet.
- **Virtuelle Maschinen:** Die Emulation von Systemlautsprechern ist häufig nicht vorhanden oder unzuverlässig; Nicht-PNP0800-Ausgänge sind möglicherweise nicht verfügbar.
- **So erkennen Sie das Problem:** Wenn Sie die oben genannten Warnungen sehen, den Systemlautsprecher aber in NeoBleeper aktivieren und testen können, verfügt Ihr Computer wahrscheinlich über einen versteckten oder nicht standardmäßigen Ausgang.
  
---

## 5. Ich höre keinen Ton!

- **Überprüfen Sie Ihre NeoBleeper-Einstellungen:**
  Wenn Ihr Systemlautsprecher nicht verfügbar ist, stellen Sie sicher, dass Ihr Audiogerät (Lautsprecher/Kopfhörer) richtig ausgewählt ist und funktioniert.
- **Überprüfen Sie den Windows-Lautstärkemixer:**
  Stellen Sie sicher, dass NeoBleeper im Systemlautstärkemixer nicht stummgeschaltet ist.
- **Versuchen Sie es mit der Schaltfläche „Systemlautsprecher testen“:**
  Testen Sie damit Ihren PC-Lautsprecher.
- **Lesen Sie die Warnmeldungen:**
  NeoBleeper gibt Ihnen spezifische Anweisungen, wenn kein Zugriff auf Ihren Systemlautsprecher möglich ist.
  
---

## 6. Häufig gestellte Fragen

### F: Kann ich den Systemlautsprecher verwenden, wenn meine Hardware kein PNP0800-Gerät hat?
**A:** Ja! NeoBleeper versucht nun, versteckte oder nicht PNP0800-Systemlautsprecherausgänge zu erkennen und zu verwenden, sofern möglich. Bei Erfolg können Sie den Systemlautsprecher auch dann verwenden, wenn Windows kein Standardgerät meldet.

### F: Warum wird die Einstellung „Audiogerät zum Erzeugen eines Signaltons verwenden“ manchmal dauerhaft (in älteren Versionen)?
**A:** Wenn kein Standard-Systemlautsprecherausgang erkannt wird (in älteren Versionen), erzwingt NeoBleeper diese Einstellung, um die Tonausgabe weiterhin zu ermöglichen.

### F: Gibt es eine Problemumgehung für fehlende Systemlautsprecher?
**A:** Sie müssen Ihr normales Audiogerät (Lautsprecher/Kopfhörer) verwenden, wenn kein Standard-Systemlautsprecherausgang gefunden werden kann (in älteren Versionen).

### F: Was ist, wenn das Beep-Stopper-Tool den feststeckenden Signalton nicht stoppt?
**A:** Starten Sie Ihren Computer neu, um die Lautsprecherhardware zurückzusetzen, falls das Dienstprogramm „Beep Stopper“ fehlschlägt.

### F: Warum höre ich beim Start Klickgeräusche?
**A:** Während des erweiterten System-Lautsprecherausgangstests (Schritt 2) sendet NeoBleeper Ultraschallsignale an die Hardware, um versteckte oder nicht standardmäßige Lautsprecherausgänge zu erkennen. Bei manchen Systemen (insbesondere mit Piezo-Summern) kann dies zu leisen Klickgeräuschen führen. Dies ist normal und stellt kein Problem dar; es bedeutet lediglich, dass der Hardwaretest läuft.

### F: Kann der Ultraschall-Hardwaretest (Schritt 2) defekte (offene Leitung) oder nicht angeschlossene Systemlautsprecher erkennen?
**A:** Dies ist derzeit nicht getestet und unbekannt. Der Test prüft zwar auf elektrische Rückkopplung und Portaktivität, unterscheidet jedoch möglicherweise nicht zuverlässig zwischen einem physisch vorhandenen, aber defekten (offenen Leitung) oder nicht angeschlossenen Lautsprecher und einem fehlenden Lautsprecher. Ist der Lautsprecher vollständig defekt oder nicht angeschlossen (offene Leitung), kann der Test „false“ zurückgeben, was bedeutet, dass kein funktionsfähiger Ausgang erkannt wurde. Dieses Verhalten ist jedoch nicht garantiert und kann von der jeweiligen Hardware und dem Fehlermodus abhängen. Wenn Sie vermuten, dass Ihr Systemlautsprecher nicht funktioniert, empfehlen wir eine physische Überprüfung oder die Verwendung eines Multimeters.

### F: Warum sehe ich auf meinem ARM64-Gerät keine Optionen für Systemlautsprecher oder Signaltöne?

**A:** Auf Windows-ARM64-Systemen deaktiviert NeoBleeper die Einstellungen für Systemlautsprecher, da ARM64-Plattformen keinen direkten Zugriff auf die Hardware der Systemlautsprecher unterstützen. Alle Signaltöne werden über Ihr reguläres Audioausgabegerät (Lautsprecher oder Kopfhörer) wiedergegeben, und die Optionen „Systemlautsprecher testen“ und „Audiogerät zum Erstellen eines Signaltons verwenden“ sind automatisch ausgeblendet. Dieses Verhalten ist beabsichtigt und kein Fehler.

**Mögliche zukünftige Updates:**
Sollte NeoBleeper durch zukünftige Tests oder Entwicklungen defekte oder nicht angeschlossene Systemlautsprecher zuverlässig über den Ultraschall-Hardwaretest erkennen können, werden diese FAQ und die Erkennungslogik entsprechend aktualisiert. Weitere Informationen finden Sie in Änderungsprotokollen oder neuen Versionen.

---

## 7. Hilfe erhalten

- **Computer- und Umgebungsdetails angeben:** Wenn Sie Probleme mit der Hardwareerkennung oder dem Sound melden, geben Sie bitte Details zu Ihrem Computer (Desktop/Laptop, Hersteller/Modell, Betriebssystem) und der relevanten Hardware an.
- **Screenshots oder Fehlerdialoge anhängen:** Screenshots von Fehler- oder Warndialogen sind sehr hilfreich. Geben Sie genau an, wann das Problem auftritt.
- **Protokolldatei anhängen:** Ab neueren Versionen erstellt NeoBleeper eine detaillierte Protokolldatei namens „DebugLog.txt“ im Programmordner. Bitte hängen Sie diese Datei an, wenn Sie Hilfe benötigen, da sie hilfreiche Diagnoseinformationen enthält.
- **Schritte zur Reproduktion des Problems beschreiben:** Beschreiben Sie klar, was Sie getan haben, als das Problem auftrat.
- **Problem auf GitHub eröffnen:** Für weitere Unterstützung eröffnen Sie ein Problem auf GitHub und geben Sie alle oben genannten Details an, um bestmöglichen Support zu erhalten.

_Diese Anleitung wird aktualisiert, sobald neue Probleme und Lösungen entdeckt werden. Für weitere Unterstützung eröffnen Sie bitte ein Problem auf GitHub mit detaillierten Informationen zu Ihrem Setup und dem aufgetretenen Problem._
