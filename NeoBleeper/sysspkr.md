# sysspkr.p

## Overview

`sysspkr.p` is a PawnIO module that exposes controlled access to the PC system speaker and the timer ports used to drive it. The code only supports `ARCH_X64` and returns `STATUS_NOT_SUPPORTED` on other architectures.

The module is intentionally restrictive:

- Only three I/O ports are allowed: the system speaker gate port, timer channel 2 data port, and timer control port.
- Timer control writes are limited to two accepted values.
- Speaker gate writes preserve the upper six bits of the current register value and only allow the low two bits to change.
- Frequency programming is limited to the range 32 through 65535.

## Hardware ports

The module uses these fixed ports and values:

- `0x61` - system speaker gate
- `0x42` - timer channel 2 data/output
- `0x43` - timer control

Supporting constants:

- `BaseFrequency = 0x1234DC`
- `LowByteMask = 0xFF`
- `HighByteShift = 8`
- `SquareWaveMode = 0xB6`
- `ResetMode = 0xB0`
- `OpenGateMask = 0x03`
- `CloseGateMask = 0xFC`
- `OpenTimerOnlyMask = 0x01`
- `DisableSpeakerDataMask = 0xFD`

## Validation rules

### Timer control writes

Only these values are accepted when writing the timer control port:

- `0xB6` (`SquareWaveMode`)
- `0xB0` (`ResetMode`)

### Speaker gate writes

Speaker gate writes are accepted only when the upper six bits match the current port value. In practice, this means the module allows low-bit toggles while preserving the rest of the register.

### Frequency values

`open_system_speaker_gate()` accepts frequencies in the range `32..65535`. The divisor written to timer channel 2 is computed as:

`BaseFrequency / frequency`

The low byte is written first, then the high byte.

## Helper functions

### `is_allowed_timer_ctrl(value)`

Returns true when `value` is one of the supported timer control modes.

### `is_allowed_speaker_gate(value)`

Compares the requested value against the current `0x61` port value and only allows changes in the low two bits.

### `is_valid_frequency(frequency)`

Ensures the requested frequency falls within the supported range.

### `is_port_allowed(port)`

Checks whether a port is one of the three supported hardware registers.

### `write_system_speaker_gate_value(value)`

Validates and writes a byte to port `0x61`.

### `write_timer_ctrl_value(value)`

Validates and writes a byte to port `0x43`.

### `write_timer_speaker_out(value)`

Validates and writes a byte to port `0x42`.

### `open_system_speaker_gate(frequency)`

Programs timer channel 2 for square-wave output, writes the divisor derived from the requested frequency, and sets the speaker gate bits to enable output.

### `close_system_speaker_gate()`

Clears the speaker enable bits in the gate register.

## IOCTL entry points

### `ioctl_pio_inb`

Reads one byte from an allowed port.

- Input: port number
- Output: read byte
- Access: denied for disallowed ports

### `ioctl_pio_outb`

Writes one byte to an allowed port.

- Input: port number and value
- Output: none
- Access: denied for disallowed ports
- Special handling:
  - port `0x61` uses speaker-gate validation
  - port `0x43` uses timer-control validation
  - port `0x42` checks byte range and writes directly

### `ioctl_open_system_speaker_gate`

Programs the timer and enables the speaker gate for a requested frequency.

### `ioctl_close_system_speaker_gate`

Disables the speaker gate.

### `ioctl_get_spk_gate`

Reads the current speaker gate value from port `0x61`.

### `ioctl_set_spk_gate`

Writes a validated speaker gate value to port `0x61`.

### `ioctl_get_timer_ctrl`

Reads the current timer control value from port `0x43`.

### `ioctl_get_timer_out`

Reads the current timer channel 2 data value from port `0x42`.

## Module lifecycle

### `main()`

Returns `STATUS_NOT_SUPPORTED` unless the runtime architecture is `ARCH_X64`. On x64, initialization succeeds.

### `unload()`

Always returns `STATUS_SUCCESS`.

## Method usage instructions

### High-level speaker control

Use these methods for normal tone generation:

1. Call `ioctl_open_system_speaker_gate` with `in[0] = frequency`.
2. Wait in caller logic for the desired beep/tone duration.
3. Call `ioctl_close_system_speaker_gate` to stop audio.

Expected statuses:

- `STATUS_SUCCESS` on valid sequence.
- `STATUS_INVALID_PARAMETER` if frequency is outside `32..65535`.
- `STATUS_ACCESS_DENIED` if any protected write check fails.

### Helper methods

#### `is_allowed_timer_ctrl(value)`

How to use:

- Internal validation before writing timer control.
- Accepts only `0xB6` or `0xB0`.

#### `is_allowed_speaker_gate(value)`

How to use:

- Internal validation before writing port `0x61`.
- Ensures only low two bits are changed compared to current register state.

#### `is_valid_frequency(frequency)`

How to use:

- Run before calling `open_system_speaker_gate`.
- Valid range is `32..65535`.

#### `is_port_allowed(port)`

How to use:

- Check before generic port read/write paths.
- Allowed ports: `0x61`, `0x42`, `0x43`.

#### `write_system_speaker_gate_value(value)`

How to use:

- Use when writing speaker gate bits safely.
- Returns `STATUS_ACCESS_DENIED` if protected bits would change.

#### `write_timer_ctrl_value(value)`

How to use:

- Use when programming timer control register.
- Only accepts square wave (`0xB6`) or reset (`0xB0`) modes.

#### `write_timer_speaker_out(value)`

How to use:

- Use for each byte of timer channel 2 divisor.
- Accepted value range is one byte (`0x00..0xFF`).

#### `open_system_speaker_gate(frequency)`

How to use:

1. Pass target frequency.
2. Method sets timer control to square-wave mode.
3. Method writes divisor low byte, then high byte.
4. Method enables gate bits on `0x61`.

Use this instead of manual `ioctl_pio_outb` for safe programming.

#### `close_system_speaker_gate()`

How to use:

- Call to disable speaker output by clearing enable bits on `0x61`.

### IOCTL methods

#### `ioctl_pio_inb`

How to use:

- Input buffer: `in[0] = port`
- Output buffer: `out[0] = byte read`
- Use only for `0x61`, `0x42`, `0x43`

#### `ioctl_pio_outb`

How to use:

- Input buffer: `in[0] = port`, `in[1] = value`
- Writes are validated by port-specific safety checks.
- Use only when you need direct low-level access.

#### `ioctl_open_system_speaker_gate`

How to use:

- Input buffer: `in[0] = frequency`
- Recommended API for enabling tone.

#### `ioctl_close_system_speaker_gate`

How to use:

- No input required.
- Recommended API for stopping tone.

#### `ioctl_get_spk_gate`

How to use:

- No input required.
- Output buffer: `out[0] = current port 0x61 value`

#### `ioctl_set_spk_gate`

How to use:

- Input buffer: `in[0] = value`
- Value is validated to protect upper six bits.

#### `ioctl_get_timer_ctrl`

How to use:

- No input required.
- Output buffer: `out[0] = current port 0x43 value`

#### `ioctl_get_timer_out`

How to use:

- No input required.
- Output buffer: `out[0] = current port 0x42 value`

### Lifecycle methods

#### `main()`

How to use:

- Module entry point.
- Must run on `ARCH_X64`; otherwise returns `STATUS_NOT_SUPPORTED`.

#### `unload()`

How to use:

- Module unload callback.
- Returns `STATUS_SUCCESS`.

## Notes

- The module depends on `pawnio.inc`.
- The implementation is designed to prevent arbitrary port access by enforcing a strict allowlist.
- The speaker enable flow follows the standard timer-channel-2 square-wave programming sequence.