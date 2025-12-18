using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JA.LinearAlgebra;

[TestClass]
public class MatrixTests
{
    [TestMethod]
    public void FromArray2_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        double[,] elements = new double[,] {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 } 
            };

        // Act
        var matrix = Matrix.FromArray2(elements);

        // Assert
        for (int i = 0; i < elements.GetLength(0); i++)
        {
            for (int j = 0; j < elements.GetLength(1); j++)
            {
                Assert.AreEqual(elements[i, j], matrix.Elements[i][j]);
            }
        }
        
    }

    [TestMethod]
    public void ToJaggedArray_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var matrix = new Matrix(3,3, 
            1d,2d,3d,
            4d,5d,6d,
            7d,8d,9d);

        // Act
        double[][] expected = new double[3][] {
            new double[3] { 1, 2, 3 },
            new double[3] { 4, 5, 6 },
            new double[3] { 7, 8, 9 } 
            };

        double[][] elements = matrix.Elements;

        // Assert
        CollectionAssert.AreEqual(expected, elements);
    }

    [TestMethod]
    public void ToArray2_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var matrix = new Matrix(3,3, 1d,2d,3d,4d,5d,6d,7d,8d,9d);            

        // Act
        double[,] expected = new double[,] {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 } 
            };
        double[,] actual = matrix.ToArray2();

        // Assert
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ToString_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var matrix = new Matrix(3,3, 1d,2d,3d,4d,5d,6d,7d,8d,9d);            
        // Act
        string result = matrix.ToString();
        string expected = $"[1,2,3|4,5,6|7,8,9]";

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void GetRow_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var matrix = new Matrix(3, 3, 1d, 2d, 3d, 4d, 5d, 6d, 7d, 8d, 9d);

        // Act
        var byRows = matrix.ToJaggedArray();

        // Assert
        for (int row_index = 0; row_index < matrix.Rows; row_index++)
        {
            var result = matrix.GetRow(row_index); 
            var expected = byRows[row_index]; 
            CollectionAssert.AreEqual(expected, result);
        }
    }

    [TestMethod]
    public void SetRow_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var matrix = new Matrix(3, 3);
        double[][] byRows = new double[3][] {
            new double[3] { 1, 2, 3 },
            new double[3] { 4, 5, 6 },
            new double[3] { 7, 8, 9 } 
            };

        // Act
        for (int row_index = 0; row_index < matrix.Rows; row_index++)
        {
            var row = byRows[row_index];
            matrix.SetRow(row_index,row); 
        }

        var expectedMatrix = new Matrix(byRows);

        // Assert
        Assert.AreEqual(expectedMatrix, matrix);
    }

    [TestMethod]
    public void Transpose_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        double[,] elements = new double[,] {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 } 
            };
        double[,] expected = new double[,] {
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 3, 6, 9 } 
            };

        // Act
        var matrix = Matrix.FromArray2(elements);
        var transposed = matrix.Transpose();
        var actual = transposed.ToArray2();

        // Assert
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Solve_StateUnderTest_ExpectedBehavior()
    {
        //tex: $$\begin{bmatrix}1 & 115/74 & 0.6\\
        //\text{-}0.7 & 1.6 & \text{-}0.4\\
        //\text{-}0.4 & 0.2 & 4
        //\end{bmatrix}\begin{bmatrix}\mbox{-}34/37\\
        //1\\
        //45/74
        //\end{bmatrix}=\begin{bmatrix}1\\
        //2\\
        //3
        //\end{bmatrix}$$

        // Arrange
        var matrix = new Matrix(3,3,
            1d, 115d/74, 0.6,
            -0.7, 1.6, -0.4,
            -0.4, 0.2, 4d);
        Vector b = Vector.FromValues(1d, 2d, 3d);
        double maxResidual = 1e-10;
        Vector expected = Vector.FromValues(-34d/37, 1d, 45d/74);

        // Act
        Vector actual = matrix.Solve(b, out var residual);

        // Assert
        Assert.IsTrue(expected.ApproxEquals(actual));
        Assert.IsLessThan(maxResidual, residual);
    }

    [TestMethod]
    public void Solve_StateUnderTest_ExpectedBehavior1()
    {
        //tex: $$\begin{bmatrix}1 & \frac{115}{74} & \tfrac{3}{5}\\
        //\text{-}\tfrac{7}{10} & \tfrac{8}{5} & \text{-}\tfrac{2}{5}\\
        //\text{-}\tfrac{2}{5} & \tfrac{1}{5} & 4
        //\end{bmatrix}\begin{bmatrix}\mbox{-}\tfrac{34}{37} & \text{-}\tfrac{12784}{21053} & \tfrac{18834}{21053}\\
        //1 & \tfrac{931}{569} & \tfrac{654}{569}\\
        //\tfrac{45}{74} & \tfrac{4525}{42106} & \tfrac{11200}{21053}
        //\end{bmatrix}=\begin{bmatrix}1 & 2 & 3\\
        //2 & 3 & 1\\
        //3 & 1 & 2
        //\end{bmatrix}$$
        
        // Arrange:
        var matrix = new Matrix(3,3,
            1d, 115d/74, 0.6,
            -0.7, 1.6, -0.4,
            -0.4, 0.2, 4d);

        Matrix B = new Matrix(3,3,
            1d, 2d, 3d,
            2d, 3d, 1d,
            3d, 1d, 2d);

        double maxResidual = 1e-10;
        Matrix expected = Matrix.FromArray2(new double[,] {
            { -34d/37, -12784d/21053    , 18834d/21053  },
            { 1d     , 931d/569         , 654d/569      },
            { 45d/74 , 4525d/42106      , 11200d/21053  }
            });

        // Act
        Matrix actual = matrix.Solve(B,out var residual);

        // Assert
        Assert.IsTrue(expected.ApproxEquals(actual));
        Assert.IsLessThan(maxResidual, residual);
    }

    [TestMethod]
    public void Equals_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        double[][] fromJaggedArray = new double[3][] {
            new double[3] { 1, 2, 3 },
            new double[3] { 4, 5, 6 },
            new double[3] { 7, 8, 9 } 
            };
        double[,] fromArray2 = new double[,] {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 9 } 
            };

        Matrix lhs = new Matrix(fromJaggedArray);
        Matrix rhs = Matrix.FromArray2(fromArray2);

        // Act
        var result = lhs.Equals(rhs);

        // Assert
        Assert.IsTrue(result);
    }

}
