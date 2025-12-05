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


  ![image1](https://github.com/user-attachments/assets/ea3e2056-0c18-4424-ab97-79fd898f8179)
    
    **Der Systemlautsprecher piept nicht oder anders. Keine Aktion ausgeführt.**  
    Diese Meldung erscheint, wenn das Dienstprogramm den Systemlautsprecher überprüft und feststellt, dass dieser entweder keinen Piepton erzeugt oder auf eine Art und Weise piept, die vom Tool nicht gesteuert werden kann. In diesem Fall ergreift der Beep Stopper keine weiteren Maßnahmen.
    - *Tipp:* Wenn der Piepton weiterhin anhält, starten Sie Ihren Computer neu.

  ![image2](https://github.com/user-attachments/assets/be29f4de-cc1f-4a72-b9d4-7c53aa0ef94f)
    
    **Signalton erfolgreich gestoppt!**  
    Diese Meldung bestätigt, dass das Dienstprogramm „Beep Stopper“ einen feststeckenden Piepton erkannt und erfolgreich gestoppt hat. Es sind keine weiteren Maßnahmen erforderlich.

  ![image3](https://github.com/user-attachments/assets/8c2493b3-5938-4d56-9ce9-9c41fdba0809)
  
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

![image2](https://github.com/user-attachments/assets/08f58d2d-0dab-4c4b-a74c-bd6903906211)

![image3](https://github.com/user-attachments/assets/521316cf-d69a-4c4e-8727-3af322df805c)


- **Verfügbarkeit der Schaltfläche „Systemlautsprecher testen“:**
  Diese Option wird aktiviert, wenn NeoBleeper einen nutzbaren Systemlautsprecherausgang erkennt, einschließlich versteckter oder nicht PNP0800-Ausgänge.
- **Einstellung „Audiogerät zum Erzeugen eines Signaltons verwenden“:**
  Sie können diese Funktion jetzt deaktivieren, wenn ein versteckter oder nicht standardmäßiger Systemlautsprecherausgang erkannt wird.

#### Was bedeutet „nicht standardmäßiger Systemlautsprecherausgang“?
Manche moderne Computer, Laptops oder virtuelle Maschinen verfügen nicht über einen echten PC-Lautsprecher, oder die Signalführung ist nicht standardmäßig. NeoBleeper versucht nun, solche versteckten Systemlautsprecherausgänge (nicht als PNP0800-Geräte identifiziert) zu erkennen und zu nutzen, kann die Systemlautsprecheroption jedoch nur aktivieren, wenn sie auf Hardwareebene tatsächlich zugänglich ist. Wenn kein nutzbarer Ausgang gefunden wird, müssen Sie Ihr normales Audiogerät verwenden.

## 2.1 Systemlautsprecher-Ausgangstest (Ultraschallfrequenzerkennung)

NeoBleeper enthält jetzt einen neuen, erweiterten Hardwaretest zur Erkennung des Systemlautsprecherausgangs (auch PC-Lautsprecher genannt), selbst wenn das Gerät von Windows nicht erkannt wird (z. B. bei bestimmten IDs wie PNP0C02 statt PNP0800). Dieser Test verwendet Ultraschallfrequenzen (typischerweise 30–38 kHz, die nicht hörbar sind) und analysiert die elektrische Rückkopplung am Systemlautsprecheranschluss.

- **Funktionsweise:**
Beim Start führt NeoBleeper nach der üblichen Geräte-ID-Prüfung einen zweiten Schritt durch. Es sendet Ultraschallsignale an den Systemlautsprecheranschluss und überwacht die Hardware-Rückkopplung, um das Vorhandensein eines funktionierenden Lautsprecherausgangs zu erkennen – auch wenn dieser versteckt oder nicht standardmäßig ist.

- **Was Sie bemerken könnten:**
Auf einigen Systemen, insbesondere solchen mit Piezo-Summern, können während dieser Phase leise Klickgeräusche auftreten. Dies ist normal und zeigt an, dass der Hardwaretest ausgeführt wird.

![image4](https://github.com/user-attachments/assets/b6c4a7ea-5c7c-4c8e-ad70-9834f7610c33)

*Überprüfen der Systemlautsprecher (PC-Lautsprecher) in Schritt 2/2... (Klickgeräusche sind möglicherweise zu hören)*

- **Warum dieser Test?** Viele moderne Systeme besitzen keinen PNP0800-Systemlautsprecher, haben aber dennoch einen nutzbaren (versteckten) Lautsprecherausgang. NeoBleeper verwendet diese fortschrittliche Methode, um die Piepton-Funktion auf mehr Hardware zu aktivieren.

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

## 6. KI-Warnungen, Fehler und Fehlerbehebung der Google Gemini™ API

Die Funktion „Musik mit KI erstellen“ von NeoBleeper nutzt die Google Gemini™ API. Es können Fehlermeldungen oder Warnungen bezüglich der API-Verfügbarkeit, Nutzungsbeschränkungen oder Länderbeschränkungen auftreten.

### 6.1 Kontingent- oder Ratenbegrenzungsfehler (429 RESOURCE_EXHAUSTED)

![image1](https://github.com/user-attachments/assets/8a1428b8-c78e-4ce4-abb3-2d6c987a40e9)

**Mögliche Ursachen:**

- **Das API-Kontingent für Ihr Konto ist erschöpft.** Wenn Sie einen kostenlosen API-Schlüssel verwenden, sind bestimmte Modelle (z. B. `gemini-2.0-pro-exp`) möglicherweise nicht verfügbar oder unterliegen sehr niedrigen/festen Nutzungsbeschränkungen für kostenlose Konten.

- **Einschränkungen im kostenlosen Tarif:** Einige neuere generative Modelle (wie Gemini Pro Exp) stehen Nutzern des kostenlosen Tarifs *nicht* zur Verfügung. Bei dem Versuch, diese zu verwenden, tritt ein Kontingent- oder Verfügbarkeitsfehler auf.

- **Überschreitung der Ratenbegrenzungen:** Wenn Sie zu viele Anfragen zu schnell senden, können Sie die Ratenbegrenzungen der API erreichen, selbst mit einem kostenpflichtigen Tarif.

**Lösung:**
- **Überprüfen Sie Ihr API-Kontingent und Ihre Abrechnung:** Melden Sie sich in Ihrem Google Cloud-/Gemini-Konto an, um Ihre Nutzung zu überprüfen und Ihren Tarif gegebenenfalls zu aktualisieren.

- **Nur unterstützte Modelle verwenden:** Nutzer des kostenlosen Tarifs sind möglicherweise auf bestimmte Modelle beschränkt. Informationen zu verfügbaren Modellen finden Sie in der Dokumentation. Alternativ können Sie zu einem unterstützten Modell wechseln.

- **Warten und später erneut versuchen:** Manchmal kann ein kurzes Warten das Kontingent vorübergehend auffüllen, wie der Countdown in der Meldung anzeigt.

- **Bitte lesen Sie die [Gemini API-Dokumentation](https://ai.google.dev/gemini-api/docs/rate-limits) für aktuelle Nutzungsrichtlinien und Ratenbegrenzungen.**

---

### 6.2 Fehlerbehebung für sehr neue oder undokumentierte Gemini-Modelle (z. B. Gemini 3 Pro Preview)

Einige Gemini-Modelle – insbesondere brandneue Versionen wie **Gemini 3 Pro Preview** – werden möglicherweise bei Veröffentlichung nicht in der offiziellen Gemini-API-Preis- oder Kontingentdokumentation angezeigt. Es können Kontingent-, Zugriffs- oder „RESOURCE_EXHAUSTED“-Fehler auftreten, selbst wenn Ihr Gesamtkontingent scheinbar ungenutzt ist.

**Wichtige Hinweise zu sehr neuen Modellen:**

- Google beschränkt den Zugriff auf Preview-Modelle (wie Gemini 3 Pro Preview) häufig auf ausgewählte Konten oder bestimmte Regionen und kann deutlich strengere Anfrage- und Nutzungslimits festlegen.

- Kostenlose Konten haben möglicherweise kein Kontingent für diese Modelle, oder Anfragen werden vollständig blockiert.

- Das Modell ist möglicherweise erst mehrere Wochen nach Veröffentlichung in den Tabs für Kontingent/Preise oder in der Google-Dokumentation sichtbar.

- Preise, Zugriff und Verfügbarkeit neuer Gemini-Modelle können sich häufig ändern.

**Was tun bei Fehlern?**

- Überprüfen Sie Ihre [API-Nutzung und -Kontingente](https://ai.dev/usage?tab=rate-limit) und ob das neue Modell in Ihrer Konsole angezeigt wird.

- Lesen Sie die [Gemini-API-Dokumentation](https://ai.google.dev/gemini-api/docs/rate-limits). Beachten Sie jedoch, dass die Dokumentation möglicherweise nicht mit neu veröffentlichten Modellen übereinstimmt.

- Wenn Sie Fehler wie „RESOURCE_EXHAUSTED“ für ein Modell sehen, das nicht in den offiziellen Preistabellen aufgeführt ist, bedeutet dies wahrscheinlich, dass das Modell noch nicht allgemein verfügbar ist oder nur eingeschränkten Zugriff auf die Vorschau hat.

- Warten Sie, bis Google die Dokumentation aktualisiert und die Modelle breiter verfügbar gemacht hat, falls Sie diese experimentellen Modelle verwenden müssen.

> **Hinweis:**

> NeoBleeper und ähnliche Anwendungen können diese Beschränkungen nicht umgehen. Wenn Ihr Konto oder Ihre Region nicht berechtigt ist, müssen Sie warten, bis Google den Zugriff offiziell freigibt oder das Kontingent für Ihr gewähltes Gemini-Modell erhöht.

---

### 6.3 Regionale oder länderspezifische Einschränkungen

#### "API ist in Ihrem Land nicht verfügbar"

![image4](https://github.com/user-attachments/assets/45fc0002-ae4d-4c06-8ba5-37a9cdae536f)

Die Google Gemini™ API wird aufgrund regionaler oder rechtlicher Beschränkungen in einigen Regionen nicht unterstützt.

**Mögliche Gründe:**

- Die Verfügbarkeit der Gemini API ist in Ihrem Land eingeschränkt.

- Ihr API-Schlüssel ist in einer Region registriert, die keinen Zugriff hat.

**Lösung:**

- **Prüfen Sie die Liste der von der Google Gemini™ API unterstützten Länder in der offiziellen Dokumentation.**

- Wenn Sie sich in einem Land mit Einschränkungen befinden, können die KI-Funktionen nicht genutzt werden.

#### Regionsspezifischer Hinweis (Einstellungen)

![image3](https://github.com/user-attachments/assets/72494c96-ff61-4965-9189-b03800d8557e)

Im Europäischen Wirtschaftsraum, in der Schweiz oder im Vereinigten Königreich kann für die Gemini™ API ein kostenpflichtiges Google-Konto erforderlich sein.

- Wenn diese Warnung angezeigt wird, stellen Sie sicher, dass Sie Ihren Gemini API-Plan aktualisiert haben, bevor Sie die KI-Funktionen nutzen.

---

### 6.4 Allgemeine Hinweise zur KI-API

- Geben Sie nur Ihren eigenen API-Schlüssel ein; geben Sie ihn aus Sicherheitsgründen nicht weiter.

- NeoBleeper übermittelt Ihren API-Schlüssel nur direkt an den Gemini-Dienst, wenn dies für die Nutzung der Funktionen erforderlich ist.

- Wenn wiederholt Fehler auftreten, entfernen Sie Ihren API-Schlüssel und fügen Sie ihn erneut hinzu. Überprüfen Sie außerdem, ob Ihr Schlüssel aktiv ist.

---

## 7. Hinweise zu Systemlautsprechern und Sound für bestimmte Chipsätze (inkl. Intel B660)

### Wenn Sie keinen Ton hören, der Ton verzerrt ist oder der Systemlautsprecher unzuverlässig funktioniert:

Einige moderne Chipsätze – darunter die der Intel B660-Serie und neuer – können Probleme beim Initialisieren oder Reinitialisieren des Systemlautsprechers (PC-Piepton) haben, was zu Stille oder Tonproblemen führen kann.

**Hinweise für betroffene Benutzer:**

- **Versetzen Sie Ihren Computer in den Ruhemodus und wecken Sie ihn wieder auf.**

Dies kann helfen, den für den Systemlautsprecher zuständigen Hardware-Port neu zu initialisieren oder zurückzusetzen und die Pieptonfunktion wiederherzustellen.

- **Verwenden Sie die Funktion „Audiogerät zum Erzeugen eines Pieptons verwenden“** als Ausweichmöglichkeit, falls die Ausgabe des Systemlautsprechers unzuverlässig ist.

- **Prüfen Sie, ob BIOS- oder Firmware-Updates verfügbar sind:** Einige Mainboard-Hersteller veröffentlichen möglicherweise Updates, die die Kompatibilität des Lautsprecheranschlusses verbessern.

**Desktop-spezifisch:** Wenn Sie Systemlautsprecher hinzugefügt, entfernt oder neu angeschlossen haben, führen Sie einen vollständigen Neustart durch.

_Diese Problemumgehung ist in den Einstellungen hervorgehoben:_

![image2](https://github.com/user-attachments/assets/2b858348-c5e5-41a1-b381-4415b286b1f1)

> *Wenn Sie keinen Ton hören oder der Ton verzerrt ist, versetzen Sie Ihren Computer in den Ruhemodus und wecken Sie ihn anschließend wieder auf. Dies kann helfen, die Systemlautsprecher auf betroffenen Chipsätzen neu zu initialisieren.*

---

*Bei allen Ton- oder KI-bezogenen Problemen, die hier nicht behandelt werden, fügen Sie bitte Screenshots der Fehlermeldung, Details zu Ihrer PC-Hardware (insbesondere Hersteller und Modell des Motherboards/Chipsatzes) sowie Ihr Land/Ihre Region bei, wenn Sie Support anfordern oder ein GitHub-Issue erstellen.*

---

## 8. Häufig gestellte Fragen

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

### F: Was bedeutet die Warnung „Nicht standardmäßiger Systemlautsprecherausgang vorhanden“?
**A:** NeoBleeper hat Lautsprecherhardware erkannt, die nicht den gängigen PC-Lautsprecherstandards entspricht (z. B. kein PNP0800-Gerät). Dies kann ein „versteckter“ Lautsprecherausgang moderner Desktop-PCs oder virtueller Maschinen sein. In diesen Fällen funktionieren möglicherweise nicht alle Signaltonfunktionen zuverlässig. NeoBleeper versucht jedoch, jeden erkannten kompatiblen Ausgang zu verwenden.

### F: Warum wird die Schaltfläche „Systemlautsprecher testen“ angezeigt, obwohl Windows kein PC-Lautsprechergerät auflistet?
**A:** NeoBleeper erkennt versteckte oder nicht standardmäßige Systemlautsprecherausgänge. Wenn die Schaltfläche angezeigt wird, bedeutet dies, dass NeoBleeper einen potenziellen Hardwareanschluss für die Lautsprecherausgabe gefunden hat, auch wenn dieser von Windows nicht als Gerät erkannt wird.

### F: Ich verwende die Google Gemini™ API für KI-Funktionen und erhalte die Meldung „Kontingent erschöpft“ oder „API in Ihrem Land nicht verfügbar“. Was soll ich tun?
**A:** Lesen Sie Abschnitt 6 dieses Leitfadens. Stellen Sie sicher, dass Ihr API-Schlüssel und Ihr Abrechnungskonto/Kontingent gültig sind und Ihre Nutzung den regionalen Beschränkungen von Google entspricht. Wenn Sie sich in einer eingeschränkten Region befinden, stehen Ihnen KI-Funktionen möglicherweise nicht zur Verfügung.

### F: Ich habe ein System mit Intel B660 (oder neuer) und mein PC-Lautsprecher funktioniert manchmal nicht oder hängt sich auf. Ist das normal?
**A:** Bei einigen neueren Chipsätzen sind Kompatibilitätsprobleme mit der Neuinitialisierung des Systemlautsprechers bekannt. Versuchen Sie, Ihren Computer in den Ruhemodus zu versetzen und ihn dann wieder aufzuwecken oder verwenden Sie Ihr gewohntes Audiogerät. Prüfen Sie, ob BIOS-/Firmware-Updates verfügbar sind, die die Lautsprecherunterstützung verbessern könnten.

### F: Wie melde ich am besten Probleme mit Ton oder KI an den Support?
**A:** Geben Sie immer so viele Informationen wie möglich an: Hersteller und Modell Ihres Computers, Region, Screenshots von Fehlermeldungen und Ihre Datei „DebugLog.txt“ aus dem NeoBleeper-Ordner. Bei Problemen mit der KI geben Sie bitte den vollständigen Text der Fehlermeldungen an und beschreiben Sie Ihren Gemini-API-Kontostatus.

### F: Nach einem Absturz oder erzwungenem Schließen hat der Beep Stopper von NeoBleeper den Dauerton nicht gestoppt. Gibt es eine andere Möglichkeit, das Problem zu beheben?
**A:** Sollte der Beep Stopper nicht funktionieren, setzt ein Neustart Ihres Computers die Systemlautsprecher zurück und beendet den Dauerton.

### F: Kann ich den Beep Stopper bedenkenlos verwenden, wenn eine Warnmeldung über eine nicht standardmäßige oder fehlende Systemlautsprecherausgabe erscheint?
**A:** Ja, aber beachten Sie, dass das Dienstprogramm die Hardware möglicherweise nicht steuern kann und in seltenen Fällen zu Instabilität oder fehlender Wirkung führen kann. Wenn Sie sich unsicher sind, brechen Sie die Verwendung ab und starten Sie stattdessen Ihren Computer neu.

### F: Auf virtuellen Maschinen funktioniert der Systemlautsprecher überhaupt nicht. Handelt es sich um einen Fehler?
**A:** Nicht unbedingt. Viele virtuelle Maschinen emulieren PC-Lautsprecher nicht korrekt oder geben den Ton auf eine Weise aus, die sich nicht programmatisch steuern lässt. Verwenden Sie Ihr Standard-Audioausgabegerät für optimale Ergebnisse.

**Mögliche zukünftige Updates:**
Sollte NeoBleeper durch zukünftige Tests oder Entwicklungen defekte oder nicht angeschlossene Systemlautsprecher zuverlässig über den Ultraschall-Hardwaretest erkennen können, werden diese FAQ und die Erkennungslogik entsprechend aktualisiert. Weitere Informationen finden Sie in Änderungsprotokollen oder neuen Versionen.

---

## 9. Hilfe erhalten

- **Computer- und Umgebungsdetails angeben:** Wenn Sie Probleme mit der Hardwareerkennung oder dem Sound melden, geben Sie bitte Details zu Ihrem Computer (Desktop/Laptop, Hersteller/Modell, Betriebssystem) und der relevanten Hardware an.
- **Screenshots oder Fehlerdialoge anhängen:** Screenshots von Fehler- oder Warndialogen sind sehr hilfreich. Geben Sie genau an, wann das Problem auftritt.
- **Protokolldatei anhängen:** Ab neueren Versionen erstellt NeoBleeper eine detaillierte Protokolldatei namens „DebugLog.txt“ im Programmordner. Bitte hängen Sie diese Datei an, wenn Sie Hilfe benötigen, da sie hilfreiche Diagnoseinformationen enthält.
- **Schritte zur Reproduktion des Problems beschreiben:** Beschreiben Sie klar, was Sie getan haben, als das Problem auftrat.
- **Problem auf GitHub eröffnen:** Für weitere Unterstützung eröffnen Sie ein Problem auf GitHub und geben Sie alle oben genannten Details an, um bestmöglichen Support zu erhalten.

_Diese Anleitung wird aktualisiert, sobald neue Probleme und Lösungen entdeckt werden. Für weitere Unterstützung eröffnen Sie bitte ein Problem auf GitHub mit detaillierten Informationen zu Ihrem Setup und dem aufgetretenen Problem._
