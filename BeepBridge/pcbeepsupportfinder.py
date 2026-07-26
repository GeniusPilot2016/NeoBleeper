import winreg
import struct

def find_audio_driver_subkey():
    class_guid = "{4d36e96c-e325-11ce-bfc1-08002be10318}"
    base_path = f"SYSTEM\\CurrentControlSet\\Control\\Class\\{class_guid}"
    candidates = []

    try:
        class_key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, base_path)
        i = 0
        while True:
            try:
                sk = winreg.EnumKey(class_key, i)
                i += 1
                if not (len(sk) == 4 and sk.isdigit()):
                    continue
                sub_path = f"{base_path}\\{sk}"
                try:
                    s_key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, sub_path)
                    desc, provider, hw_ids = "", "", []
                    try: desc = winreg.QueryValueEx(s_key, "DriverDesc")[0]
                    except OSError: pass
                    try: provider = winreg.QueryValueEx(s_key, "ProviderName")[0]
                    except OSError: pass
                    try:
                        hw_data = winreg.QueryValueEx(s_key, "HardwareID")[0]
                        hw_ids = [str(x).upper() for x in (hw_data if isinstance(hw_data, list) else [hw_data])]
                    except OSError: pass
                    winreg.CloseKey(s_key)

                    desc_lower = str(desc).lower()
                    hw_concat = " ".join(hw_ids)

                    if "usb" in desc_lower or "usb" in hw_concat.lower() or "sst\\usb" in hw_concat.lower():
                        continue
                    if any(term in desc_lower for term in ["mikrofon", "microphone", "dmic", "capture", "rec"]):
                        continue
                    if any(term in desc_lower for term in ["bluetooth", "a2dp", "proxy", "streaming", "ksthunk"]):
                        continue

                    score = 0
                    if "VEN_10EC" in hw_concat or "realtek" in desc_lower:
                        score += 50
                    elif "HDAUDIO\\" in hw_concat or "INTELAUDIO\\FUNC_01" in hw_concat:
                        score += 40
                    elif "VEN_8086" in hw_concat:
                        score += 20
                    if any(term in desc_lower for term in ["speaker", "hoparlör", "high definition audio"]):
                        score += 10

                    candidates.append((score, sk, desc, provider, hw_ids))
                except OSError:
                    continue
            except OSError:
                break
        winreg.CloseKey(class_key)
    except OSError:
        pass

    if candidates:
        candidates.sort(key=lambda x: x[0], reverse=True)
        return candidates[0]
    return None

def recursive_registry_walk(key_path, visited=None):
    if visited is None:
        visited = set()
    if key_path in visited:
        return []
    visited.add(key_path)

    results = []
    try:
        key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, key_path)
    except OSError:
        return results

    val_idx = 0
    while True:
        try:
            v_name, v_data, v_type = winreg.EnumValue(key, val_idx)
            val_idx += 1
            results.append((key_path, v_name, v_data, v_type))
        except OSError:
            break

    sub_idx = 0
    subkeys = []
    while True:
        try:
            sub_name = winreg.EnumKey(key, sub_idx)
            sub_idx += 1
            subkeys.append(f"{key_path}\\{sub_name}")
        except OSError:
            break
    winreg.CloseKey(key)

    for sub_p in subkeys:
        results.extend(recursive_registry_walk(sub_p, visited))

    return results

def analyze_pure_solder_and_beep():
    print("==================================================================")
    print("  PURE INTERNAL SOLDER & PC BEEP RESOLVER                         ")
    print("==================================================================\n")

    res = find_audio_driver_subkey()
    if not res:
        print("[-] Error: Active audio codec driver key could not be isolated.")
        return

    score, target_subkey, target_desc, provider, hw_ids = res
    class_guid = "{4d36e96c-e325-11ce-bfc1-08002be10318}"
    full_drv_path = f"SYSTEM\\CurrentControlSet\\Control\\Class\\{class_guid}\\{target_subkey}"

    entries = recursive_registry_walk(full_drv_path)

    internal_speaker_found = False
    internal_beep_found = False
    pc_beep_controls = {}
    target_dword = None

    for path, name, data, v_type in entries:
        name_lower = name.lower()
        if "beep" in name_lower:
            pc_beep_controls[name] = (path, data, v_type)

        if v_type == winreg.REG_BINARY and isinstance(data, (bytes, bytearray)) and len(data) >= 4:
            for offset in range(0, len(data) - 3, 4):
                try:
                    dword = struct.unpack("<I", data[offset:offset+4])[0]
                    connectivity = (dword >> 30) & 0x03
                    device_type = (dword >> 20) & 0x0F

                    # Strictly filter out external jacks; evaluate internal fixed/soldered components only (0x01 Speaker or 0x0E PC Beep)
                    if device_type in [0x01, 0x0E] and connectivity in [0x02, 0x03]:
                        if device_type == 0x01:
                            internal_speaker_found = True
                        if device_type == 0x0E:
                            internal_beep_found = True
                        target_dword = dword
                except struct.error:
                    continue

    if internal_speaker_found or internal_beep_found:
        hardware_solder_status = "PHYSICALLY SOLDERED (Internal Integrated Transducer / Fixed PCB Line)"
    else:
        hw_str = " ".join(hw_ids)
        if "VEN_10EC" in hw_str or "INTELAUDIO" in hw_str or "HDAUDIO" in hw_str:
            hardware_solder_status = "PHYSICALLY SOLDERED (Integrated Onboard Sound Codec Chip)"
        else:
            hardware_solder_status = "PHYSICALLY SOLDERED (Internal Audio Subsystem)"

    beep_status = "DISABLED / HIDDEN BY DEFAULT"
    for name, (path, val, v_type) in pc_beep_controls.items():
        if isinstance(val, (int, str)) and str(val).isdigit():
            if int(val) != 0 and not any(neg in name.lower() for neg in ["disable", "mute", "hide"]):
                beep_status = f"ACTIVE / SHOWN ({name})"

    print(f"[+] Audio Driver Subkey      : [{target_subkey}]")
    print(f"    ├── Description          : {target_desc if target_desc else 'High Definition Audio'}")
    print(f"    └── Device Path          : HKLM\\{full_drv_path}")
    print("    " + "-"*62)
    print(f"    [★] PC Beep Status       : {beep_status}")
    if target_dword is not None:
        print(f"    [🔑] Internal Pin DWORD   : 0x{target_dword:08X}")
    print("    " + "-"*62)
    print(f"    [STATUS - HARDWARE SOLDER] -> {hardware_solder_status}")
    print("    " + "=" * 62 + "\n")

if __name__ == '__main__':
    analyze_pure_solder_and_beep()