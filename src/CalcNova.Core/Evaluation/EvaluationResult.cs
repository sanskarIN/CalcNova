using CalcNova.Core.Errors;
using CalcNova.Core.Numerics;

namespace CalcNova.Core.Evaluation;

public sealed record EvaluationResult
{
    private EvaluationResult(bool success, NumberValue value, CalculationErrorCode errorCode, string? errorMessage)
    {
        Success = success;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool Success { get; }

    public NumberValue Value { get; }

    public CalculationErrorCode ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static EvaluationResult FromValue(NumberValue value) =>
        new(true, value, CalculationErrorCode.None, null);

    public static EvaluationResult FromError(CalculationErrorCode code, string message) =>
        new(false, NumberValue.Zero, code, message);
}
