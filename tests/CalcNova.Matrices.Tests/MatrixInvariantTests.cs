using Xunit;

namespace CalcNova.Matrices.Tests;

/// <summary>
/// Invariant coverage for the linear-algebra routines.
/// </summary>
/// <remarks>
/// The hand-picked cases in <see cref="MatrixTests"/> pin a handful of known answers.
/// These exercise the same routines across a deterministic spread of shapes and values,
/// asserting the identities the algorithms must preserve rather than individual results,
/// so an elimination or pivoting mistake that happens to survive one 2x2 example is still
/// caught. The generator is a fixed linear congruential sequence, so a failure reproduces
/// exactly.
/// </remarks>
public sealed class MatrixInvariantTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void Inverse_TimesOriginal_ApproximatesIdentity(int size)
    {
        var random = new DeterministicValues(seed: 20260915u + (uint)size);

        for (var trial = 0; trial < 12; trial++)
        {
            var matrix = CreateWellConditioned(random, size);
            var product = matrix.Multiply(matrix.Inverse());

            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    Assert.Equal(row == column ? 1d : 0d, product[row, column], 9);
                }
            }
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void Solve_ReturnsAVectorThatSatisfiesTheOriginalSystem(int size)
    {
        var random = new DeterministicValues(seed: 777u + (uint)size);

        for (var trial = 0; trial < 12; trial++)
        {
            var matrix = CreateWellConditioned(random, size);
            var rightHandSide = new double[size];
            for (var row = 0; row < size; row++)
            {
                rightHandSide[row] = random.NextSigned();
            }

            var solution = matrix.Solve(rightHandSide);

            for (var row = 0; row < size; row++)
            {
                var residual = 0d;
                for (var column = 0; column < size; column++)
                {
                    residual += matrix[row, column] * solution[column];
                }

                Assert.Equal(rightHandSide[row], residual, 9);
            }
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Determinant_IsMultiplicativeOverProducts(int size)
    {
        var random = new DeterministicValues(seed: 31337u + (uint)size);

        for (var trial = 0; trial < 12; trial++)
        {
            var left = CreateWellConditioned(random, size);
            var right = CreateWellConditioned(random, size);

            var productDeterminant = left.Multiply(right).Determinant();
            var expected = left.Determinant() * right.Determinant();

            // Determinants of well-conditioned matrices grow quickly with size, so compare
            // against a relative budget instead of a fixed number of decimal places.
            Assert.True(
                Math.Abs(productDeterminant - expected) <= 1e-9 * Math.Max(1d, Math.Abs(expected)),
                $"det(AB)={productDeterminant} but det(A)*det(B)={expected}");
        }
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public void Determinant_OfTranspose_MatchesTheOriginal(int size)
    {
        var random = new DeterministicValues(seed: 4242u + (uint)size);

        for (var trial = 0; trial < 12; trial++)
        {
            var matrix = CreateWellConditioned(random, size);
            var expected = matrix.Determinant();

            Assert.True(
                Math.Abs(matrix.Transpose().Determinant() - expected) <= 1e-9 * Math.Max(1d, Math.Abs(expected)),
                $"det(A^T) disagreed with det(A)={expected}");
        }
    }

    [Fact]
    public void Transpose_AppliedTwice_ReturnsTheOriginalValues()
    {
        var random = new DeterministicValues(seed: 99u);

        for (var rows = 1; rows <= 4; rows++)
        {
            for (var columns = 1; columns <= 4; columns++)
            {
                var matrix = CreateArbitrary(random, rows, columns);
                var roundTripped = matrix.Transpose().Transpose();

                Assert.Equal(matrix.Rows, roundTripped.Rows);
                Assert.Equal(matrix.Columns, roundTripped.Columns);

                for (var row = 0; row < rows; row++)
                {
                    for (var column = 0; column < columns; column++)
                    {
                        Assert.Equal(matrix[row, column], roundTripped[row, column]);
                    }
                }
            }
        }
    }

    [Fact]
    public void Multiply_ByIdentity_LeavesTheMatrixUnchanged()
    {
        var random = new DeterministicValues(seed: 5150u);

        for (var size = 1; size <= 5; size++)
        {
            var matrix = CreateArbitrary(random, size, size);
            var product = matrix.Multiply(Matrix.Identity(size));

            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    Assert.Equal(matrix[row, column], product[row, column], 12);
                }
            }
        }
    }

    [Fact]
    public void Rank_NeverExceedsTheSmallerDimension()
    {
        var random = new DeterministicValues(seed: 606u);

        for (var rows = 1; rows <= 5; rows++)
        {
            for (var columns = 1; columns <= 5; columns++)
            {
                var matrix = CreateArbitrary(random, rows, columns);

                Assert.InRange(matrix.Rank(), 0, Math.Min(rows, columns));
            }
        }
    }

    [Fact]
    public void Rank_DropsWhenARowIsAScaledCopyOfAnother()
    {
        var random = new DeterministicValues(seed: 1234u);

        for (var size = 2; size <= 5; size++)
        {
            var values = new double[size, size];
            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    values[row, column] = random.NextSigned();
                }
            }

            // Make the last row three times the first, which removes exactly one
            // independent direction however the remaining rows happen to fall.
            for (var column = 0; column < size; column++)
            {
                values[size - 1, column] = 3d * values[0, column];
            }

            var matrix = new Matrix(values);

            Assert.True(matrix.Rank() < size, $"A {size}x{size} matrix with a duplicated direction reported full rank.");
            Assert.Equal(0d, matrix.Determinant(), 9);
        }
    }

    [Fact]
    public void Inverse_OfIdentity_IsIdentity()
    {
        for (var size = 1; size <= 6; size++)
        {
            var inverse = Matrix.Identity(size).Inverse();

            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    Assert.Equal(row == column ? 1d : 0d, inverse[row, column], 12);
                }
            }
        }
    }

    /// <summary>
    /// Builds a diagonally dominant matrix, which is invertible and well conditioned, so a
    /// failure means the routine is wrong rather than that the draw was near-singular.
    /// </summary>
    private static Matrix CreateWellConditioned(DeterministicValues random, int size)
    {
        var values = new double[size, size];
        for (var row = 0; row < size; row++)
        {
            var offDiagonalMagnitude = 0d;
            for (var column = 0; column < size; column++)
            {
                if (row == column)
                {
                    continue;
                }

                values[row, column] = random.NextSigned();
                offDiagonalMagnitude += Math.Abs(values[row, column]);
            }

            values[row, row] = offDiagonalMagnitude + 1d + Math.Abs(random.NextSigned());
        }

        return new Matrix(values);
    }

    private static Matrix CreateArbitrary(DeterministicValues random, int rows, int columns)
    {
        var values = new double[rows, columns];
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                values[row, column] = random.NextSigned();
            }
        }

        return new Matrix(values);
    }

    /// <summary>A fixed linear congruential generator, so every trial reproduces exactly.</summary>
    private sealed class DeterministicValues(uint seed)
    {
        private uint _state = seed == 0u ? 1u : seed;

        /// <summary>Returns a value in [-5, 5) that is never close enough to zero to be degenerate.</summary>
        public double NextSigned()
        {
            _state = (1664525u * _state) + 1013904223u;
            var unit = ((_state >> 8) & 0xFFFFFF) / (double)0x1000000;
            var value = (unit * 10d) - 5d;
            return Math.Abs(value) < 0.25d ? value + 0.5d : value;
        }
    }
}
