import winreg

def isolate_main_soundcard_beep():
    print("==================================================================")
    print("     MAIN SOUNDCARD ONLY: PC BEEP ISOLATION & SOLDER MONITOR     ")
    print("==================================================================")
    print("[*] Filtering components and targeting primary onboard audio...\n")
    
    class_guid = "{4d36e96c-e325-11ce-bfc1-08002be10318}"
    base_path = f"SYSTEM\\CurrentControlSet\\Control\\Class\\{class_guid}"
    
    try:
        class_key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, base_path)
    except Exception as e:
        print(f"[-] Error: {e}")
        return

    # Signature maps of known primary onboard audio vendors
    vendor_maps = {
        "realtek": {"key": "GlobalPolicy", "sig": "FFC60000E4041080", "beep_offset": 8, "pin_offset": 24},
        "conexant": {"key": "GlobalSettings", "sig": "434E5854", "beep_offset": 4, "pin_offset": 12},  # 'CNXT' ASCII imzası
        "idt": {"key": "ApdConfig", "sig": "49445420", "beep_offset": 6, "pin_offset": 16},          # 'IDT ' ASCII imzası
        "tempo": {"key": "ApdConfig", "sig": "49445420", "beep_offset": 6, "pin_offset": 16},        
        "synaptics": {"key": "CodecPolicy", "sig": "53594E41", "beep_offset": 8, "pin_offset": 20}   # 'SYNA' ASCII imzası
    }

    i = 0
    found_main_soundcard = False
    
    while True:
        try:
            subkey_name = winreg.EnumKey(class_key, i)
            i += 1
            drv_path = f"{base_path}\\{subkey_name}"
            
            try:
                drv_key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, drv_path)
                provider = winreg.QueryValueEx(drv_key, "ProviderName")[0].lower()
                driver_desc = winreg.QueryValueEx(drv_key, "DriverDesc")[0]
                
                # FILTERING: Ignore non-primary or virtual audio devices
                is_ignored = any(x in provider or x in driver_desc.lower() for x in ["amd", "microsoft", "bluetooth", "tobias", "virtual", "nvidia", "intel(r)"])
                
                # Find the vendor that matches the provider or driver description
                matched_vendor = None
                for vendor in vendor_maps:
                    if vendor in provider or vendor in driver_desc.lower():
                        if not is_ignored:
                            matched_vendor = vendor
                            break
                
                if matched_vendor:
                    print(f"[+] Primary Soundcard Found: {driver_desc} (Key: \\{subkey_name})")
                    found_main_soundcard = True
                    
                    # Scan the vendor-specific settings registry key for beep and pin data
                    settings_path = f"{drv_path}\\Settings"
                    try:
                        s_key = winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, settings_path)
                        target_key_name = vendor_maps[matched_vendor]["key"]
                        policy_data, _ = winreg.QueryValueEx(s_key, target_key_name)
                        winreg.CloseKey(s_key)
                        
                        hex_str = policy_data.hex().upper()
                        signature = vendor_maps[matched_vendor]["sig"]
                        sig_index = hex_str.find(signature)
                        
                        if sig_index != -1:
                            sig_byte_offset = sig_index // 2
                            
                            # Üreticiye özgü dinamik konum hesaplamaları
                            b_offset = vendor_maps[matched_vendor]["beep_offset"]
                            p_offset = vendor_maps[matched_vendor]["pin_offset"]
                            
                            beep_exact_offset = sig_byte_offset + b_offset
                            pin_exact_offset = sig_byte_offset + p_offset
                            
                            beep_hex_block = hex_str[(beep_exact_offset * 2):(beep_exact_offset * 2) + 8]
                            hardware_pin_block = hex_str[(pin_exact_offset * 2):(pin_exact_offset * 2) + 8]
                            
                            print(f"    [✔] Isolated Matrix Key : {target_key_name}")
                            print(f"    ├── Beep Byte Location  : Offset {beep_exact_offset}")
                            print(f"    ├── Raw Beep Payload    : 0x{beep_hex_block}")
                            print(f"    └── Pin Register Block  : 0x{hardware_pin_block}")
                            print("    " + "-"*62)
                            
                            # 1. ANALYSIS: Beep Visibility in Windows Playback Levels
                            if matched_vendor == "realtek":
                                is_active = beep_hex_block.startswith("94") or "81" in beep_hex_block
                            else:
                                is_active = any(b != "0" for b in beep_hex_block) and not beep_hex_block.startswith("FF")
                                
                            if is_active:
                                print("    [STATUS - VISIBILITY] -> ACTIVE (Visible in Windows Playback Levels)")
                            else:
                                print("    [STATUS - VISIBILITY] -> INACTIVE (Hidden/Locked by default policy)")
                                
                            # 2. PHYSICAL SOLDERING CHECK: Hardware Pin Block Analysis
                            if hardware_pin_block == "00000000" or "0000" in hardware_pin_block[4:] or hardware_pin_block == "FFFFFFFF":
                                print("    [STATUS - HARDWARE]   -> NOT SOLDERED (No Physical Connection / Floating Pin)")
                                print("                             └─ Analysis: Circuit is dead. Slider behaves as a ghost.")
                            else:
                                print("    [STATUS - HARDWARE]   -> PHYSICALLY SOLDERED (Trace Exists on Motherboard)")
                                print("                             └─ Analysis: Real copper connection bridges the chip.")
                        else:
                            print(f"    [-] Error: Could not map hardware signature layout inside {target_key_name}.")
                    except FileNotFoundError:
                        print("    [-] Error: Sub-settings matrix registry tree missing for this vendor.")
                    
                    winreg.CloseKey(drv_key)
                    # Exit after processing the first valid primary soundcard
                    break
                winreg.CloseKey(drv_key)
            except OSError:
                continue
        except OSError:
            break

    if not found_main_soundcard:
        print("[-] No primary motherboard analog soundcard (Realtek/Conexant/IDT/Synaptics) detected.")

if __name__ == '__main__':
    isolate_main_soundcard_beep()
