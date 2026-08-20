# CalcNova 2.8.03 Onboarding

CalcNova onboarding is optional, local-first, skippable, versioned, and part of the completed 2.8.03 shared application baseline.

It introduces important capabilities without requiring an account, collecting onboarding telemetry, or repeatedly interrupting returning users.

## Completed implementation

The onboarding source includes:

- `AppSettings.CompletedOnboardingVersion` for local completion state;
- `OnboardingPolicy.CurrentVersion` for the current content boundary;
- normalization/rejection rules for invalid negative versions;
- deferred first-run display until settings load succeeds;
- `SettingsViewModel.ShouldShowOnboarding` policy state;
- safe startup behavior when settings cannot be loaded;
- Complete and Skip commands that persist the current onboarding version;
- settings-repository integration for native and Browser storage;
- ordinary settings reset behavior that preserves onboarding completion;
- the shared `OnboardingOverlay.axaml` surface;
- major-mode, keyboard/touch, privacy/local-first, optional currency-network, and no-account guidance;
- `Ctrl+PageUp/PageDown` and `Ctrl+Home/End` keyboard guidance;
- explicit accessible Skip/Start actions;
- suppression of background calculator/mode shortcuts while onboarding is open;
- focus entry into the onboarding action surface;
- queued focus restoration to Calculator input after dismissal;
- reviewed English/Hindi live localization for onboarding semantic text;
- unit/source/headless regression coverage;
- SDK-independent onboarding source validation.

Target-platform behavior such as screen-reader announcements, actual focus timing, text scaling, and orientation remains runtime evidence and should only be marked PASS when observed.

## User flow

The onboarding experience intentionally uses one concise scrollable surface instead of a forced multi-page tour.

Its structure covers:

1. **Welcome** — CalcNova's fast/precise/private positioning.
2. **Calculate your way** — standard/scientific and advanced mode overview.
3. **Keyboard and touch friendly** — navigation shortcuts and hardware-key guidance.
4. **Local-first by default** — local history/preferences, offline physical units, optional network-enhanced currency behavior, and no account requirement.
5. **Ready** — immediate Skip and Start calculating actions.

The design avoids making users traverse decorative pages before reaching the calculator.

Future onboarding expansion should remain concise and be justified by a material first-run need.

## Versioning contract

`OnboardingPolicy.CurrentVersion` starts at `1` for the current baseline.

Increase the onboarding content version only when an important new first-run explanation should be offered to people who already completed the previous onboarding version.

Do not increment it solely for:

- wording corrections;
- spacing;
- color;
- minor illustration/branding changes;
- localization fixes that do not materially change the onboarding contract.

Rules include:

- stored version `0` means current onboarding is not completed;
- negative values are normalized/rejected at the appropriate application/storage boundary;
- stored version equal to or above the current version does not trigger onboarding;
- onboarding remains hidden until settings have loaded successfully;
- skipping counts as completing the current version;
- corrupt/unsupported settings must fail safely without trapping calculator startup.

The overall application settings schema is versioned independently from onboarding content. See [SETTINGS_MIGRATION.md](SETTINGS_MIGRATION.md).

## Persistence

Onboarding completion uses the same local settings abstraction as the rest of CalcNova.

Native targets use native local settings composition; Browser/WebAssembly uses Browser-safe local storage composition.

No cloud account or remote profile is needed to remember onboarding completion.

Ordinary settings reset is intentionally not equivalent to “pretend this is a brand-new user” and therefore preserves completed onboarding state unless a dedicated product behavior explicitly says otherwise.

## Privacy

Onboarding state remains local.

The onboarding workflow does not require:

- sign-in;
- advertising identifiers;
- contacts;
- location;
- microphone/camera;
- behavioral analytics;
- donation/payment.

The onboarding surface explains the local-first posture and distinguishes offline fixed-unit behavior from optional network-enhanced currency behavior.

See [PRIVACY.md](PRIVACY.md).

## Accessibility

The completed shared source includes:

- visible text actions for Skip and Start;
- explicit automation names for the two primary actions;
- scrollability for constrained height/large text;
- wrapped explanatory text;
- readable maximum content width on large surfaces;
- suppression of background calculator/mode shortcuts while onboarding is visible;
- early access to Skip without requiring interaction with decorative content;
- focus handoff into onboarding;
- queued focus restoration to Calculator input after dismissal;
- localization of reviewed onboarding semantic text.

Runtime evidence should verify, on representative supported targets:

- keyboard-only completion/skipping;
- initial focus placement/visibility;
- screen-reader context/action announcements;
- large text/Dynamic Type behavior;
- Hindi/English layout;
- high-contrast behavior;
- narrow portrait/landscape layout;
- focus restoration timing.

Record observed results in [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md).

## Keyboard containment

While onboarding is visible, global calculator/mode shortcuts are suppressed so input cannot unintentionally activate hidden background UI.

This includes the shared mode-navigation shortcuts covered by the application source contracts.

After dismissal, focus is queued back toward Calculator input so keyboard use can resume predictably.

## Localization

Onboarding is included in the reviewed live-localization baseline.

English and Hindi semantic text is supplied through the shared localization architecture and refreshes with supported culture changes.

Canonical mathematical syntax, shortcuts, URLs, and technical identifiers are not translated in ways that would change their meaning.

See [LOCALIZATION.md](LOCALIZATION.md) and [LIVE_LOCALIZATION.md](LIVE_LOCALIZATION.md).

## Testing and validation

Onboarding coverage protects scenarios such as:

- pre-settings-load state;
- first-run/new-user state;
- settings-load failure;
- Complete;
- Skip;
- invalid stored onboarding version;
- persistence behavior;
- shell attachment;
- accessible action bindings;
- shortcut suppression;
- focus behavior;
- reviewed localization.

The integrated source gate is:

```bash
python tools/release_preflight.py
```

Compiled/headless coverage runs through the normal .NET test path documented in [TESTING.md](TESTING.md) and [UI_AUTOMATION.md](UI_AUTOMATION.md).

Target-runtime checks remain independent evidence and use:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

## Runtime validation checklist

For a release/runtime evidence pass, check as applicable:

- clean first launch on native storage;
- clean first launch on Browser storage;
- Complete persistence across restart;
- Skip persistence across restart;
- ordinary settings reset does not unexpectedly re-trigger onboarding;
- deliberate `CurrentVersion` increment re-triggers the new content boundary;
- settings-load failure leaves Calculator usable;
- background shortcuts remain suppressed while the overlay is visible;
- keyboard/screen-reader/large-text/high-contrast behavior;
- English/Hindi layout;
- compact portrait/landscape layout;
- predictable focus restoration after dismissal;
- no onboarding surface blocks calculation after dismissal.

Unchecked runtime evidence does not mean the 2.8.03 source implementation is missing; it records only what has or has not been observed in that environment.

## 2.8.03 classification

- versioned local onboarding state: **COMPLETE**;
- shared onboarding visual surface: **COMPLETE**;
- Complete/Skip persistence: **COMPLETE**;
- keyboard shortcut containment: **COMPLETE**;
- focus entry/restoration source behavior: **COMPLETE**;
- English/Hindi reviewed live localization: **COMPLETE**;
- target-platform runtime/accessibility evidence: recorded independently.

Future onboarding changes are maintenance or optional content improvements rather than missing 2.8.03 requirements.
