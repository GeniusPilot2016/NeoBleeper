# NeoBleeper Troubleshooting Guide

This guide provides solutions for common issues encountered when using NeoBleeper, especially those related to system speaker behavior, sound output, hardware compatibility, and persistent system beeps.

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

- **Restart your computer:**  
  If the Beep Stopper does not resolve the issue, a system restart will reset the speaker hardware.

- **Prevention:**  
  Always close NeoBleeper normally. Avoid force-closing it via Task Manager or similar tools while sound is playing.

---

## 2. System Speaker Detection & Compatibility

NeoBleeper includes detection logic to check whether your system has a standard PC speaker output. If your hardware does not support it, or the output is non-standard, you may see warning messages or limited options.

### Example Warning (Image 1):

![image1](https://github.com/user-attachments/assets/a419367f-39ab-44c5-8d66-036f031b3dd3)

> **Explanation:**  
> Your computer's motherboard either does not have a system speaker output, or the output is non-standard. In this case, NeoBleeper will switch to using your regular sound device (like speakers or headphones) to generate beeps.

### Settings Dialogs (Images 2 and 3):

![image2](https://github.com/user-attachments/assets/79163339-94a2-4bc7-8096-7dbacaee8505)

![image3](https://github.com/user-attachments/assets/6935af64-f727-44b1-b06e-ed2e5bd41355)

- **"Test System Speaker" button is disabled:**  
  This option is unavailable if your hardware does not support a standard system speaker.  
- **"Use sound device to create beep" is permanently enabled:**  
  If the system speaker is missing or non-standard, you cannot disable this feature.

#### What does "non-standard system speaker output" mean?
Some modern computers, laptops, or virtual machines do not have a true PC speaker, or the signal routing is non-standard. NeoBleeper can only use the system speaker (aka  PC speaker) if it is directly accessible.

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

- Please provide details of your computer (desktop/laptop, manufacturer/model, operating system) if reporting hardware detection issues.
- Attach screenshots of any error or warning dialogs, and specify when the problem occurs.

---

_This guide is updated as new issues and solutions are discovered. For further assistance, please open an issue on GitHub with detailed information about your setup and the problem encountered._
