# NeoBleeper Manual
- ## How To Use NeoBleeper?
  The two main areas are the keyboard (horizontal area at the top of the screen) and the music list (the listView near the bottom-right with three columns). 
  The keyboard is divided into three colored sections, each representing an octave. 
  To access notes outside the current range (higher than the green notes or lower than the red notes), adjust the 'Octave' value located to the right and left of the keyboard. 
  Increasing this value by one shifts the keyboard notes one octave higher.
  
  ![image](https://github.com/user-attachments/assets/34edbb36-bc79-4d49-8eae-3333337fee12)
  
  The music list contains lines of notes. Up to 4 notes may play simultaneously. 
  The seven columns in the music list are "Length" (duration of the note or notes on the line), the middle four columns for the four notes, the sixth column "Mod" (length modifier), and the seventh column "Art" (articulations). 
  As notes are clicked on the keyboard, they are played immediately through the system speaker and entered into the music list in the "Note 1" column by default. 
  This behavior can be changed to enter notes into either "Note 1", "Note 2" "Note 3" or "Note 4", allowing users to identify notes by listening. 
  Notes clicked on the keyboard are inserted above the currently selected line in the music list. 
  If no line is selected, notes are added to the end of the music. 
  To add notes to the end when a line is selected, click "Un-select line" (turquoise button to the right of the music list).
  
  ![image](https://github.com/user-attachments/assets/80093bf4-d36f-4178-b5db-409587089bd7)
  
  To switch between inserting a note or replacing the currently selected note, and to choose which of the two note columns a note is inserted into or replaces, use the options box under the left side of the keyboard titled "When key is clicked."
  This box also includes the option to play notes without inserting or replacing them.
  
  ![image](https://github.com/user-attachments/assets/e44a644d-e946-4c11-9c35-6a2228835274)

  To change the length of the inserted note, adjust the "Note length" value located under the middle of the keyboard.
  This drop-down menu allows selection of whole, 1/2 (half), 1/4 (quarter), 1/8, 1/16, or 1/32 notes.

  ![image](https://github.com/user-attachments/assets/55672d66-050a-4ead-99f0-40e7be6650ff)
  
  For example, to create a dotted half note, insert a half note and then a quarter note, or insert a half note and apply a "Dotted" length modifier.

  ![image](https://github.com/user-attachments/assets/445d05ef-09a9-4941-846b-239cc217a1a0)

  If a 1/32 note is not short enough, the tempo (BPM) of the song can be set up to 600 BPM, which is double the capability of most MIDI sequencers.
  This allows for increased tempo and compensating by making every other note twice as long.
  The tempo control is located to the bottom of the keyboard, just right of the time signature control.
  It sets the number of beats per minute and includes a "Metronome" button to audibly represent the beat speed.

  ![image](https://github.com/user-attachments/assets/0f766883-1f7e-48ee-967f-69a836eb0026)

  A folder named 'Music' is extracted from the ZIP file. It contains several save-files (NBPML) for this program, which can be loaded as demonstrations. Refer to the "Opening and Saving" section below for more information.

  ![image](https://github.com/user-attachments/assets/7d69785d-5e00-45f2-b529-24a869810a98)

- ## Alternating Between Notes 
  Since the system speaker can only play one note at a time, the program alternates between "Note 1", "Note 2", "Note 3" and "Note 4" if multiple columns contain a note on the same line in the music list.
  The speed of this alternation is controlled by the value entered in the "Switch between alternating notes every: ... mS" text box, located to the left of the music list.
  Also, the order of alternating notes are can be determined radio buttons under "Switch between alternating notes every: ... mS" text box.
  
  ![image](https://github.com/user-attachments/assets/00497206-77ae-48ca-8972-283a647e3c90)

- ## Note/Silence Ratio
  This setting defines the proportion of time a line in the music list produces sound versus silence. Adjusting this ratio can reduce continuous tone output.

  ![image](https://github.com/user-attachments/assets/abc11216-24e6-4445-afca-29ca5c094d34)
  
- ## Opening and Saving File
  Music and settings can be saved and loaded using "Open", "Save" and "Save As" buttons in the "File" button in menu bar. The save operation includes the music list and most configuration options.

  Saved files use the ".NBPML" extension and are XML text-based. These files can be edited with text editors such as Notepad.
  Also, NeoBleeper can open ".BMM" files, which is file format of Bleeper Music Maker, but it can't be overwrited and it should be saved as seperate ".NBPML" file.

  ![image](https://github.com/user-attachments/assets/da465b77-70a0-483a-9855-a6af17db9159)

 - ## Tips for While Editing Music
   Both NBPML and BMM files are text-based and compatible with standard text editors. Copy-and-paste and find-and-replace functions can assist with repetitive tasks or correcting errors.
  
   To erase a line, use the red "Erase whole line" button. To clear only one note column, use the two blue buttons in the same area.
    
   ![image](https://github.com/user-attachments/assets/82e09f78-fb6a-4700-aefa-3ffa5bdd7b6f)
  
   To replace note lengths, select the replace option and enable length replacement. Then click "Blank line" for each line to update the length without altering the notes.
  
   ![image](https://github.com/user-attachments/assets/1cda1de9-8b57-4597-97ad-736f34e28829)
  
   ![image](https://github.com/user-attachments/assets/4084d3eb-f58f-419e-8f6a-b0525e557b58)

- ## Playing Music
  Use the top green "Play all" button to play all notes in the music list. Playback loops to the start upon reaching the end, if checkbox below in "Blank Line" button is chcked. The middle green button plays from the currently selected line and loops back to it.
  The bottom green button stops playback after the current note finishes.
  
  ![image](https://github.com/user-attachments/assets/3eae9551-d017-4e4f-a318-3bd9772d27ad)

  Clicking a line in the music list plays that line by default. To modify this behavior or restrict playback to one note column, adjust the checkboxes in the "When a line is clicked" box under "Which notes in list to play" in the bottom-left of the main window.
  Similar checkboxes under "When the music is being played" control playback behavior during automatic playback.

  ![image](https://github.com/user-attachments/assets/52e7e563-040b-4723-9a9f-8d6b87d97406)

- ## Length Modifiers and Articulations

  NeoBleeper provides support for dotted and triplet notes and Staccato, Spiccato and Fermata. The "Mod" column in the music list displays "Dot" for dotted notes and "Tri" for triplet notes and the "Art" column in the music list displays "Sta" for Staccato,
  "Spi" for Spiccato and "Fer" for Fermata.
  
  ![image](https://github.com/user-attachments/assets/46012e79-ce6c-48c0-b3c1-b97df135616c)

  To apply a dotted modifier (1.5x original length), select a line and click the "Dotted" button above the music list. A dotted note equals the original length plus the next shortest note. For example, a dotted quarter note equals a quarter note plus an eighth note.

  To apply a triplet modifier (1/3 original length), select a line and click the "Triplet" button. Three triplet notes of the same length equal one note of the original length. A line cannot be both dotted and triplet.

  To apply a Staccato modifier (half of original length, then silence), select a line and click the "Staccato" button.
  
  To apply a Spiccato modifier (0.25x of original length, then silence), select a line and click the "Spiccato" button.

  To apply a Fermata modifier (double of original length), select a line and click the "Fermata" button. A line cannot be Staccato, Spiccato and Fermata in same time.
  
  To insert dotted, triplet, Staccato, Spiccato or Fermata notes, press the corresponding button and then click notes on the keyboard. During playback, the "Dotted", "Triplet", "Staccato", "Spiccato" and "Fermata" buttons activate automatically when such modifiers and
  articulations are encountered.

  ![image](https://github.com/user-attachments/assets/f208110a-622f-4e9a-a024-cda8fb91b715)

- ## Time Signature and Position Displays

  NeoBleeper provides "Time Signature" setting, located in the left of the BPM setting. It defines the number of beats per measure. This setting affects metronome  sound and position display behavior but does not alter playback sound.

  ![image](https://github.com/user-attachments/assets/7c206e6b-875e-467d-87e9-af5b634d38ed)
  
  Three position displays in the bottom-right corner show the current position in the music. The top display shows the measure, the middle shows the beat within    the measure, and the bottom shows a traditional representation using whole, half (1/2), quarter (1/4), 1/8, 1/16 or 1/32 notes.
  
  ![image](https://github.com/user-attachments/assets/8a864acd-a228-447c-a81c-d126ba8c992f)

  Lower time signatures result in faster changes in the top display. The middle display resets to 1 at the start of each new measure.
  
  The bottom display cannot represent positions more accurate than 1/32 notes. It shows "(Error)" with a red text for unsupported positions, such as those created  by dotted sixteenth notes (3/64). Once the position becomes divisible by a 1/32 note again, the display resumes normal operation.

  ![image](https://github.com/user-attachments/assets/db64f8b6-af9e-463a-8913-33b1fb5d3533)

  Triplet notes also affect display accuracy. After three triplet notes of the same length are entered, the position becomes divisible by a 1/32 note, restoring    display functionality.
  
  Triplet playback near the end of a long music list may require significant CPU resources. If performance issues occur, enable the "Do not update" checkbox below  the position displays to disable updates during playback. Editing mode updates remain active.
  
  Older BMM files created with versions prior to revision 127 of Bleeper Music Maker default to a time signature of 4 when opened in NeoBleeper. Changing and saving the time signature in .NBPML files preserves the setting.

- ## Debug Logging

  The program is debugged by launching it directly from Visual Studio. Logging and diagnostics are handled internally using Visual Studio's built-in tools, including the Output window, breakpoints, and diagnostic panels.

  External trigger files such as "logenable" and log files like "debuglog.txt" are no longer used. All relevant debugging information is displayed within the Visual Studio environment during development and testing.

- ## Mods

  The program includes several modifications that alter its behavior from the original design. These modifications are listed near the bottom-left of the screen, adjacent to the music list. Each mod has a checkbox to enable or disable it. If a checkbox cannot be unticked, closing the mod's window will disable the mod and untick the checkbox.

  Click the question-mark button next to a mod's checkbox to view a brief description of its function (available for most mods).

  ![image](https://github.com/user-attachments/assets/1df56aa4-d264-4d21-aeab-148fed995c1e)

- ## Settings
  The NeoBleeper Settings window is divided into four main tabs, each targeting a different aspect of the app’s configuration
  
  - ### General Settings
    This tab focuses on foundational preferences and system-level integration:
    
    ![image](https://github.com/user-attachments/assets/f216a677-b0af-4920-976b-b7081f246f0d)
    
    - #### General Appearance
      **Theme Selector:** Lets you choose between NeoBleeper’s custom themes or default to your operating system’s look.
      
      **Classic Bleeper Mode:** A legacy toggle for users who prefer the original interface or behavior.
      
    - #### Create Music With AI
      **Google Gemini™ API Key Field:**  Secure input for enabling AI-generated music features.
      
      **Security Warning:** Advises users not to share their API key.
      
      **Update/Reset Buttons:** Manage the API key lifecycle. The update button is disabled, likely pending valid input.
      
    - #### Testing System Speaker
      **Test Button:** Plays a beep to confirm speaker functionality.
      
      **Fallback Message:** Suggests using an alternative sound device if no sound is heard from system speaker.
  
  - ### Creating Sounds Settings
    This tab is dedicated to configuring how NeoBleeper generates audio beeps using your system’s sound capabilities. It offers both technical control and creative flexibility for shaping the tone and texture of the sounds you produce.

    ![image](https://github.com/user-attachments/assets/f35e02a4-c1e3-4d03-9b02-bc4db01f6cdd)
    
    - #### Use sound device to create beep:
      A checkbox that enables or disables the use of your system’s sound device for beep generation instead of system speaker. When unchecked, NeoBleeper uses system speaker for creating sound. Enabling this option allows for richer, waveform-based sound synthesis.
    
    - #### Beep Creation from Sound Device Settings
      - ##### Tone Waveform Selection
        **Choose the shape of the waveform used to generate beeps. Each option affects the timbre and character of the sound:**
        
        **Square (Default):** Produces a sharp, buzzy tone. Ideal for classic digital beeps and retro-style alerts.
        
        **Sine:** Smooth and pure tone. Great for subtle notifications or musical applications.
        
        **Triangle:** Softer than square, with a slightly hollow sound. Balanced between sharpness and smoothness.
        
        **Noise:** Generates random signal bursts, useful for sound effects like static, bursts, or percussion-like textures. 

  - ### Devices Settings
    
    This tab allows you to configure how NeoBleeper interacts with external MIDI hardware and virtual instruments. Whether you're integrating live input or routing output to a synth, this is where you define your signal flow
    
    ![image](https://github.com/user-attachments/assets/2cd17fd7-3c78-4f90-9e2e-2bc64e4eae85)

    - #### MIDI Input Devices
       **Use live MIDI input:** Enables real-time MIDI signal reception from external controllers or software. When checked, NeoBleeper listens for incoming MIDI messages to trigger sounds or actions.
  
       **Select MIDI Input Device:** A dropdown menu listing available MIDI input sources. Choose your preferred device to begin receiving MIDI data.
  
       **Refresh:** Updates the list of available input devices, useful when connecting new hardware or launching virtual MIDI ports.

    - #### MIDI Output Devices
      **Use MIDI output:** Activates MIDI transmission from NeoBleeper to external devices or virtual instruments.
      
      **Select MIDI Output Device:** Choose where NeoBleeper sends its MIDI signals. The default option is typically a general-purpose synth like Microsoft GS Wavetable Synth.
      
      **Channel:** Selects the MIDI channel (1–16) used for output. This allows routing to specific instruments or tracks in multi-channel setups.
      
      **Instrument:** Defines the General MIDI instrument used for playback. Options range from pianos and strings to synths and percussion, giving you control over the timbre of the output.
      
      **Refresh:** Updates the list of available output devices, ensuring newly connected gear is recognized.

  - ### Appearance Settings
    This tab gives you full control over the visual identity of NeoBleeper, allowing you to customize the colors of key interface elements for clarity, aesthetics, or personal flair. It’s organized into three intuitive sections:
    
    ![image](https://github.com/user-attachments/assets/eb7a2939-a106-4ec5-8f5e-af32fb5d8ed0)

    - #### Keyboard Colors
      **Define the color scheme for different octaves on the virtual keyboard:**
      
      **First Octave Color:** Light orange
      
      **Second Octave Color:** Light blue
      
      **Third Octave Color:** Light green

      These settings help visually distinguish pitch ranges, aiding both performance and composition.

    - #### Buttons and Controls Colors
      **Customize the look of interactive elements across the interface:**
      
      **Blank Line Color:** Light orange
      
      **Clear Notes Color:** Blue
      
      **Un-select Line Color:** Light cyan
      
      **Erase Whole Line Color:** Red
      
      **Playback Buttons Color:** Light green
      
      **Metronome Color:** Light blue
      
      **Keyboard Markup Color:** Light gray
      
      These color assignments enhance usability by making actions and states visually intuitive.
    
    - #### Indicator Colors
      **Set the colors for real-time feedback indicators:**
      
      **Beep Indicator Color:** Red
      
      **Note Indicator Color:** Red
      
      These indicators flash or highlight during playback or input, helping you monitor activity at a glance.
    
    - #### Reset Option
      **Reset Colors to Default Values:** A one-click button to restore all color settings to their original defaults—perfect for undoing experiments or starting fresh.




