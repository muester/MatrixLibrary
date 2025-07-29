using MatrixLib;
using System;

namespace MatrixApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double[,] data = {
                    {2,3,1},
                    {4,7,7},
                    {-2,4,5},
            };

            double[,] datatwo = {
                    {1,2,-1},
                    {4,3,3},
                    {-2,5,1},
            };


            // Algorithms.Precision = 2;

            RealMatrix m = RealMatrix.From(data);
            RealMatrix b = RealMatrix.From(datatwo);

            var c = m - b;

            c.Print();
        }
    }
}
