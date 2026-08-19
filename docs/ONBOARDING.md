# CalcNova Onboarding

CalcNova onboarding is optional, local-first, skippable, and versioned. It must help a new user understand important capabilities without blocking calculator use or requiring an account.

## Current implementation state

The persistence and view-model foundation is implemented:

- `AppSettings.CompletedOnboardingVersion` stores the locally completed onboarding version;
- `OnboardingPolicy.CurrentVersion` defines the current first-run content generation;
- invalid negative stored versions are normalized to zero;
- `SettingsViewModel.ShouldShowOnboarding` reports whether the current onboarding version still needs to be shown;
- `CompleteOnboardingCommand` and `SkipOnboardingCommand` both persist the current completion boundary;
- completion and skipping use the existing settings repository abstraction, so they follow each platform's local settings storage path;
- resetting ordinary settings preserves onboarding completion instead of unexpectedly forcing the first-run experience again;
- unit tests cover new-user, completion, skip, and invalid-version behavior;
- a source-level CI validator protects the persistence and command contracts.

A dedicated visual onboarding surface is still pending. The project must not claim that onboarding UI is complete until that surface has been implemented and validated on supported targets.

## Product requirements

The eventual visual flow should remain short and dismissible. Suggested pages are:

1. **Welcome to CalcNova** — explain fast, private, local-first calculation.
2. **Choose a mode** — briefly introduce Calculator, Programmer, Converter, Graphing, Statistics, Equations, Matrices, Date/Duration, Currency, and History.
3. **Keyboard and touch** — show essential input, `Ctrl+PageUp/PageDown` mode cycling on supported keyboard targets, and touch-friendly interaction.
4. **Privacy and history** — explain local history/settings and optional network-backed currency-rate behavior without implying an account is required.
5. **Ready** — enter the calculator immediately.

Every page must provide a visible **Skip** action. Completing or skipping the current flow records the same version boundary so the user is not repeatedly interrupted.

## Versioning contract

`OnboardingPolicy.CurrentVersion` starts at `1`.

Increase the version only when a materially important new first-run explanation should be offered to users who previously completed onboarding. Do not increment it for wording, spacing, color, or minor illustration changes.

Rules:

- stored version `0` means onboarding has not been completed;
- stored versions below zero are treated as `0`;
- stored versions equal to or above the current version do not trigger onboarding;
- skipping counts as completing the current version;
- unsupported or corrupt settings must fail safely without blocking calculator startup.

## Privacy

Onboarding state must remain local. It must not require sign-in, telemetry, advertising identifiers, contacts, location, or any unrelated permission.

The onboarding flow must not include deceptive consent patterns or make optional support/donation actions look required for use.

## Accessibility

The eventual UI must support:

- keyboard-only completion and skipping;
- visible focus;
- screen-reader names and page context;
- text scaling and long localized strings;
- reduced-motion preferences;
- high-contrast behavior;
- narrow portrait and landscape layouts;
- immediate access to Skip without traversing decorative content.

No essential explanation should exist only in an illustration or animation.

## Localization

Onboarding prose must use semantic localization keys once the shared XAML localization binding path is established. New language packs require review for mathematical, privacy, and accessibility terminology before being listed as supported.

## Testing still required

Before the visual flow is considered release-ready:

- verify first launch on clean native storage;
- verify first launch on Browser storage;
- verify Complete persists across restart;
- verify Skip persists across restart;
- verify ordinary settings reset does not re-trigger onboarding;
- verify a deliberate `CurrentVersion` increment re-triggers the new flow;
- verify keyboard, screen reader, large text, and compact layouts;
- verify no onboarding surface blocks calculation after dismissal.
