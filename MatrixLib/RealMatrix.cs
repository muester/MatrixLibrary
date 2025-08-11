using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MatrixLib
{
    public class RealMatrix 
    {
        // Private constructors

        private RealMatrix(int t_Height, int t_Width)
        {
            Height = t_Height;
            Width = t_Width;

            m_Elements = new double[Height, Width];
        }

        // Public "constructors"

        public static RealMatrix Zeros(int t_Length)
        {
            return Zeros(t_Length, t_Length);
        }

        public static RealMatrix Zeros(int t_Height, int t_Width)
        {
            RealMatrix r_Matrix = new RealMatrix(t_Height, t_Width);
            return r_Matrix;
        }

        public static RealMatrix Eye(int t_Length)
        {
            return Eye(t_Length, t_Length);
        }

        public static RealMatrix Eye(int t_Height, int t_Width)
        {
            RealMatrix r_Matrix = new RealMatrix(t_Height, t_Width);

            for (int d = 1; d <= r_Matrix.Height && d <= r_Matrix.Width; ++d)
            {
                r_Matrix[d, d] = 1;
            }

            return r_Matrix;
        }

        public static RealMatrix Elementary(int t_Length, int t_Nonzero)
        {
            RealMatrix r_Matrix = Zeros(t_Length, 1);

            r_Matrix[t_Nonzero, 1] = 1;

            return r_Matrix;
        }

        public static RealMatrix From(double[,] t_Elements)
        {
            (int height, int width) = (t_Elements.GetLength(0), t_Elements.GetLength(1));
            
            RealMatrix r_Matrix = new RealMatrix(height, width);

            for(int r = 1; r <= r_Matrix.Height; ++r)
            {
                for(int c = 1; c <= r_Matrix.Width; ++c)
                {
                    r_Matrix[r, c] = t_Elements[r-1, c-1];
                }
            }

            return r_Matrix;
        }
        
        public static RealMatrix[] From(string t_File)
        {
            if (t_File.Length < 5 || t_File.Substring(t_File.Length - 4) != ".mat")
            {
                throw new ArgumentException("Error: Unsupported file name! (check file extension?)");
            }

            if (!File.Exists(t_File))
            {
                throw new FileNotFoundException("Error: File not found!");
            }

            using (StreamReader fileData = new StreamReader(t_File))
            {

                string? metadata = fileData.ReadLine() ?? throw new FormatException("Error: Incorrect matrix file format!");
                uint matrices = uint.Parse(metadata.Trim());

                RealMatrix[] r_Matrices = new RealMatrix[matrices];

                fileData.ReadLine();

                string? header = fileData.ReadLine();

                for (int m = 0; header != null; ++m)
                {
                    string[] dimensions = header.Split(' ');

                    if (dimensions.Length != 2)
                    {
                        throw new RankException("Error: Incorrect amount of dimensions provided!");
                    }

                    (int height, int width) = (Int32.Parse(dimensions[0]), Int32.Parse(dimensions[1]));
                    RealMatrix r_Matrix = Zeros(height, width);

                    for (int r = 1; r <= height; ++r)
                    {
                        string? row = fileData.ReadLine() ?? throw new EndOfStreamException("Error: Expected matrix data not found!");
                        string[] entries = Regex.Replace(row.Trim(), @"\s+", " ").Split(" ");

                        for (int c = 1; c <= width; ++c)
                        {
                            r_Matrix[r, c] = Double.Parse(entries[c - 1]);
                        }
                    }

                    r_Matrices[m] = r_Matrix;

                    fileData.ReadLine();

                    header = fileData.ReadLine();
                }
                return r_Matrices;
            }
        }

        // Saving matrices to a file

        public static void SaveMatricesTo(RealMatrix t_Matrix, string t_File)
        {
            SaveMatricesTo([t_Matrix], t_File);
        }

        public static void SaveMatricesTo(RealMatrix[] t_Matrices, string t_File)
        {
            if (t_File.Length < 5 || t_File.Substring(t_File.Length - 4) != ".mat")
            {
                throw new ArgumentException("Error: Unsupported file name! (check file extension?)");
            }

            using (StreamWriter fileData = File.CreateText(t_File))
            {
                fileData.WriteLine(t_Matrices.Length);
                for (int m = 0; m < t_Matrices.Length; ++m)
                {

                    fileData.WriteLine();
                    fileData.WriteLine($"{t_Matrices[m].Height} {t_Matrices[m].Width}");

                    for (int r = 1; r <= t_Matrices[m].Height; ++r)
                    {
                        for (int c = 1; c <= t_Matrices[m].Width; ++c)
                        {
                            fileData.Write(t_Matrices[m][r, c] + " ");
                        }
                        fileData.Write("\n");
                    }
                }
            }
        }

        // Indexers and operator overloads
        
        public double this[int t_Row, int t_Column]
        {
            get
            {
                if (t_Row > Height  || t_Column > Width)
                {
                    throw new IndexOutOfRangeException("Error: Non-existent element accessed!");
                }

                return m_Elements[t_Row -1, t_Column -1];
            }
            set
            {
                if (t_Row > Height || t_Column > Width)
                {
                    throw new IndexOutOfRangeException("Error: Non-existent element accessed!");
                }

                m_Elements[t_Row -1, t_Column -1] = value;
            }
        }

        // Matrix copy operator

        public static RealMatrix operator ~(RealMatrix t_Matrix)
        {
            RealMatrix r_Matrix = new RealMatrix(t_Matrix.Height, t_Matrix.Width);

            for (int r = 1; r <= r_Matrix.Height; ++r)
            {
                for (int c = 1; c <= r_Matrix.Width; ++c)
                {
                    r_Matrix[r, c] = t_Matrix[r, c];
                }
            }

            return r_Matrix;
        }

        // Return a copy of a matrix with opposite elements

        public static RealMatrix operator -(RealMatrix t_Matrix)
        {
            RealMatrix r_Matrix = new RealMatrix(t_Matrix.Height, t_Matrix.Width);

            for (int r = 1; r <= r_Matrix.Height; ++r)
            {
                for (int c = 1; c <= r_Matrix.Width; ++c)
                {
                    r_Matrix[r, c] = -1 * t_Matrix[r, c];
                }
            }

            return r_Matrix;
        }
        
        // Matrix multiplication

        public static RealMatrix operator *(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)
        {
            if (t_LeftMatrix.Width != t_RightMatrix.Height)
            {
                throw new RankException("Error: Incompatible matrix dimensions!");
            }
            
            RealMatrix r_Matrix = Zeros(t_LeftMatrix.Height, t_RightMatrix.Width);

            for(int r = 1; r <= r_Matrix.Height; ++r)
            {
                for (int c = 1; c <= r_Matrix.Width; ++c)
                {
                    for(int o = 1; o <= t_LeftMatrix.Width; ++o)
                    {
                        r_Matrix[r, c] += (t_LeftMatrix[r, o] * t_RightMatrix[o, c]);
                    }
                }
            }

            return r_Matrix;
        }

        public static RealMatrix ElementwiseOperation(Operation t_operation, RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)
        {
            if (t_LeftMatrix.Height != t_RightMatrix.Height|| t_LeftMatrix.Width != t_RightMatrix.Width)
            {
                throw new RankException("Error: Incompatible matrix dimensions!");
            }

            RealMatrix r_Matrix = Zeros(t_LeftMatrix.Height, t_LeftMatrix.Width);

            for (int r = 1; r <= r_Matrix.Height; ++r)
            {
                for (int c = 1; c <= r_Matrix.Width; ++c)
                {
                    r_Matrix[r, c] = t_operation(t_LeftMatrix[r, c],t_RightMatrix[r, c]);
                }
            }

            return r_Matrix;
        }

        public static RealMatrix operator +(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)
        {
            return ElementwiseOperation((a, b) => a + b, t_LeftMatrix, t_RightMatrix);
        }

        public static RealMatrix operator -(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)
        {
            return ElementwiseOperation((a, b) => a - b, t_LeftMatrix, t_RightMatrix);
        }

        public static bool operator ==(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)
        {
            if(t_LeftMatrix.Height != t_RightMatrix.Height || t_LeftMatrix.Width != t_RightMatrix.Width)
            {
                return false;
            }

            for (int r = 1; r <= t_LeftMatrix.Height; ++r)
            {
                for (int c = 1; c <= t_LeftMatrix.Width; ++c)
                {
                    if (t_LeftMatrix[r,c] != t_RightMatrix[r,c])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool operator !=(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)
        {
            return !(t_LeftMatrix == t_RightMatrix);
        }

        public override bool Equals(object? obj)
        {
            return obj is null || GetType() != obj.GetType() ? false : this == (RealMatrix) obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Extracting submatrices

        public RealMatrix Column(int t_Column)
        {
            if(0 > t_Column || t_Column > Width)
            {
                throw new IndexOutOfRangeException("Error: Non-existent column accessed!");
            }
             
            RealMatrix r_Matrix = Zeros(Height, 1);

            for(int r = 1; r <= Height; ++r)
            {
                r_Matrix[r, 1] = this[r, t_Column];
            }

            return r_Matrix;
        }

        public RealMatrix Row(int t_Row)
        {
            if (0 > t_Row || t_Row > Height)
            {
                throw new IndexOutOfRangeException("Error: Non-existent row accessed!");
            }

            RealMatrix r_Matrix = Zeros(1, Width);

            for (int r = 1; r <= Width; ++r)
            {
                r_Matrix[1, r] = this[t_Row, r];
            }

            return r_Matrix;
        }

        public RealMatrix SubMatrix(int t_RowBegin, int t_RowEnd, int t_ColumnBegin, int t_ColumnEnd)
        {

            bool correctHeight = (0 < t_RowBegin) && (t_RowBegin <= t_RowEnd) && (t_RowEnd <= Height);
            bool correctWidth = (0 < t_ColumnBegin) && (t_ColumnBegin <= t_ColumnEnd) && (t_ColumnEnd <= Width);

            if (!(correctHeight && correctWidth)) 
            {
                throw new IndexOutOfRangeException("Error: Non-existent elements requested!");
            }

            int height = (t_RowEnd - t_RowBegin) + 1;
            int width = (t_ColumnEnd - t_ColumnBegin) + 1;

            RealMatrix r_Matrix = Zeros(height,width);

            for (int r = t_RowBegin; r <= t_RowEnd; ++r)
            {
                for(int c = t_ColumnBegin; c <= t_ColumnEnd; ++c)
                {
                    r_Matrix[1 + (r - t_RowBegin), 1 + (c - t_ColumnBegin)] = this[r, c];
                }
            }

            return r_Matrix;
        }

        public RealMatrix Insert(int t_Row, int t_Column, RealMatrix t_InsertedMatrix)
        {
            int requiredWidth = (t_InsertedMatrix.Width + t_Column) - 1;
            int requiredHeight = (t_InsertedMatrix.Height + t_Row) - 1;

            if (requiredWidth > Width || requiredHeight > Height)
            {
                throw new RankException("Error: Matrix dimensions too large to be inserted at the requested position!");
            }

            RealMatrix r_Matrix = ~this;

            for (int r = 1; r <= t_InsertedMatrix.Height; ++r)
            {
                for (int c = 1; c <= t_InsertedMatrix.Width; ++c)
                {
                    r_Matrix[(r + t_Row) - 1, (c + t_Column) - 1] = t_InsertedMatrix[r, c];
                }
            }

            return r_Matrix;
        }

        public RealMatrix Modify(Operation t_operation, double t_value)
        {
            RealMatrix r_Matrix = Zeros(Height, Width);

            for(int r = 1; r <= Height; ++r)
            {
               for(int c = 1; c <= Width; ++c)
               {
                    r_Matrix[r, c] = t_operation(this[r, c], t_value);
               }
            }

            return r_Matrix;
        }

        public RealMatrix Round(int Precision)
        {
            if (Precision == 16)
                return this;
            return Modify((a, b) => (double)Math.Round((double)a, (int)b), Precision);
        }

        public RealMatrix Transpose()
        {
            RealMatrix r_TransposedMatrix = Zeros(Width, Height);
            
            for(int r = 1; r <= Height; ++r)
            {
                for(int c = 1; c <= Width; ++c)
                {
                    r_TransposedMatrix[c, r] = this[r, c];
                }
            }
            return r_TransposedMatrix;
        }

        public void Print()
        {
            int paddingOffset = 2;

            int[] paddings = new int[Width];

            for (int c = 1; c <= Width; ++c)
            {
                for (int r = 1; r <= Height; ++r)
                {
                    int length = Math.Sign(this[r, c]) * Math.Abs(Math.Floor(this[r, c])).ToString().Length;

                    paddings[c-1] = paddings[c-1] < length ? length : paddings[c-1];
                }
            }

            for (int r = 1; r <= Height; ++r)
            {
                for (int c = 1; c <= Width; ++c)
                {
                    double entry = Algorithms.Precision == 16 ? this[r,c] : Math.Round(this[r,c], Algorithms.Precision);

                    Console.Write(entry.ToString().PadLeft(paddings[c-1]+ Algorithms.Precision + paddingOffset,' ') + " ");
                }

                Console.WriteLine();
            }
        }
        
        public double Norm
        {
            get
            {
                double r_Norm = 0;

                for (int r = 1; r <= Height; ++r)
                {
                    for (int c = 1; c <= Width; ++c)
                    {
                        r_Norm += this[r, c] * this[r,c];
                    }
                }

                return Math.Sqrt(r_Norm);
            }

            private set {}
        }
        
        public delegate double Operation(double t_first, double t_second);

        public int Width { get; private set; }
        public int Height { get; private set; }

        private double[,] m_Elements;
    }
}
