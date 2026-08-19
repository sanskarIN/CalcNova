# Programmer Mode

CalcNova programmer tooling is built on `System.Numerics.BigInteger` with explicit word-size boundaries for operations that model fixed-width machine integers.

## Radix conversion

`RadixConverter` supports bases 2 through 36.

Common app representations are:

- binary (2)
- octal (8)
- decimal (10)
- hexadecimal (16)

The domain converter also supports custom bases through 36 even when the current shared UI only exposes a subset as quick selections.

## Word-size semantics

`BitwiseCalculator` validates word sizes from 1 through 4096 bits. The shared programmer view model currently offers practical presets of 8, 16, 32, 64, and 128 bits.

Fixed-width operations mask results to the configured word size. Signed interpretation uses two's-complement semantics.

Supported operations include:

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

## Bit inspection and toggling

`BitwiseCalculator.IsBitSet(...)` and `ToggleBit(...)` validate the requested bit index against the configured word size.

Bit index 0 is the least-significant bit. Out-of-range indexes are rejected rather than silently wrapped.

`ProgrammerViewModel.ToggleBit(...)` updates the current input in its selected radix and refreshes binary, octal, decimal, hexadecimal, bit-pattern, and interpreted-value displays together.

The remaining UI task is to render an accessible bit-toggle grid that binds indexes to `ToggleBitCommand` while remaining usable for large word sizes.

## Unicode code-point utilities

`UnicodeCodePointHelper` supports Unicode scalar values without treating individual UTF-16 code units as complete characters.

Supported backend operations include:

- parsing forms such as `U+0041`, `0x03C0`, and `1F600`;
- rejecting surrogate code points and values outside the Unicode scalar range;
- formatting scalars as canonical `U+XXXX` style text;
- converting a valid scalar to its text representation;
- enumerating a string by Unicode scalars using `Rune` semantics;
- enforcing a configurable inspection limit to prevent unbounded UI work.

A visible code-point helper should provide both text-to-code-point and code-point-to-text workflows with screen-reader-friendly labels.

## Accessibility requirements for the bit grid

When the bit grid is implemented:

- expose a readable accessible name such as `Bit 7, set`;
- keep logical keyboard order predictable;
- show group separators without relying on color alone;
- allow keyboard activation of every bit;
- provide a non-grid textual bit pattern as an alternative representation;
- avoid forcing horizontal scrolling for common 8/16/32-bit views on compact screens;
- virtualize or group larger representations rather than rendering an uncontrolled number of focusable elements.

## Correctness notes

Programmer bitwise results model fixed-width binary representations; radix conversion itself remains arbitrary-precision. These are distinct semantics and should stay distinct in the UI.

## Validation

New bit-toggle and Unicode helper regression tests are present in programmer/app test projects. In the current continuation environment, the .NET SDK is unavailable, so those tests are **NOT RUN locally** until an actual compatible build/test environment executes them.
