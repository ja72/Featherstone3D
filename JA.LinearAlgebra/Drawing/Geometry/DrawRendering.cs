using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using static System.Math;

namespace JA.Drawing.Geometry
{
    using Vector2  = System.Numerics.Vector2;
    using Point2   = JA.Drawing.Geometry.Planar.Point2;
    using Circle2  = JA.Drawing.Geometry.Planar.Circle2;
    using Ellipse2 = JA.Drawing.Geometry.Planar.Ellipse2;
    using Line2    = JA.Drawing.Geometry.Planar.Line2;

    public interface IMouseControl
    {
        MouseButtons Buttons { get; }
        Point2 MouseDownPoint { get; }
        Point2 MouseMovePoint { get; }
        Point2 MouseUpPoint { get; }
    }

    public interface IDrawControl : IMouseControl
    {
        Size DrawSize { get; }
        PointF DrawOrigin { get; }
        float DrawScale { get; }
        Graphics LastGraphics { get; }
        Rectangle LastClip { get; }
        DrawStyles DrawStyles { get; }
    }
    public static class DrawConversions
    {
        public static PointF GetPixel(this IDrawControl control, Point2 point)
            => GetPixel(control, point.AsVector());
        public static PointF[] GetPixels(this IDrawControl control, params Point2[] points)
            => GetPixels(control, points.Select(pt => pt.AsVector()));

        public static PointF GetPixel(this IDrawControl control, Vector2 coordinate)
        {
            var origin = control.DrawOrigin;

            return new PointF(
                origin.X+control.DrawScale*coordinate.X,
                origin.Y-control.DrawScale*coordinate.Y);
        }

        public static PointF[] GetPixels(this IDrawControl control, IEnumerable<Vector2> coordinates)
        {
            var origin = control.DrawOrigin;

            var points = new List<PointF>(16);
            foreach (var item in points)
            {
                points.Add(new PointF(
                    origin.X+control.DrawScale*item.X,
                    origin.Y-control.DrawScale*item.Y));

            }
            return points.ToArray();
        }
        public static PointF[] GetPixels(this IDrawControl control, params Vector2[] coordinates)
        {
            return GetPixels(control, coordinates.AsEnumerable());
        }
        public static Point2 GetPoint(this IDrawControl control, PointF pixel)
        {
            var origin = control.DrawOrigin;

            return new Point2(
                 ( pixel.X-origin.X )/control.DrawScale,
                -( pixel.Y-origin.Y )/control.DrawScale,
                1);
        }
        public static Vector2 GetVector(this IDrawControl control, PointF pixel)
        {
            var origin = control.DrawOrigin;

            return new Vector2(
                 ( pixel.X-origin.X )/control.DrawScale,
                -( pixel.Y-origin.Y )/control.DrawScale);
        }
    }

    public static class DrawRendering
    {
        public static void DrawLine(this IDrawControl canvas, Line2 line)
        {
            float size = (float)Sqrt( canvas.DrawSize.Width*canvas.DrawSize.Width
                + canvas.DrawSize.Height*canvas.DrawSize.Height )/canvas.DrawScale;

            float d = line.ParallelDistanceTo(line.GetCenter());

            var from = line.GetPointAlong(d - size);
            var to = line.GetPointAlong(d + size);

            canvas.LastGraphics.DrawLine(canvas.DrawStyles, canvas.GetPixel(from), canvas.GetPixel(to));
        }
        public static void DrawLine(this IDrawControl canvas, Point2 point1, Point2 point2)
            => canvas.LastGraphics.DrawLine(canvas.DrawStyles, canvas.GetPixel(point1), canvas.GetPixel(point2));
        public static void DrawPoint(this IDrawControl canvas, Point2 point, float size = -1)
            => canvas.LastGraphics.DrawPoint(canvas.DrawStyles, canvas.GetPixel(point), size);
        public static void FillPoint(this IDrawControl canvas, Point2 point, float size = -1)
            => canvas.LastGraphics.FillPoint(canvas.DrawStyles, canvas.GetPixel(point), size);
        public static void DrawPoints(this IDrawControl canvas, Point2[] nodes, float size = -1)
            => canvas.LastGraphics.DrawPoints(canvas.DrawStyles, canvas.GetPixels(nodes), size);
        public static void FillPoints(this IDrawControl canvas, Point2[] nodes, float size = -1)
            => canvas.LastGraphics.FillPoints(canvas.DrawStyles, canvas.GetPixels(nodes), size);
        public static void DrawCircle(this IDrawControl canvas, Circle2 circle)
            => DrawCircle(canvas, circle.Center, circle.Radius);
        public static void DrawCircle(this IDrawControl canvas, Point2 center, float radius)
            => canvas.LastGraphics.DrawCircle(canvas.DrawStyles, canvas.GetPixel(center), canvas.DrawScale*radius);
        public static void DrawEllipse(this IDrawControl canvas, Ellipse2 ellipse, float tiltAngleDegrees = 0)
            => DrawEllipse(canvas, ellipse.Center, ellipse.MajorAxis, ellipse.MinorAxis);
        public static void DrawEllipse(this IDrawControl canvas, Point2 center, float radius1, float radius2, float tiltAngleDegrees = 0)
            => canvas.LastGraphics.DrawEllipse(canvas.DrawStyles, canvas.GetPixel(center), canvas.DrawScale*radius1, canvas.DrawScale*radius2, tiltAngleDegrees);
        public static void DrawCircleArc(this IDrawControl canvas, Point2 center, float radius, float startAngle, float sweepAngleDegrees)
            => canvas.LastGraphics.DrawEllipseArc(canvas.DrawStyles, canvas.GetPixel(center), canvas.DrawScale*radius, canvas.DrawScale*radius, startAngle, sweepAngleDegrees);
        public static void DrawEllipseArc(this IDrawControl canvas, Point2 center, float radius1, float radius2, float startAngle, float sweepAngleDegrees, float tiltAngleDegrees = 0)
            => canvas.LastGraphics.DrawEllipseArc(canvas.DrawStyles, canvas.GetPixel(center), canvas.DrawScale*radius1, canvas.DrawScale*radius2, startAngle, sweepAngleDegrees, tiltAngleDegrees);
        public static void FillCircle(this IDrawControl canvas, Point2 center, float radius)
            => canvas.LastGraphics.FillCircle(canvas.DrawStyles, canvas.GetPixel(center), canvas.DrawScale*radius);
        public static void FillEllipse(this IDrawControl canvas, Point2 center, float radius1, float radius2, float tiltAngleDegrees = 0)
            => canvas.LastGraphics.FillEllipse(canvas.DrawStyles, canvas.GetPixel(center), canvas.DrawScale*radius1, canvas.DrawScale*radius2, tiltAngleDegrees);
        public static void DrawArcBetweenTwoPoints(this IDrawControl canvas, Point2 a, Point2 b, float radius, bool flip = false)
            => canvas.LastGraphics.DrawArcBetweenTwoPoints(canvas.DrawStyles, canvas.GetPixel(a), canvas.GetPixel(b), canvas.DrawScale*radius, flip);
        public static void DrawArcFromCenter(this IDrawControl canvas, Point2 center, Point2 point, float sweepAngleDegrees)
            => canvas.LastGraphics.DrawArcFromCenter(canvas.DrawStyles, canvas.GetPixel(center), canvas.GetPixel(point), sweepAngleDegrees);
        public static void DrawLineArrow(this IDrawControl canvas, Point2 from, Vector2 delta, float arrowSize, bool bothSides = false)
            => canvas.LastGraphics.DrawLineArrow(canvas.DrawStyles, canvas.GetPixel(from), canvas.GetPixel(from+delta), arrowSize, bothSides);
        public static void DrawLineArrow(this IDrawControl canvas, Point2 from, Point2 to, float arrowSize, bool bothSides = false)
            => canvas.LastGraphics.DrawLineArrow(canvas.DrawStyles, canvas.GetPixel(from), canvas.GetPixel(to), arrowSize, bothSides);
        public static void DrawLineArrow(this IDrawControl canvas, Point2 from, float offset_x, float offset_y, float arrowSize, bool bothSides = false)
            => canvas.LastGraphics.DrawLineArrow(canvas.DrawStyles, canvas.GetPixel(from), offset_x, offset_y, arrowSize, bothSides);
        public static void DrawText(this IDrawControl canvas, string text, Point2 anchor, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
            => canvas.LastGraphics.DrawText(canvas.DrawStyles, text, canvas.GetPixel(anchor), alignment, offset_x, offset_y, padding);
        public static void DrawTextBetweenTwoPoints(this IDrawControl canvas, string text, Point2 start, Point2 end, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
            => canvas.LastGraphics.DrawTextBetweenTwoPoints(canvas.DrawStyles, text, canvas.GetPixel(start), canvas.GetPixel(end), alignment, offset_x, offset_y, padding);
        public static void DrawCaption(this IDrawControl canvas, Point2 anchor, Size area, string text, ContentAlignment alingment, float offset_x = 0, float offset_y = 0)
            => canvas.LastGraphics.DrawCaption(canvas.DrawStyles, canvas.GetPixel(anchor), area, text, alingment, offset_x, offset_y);
        public static void HorizontalArrow(this IDrawControl canvas, Point2 start, float length, string text, bool bothSides = false)
            => canvas.LastGraphics.HorizontalArrow(canvas.DrawStyles, canvas.GetPixel(start), canvas.DrawScale*length, text, bothSides);
        public static void VerticalArrow(this IDrawControl canvas, Point2 start, float length, string text, bool bothSides = false)
            => canvas.LastGraphics.VerticalArrow(canvas.DrawStyles, canvas.GetPixel(start), canvas.DrawScale*length, text, bothSides);
    }
}
