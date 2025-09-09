# NeoBleeper Troubleshooting Guide

This guide provides solutions for common issues encountered when using NeoBleeper, especially those related to system speaker behavior, sound output, hardware compatibility, and persistent system beeping.

---

## 1. Sound Stuck in System Speaker After Crash or Force Close

**Problem:**  
If NeoBleeper crashes or is forcefully closed while audio is playing through the system (PC) speaker, the sound may become "stuck," resulting in continuous beeping or buzzing.

**Why This Happens:**  
The system speaker is controlled at a low hardware/software level. If the application does not properly release or reset the speaker on exit, the tone may persist.

**Solutions:**
- **Use the NeoBleeper Beep Stopper utility:**  
  NeoBleeper comes with a tool called "NeoBleeper Beep Stopper" in the program folder.
 
  ![image4](https://github.com/user-attachments/assets/8503d816-3ec1-477f-8233-28971640a8b4)
  
  - Launch this tool and press the **Stop Beep** button to stop the stuck beep from the system speaker.
  - Only use this utility when the beep continues after a crash or force quit.

  #### Beep Stopper Messages and Their Meanings

  When you use the Beep Stopper utility, you may see the following messages:

  ![image1](https://github.com/user-attachments/assets/a703c888-c4eb-4387-b713-a18b3c6e213c)
    
    **System speaker is not beeping or the system speaker is beeping in a different way. No action taken.**  
    This message appears when the utility checks the system speaker and determines that it is either not producing a beep, or is beeping in a manner that cannot be controlled by the tool. In this case, the Beep Stopper will not take any further action.  
    - *Tip:* If you still hear a persistent beep, try restarting your computer.

  ![image2](https://github.com/user-attachments/assets/7ecf60b1-6c6e-464a-9f36-df0e821c2ae9)
    
    **Beep is successfully stopped!**  
    This message confirms that the Beep Stopper utility detected a stuck beep and was able to stop it successfully. No further action is required.

  ![image3](https://github.com/user-attachments/assets/4f02f8e0-ac3b-4c9b-bed7-6835872a84e4)
  
    **System speaker is not present. Therefore, stopping the beep is not possible.**  
    This message is shown when the Beep Stopper utility is started and detects that your system does not have a system (PC) speaker. In this case, the tool cannot attempt to stop a stuck beep, as there is no speaker hardware to control.

    - *Tip:* If you receive this message, it means your computer does not have a compatible system speaker, and any beeping or buzzing you hear must be coming from another audio device. Use your standard sound device settings to address sound issues.

- **Restart your computer:**  
  If the Beep Stopper does not resolve the issue, a system restart will reset the speaker hardware.

- **Prevention:**  
  Always close NeoBleeper normally. Avoid force-closing it via Task Manager or similar tools while sound is playing.

---

## 2. System Speaker Detection & Compatibility

NeoBleeper includes detection logic to check whether your system has a standard PC speaker output. If your hardware does not support it, or the output is non-standard, you may see warning messages or limited options in the settings dialog.

### Example Warning (Image 1):

![image1](https://github.com/user-attachments/assets/a419367f-39ab-44c5-8d66-036f031b3dd3)

> **Explanation:**  
> Your computer's motherboard either does not have a system speaker output, or the output is non-standard. In this case, NeoBleeper will switch to using your regular sound device (like speakers or headphones).

### Settings Dialogs (Images 2 and 3):

![image2](https://github.com/user-attachments/assets/79163339-94a2-4bc7-8096-7dbacaee8505)

![image3](https://github.com/user-attachments/assets/6935af64-f727-44b1-b06e-ed2e5bd41355)

- **"Test System Speaker" button is disabled:**  
  This option is unavailable if your hardware does not support a standard system speaker.  
- **"Use sound device to create beep" is permanently enabled:**  
  If the system speaker is missing or non-standard, you cannot disable this feature.

#### What does "non-standard system speaker output" mean?
Some modern computers, laptops, or virtual machines do not have a true PC speaker, or the signal routing is non-standard. NeoBleeper can only use the system speaker (aka  PC speaker) if it is directly accessible by the hardware.

---

## 3. How to Check for System Speaker Presence

- **Desktop computers:** Most older desktops have a PC speaker header on the motherboard. Newer systems may omit this feature.
- **Laptops:** Most laptops do not have a separate system speaker; all sound is routed through the main audio system.
- **Virtual machines:** System speaker emulation is often absent or unreliable.
- **How to tell:** If you see the warnings above, your computer likely does not have a standard system speaker output.

---

## 4. I Don't Hear Any Sound!

- **Check your NeoBleeper settings:**  
  If your system speaker is unavailable, ensure your sound device (speakers/headphones) is correctly selected and working.
- **Check Windows volume mixer:**  
  Make sure NeoBleeper is not muted in the system volume mixer.
- **Try the "Test System Speaker" button:**  
  If enabled, use it to test your PC speaker. If disabled, you must use your regular sound device.
- **Read the warning messages:**  
  NeoBleeper will provide specific instructions if it cannot access your system speaker.

---

## 5. Frequently Asked Questions

### Q: Can I force NeoBleeper to use the system speaker on unsupported hardware?
**A:** No. If the hardware is not present or the output is non-standard, this is not possible.

### Q: Why does the "Use sound device to create beep" setting sometimes become permanent?
**A:** When the system speaker cannot be used, NeoBleeper enforces this setting to ensure sound output is still possible.

### Q: Is there any workaround for missing system speaker?
**A:** You must use your regular sound device (speakers/headphones). There is no software workaround for missing PC speaker hardware.

### Q: What if the Beep Stopper tool does not stop the stuck beep?
**A:** Restart your computer to reset the speaker hardware if the Beep Stopper utility fails.

---

## 6. Getting Help

- **Provide computer and environment details:** When reporting hardware detection or sound issues, please include details about your computer (desktop/laptop, manufacturer/model, operating system) and any relevant hardware.
- **Attach screenshots or error dialogs:** Screenshots of error or warning dialogs are very helpful. Specify exactly when the problem occurs.
- **Include the log file:** Starting with newer versions, NeoBleeper creates a detailed log file called `DebugLog.txt` in the program folder. Please attach this file when seeking help, as it contains helpful diagnostic information.
- **Describe the steps to reproduce the issue:** Clearly outline what you were doing when the problem happened.
- **Open an issue on GitHub:** For further assistance, open an issue on GitHub and include all the above details for the best support.

_This guide is updated as new issues and solutions are discovered. For further assistance, please open an issue on GitHub with detailed information about your setup and the problem encountered._
