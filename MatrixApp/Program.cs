using MatrixLib;
using System;

namespace MatrixApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double[,] data = { { 1, 2, 3 },{ 1, -1, 2 },{ -4,5,-6} };

            RealMatrix m = RealMatrix.From(data);

            MatrixLib.Algorithms.Precision = 4;

            double t = m.Determinant;

            Console.WriteLine(t);

        }
    }
}
