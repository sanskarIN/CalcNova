# Programmer Mode

CalcNova programmer tooling is built on `System.Numerics.BigInteger` with explicit word-size boundaries for operations that model fixed-width machine integers.

## Radix conversion

`RadixConverter` supports bases 2 through 36. The shared programmer UI now exposes the full 2–36 range rather than only common-radix shortcuts.

Synchronized output representations remain:

- binary (2)
- octal (8)
- decimal (10)
- hexadecimal (16)

## Word-size semantics

`BitwiseCalculator` validates domain word sizes from 1 through 4096 bits. The shared programmer UI currently offers practical presets of 8, 16, 32, 64, and 128 bits.

Fixed-width operations mask results to the configured word size. Signed interpretation uses two's-complement semantics.

Supported interactive operations include:

- AND
- OR
- XOR
- NOT
- left shift
- logical right shift
- arithmetic right shift
- unsigned interpretation
- signed two's-complement interpretation
- fixed-width bit-string output

Binary, octal, and hexadecimal displays stay masked to the selected word width. Decimal and interpreted-value displays follow the selected signed/unsigned interpretation. This avoids misleading negative non-decimal formatting while preserving two's-complement behavior.

## Bit inspection and toggling

`BitwiseCalculator.IsBitSet(...)` and `ToggleBit(...)` validate the requested bit index against the configured word size. Bit index 0 is the least-significant bit.

The shared UI renders a most-significant-bit-first interactive collection for the selected 8/16/32/64/128-bit word size. Every bit cell has a readable state label such as `Bit 7, set`, while the fixed-width textual pattern remains available as a non-grid representation.

`ProgrammerViewModel` keeps input and binary/octal/decimal/hex/bit-pattern/interpreted-value displays synchronized after a bit toggle or bitwise operation.

## Unicode code-point utilities

`UnicodeCodePointHelper` supports Unicode scalar values without treating individual UTF-16 code units as complete characters.

Supported operations include:

- parsing forms such as `U+0041`, `0x03C0`, and `1F600`;
- rejecting surrogate code points and values outside the Unicode scalar range;
- formatting scalars as canonical `U+XXXX` style text;
- converting a valid scalar to its text representation;
- enumerating a string by Unicode scalars using `Rune` semantics;
- enforcing a configurable inspection limit to prevent unbounded UI work.

The shared `Code` tab exposes both code-point-to-text and text-to-code-point workflows.

## Accessibility and compact-layout follow-up

The bit grid is functionally wired and keyboard-focusable, and bit buttons expose accessible names. Remaining release work includes:

- verify actual screen-reader announcements on each supported UI platform;
- verify logical focus order at 64/128-bit widths;
- add byte/nibble grouping without relying on color alone;
- improve compact/mobile grouping or virtualization where needed;
- add convenient accessible copy actions for radix representations;
- test high-contrast, large-text, landscape, and narrow-window behavior.

## Correctness notes

Programmer bitwise results model fixed-width binary representations; radix conversion itself remains arbitrary-precision. These are distinct semantics and stay distinct in the UI.

Shift counts are non-negative and the shared programmer workflow rejects counts beyond the selected word size.

## Validation

Bit-toggle, full-grid, custom-radix, bitwise-operation, signed-display, and Unicode helper regression tests are present across programmer/app test projects. In the current continuation environment, the .NET SDK is unavailable, so those tests are **NOT RUN locally** until an actual compatible build/test environment executes them.
