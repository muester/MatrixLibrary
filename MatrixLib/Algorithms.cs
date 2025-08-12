using System;

namespace MatrixLib
{
    public static class Algorithms
    {
        public static int Precision = 16; 

        internal static RealMatrix HHMatrix(RealMatrix t_Matrix, RealMatrix t_Vector)
        {
            int offset = (t_Matrix.Height - t_Vector.Height) + 1;

            double alpha = -Math.Sign(t_Vector[1, 1]) * t_Vector.Norm;

            RealMatrix v = t_Vector - RealMatrix.Elementary(1 + (t_Matrix.Height - offset), 1).Modify((a, b) => a * b, alpha);

            v = v.Norm != 0 ? v.Modify((a, b) => a / b, v.Norm) : v;

            RealMatrix r_Householder = RealMatrix.Eye(1 + (t_Matrix.Height - offset), 1 + (t_Matrix.Height - offset));

            r_Householder -= (v * v.Transpose()).Modify((a, b) => a * b, 2);

            r_Householder = RealMatrix.Eye(t_Matrix.Height, t_Matrix.Height).Insert(offset, offset, r_Householder);

            return r_Householder;
        }

        public static (RealMatrix, RealMatrix) QR(RealMatrix t_Matrix)
        {
            RealMatrix r_RMatrix = ~t_Matrix;
            RealMatrix r_QMatrix = RealMatrix.Eye(t_Matrix.Height,t_Matrix.Height);

            for(int c = 1; c <= Math.Min(t_Matrix.Width,t_Matrix.Height); ++c)
            {
                RealMatrix projector = r_RMatrix.SubMatrix(c, r_RMatrix.Height, c, c);

                RealMatrix householder = HHMatrix(r_RMatrix, projector);
;
                r_QMatrix = r_QMatrix * householder;
                r_RMatrix = householder * r_RMatrix;
            }

            r_QMatrix = r_QMatrix.SubMatrix(1, r_QMatrix.Height, 1, Math.Min(t_Matrix.Width,r_QMatrix.Width));
            r_RMatrix = r_RMatrix.SubMatrix(1, Math.Min(t_Matrix.Width, t_Matrix.Height), 1, Math.Min(t_Matrix.Width,r_RMatrix.Width));

            r_QMatrix = r_QMatrix.Round(Precision);
            r_RMatrix = r_RMatrix.Round(Precision);

            return (r_QMatrix, r_RMatrix);
        }

        public static (RealMatrix,RealMatrix) GivensQR(RealMatrix t_Matrix)
        {

            double eps = Math.Pow(10, -Precision);

            RealMatrix r_RMatrix = ~t_Matrix;
            RealMatrix r_QMatrix = RealMatrix.Eye(t_Matrix.Height, t_Matrix.Height);

            for (int c = 1; c < r_RMatrix.Width; ++c)
            {
                for(int r = c+1; r <= r_RMatrix.Height; ++r)
                {
                    if (Math.Abs(r_RMatrix[r,c]) > eps)
                    {
                        RealMatrix givensMatrix = RealMatrix.Eye(r_RMatrix.Height, r_RMatrix.Height);

                        var hypotenuse = Math.Sqrt(Math.Pow(r_RMatrix[c,c],2) + Math.Pow(r_RMatrix[r,c],2));
                        var cosine = r_RMatrix[c, c] / hypotenuse;
                        var sine = -r_RMatrix[r, c] / hypotenuse;

                        givensMatrix[c, c] = cosine;
                        givensMatrix[r, r] = cosine;

                        givensMatrix[r, c] = sine;
                        givensMatrix[c, r] = -sine;

                        r_RMatrix = givensMatrix * r_RMatrix;

                        r_QMatrix *= givensMatrix.Transpose();
                    }
                }
            }

            r_QMatrix = r_QMatrix.SubMatrix(1, r_QMatrix.Height, 1, Math.Min(t_Matrix.Width, r_QMatrix.Width));
            r_RMatrix = r_RMatrix.SubMatrix(1, Math.Min(t_Matrix.Width, t_Matrix.Height), 1, Math.Min(t_Matrix.Width, r_RMatrix.Width));

            return (r_QMatrix, r_RMatrix);
        }

        internal static RealMatrix Hessenberg(RealMatrix t_Matrix)
        {
            if(t_Matrix.Height != t_Matrix.Width)
            {
                throw new RankException("Error: Matrix is not square!");
            }

            RealMatrix r_Matrix =~ t_Matrix;

            for (int s = 1; s <= r_Matrix.Height - 2; ++s)
            {

                RealMatrix column = r_Matrix.SubMatrix(1+s, r_Matrix.Height, s, s);

                RealMatrix householder = HHMatrix(r_Matrix, column);

                r_Matrix = householder * r_Matrix * householder.Transpose();
            }

            r_Matrix = r_Matrix.Round(Precision);

            return r_Matrix;
        }

        internal static RealMatrix? Triangular(RealMatrix t_Matrix)
        {
            double eps = Math.Pow(10, -Precision);

            int iteration = 0;
            double error = 0;
            double lastError;

            bool repeat = true;

            while(repeat)
            {
                ++iteration;
                
                lastError = error;
                error = 0;
                repeat = false;

                for(int i = 1; i < t_Matrix.Width; ++i)
                {
                    error += Math.Abs(t_Matrix[i + 1, i]);
                    if (Math.Abs(t_Matrix[i+1, i]) > eps)
                    {
                        repeat = true;
                    }      
                }
                

                // Algorithm does not converge on this matrix
                if(iteration > 3 && (lastError-error) <= 0)
                {
                    return null;
                }    

                var (Q, R) = GivensQR(t_Matrix);
                t_Matrix = R * Q;
            }
            return t_Matrix;
        }

        internal static RealMatrix BackwardTriangle(RealMatrix t_Matrix)
        {
            if (t_Matrix.Height != t_Matrix.Width)
            {
                throw new RankException("Error: Matrix is not square!");
            }

            RealMatrix r_Matrix = RealMatrix.Zeros(t_Matrix.Height, t_Matrix.Width);

            for (int c = 1; c <= r_Matrix.Width; ++c)
            {
                for (int r = r_Matrix.Height; r >= 1; --r)
                {
                    double kronecker = (c == r ? 1 : 0);

                    double res = kronecker;

                    for (int o = r + 1; o <= r_Matrix.Height; ++o)
                    {
                        res -= t_Matrix[r, o] * r_Matrix[o, c];
                    }

                    r_Matrix[r, c] = res / t_Matrix[r,r];
                }
            }

            return r_Matrix;
        }

        internal static RealMatrix ForwardTriangle(RealMatrix t_Matrix)
        {
            if (t_Matrix.Height != t_Matrix.Width)
            {
                throw new RankException("Error: Matrix is not square!");
            }

            RealMatrix r_Matrix = RealMatrix.Zeros(t_Matrix.Height, t_Matrix.Width);

            for (int c = 1; c <= r_Matrix.Width; ++c)
            {
                for (int r = 1; r <= r_Matrix.Height; ++r)
                {
                    double kronecker = (c == r ? 1 : 0);

                    double res = kronecker;

                    for (int o = 1; o <= r - 1; ++o)
                    {
                        res -= t_Matrix[r, o] * r_Matrix[o, c];
                    }

                    r_Matrix[r, c] = res / t_Matrix[r, r];
                }
            }

            return r_Matrix;
        }

        public static RealMatrix Inverse(RealMatrix t_Matrix)
        {
            if(Determinant(t_Matrix) == 0)
            {
                throw new ArgumentException("Error: Singular matrices do not have a clearly defined inverse!");
            }
            
            (RealMatrix L, RealMatrix U, RealMatrix P) = LU(t_Matrix);

            RealMatrix r_Matrix = BackwardTriangle(U) * ForwardTriangle(L) * P;

            return r_Matrix;
        }

        public static RealMatrix LinSys(RealMatrix t_Matrix, RealMatrix b_Matrix)
        {
            if (t_Matrix.Height != t_Matrix.Width)
            {
                throw new RankException("Error: Matrix is not square!");
            }

            return Inverse(t_Matrix) * b_Matrix;
        }

        internal static RealMatrix SwapRows(RealMatrix t_Matrix, int t_First, int t_Second, int t_ColumnStart, int t_ColumnEnd)
        {
            for(int c = t_ColumnStart; c <= t_ColumnEnd; ++c)
            {
                (t_Matrix[t_First, c], t_Matrix[t_Second, c]) = (t_Matrix[t_Second, c], t_Matrix[t_First, c]);
            }

            return t_Matrix;
        }

        internal static (RealMatrix, RealMatrix, RealMatrix, int) InternalLUP(RealMatrix t_Matrix)
        {
            RealMatrix r_RMatrix = ~t_Matrix;
            RealMatrix r_LMatrix = RealMatrix.Eye(t_Matrix.Height, Math.Min(t_Matrix.Width,t_Matrix.Height));
            RealMatrix r_PMatrix = RealMatrix.Eye(t_Matrix.Height, t_Matrix.Height);

            int r_Swaps = 0;
            int currentRow = 1;
            int currentColumn = 1;

            while (currentRow <= r_RMatrix.Height && currentColumn <= r_RMatrix.Width)
            {
                int max_index = currentRow;
                double max_value = r_RMatrix[currentRow, currentColumn];

                for(int index = currentRow; index <= r_RMatrix.Height; ++index)
                {
                    if (Math.Abs(r_RMatrix[index,currentColumn]) > max_value)
                    {
                        max_index = index;
                        max_value = Math.Abs(r_RMatrix[index, currentColumn]);
                    }
                }

                if (max_value < Math.Pow(10, -Precision))
                {
                    ++currentColumn;
                    continue;
                }    

                if(currentRow != max_index)
                {
                    ++r_Swaps;

                    r_LMatrix = SwapRows(r_LMatrix, currentRow, max_index, 1, currentColumn - 1);
                    r_RMatrix = SwapRows(r_RMatrix, currentRow, max_index, 1, r_RMatrix.Width);
                    r_PMatrix = SwapRows(r_PMatrix, currentRow, max_index, 1, r_PMatrix.Width);
                }

                for (int i = currentRow+1; i <= r_RMatrix.Height; ++i)
                {
                    double coefficient = r_RMatrix[i, currentColumn] / r_RMatrix[currentRow, currentColumn];

                    r_LMatrix[i, currentColumn] = coefficient;
                    r_RMatrix[i, currentColumn] = 0;

                    for(int j = currentColumn+1; j <= r_RMatrix.Width; ++j)
                    {
                        r_RMatrix[i, j] = r_RMatrix[i, j] - r_RMatrix[currentRow, j] * coefficient;
                    }
                }
                ++currentRow;
                ++currentColumn;
            }

            return (r_LMatrix,r_RMatrix.SubMatrix(1,Math.Min(t_Matrix.Width,t_Matrix.Height),1,t_Matrix.Width), r_PMatrix, r_Swaps);
        }

        public static (RealMatrix, RealMatrix, RealMatrix) LU(RealMatrix t_Matrix)
        {
            var (L,U,P,_) = InternalLUP(t_Matrix);
            return (L,U,P);
        }

        public static double Determinant(RealMatrix t_Matrix)
        {
            if (t_Matrix.Height != t_Matrix.Width)
            {
                throw new RankException("Error: Matrix is not square!");
            }

            var (L, U, P, S) = InternalLUP(t_Matrix);

            double LDeterminant = 1;
            double UDeterminant = 1;

            double sign = (-2) * (S % 2) + 1;

            // Can be collapsed into one loop, keeping for clarity for now
            for(int i = 1; i <= L.Height; ++i)
            {
                LDeterminant *= L[i, i];
            }

            for (int i = 1; i <= U.Height; ++i)
            {
                UDeterminant *= U[i, i];
            }

            if (Precision == 16)
                return sign * LDeterminant * UDeterminant;
            return Math.Round(sign * LDeterminant * UDeterminant, Precision);
        }

        public static double[] Eigenvalues(RealMatrix t_Matrix)
        {
            RealMatrix h_Matrix = Hessenberg(t_Matrix);
            RealMatrix? tri_Matrix = Triangular(h_Matrix);

            if(tri_Matrix is null)
            {
                return [];
            }

            var eigs = new double[tri_Matrix.Width];

            for (int i = 1; i <= tri_Matrix.Width; ++i)
            {
                if (Precision == 16)
                {
                    eigs[i - 1] = tri_Matrix[i, i];
                }
                else
                {
                    eigs[i - 1] = Math.Round(tri_Matrix[i, i], Precision);
                }
            }

            return eigs;
        } 
    }
}
