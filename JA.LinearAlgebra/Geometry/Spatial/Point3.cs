using System;
using System.Globalization;
using System.Runtime.CompilerServices;

using static System.Math;

using Vector3 = System.Numerics.Vector3;
using Quaternion3 = System.Numerics.Quaternion;
using System.Numerics;

namespace JA.Geometry.Spatial
{
    public readonly struct Point3 : IEquatable<Point3>
    {
        private readonly (Vector3 position, float w) data;

        /// <summary>Creates a <see cref="T:System.Numerics.Point" /> object from the X, Y, and Z components of its normal, and its distance from the origin on that normal.</summary>
        /// <param name="x">The X component of the position.</param>
        /// <param name="y">The Y component of the position.</param>
        /// <param name="z">The Z component of the position.</param>
        /// <param name="w">The weight of the point.</param>
        public Point3(float x, float y, float z, float w)
        {
            if (w>0)
            {
                x /= w;
                y /= w;
                z /= w;
            }
            data =(new Vector3(x, y, z), w);
        }

        /// <summary>Creates a <see cref="T:System.Numerics.Point" /> object from a specified normal and the distance along the normal from the origin.</summary>
        /// <param name="vector">The point's position vector.</param>
        /// <param name="weight">The weight of the point.</param>
        public Point3(Vector3 vector, float weight)
        {
            if (weight>0)
            {
                vector /= weight;
            }
            data = (vector, weight);
        }

        /// <summary>Creates a <see cref="T:System.Numerics.Point" /> object from a specified four-dimensional vector.</summary>
        /// <param name="value">A vector whose first three elements describe the position vector, and whose <see cref="F:System.Numerics.Vector4.W" /> defines the weight of the point.</param>
        public Point3(Vector4 value)
        {
            float w = value.W;
            if (w>0)
            {
                value /= w;
            }
            data = (new Vector3(value.X, value.Y, value.Z), w);
        }
        public static Point3 FromPosition(float x, float y, float z)
            => new Point3(x, y, z, 1);
        public static Point3 FromPosition(Vector3 position)
            => new Point3(position.X, position.Y, position.Z, 1);
        public static Point3 Lerp(Point3 a, Point3 b, float along)
        {
            Point3 p_a = ( 1-along )*a;
            Point3 p_b = along*b;
            return p_a + p_b;
        }
        public static Point3 Empty { get; } = new Point3(Vector3.Zero, 0);
        public static Point3 Origin { get; } = new Point3(Vector3.Zero, 1);
        public static Point3 InfX { get; } = new Point3(Vector3.UnitX, 0);
        public static Point3 InfY { get; } = new Point3(Vector3.UnitY, 0);
        public static Point3 InfZ { get; } = new Point3(Vector3.UnitZ, 0);

        /// <summary>The vector part of the point.</summary>
        public Vector3 Vector => data.position;
        /// <summary>The weight of the point.</summary>
        public float W => data.w;
        public bool IsFinite => data.w*data.w > 0;

        /// <summary>The position vector of the point.</summary>
        public Vector3 Position => Vector/W;

        /// <summary>Creates a new <see cref="T:System.Numerics.Point" /> object whose normal vector is the source point's normal vector normalized.</summary>
        /// <param name="value">The source point.</param>
        /// <returns>The normalized point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3 Normalize(Point3 value)
        {
            const float tolsq = 1.1920929E-07f;
            float w2 = value.W*value.W;
            if (Abs(w2 - 1f) < tolsq)
            {
                return value;
            }
            return new Point3(value.Vector / value.W, 1f);
        }
        /// <summary>
        /// Direction vector from point A to point B
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Direction(Point3 A, Point3 B) => Vector3.Normalize(B-A);
        /// <summary>Transforms a normalized point by a Quaternion rotation.</summary>
        /// <param name="point">The normalized point to transform.</param>
        /// <param name="rotation">The Quaternion rotation to apply to the point.</param>
        /// <returns>A new point that results from applying the Quaternion rotation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3 Transform(Point3 point, Quaternion3 rotation) 
            => new Point3(Vector3.Transform(point.Vector, rotation), point.W);
        public static Point3 Transform(Point3 point, Matrix4x4 transform)
        {
            var v4 = new Vector4(point.Vector, point.W);
            v4 = Vector4.Transform(v4, transform);
            return new Point3(new Vector3(v4.X, v4.Y, v4.Z), v4.W);
        }
        #region Geometric Algebra

        /// <summary>
        /// Implements the join operator.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="line">The line.</param>
        /// <returns>The plane joining a point and a line.</returns>
        public static Plane3 operator &(Point3 point, Line3 line)
          => Plane3.FromLineAndPoint(line, point);

        /// <summary>
        /// Implements the join operator.
        /// </summary>
        public static Line3 operator &(Point3 point1, Point3 point2)
            => Line3.FromTwoPoints(point1, point2);

        public static float operator *(Point3 point, Plane3 plane)
            => Vector3.Dot(point.Vector, plane.Normal) + point.W*plane.D;

        public static float operator *(Point3 point, Line3 line)
            => line * point;

        #endregion

        #region Formatting
        /// <summary>Returns the string representation of this point object.</summary>
        public override string ToString()
        {
            return $"{{Vector:{Vector} W:{W}}}";
        }
        #endregion

        #region Comparisons
        /// <summary>Returns a value that indicates whether two planes are equal.</summary>
        /// <param name="value1">The first point to compare.</param>
        /// <param name="value2">The second point to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool operator ==(Point3 value1, Point3 value2)
        {
            if (value1.Vector.X == value2.Vector.X && value1.Vector.Y == value2.Vector.Y && value1.Vector.Z == value2.Vector.Z)
            {
                return value1.W == value2.W;
            }
            return false;
        }

        /// <summary>Returns a value that indicates whether two planes are not equal.</summary>
        /// <param name="value1">The first point to compare.</param>
        /// <param name="value2">The second point to compare.</param>
        /// <returns>
        ///   <see langword="true" /> if <paramref name="value1" /> and <paramref name="value2" /> are not equal; otherwise, <see langword="false" />.</returns>
        public static bool operator !=(Point3 value1, Point3 value2)
        {
            if (value1.Vector.X == value2.Vector.X && value1.Vector.Y == value2.Vector.Y && value1.Vector.Z == value2.Vector.Z)
            {
                return value1.W != value2.W;
            }
            return true;
        }

        /// <summary>Returns a value that indicates whether this instance and another point object are equal.</summary>
        /// <param name="other">The other point.</param>
        /// <returns>
        ///   <see langword="true" /> if the two planes are equal; otherwise, <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Point3 other)
        {
            return data.Equals(other.data);
        }

        /// <summary>Returns a value that indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        ///   <see langword="true" /> if the current instance and <paramref name="obj" /> are equal; otherwise, <see langword="false" />. If <paramref name="obj" /> is <see langword="null" />, the method returns <see langword="false" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (obj is Point3 point)
            {
                return Equals(point);
            }
            return false;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hc = -1817952719;
                hc = -1521134295*hc + data.GetHashCode();
                return hc;
            }
        }

        #endregion

        #region Algebra
        public static Point3 Offset(Point3 a, Vector3 dir)
            => new Point3(
                a.Vector + dir * a.W,
                a.W);
        public static Vector3 Delta(Point3 a, Point3 b)
            => a.Vector / a.W - b.Vector / b.W;
        public static Point3 Negate(Point3 a) => Scale(-1, a);
        public static Point3 Scale(float factor, Point3 a)
            => new Point3(
                factor*a.Vector,
                factor*a.W);
        public static Point3 Add(Point3 a, Point3 b)
            => new Point3(
                a.Vector+b.Vector,
                a.W+b.W);
        public static Point3 operator +(Point3 a, Vector3 d) => Offset(a, d);
        public static Point3 operator -(Point3 a, Vector3 d) => Offset(a, -d);
        public static Vector3 operator -(Point3 a, Point3 b) => Delta(a, b);
        public static Point3 operator-(Vector3 a, Point3 b)  => FromPosition( a - b.Position );
        public static Point3 operator +(Point3 a, Point3 b) => Add(a, b);
        public static Point3 operator -(Point3 a) => Negate(a);
        public static Point3 operator *(float f, Point3 a) => Scale(f, a);
        public static Point3 operator *(Point3 a, float f) => Scale(f, a);
        public static Point3 operator /(Point3 a, float d) => Scale(1/d, a);
        #endregion

        #region Geometry        
        /// <summary>Normalize the point and return it as the geometric center.</summary>
        public Point3 Center => Normalize(this);

        public static Point3 FromThreePlanes(Plane3 A, Plane3 B, Plane3 C)
        {
            return A ^ B ^ C;
        }
        public Point3 Reflect(Vector3 normal, Point3 origin)
            => origin + Vector3.Reflect(this - origin, normal);

        public float DistanceFromOrigin()
            => Vector.Length()/Abs(W);

        public static Point3 FromLineAndPlane(Line3 line, Plane3 plane)
            => new Point3(
                Vector3.Cross(plane.Normal, line.Moment) - plane.D * line.Vector, 
                Vector3.Dot(plane.Normal, line.Vector));

        public float DistanceTo(Plane3 plane)
            => Abs(Vector3.Dot(plane.Normal, Vector) + plane.D*W)
            /( W*plane.Normal ).Length();

        public float DistanceTo(Line3 line) => line.DistanceTo(this);
        public static float Distance(Point3 point, Point3 center) => Delta(point, center).Length();

        #endregion

    }
}
