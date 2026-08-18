namespace CalcNova.Core.Errors;

public enum CalculationErrorCode
{
    None = 0,
    EmptyExpression,
    SyntaxError,
    DivideByZero,
    DomainError,
    NumericOverflow,
    InvalidArgument,
    UnsupportedFunction,
    InputTooLong,
    WorkloadLimitExceeded
}
