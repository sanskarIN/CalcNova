namespace CalcNova.Core.Errors;

public sealed class CalculationException : Exception
{
    public CalculationException(CalculationErrorCode code, string message)
        : base(message)
    {
        Code = code;
    }

    public CalculationErrorCode Code { get; }
}
