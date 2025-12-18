using System;
using System.Drawing;
using System.Linq;

using static System.Math;

namespace JA.Drawing
{
    using Vector2 = System.Numerics.Vector2;

    using JA.Drawing.Geometry.Planar;

    public readonly struct Frame
    {
        Frame(Size target, PointF origin, float scale)
        {
            Target=target;
            Scale=scale;
            Origin=origin;
        }
        public static Frame Empty { get; } = new Frame(Size.Empty, PointF.Empty, 0f);
        public static Frame CenteredWithSize(Size target, float size)
        {
            return RelativeOriginWithSize(target, size, 0.5f, 0.5f);
        }
        public static Frame CenteredWithScale(Size target, float scale)
        {
            return RelativeOriginWithScale(target, scale, 0.5f, 0.5f);
        }
        public static Frame RelativeOriginWithSize(Size target, float size, float relx, float rely)
        {
            var orign = new PointF(target.Width  * relx, target.Height * rely);
            return AtOriginWithSize(target, orign, size);
        }
        public static Frame RelativeOriginWithScale(Size target, float scale, float relx, float rely)
        {
            var orign = new PointF(target.Width  * relx, target.Height * rely);
            return AtOriginWithScale(target, orign, scale);
        }
        public static Frame AtOriginWithSize(Size target, PointF origin, float size)
        {
            if (size<=0)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be positive.");
            int pixels = Max(1, Min(target.Height, target.Width));
            float scale = pixels/size;
            return AtOriginWithScale(target, origin, scale);
        }
        public static Frame AtOriginWithScale(Size target, PointF origin, float scale)
        {
            return new Frame(target, origin, scale);
        }

        public Size Target { get; }
        public float Scale { get; }
        public PointF Origin { get; }

        public Point2 TopLeft => Point2.FromVector(
            GetVector(new PointF(0, 0)));
        public Point2 TopRight => Point2.FromVector(
            GetVector(new PointF(Target.Width-1, 0)));
        public Point2 BottomLeft => Point2.FromVector(
            GetVector(new PointF(0, Target.Height-1)));
        public Point2 BottomRight => Point2.FromVector(
            GetVector(new PointF(Target.Width-1, Target.Height-1)));

        #region Conversions
        public PointF GetPixel(Point2 point)
            => GetPixel(point.AsVector());

        public PointF GetPixel(Vector2 coordinate)
        {
            var origin = Origin;

            return new PointF(
                origin.X+Scale*coordinate.X,
                origin.Y-Scale*coordinate.Y);
        }

        public PointF[] GetPixels(params Point2[] points)
            => GetPixels(points.Select(pt => pt.AsVector()).ToArray());

        public PointF[] GetPixels(params Vector2[] coordinates)
        {
            var origin = Origin;

            var points = new PointF[coordinates.Length];
            for (var i = 0; i<points.Length; i++)
            {
                points[i]=new PointF(
                origin.X+Scale*coordinates[i].X,
                origin.Y-Scale*coordinates[i].Y);
            }
            return points;
        }
        public Point2 GetPoint(PointF pixel)
        {
            var origin = Origin;

            return new Point2(
                ( pixel.X-origin.X )/Scale,
                -( pixel.Y-origin.Y )/Scale,
                1);
        }
        public Vector2 GetVector(PointF pixel)
        {
            var origin = Origin;

            return new Vector2(
                ( pixel.X-origin.X )/Scale,
                -( pixel.Y-origin.Y )/Scale);
        }
        #endregion
    }
}
