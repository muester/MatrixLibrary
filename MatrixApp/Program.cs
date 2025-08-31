using MatrixLib;
using System;
using System.Collections.Generic;

namespace MatrixApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BasicShowcase();

            Console.WriteLine();
            
            DecompositionShowcase();

            Console.WriteLine();

            SpecificShowcase();
        }
    
        static void BasicShowcase()
        {

            int size = 7;
            int second_size = 5;

            RealMatrix showcase = RealMatrix.Zeros(size, second_size);
            
            Console.WriteLine("Null matrix");
            showcase.Print();

            showcase = RealMatrix.Eye(size, second_size);
            
            Console.WriteLine("Diagonal eye matrix");
            showcase.Print();

            showcase = RealMatrix.Elementary(size, second_size);

            Console.WriteLine("e5 Matrix");
            showcase.Print();

            showcase[3, 1] = 5;

            Console.WriteLine("Direct matrix access");
            showcase.Print();

            RealMatrix diagonal = RealMatrix.Eye(size);

            RealMatrix threes = RealMatrix.Zeros(second_size).Modify((a, b) => (a + b), 3);

            showcase = diagonal.Insert(3, 3, threes);

            Console.WriteLine("Combined matrices");
            showcase.Print();

            showcase = showcase.SubMatrix(1, 3, 1, showcase.Width);

            Console.WriteLine("Extracted matrices");
            showcase.Print();

            showcase = showcase.Transpose();

            Console.WriteLine("Transposed matrices");
            showcase.Print();
        }
        
        static void DecompositionShowcase()
        {
            Algorithms.Precision = 16;

            int size = 50;

            Random rand = new Random();

            double[,] Hilbert = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                   Hilbert[i, j] = 1.0 / (i + j + 1);
                }
            }

            double[,] Good = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Good[i, j] = rand.NextDouble()*rand.Next(-100000,100000);
                }
            }

            Console.WriteLine($"Test 1: Hilbert matrix {size} x {size}");
            RealMatrix HilbertMatrix = RealMatrix.From(Hilbert);
            (RealMatrix L, RealMatrix U, RealMatrix P) = Algorithms.LU(HilbertMatrix);
            (RealMatrix Q, RealMatrix R) = Algorithms.QR(HilbertMatrix);
            RealMatrix Reconstructed = P.Transpose() * L * U;
            RealMatrix Difference = HilbertMatrix - Reconstructed;

            Console.WriteLine("Hilbert matrix LUP relative error: " + Difference.Norm/HilbertMatrix.Norm);

            Difference = HilbertMatrix - (Q * R);
            RealMatrix Loss = (RealMatrix.Eye(Q.Width) - (Q.Transpose() * Q));

            Console.WriteLine("Hilbert matrix QR relative error: " + Difference.Norm / HilbertMatrix.Norm);
            Console.WriteLine("Loss of orthogonality: " + Loss.Norm);

            Console.WriteLine();

            Console.WriteLine($"Test 2: Random (well conditioned) matrix {size} x {size}");
            RealMatrix GoodMatrix = RealMatrix.From(Good);
            (L,U,P) = Algorithms.LU(GoodMatrix);
            (Q,R) = Algorithms.QR(GoodMatrix);
            Reconstructed = P.Transpose() * L * U;
            Difference = GoodMatrix - Reconstructed;

            Console.WriteLine("Good matrix LUP relative error: " + Difference.Norm / GoodMatrix.Norm);

            Difference = GoodMatrix - (Q * R);
            Loss = (RealMatrix.Eye(Q.Width) - (Q.Transpose() * Q));

            Console.WriteLine("Good matrix QR relative error: " + Difference.Norm / GoodMatrix.Norm);
            Console.WriteLine("Loss of orthogonality: " + Loss.Norm);


            RealMatrix Inverse = Algorithms.Inverse(GoodMatrix);
            Difference = (RealMatrix.Eye(size) - Inverse * GoodMatrix);

            Console.WriteLine("Good inverse relative error: " +  Difference.Norm/GoodMatrix.Norm);

        }

        static void SpecificShowcase()
        {

            int size = 20;

            double[,] tridiagonal = new double[size, size];

            for(int i = 0; i < size; ++i)
            {
                for(int j = 0; j < size; ++j)
                {
                    if (i == j)
                        tridiagonal[i, j] = 2;
                    else if (Math.Abs(i - j) == 1)
                        tridiagonal[i, j] = -1;
                    else
                        tridiagonal[i, j] = 0;
                }
            }


            List<double> eigs = new List<double>();

            for (int k = 1; k <= size; ++k)
            {
                eigs.Add(2-2*Math.Cos((k*Math.PI)/ (size+1)));
            }

            eigs.Sort();
            eigs.Reverse();

            RealMatrix TridiagonalMatrix = RealMatrix.From(tridiagonal);

            Console.WriteLine("Matrix test subject: Tridiagonal laplacian matrix (heat equatin FEM)");
            // TridiagonalMatrix.Print();
            Console.WriteLine();

            double det = Algorithms.Determinant(TridiagonalMatrix);

            Console.WriteLine($"Determinant: {det}, correct: {size+1}");

            int t = 0;

            foreach (double e in Algorithms.Eigenvalues(TridiagonalMatrix))
            {
                Console.WriteLine($"Eigenvalue: {e}, correct: { eigs[t]} ");
                ++t;
            }
        }
    }
}
