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

A future clipboard service must call this import path rather than assigning clipboard text directly to `Expression`.

## Clipboard integration requirements

When platform clipboard support is wired:

1. keep the clipboard interface in a platform abstraction rather than `CalcNova.Core`;
2. never require clipboard permission for ordinary calculator use;
3. import text through `CalculatorViewModel.ImportExpression`;
4. do not auto-evaluate pasted/imported expressions unless the user explicitly requests evaluation;
5. avoid reading clipboard content in the background;
6. avoid telemetry/logging of clipboard content;
7. impose the existing expression length limit before evaluation;
8. provide an accessible error message when text cannot be imported.

## Testing expectations

Regression tests should cover:

- calculator glyph normalization;
- multiline whitespace normalization;
- unsupported symbols/control characters;
- maximum-length enforcement;
- view-model preservation of the previous expression on rejected import;
- successful evaluation after a sanitized import.

## Validation rule

Source/tests existing does not mean a platform clipboard path has been validated. Clipboard integration must be compiled and exercised on the relevant target before it is marked PASS.
