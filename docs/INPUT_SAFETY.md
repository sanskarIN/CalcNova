# Input Safety and Expression Import

CalcNova evaluates calculator expressions with project-owned tokenizer/parser/evaluator code. It does not pass calculator input to a shell, scripting engine, reflection-based evaluator, dynamic compiler, or general-purpose code execution path.

## Direct calculator input

Interactive expression text is parsed by `CalcNova.Core`. Evaluation is subject to configured limits such as maximum expression length, factorial input, and integer exponent size.

## Imported expression text

Text coming from a clipboard, share target, file import, or other external source must go through `ExpressionTextSanitizer` before being assigned to the calculator expression model.

The sanitizer:

- trims surrounding whitespace;
- removes a leading calculator-style `=` marker;
- normalizes `×` and `·` to `*`;
- normalizes `÷` to `/`;
- normalizes common Unicode minus/dash forms to `-`;
- normalizes `π` to `pi` and `τ` to `tau`;
- normalizes superscript 2/3 to `^2`/`^3`;
- converts line/tab separators to safe spacing;
- rejects unsupported control characters;
- rejects unsupported punctuation/symbol characters;
- enforces the same maximum-expression-length policy used by the evaluator.

Sanitization is not a replacement for parsing. Sanitized text is still parsed and evaluated through the normal CalcNova expression engine, so unknown identifiers, invalid function calls, domain errors, and workload limits continue to be handled by the evaluator.

## View-model integration

`CalculatorViewModel.ImportExpression(...)` and `ImportExpressionCommand` use the sanitizer and preserve the existing expression when imported text is rejected. The user-facing status message receives the rejection reason.

`PasteAsync` obtains text through `IClipboardService` and sends it through the same sanitizer path. It does not auto-evaluate the pasted expression. `CopyResultAsync` writes only a valid displayed result.

## Clipboard architecture

Clipboard access is isolated behind `CalcNova.Platform.Clipboard.IClipboardService`. The shared app provides `AvaloniaClipboardService`, which is attached to the active Avalonia top-level clipboard while `MainView` is attached to the visual tree and detached when the view leaves it.

Current Desktop, Browser/WebAssembly, Android, and iOS compositions inject this shared adapter.

Privacy and safety requirements are enforced by design:

1. ordinary calculator use does not require clipboard access;
2. clipboard text is read only after the user invokes paste;
3. clipboard content is not evaluated automatically;
4. imported text always uses `CalculatorViewModel.ImportExpression`;
5. clipboard content is not intentionally logged or sent to telemetry;
6. evaluator expression-length limits still apply;
7. unavailable/cancelled/rejected clipboard operations produce user-facing status messages.

## Testing expectations

Regression coverage includes:

- calculator glyph normalization;
- multiline whitespace normalization;
- unsupported symbols/control characters;
- maximum-length enforcement;
- view-model preservation of the previous expression on rejected import;
- successful evaluation after sanitized import;
- sanitized clipboard paste through a fake clipboard service;
- unsafe clipboard rejection;
- valid result copy;
- unavailable clipboard reporting.

## Validation rule

Source and regression tests exist, but target-specific clipboard behavior must still be compiled and exercised on each supported platform before release readiness is marked PASS. In the current continuation environment, local .NET restore/build/test execution is **NOT RUN** because the required SDK is unavailable.
