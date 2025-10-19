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

  ![image3](https://github.com/user-attachments/assets/7d7d678e-5937-49e0-a6d5-95cc8941a3cb)
  
    **System speaker output is not present or non-standard system speaker output is present. Beep Stopper may cause instability or undesirable behaviors. Do you want to continue?**  
    This message appears when the Beep Stopper utility is started and detects that your system either does not have a standard (PC) system speaker, or the system speaker output is "non-standard." In this case, the utility warns you that attempting to use the Beep Stopper may not work as expected and could potentially cause unexpected behavior or instability.

If you proceed, the tool will try to stop the beep, but it may be ineffective or have side effects if your hardware is unsupported or non-standard.
If you choose not to continue, the tool will exit without making any changes.
  - *Tip:* If you receive this message, it means your computer does not have a compatible system speaker, or its output cannot be reliably controlled. Any beeping or buzzing you hear is likely coming from another audio device (such as your main speakers or headphones). Use your standard sound device settings to address sound issues, and close any applications that may be producing unwanted audio. If the problem persists, try restarting your computer or checking your device's sound settings.

- **Restart your computer:**  
  If the Beep Stopper does not resolve the issue, a system restart will reset the speaker hardware.

- **Prevention:**  
  Always close NeoBleeper normally. Avoid force-closing it via Task Manager or similar tools while sound is playing.
---

## 2. System Speaker Detection & Compatibility

NeoBleeper includes detection logic to check whether your system has a standard PC speaker output, as well as support for "hidden" system speaker outputs (such as those not using the PNP0800 ID). If your hardware does not support a standard or hidden system speaker, or the output is non-standard and not usable, you may see warning messages or have to rely on your regular sound device for beeps. However, starting with recent versions, NeoBleeper no longer forces you to use the sound device exclusively when a standard speaker is missing—it now allows use of hidden/non-PNP0800 system speaker outputs if present.

### Example Warning (Image 1):

![image1](https://github.com/user-attachments/assets/aed7987d-fbed-4f0c-b333-a88689bfe5bd)

> **Explanation:**  
> Your computer's motherboard either does not have a standard system speaker output, or the output is non-standard. NeoBleeper will attempt to detect and offer use of "hidden" system speaker outputs not identified as PNP0800. If such an output is available, you can now use the system speaker even if this warning appears. Otherwise, NeoBleeper will fall back to your regular sound device (like speakers or headphones).

### Settings Dialogs (Images 2 and 3):


![image2](https://github.com/user-attachments/assets/0e66b563-6000-4018-b89a-9c4dfd6c1b30)

![image3](https://github.com/user-attachments/assets/a23935f9-bf26-43f6-b8ae-5fd79a74167e)


- **"Test System Speaker" button availability:**  
  This option is enabled if NeoBleeper detects any usable system speaker output, including hidden or non-PNP0800 outputs. 
- **"Use sound device to create beep" setting:**  
  You are now allowed to disable this feature if a hidden or non-standard system speaker output is detected.

#### What does "non-standard system speaker output" mean?
Some modern computers, laptops, or virtual machines do not have a true PC speaker, or the signal routing is non-standard. NeoBleeper now attempts to detect and use such hidden system speaker outputs (not identified as PNP0800 devices), but can only enable the system speaker option if it is actually accessible at the hardware level. If no usable output is found, you will need to use your regular sound device.

---

## 3. How to Check for System Speaker Presence

- **Desktop computers:** Most older desktops have a PC speaker header on the motherboard. Newer systems may omit this feature, or may present the output in a hidden/non-PNP0800 form that NeoBleeper can now utilize.
- **Laptops:** Most laptops do not have a separate system speaker; all sound is routed through the main audio system.
- **Virtual machines:** System speaker emulation is often absent or unreliable; non-PNP0800 outputs may not be available.
- **How to tell:** If you see the warnings above but are able to enable and test the system speaker in NeoBleeper, your computer likely has a hidden or non-standard output.

---

## 2.1 System Speaker Output Test (Ultrasonic Frequency Detection)

NeoBleeper now includes a new, advanced hardware test to detect system speaker (aka PC speaker) output, even if the device is not reported by Windows (in certain IDs such as PNP0C02 instead of PNP0800). This test uses ultrasonic frequencies (typically 30–38 kHz, which are inaudible) and analyzes electrical feedback on the system speaker port.

- **How it works:**  
  During startup, NeoBleeper performs a second step after the usual device ID check. It sends ultrasonic signals to the system speaker port and monitors hardware feedback to detect the presence of a functional speaker output—even if hidden or non-standard.

- **What you may notice:**  
  On some systems, especially those with piezo buzzers, you might hear faint clicking noises during this stage. This is normal and indicates the hardware test is running.


  ![image4](https://github.com/user-attachments/assets/f55de735-c506-4abb-8041-0795f25a529a)
  
  *Checking for system speaker (PC speaker) output presence in step 2/2… (you may hear clicking sounds)*

- **Why this test?**  
  Many modern systems lack a PNP0800 system speaker device, but still have a usable (hidden) speaker output. NeoBleeper uses this advanced method to enable beep features on more hardware.

## 4. I Don't Hear Any Sound!

- **Check your NeoBleeper settings:**  
  If your system speaker is unavailable, ensure your sound device (speakers/headphones) is correctly selected and working.
- **Check Windows volume mixer:**  
  Make sure NeoBleeper is not muted in the system volume mixer.
- **Try the "Test System Speaker" button:**  
  Use it to test your PC speaker. 
- **Read the warning messages:**  
  NeoBleeper will provide specific instructions if it cannot access your system speaker.

---

## 5. Frequently Asked Questions

### Q: Can I use the system speaker if my hardware doesn't have a PNP0800 device?
**A:** Yes! NeoBleeper now attempts to detect and use hidden or non-PNP0800 system speaker outputs where possible. If successful, you can use the system speaker even if Windows doesn't report a standard device.

### Q: Why does the "Use sound device to create beep" setting sometimes become permanent (in older versions)?
**A:** When no standard system speaker output is detected (in older versions), NeoBleeper enforces this setting to ensure sound output is still possible.

### Q: Is there any workaround for missing system speaker?
**A:** You must use your regular sound device (speakers/headphones) if a standard system speaker output can be found (in older versions).

### Q: What if the Beep Stopper tool does not stop the stuck beep?
**A:** Restart your computer to reset the speaker hardware if the Beep Stopper utility fails.

### Q: Why do I hear clicking sounds during startup?
**A:** During the advanced system speaker output test (step 2), NeoBleeper sends ultrasonic signals to the hardware to detect hidden or non-standard speaker outputs. On some systems (especially those with piezo buzzers), this may cause faint clicking noises. This is normal and does not indicate a problem; it simply means the hardware test is running.

### Q: Can the ultrasonic hardware test (step 2/2) detect broken (open circuit) or disconnected system speakers?
**A:** This is currently untested and unknown. While the test checks for electrical feedback and port activity, it may not reliably distinguish between a physically present but broken (open circuit) or disconnected speaker and a missing speaker. If the speaker is completely broken or disconnected (open circuit), the test may return false, indicating no functional output detected. However, this behavior is not guaranteed and may depend on the specific hardware and failure mode. If you suspect your system speaker is not working, physical inspection or using a multimeter is recommended.

**Potential future updates:**  
If future testing or development enables NeoBleeper to reliably detect broken or disconnected system speakers via the ultrasonic hardware test, this FAQ and detection logic will be updated to reflect those improvements. Watch for changelogs or new releases for details.

---

## 6. Getting Help

- **Provide computer and environment details:** When reporting hardware detection or sound issues, please include details about your computer (desktop/laptop, manufacturer/model, operating system) and any relevant hardware.
- **Attach screenshots or error dialogs:** Screenshots of error or warning dialogs are very helpful. Specify exactly when the problem occurs.
- **Include the log file:** Starting with newer versions, NeoBleeper creates a detailed log file called `DebugLog.txt` in the program folder. Please attach this file when seeking help, as it contains helpful diagnostic information.
- **Describe the steps to reproduce the issue:** Clearly outline what you were doing when the problem happened.
- **Open an issue on GitHub:** For further assistance, open an issue on GitHub and include all the above details for the best support.

_This guide is updated as new issues and solutions are discovered. For further assistance, please open an issue on GitHub with detailed information about your setup and the problem encountered._
