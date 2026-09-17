using CalcNova.App.ViewModels;
using CalcNova.Platform.Haptics;
using Xunit;

namespace CalcNova.App.Tests;

/// <summary>
/// Covers the haptics preference end to end through the calculator.
/// </summary>
/// <remarks>
/// The Haptics setting was persisted, localized and shown as a checkbox long before anything
/// implemented it, so these assert the two things that make it real: that input actually asks
/// the device for feedback, and that turning the preference off stops it.
/// </remarks>
public sealed class CalculatorHapticFeedbackTests
{
    [Fact]
    public void Append_AcceptedToken_RequestsSelectionFeedback()
    {
        var haptics = new RecordingHapticFeedbackService();
        var calculator = CreateCalculator(haptics);

        calculator.AppendCommand.Execute("7");

        Assert.Equal([HapticFeedbackKind.Selection], haptics.Requests);
        Assert.Equal("7", calculator.Expression);
    }

    [Fact]
    public void Append_RejectedToken_RequestsWarningFeedback()
    {
        var haptics = new RecordingHapticFeedbackService();
        var calculator = CreateCalculator(haptics);

        // A token longer than the expression budget cannot be applied, so the edit is refused
        // and the user gets the rejection buzz rather than the acceptance one.
        calculator.AppendCommand.Execute(new string('7', 5000));

        Assert.Equal([HapticFeedbackKind.Warning], haptics.Requests);
        Assert.Empty(calculator.Expression);
    }

    [Fact]
    public async Task Evaluate_Succeeds_RequestsSuccessFeedback()
    {
        var haptics = new RecordingHapticFeedbackService();
        var calculator = CreateCalculator(haptics);
        calculator.Expression = "2 + 2";
        haptics.Requests.Clear();

        await calculator.EvaluateAsync();

        Assert.Equal("4", calculator.Result);
        Assert.Equal([HapticFeedbackKind.Success], haptics.Requests);
    }

    [Fact]
    public async Task Evaluate_Fails_RequestsWarningFeedback()
    {
        var haptics = new RecordingHapticFeedbackService();
        var calculator = CreateCalculator(haptics);
        calculator.Expression = "2 +";
        haptics.Requests.Clear();

        await calculator.EvaluateAsync();

        Assert.Equal("Error", calculator.Result);
        Assert.Equal([HapticFeedbackKind.Warning], haptics.Requests);
    }

    [Fact]
    public void ClearAndBackspace_RequestSelectionFeedback()
    {
        var haptics = new RecordingHapticFeedbackService();
        var calculator = CreateCalculator(haptics);
        calculator.Expression = "12";
        calculator.UpdateSelection(2, 2);
        haptics.Requests.Clear();

        calculator.Backspace();
        calculator.Clear();

        Assert.Equal([HapticFeedbackKind.Selection, HapticFeedbackKind.Selection], haptics.Requests);
    }

    [Fact]
    public void Backspace_WithNothingToRemove_RequestsNoFeedback()
    {
        var haptics = new RecordingHapticFeedbackService();
        var calculator = CreateCalculator(haptics);

        calculator.Backspace();

        Assert.Empty(haptics.Requests);
    }

    [Fact]
    public async Task PreferenceOff_SuppressesEveryRequest()
    {
        var haptics = new RecordingHapticFeedbackService();
        var calculator = CreateCalculator(haptics, hapticsEnabled: false);

        calculator.AppendCommand.Execute("9");
        calculator.Backspace();
        calculator.Clear();
        calculator.Expression = "1 + 1";
        await calculator.EvaluateAsync();

        Assert.Empty(haptics.Requests);
    }

    [Fact]
    public async Task NoHapticService_LeavesTheCalculatorFullyFunctional()
    {
        // Desktop and the browser supply no haptic engine. The calculator must not branch on
        // that, so the default has to absorb every request silently.
        var calculator = new CalculatorViewModel();

        calculator.AppendCommand.Execute("6");
        calculator.AppendCommand.Execute("*");
        calculator.AppendCommand.Execute("7");
        await calculator.EvaluateAsync();

        Assert.Equal("42", calculator.Result);
    }

    [Fact]
    public void NullHapticFeedbackService_ReportsUnavailableAndDoesNothing()
    {
        var service = NullHapticFeedbackService.Instance;

        Assert.False(service.IsAvailable);
        Assert.Same(NullHapticFeedbackService.Instance, service);

        foreach (var kind in Enum.GetValues<HapticFeedbackKind>())
        {
            service.Perform(kind);
        }
    }

    private static CalculatorViewModel CreateCalculator(
        IHapticFeedbackService haptics,
        bool hapticsEnabled = true) =>
        new(
            hapticFeedbackService: haptics,
            hapticsEnabledProvider: () => hapticsEnabled);

    private sealed class RecordingHapticFeedbackService : IHapticFeedbackService
    {
        public List<HapticFeedbackKind> Requests { get; } = [];

        public bool IsAvailable => true;

        public void Perform(HapticFeedbackKind kind) => Requests.Add(kind);
    }
}
