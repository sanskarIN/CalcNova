using System.Numerics;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Programmer;

namespace CalcNova.App.ViewModels;

public sealed class ProgrammerViewModel : ViewModelBase
{
    private string _input = "42";
    private int _inputBase = 10;
    private int _wordSize = 64;
    private bool _signed = true;
    private string _binary = "101010";
    private string _octal = "52";
    private string _decimal = "42";
    private string _hexadecimal = "2A";
    private string _bitPattern = BitwiseCalculator.ToBitString(new BigInteger(42), 64);
    private string _interpretedValue = "42";
    private string _errorMessage = string.Empty;
    private IReadOnlyList<BitCellViewModel> _bits = Array.Empty<BitCellViewModel>();

    public ProgrammerViewModel()
    {
        ConvertCommand = new RelayCommand(_ => Convert());
        ToggleBitCommand = new RelayCommand(ToggleBit);
        Convert();
    }

    public IReadOnlyList<int> CommonBases { get; } = [2, 8, 10, 16, 36];

    public IReadOnlyList<int> SupportedBases { get; } = Enumerable.Range(2, 35).ToArray();

    public IReadOnlyList<int> WordSizes { get; } = [8, 16, 32, 64, 128];

    public string Input
    {
        get => _input;
        set => SetField(ref _input, value ?? string.Empty);
    }

    public int InputBase
    {
        get => _inputBase;
        set
        {
            if (value is < 2 or > 36)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Input base must be between 2 and 36.");
            }

            SetField(ref _inputBase, value);
        }
    }

    public int WordSize
    {
        get => _wordSize;
        set
        {
            if (SetField(ref _wordSize, value))
            {
                Convert();
            }
        }
    }

    public bool Signed
    {
        get => _signed;
        set
        {
            if (SetField(ref _signed, value))
            {
                Convert();
            }
        }
    }

    public string Binary
    {
        get => _binary;
        private set => SetField(ref _binary, value);
    }

    public string Octal
    {
        get => _octal;
        private set => SetField(ref _octal, value);
    }

    public string Decimal
    {
        get => _decimal;
        private set => SetField(ref _decimal, value);
    }

    public string Hexadecimal
    {
        get => _hexadecimal;
        private set => SetField(ref _hexadecimal, value);
    }

    public string BitPattern
    {
        get => _bitPattern;
        private set => SetField(ref _bitPattern, value);
    }

    public string InterpretedValue
    {
        get => _interpretedValue;
        private set => SetField(ref _interpretedValue, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public IReadOnlyList<BitCellViewModel> Bits
    {
        get => _bits;
        private set => SetField(ref _bits, value);
    }

    public ICommand ConvertCommand { get; }

    public ICommand ToggleBitCommand { get; }

    public void ToggleBit(int bitIndex)
    {
        ToggleBit((object?)bitIndex);
    }

    private void Convert()
    {
        try
        {
            var value = RadixConverter.Parse(Input, InputBase);
            UpdateRepresentations(value);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or OverflowException)
        {
            ErrorMessage = exception.Message;
        }
    }

    private void ToggleBit(object? parameter)
    {
        if (!TryGetBitIndex(parameter, out var bitIndex))
        {
            ErrorMessage = "A valid bit index is required.";
            return;
        }

        try
        {
            var value = RadixConverter.Parse(Input, InputBase);
            var toggled = BitwiseCalculator.ToggleBit(value, bitIndex, WordSize);
            Input = RadixConverter.Format(toggled, InputBase);
            UpdateRepresentations(toggled);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or OverflowException)
        {
            ErrorMessage = exception.Message;
        }
    }

    private void UpdateRepresentations(BigInteger value)
    {
        Binary = RadixConverter.Format(value, 2);
        Octal = RadixConverter.Format(value, 8);
        Decimal = RadixConverter.Format(value, 10);
        Hexadecimal = RadixConverter.Format(value, 16);
        BitPattern = BitwiseCalculator.ToBitString(value, WordSize);
        var interpreted = Signed
            ? BitwiseCalculator.ToSigned(value, WordSize)
            : BitwiseCalculator.ToUnsigned(value, WordSize);
        InterpretedValue = interpreted.ToString();
        UpdateBits(value);
    }

    private void UpdateBits(BigInteger value)
    {
        if (Bits.Count != WordSize)
        {
            Bits = Enumerable.Range(0, WordSize)
                .Select(offset => WordSize - offset - 1)
                .Select(index => new BitCellViewModel(
                    index,
                    BitwiseCalculator.IsBitSet(value, index, WordSize),
                    ToggleBit))
                .ToArray();
            return;
        }

        foreach (var bit in Bits)
        {
            bit.Update(BitwiseCalculator.IsBitSet(value, bit.Index, WordSize));
        }
    }

    private static bool TryGetBitIndex(object? parameter, out int bitIndex)
    {
        if (parameter is int integer)
        {
            bitIndex = integer;
            return true;
        }

        if (parameter is string text && int.TryParse(text, out bitIndex))
        {
            return true;
        }

        bitIndex = default;
        return false;
    }
}
