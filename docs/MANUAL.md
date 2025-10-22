# NeoBleeper Manual
- ## How To Use NeoBleeper?
  The two main areas are the keyboard (horizontal area at the top of the screen) and the music list (the listView near the bottom-right with seven columns). 
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

  ![image](https://github.com/user-attachments/assets/d6f8f30e-7002-45c7-ae7b-e541c482baf9)

  - ### Note List Multi-Purpose Checkboxes
  
    NeoBleeper now features multi-purpose checkboxes in the note list, adding powerful editing and playback capabilities with a simple, intuitive interface.
  
    ![image1](https://github.com/user-attachments/assets/9bf6cd87-670e-4311-83e1-421d35d78ed3)

    **Key Features of Multi-Purpose Checkboxes:**
    - **Play Beat in Checked Lines (No Whole Number Limitation):**  
      When the "Play beat" feature or a beat-mod is enabled, beats are played only in the lines where the checkbox is checked. This allows for custom rhythmic patterns regardless of measure boundaries or whole-number positions—beats can play on any checked line for creative flexibility.
    - **Cutting and Copying Multiple Notes:**  
      To cut or copy several lines at once, check the desired lines, then use the "Cut" or "Copy" actions. All checked lines are included in the operation, enabling efficient batch editing.
    - **Erasing Multiple Notes:**  
      To erase several notes at once, check the lines you wish to remove and use the "Erase" button. This allows for rapid deletion of multiple notes, reducing repetitive work and minimizing mistakes.
    - **Voice Playback on Checked Lines:**  
      When using the voice synthesis system, you can restrict voice playback to only the checked lines. This makes it easy to highlight certain musical phrases or experiment with alternating between traditional beeps and voice synthesis within a song.
  
    **Usage Tips:**
    - You can select or deselect multiple lines in rapid succession to quickly configure which parts of your music are affected by each operation.
    - The multi-purpose checkboxes work in conjunction with other editing features, allowing for advanced batch operations without switching modes.
    - These checkboxes are independent of line selection for note entry—lines can be checked for batch actions even if another line is selected for editing.
  
    This addition streamlines both composition and editing, giving you more control over playback, voice effects, and batch manipulation of your music.
    
  To switch between inserting a note or replacing the currently selected note, and to choose which of the four note columns a note is inserted into or replaces, use the options box under the left side of the keyboard titled "When key is clicked."
  This box also includes the option to play notes without inserting or replacing them.

  ![image](https://github.com/user-attachments/assets/0e4d2ede-24f1-490a-bd49-d20d500d1492)

  To change the length of the inserted note, adjust the "Note length" value located under the middle of the keyboard.
  This drop-down menu allows selection of whole, 1/2 (half), 1/4 (quarter), 1/8, 1/16, or 1/32 notes.

  ![image](https://github.com/user-attachments/assets/55672d66-050a-4ead-99f0-40e7be6650ff)
  
  For example, to create a dotted half note, insert a half note and then a quarter note, or insert a half note and apply a "Dotted" length modifier.

  ![image](https://github.com/user-attachments/assets/bd5f13da-1f61-4763-be54-6fda1566e8de)

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

  ![image](https://github.com/user-attachments/assets/e1bf6628-de78-4e81-bcf3-9c7d18ca8e70)

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

  ![image](https://github.com/user-attachments/assets/046f03b9-61d3-44b2-bc4a-fabd90d42c28)

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
  
  ![image](https://github.com/user-attachments/assets/6a23a907-0f06-4802-b460-6cd5e406c737)

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

  From version 0.18.0 Alpha, NeoBleeper uses the `Logger` class for handling all logging and diagnostics. Logging output is saved to a file named `DebugLog.txt` located in the application's root directory. 

  The `Logger` class provides detailed runtime information, including errors, warnings, and general debug messages. This log file is automatically created and updated during the application's execution. 

  For advanced debugging, you can still launch NeoBleeper directly from Visual Studio to utilize its built-in tools, such as breakpoints and the Output window. However, the `DebugLog.txt` file ensures that logging is consistently available even outside of the Visual Studio development environment.

  External trigger files such as `logenable` and older diagnostic methods are no longer supported. All relevant information is now centralized in the `DebugLog.txt` file for ease of access and review.

- ## Mods

  The program includes several modifications that alter its behavior from the original design. These modifications are listed near the bottom-left of the screen, adjacent to the music list. Each mod has a checkbox to enable or disable it. If a checkbox cannot be unticked, closing the mod's window will disable the mod and untick the checkbox.

  Click the question-mark button next to a mod's checkbox to view a brief description of its function (available for most mods).
  
  ![image](https://github.com/user-attachments/assets/3490b6cc-dc60-4287-a2c2-497a84a80d2b)
  
  - ### Synchronized Play Mod
    
    ![image](https://github.com/user-attachments/assets/58b19aec-1f6e-4ad4-b608-1c699bfa7d24)

    This mod enables NeoBleeper to begin playback at a specified system time. It is designed for synchronizing multiple instances of NeoBleeper, particularly when using separate NBPML or BMM files for different sections of a composition. By configuring each instance to start at the same time, synchronized playback across instances can be achieved.
    
    Activating the mod opens a configuration window. This window allows users to input a target start time (hour, minute, second) based on the system clock. The current system time is displayed for reference. By default, the target time is set to one minute ahead of the current time, but this value can be adjusted manually. Users can also specify whether playback should begin from the beginning of the music or from the currently selected line in the music list. The program will execute the corresponding playback command ("Play all" or "Play from selected line") when the target time is reached.

    A control button is provided to initiate the waiting state. Once activated, the interface indicates that the program is waiting, and the button label changes to "Stop waiting." If the program is not in a waiting state when the target time arrives, no playback will occur.

    The checkbox of "Synchronized Play" is unchecked window is closed. To reopen the window, Disabling this option will cancel any active waiting state.

    Playback is stopped automatically when initiating the waiting state to prevent issues, which is different from original Bleeper Music Maker.

    Synchronization across multiple computers is possible if all system clocks are precisely aligned. It is recommended to synchronize system clocks before using this feature across devices.

  - ### Play Beat Sound Mod

    ![image](https://github.com/user-attachments/assets/b8426d40-877c-440b-b9bd-ea30d69f0fe5)

    This modification ensures the system speaker/sound device to emit a beat sound on every beat or on every other beat, depending on the selected configuration. The sound resembles a techno-style beat due to the electronic nature of the system speaker/sound device. When the "Play a beat sound" checkbox is selected, a configuration window appears. This window allows users to choose whether the beat sound plays on every beat or on every odd-numbered beat. The latter option effectively halves the tempo of the beat sounds.

    To demonstrate the tempo change, users can start the program, add four quarter-notes to the music list, enable the "Play a beat sound" option, and toggle between the two beat sound settings. The difference in tempo should be audible. The "Play beat sound" checkbox is unchecked when the configuration window is closed.
    
  - ### Bleeper Portamento Mod

    ![image](https://github.com/user-attachments/assets/850bedb0-1486-4306-95dc-f2b2491b3720)

    This modification causes the tone of the system speaker/sound device to transition gradually from the previous note to the current note. When the "Bleeper Portamento" checkbox is selected, a settings window appears. This window allows users to adjust the transition speed between notes, ranging from nearly instantaneous to extended durations. Users can also configure the note duration when clicked or set the note to continue playing indefinitely.

  - ### Use Keyboard As Piano Mod

    ![image](https://github.com/user-attachments/assets/e21e5879-23d8-4df2-a0d2-ca5a7f3e7de2)

    This feature maps the computer keyboard to musical notes, allowing direct playback using key presses without any MIDI input devices. Each key corresponds to a specific note on the virtual piano. The mapping follows a predefined layout, typically aligned with the visible keyboard labels.
    
  When enabled, pressing a key will immediately trigger the associated note using the current synthesis method.

  - ### Voice System ("Voice Internals")

    NeoBleeper now includes a powerful voice synthesis system, accessible through the "Voice Internals" window. This system enables advanced control over synthesized voices, including vowel formants, noise, and sybilance, allowing you to create human-like or experimental vocal sounds directly in your compositions.

    ![image](https://github.com/user-attachments/assets/664f5c25-330f-45a7-9fe5-9ff0bac76fc9)

      - #### **Accessing the Voice System**
      
        To open the Voice Internals window, look for the "Voice System" or "Voice Internals" option in the menu or in the output device selection for each note.
        Each note column (Note 1–4) can now be individually routed to the voice system, the traditional beep, or other output devices using the new "Output options" dropdowns.

      - #### **Voice Internals Window Overview**
      
      The Voice Internals window is organized into sections, each giving you fine control over different aspects of the synthesized voice:

      - ##### **Formant Control**
        
          There are four formant sliders, each representing a key resonance of the human vocal tract:
            - Adjust the **Volume** and **Frequency (Hz)** for each formant.
            - Preset buttons ("Open vowel", "Close front", etc.) allow quick selection of typical vowel sounds.
        
        - ##### **Oscillator Section**
        
          **Saw Vol** and **Noise Vol** sliders control the level of the sawtooth oscillator and noise source, forming the basis of the voice timbre.
          These can be mixed with the formant filters for a variety of synthetic and vocal effects.
        
        - ##### **Sybillance and Sybillance Masking**
        
          Four masking controls let you simulate sybilance or consonant effects by shaping noise components and masking frequencies.
          The "Cutoff Hz" slider sets a frequency cutoff for noise masking.
          
        ##### **Random Variations of Formants**
        
          - Pitch and range sliders introduce subtle random variation to formant frequencies, adding realism or special effects.
        
        - ##### **Output Options**
        
          - Assign which sound engine each note column uses:
            "System speaker/Sound device beep"
            "Voice system"
            ...and others as available.
        
          You can play a mix of voice-synthesized and system speaker/sound device beeps in a single song.
        
        - ##### **Key and Usage Notes**
        
          The window provides a legend for color coding and parameter abbreviations. 
          A dropdown lets you choose when to play the voice (all lines, specific lines, etc.).
        
      - #### **How to Use the Voice System**
      
        1. **Assign a Note to the Voice System**  
           In the "Output options" of the Voice Internals window or main interface, set a note column (e.g., Note 2) to "Voice system".
        2. **Edit Formants and Oscillator Settings**  
           Use the sliders and preset buttons to shape the vowel, timbre, and sibilance.
        3. **Playback**  
           When you play music, the selected note column(s) will use the voice synthesizer based on your settings.
        4. **Experiment**  
           Try different combinations, randomization ranges, and oscillator mixes for robotic, natural, or unique synthetic voices.
      
      - #### **Tips**
      
        - Mix and match: Assign some notes to the voice system and others to beeps for rich, layered soundtracks.
        - For best results, adjust formants to match the pitch of your notes.
        - Use the randomization sliders for more "human" irregularity or robotic artifacts.
        - The voice system can be used for experimental sound design, not just vocals.

- ## Settings
  The NeoBleeper Settings window is divided into four main tabs, each targeting a different aspect of the app's configuration
  
  - ### General Settings
    This tab focuses on foundational preferences and system-level integration:

    ![image](https://github.com/user-attachments/assets/96d082ed-c0e5-4dd2-8b18-56e7a20b5995)

    - #### Language
      **Language Selector:** Lets you choose language of NeoBleeper between English, German, French, Italian, Spanish, Turkish, Russian, Ukrainian and Vietnamese.
      
    - #### General Appearance
      **Theme Selector:** Lets you choose between NeoBleeper's custom themes or default to your operating system's look.
      
      **Classic Bleeper Mode:** A legacy toggle for users who prefer the original interface or behavior.
      
    - #### Create Music With AI
      **Google Gemini™ API Key Field:**  Secure input for enabling AI-generated music features.
      
      **Security Warning:** Advises users not to share their API key.
      
      **Update/Reset Buttons:** Manage the API key lifecycle. The update button is disabled, likely pending valid input.
      
    - #### Testing System Speaker
      **Test Button:** Plays a beep to confirm speaker functionality.
      
      **Fallback Message:** Suggests using an alternative sound device if no sound is heard from system speaker.
  
  - ### Creating Sounds Settings
    This tab is dedicated to configuring how NeoBleeper generates audio beeps using your system's sound capabilities. It offers both technical control and creative flexibility for shaping the tone and texture of the sounds you produce.

    ![image](https://github.com/user-attachments/assets/f35e02a4-c1e3-4d03-9b02-bc4db01f6cdd)
    
    - #### Use sound device to create beep:
      A checkbox that enables or disables the use of your system's sound device for beep generation instead of system speaker. When unchecked, NeoBleeper uses system speaker for creating sound. Enabling this option allows for richer, waveform-based sound synthesis.
    
    - #### Beep Creation from Sound Device Settings
      - ##### Tone Waveform Selection
        **Choose the shape of the waveform used to generate beeps. Each option affects the timbre and character of the sound:**
        
        **Square (Default):** Produces a sharp, buzzy tone. Ideal for classic digital beeps and retro-style alerts.
        
        **Sine:** Smooth and pure tone. Great for subtle notifications or musical applications.
        
        **Triangle:** Softer than square, with a slightly hollow sound. Balanced between sharpness and smoothness.
        
        **Noise:** Generates random signal bursts, useful for sound effects like static, bursts, or percussion-like textures. 

  - ### Devices Settings
    
    This tab allows you to configure how NeoBleeper interacts with external MIDI hardware, virtual instruments and other external hardwares. Whether you're integrating live input or routing output to a synth, this is where you define your signal flow
    
    ![image](https://github.com/user-attachments/assets/2cd17fd7-3c78-4f90-9e2e-2bc64e4eae85)

    - #### MIDI Input Devices
      **Use live MIDI input:** Enables real-time MIDI signal reception from external controllers or software. When checked, NeoBleeper listens for incoming MIDI messages to trigger sounds or actions.
  
      **Select MIDI Input Device:** A dropdown menu listing available MIDI input sources. Choose your preferred device to begin receiving MIDI data.
  
      **Refresh:** Updates the list of available input devices, useful when connecting new hardware or launching virtual MIDI ports.

    - #### MIDI Output Devices
      **Use MIDI output:** Activates MIDI transmission from NeoBleeper to external devices or virtual instruments.
      
      **Select MIDI Output Device:** Choose where NeoBleeper sends its MIDI signals. The default option is typically a general-purpose synth like Microsoft GS Wavetable Synth.
      
      **Channel:** Selects the MIDI channel (1/16) used for output. This allows routing to specific instruments or tracks in multi-channel setups.
      
      **Instrument:** Defines the General MIDI instrument used for playback. Options range from pianos and strings to synths and percussion, giving you control over the timbre of the output.
      
      **Refresh:** Updates the list of available output devices, ensuring newly connected gear is recognized.

    - #### Other Devices & Microcontroller Firmware

      NeoBleeper also supports interaction with various external hardware devices—such as buzzers, motors, and microcontrollers—which expands its capabilities beyond traditional MIDI devices. The **Other devices** group within the Devices Settings tab provides configuration options and firmware generation tools for these external components.

      ![image1](https://github.com/user-attachments/assets/9bf2a395-6cde-47a7-b1d9-b8da6ddbb583)

      **Microcontroller Firmware Generator:**
      - This feature allows you to quickly generate and copy ready-to-use firmware for microcontrollers (such as Arduino) directly from NeoBleeper.
      - The firmware enables control of hardware like buzzers and stepper motors, allowing your musical compositions to trigger physical actions and sounds.
      - You can select your microcontroller type (e.g., "Arduino (ino file)") from the dropdown menu.
      - The code window displays the generated firmware tailored for the selected device.
      - Click the "Copy Firmware to Clipboard" button to easily copy the code for uploading to your microcontroller.

      **Example Use Case:**
      - With this feature, you can synchronize music playback with hardware—such as activating buzzers or driving stepper motors—using the system's output signals or exported GCode.
      - The generated Arduino firmware includes serial command handling for device identification and motor speed control, making it easy to integrate NeoBleeper with robotics or custom installations.

      **Integration Tips:**
      - Combine NeoBleeper's GCode export with the microcontroller firmware to translate music into mechanical movements or audible outputs.
      - The "Other devices" group simplifies connecting your PC to external hardware, expanding creative possibilities for music-driven machines, kinetic performances, or experimental sound art.

      > For further details or troubleshooting, refer to the NeoBleeper support channels or documentation for your microcontroller.

  - ### Appearance Settings
    This tab gives you full control over the visual identity of NeoBleeper, allowing you to customize the colors of key interface elements for clarity, aesthetics, or personal flair. It's organized into sections for keyboard, buttons, indicators, and text event display.

    ![image5](https://github.com/user-attachments/assets/183112ef-b1fa-40b5-9af3-50779946e223)

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

    - #### Lyrics/Text Events Settings
      **Lyrics/Text Events Size:** Adjust the size (in points) of the lyrics or text events that are displayed during playback of MIDI files or other event-driven features.
      
      **Preview Lyrics/Text Event Settings:** Use this button to preview how lyrics or text events will appear, ensuring readability and style match your preference.

    - #### Reset Option
      **Reset Appearance Settings to Default Values:** A one-click button to restore all color and appearance settings to their original defaults, perfect for undoing experiments or starting fresh.
      
  - ## Tools
    These compact yet powerful tools in `File` menu provides quick access to three core features of NeoBleeper, each designed to streamline your workflow and expand creative possibilities. Each option is paired with a keyboard shortcut for fast, hands-on control:

    ![image](https://github.com/user-attachments/assets/96e98471-f754-4521-b1d1-183bd27ff48e)

    - ### Play MIDI File - `Ctrl + M`
      Instantly load and play a MIDI through system speaker or sound device file within NeoBleeper. This feature is ideal for previewing compositions, testing playback accuracy, or integrating external MIDI data into your workflow.

      ![image](https://github.com/user-attachments/assets/c5845de3-5d04-48d6-87af-6f8620be3e73)

      You choose the MIDI file by clicking "Browse File" on the "MIDI File Playback Settings" window. The selected MIDI file appears in the text box to the left of the button.

      Time is shown as "00:00.00" (minutes, seconds, hundredths of a second). It updates only when playback timer is tick and MIDI messages are played at the correct times, provided the tempo remains unchanged.
      Percentage indicates the proportion of MIDI messages that have been processed. For example, if the first half contains few messages and the second half is dense, the percentage may not reach 50% until late in the playback. A "Loop" checkbox allows the MIDI file to restart automatically when finished.
      The three buttons below the slider, from left to right, are for rewinding (jumping to the beginning of the MIDI file), playing (from the current position), and stopping (without rewinding). A checkbox below these controls enables looping playback.

      In this window, users can select specific channels for input. Channels not selected will be ignored. Users can tick or untick the checkboxes, and changes take effect immediately. When a checkbox is selected, the corresponding channel's notes will be processed during playback.
    
      At the bottom of the "Play MIDI File" window, a grid of rectangles displays held notes. Each rectangle represents one note being held. Up to 32 rectangles can be shown simultaneously. If more than 32 notes are held, only the first 32 are displayed.
  
      Modifying the "Switch between notes every ... mS" setting in the "Play MIDI File" window affects the cycling speed of notes received from MIDI input.

      If the "Only play each note once (don't keep alternating)" checkbox is ticked, each note is played once for the duration specified by the "Switch between notes every ... mS" setting. This produces a more staccato effect.

      If the "Try to make each cycle last 30 mS (with max alternating time capped to 15mS per note)" checkbox is ticked, the alternate length is automatically adjusted to meet this timing behavior. This helps to maintain precise timing when multiple notes are played in rapid succession.

      #### Displaying Lyrics and Text Events

      NeoBleeper's MIDI file player includes a feature to show lyrics or text events embedded in MIDI files, providing real-time visual feedback of vocal lines or cues for karaoke and performance applications.

      ![image1](https://github.com/user-attachments/assets/cbc132ed-34db-4d11-b874-5406f813b4e1)

      When the "Show lyrics or text events" checkbox is enabled in the "Play MIDI File" window, any lyric or text events embedded within the currently playing MIDI file are displayed prominently at the bottom of the application window. These events appear as large, clear text overlays, updating in synchronization with the song's progression.

      This feature is particularly useful for following along with vocal parts, cueing live performers, or simply enjoying karaoke-style playback. If the MIDI file contains no lyric or text events, the overlay remains hidden.

      The lyrics/text display updates automatically as new events are encountered during playback and will disappear when playback is stopped or when a new file is loaded.

    - ### Create Music with AI - `Ctrl + Alt + A`
      Harness the power of AI to generate musical ideas. Whether you're seeking inspiration, filling in gaps, or experimenting with new styles, this tool offers intelligent, context-aware suggestions for melodies, harmonies, and rhythms.

      ![image](https://github.com/user-attachments/assets/3713c655-4e73-4828-9c7d-b4da65afb825)

      **How it works:**
      - Open the "Create Music with AI" window from the File menu or by using the shortcut.
      - Choose your desired **AI Model** (e.g., Gemini 2.5 Flash) from the dropdown.
      - Enter a musical prompt in the **Prompt** box (e.g., "Generate a folk tune with acoustic guitar").
      - Click **Create** to have the AI generate music. A progress bar will indicate when the request is being processed.
      - A warning reminds you that results are inspirational suggestions and may contain mistakes.
      - The feature is powered by Google Gemini™.

      **Prompt Guidance and AI Restrictions:**
      - The AI tool will only process prompts related to music composition. If your prompt is not related to music (e.g., "write a joke"), you will receive an error:  

        ![image](https://github.com/user-attachments/assets/3741d41b-883a-4732-b80b-29939eb26492)
        
        *"The request is not related to music composition."*
      - Prompts that contain offensive or inappropriate content are not allowed. If detected, an error will appear:
        
        ![image3](https://github.com/user-attachments/assets/3fe0e1f7-ed7c-4dca-a327-7141d874432e) 

        *"Your prompt contains offensive or inappropriate content. Please try again with a different request."*
      - Valid prompts should be specific and musically focused (e.g., "Generate a jazz melody for piano" or "Create a fast techno drum pattern").

      **Notes:**
      - If no prompt is written when "Create" button is clicked, the AI will use placeholder prompt text in textbox as prompt.
      - AI-generated music is intended to spark inspiration and should be reviewed before using in final compositions.
      - The AI does not guarantee perfect or stylistically accurate results.
      - All generated content should be checked for accuracy and musicality before public use.

      **Integration with Output Options:**
      - You can use AI-generated music with any output engine (system beep, sound device, or voice system).
      - Assign AI music to specific note columns and combine with traditional or voice synthesis features for unique results.
        
    - ### Convert to GCode - `Ctrl + Shift + G`

      Transform musical data into GCode for buzzers or motors of CNC machines or 3D printers. This bridges the gap between sound and motion, enabling physical representations of musical sequences, perfect for experimental art or educational tools.

      ![image](https://github.com/user-attachments/assets/544a1664-56ee-4b67-9cde-cdf5dfbda767)

      This feature converts selected musical note configurations into GCode instructions for use with CNC machines or 3D printers. Up to four notes can be defined, each assigned to a component type (M3/M4 motor and M300 for buzzer). Notes can be toggled individually.
      
      Playback order can be configured to alternate notes sequentially or by column parity (odd-numbered columns first, then even-numbered).

      When activated, the system generates GCode that triggers the assigned components in accordance with the selected note pattern. Timing and modulation are determined by the playback logic.

      Use the "Export As GCode" button to save the output. Ensure compatibility with your target machine before execution.

    - ### Convert to Beep Command for Linux - `Ctrl + Shift + B`

      Quickly convert your musical compositions into a Linux-compatible beep command script for easy playback on Linux systems.

      ![image1](https://github.com/user-attachments/assets/bb76a21f-516f-4a3e-832a-a31c67975ba4)

      **Feature Overview:**
      - NeoBleeper generates a sequence of beep commands representing your music, formatted for the Linux `beep` utility.
      - Each note and rest is translated into appropriate beep parameters (`-f` for frequency, `-l` for length/duration, `-D` for delay, and `-n` for chaining notes).
      - The result is a single command (or a series of commands) that can be executed in a Linux terminal to reproduce your music using the system speaker.

      **How To Use:**
      1. Compose your music in NeoBleeper as usual.
      2. Open the "Convert to Beep Command for Linux" tool from the File menu.
      3. Your music will be instantly converted into a beep command script and displayed in a text area.
      4. Use the "Copy Beep Command to Clipboard" button to copy the command for use in your terminal.
      5. Alternatively, save the command as a `.sh` file by clicking "Save As .sh File" for later execution on any compatible Linux system.

      **Example Output:**
      - The command may look like:
        ```
        beep -f 554 -l 195 -n -f 0 -l 0 -D 5 -n -f 523 -l 195 -n ...
        ```
        Each group of parameters corresponds to a musical note or rest.

      **Integration & Tips:**
      - Ideal for sharing music with users on Linux or for use in shell scripts.
      - The command is compatible with the standard Linux `beep` utility (make sure it's installed and you have permissions to use the system speaker).
      - Editing the generated command allows quick adjustments to tempo, pitch, or rhythm.

      This feature streamlines the process of bringing your music to Linux environments and enables creative uses such as musical notifications, alert systems, or simply enjoying your compositions outside NeoBleeper.
