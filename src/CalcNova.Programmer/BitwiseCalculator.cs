using System.Numerics;

namespace CalcNova.Programmer;

public static class BitwiseCalculator
{
    public static BigInteger And(BigInteger left, BigInteger right, int wordSize) =>
        ToUnsigned(left & right, wordSize);

    public static BigInteger Or(BigInteger left, BigInteger right, int wordSize) =>
        ToUnsigned(left | right, wordSize);

    public static BigInteger Xor(BigInteger left, BigInteger right, int wordSize) =>
        ToUnsigned(left ^ right, wordSize);

    public static BigInteger Not(BigInteger value, int wordSize) =>
        ToUnsigned(~value, wordSize);

    public static BigInteger ShiftLeft(BigInteger value, int shift, int wordSize)
    {
        ValidateShift(shift);
        return ToUnsigned(value << shift, wordSize);
    }

    public static BigInteger LogicalShiftRight(BigInteger value, int shift, int wordSize)
    {
        ValidateShift(shift);
        return ToUnsigned(value, wordSize) >> shift;
    }

    public static BigInteger ArithmeticShiftRight(BigInteger value, int shift, int wordSize)
    {
        ValidateShift(shift);
        return ToSigned(value, wordSize) >> shift;
    }

    public static BigInteger ToUnsigned(BigInteger value, int wordSize)
    {
        var mask = CreateMask(wordSize);
        return value & mask;
    }

    public static BigInteger ToSigned(BigInteger value, int wordSize)
    {
        ValidateWordSize(wordSize);
        var unsigned = ToUnsigned(value, wordSize);
        var signBit = BigInteger.One << (wordSize - 1);
        if ((unsigned & signBit) == BigInteger.Zero)
        {
            return unsigned;
        }

        return unsigned - (BigInteger.One << wordSize);
    }

    public static string ToBitString(BigInteger value, int wordSize)
    {
        ValidateWordSize(wordSize);
        var unsigned = ToUnsigned(value, wordSize);
        var characters = new char[wordSize];
        for (var index = 0; index < wordSize; index++)
        {
            var bit = wordSize - index - 1;
            characters[index] = ((unsigned >> bit) & BigInteger.One) == BigInteger.One ? '1' : '0';
        }

        return new string(characters);
    }

    private static BigInteger CreateMask(int wordSize)
    {
        ValidateWordSize(wordSize);
        return (BigInteger.One << wordSize) - BigInteger.One;
    }

    private static void ValidateWordSize(int wordSize)
    {
        if (wordSize is < 1 or > 4096)
        {
            throw new ArgumentOutOfRangeException(nameof(wordSize), wordSize, "Word size must be between 1 and 4096 bits.");
        }
    }

    private static void ValidateShift(int shift)
    {
        if (shift < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(shift), shift, "Shift count cannot be negative.");
        }
    }
}
