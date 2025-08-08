using MatrixLib;
using System;

namespace MatrixApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double[,] data = {
                    {1,2,3},
                    {4,5,6},
                    {7,8,9},
            };

            Algorithms.Precision = 8;

            RealMatrix matrices = RealMatrix.From(data);

            var x = Algorithms.Eigenvalues(matrices);

            foreach (var matrix in x)
            {
                {
                    Console.WriteLine(matrix);
                }
            }
        }
    }
}
