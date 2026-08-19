# CalcNova Accessibility Runtime Test Matrix

This matrix records release evidence that cannot be proven by source inspection alone. Every entry begins as `NOT RUN` and must remain that way until the named target/environment is actually exercised.

## Evidence rules

Use only these result states:

- `PASS` — the check ran on the named target and passed;
- `FAIL` — the check ran and a reproducible problem was observed;
- `BLOCKED` — the target/check was available but an external prerequisite prevented completion;
- `NOT RUN` — no valid execution evidence exists yet.

Do not convert source-level validators, unit-test source files, or workflow definitions into runtime PASS evidence.

## Desktop

| Check | Windows | Linux | macOS |
| --- | --- | --- | --- |
| Keyboard-only calculator workflow | NOT RUN | NOT RUN | NOT RUN |
| Mode navigation and focus visibility | NOT RUN | NOT RUN | NOT RUN |
| Graph keyboard pan/zoom/reset/fit | NOT RUN | NOT RUN | NOT RUN |
| 64/128-bit programmer focus order | NOT RUN | NOT RUN | NOT RUN |
| High-contrast preference | NOT RUN | NOT RUN | NOT RUN |
| Large text/display scaling | NOT RUN | NOT RUN | NOT RUN |
| Clipboard paste/copy | NOT RUN | NOT RUN | NOT RUN |
| Onboarding initial/restored focus | NOT RUN | NOT RUN | NOT RUN |
| Screen-reader navigation | NOT RUN | NOT RUN | NOT RUN |

Suggested assistive-technology coverage should use platform-standard tools where available, but the exact tool/version belongs in release evidence rather than being assumed here.

## Browser/WebAssembly

| Check | Chromium-family | Firefox | WebKit/Safari |
| --- | --- | --- | --- |
| Keyboard-only calculator workflow | NOT RUN | NOT RUN | NOT RUN |
| Shell shortcut conflict check | NOT RUN | NOT RUN | NOT RUN |
| Graph keyboard interaction | NOT RUN | NOT RUN | NOT RUN |
| Focus visibility | NOT RUN | NOT RUN | NOT RUN |
| 200%+ browser zoom/large text | NOT RUN | NOT RUN | NOT RUN |
| Clipboard permission flows | NOT RUN | NOT RUN | NOT RUN |
| Onboarding keyboard containment | NOT RUN | NOT RUN | NOT RUN |
| Screen-reader/browser combination | NOT RUN | NOT RUN | NOT RUN |

## Android

| Check | Status |
| --- | --- |
| TalkBack traversal | NOT RUN |
| Switch/external-keyboard traversal | NOT RUN |
| Portrait compact layout | NOT RUN |
| Landscape compact layout | NOT RUN |
| Large font/display size | NOT RUN |
| 64/128-bit programmer interaction | NOT RUN |
| Clipboard actions | NOT RUN |
| High-contrast preference behavior | NOT RUN |
| Onboarding traversal and dismissal | NOT RUN |

## iOS/iPadOS

| Check | Status |
| --- | --- |
| VoiceOver traversal | NOT RUN |
| External-keyboard traversal | NOT RUN |
| iPhone portrait compact layout | NOT RUN |
| iPhone landscape compact layout | NOT RUN |
| iPad adaptive layout | NOT RUN |
| Dynamic Type / larger accessibility sizes | NOT RUN |
| 64/128-bit programmer interaction | NOT RUN |
| Clipboard actions | NOT RUN |
| Onboarding traversal and dismissal | NOT RUN |

## Required workflow scenarios

Regardless of platform, validate these user journeys:

1. launch with onboarding visible, navigate its actions, dismiss it, and confirm focus restoration;
2. enter/evaluate a calculation without pointer input where a hardware keyboard exists;
3. move between modes and return to Calculator without losing a usable focus position;
4. toggle calculator memory and angle-unit controls;
5. operate programmer word-size/radix controls and a representative set of bit cells;
6. search/select converter units, save favorites, clear recents, and copy a result;
7. use graph textual output and keyboard/pointer viewport interaction;
8. search/favorite/export history;
9. enable high contrast and repeat representative focus/navigation checks;
10. increase text/display scaling and confirm content remains reachable rather than clipped.

## Evidence recording

For every PASS/FAIL/BLOCKED transition, record at minimum:

- platform and OS version;
- CalcNova commit SHA or release tag;
- device/browser and relevant assistive-technology version;
- exact scenario/check;
- observed result;
- linked issue for every failure or accepted limitation.

This document is deliberately conservative. Its initial `NOT RUN` state is evidence discipline, not a statement that the implemented source contracts are absent.
