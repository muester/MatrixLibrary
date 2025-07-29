using System;
using System.Collections.Generic;
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

                //TODO: Fix zero bug
                double alpha = -Math.Sign(x[1, 1]) * x.Norm;

                RealMatrix v = x - RealMatrix.Elementary(r_Matrix.Height - c + 1, 1).Modify((a, b) => a * b, alpha);

                if(v.Norm != 0)
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

        public static (RealMatrix,RealMatrix) Givens(RealMatrix t_Matrix)
        {

            double eps = Math.Pow(10, -10);

            RealMatrix r_Matrix = ~t_Matrix;
            RealMatrix Q_Matrix = RealMatrix.Eye(t_Matrix.Height, t_Matrix.Height);

            for (int c = 1; c < r_Matrix.Width; ++c)
            {
                for(int r = c+1; r <= r_Matrix.Height; ++r)
                {
                    if (Math.Abs(r_Matrix[r,c]) > eps)
                    {
                        RealMatrix g_Matrix = RealMatrix.Eye(r_Matrix.Height, r_Matrix.Height);

                        var re = Math.Sqrt(Math.Pow(r_Matrix[c,c],2) + Math.Pow(r_Matrix[r,c],2));
                        var ce = r_Matrix[c, c] / re;
                        var se = -r_Matrix[r, c] / re;

                        g_Matrix[c, c] = ce;
                        g_Matrix[r, r] = ce;
                        g_Matrix[r, c] = se;
                        g_Matrix[c, r] = -se;

                        r_Matrix = g_Matrix*r_Matrix;

                        Q_Matrix *= g_Matrix.Transpose();
                    }
                }

            }
            return (Q_Matrix, r_Matrix);
        }

        public static RealMatrix Hessenberg(RealMatrix t_Matrix)
        {
            // TODO: Check square matrix

            RealMatrix r_Matrix =~ t_Matrix;

            for (int s = 1; s <= t_Matrix.Height - 2; ++s)
            {

                RealMatrix t_Column = t_Matrix.SubMatrix(1+s, t_Matrix.Height, s, s);

                int sign = 1;

                if (t_Column[1, 1] <= 0)
                    sign = -1;

                RealMatrix t_Projector = RealMatrix.Elementary(t_Column.Height, 1);
                if(t_Column.Norm != 0)
                    t_Projector =  t_Projector.Modify((a, b) => (a * b), t_Column.Norm * sign);

                t_Projector += t_Column;

                t_Projector.Modify((a, b) => a / b, t_Projector.Norm);

                RealMatrix t_Householder = RealMatrix.Eye(t_Column.Height, t_Column.Height);
                t_Householder -= (t_Projector * t_Projector.Transpose()).Modify((a, b) => (a * b), 2 / (t_Projector.Norm * t_Projector.Norm));

                RealMatrix Q_Matrix = RealMatrix.Eye(t_Matrix.Height, t_Matrix.Width).Insert(1 + s, 1 + s, t_Householder);

                t_Matrix = Q_Matrix * t_Matrix * Q_Matrix.Transpose();
            }

            t_Matrix = t_Matrix.Modify((a, b) => (double)Math.Round((decimal)a, (int)b), Precision);

            return t_Matrix;
        }

        public static RealMatrix Triangular(RealMatrix t_Matrix)
        {
            //TODO: Implement GIVENS ROTATIONS to save MAJOR COMPUTATIONAL power
            //TODO: FIX PRECISION BUG

            RealMatrix o_Matrix = ~t_Matrix;

            double eps = Math.Pow(10, -10);

            int iteration = 0;
            double error = 0;
            double lasterror;

            bool repeat = true;

            while(repeat)
            {
                ++iteration;
                lasterror = error;
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
                
                if(iteration > 3 && (lasterror-error) <= 0)
                {
                    Console.WriteLine("COMPLEX EIGENVALUES PLACEHOLDER");
                    return o_Matrix;
                }    

                var (Q, R) = Givens(t_Matrix);
                t_Matrix = R * Q;
            }
            return t_Matrix;
        }

      
        public static List<double> Eigenvalues (RealMatrix t_Matrix)
        {
            RealMatrix h_Matrix = Hessenberg(t_Matrix);
            RealMatrix tri_Matrix = Triangular(h_Matrix);

            var eigs = new List<double>();

            for (int i = 1; i <= tri_Matrix.Width; ++i)
            {
                eigs.Add(Math.Round(tri_Matrix[i, i],Precision));
            }

            return eigs;
        }

        public static RealMatrix BackwardTriangle(RealMatrix t_Matrix)
        {
            //TODO: squaaaaaare

            RealMatrix r_Matrix = RealMatrix.Zeros(t_Matrix.Height, t_Matrix.Width);

            for (int c = 1; c <= r_Matrix.Width; ++c)
            {
                for (int r = r_Matrix.Height; r >= 1; --r)
                {
                    double Kronecker = (c == r ? 1 : 0);

                    double res = Kronecker;

                    for (int o = r + 1; o <= r_Matrix.Height; ++o)
                    {
                        res -= t_Matrix[r, o] * r_Matrix[o, c];
                    }

                    r_Matrix[r, c] = res / t_Matrix[r,r];
                }
            }

            return r_Matrix;
        }

        public static RealMatrix ForwardTriangle(RealMatrix t_Matrix)
        {
            //TODO: squaaaaaare

            RealMatrix r_Matrix = RealMatrix.Zeros(t_Matrix.Height, t_Matrix.Width);

            for (int c = 1; c <= r_Matrix.Width; ++c)
            {
                for (int r = 1; r <= r_Matrix.Height; ++r)
                {
                    double Kronecker = (c == r ? 1 : 0);

                    double res = Kronecker;

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
            var (L, U, P) = Algorithms.LU(t_Matrix);

            var r_Matrix = BackwardTriangle(U) * ForwardTriangle(L) * P;

            return r_Matrix;
        }

        public static RealMatrix LinSys(RealMatrix t_Matrix, RealMatrix b_Matrix)
        {
            return Inverse(t_Matrix) * b_Matrix;
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
            var (L,U,P,_) = InternalLUP(t_Matrix);
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
