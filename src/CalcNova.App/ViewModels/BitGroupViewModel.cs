namespace CalcNova.App.ViewModels;

public sealed class BitGroupViewModel
{
    public BitGroupViewModel(int byteIndex, IReadOnlyList<BitCellViewModel> bits)
    {
        if (byteIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(byteIndex));
        }

        ArgumentNullException.ThrowIfNull(bits);
        if (bits.Count is < 1 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(bits), "A bit group must contain between one and eight bits.");
        }

        ByteIndex = byteIndex;
        Bits = bits;
    }

    public int ByteIndex { get; }

    public string Label => $"Byte {ByteIndex}";

    public IReadOnlyList<BitCellViewModel> Bits { get; }
}
