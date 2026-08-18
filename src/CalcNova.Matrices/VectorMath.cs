namespace CalcNova.Matrices;

public static class VectorMath
{
    public static double Magnitude(IReadOnlyList<double> vector)
    {
        ValidateVector(vector, nameof(vector));

        var scale = 0d;
        var sumOfSquares = 1d;
        foreach (var value in vector)
        {
            var absolute = Math.Abs(value);
            if (absolute == 0d)
            {
                continue;
            }

            if (scale < absolute)
            {
                var ratio = scale / absolute;
                sumOfSquares = 1d + (sumOfSquares * ratio * ratio);
                scale = absolute;
            }
            else
            {
                var ratio = absolute / scale;
                sumOfSquares += ratio * ratio;
            }
        }

        return scale == 0d ? 0d : scale * Math.Sqrt(sumOfSquares);
    }

    public static double Dot(IReadOnlyList<double> left, IReadOnlyList<double> right)
    {
        ValidateVector(left, nameof(left));
        ValidateVector(right, nameof(right));
        if (left.Count != right.Count)
        {
            throw new ArgumentException("Dot product vectors must have the same dimension.");
        }

        var sum = 0d;
        var compensation = 0d;
        for (var index = 0; index < left.Count; index++)
        {
            var product = left[index] * right[index];
            if (!double.IsFinite(product))
            {
                throw new OverflowException("The vector dot product exceeded the supported floating-point range.");
            }

            var adjusted = product - compensation;
            var next = sum + adjusted;
            compensation = (next - sum) - adjusted;
            sum = next;
        }

        if (!double.IsFinite(sum))
        {
            throw new OverflowException("The vector dot product exceeded the supported floating-point range.");
        }

        return sum == 0d ? 0d : sum;
    }

    public static double[] Cross(IReadOnlyList<double> left, IReadOnlyList<double> right)
    {
        ValidateVector(left, nameof(left));
        ValidateVector(right, nameof(right));
        if (left.Count != 3 || right.Count != 3)
        {
            throw new ArgumentException("Cross product is defined here only for three-dimensional vectors.");
        }

        var result = new[]
        {
            (left[1] * right[2]) - (left[2] * right[1]),
            (left[2] * right[0]) - (left[0] * right[2]),
            (left[0] * right[1]) - (left[1] * right[0])
        };

        if (result.Any(value => !double.IsFinite(value)))
        {
            throw new OverflowException("The vector cross product exceeded the supported floating-point range.");
        }

        return result;
    }

    private static void ValidateVector(IReadOnlyList<double> vector, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(vector);
        if (vector.Count == 0)
        {
            throw new ArgumentException("A vector must contain at least one value.", parameterName);
        }

        for (var index = 0; index < vector.Count; index++)
        {
            if (!double.IsFinite(vector[index]))
            {
                throw new ArgumentException("Vector values must be finite.", parameterName);
            }
        }
    }
}
