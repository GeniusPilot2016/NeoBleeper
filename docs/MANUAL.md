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
  
  - ### Synchronized Play Mod
    
    ![image](https://github.com/user-attachments/assets/58b19aec-1f6e-4ad4-b608-1c699bfa7d24)

    This mod enables NeoBleeper to begin playback at a specified system time. It is designed for synchronizing multiple instances of NeoBleeper, particularly when using separate NBPML or BMM files for different sections of a composition. By configuring each instance to start at the same time, synchronized playback across instances can be achieved.
    
    Activating the mod opens a configuration window. This window allows users to input a target start time (hour, minute, second) based on the system clock. The current system time is displayed for reference. By default, the target time is set to one minute ahead of the current time, but this value can be adjusted manually. Users can also specify whether playback should begin from the beginning of the music or from the currently selected line in the music list. The program will execute the corresponding playback command (“Play all” or “Play from selected line”) when the target time is reached.

    A control button is provided to initiate the waiting state. Once activated, the interface indicates that the program is waiting, and the button label changes to “Stop waiting.” If the program is not in a waiting state when the target time arrives, no playback will occur.

    The checkbox of "Synchronized Play" is unchecked window is closed. To reopen the window, Disabling this option will cancel any active waiting state.

    Playback should be stopped before initiating the waiting state. Starting the waiting process while music is already playing may result in unexpected behavior. If this occurs, stopping the waiting state is recommended.

    Synchronization across multiple computers is possible if all system clocks are precisely aligned. It is recommended to synchronize system clocks before using this feature across devices.

  - ### Play Beat Sound Mod
    
    ![image](https://github.com/user-attachments/assets/fd702244-279f-4cea-bb54-abae02960ded)

    This modification ensures the system speaker/sound device to emit a beat sound on every beat or on every other beat, depending on the selected configuration. The sound resembles a techno-style beat due to the electronic nature of the system speaker/sound device. When the "Play a beat sound" checkbox is selected, a configuration window appears. This window allows users to choose whether the beat sound plays on every beat or on every odd-numbered beat. The latter option effectively halves the tempo of the beat sounds.

    To demonstrate the tempo change, users can start the program, add four quarter-notes to the music list, enable the "Play a beat sound" option, and toggle between the two beat sound settings. The difference in tempo should be audible. The "Play beat sound" checkbox is unchecked when the configuration window is closed.
    
  - ### Bleeper Portamento Mod

    ![image](https://github.com/user-attachments/assets/850bedb0-1486-4306-95dc-f2b2491b3720)

    This modification causes the tone of the system speaker/sound device to transition gradually from the previous note to the current note. When the "Bleeper Portamento" checkbox is selected, a settings window appears. This window allows users to adjust the transition speed between notes, ranging from nearly instantaneous to extended durations. Users can also configure the note duration when clicked or set the note to continue playing indefinitely.

  - ### Use Keyboard As Piano Mod
 
    ![image](https://github.com/user-attachments/assets/7199d366-4655-4469-921c-ce5246260fd9)

    This feature maps the computer keyboard to musical notes, allowing direct playback using key presses without any MIDI input devices. Each key corresponds to a specific note on the virtual piano. The mapping follows a predefined layout, typically aligned with the visible keyboard labels.
    
When enabled, pressing a key will immediately trigger the associated note using the current synthesis method.

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
      
  - ## Tools
    These compact yet powerful tools in `File` menu provides quick access to three core features of NeoBleeper, each designed to streamline your workflow and expand creative possibilities. Each option is paired with a keyboard shortcut for fast, hands-on control:
    
    ![image](https://github.com/user-attachments/assets/745efab0-c7c7-4731-ae57-bde327243e6c)

    - ### Play MIDI File — `Ctrl + M`
      Instantly load and play a MIDI through system speaker or sound device file within NeoBleeper. This feature is ideal for previewing compositions, testing playback accuracy, or integrating external sequences into your workflow.
      
      ![image](https://github.com/user-attachments/assets/57e9e124-63a5-42fe-a1fe-ecb0974808c2)

      You choose the MIDI file by clicking "Browse File" on the "MIDI File Playback Settings" window. The selected MIDI file appears in the text box to the left of the button.

      Time is shown as "00:00.00" (minutes, seconds, hundredths of a second). It updates only when playback timer is tick and MIDI messages are played at the correct times, provided the tempo remains unchanged.
      Percentage indicates the proportion of MIDI messages that have been processed. For example, if the first half contains few messages and the second half is dense, the percentage may not reach 50% or may exceed 50% by the midpoint. The slider operates on the same principle, jumping to the most recent frame before the selected percentage.
      The three buttons below the slider, from left to right, are for rewinding (jumping to the beginning of the MIDI file), playing (from the current position), and stopping (without rewinding). A checkbox enables looping playback, causing it to restart from the beginning when it reaches the end.

      In this window, users can select specific channels for input. Channels not selected will be ignored. Users can tick or untick the checkboxes, and changes take effect immediately. When a checkbox is unticked, any currently playing notes on that channel are stopped. This prevents notes from continuing indefinitely due to missing "note off" messages.
    
      At the bottom of the "Play MIDI File" window, a grid of rectangles displays held notes. Each rectangle represents one note being held. Up to 32 rectangles can be shown simultaneously. If more than 32 notes are held, the display shows "(x more)", where "x" is the number of additional notes. As the program cycles through each held note, the rectangle corresponding to the currently played note lights up in bright red. Rectangles for notes not currently played are shown in dark red which can be customized from "Appearance" tab in "Settings".
  
      Modifying the "Switch between notes every ... mS" setting in the "Play MIDI File" window affects the cycling speed of notes received from MIDI input.

      If the "Only play each note once (don't keep alternating)" checkbox is ticked, each note is played once for the duration specified by the "Switch between notes every ... mS" setting. This produces short, percussive sounds rather than continuous tones.

      If the "Try to make each cycle last 30 mS (with max alternating time capped to 15mS per note)" checkbox is ticked, the alternate length is automatically adjusted to meet this timing behavior. This checkbox is enabled by default.

    - ### Create Music with AI — `Ctrl + Alt + A`
      Harness the power of AI to generate musical ideas. Whether you're seeking inspiration, filling in gaps, or experimenting with new styles, this tool offers intelligent, context-aware suggestions to kickstart your creativity.
   
      ![image](https://github.com/user-attachments/assets/5668346d-e216-4d73-80aa-a3b532a24d02)

      This feature generates music based on a user-defined text prompt. The selected AI model interprets the prompt and produces a musical composition. The result is rendered internally and can be previewed or exported. If the AI model supports genre or instrumentation awareness, those elements may be reflected in the output. Prompt processing is powered by Google Gemini™. No manual note input is required.

    - ### Convert to GCode — `Ctrl + Shift + G`

      Transform musical data into GCode for buzzers or motors of CNC machines or 3D printers. This bridges the gap between sound and motion, enabling physical representations of musical sequences—perfect for experimental art or educational tools.

      ![image](https://github.com/user-attachments/assets/544a1664-56ee-4b67-9cde-cdf5dfbda767)

      This feature converts selected musical note configurations into GCode instructions for use with CNC machines or 3D printers. Up to four notes can be defined, each assigned to a component type (M3/M4 motor and M300 for buzzer). Notes can be toggled individually.
      
      Playback order can be configured to alternate notes sequentially or by column parity (odd-numbered columns first, then even-numbered).

      When activated, the system generates GCode that triggers the assigned components in accordance with the selected note pattern. Timing and modulation are determined by the playback logic.

      Use the "Export As GCode" button to save the output. Ensure compatibility with your target machine before execution.






