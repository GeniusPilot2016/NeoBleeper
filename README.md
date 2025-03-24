# NeoBleeper

NeoBleeper is a tune creation software using the system speaker on the motherboard, inspired by the Bleeper Music Maker software that Robbi-985 (aka SomethingUnreal) developed around late 2007-early 2008 but abandoned in 2011 due to the fact that beeps were directed to the default sound device instead of the system speaker on the motherboard in Windows 7 and above. NeoBleeper provides a more stable music making experience by solving the problem of system beeps being directed to the default sound device instead of the system speaker on the motherboard in Windows 7 and above by accessing the physical port of the system speaker 🖥️🎶

![image](https://github.com/user-attachments/assets/2f6a60b9-e867-41ea-94d6-f292b72fad77)

## Status

This project is **still in development** and some features and components are still unfinished and even some of its parts have not yet been created. Please use the program with this in mind and provide feedback for possible errors or omissions.

## Features

- ### User-friendly, familiar, more ergonomic and more natural interface
  NeoBleeper offers a user-friendly, familiar, ergonomic and more natural interface that compatible with high resolution displays.
  
  ![image](https://github.com/user-attachments/assets/54c9ceae-2fa5-4351-892a-4b48a7f9f9df)

  ![image](https://github.com/user-attachments/assets/fbaad33a-8b1b-4d94-8b95-299c2bb558d0)
  
  ![image](https://github.com/user-attachments/assets/27fada73-e245-4a08-ae18-0660c27726b3)

  ![image](https://github.com/user-attachments/assets/24ff32f4-8b9e-40ee-a014-a165a2f7a250)

  ![image](https://github.com/user-attachments/assets/17661b33-340b-4882-a702-25e3b9b98443)

- ### Add and edit notes
  NeoBleeper allows users to add and notes easily and quickly. With its user-friendly, familiar and more natural interface, you can creatively add notes to your music or edit notes in your music.

  ![image](https://github.com/user-attachments/assets/cab66c2a-21d9-4c26-a6eb-dade46d0e8ab)

  ![image](https://github.com/user-attachments/assets/41739606-1383-4457-8756-4da529b406f5)

- ### Time signature and BPM settings
  NeoBleeper allows users to adjust the time signature of music and edit rhythm using a metronome. With its user-friendly, familiar and more natural interface, time signature adjustments and metronome usage help you create more accurate and consistent musics.

  ![image](https://github.com/user-attachments/assets/5e526516-c731-492b-aa95-1520c3fb1250)

- ### More precise position indicator
- NeoBleeper offers a position indicator that compatible with 1/32 note length.

  ![image](https://github.com/user-attachments/assets/f729a905-f198-4558-8c18-a290fda96f26)

- ### Easy note entry with virtual keyboard
  NeoBleeper allows you to enter notes quickly and easily with a virtual keyboard. With its user-friendly, familiar and more natural interface, you can effortlessly enter and edit notes, speeding up your creative processes.
  
  ![image](https://github.com/user-attachments/assets/d42b9f00-5d95-4057-a59a-a5ad5c4323b9)

- ### Also you can use your regular keyboard as piano
  NeoBleeper turns your regular keyboard into a piano, allowing you to easily play notes without needing a MIDI input device.

  ![image](https://github.com/user-attachments/assets/e4cc765a-fbcf-4813-b12e-046802f0d591)

- ### Backwards Compatible
  NeoBleeper supports its own file format (.NBPML) as well as the file format of the Bleeper Music Maker program (.BMM), from which it was inspired.

  ![image](https://github.com/user-attachments/assets/1c1a01dd-83e0-4f5d-b13a-4c20e85404fd)

- ### You can personalize as you wish
  You can personalize the NeoBleeper program by changing the octave colors, some buttons' colors and indicators' colors to any of millions of colors and by choosing between dark and light themes. The only limit to this feature is your imagination.

  ![image](https://github.com/user-attachments/assets/1be1b9ad-44a8-4ae9-8840-9761b5cb011c)

- ### Detects system speaker for you
  If NeoBleeper doesn't detect the system speaker on your computer's motherboard, it will automatically enable the 'Use sound device to create the beep' option permanently.
  
  ![image](https://github.com/user-attachments/assets/cda71c5b-df10-4ab4-bac6-3b6d176303a0)

- ### Smarter than you think
  NeoBleeper recognizes your computer type and prevents possible undesired situations by automatically activating the 'Use sound device to create the beep' option on compact computers.

  ![image](https://github.com/user-attachments/assets/69ff23e3-7ee8-4c9f-92d1-ea57c464eb40)


## System Requirements

**Windows Version:** Windows 10 (1809 and above) 64-bit (minimum)/Windows 11 (recommended)

**CPU:** Intel® Core i3 or equivalent (minimum)

**RAM:** 2 GB (minimum), 4 GB (recommended)

**Display Resolution:** 1024x768 (minimum), 1920x1080 (recommended)

**Storage:** 201 MB (minimum)

**Required Hardware:** System speaker (recommended)

## Cloning

1. Open Visual Studio.

2. On the start window, select `Clone a repository`.

3. Enter or type the this repository location, then select the `Clone` button:
```sh
git clone https://github.com/GeniusPilot2016/NeoBleeper.git
```

4. To run NeoBleeper, select `NeoBleeper` next to the Solution Platforms ComboBox.

5. If you have contributed to NeoBleeper, don't forget to select the `listView1` component in the `about_neobleeper.cs` file, select the small arrow in the upper right corner, select `Edit Items`, add a ListViewItem, write your name/nickname in the `Text` property, select the triple dots to the right of `(Collection)` in the `SubItems` property, and write the tasks you have done while contributing to NeoBleeper in the `Text` property or edit your own existing `ListViewItem`, before committing the project.

## Contributing
Contributions are always welcome! You can contribute by submitting a pull request or opening an issue.

### Using Visual Studio

1. **Fork the Repository**: 
   - Fork the NeoBleeper repository on GitHub to your own account.

2. **Clone the Repository**:
   - Open Visual Studio.
   - Clone the GitHub repository to your local machine from the `File > Clone Repository` menu.

3. **Create a New Branch**:
   - Create a new branch with `Git > New Branch`.
   - Specify the branch name and click "Create".
   - Alternatively, open the terminal and use the command: `git checkout -b feature/AmazingFeature`

4. **Make Your Changes**:
   - Edit the project and make necessary code changes.
   - Save and test your changes regularly.

5. **Commit Your Changes**:
   - Click `Git > Commit` to save the changes made.
   - Add the commit message and click the "Commit" button.
   - Alternatively, use the following command in the terminal: `git commit -m 'Add feature: AmazingFeature'`

6. **Push Your Changes**:
   - Use `Git > Push` to push your local changes to the remote repository.
   - Select which remote branch you want to push your changes to and click the "Push" button.
   - Alternatively, use the following command in terminal: `git push origin feature/AmazingFeature`

7. **Open a Pull Request**:
   - Create a pull request on the main repository on GitHub.
   - Add a title and description explaining your changes.

### General Instructions
Start by forking.

Create a new Branch: `git checkout -b feature/AmazingFeature`

Commit your changes: `git commit -m 'Add feature: AmazingFeature'`

Do a Push: `git push origin feature/AmazingFeature`

Open a Pull Request.

## Contact

For questions or feedback about the project, please contact: [nisaozdogan280@gmail.com](mailto:nisaozdogan280@gmail.com)

[YouTube](https://www.youtube.com/@geniuspilot2016)

[TikTok](https://www.tiktok.com/@geniuspilot2016)

[My Website](https://geniuspilot2016.wordpress.com)
