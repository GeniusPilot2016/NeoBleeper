//  PawnIO Modules - Modules for various hardware to be used with PawnIO.
//  Copyright (C) 2026 GeniusPilot2016
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 2.1 of the License, or (at your option) any later version.
//
//  This library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//  Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//  SPDX-License-Identifier: LGPL-2.1-or-later

#include <pawnio.inc>

#define SPEAKER_MIN_HZ          20
#define SPEAKER_MAX_HZ          20000
#define SPEAKER_MAX_DURATION_MS 60000

new VAProc:hal_make_beep;

bool:is_valid_frequency(frequency) {
    return frequency >= SPEAKER_MIN_HZ && frequency <= SPEAKER_MAX_HZ;
}

NTSTATUS:make_beep(frequency) {
    new result;
    new NTSTATUS:status = invoke(hal_make_beep, result, frequency);

    if (status != STATUS_SUCCESS)
        return status;

    return result ? STATUS_SUCCESS : STATUS_UNSUCCESSFUL;
}

/// Start a continuous tone through the system speaker.
///
/// @param in [0] = frequency in hertz (20 through 20000)
/// @param in_size Must be 1
/// @return An NTSTATUS
/// @note Call ioctl_stop when the tone is no longer needed.
DEFINE_IOCTL_SIZED(ioctl_start, 1, 0) {
    if (!is_valid_frequency(in[0]))
        return STATUS_INVALID_PARAMETER;

    return make_beep(in[0]);
}

/// Stop the current system-speaker tone.
///
/// @param in Unused
/// @param in_size Must be 0
/// @return An NTSTATUS
DEFINE_IOCTL_SIZED(ioctl_stop, 0, 0) {
    return make_beep(0);
}

/// Play a system-speaker tone synchronously.
///
/// @param in [0] = frequency in hertz (20 through 20000)
///            [1] = duration in milliseconds (1 through 60000)
/// @param in_size Must be 2
/// @return An NTSTATUS
DEFINE_IOCTL_SIZED(ioctl_beep, 2, 0) {
    new frequency = in[0];
    new duration_ms = in[1];

    if (!is_valid_frequency(frequency) ||
        duration_ms < 1 || duration_ms > SPEAKER_MAX_DURATION_MS)
        return STATUS_INVALID_PARAMETER;

    new NTSTATUS:status = make_beep(frequency);
    if (status != STATUS_SUCCESS)
        return status;

    status = microsleep(duration_ms * 1000);
    new NTSTATUS:stop_status = make_beep(0);

    return status != STATUS_SUCCESS ? status : stop_status;
}

NTSTATUS:main() {
    hal_make_beep = get_proc_address("HalMakeBeep");
    return hal_make_beep == VAProc:0 ?
        STATUS_PROCEDURE_NOT_FOUND : STATUS_SUCCESS;
}

public NTSTATUS:unload() {
    if (hal_make_beep != VAProc:0)
        make_beep(0);

    return STATUS_SUCCESS;
}
