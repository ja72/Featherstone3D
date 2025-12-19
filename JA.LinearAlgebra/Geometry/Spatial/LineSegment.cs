using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static System.Math;

namespace JA.Geometry.Spatial
{
    public readonly struct LineSegment
    {
        public LineSegment(Point3 a, Point3 b)
        {
            A=a;
            B=b;            
        }

        public Point3 A { get; }
        public Point3 B { get; }
        public float Length { get => Point3.Distance(A, B); }

        public Vector3 Direction { get => Point3.Direction(A, B);  }

        public LineSegment Flip() => new LineSegment(B, A);

        public Point3 GetPointAlong(float along)
            => Point3.Lerp(A, B, along);


        public float ProjectPointAlong(Point3 point)
        {
            Vector3 a = A.Position;
            Vector3 b = B.Position;
            float aa = Vector3.Dot(a, a), bb=Vector3.Dot(b, b), ab = Vector3.Dot(a, b);

            return Min(1,Max(0,(aa-ab+Vector3.Dot(B-A, point.Position))/(aa+bb-2*ab)));
        }

        public Point3 ClosestPointTo(Point3 point)=> GetPointAlong(ProjectPointAlong(point));

        public float DistanceTo(Point3 point) => Point3.Distance(point, ClosestPointTo(point));

        #region Algebra
        public LineSegment Offset(Vector3 delta)
        {
            return new LineSegment(A+delta, B+delta);
        }

        public LineSegment Scale(float factor)
        {
            return new LineSegment(factor*A, factor*B);
        }

        public LineSegment Transform(Matrix4x4 transform)
        {
            return new LineSegment(
                Point3.Transform(A, transform), 
                Point3.Transform(B, transform));
        }
        public LineSegment Rotate(Quaternion rotation)
        {
            return new LineSegment(
                Point3.Transform( A, rotation), 
                Point3.Transform( B, rotation));
        }
        public LineSegment Rotate(Quaternion rotation, Point3 pivot)
        {
            return new LineSegment(
                pivot + Vector3.Transform( A - pivot, rotation), 
                pivot + Vector3.Transform( B - pivot, rotation));
        }
        public LineSegment Reflect(Vector3 normal) => Reflect(normal, Point3.Origin);
        public LineSegment Reflect(Vector3 normal, Point3 origin)
            => new LineSegment(
                origin + Vector3.Reflect(A - origin, normal),
                origin + Vector3.Reflect(B - origin, normal));
        #endregion

        #region Operators
        public static LineSegment operator +(LineSegment a, Vector3 b) => a.Offset(b);
        public static LineSegment operator -(LineSegment a) => a.Scale(-1);
        public static LineSegment operator -(LineSegment a, Vector3 b) => a.Offset(-b);
        public static LineSegment operator *(float a, LineSegment b) => b.Scale(a);
        public static LineSegment operator *(LineSegment a, float b) => a.Scale(b);
        public static LineSegment operator /(LineSegment a, float b) => a.Scale(1 / b);
        #endregion



    }
}
