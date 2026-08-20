# CalcNova Programmer Mode

CalcNova programmer tooling is a completed 2.8.03 feature built on `System.Numerics.BigInteger` with explicit word-size boundaries for operations that model fixed-width machine integers.

## Radix conversion

`RadixConverter` supports bases 2 through 36.

The shared programmer UI exposes the full 2–36 range while keeping synchronized common-radix representations for:

- binary (2);
- octal (8);
- decimal (10);
- hexadecimal (16).

Radix parsing/formatting is arbitrary-precision and is distinct from fixed-width machine-integer interpretation.

## Word-size semantics

`BitwiseCalculator` validates domain word sizes from 1 through 4096 bits. The shared programmer UI offers practical presets of:

- 8 bits;
- 16 bits;
- 32 bits;
- 64 bits;
- 128 bits.

Fixed-width operations mask results to the configured word size. Signed interpretation uses two's-complement semantics.

Supported interactive operations include:

- AND;
- OR;
- XOR;
- NOT;
- left shift;
- logical right shift;
- arithmetic right shift;
- unsigned interpretation;
- signed two's-complement interpretation;
- fixed-width bit-string output.

Binary, octal, and hexadecimal displays remain masked to the selected word width. Decimal and interpreted-value displays follow the selected signed/unsigned interpretation. This preserves two's-complement behavior without presenting misleading negative non-decimal formatting.

## Bit inspection and toggling

`BitwiseCalculator.IsBitSet(...)` and `ToggleBit(...)` validate the requested bit index against the configured word size. Bit index 0 is the least-significant bit.

The shared UI renders a most-significant-bit-first interactive collection for the selected 8/16/32/64/128-bit word size.

Each interactive bit exposes an understandable accessible state label such as:

```text
Bit 7, set
```

or the corresponding cleared state.

`ProgrammerViewModel` keeps input, binary/octal/decimal/hex, fixed-width bit pattern, and interpreted-value displays synchronized after bit toggles or bitwise operations.

## Large-word presentation

Large programmer grids use deterministic grouping so 64-bit and 128-bit representations remain easier to scan.

The shared presentation includes byte-grouped display behavior for large word sizes while retaining the complete textual fixed-width representation. Grouping is structural and does not rely on color alone.

## Copy workflows

Programmer mode includes explicit copy actions for:

- radix representations;
- fixed-width bit representations.

Clipboard access is user-triggered through the shared platform abstraction. A clipboard failure must not change the underlying programmer value or fabricate a copy success state.

## Unicode code-point utilities

Programmer tooling also provides Unicode scalar/code-point utilities without treating individual UTF-16 code units as complete characters.

`UnicodeCodePointHelper` supports:

- parsing forms such as `U+0041`, `0x03C0`, and `1F600`;
- rejecting surrogate code points;
- rejecting values outside the Unicode scalar range;
- formatting scalars as canonical `U+XXXX`-style text;
- converting a valid scalar to its text representation;
- enumerating text by Unicode scalars using `Rune` semantics;
- enforcing a configurable inspection limit to prevent unbounded UI work.

The shared Code/Unicode surface exposes code-point-to-text and text-to-code-point workflows plus local metadata presentation.

See [UNICODE_METADATA.md](UNICODE_METADATA.md).

## Accessibility

Programmer mode participates in the shared accessibility contract:

- bit buttons are keyboard-focusable on keyboard-capable targets;
- bit cells expose state-aware accessible names;
- fixed-width text remains available as a non-grid representation;
- large grids remain reachable through the adaptive/overflow behavior;
- grouping does not depend only on color;
- copy actions are explicit controls;
- shared focus/high-contrast/touch-target rules apply.

Actual TalkBack, VoiceOver, desktop screen-reader, large-text, and platform focus-order behavior is runtime evidence. It should only be marked PASS after testing on the relevant platform/toolchain.

See [ACCESSIBILITY.md](ACCESSIBILITY.md), [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md), and [ADAPTIVE_LAYOUT.md](ADAPTIVE_LAYOUT.md).

## Correctness notes

Programmer bitwise results model fixed-width binary representations; radix conversion itself remains arbitrary-precision. These are intentionally distinct semantics.

Shift counts are non-negative, and the shared programmer workflow rejects counts beyond the selected word size where required by the operation contract.

Signed and unsigned interpretations must remain deterministic for the selected width. Any change to masking, shifting, or two's-complement behavior requires boundary regression tests.

## Validation

Programmer behavior is protected by focused domain/application regression coverage for areas including:

- base 2–36 conversion;
- large integer round trips;
- signed/two's-complement interpretation;
- fixed-width bitwise operations;
- shifts;
- bit inspection/toggling;
- full interactive grids;
- grouped large-word presentation;
- radix/fixed-width copy workflows;
- Unicode scalar helpers;
- invalid radix/input boundaries.

The integrated SDK-independent gate is:

```bash
python tools/release_preflight.py
```

Compiled tests run through the normal .NET test gate described in [TESTING.md](TESTING.md).

Source/test presence is not a substitute for observed platform accessibility/runtime evidence. Record target-specific results using `PASS / FAIL / BLOCKED / NOT RUN`.

## 2.8.03 classification

For CalcNova 2.8.03:

- base 2–36 conversion: **COMPLETE**;
- 8/16/32/64/128-bit UI presets: **COMPLETE**;
- signed/unsigned two's-complement interpretation: **COMPLETE**;
- AND/OR/XOR/NOT: **COMPLETE**;
- left/logical-right/arithmetic-right shifts: **COMPLETE**;
- interactive bit grids: **COMPLETE**;
- accessible bit-state names: **COMPLETE**;
- large-word grouping: **COMPLETE**;
- radix/fixed-width copy actions: **COMPLETE**;
- Unicode scalar utilities: **COMPLETE**.

Future changes are maintenance or optional enhancements rather than missing 2.8.03 requirements.
