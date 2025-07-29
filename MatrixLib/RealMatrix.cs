/*
 * Z technického hlediska, ve finálním produktu by (mimo jiné) neměla chybět: vhodná implementace I/O, především v nějakém vhodném souborovém formátu,
reprezentace obecné matice reálných čísel, základních maticových operací vč. determinantu, výpočtu inverzu, dále LU a QR rozklad matice, řešení SLR udáno maticí,
výpočet vlastních čísel, a to celé včetně vhodného ošetření vstupů a povolených operací. Jelikož pracujeme s reálnými čísly a je nutnost některé metody dělat iteračně,
např. výpočet vlastních čísel, (viz Abel-Ruffiniova věta), bude kladen zájem o to, aby algoritmy byly dostatečně rychlé a zároveň stabilní.
*/


using System;

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

            for (int d = 1; d <= r_Matrix.Height && d < r_Matrix.Width; ++d)
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
            (int t_Height, int t_Width) = (t_Elements.GetLength(0), t_Elements.GetLength(1));
            
            RealMatrix r_Matrix = new RealMatrix(t_Height, t_Width);

            for(int r = 1; r <= r_Matrix.Height; ++r)
            {
                for(int c = 1; c <= r_Matrix.Width; ++c)
                {
                    r_Matrix[r, c] = t_Elements[r-1, c-1];
                }
            }

            return r_Matrix;
        }
        
        // Indexers and operator overloads
        
        public double this[int t_Row, int t_Column]
        {
            get
            {
                if (t_Row > Height  || t_Column > Width)
                {
                    throw new IndexOutOfRangeException("Error: Non-existing element accessed!");
                }
                return m_Elements[t_Row -1, t_Column -1];
            }
            set
            {
                if (t_Row > Height || t_Column > Width)
                {
                    throw new IndexOutOfRangeException("Error: Non-existing element accessed!");
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

        public static RealMatrix ElementwiseOperation(Operation operation, RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)
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
                    r_Matrix[r, c] = operation(t_LeftMatrix[r, c],t_RightMatrix[r, c]);
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

        // Extracting submatrices

        public RealMatrix Column(int t_Column)
        {
            if(t_Column > Width)
            {
                throw new IndexOutOfRangeException("");
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
            if (t_Row > Height)
            {
                throw new IndexOutOfRangeException("");
            }

            RealMatrix r_Matrix = RealMatrix.Zeros(1, Width);

            for (int r = 1; r <= Width; ++r)
            {
                r_Matrix[1, r] = this[t_Row, 1];
            }

            return r_Matrix;
        }
        public RealMatrix SubMatrix(int t_RowBegin, int t_RowEnd, int t_ColumnBegin, int t_ColumnEnd)
        {
            //TODO: Check for logical boundary errors
            if(t_RowEnd > Height || t_ColumnEnd > Width)
            {
                throw new IndexOutOfRangeException("");
            }

            int t_Height = (t_RowEnd - t_RowBegin) + 1;
            int t_Width = (t_ColumnEnd - t_ColumnBegin) + 1;

            RealMatrix r_Matrix = RealMatrix.Zeros(t_Height,t_Width);

            for (int r = t_RowBegin; r <= t_RowEnd; ++r)
            {
                for(int c = t_ColumnBegin; c <= t_ColumnEnd; ++c)
                {
                    r_Matrix[(r + 1) - t_RowBegin, (c + 1) - t_ColumnBegin] = this[r, c];
                }
            }

            return r_Matrix;
        }
        public RealMatrix Insert(int t_Row, int t_Column, RealMatrix t_InsertedMatrix)
        {
            int RequiredWidth = (t_InsertedMatrix.Width + t_Column) - 1;
            int RequiredHeight = (t_InsertedMatrix.Height + t_Row) - 1;

            if (RequiredWidth > this.Width || RequiredHeight > this.Height)
            {
                throw new IndexOutOfRangeException("");
            }

            RealMatrix r_Matrix = ~this;

            for (int r = 0; r < t_InsertedMatrix.Height; ++r)
            {
                for (int c = 0; c < t_InsertedMatrix.Width; ++c)
                {
                    r_Matrix[r + t_Row, c + t_Column] = t_InsertedMatrix[r+1, c+1];
                }
            }

            return r_Matrix;
        }
        
        public RealMatrix Modify(Operation t_operation, double t_value)
        {
            RealMatrix r_Matrix = RealMatrix.Zeros(this.Height, this.Width);

            for(int r = 0; r < Height; ++r)
            {
               for(int c = 0; c < Width; ++c)
                {
                    r_Matrix[r + 1, c + 1] = t_operation(this[r + 1, c + 1], t_value);
                }
            }

            return r_Matrix;
        }
        public RealMatrix Transpose()
        {
            RealMatrix r_TransposedMatrix = new RealMatrix(Width, Height);
            
            for(int r = 0; r < Height; ++r)
            {
                for(int c = 0; c < Width; ++c)
                {
                    r_TransposedMatrix[c+1, r+1] = m_Elements[r, c];
                }
            }
            return r_TransposedMatrix;
        }

        //TODO: Rewrite print as pretty print
        public void Print()
        {
            for (int r = 1; r <= this.Height; ++r)
            {
                for (int c = 1; c <= this.Width; ++c)
                {
                    Console.Write(this[r, c] + " ");
                }
                Console.WriteLine();
            }
        }
        
        public delegate double Operation(double t_first, double t_second);
        
        public double Norm
        {
            get
            {
                double r_Norm = 0;

                for(int r = 0; r < Height; ++r)
                {
                    for (int c = 0; c < Width; ++c)
                    {
                        r_Norm += m_Elements[r, c]* m_Elements[r,c];
                    }
                }

                return Math.Sqrt(r_Norm);
            }

            private set {}
        }
        
        //TODO: IMPLEMENT SQUARE CHECK
        public double Determinant
        {
            get
            {
                return MatrixLib.Algorithms.Determinant(this);
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        private double[,] m_Elements;
    }
}
