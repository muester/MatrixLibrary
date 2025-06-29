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
        // Private constructory

        //TODO: Square matrix constructor shorthand
        private RealMatrix(int t_Length) : this(t_Length, t_Length) { }
        private RealMatrix(int t_Height, int t_Width)
        {
            Height = t_Height;
            Width = t_Width;

            m_Elements = new double[Height, Width];
        }
        // Public constructory
        public static RealMatrix Zeros(int t_Height, int t_Width)
        {
            RealMatrix r_Matrix = new RealMatrix(t_Height, t_Width);
            return r_Matrix;
        }
        public static RealMatrix Eye(int t_Height, int t_Width)
        {
            RealMatrix r_Matrix = new RealMatrix(t_Height, t_Width);

            for (int d = 0; d < t_Height && d < t_Width; ++d)
            {
                r_Matrix.m_Elements[d, d] = 1;
            }

            return r_Matrix;
        }
        public static RealMatrix From(double[,] t_Elements)
        {
            RealMatrix r_Matrix = new RealMatrix(t_Elements.GetLength(0), t_Elements.GetLength(1));

            for(int r = 0; r < r_Matrix.Height; ++r)
            {
                for(int c = 0; c < r_Matrix.Width; ++c)
                {
                    r_Matrix[r+1, c+1] = t_Elements[r, c];
                }
            }

            return r_Matrix;
        }
        
        public static RealMatrix Elementary(int t_Length, int t_Nonzero)
        {
            RealMatrix r_Matrix = RealMatrix.Zeros(t_Length, 1);

            r_Matrix[t_Nonzero, 1] = 1;

            return r_Matrix;
        }
        // Indexers and operator overloads
        public double this[int t_Row, int t_Column]
        {
            get
            {
                if (t_Row > Height  || t_Column > Width)
                {
                    throw new IndexOutOfRangeException("");
                }
                return m_Elements[t_Row -1, t_Column -1];
            }
            set
            {
                if (t_Row > Height || t_Column > Width)
                {
                    throw new IndexOutOfRangeException("");
                }
                m_Elements[t_Row -1, t_Column -1] = value;
            }
        }

        // Generuje kopii matice
        public static RealMatrix operator ~(RealMatrix t_Matrix)
        {
            RealMatrix r_Matrix = new RealMatrix(t_Matrix.Height, t_Matrix.Width);

            for (int r = 0; r < r_Matrix.Height; ++r)
            {
                for (int c = 0; c < r_Matrix.Width; ++c)
                {
                    r_Matrix.m_Elements[r, c] = t_Matrix[r+1, c+1];
                }
            }

            return r_Matrix;
        }

        // Generuje obrácenou kopii matice
        public static RealMatrix operator -(RealMatrix t_Matrix)
        {
            RealMatrix r_Matrix = new RealMatrix(t_Matrix.Height, t_Matrix.Width);

            for (int r = 0; r < r_Matrix.Height; ++r)
            {
                for (int c = 0; c < r_Matrix.Width; ++c)
                {
                    r_Matrix.m_Elements[r, c] = -1 * t_Matrix[r+1, c+1];
                }
            }

            return r_Matrix;
        }
        
        // Násobení matic
        // TODO: DIMENSION CHECKING
        public static RealMatrix operator *(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)
        {
            RealMatrix r_Matrix = RealMatrix.Zeros(t_LeftMatrix.Height, t_RightMatrix.Width);

            for(int r = 0; r < r_Matrix.Height; ++r)
            {
                for (int c = 0; c < r_Matrix.Width; ++c)
                {
                    for(int o = 0; o < t_LeftMatrix.Width; ++o)
                    {
                        r_Matrix[r + 1, c + 1] += (t_LeftMatrix[r + 1, o+1] * t_RightMatrix[o+1, c + 1]);
                    }
                }
            }

            return r_Matrix;
        }

        public static RealMatrix operator -(RealMatrix t_LeftMatrix, RealMatrix t_RightMatrix)
        {
            RealMatrix r_Matrix = RealMatrix.Zeros(t_LeftMatrix.Height, t_LeftMatrix.Width);

            for(int r = 0; r < r_Matrix.Height; ++r)
            {
                for(int c = 0; c < r_Matrix.Width; ++c)
                {
                    r_Matrix[r + 1, c + 1] = t_LeftMatrix[r + 1, c + 1] - t_RightMatrix[r + 1, c + 1];
                }
            }

            return r_Matrix;
        }

        public RealMatrix Column(int t_Column)
        {
            if(t_Column > Width)
            {
                throw new IndexOutOfRangeException("");
            }

            RealMatrix r_Matrix = RealMatrix.Zeros(Height, 1);

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
