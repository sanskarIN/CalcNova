namespace CalcNova.Matrices;

public sealed class Matrix
{
    private readonly double[,] _values;

    public Matrix(double[,] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (values.GetLength(0) == 0 || values.GetLength(1) == 0)
        {
            throw new ArgumentException("A matrix must contain at least one row and one column.", nameof(values));
        }

        _values = (double[,])values.Clone();
        Rows = _values.GetLength(0);
        Columns = _values.GetLength(1);

        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < Columns; column++)
            {
                if (!double.IsFinite(_values[row, column]))
                {
                    throw new ArgumentException("Matrix values must be finite.", nameof(values));
                }
            }
        }
    }

    public int Rows { get; }

    public int Columns { get; }

    public double this[int row, int column] => _values[row, column];

    public static Matrix FromRows(params double[][] rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        if (rows.Length == 0)
        {
            throw new ArgumentException("A matrix must contain at least one row.", nameof(rows));
        }

        if (rows[0] is null || rows[0].Length == 0)
        {
            throw new ArgumentException("Matrix rows must contain at least one value.", nameof(rows));
        }

        var columnCount = rows[0].Length;
        var values = new double[rows.Length, columnCount];
        for (var row = 0; row < rows.Length; row++)
        {
            if (rows[row] is null || rows[row].Length != columnCount)
            {
                throw new ArgumentException("All matrix rows must have the same length.", nameof(rows));
            }

            for (var column = 0; column < columnCount; column++)
            {
                values[row, column] = rows[row][column];
            }
        }

        return new Matrix(values);
    }

    public static Matrix Identity(int size)
    {
        if (size < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size), size, "Identity matrix size must be positive.");
        }

        var values = new double[size, size];
        for (var index = 0; index < size; index++)
        {
            values[index, index] = 1d;
        }

        return new Matrix(values);
    }

    public Matrix Add(Matrix other)
    {
        ArgumentNullException.ThrowIfNull(other);
        EnsureSameDimensions(other);

        var result = new double[Rows, Columns];
        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < Columns; column++)
            {
                result[row, column] = EnsureFinite(_values[row, column] + other._values[row, column]);
            }
        }

        return new Matrix(result);
    }

    public Matrix Subtract(Matrix other)
    {
        ArgumentNullException.ThrowIfNull(other);
        EnsureSameDimensions(other);

        var result = new double[Rows, Columns];
        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < Columns; column++)
            {
                result[row, column] = EnsureFinite(_values[row, column] - other._values[row, column]);
            }
        }

        return new Matrix(result);
    }

    public Matrix Multiply(Matrix other)
    {
        ArgumentNullException.ThrowIfNull(other);
        if (Columns != other.Rows)
        {
            throw new InvalidOperationException($"Cannot multiply a {Rows}x{Columns} matrix by a {other.Rows}x{other.Columns} matrix.");
        }

        var result = new double[Rows, other.Columns];
        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < other.Columns; column++)
            {
                var sum = 0d;
                for (var inner = 0; inner < Columns; inner++)
                {
                    sum = EnsureFinite(sum + EnsureFinite(_values[row, inner] * other._values[inner, column]));
                }

                result[row, column] = sum;
            }
        }

        return new Matrix(result);
    }

    public Matrix Multiply(double scalar)
    {
        if (!double.IsFinite(scalar))
        {
            throw new ArgumentOutOfRangeException(nameof(scalar), scalar, "Matrix scalar must be finite.");
        }

        var result = new double[Rows, Columns];
        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < Columns; column++)
            {
                result[row, column] = EnsureFinite(_values[row, column] * scalar);
            }
        }

        return new Matrix(result);
    }

    public Matrix Transpose()
    {
        var result = new double[Columns, Rows];
        for (var row = 0; row < Rows; row++)
        {
            for (var column = 0; column < Columns; column++)
            {
                result[column, row] = _values[row, column];
            }
        }

        return new Matrix(result);
    }

    public double Determinant(double tolerance = 1e-12d)
    {
        EnsureSquare();
        ValidateTolerance(tolerance);

        var data = ToArray();
        var determinant = 1d;
        var sign = 1d;

        for (var pivotColumn = 0; pivotColumn < Columns; pivotColumn++)
        {
            var pivotRow = FindPivotRow(data, pivotColumn, pivotColumn, Rows);
            var pivotMagnitude = Math.Abs(data[pivotRow, pivotColumn]);
            if (pivotMagnitude <= tolerance)
            {
                return 0d;
            }

            if (pivotRow != pivotColumn)
            {
                SwapRows(data, pivotRow, pivotColumn, Columns);
                sign = -sign;
            }

            var pivot = data[pivotColumn, pivotColumn];
            determinant = EnsureFinite(determinant * pivot);

            for (var row = pivotColumn + 1; row < Rows; row++)
            {
                var factor = data[row, pivotColumn] / pivot;
                data[row, pivotColumn] = 0d;
                for (var column = pivotColumn + 1; column < Columns; column++)
                {
                    data[row, column] = EnsureFinite(data[row, column] - (factor * data[pivotColumn, column]));
                }
            }
        }

        return EnsureFinite(sign * determinant);
    }

    public int Rank(double tolerance = 1e-12d)
    {
        ValidateTolerance(tolerance);
        var data = ToArray();
        var pivotRow = 0;
        var rank = 0;

        for (var column = 0; column < Columns && pivotRow < Rows; column++)
        {
            var bestRow = FindPivotRow(data, pivotRow, column, Rows);
            if (Math.Abs(data[bestRow, column]) <= tolerance)
            {
                continue;
            }

            SwapRows(data, bestRow, pivotRow, Columns);
            var pivot = data[pivotRow, column];

            for (var row = pivotRow + 1; row < Rows; row++)
            {
                var factor = data[row, column] / pivot;
                data[row, column] = 0d;
                for (var remainingColumn = column + 1; remainingColumn < Columns; remainingColumn++)
                {
                    data[row, remainingColumn] = EnsureFinite(
                        data[row, remainingColumn] - (factor * data[pivotRow, remainingColumn]));
                }
            }

            rank++;
            pivotRow++;
        }

        return rank;
    }

    public Matrix Inverse(double tolerance = 1e-12d)
    {
        EnsureSquare();
        ValidateTolerance(tolerance);

        var size = Rows;
        var augmented = new double[size, size * 2];
        for (var row = 0; row < size; row++)
        {
            for (var column = 0; column < size; column++)
            {
                augmented[row, column] = _values[row, column];
                augmented[row, size + column] = row == column ? 1d : 0d;
            }
        }

        GaussJordan(augmented, size, size * 2, tolerance);

        var inverse = new double[size, size];
        for (var row = 0; row < size; row++)
        {
            for (var column = 0; column < size; column++)
            {
                inverse[row, column] = augmented[row, size + column];
            }
        }

        return new Matrix(inverse);
    }

    public double[] Solve(IReadOnlyList<double> rightHandSide, double tolerance = 1e-12d)
    {
        ArgumentNullException.ThrowIfNull(rightHandSide);
        EnsureSquare();
        ValidateTolerance(tolerance);

        if (rightHandSide.Count != Rows)
        {
            throw new ArgumentException("The right-hand side length must match the matrix row count.", nameof(rightHandSide));
        }

        var size = Rows;
        var augmented = new double[size, size + 1];
        for (var row = 0; row < size; row++)
        {
            for (var column = 0; column < size; column++)
            {
                augmented[row, column] = _values[row, column];
            }

            var value = rightHandSide[row];
            if (!double.IsFinite(value))
            {
                throw new ArgumentException("The right-hand side may only contain finite values.", nameof(rightHandSide));
            }

            augmented[row, size] = value;
        }

        GaussJordan(augmented, size, size + 1, tolerance);

        var result = new double[size];
        for (var row = 0; row < size; row++)
        {
            result[row] = augmented[row, size];
        }

        return result;
    }

    public double[,] ToArray() => (double[,])_values.Clone();

    private static void GaussJordan(double[,] data, int pivotCount, int columnCount, double tolerance)
    {
        for (var pivotColumn = 0; pivotColumn < pivotCount; pivotColumn++)
        {
            var pivotRow = FindPivotRow(data, pivotColumn, pivotColumn, pivotCount);
            var pivot = data[pivotRow, pivotColumn];
            if (Math.Abs(pivot) <= tolerance)
            {
                throw new InvalidOperationException("The matrix is singular within the configured tolerance.");
            }

            SwapRows(data, pivotRow, pivotColumn, columnCount);
            pivot = data[pivotColumn, pivotColumn];

            for (var column = 0; column < columnCount; column++)
            {
                data[pivotColumn, column] = EnsureFinite(data[pivotColumn, column] / pivot);
            }

            for (var row = 0; row < pivotCount; row++)
            {
                if (row == pivotColumn)
                {
                    continue;
                }

                var factor = data[row, pivotColumn];
                if (Math.Abs(factor) <= tolerance)
                {
                    data[row, pivotColumn] = 0d;
                    continue;
                }

                for (var column = 0; column < columnCount; column++)
                {
                    data[row, column] = EnsureFinite(data[row, column] - (factor * data[pivotColumn, column]));
                }
            }
        }
    }

    private static int FindPivotRow(double[,] data, int startRow, int column, int rowCount)
    {
        var bestRow = startRow;
        var bestMagnitude = Math.Abs(data[startRow, column]);
        for (var row = startRow + 1; row < rowCount; row++)
        {
            var magnitude = Math.Abs(data[row, column]);
            if (magnitude > bestMagnitude)
            {
                bestMagnitude = magnitude;
                bestRow = row;
            }
        }

        return bestRow;
    }

    private static void SwapRows(double[,] data, int firstRow, int secondRow, int columnCount)
    {
        if (firstRow == secondRow)
        {
            return;
        }

        for (var column = 0; column < columnCount; column++)
        {
            (data[firstRow, column], data[secondRow, column]) = (data[secondRow, column], data[firstRow, column]);
        }
    }

    private void EnsureSameDimensions(Matrix other)
    {
        if (Rows != other.Rows || Columns != other.Columns)
        {
            throw new InvalidOperationException(
                $"Matrix dimensions must match. Left: {Rows}x{Columns}; right: {other.Rows}x{other.Columns}.");
        }
    }

    private void EnsureSquare()
    {
        if (Rows != Columns)
        {
            throw new InvalidOperationException("This operation requires a square matrix.");
        }
    }

    private static void ValidateTolerance(double tolerance)
    {
        if (!double.IsFinite(tolerance) || tolerance < 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), tolerance, "Tolerance must be finite and non-negative.");
        }
    }

    private static double EnsureFinite(double value)
    {
        if (!double.IsFinite(value))
        {
            throw new OverflowException("A matrix operation produced a non-finite value.");
        }

        return value == 0d ? 0d : value;
    }
}
