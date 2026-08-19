# CalcNova Onboarding

CalcNova onboarding is optional, local-first, skippable, and versioned. It helps a new user understand important capabilities without requiring an account or repeatedly interrupting returning users.

## Current implementation state

The persistence, view-model, and shared visual foundation are implemented:

- `AppSettings.CompletedOnboardingVersion` stores the locally completed onboarding version;
- `OnboardingPolicy.CurrentVersion` defines the current first-run content generation;
- invalid negative stored versions are normalized to zero;
- `SettingsViewModel.IsLoaded` prevents the first-run surface from appearing before persisted settings have loaded successfully;
- `SettingsViewModel.ShouldShowOnboarding` reports whether the loaded settings still require the current onboarding version;
- failed/corrupt settings loading does not raise the onboarding surface and therefore does not trap startup behind first-run UI;
- `CompleteOnboardingCommand` and `SkipOnboardingCommand` both persist the current completion boundary;
- completion and skipping use the existing settings repository abstraction, so they follow each platform's local settings storage path;
- resetting ordinary settings preserves onboarding completion instead of unexpectedly forcing the first-run experience again;
- `OnboardingOverlay.axaml` provides the shared first-run visual surface used by the shared application shell;
- the overlay introduces major modes, keyboard/touch behavior, local-first storage, optional currency networking, and the no-account requirement;
- keyboard guidance now documents Ctrl+PageUp/PageDown cycling plus Ctrl+Home/End first/last mode navigation;
- both **Skip** and **Start calculating** have explicit accessible names;
- the main shell suppresses calculator/mode keyboard shortcuts while onboarding is visible so background actions do not fire through the overlay;
- unit tests cover pre-load state, new-user state, failed settings loading, completion, skip, and invalid-version behavior;
- a source-level CI validator protects persistence, deferred display, visual bindings, accessible actions, and shell attachment.

The shared onboarding UI is therefore implemented, but it is **not yet target-platform validated**. Platform release readiness still requires actual Desktop, Browser, Android, and iOS build/runtime checks plus keyboard/screen-reader/large-text testing.

## Current visual flow

The initial implementation intentionally uses one concise, scrollable surface instead of several forced pages. It includes:

1. **Welcome to CalcNova** — fast, precise, private positioning.
2. **Calculate your way** — overview of the standard/scientific and advanced modes.
3. **Keyboard and touch friendly** — cyclic mode navigation, first/last mode shortcuts, and hardware number-pad guidance.
4. **Local-first by default** — local history/preferences, offline physical units, and optional network-enhanced currency behavior.
5. **Ready** — immediate **Skip** and **Start calculating** actions.

This avoids making users traverse multiple pages just to reach the calculator. Future onboarding expansion should remain similarly concise and justified by a real first-run need.

## Versioning contract

`OnboardingPolicy.CurrentVersion` starts at `1`.

Increase the version only when a materially important new first-run explanation should be offered to users who previously completed onboarding. Do not increment it for wording, spacing, color, or minor illustration changes.

Rules:

- stored version `0` means onboarding has not been completed;
- stored versions below zero are treated as `0` at the application policy boundary;
- storage repositories reject negative versions as malformed persisted state;
- stored versions equal to or above the current version do not trigger onboarding;
- onboarding is hidden until settings load succeeds;
- skipping counts as completing the current version;
- unsupported or corrupt settings must fail safely without blocking calculator startup.

The settings container itself is also schema-versioned independently of onboarding content. See [SETTINGS_MIGRATION.md](SETTINGS_MIGRATION.md).

## Privacy

Onboarding state remains local. It does not require sign-in, telemetry, advertising identifiers, contacts, location, or unrelated permissions.

The onboarding surface does not include donation/payment prompts and does not make optional support actions look required for use.

## Accessibility

The implemented shared surface includes:

- visible text actions for both completion and skipping;
- explicit automation names on the two first-run actions;
- scrollability for constrained heights;
- wrapped explanatory text;
- a maximum content width to preserve readable line lengths on larger displays;
- suppression of background calculator/mode keyboard shortcuts while the overlay is active;
- immediate access to Skip without traversing decorative content;
- shell focus handoff to the onboarding action surface and queued restoration to Calculator after dismissal.

Target-platform validation is still required for:

- keyboard-only completion and skipping;
- initial focus placement and visible focus;
- screen-reader page context and action announcements;
- text scaling and long localized strings;
- reduced-motion/high-contrast interaction with future visual refinements;
- narrow portrait and landscape layouts;
- focus restoration timing after dismissal.

Record actual results in [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md). No essential explanation should exist only in an illustration or animation.

## Localization

The current onboarding copy is English source text. English and Hindi semantic catalogs now exist for the current shared key set, but onboarding has not yet migrated to localized bindings. It must migrate to semantic localization keys as the shared XAML localization binding path is expanded.

New language packs require review for mathematical, privacy, and accessibility terminology before being listed as supported.

## Testing still required

Before onboarding is considered release-ready:

- verify first launch on clean native storage;
- verify first launch on Browser storage;
- verify Complete persists across restart;
- verify Skip persists across restart;
- verify ordinary settings reset does not re-trigger onboarding;
- verify a deliberate `CurrentVersion` increment re-triggers the new flow;
- verify settings-load failure leaves the calculator usable;
- verify keyboard, screen reader, large text, high contrast, and compact layouts;
- verify background calculator shortcuts do not activate while onboarding is visible;
- verify Ctrl+Home/End and Ctrl+PageUp/PageDown remain suppressed behind onboarding;
- verify focus returns predictably after dismissal;
- verify no onboarding surface blocks calculation after dismissal.
