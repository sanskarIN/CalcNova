namespace CalcNova.App.ViewModels;

public sealed record ProgrammerBitCell(int BitIndex, bool IsSet)
{
    public string Label => BitIndex.ToString();

    public string Value => IsSet ? "1" : "0";
}
