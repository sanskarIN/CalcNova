#!/usr/bin/env python3
"""Validate deterministic CalcNova focus-visibility style contracts."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

BASE_FOCUS_SELECTORS = (
    'Button:focus',
    'TextBox:focus',
    'ComboBox:focus',
    'CheckBox:focus',
    'TabItem:focus',
    'ListBoxItem:focus',
)

HIGH_CONTRAST_FOCUS_SELECTORS = tuple(
    f'UserControl.high-contrast {selector}' for selector in BASE_FOCUS_SELECTORS
)


def validate(root: Path) -> list[str]:
    app_xaml = root / 'src' / 'CalcNova.App' / 'App.axaml'
    if not app_xaml.is_file():
        return [f'Missing application styles: {app_xaml}']

    source = app_xaml.read_text(encoding='utf-8')
    failures: list[str] = []

    for selector in BASE_FOCUS_SELECTORS:
        marker = f'<Style Selector="{selector}">'
        if marker not in source:
            failures.append(f'Missing visible focus style: {selector}')

    for selector in HIGH_CONTRAST_FOCUS_SELECTORS:
        marker = f'<Style Selector="{selector}">'
        if marker not in source:
            failures.append(f'Missing high-contrast focus style: {selector}')

    if source.count('<Setter Property="BorderThickness" Value="3" />') < len(BASE_FOCUS_SELECTORS):
        failures.append('Base focus styles must retain a 3-DIP border emphasis.')

    if source.count('<Setter Property="BorderThickness" Value="4" />') < len(HIGH_CONTRAST_FOCUS_SELECTORS):
        failures.append('High-contrast focus styles must retain a 4-DIP border emphasis.')

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description='Validate CalcNova focus visibility contracts.')
    parser.add_argument('root', nargs='?', default='.', help='Repository root')
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print('Focus visibility validation failed:', file=sys.stderr)
        for failure in failures:
            print(f'- {failure}', file=sys.stderr)
        return 1

    print('Validated shared and high-contrast keyboard focus visibility styles.')
    return 0


if __name__ == '__main__':
    raise SystemExit(main())
