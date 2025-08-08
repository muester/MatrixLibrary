using MatrixLib;

namespace MatrixLibTests
{
    [TestClass]
    public sealed class AlgorithmTests
    {
        Random rand = new();

        [TestMethod]
        public void LUDecomposition()
        {
            int height = rand.Next(1, 100);
            int width = rand.Next(1, 100);

            int precision = rand.Next(7, 15);

            Algorithms.Precision = precision;

            RealMatrix original = RealMatrix.Zeros(height,width);

            for (int r = 1; r <= original.Height; r++)
            {
                for (int c = 1; c <= original.Width; ++c)
                {
                    original[r, c] = rand.NextDouble() * 2 - 1;
                }
            }

            (RealMatrix L, RealMatrix U, RealMatrix P) = Algorithms.LU(original);

            RealMatrix retransforemd = P.Transpose() * L * U;

            for (int r = 1; r <= original.Height; r++)
            {
                for (int c = 1; c <= original.Width; ++c)
                {
                    Assert.AreEqual(original[r, c], retransforemd[r, c], Math.Pow(10, -Algorithms.Precision));
                }
            }
        }

        [TestMethod]
        public void QRDecomposition()
        {
            int height = rand.Next(1, 100);
            int width = rand.Next(1, 100);

            int precision = rand.Next(7, 15);

            Algorithms.Precision = precision;

            RealMatrix original = RealMatrix.Zeros(height, width);

            for (int r = 1; r <= original.Height; r++)
            {
                for (int c = 1; c <= original.Width; ++c)
                {
                    original[r, c] = rand.NextDouble() * 2 - 1;
                }
            }

            (RealMatrix Q, RealMatrix R) = Algorithms.QR(original);

            RealMatrix retransforemd = Q * R;

            for (int r = 1; r <= original.Height; r++)
            {
                for (int c = 1; c <= original.Width; ++c)
                {
                    Assert.AreEqual(original[r, c], retransforemd[r, c], Math.Pow(10,1-Algorithms.Precision));
                }
            }
        }


        [TestMethod]
        public void InverseAndLinSys()
        {
            int side = rand.Next(1, 100);

            RealMatrix original = RealMatrix.Zeros(side);
            RealMatrix rightSide = RealMatrix.Zeros(side, 1);

            Algorithms.Precision = rand.Next(7, 16);

            for (int r = 1; r <= original.Height; r++)
            {
                for(int c = 1; c <= original.Width; ++c)
                {
                    original[r,c] = rand.NextDouble() * 2 - 1;

                    if(c == 1)
                    {
                        rightSide[r,c] = rand.NextDouble();
                    }
                }
            }

            RealMatrix inverse = Algorithms.Inverse(original);

            RealMatrix Identity = original * inverse;

            for (int r = 1; r <= original.Height; r++)
            {
                for (int c = 1; c <= original.Width; ++c)
                {
                    if(r == c)
                    {
                        Assert.AreEqual(1, Identity[r, c], Math.Pow(10, 3-Algorithms.Precision));
                    }
                    else
                    {
                        Assert.AreEqual(0, Identity[r, c], Math.Pow(10, 3-Algorithms.Precision));
                    }
                }
            }

            RealMatrix x = Algorithms.LinSys(original, rightSide);

            RealMatrix result = original * x;

            for (int r = 1; r <= original.Height; r++)
            {
                Assert.AreEqual(result[r, 1], rightSide[r, 1], Math.Pow(10, 3 - Algorithms.Precision));
            }

        }

        [TestMethod]
        public void Eigenvalues()
        {
            double[,] dataOne = { { 0, 1, 0, 0 }, { 1, 0, 0, 0 }, { 0, 0, 0, 1 }, { 0, 0, 1, 0 } };
            double[,] dataTwo = { { 4, 2, 1, 3 }, { 1, 3, 2, 1 }, { 0, 2, 5, 4 }, { 2, 1, 3, 6 } };

            Algorithms.Precision = 7;

            RealMatrix error = RealMatrix.From(dataOne);
            RealMatrix converges = RealMatrix.From(dataTwo);

            double[] eigenvaluesOne = Algorithms.Eigenvalues(error);
            double[] eigenvaluesTwo = Algorithms.Eigenvalues(converges);

            Assert.IsTrue(eigenvaluesOne.Length == 0);

            Assert.AreEqual(eigenvaluesTwo[0], 10.5741, Math.Pow(10, -4));
            Assert.AreEqual(eigenvaluesTwo[1], 3.74323, Math.Pow(10, -4));
            Assert.AreEqual(eigenvaluesTwo[2], 3.14433, Math.Pow(10, -4));
            Assert.AreEqual(eigenvaluesTwo[3], 0.53834, Math.Pow(10, -4));
        }
    }
}