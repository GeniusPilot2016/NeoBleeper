<#
.SYNOPSIS
    Detailed, name-independent pretest for whether port 0x61 (legacy PC
    speaker / NMI status register) is plausibly speaker-functional on
    this machine, used to decide whether NeoBleeper should prompt for
    PawnIO installation.

.DESCRIPTION
    Does NOT rely on:
      - Win32_IRQResource (legacy PnP-BIOS data, unreliable/empty on
        modern ACPI systems)
      - Device display name ("System Speaker" vs "Motherboard Resources"
        is not a reliable indicator - PNP0C02 "Motherboard Resources"
        frequently hides the real 0x61 reservation)
      - beep.sys (rewritten since Windows 7 to use the audio stack
        instead of port 0x61, so it cannot be used to confirm legacy
        port functionality)
      - AML/ACPI table parsing (heuristic, fragile, requires admin
        rights to read HKLM\HARDWARE\ACPI, and OEM device naming is
        inconsistent/arbitrary, so name resolution from raw AML bytes
        is unreliable)

    Instead, this script reasons purely from WMI's resource-allocation
    graph (Win32_PnPAllocatedResource -> Win32_PortResource), which
    reflects real ACPI-resolved port claims and requires no elevation:

      1. Finds every PNP device that has been allocated a port range
         containing 0x61, regardless of device name or PNP ID class
         (PNP0800 "System Speaker" or any PNP0C02 "Motherboard
         Resources" instance).
      2. For each such device, computes the WIDTH of the specific
         range containing 0x61. A width of 1-2 bytes matches the
         known shape of a dedicated legacy register reservation
         (0x61 alone, or 0x61-0x62 grouped with the NMI status bit).
         A wide multi-port range is more likely an unrelated chipset
         catch-all block that merely happens to span over 0x61.
      3. For each such device, checks whether it claims ANY OTHER
         port ranges. A device that claims ONLY the narrow range
         containing 0x61 - and nothing else - is behaving like a
         dedicated single-purpose legacy device (a "shadow PNP0800"),
         which is strong evidence of a deliberate, real reservation
         rather than an incidental sweep inside a generic block.
      4. Cross-references whether 0x60/0x64 (8042 keyboard controller
         registers) are claimed by the SAME device, purely as
         informational context - real-world systems can show 0x61
         grouped with the keyboard controller OR split into its own
         separate device entirely, both are plausible legitimate
         patterns and are reported, not scored.

    VERDICT TIERS (for use as a PawnIO-prompt gate):
      - "No claim"        -> No device claims 0x61 at all. No legacy
                              IO path exists regardless of OEM naming.
                              Safe to skip the PawnIO prompt entirely.
      - "High confidence"  -> Narrow + standalone claim. Strong,
                              name-independent positive signal.
                              Worth prompting for PawnIO.
      - "Low confidence"   -> 0x61 only appears inside a broad,
                              multi-purpose block. Ambiguous - cannot
                              be confirmed or ruled out from resource
                              data alone. Treat as inconclusive; only
                              an actual runtime I/O attempt through
                              PawnIO can resolve this case definitively.

    NOTE: No static check (including this one) can prove a PHYSICAL
    speaker/buzzer is actually wired to the gate - only that an IO
    range is reserved for it. OEM firmware can declare the legacy
    descriptor for compatibility even after removing the speaker from
    the BOM. This script is a fast pre-filter to avoid prompting on
    machines with clearly no IO path, not a guarantee of audible
    output.

.NOTES
    Requires no administrator privileges.
    Requires no external tools (acpidump/iasl/etc).
    Works on x64 only - port I/O (IN/OUT instructions) and therefore
    port 0x61 itself does not exist as a concept on ARM64, which has
    no legacy PC/AT IO port space. Caller should short-circuit on
    ARM64 before invoking this script.
#>

[CmdletBinding()]
param()

function Get-PnpFriendlyNameMap {
    [CmdletBinding()]
    param()

    # Build a DeviceID -> Name lookup once, so every claim can be
    # annotated with its human-readable device name (e.g. "Motherboard
    # Resources" or "System Speaker") alongside the raw DeviceID -
    # without re-querying WMI per claim.
    $map = @{}
    try {
        Get-CimInstance -ClassName Win32_PNPEntity -ErrorAction Stop | ForEach-Object {
            if ($_.DeviceID) {
                $map[$_.DeviceID] = $_.Name
            }
        }
    } catch {
        Write-Verbose "Could not enumerate Win32_PNPEntity for friendly names: $_"
    }
    return $map
}

function Get-Port0x61ClaimDetails {
    [CmdletBinding()]
    param()

    Write-Verbose "Querying Win32_PnPAllocatedResource for port resource associations..."
    $assoc = @(Get-CimInstance -ClassName Win32_PnPAllocatedResource -ErrorAction Stop |
        Where-Object { $_.Antecedent -match 'Win32_PortResource' })

    $nameMap = Get-PnpFriendlyNameMap

    $emptyResult = [PSCustomObject]@{
        Owners        = @()
        AllScanned    = @()
        TotalAssocCount = $assoc.Count
    }

    if ($assoc.Count -eq 0) {
        Write-Verbose "No Win32_PortResource associations found at all on this system."
        return $emptyResult
    }

    Write-Verbose "Found $($assoc.Count) total port resource associations. Parsing ranges..."

    # Pre-parse every association once into a flat, typed table so we
    # can both find 0x61 owners AND look up "what else does this same
    # device own" without re-querying WMI repeatedly. This is kept in
    # full (not filtered down to 0x61 matches) so a negative result can
    # still show every device that WAS examined and what it claims.
    $parsed = @(foreach ($a in $assoc) {
        # Antecedent/Dependent are CimInstance objects (NOT strings) -
        # access their properties directly rather than regex-parsing
        # a path representation. This is more robust and is what the
        # diagnostic dump confirmed is actually returned here.
        $start = $null
        $end   = $null

        $startProp = $a.Antecedent.CimInstanceProperties['StartingAddress']
        if ($startProp -and $null -ne $startProp.Value) {
            $start = [Convert]::ToInt64($startProp.Value)
        }

        $endProp = $a.Antecedent.CimInstanceProperties['EndingAddress']
        if ($endProp -and $null -ne $endProp.Value) {
            $end = [Convert]::ToInt64($endProp.Value)
        }

        # Some single-port reservations may not populate EndingAddress
        # distinctly from StartingAddress (single-byte range) - fall
        # back to treating it as a 1-byte range rather than dropping
        # the row entirely if EndingAddress couldn't be read.
        if ($null -ne $start -and $null -eq $end) { $end = $start }

        $deviceId = $null
        try { $deviceId = $a.Dependent.CimInstanceProperties['DeviceID'].Value } catch {}
        if (-not $deviceId) { $deviceId = "<unparsed>" }

        $deviceName = if ($nameMap.ContainsKey($deviceId)) { $nameMap[$deviceId] } else { "<unknown device name>" }

        if ($null -ne $start -and $null -ne $end) {
            [PSCustomObject]@{
                DeviceID   = $deviceId
                DeviceName = $deviceName
                RangeStart = $start
                RangeEnd   = $end
                Dependent  = $deviceId   # used as a same-device grouping key below
            }
        }
    })

    if ($parsed.Count -eq 0) {
        Write-Verbose "Associations were found but none could be parsed into valid port ranges."
        return $emptyResult
    }

    # Devices that have a range covering 0x61
    $owners = @($parsed | Where-Object { 0x61 -ge $_.RangeStart -and 0x61 -le $_.RangeEnd })

    if ($owners.Count -eq 0) {
        Write-Verbose "No range among the parsed associations contains 0x61."
        return [PSCustomObject]@{
            Owners          = @()
            AllScanned      = $parsed
            TotalAssocCount = $assoc.Count
        }
    }

    $results = foreach ($owner in $owners) {

        $width = $owner.RangeEnd - $owner.RangeStart + 1

        # Every range claimed by this SAME device (by Dependent path,
        # which uniquely identifies the device instance)
        $sameDeviceRanges = $parsed | Where-Object { $_.Dependent -eq $owner.Dependent }

        $distinctRangeStrings = $sameDeviceRanges |
            ForEach-Object { "0x{0:X2}-0x{1:X2}" -f $_.RangeStart, $_.RangeEnd } |
            Sort-Object -Unique

        $isStandaloneDevice = ($distinctRangeStrings.Count -eq 1)

        # Informational only: does this same device also touch the
        # 8042 keyboard controller registers? Both grouped-with and
        # separate-from the keyboard controller are plausible real
        # patterns in the wild, so this is reported, not scored.
        $alsoClaims60 = ($sameDeviceRanges | Where-Object { 0x60 -ge $_.RangeStart -and 0x60 -le $_.RangeEnd }) -as [bool]
        $alsoClaims64 = ($sameDeviceRanges | Where-Object { 0x64 -ge $_.RangeStart -and 0x64 -le $_.RangeEnd }) -as [bool]

        [PSCustomObject]@{
            DeviceID            = $owner.DeviceID
            DeviceName          = $owner.DeviceName
            RangeStart          = ('0x{0:X2}' -f $owner.RangeStart)
            RangeEnd            = ('0x{0:X2}' -f $owner.RangeEnd)
            Width               = $width
            IsNarrowClaim       = ($width -le 2)
            IsStandaloneDevice  = $isStandaloneDevice
            OtherRangesOnDevice = $distinctRangeStrings
            AlsoClaims0x60      = [bool]$alsoClaims60
            AlsoClaims0x64      = [bool]$alsoClaims64
        }
    }

    return [PSCustomObject]@{
        Owners          = @($results)
        AllScanned      = $parsed
        TotalAssocCount = $assoc.Count
    }
}

function Write-Port0x61Report {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        $Result   # PSCustomObject: Owners, AllScanned, TotalAssocCount
    )

    $divider = "-" * 78
    $owners     = @($Result.Owners)
    $allScanned = @($Result.AllScanned)

    Write-Host ""
    Write-Host $divider
    Write-Host "  Port 0x61 Resource Claim Report"
    Write-Host $divider
    Write-Host ""
    Write-Host ("  Total port-resource associations examined: {0}" -f $Result.TotalAssocCount)
    Write-Host ("  Distinct device/range entries parsed       : {0}" -f $allScanned.Count)
    Write-Host ("  Entries whose range covers 0x61            : {0}" -f $owners.Count)

    if ($owners.Count -eq 0) {
        Write-Host ""
        Write-Host "  REASON: No PNP device claims port 0x61 on this system." -ForegroundColor Red
        Write-Host "  No legacy IO path to the speaker gate exists, regardless of OEM"
        Write-Host "  device naming (checked both 'System Speaker' / PNP0800-style and"
        Write-Host "  'Motherboard Resources' / PNP0C02-style devices). PawnIO cannot"
        Write-Host "  help here - safe to skip the prompt."

        if ($allScanned.Count -gt 0) {
            Write-Host ""
            Write-Host "  For reference, every device + port range that WAS examined:" -ForegroundColor DarkGray
            Write-Host "  $('-' * 60)" -ForegroundColor DarkGray

            $byDevice = $allScanned | Group-Object DeviceID
            foreach ($grp in $byDevice) {
                $first = $grp.Group[0]
                $ranges = ($grp.Group | ForEach-Object { "0x{0:X2}-0x{1:X2}" -f $_.RangeStart, $_.RangeEnd }) -join ", "
                Write-Host ("    {0}" -f $first.DeviceName) -ForegroundColor DarkGray
                Write-Host ("      DeviceID : {0}" -f $first.DeviceID) -ForegroundColor DarkGray
                Write-Host ("      Ranges   : {0}" -f $ranges) -ForegroundColor DarkGray
            }
        } else {
            Write-Host ""
            Write-Host "  No port-resource associations existed to examine at all - this" -ForegroundColor DarkGray
            Write-Host "  system reports zero PNP devices with any claimed IO port range." -ForegroundColor DarkGray
        }

        Write-Host ""
        Write-Host $divider
        return
    }

    $index = 0
    foreach ($c in $owners) {
        $index++
        Write-Host ""
        Write-Host "  Claim #$index" -ForegroundColor Cyan
        Write-Host "  $('-' * 60)"
        Write-Host ("  {0,-22}: {1}" -f "DeviceName",          $c.DeviceName)
        Write-Host ("  {0,-22}: {1}" -f "DeviceID",            $c.DeviceID)
        Write-Host ("  {0,-22}: {1}" -f "RangeStart",          $c.RangeStart)
        Write-Host ("  {0,-22}: {1}" -f "RangeEnd",            $c.RangeEnd)
        Write-Host ("  {0,-22}: {1} byte(s)" -f "Width",       $c.Width)

        $narrowColor = if ($c.IsNarrowClaim) { "Green" } else { "Yellow" }
        Write-Host ("  {0,-22}: {1}" -f "IsNarrowClaim", $c.IsNarrowClaim) -ForegroundColor $narrowColor

        $standaloneColor = if ($c.IsStandaloneDevice) { "Green" } else { "Yellow" }
        Write-Host ("  {0,-22}: {1}" -f "IsStandaloneDevice", $c.IsStandaloneDevice) -ForegroundColor $standaloneColor

        $otherRangesDisplay = if ($c.OtherRangesOnDevice.Count -gt 0) {
            $c.OtherRangesOnDevice -join ", "
        } else {
            "<none>"
        }
        Write-Host ("  {0,-22}: {1}" -f "OtherRangesOnDevice", $otherRangesDisplay)

        Write-Host ("  {0,-22}: {1}" -f "AlsoClaims0x60 (kbd)", $c.AlsoClaims0x60)
        Write-Host ("  {0,-22}: {1}" -f "AlsoClaims0x64 (kbd)", $c.AlsoClaims0x64)

        $strongPositive = $c.IsNarrowClaim -and $c.IsStandaloneDevice
        if ($strongPositive) {
            Write-Host "  Assessment            : HIGH CONFIDENCE - dedicated, standalone, narrow claim" -ForegroundColor Green
            Write-Host "  Reason                : This device claims ONLY a narrow (<=2 byte) range" -ForegroundColor Green
            Write-Host "                          covering 0x61 and nothing else - the resource shape" -ForegroundColor Green
            Write-Host "                          matches a dedicated legacy speaker/NMI register," -ForegroundColor Green
            Write-Host "                          regardless of its PNP class or display name." -ForegroundColor Green
        } else {
            Write-Host "  Assessment            : LOW CONFIDENCE - part of a broader / shared claim" -ForegroundColor Yellow
            $reasons = @()
            if (-not $c.IsNarrowClaim)      { $reasons += "range is wider than 2 bytes (likely a generic chipset block, not a dedicated register)" }
            if (-not $c.IsStandaloneDevice) { $reasons += "this device also claims other unrelated port ranges ($otherRangesDisplay)" }
            foreach ($r in $reasons) {
                Write-Host "  Reason                : $r" -ForegroundColor Yellow
            }
        }
    }

    Write-Host ""
    Write-Host $divider
}

function Get-Port0x61Verdict {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory)]
        $Result   # PSCustomObject: Owners, AllScanned, TotalAssocCount
    )

    $owners = @($Result.Owners)

    if ($owners.Count -eq 0) {
        return [PSCustomObject]@{
            Verdict        = "No claim at all: 0x61 unclaimed - safe to skip PawnIO prompt"
            RecommendPawnIOPrompt = $false
            StrongPositive = @()
            Weaker         = @()
        }
    }

    $strongPositive = @($owners | Where-Object { $_.IsNarrowClaim -and $_.IsStandaloneDevice })
    $weaker         = @($owners | Where-Object { -not ($_.IsNarrowClaim -and $_.IsStandaloneDevice) })

    if ($strongPositive.Count -gt 0) {
        $verdict = "High confidence: 0x61 has its own dedicated, standalone reservation"
        $recommend = $true
    } else {
        $verdict = "Low confidence: 0x61 only appears inside a broad, shared, or multi-purpose block"
        $recommend = $true   # still ambiguous-positive: cannot be ruled out, so still attempt
    }

    return [PSCustomObject]@{
        Verdict               = $verdict
        RecommendPawnIOPrompt = $recommend
        StrongPositive        = $strongPositive
        Weaker                = $weaker
    }
}

# --- Main ---

if ($env:PROCESSOR_ARCHITECTURE -eq "ARM64") {
    Write-Host ""
    Write-Host "Architecture: ARM64 detected." -ForegroundColor Yellow
    Write-Host "No legacy PC/AT port IO space exists on ARM64 - port 0x61 is not a" -ForegroundColor Yellow
    Write-Host "meaningful concept on this architecture. PawnIO is not applicable." -ForegroundColor Yellow
    Write-Host ""
    return
}

try {
    $result = Get-Port0x61ClaimDetails -Verbose:$VerbosePreference
} catch {
    Write-Host ""
    Write-Host "ERROR: Failed to query WMI for port resource allocation: $_" -ForegroundColor Red
    Write-Host "Treat this as inconclusive - do not assume 0x61 is unclaimed on a query failure." -ForegroundColor Red
    Write-Host ""
    return
}

Write-Port0x61Report -Result $result

$verdictObj = Get-Port0x61Verdict -Result $result

Write-Host ""
Write-Host "  Final Verdict: $($verdictObj.Verdict)" -ForegroundColor Cyan
Write-Host "  Recommend PawnIO prompt: $($verdictObj.RecommendPawnIOPrompt)" -ForegroundColor Cyan
Write-Host ""

# Return the structured result for programmatic consumption (e.g. by
# NeoBleeper's installer/setup logic calling this script and parsing
# its final output object).
[PSCustomObject]@{
    Verdict               = $verdictObj.Verdict
    RecommendPawnIOPrompt = $verdictObj.RecommendPawnIOPrompt
    MatchingClaims        = @($result.Owners)
    AllScannedDevices     = @($result.AllScanned)
    StrongPositiveClaims  = $verdictObj.StrongPositive
    WeakerClaims          = $verdictObj.Weaker
}