namespace CalcNova.DateTimeTools;

public sealed record DateDifferenceResult(
    int SignedDays,
    int AbsoluteDays,
    int WholeWeeks,
    int RemainingDays);
