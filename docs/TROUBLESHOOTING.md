# NeoBleeper Troubleshooting Guide

This guide provides solutions for common issues encountered when using NeoBleeper, especially those related to system speaker behavior, sound output, hardware compatibility, and persistent system beeping.

---

## 1. Sound Stuck in System Speaker After Crash or Force Close

**Problem:**  
If NeoBleeper crashes or is forcefully closed while audio is playing through the system (PC) speaker, the sound may become "stuck," resulting in continuous beeping or buzzing.

**Why This Happens:**  
The system speaker is controlled at a low hardware/software level. If the application does not properly release or reset the speaker on exit, the tone may persist.

**Solutions:**
- **Use the NeoBleeper Beep Stopper utility (for 64-bit version):**  
  NeoBleeper comes with a tool called "NeoBleeper Beep Stopper" in the program folder.
 
  ![image4](https://github.com/user-attachments/assets/8503d816-3ec1-477f-8233-28971640a8b4)
  
  - Launch this tool and press the **Stop Beep** button to stop the stuck beep from the system speaker.
  - Only use this utility when the beep continues after a crash or force quit.

  #### Beep Stopper Messages and Their Meanings

  When you use the Beep Stopper utility, you may see the following messages:
  
  ![image1](https://github.com/user-attachments/assets/1bdcee2a-571e-4bbf-b97f-aa5835f22538)

    **System speaker is not beeping or the system speaker is beeping in a different way. No action taken.**  
    This message appears when the utility checks the system speaker and determines that it is either not producing a beep, or is beeping in a manner that cannot be controlled by the tool. In this case, the Beep Stopper will not take any further action.  
    - *Tip:* If you still hear a persistent beep, try restarting your computer.

  ![image2](https://github.com/user-attachments/assets/64cd126a-4f88-40fe-aad1-eb4fe1e569a3)
    
    **Beep is successfully stopped!**  
    This message confirms that the Beep Stopper utility detected a stuck beep and was able to stop it successfully. No further action is required.

  ![image3](https://github.com/user-attachments/assets/cd8a671d-d289-4249-bedf-a273a82f73d2)
  
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

![image2](https://github.com/user-attachments/assets/4246a2ec-6199-4bcc-acf2-65045dc53867)

![image3](https://github.com/user-attachments/assets/add9e411-5261-46e2-9762-821b0e31ff84)

- **"Test System Speaker" button availability:**  
  This option is enabled if NeoBleeper detects any usable system speaker output, including hidden or non-PNP0800 outputs. 
- **"Use sound device to create beep" setting:**  
  You are now allowed to disable this feature if a hidden or non-standard system speaker output is detected.

#### What does "non-standard system speaker output" mean?
Some modern computers, laptops, or virtual machines do not have a true PC speaker, or the signal routing is non-standard. NeoBleeper now attempts to detect and use such hidden system speaker outputs (not identified as PNP0800 devices), but can only enable the system speaker option if it is actually accessible at the hardware level. If no usable output is found, you will need to use your regular sound device.

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

---

## 3. ARM64 Support and Limitations

**ARM64-based devices:**  
On Windows ARM64 systems, the "System Speaker" test and the "Use sound device to create beeps" checkbox are **not available** in NeoBleeper. Instead, all beeps and sound outputs are always produced through your standard audio device (speakers or headphones).

- The "Test System Speaker" button and related detection features will **not** be visible in the settings on ARM64 devices.
- The "Use sound device to create beep" option is not present because this behavior is enforced automatically.
- This limitation exists because direct access to the PC/system speaker hardware is not available on ARM64 Windows platforms.
- You will always hear beeps through your normal audio output device on ARM64.

**If you are using an ARM64 machine and do not see the system speaker options in NeoBleeper, this is expected and not a bug.**

---

## 4. How to Check for System Speaker Presence

- **Desktop computers:** Most older desktops have a PC speaker header on the motherboard. Newer systems may omit this feature, or may present the output in a hidden/non-PNP0800 form that NeoBleeper can now utilize.
- **Laptops:** Most laptops do not have a separate system speaker; all sound is routed through the main audio system.
- **Virtual machines:** System speaker emulation is often absent or unreliable; non-PNP0800 outputs may not be available.
- **How to tell:** If you see the warnings above but are able to enable and test the system speaker in NeoBleeper, your computer likely has a hidden or non-standard output.

---

## 5. I Don't Hear Any Sound!

- **Check your NeoBleeper settings:**  
  If your system speaker is unavailable, ensure your sound device (speakers/headphones) is correctly selected and working.
- **Check Windows volume mixer:**  
  Make sure NeoBleeper is not muted in the system volume mixer.
- **Try the "Test System Speaker" button:**  
  Use it to test your PC speaker. 
- **Read the warning messages:**  
  NeoBleeper will provide specific instructions if it cannot access your system speaker.

---

## 6. AI Warnings, Errors, and Google Gemini™ API Troubleshooting

NeoBleeper’s "Create Music With AI" feature utilizes the Google Gemini™ API. You may encounter specific error dialogs or warnings related to API availability, usage limits, or country restrictions.

### 6.1 Quota or Rate Limit Errors (429 RESOURCE_EXHAUSTED)

![image1](https://github.com/user-attachments/assets/c6a28c4a-f96c-4aa1-8c88-d3ce5628c052)

**Example Message:**  
```
An error occurred: RESOURCE_EXHAUSTED (Code: 429): You exceeded your current quota, please check your plan and billing details...
```

**Potential Reasons:**
- **API quota for your account has been exhausted.** If you’re using a free API key, certain models (such as `gemini-2.0-pro-exp`) may not be available or may have very low/hard limits on usage for free accounts.
- **Free tier limitations:** Some newer generative models (like Gemini Pro Exp) are *not* available to free-tier users. Attempting to use them results in a quota or availability error.
- **Exceeded rate limits:** If you send too many requests too quickly, you may hit the API’s rate limits even on a paid plan.

**How to Fix:**
- **Check your API quota and billing:** Log in to your Google Cloud/Gemini account to verify your usage and upgrade your plan if needed.
- **Use only supported models:** Free-tier users may be limited to certain models. Check documentation for available models or switch to a supported one.
- **Wait and retry later:** Sometimes, waiting a few moments will allow quota to refresh temporarily, as indicated by the message’s countdown.
- **Review [Gemini API documentation](https://ai.google.dev/gemini-api/docs/rate-limits) for up-to-date usage policies and rate limits.**

### 6.2 Troubleshooting for Very New or Undocumented Gemini Models (e.g., Gemini 3 Pro Preview)

Some Gemini models—especially brand new releases such as **Gemini 3 Pro Preview**—may not appear in the official Gemini API pricing or quota documentation at launch. You may encounter quota, access, or "RESOURCE_EXHAUSTED" errors even if your overall account quota appears unused.

**Important considerations for very new models:**
- Google often limits access to preview models (like Gemini 3 Pro Preview) to select accounts or specific regions and may enforce much stricter request and usage limits.
- Free-tier accounts may have zero quota for these models, or requests may be blocked entirely.
- The model may not be visible in quota/pricing tabs or Google documentation for several weeks after release.
- Pricing, access, and availability for new Gemini models may change frequently.

**What to do if you encounter errors:**
- Double-check your [API usage and quotas](https://ai.dev/usage?tab=rate-limit) and whether the new model appears in your console.
- Review the [Gemini API documentation](https://ai.google.dev/gemini-api/docs/rate-limits), but be aware that documentation may lag behind newly released models.
- If you see errors like "RESOURCE_EXHAUSTED" for a model that is not documented in official pricing tables, it likely means the model is not yet generally available or has very restricted preview access.
- Wait for Google to update their documentation and for broader rollout if you need to use these experimental models.

> **Note:**  
> NeoBleeper and similar applications cannot bypass these limitations. If your account or region is not eligible, you must wait until Google officially enables access or increases quota for your chosen Gemini model.

---

### 6.3 Region or Country Restrictions

#### "API is not available in your country"

![image4](https://github.com/user-attachments/assets/e3ce3baf-a3a6-41b6-86ac-8f66d721adee)

Some regions are not supported for the Google Gemini™ API due to regional or legal restrictions.

**Potential Reasons:**
- Your country is a country where Gemini API availability is restricted.
- The API key you’re using is registered to a region that does not have access.

**How to Fix:**
- **Check Google Gemini™ API supported countries** in the official documentation.
- If you are in a restricted country, the AI features will not be usable.

#### Region-Specific Warning (Settings Panel)

![image3](https://github.com/user-attachments/assets/03f8a849-489c-46a9-af2c-57bb003db7c6)

In the European Economic Area, Switzerland, or United Kingdom, the Gemini™ API may require a paid (non-free) Google account.

- If you see this warning, ensure you’ve upgraded your Gemini API plan before attempting to use AI features.

---

### 6.4 General AI API Advice

- Only enter your own API key; do not share it for your security.
- NeoBleeper does not transmit your API key except directly to the Gemini service as needed for feature use.
- If you experience repeated errors, try removing and re-adding your API key, and double check that your key is active.

---

## 7. System Speaker and Sound Advice for Specific Chipsets (incl. Intel B660)

### If you hear no sound, sound is corrupted, or system speaker is unreliable:

Some modern chipsets — including those in the Intel B660 series and newer — may have issues with initializing or re-initializing the system speaker (PC beeper), resulting in silence or sound problems.

**Advice for affected users:**

- **Try putting your computer to sleep and waking it up again.**  
  This may help reinitialize or reset the low-level hardware port responsible for the system speaker and restore beep functionality.
- **Use the "Use sound device to create beep" feature** as a fallback if system speaker output is unreliable.
- **Check for BIOS or firmware updates:** Some motherboard vendors may release updates improving speaker port compatibility.
- **Desktop-specific:** If you have added, removed, or reconnected system speaker hardware, perform a full power cycle.

_This workaround is highlighted in the settings:_ 

![image2](https://github.com/user-attachments/assets/56f85637-ea14-4002-9e86-6d97730e211e)

> *If you hear no sound or the sound is corrupted, try putting your computer to sleep and waking it up. This can help reinitialize the system speaker on affected chipsets.*

---

*For any sound or AI-related issues not covered here, please include error screenshots, details of your PC hardware (especially motherboard/chipset make and model), and your country/region when requesting support or opening a GitHub issue.*

---

## 8. Frequently Asked Questions

### Q: Can I use the system speaker if my hardware doesn't have a PNP0800 device?
**A:** Yes! NeoBleeper now attempts to detect and use hidden or non-PNP0800 system speaker outputs where possible. If successful, you can use the system speaker even if Windows doesn't report a standard device.

### Q: Why does the "Use sound device to create beep" setting sometimes become permanent (in older versions)?
**A:** When no standard system speaker output is detected (in older versions), NeoBleeper enforces this setting to ensure sound output is still possible.

### Q: Is there any workaround for missing system speaker?
**A:** You must use your regular sound device (speakers/headphones) if a standard system speaker output can't be found (in older versions).

### Q: What if the Beep Stopper tool does not stop the stuck beep?
**A:** Restart your computer to reset the speaker hardware if the Beep Stopper utility fails.

### Q: Why do I hear clicking sounds during startup?
**A:** During the advanced system speaker output test (step 2), NeoBleeper sends ultrasonic signals to the hardware to detect hidden or non-standard speaker outputs. On some systems (especially those with piezo buzzers), this may cause faint clicking noises. This is normal and does not indicate a problem; it simply means the hardware test is running.

### Q: Can the ultrasonic hardware test (step 2) detect broken (open circuit) or disconnected system speakers?
**A:** This is currently untested and unknown. While the test checks for electrical feedback and port activity, it may not reliably distinguish between a physically present but broken (open circuit) or disconnected speaker and a missing speaker. If the speaker is completely broken or disconnected (open circuit), the test may return false, indicating no functional output detected. However, this behavior is not guaranteed and may depend on the specific hardware and failure mode. If you suspect your system speaker is not working, physical inspection or using a multimeter is recommended.

### Q: Why don't I see any system speaker or beep sound options on my ARM64 device?
**A:** On Windows ARM64 systems, NeoBleeper disables system speaker-related settings because ARM64 platforms do not support direct system speaker hardware access. All beeps are played through your regular sound output device (speakers or headphones), and the "Test System Speaker" and "Use sound device to create beep" options are automatically hidden. This behavior is by design and not an error.

### Q: What does it mean when I receive a "non-standard system speaker output is present" warning?
**A:** NeoBleeper has detected speaker hardware that does not conform to traditional PC speaker standards (e.g., not a PNP0800 device). This could be a "hidden" speaker output found on modern desktops or virtual machines. In these cases, not all beep features may work reliably, but NeoBleeper will attempt to use any compatible output it can detect.

### Q: Why is the "Test System Speaker" button present even if Windows doesn't list a PC speaker device?
**A:** NeoBleeper includes detection logic for hidden or non-standard system speaker outputs. If the button appears, it means NeoBleeper has found a potential hardware port for speaker output, even if it is not reported by Windows as a device.

### Q: I am using the Google Gemini™ API for AI features, and I see a "quota exhausted" or "API not available in your country" message. What should I do?
**A:** Refer to section 6 of this guide. Make sure your API key and billing/quota are in good standing, and that your usage complies with Google's regional restrictions. If you are in a restricted region, unfortunately, AI features may not be available.

### Q: I have an Intel B660 (or newer) system and my PC speaker sometimes does not work or gets stuck. Is this normal?
**A:** Some newer chipsets have known compatibility issues with reinitializing the system speaker. Try putting your computer to sleep and waking it up, or use your regular sound device. Check for BIOS/firmware updates that may improve speaker support.

### Q: What is the best way to report sound or AI issues for support?
**A:** Always provide as much information as possible: your computer make/model, region, screenshots of error dialogs, and your `DebugLog.txt` from the NeoBleeper folder. For AI issues, include the full text of error dialogs and describe your Gemini API account status.

### Q: After a crash or force-close, NeoBleeper's Beep Stopper did not stop a continuous beep. Is there another way to fix this?
**A:** If the Beep Stopper is ineffective, restarting your computer will reset the system speaker hardware and stop any persistent beeping.

### Q: Is it safe to use the Beep Stopper utility if I see a message warning about non-standard or missing system speaker output?
**A:** Yes, but be aware that the utility may not be able to control the hardware, and in rare cases may cause instability or no effect. If you are unsure, choose not to proceed and restart your computer instead.

### Q: On virtual machines, I cannot get the system speaker to work at all. Is this a bug?
**A:** Not necessarily. Many virtual machines do not properly emulate a PC speaker, or they present the output in a way that cannot be controlled programmatically. Use your standard sound output device for best results.

**Potential future updates:**  
If future testing or development enables NeoBleeper to reliably detect broken or disconnected system speakers via the ultrasonic hardware test, this FAQ and detection logic will be updated to reflect those improvements. Watch for changelogs or new releases for details.

---

## 9. Getting Help

- **Provide computer and environment details:** When reporting hardware detection or sound issues, please include details about your computer (desktop/laptop, manufacturer/model, operating system) and any relevant hardware.
- **Attach screenshots or error dialogs:** Screenshots of error or warning dialogs are very helpful. Specify exactly when the problem occurs.
- **Include the log file:** Starting with newer versions, NeoBleeper creates a detailed log file called `DebugLog.txt` in the program folder. Please attach this file when seeking help, as it contains helpful diagnostic information.
- **Describe the steps to reproduce the issue:** Clearly outline what you were doing when the problem happened.
- **Open an issue on GitHub:** For further assistance, open an issue on GitHub and include all the above details for the best support.

_This guide is updated as new issues and solutions are discovered. For further assistance, please open an issue on GitHub with detailed information about your setup and the problem encountered._
