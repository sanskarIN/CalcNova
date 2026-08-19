namespace CalcNova.App.Infrastructure;

public enum AdaptiveLayoutKind
{
    Compact,
    Medium,
    Expanded
}

public readonly record struct AdaptiveLayoutProfile(
    AdaptiveLayoutKind Kind,
    double ShellMargin,
    double CompactControlPadding,
    double TabMinimumWidth,
    bool AllowHorizontalModeScrolling)
{
    public const double CompactMaximumWidth = 599;
    public const double MediumMaximumWidth = 979;

    public bool IsCompact => Kind == AdaptiveLayoutKind.Compact;

    public bool IsMedium => Kind == AdaptiveLayoutKind.Medium;

    public bool IsExpanded => Kind == AdaptiveLayoutKind.Expanded;

    public string StyleClass => Kind switch
    {
        AdaptiveLayoutKind.Compact => "compact",
        AdaptiveLayoutKind.Medium => "medium",
        _ => "expanded"
    };

    public static AdaptiveLayoutProfile ForWidth(double width)
    {
        var normalizedWidth = double.IsFinite(width) && width > 0
            ? width
            : CompactMaximumWidth;

        if (normalizedWidth <= CompactMaximumWidth)
        {
            return new AdaptiveLayoutProfile(
                AdaptiveLayoutKind.Compact,
                ShellMargin: 8,
                CompactControlPadding: 8,
                TabMinimumWidth: 44,
                AllowHorizontalModeScrolling: true);
        }

        if (normalizedWidth <= MediumMaximumWidth)
        {
            return new AdaptiveLayoutProfile(
                AdaptiveLayoutKind.Medium,
                ShellMargin: 12,
                CompactControlPadding: 10,
                TabMinimumWidth: 48,
                AllowHorizontalModeScrolling: false);
        }

        return new AdaptiveLayoutProfile(
            AdaptiveLayoutKind.Expanded,
            ShellMargin: 16,
            CompactControlPadding: 12,
            TabMinimumWidth: 56,
            AllowHorizontalModeScrolling: false);
    }
}
