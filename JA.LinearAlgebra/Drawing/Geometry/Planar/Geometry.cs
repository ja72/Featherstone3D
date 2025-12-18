using System;
using System.Numerics;

using static System.Math;

namespace JA.Drawing.Geometry.Planar
{
    public enum CornerSolution
    {
        Inside,
        Opposing,
        Outside1,
        Outside2,
    }


    public static class Geometry
    {
        public static Vector2 Unit(this Vector2 vector)
            => Vector2.Normalize(vector);

        public static (float r, float θ) ToPolar(this Vector2 vector)
        {
            var r = vector.Length();
            var θ = (float)Atan2(vector.Y, vector.X);
            return (r, θ);
        }
        public static Vector2 Polar(float radius, float angle)
            => new Vector2((float)Cos(angle)*radius, (float)Sin(angle)*radius);
        public static Vector2 Elliptical(float majorRadius, float minorRadius, float paramAngle)
        {
#if ELLIPSE_POLAR_COORD
            double a = majorRadius;
            double b = minorRadius;
            double cos = Cos(paramAngle), sin = Sin(paramAngle);
            double denom = Sqrt(b*b*cos*cos + a*a*sin*sin);
            double x = (a*b*cos)/denom;
            double y = (a*b*sin)/denom;
            return new Vector2((float)x, (float)y);
#else
            double a = majorRadius;
            double b = minorRadius;
            double cos = Cos(paramAngle), sin = Sin(paramAngle);
            double x = a * cos, y = b*sin;
            return new Vector2((float)x, (float)y);
#endif
        }

        public static Point2 TanTanRadius(this Point2 apex, Point2 side1, Point2 side2, float radius, CornerSolution corner = CornerSolution.Inside)
        {
            var edge1 = Line2.Join(apex, side1);
            var edge2 = Line2.Join(apex, side2);

            int s1 = 0, s2 = 0;

            switch (corner)
            {
                case CornerSolution.Inside:
                    s1 = 1;
                    s2 = -1;
                    break;
                case CornerSolution.Opposing:
                    s1 = -1;
                    s2 = 1;
                    break;
                case CornerSolution.Outside1:
                    s1 = 1;
                    s2 = 1;
                    break;
                case CornerSolution.Outside2:
                    s1 = -1;
                    s2 = -1;
                    break;
            }

            var offset1 = edge1.Offset(s1*radius);
            var offset2 = edge2.Offset(s2*radius);

            return Point2.Meet(offset1, offset2);
        }
        public static bool RadicalLine(this Circle2 circle1, Circle2 circle2, out Line2 intersection)
        {

            float x_1 = circle1.Center.U;
            float y_1 = circle1.Center.V;
            float w_1 = circle1.Center.W;
            float x_2 = circle2.Center.U;
            float y_2 = circle2.Center.V;
            float w_2 = circle2.Center.W;

            float r_1_sq = circle1.Radius * circle1.Radius;
            float r_2_sq = circle2.Radius * circle2.Radius;
            float x_1_sq = x_1 * x_1, y_1_sq = y_1 * y_1, w_1_sq = w_1 * w_1;
            float x_2_sq = x_2 * x_2, y_2_sq = y_2 * y_2, w_2_sq = w_2 * w_2;

            float a = 2*w_2*x_1-2*w_1*x_2;
            float b = 2*w_2*y_1-2*w_1*y_2;
            float c = r_1_sq*w_1*w_2-r_2_sq*w_1*w_2+(w_1_sq*(x_2_sq+y_2_sq)-w_2_sq*(x_1_sq+y_1_sq))/(w_1*w_2);

            intersection=new Line2(a, b, c);

            return true;
        }


    }

}
