using System.Numerics;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Programmer;

namespace CalcNova.App.ViewModels;

public sealed class ProgrammerViewModel : ViewModelBase
{
    private string _input = "42";
    private int _inputBase = 10;
    private int _outputBase = 16;
    private int _wordSize = 64;
    private bool _signed = true;
    private string _binary = "101010";
    private string _octal = "52";
    private string _decimal = "42";
    private string _hexadecimal = "2A";
    private string _customOutput = "2A";
    private string _bitPattern = BitwiseCalculator.ToBitString(new BigInteger(42), 64);
    private IReadOnlyList<ProgrammerBitCell> _bitCells = CreateBitCells(new BigInteger(42), 64);
    private string _interpretedValue = "42";
    private string _errorMessage = string.Empty;

    public ProgrammerViewModel()
    {
        ConvertCommand = new RelayCommand(_ => Convert());
        ToggleBitCommand = new RelayCommand(ToggleBit);
    }

    public IReadOnlyList<int> Bases { get; } = Enumerable.Range(2, 35).ToArray();

    public IReadOnlyList<int> WordSizes { get; } = [8, 16, 32, 64, 128];

    public string Input
    {
        get => _input;
        set => SetField(ref _input, value ?? string.Empty);
    }

    public int InputBase
    {
        get => _inputBase;
        set => SetField(ref _inputBase, value);
    }

    public int OutputBase
    {
        get => _outputBase;
        set
        {
            if (SetField(ref _outputBase, value))
            {
                UpdateCustomOutput();
            }
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

    public string CustomOutput
    {
        get => _customOutput;
        private set => SetField(ref _customOutput, value);
    }

    public string BitPattern
    {
        get => _bitPattern;
        private set => SetField(ref _bitPattern, value);
    }

    public IReadOnlyList<ProgrammerBitCell> BitCells
    {
        get => _bitCells;
        private set => SetField(ref _bitCells, value);
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

    public ICommand ConvertCommand { get; }

    public ICommand ToggleBitCommand { get; }

    public void Convert()
    {
        try
        {
            var value = RadixConverter.Parse(Input, InputBase);
            UpdateOutputs(value);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or OverflowException)
        {
            ErrorMessage = exception.Message;
        }
    }

    private void ToggleBit(object? parameter)
    {
        if (parameter is not ProgrammerBitCell cell || cell.BitIndex < 0 || cell.BitIndex >= WordSize)
        {
            return;
        }

        try
        {
            var current = RadixConverter.Parse(Input, InputBase);
            var unsigned = BitwiseCalculator.ToUnsigned(current, WordSize);
            var toggled = unsigned ^ (BigInteger.One << cell.BitIndex);
            var inputValue = Signed ? BitwiseCalculator.ToSigned(toggled, WordSize) : toggled;
            Input = RadixConverter.Format(inputValue, InputBase);
            UpdateOutputs(toggled);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or OverflowException)
        {
            ErrorMessage = exception.Message;
        }
    }

    private void UpdateOutputs(BigInteger value)
    {
        Binary = RadixConverter.Format(value, 2);
        Octal = RadixConverter.Format(value, 8);
        Decimal = RadixConverter.Format(value, 10);
        Hexadecimal = RadixConverter.Format(value, 16);
        CustomOutput = RadixConverter.Format(value, OutputBase);
        BitPattern = BitwiseCalculator.ToBitString(value, WordSize);
        BitCells = CreateBitCells(value, WordSize);
        var interpreted = Signed
            ? BitwiseCalculator.ToSigned(value, WordSize)
            : BitwiseCalculator.ToUnsigned(value, WordSize);
        InterpretedValue = interpreted.ToString();
    }

    private void UpdateCustomOutput()
    {
        try
        {
            var value = RadixConverter.Parse(Input, InputBase);
            CustomOutput = RadixConverter.Format(value, OutputBase);
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or OverflowException)
        {
            ErrorMessage = exception.Message;
        }
    }

    private static IReadOnlyList<ProgrammerBitCell> CreateBitCells(BigInteger value, int wordSize)
    {
        var unsigned = BitwiseCalculator.ToUnsigned(value, wordSize);
        var cells = new ProgrammerBitCell[wordSize];
        for (var visualIndex = 0; visualIndex < wordSize; visualIndex++)
        {
            var bitIndex = wordSize - visualIndex - 1;
            cells[visualIndex] = new ProgrammerBitCell(
                bitIndex,
                ((unsigned >> bitIndex) & BigInteger.One) == BigInteger.One);
        }

        return cells;
    }
}
