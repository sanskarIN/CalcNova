using Xunit;

namespace CalcNova.Matrices.Tests;

public sealed class MatrixTests
{
    [Fact]
    public void Multiply_KnownMatrices_ReturnsExpectedProduct()
    {
        var left = Matrix.FromRows([1d, 2d], [3d, 4d]);
        var right = Matrix.FromRows([5d, 6d], [7d, 8d]);

        var result = left.Multiply(right);

        Assert.Equal(19d, result[0, 0], 12);
        Assert.Equal(22d, result[0, 1], 12);
        Assert.Equal(43d, result[1, 0], 12);
        Assert.Equal(50d, result[1, 1], 12);
    }

    [Fact]
    public void Determinant_KnownMatrix_ReturnsExpectedValue()
    {
        var matrix = Matrix.FromRows([4d, 7d], [2d, 6d]);

        Assert.Equal(10d, matrix.Determinant(), 12);
    }

    [Fact]
    public void Inverse_MultipliedByOriginal_ApproximatesIdentity()
    {
        var matrix = Matrix.FromRows([4d, 7d], [2d, 6d]);

        var product = matrix.Multiply(matrix.Inverse());

        Assert.Equal(1d, product[0, 0], 10);
        Assert.Equal(0d, product[0, 1], 10);
        Assert.Equal(0d, product[1, 0], 10);
        Assert.Equal(1d, product[1, 1], 10);
    }

    [Fact]
    public void Rank_DetectsDependentRows()
    {
        var matrix = Matrix.FromRows([1d, 2d, 3d], [2d, 4d, 6d], [1d, 1d, 1d]);

        Assert.Equal(2, matrix.Rank());
    }

    [Fact]
    public void Solve_KnownLinearSystem_ReturnsExpectedSolution()
    {
        var matrix = Matrix.FromRows([2d, 1d], [5d, 7d]);

        var solution = matrix.Solve([11d, 13d]);

        Assert.Equal(64d / 9d, solution[0], 10);
        Assert.Equal(-29d / 9d, solution[1], 10);
    }

    [Fact]
    public void Inverse_SingularMatrix_Throws()
    {
        var matrix = Matrix.FromRows([1d, 2d], [2d, 4d]);

        Assert.Throws<InvalidOperationException>(() => matrix.Inverse());
    }

    [Fact]
    public void VectorOperations_ReturnExpectedValues()
    {
        Assert.Equal(5d, VectorMath.Magnitude([3d, 4d]), 12);
        Assert.Equal(32d, VectorMath.Dot([1d, 2d, 3d], [4d, 5d, 6d]), 12);
        Assert.Equal([0d, 0d, 1d], VectorMath.Cross([1d, 0d, 0d], [0d, 1d, 0d]));
    }
}
