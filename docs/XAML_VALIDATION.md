# Avalonia XAML Validation

CalcNova uses two layers of markup validation.

## Lightweight XML preflight

`tools/validate_xaml.py` scans repository `.axaml` files outside generated/build folders and parses each file as XML.

This catches structural failures such as:

- truncated or unclosed markup;
- mismatched XML elements;
- invalid XML entities;
- duplicate XML attributes;
- malformed attribute quoting.

Run it from the repository root with:

```bash
python tools/validate_xaml.py .
```

The `XAML Validate` GitHub Actions workflow runs this check when Avalonia markup or the validator changes.

## Avalonia compilation remains authoritative

XML well-formedness does **not** prove that an Avalonia binding, property, control type, resource, converter, or event handler is valid. Full validation still requires the normal .NET/Avalonia restore/build/test pipeline.

The lightweight check is therefore a fast preflight gate, not a replacement for:

```bash
dotnet restore CalcNova.slnx
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

## Validation reporting rule

If the required .NET SDK/workload is unavailable, record Avalonia compilation as `NOT RUN`. A passing XML preflight must never be described as a passing Avalonia build.
