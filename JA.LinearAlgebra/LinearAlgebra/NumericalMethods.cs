using System;
using System.Diagnostics;
using static System.Math;

namespace JA.LinearAlgebra
{
    public enum SolutionSet
    {
        First,
        Second,
    }

    public static class NumericalMethods
    {
        public static int MaxIterations { get; } = 512;
        public static float LooseTolerance { get; } = 1e-6f;

        public static float UlpFloat { get; } = 1f/8388608;             //2^-23
        public static double UlpDouble { get; } = 1d/2251799813685248L; //2^-51
        public static double UlpTrig { get; } = 1d/68719476736L;        //2^-36

        #region Single
        public static float Eps(float magnitude)
        {
            magnitude=Abs(magnitude);
            float x = 1;
            while (magnitude + x != magnitude)
                x /= 2;
            return x*2;
        }

        public static bool GaussPointIteration(this Func<float, float> f, float x_init, float tol, out float x)
        {
            x = x_init;
            Debug.WriteLine(f.ToString());
            float x_old;
            int iter = 0;
            do
            {
                iter++;
                x_old = x;
                x = f(x);
                Debug.WriteLine($"iter={iter}, x={x}");
            } while (iter < MaxIterations &&  Abs(x-x_old)>tol);

            return iter < MaxIterations;
        }
        public static bool BisectionRoot(this Func<float, float> f, float x_low, float x_high, float tol, out float x)
        {
            float f_low = f(x_low), f_high = f(x_high);

            if (Abs(f_low)<=tol)
            {
                x = x_low;
                return true;
            }

            if (Abs(f_high)<=tol)
            {
                x = x_high;
                return true;
            }

            int iter = 0;

            while (iter <= MaxIterations && f_low*f_high > 0)
            {
                iter++;
                (x_low, x_high) =((3*x_low-x_high)/2, (3*x_high-x_low)/2);
                (f_low, f_high) =(f(x_low), f(x_high));
            }

            if (iter>MaxIterations)
                throw new ArgumentException("Invalid Initial Conditions for Bisection.");

            iter = 0;
            float f_mid;
            do
            {
                iter++;
                x = (x_low + x_high)/2;
                f_mid = f(x);
                if (Abs(f_mid) <= tol)
                {
                    return true;
                }

                if (f_low*f_mid<0)
                {
                    x_high = x;
                    f_high = f_mid;
                }
                else if (f_high*f_mid<0)
                {
                    x_low = x;
                    f_low = f_mid;
                }
                else
                    throw new InvalidOperationException();

            } while (iter < MaxIterations && Abs(x_high-x_low)>2*tol);

            return iter < MaxIterations;
        }
        #endregion

        #region Double

        public static double Eps(double magnitude)
        {
            magnitude=Abs(magnitude);
            double x = 1;
            while (magnitude + x != magnitude)
                x /= 2;
            return x*2;
        }

        public static double CubicRoot(this double x)
        {
            return x >= 0 ? Pow(x, 1.0 / 3.0) : -Pow(-x, 1.0 / 3.0);
        }
        /// <summary>
        /// Solves the cubic <code>x^3 + a*x^2 + b*x + c = 0</code>
        /// </summary>
        /// <param name="roots">The roots of the cubic</param>
        public static bool SolveCubic(double a, double b, double c, out double[] roots)
        {
            //Solves x^3 + a*x^2 + b*x + c = 0
            double p = b - a * a / 3;
            double q = 2 * a * a * a / 27 - a * b / 3 + c;
            double discriminant = q * q / 4 + p * p * p / 27;
            if (discriminant > 0)
            {
                double u = CubicRoot(-q / 2 + Sqrt(discriminant));
                double v = CubicRoot(-q / 2 - Sqrt(discriminant));
                roots = new double[] { u + v - a / 3 };
                return false;
            }
            else if (Abs(discriminant) < UlpDouble)
            {
                double u = CubicRoot(-q / 2);
                roots = new double[] { 2 * u - a / 3, -u - a / 3 };
                return true;
            }
            else
            {
                double r = Sqrt(-p * p * p / 27);
                double phi = Acos(-q / (2 * r));
                double t = 2 * CubicRoot(r);
                roots = new double[]
                {
                    t * Cos(phi / 3) - a / 3,
                    t * Cos((phi + 2 * PI) / 3) - a / 3,
                    t * Cos((phi + 4 * PI) / 3) - a / 3
                };
                return true;
            }
        }

        public static bool GaussPointIteration(this Func<double, double> f, double x_init, double tol, out double x)
        {
            x = x_init;
            double x_old;
            int iter = 0;
            do
            {
                iter++;
                x_old = x;
                x = f(x);
            } while (iter < MaxIterations &&  Abs(x-x_old)>tol);

            return iter < MaxIterations;
        }
        public static bool BisectionRoot(this Func<double, double> f, double x_low, double x_high, double tol, out double x)
        {
            double f_low = f(x_low), f_high = f(x_high);

            if (Abs(f_low)<=tol)
            {
                x = x_low;
                return true;
            }

            if (Abs(f_high)<=tol)
            {
                x = x_high;
                return true;
            }

            int iter = 0;

            while (iter <= MaxIterations && f_low*f_high > 0)
            {
                iter++;
                (x_low, x_high) =((3*x_low-x_high)/2, (3*x_high-x_low)/2);
                (f_low, f_high) =(f(x_low), f(x_high));
            }

            if (iter>MaxIterations)
                throw new ArgumentException("Invalid Initial Conditions for Bisection.");

            iter = 0;
            double f_mid;
            do
            {
                iter++;
                x = (x_low + x_high)/2;
                f_mid = f(x);
                if (Abs(f_mid) <= tol)
                {
                    return true;
                }

                if (f_low*f_mid<0)
                {
                    x_high = x;
                    f_high = f_mid;
                }
                else if (f_high*f_mid<0)
                {
                    x_low = x;
                    f_low = f_mid;
                }
                else
                    throw new InvalidOperationException();

            } while (iter < MaxIterations && Abs(x_high-x_low)>2*tol);

            return iter < MaxIterations;
        }
        #endregion
    }
}
