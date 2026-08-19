using System.Numerics;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Programmer;

namespace CalcNova.App.ViewModels;

public sealed class ProgrammerViewModel : ViewModelBase
{
    private string _input = "42";
    private string _operand = "15";
    private int _inputBase = 10;
    private int _wordSize = 64;
    private int _shiftCount = 1;
    private bool _signed = true;
    private string _binary = "101010";
    private string _octal = "52";
    private string _decimal = "42";
    private string _hexadecimal = "2A";
    private string _bitPattern = BitwiseCalculator.ToBitString(new BigInteger(42), 64);
    private string _interpretedValue = "42";
    private string _lastOperation = string.Empty;
    private string _errorMessage = string.Empty;
    private IReadOnlyList<BitCellViewModel> _bits = Array.Empty<BitCellViewModel>();

    public ProgrammerViewModel()
    {
        ConvertCommand = new RelayCommand(_ => Convert());
        ToggleBitCommand = new RelayCommand(ToggleBit);
        AndCommand = new RelayCommand(_ => ApplyBinaryOperation("AND", BitwiseCalculator.And));
        OrCommand = new RelayCommand(_ => ApplyBinaryOperation("OR", BitwiseCalculator.Or));
        XorCommand = new RelayCommand(_ => ApplyBinaryOperation("XOR", BitwiseCalculator.Xor));
        NotCommand = new RelayCommand(_ => ApplyUnaryOperation("NOT", BitwiseCalculator.Not));
        ShiftLeftCommand = new RelayCommand(_ => ApplyShift("SHL", BitwiseCalculator.ShiftLeft));
        LogicalShiftRightCommand = new RelayCommand(_ => ApplyShift("LSHR", BitwiseCalculator.LogicalShiftRight));
        ArithmeticShiftRightCommand = new RelayCommand(_ => ApplyShift("ASHR", BitwiseCalculator.ArithmeticShiftRight));
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

    public string Operand
    {
        get => _operand;
        set => SetField(ref _operand, value ?? string.Empty);
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

    public int ShiftCount
    {
        get => _shiftCount;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Shift count cannot be negative.");
            }

            SetField(ref _shiftCount, value);
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

    public string LastOperation
    {
        get => _lastOperation;
        private set => SetField(ref _lastOperation, value);
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

    public ICommand AndCommand { get; }

    public ICommand OrCommand { get; }

    public ICommand XorCommand { get; }

    public ICommand NotCommand { get; }

    public ICommand ShiftLeftCommand { get; }

    public ICommand LogicalShiftRightCommand { get; }

    public ICommand ArithmeticShiftRightCommand { get; }

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
            ApplyResult(toggled, $"Toggle b{bitIndex}");
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or OverflowException)
        {
            ErrorMessage = exception.Message;
        }
    }

    private void ApplyBinaryOperation(
        string label,
        Func<BigInteger, BigInteger, int, BigInteger> operation)
    {
        try
        {
            var left = RadixConverter.Parse(Input, InputBase);
            var right = RadixConverter.Parse(Operand, InputBase);
            ApplyResult(operation(left, right, WordSize), label);
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or OverflowException)
        {
            ErrorMessage = exception.Message;
        }
    }

    private void ApplyUnaryOperation(
        string label,
        Func<BigInteger, int, BigInteger> operation)
    {
        try
        {
            var value = RadixConverter.Parse(Input, InputBase);
            ApplyResult(operation(value, WordSize), label);
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or OverflowException)
        {
            ErrorMessage = exception.Message;
        }
    }

    private void ApplyShift(
        string label,
        Func<BigInteger, int, int, BigInteger> operation)
    {
        try
        {
            if (ShiftCount > WordSize)
            {
                throw new ArgumentOutOfRangeException(nameof(ShiftCount), ShiftCount, "Shift count cannot exceed the selected word size.");
            }

            var value = RadixConverter.Parse(Input, InputBase);
            ApplyResult(operation(value, ShiftCount, WordSize), $"{label} {ShiftCount}");
        }
        catch (Exception exception) when (exception is FormatException or ArgumentException or OverflowException)
        {
            ErrorMessage = exception.Message;
        }
    }

    private void ApplyResult(BigInteger result, string operationLabel)
    {
        var unsigned = BitwiseCalculator.ToUnsigned(result, WordSize);
        var signed = BitwiseCalculator.ToSigned(result, WordSize);
        var inputValue = InputBase == 10 && Signed ? signed : unsigned;
        Input = RadixConverter.Format(inputValue, InputBase);
        UpdateRepresentations(result);
        LastOperation = operationLabel;
        ErrorMessage = string.Empty;
    }

    private void UpdateRepresentations(BigInteger value)
    {
        var unsigned = BitwiseCalculator.ToUnsigned(value, WordSize);
        var signed = BitwiseCalculator.ToSigned(value, WordSize);
        Binary = RadixConverter.Format(unsigned, 2);
        Octal = RadixConverter.Format(unsigned, 8);
        Decimal = RadixConverter.Format(Signed ? signed : unsigned, 10);
        Hexadecimal = RadixConverter.Format(unsigned, 16);
        BitPattern = BitwiseCalculator.ToBitString(unsigned, WordSize);
        InterpretedValue = (Signed ? signed : unsigned).ToString();
        UpdateBits(unsigned);
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
