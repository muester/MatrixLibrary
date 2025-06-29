using System;
namespace MatrixLib
{
    public static class Algorithms
    {
        public static int Precision = 16;
        public static (RealMatrix, RealMatrix) QR(RealMatrix t_Matrix)
        {
            RealMatrix r_Matrix = ~t_Matrix;
            RealMatrix Q_Matrix = RealMatrix.Eye(t_Matrix.Height,t_Matrix.Height);

            for(int c = 1; c <= Math.Min(t_Matrix.Width,t_Matrix.Height); ++c)
            {
                RealMatrix x = r_Matrix.SubMatrix(c, r_Matrix.Height, c, c);

                double alpha = -Math.Sign(x[1, 1]) * x.Norm;

                RealMatrix v = x - RealMatrix.Elementary(r_Matrix.Height - c + 1, 1).Modify((a, b) => a * b, alpha);

                v = v.Modify((a, b) => a / b, v.Norm);

                RealMatrix vT = v.Transpose();

                RealMatrix Hc = RealMatrix.Eye(r_Matrix.Height - c + 1, r_Matrix.Height - c + 1) - (v * vT).Modify((a, b) => a * b, 2);

                RealMatrix HTc = RealMatrix.Eye(r_Matrix.Height, r_Matrix.Height).Insert(c, c, Hc);

                Q_Matrix = Q_Matrix * HTc;
                r_Matrix = HTc * r_Matrix;
            }

            Q_Matrix = Q_Matrix.Modify((a,b) => (double)Math.Round((decimal)a, (int)b), Precision);
            r_Matrix = r_Matrix.Modify((a,b) => (double)Math.Round((decimal)a, (int)b), Precision);

            Q_Matrix = Q_Matrix.SubMatrix(1, Q_Matrix.Height, 1, Math.Min(t_Matrix.Width,Q_Matrix.Width));
            r_Matrix = r_Matrix.SubMatrix(1, Math.Min(t_Matrix.Width, t_Matrix.Height), 1, Math.Min(t_Matrix.Width,r_Matrix.Width));

            return (Q_Matrix, r_Matrix);
        }
        
        // TODO: Fix precision
        internal static (RealMatrix, RealMatrix, RealMatrix, int) InternalLUP(RealMatrix t_Matrix)
        {
            RealMatrix r_Matrix = t_Matrix.SubMatrix(1,t_Matrix.Height,1,t_Matrix.Width);
            RealMatrix L_Matrix = RealMatrix.Eye(t_Matrix.Height, Math.Min(t_Matrix.Width,t_Matrix.Height));
            RealMatrix P_Matrix = RealMatrix.Eye(t_Matrix.Height, t_Matrix.Height);

            int Swaps = 0;
            int h = 1;
            int k = 1;

            while (h <= r_Matrix.Height && k <= r_Matrix.Width)
            {
                int i_max = h;
                double max_value = r_Matrix[h, k];

                for(int i = h; i <= r_Matrix.Height; ++i)
                {
                    if (Math.Abs(r_Matrix[i,k]) > max_value)
                    {
                        i_max = i;
                        max_value = Math.Abs(r_Matrix[i, k]);
                    }
                }

                if (max_value == 0)
                {
                    ++k;
                    continue;
                }    

                if(h != i_max)
                {
                    ++Swaps;

                    RealMatrix TopRow = r_Matrix.SubMatrix(h, h, 1, r_Matrix.Width);
                    RealMatrix BestRow = r_Matrix.SubMatrix(i_max, i_max, 1, r_Matrix.Width);

                    RealMatrix LTopRow = L_Matrix.SubMatrix(h, h, 1, k - 1);
                    RealMatrix LBestRow = L_Matrix.SubMatrix(i_max, i_max, 1, k - 1);

                    RealMatrix PTopRow = P_Matrix.SubMatrix(h, h, 1, P_Matrix.Width);
                    RealMatrix PBestRow = P_Matrix.SubMatrix(i_max, i_max, 1, P_Matrix.Width);

                    r_Matrix = r_Matrix.Insert(h, 1, BestRow);
                    r_Matrix = r_Matrix.Insert(i_max, 1, TopRow);

                    L_Matrix = L_Matrix.Insert(h, 1, LBestRow);
                    L_Matrix = L_Matrix.Insert(i_max, 1, LTopRow);

                    P_Matrix = P_Matrix.Insert(h, 1, PBestRow);
                    P_Matrix = P_Matrix.Insert(i_max, 1, PTopRow);
                }


                for (int i = h+1; i <= Math.Min(r_Matrix.Width,r_Matrix.Height); ++i)
                {
                    double f = r_Matrix[i, k] / r_Matrix[h, k];

                    L_Matrix[i, k] = f;
                    r_Matrix[i, k] = 0;

                    for(int j = k+1; j <= r_Matrix.Width; ++j)
                    {
                        r_Matrix[i, j] = r_Matrix[i, j] - r_Matrix[h, j] * f;
                    }
                }
                ++h;
                ++k;
            }

            return (L_Matrix,r_Matrix, P_Matrix, Swaps);
        }

        public static (RealMatrix, RealMatrix, RealMatrix) LU(RealMatrix t_Matrix)
        {
            var (L,U,P,S) = InternalLUP(t_Matrix);
            return (L,U,P);
        }

        //TODO: CHECK THE MATRIX IS INDEED SQUARE
        internal static double Determinant(RealMatrix t_Matrix)
        {
            var (L, U, P, S) = InternalLUP(t_Matrix);

            double L_Determinant = 1;
            double U_Determinant = 1;

            double Sign = (-2) * (S % 2) + 1;

            // Can be collapsed into one loop, keeping for clarity for now
            for(int i = 1; i <= L.Height; ++i)
            {
                L_Determinant *= L[i, i];
            }

            for (int i = 1; i <= U.Height; ++i)
            {
                U_Determinant *= U[i, i];
            }

            return Math.Round(Sign * L_Determinant * U_Determinant,Precision);
        }
    }
}
