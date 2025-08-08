using MatrixLib;

namespace MatrixLibTests
{
    [TestClass]
    public sealed class CoreTests
    {
        Random rand = new();

        [TestMethod]
        public void GeneralMatrixCreation()
        {
            int height = rand.Next(1, 100);
            int width = rand.Next(1, 100);
            int nonzero = rand.Next(1,1 + height);

            RealMatrix empty = RealMatrix.Zeros(height,width);
            RealMatrix diagonal = RealMatrix.Eye(height,width);
            RealMatrix elementary = RealMatrix.Elementary(height,nonzero);

            for(int r = 1; r <= empty.Height; ++r)
            {
                for(int c = 1; c <= empty.Width; ++c)
                {
                    // Zero

                    Assert.AreEqual(0, empty[r, c]);
                    
                    // Eye

                    if(r == c)
                    {
                        Assert.AreEqual(1, diagonal[r, c]);
                    }
                    else
                    {
                        Assert.AreEqual(0, diagonal[r, c]);
                    }

                    // Elementary

                    if(c == 1)
                    {
                        if(r == nonzero)
                        {
                            Assert.AreEqual(1, elementary[r, c]);
                        }
                        else
                        {
                            Assert.AreEqual(0, elementary[r, c]);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public void CopiedMatrixCreation()
        {
            int dimensionOne = rand.Next(1, 100);
            int dimensionTwo = rand.Next(1, 100);

            double[,] data = new double[dimensionOne, dimensionTwo];

            for(int i = 0; i < dimensionOne; ++i)
            {
                for (int j = 0; j < dimensionTwo; ++j)
                {
                    data[i, j] = rand.Next(-100, 100);
                }
            }

            RealMatrix copied = RealMatrix.From(data);

            for (int r = 1; r <= copied.Height; ++r)
            {
                for (int c = 1; c <= copied.Width; ++c)
                {
                    Assert.AreEqual(data[r - 1, c - 1], copied[r, c]);
                }
            }

            RealMatrix transposed = copied.Transpose();

            RealMatrix[] saved = { copied, transposed };

            RealMatrix.SaveMatricesTo(saved, "coretests.mat");

            RealMatrix[] loaded = RealMatrix.From("coretests.mat");

            for (int m = 0; m < loaded.Length; ++m)
            {
                for (int r = 1; r <= loaded[m].Height; ++r)
                {
                    for (int c = 1; c <= loaded[m].Width; ++c)
                    {
                        Assert.AreEqual(saved[m][r,c], loaded[m][r,c]);
                    }
                }
            }
        }

        [TestMethod]
        public void ExtractingAndModifying()
        {
            double[,] data = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };

            double[,] correctRow = { { 4, 5, 6 } };

            double[,] correstColumn = { { 3 }, { 6 }, { 9 } };


            RealMatrix original = RealMatrix.From(data);

            RealMatrix secondRow = original.Row(2);

            RealMatrix thirdColumn = original.Column(3);

            RealMatrix correctSecondRow = RealMatrix.From(correctRow);

            RealMatrix correctThirdColumn = RealMatrix.From(correstColumn);

            Assert.IsTrue(correctSecondRow == secondRow);
            Assert.IsTrue(correctThirdColumn == thirdColumn);

            RealMatrix modified = original.Modify((a, b) => a * b, 3);

            RealMatrix inserted = RealMatrix.Eye(2);

            RealMatrix mangled = modified.Insert(2, 2, inserted);

            double[,] expected = { { 12, 1, 0 }, { 21, 0, 1 } };

            RealMatrix expectedMatrix = RealMatrix.From(expected);

            RealMatrix extracted = mangled.SubMatrix(2, 3, 1, 3);

            Assert.IsTrue(expectedMatrix == extracted);
        }
    }
}
