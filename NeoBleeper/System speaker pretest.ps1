<#
.SYNOPSIS
    Automated check for functional legacy interrupt paths.
.DESCRIPTION
    Checks the system's IRQ allocation to see if the PCH has assigned 
    hardware interrupts to legacy timer services, which is the 
    prerequisite for a functional 0x61 speaker path.
#>

# Get the hardware resources and filter for IRQ usage
$Resources = Get-CimInstance -ClassName Win32_IRQResource | Where-Object { $_.Availability -eq 1 }

# If the legacy IRQ 0 (Timer) is NOT present in the system, 
# then the Speaker/NMI logic is not hardware-routed.
$LegacyTimerPath = $Resources | Where-Object { $_.IRQNumber -eq 0 }
# Output the results as a custom object for easy consumption
[PSCustomObject]@{
    LegacyTimerIRQ_Active = if ($LegacyTimerPath) { $true } else { $false }
    Port0x61_Status       = if ($LegacyTimerPath) { "Potentially Functional" } else { "Dead End (No IRQ path)" }
}