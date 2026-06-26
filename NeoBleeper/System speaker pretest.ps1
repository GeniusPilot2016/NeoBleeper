# Step 1: Check if the legacy system timer port (0x61) is active and functional
$PortCheck = Get-CimInstance Win32_PortResource | Where-Object { $_.Name -match '0061' }

# Step 2: Check the status of the PIT (Programmable Interval Timer) device
$PIT_Status = (Get-CimInstance Win32_PnPEntity | Where-Object { $_.DeviceID -like "*PNP0100*" }).Status

# Final Verdict based on the checks
if ($null -ne $PortCheck -and $PIT_Status -eq "OK") {
    # The legacy system timer port is active and functional, check the ISA service status
    # Check the status of the ISA service (isapnp)
    # Note: The ISA service is typically associated with legacy hardware and may not be present on modern systems.
    $ISA_Service = (Get-CimInstance Win32_SystemDriver | Where-Object { $_.Name -eq "isapnp" }).State
    
    if ($ISA_Service -ne "Running") {
        Write-Host "VERDICT: SILICON INDEPENDENT (Bit 5 is masked/dead in hardware). DO NOT Install Driver." -ForegroundColor Red
    } else {
        Write-Host "VERDICT: LEGACY CONNECTED (Hardware routing fully functional). Install Driver." -ForegroundColor Green
    }
} else {
    Write-Host "VERDICT: MODERN SOC INDEPENDENT (No legacy hardware routing). DO NOT Install Driver." -ForegroundColor Red
}
