
If NeoBleeper cannot detect your PC speaker even though it is plugged into your motherboard, you can connect the speaker to a separate sound card instead.

**What you will need:**
1. The cheapest USB sound card you can find (using a cheap external USB card protects your motherboard’s built-in audio from accidental short circuits).
2. A 3.5mm AUX cable (male to male).
3. A PC speaker (the small motherboard buzzer).

**Instructions:**

1. **Unplug the PC speaker from your motherboard** if it is still connected.
2. **Cut and strip the AUX cable:** Cut off one end of the AUX cable and strip back the outer rubber jacket to expose the inner wires. You will usually see 3 wires inside: **black** (Ground), **red** (Left channel), and **green** (Right channel). *(Note: Wire colors can vary depending on the cable for example, white instead of green, or bare copper shielding instead of black, but the black or uninsulated bare wire is almost always ground).*
3. **Strip the inner wires:** Remove a small amount of insulation from the tips of the **ground** wire and **one** of the audio channel wires (either left or right, it doesn’t matter which). 
   *Tip:* Thin AUX wires often use very thin plastic insulation or a painted on enamel coating. The easiest way to strip them is to briefly burn the tips with a lighter, then wipe off any black soot so you have clean bare metal for good contact.
4. **Connect the wires to the PC speaker:** Look at the plastic connector at the end of your PC speaker’s wires. Insert the bare **ground** wire from the AUX cable into the pin hole that leads to the speaker’s **black** wire. Insert the bare **channel** wire from the AUX cable into the pin hole that leads to the speaker’s **red** wire.
   *Tips:* Fold each bare wire tip in half once before inserting it this makes it thicker so it fits snugly and won’t fall out easily. You can also wrap the connector with electrical tape afterward to keep the wires in place.
5. **Prevent short circuits:** Make sure the exposed metal parts of the ground wire, the connected channel wire, and the unused third wire do **not** touch each other. A short circuit can fry your USB sound card. (It’s best to trim or tape over the unused third wire).
6. **Connect to your PC:** Plug the intact 3.5mm end of the AUX cable into the **headphone / audio output jack** on your USB sound card (usually colored green or marked with a headphone icon, do not plug it into the microphone jack). Then plug the USB sound card into your PC.
7. **Configure Windows:** Open Windows Sound Settings and select your USB sound card as your **audio output device** (playback device).

### Audio Tuning (Fixing harsh resonances)

Almost every PC speaker has massive volume "peaks" (resonant frequencies) at certain pitches, which can make the audio sound piercing, distorted, or unpleasant. You can fix this with a quick EQ adjustment:

1. **Find your speaker’s resonant frequencies:** 
   * Open [https://www.szynalski.com/tone-generator/](https://www.szynalski.com/tone-generator/) *(start with a low volume first)*.
   * Slowly sweep through the higher frequencies from **1,000 Hz** to **10,000 Hz** while listening to the PC speaker.
   * You will immediately notice where the peaks are at those specific frequencies, the volume will suddenly spike and sound disproportionately loud and harsh (it’s hard to describe in words, you’ll instantly recognize it when you hear it). Write down those frequencies.
2. **Install equalizer APO:** 
   * Download and install **equalizer APO** *(I won’t include installation instructions here as there are plenty of guides online, just make sure to enable it **only** for your USB sound card in the Equalizer APO Configurator so it doesn’t alter the sound of your main headphones or speakers)*.
3. **Lower the peaking frequencies:** 
   * Open the **equalizer APO configuration editor**.
   * Click the green **`+`** icon in the bottom-left area and select **`Graphic equalizers` → `31-band graphic equalizer`**.
   * Find the sliders matching the resonant frequencies you found earlier and pull them down until the volume sounds balanced.
   * *Example:* On my PC speaker, I found strong resonances at **2500 Hz** (lowered to **-16 dB**) and **4000 Hz** (lowered to **-19 dB**). Your speaker’s peaks may be at different frequencies.
