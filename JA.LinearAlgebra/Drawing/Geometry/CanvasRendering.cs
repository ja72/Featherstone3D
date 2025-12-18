using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static System.Math;

namespace JA.Drawing.Geometry
{
    using Point2   = JA.Drawing.Geometry.Planar.Point2;
    using Line2    = JA.Drawing.Geometry.Planar.Line2;
    using Circle2  = JA.Drawing.Geometry.Planar.Circle2;
    using Ellipse2 = JA.Drawing.Geometry.Planar.Ellipse2;

    public interface ICanvas
    {
        Frame Frame { get; }
        Graphics LastGraphics { get; }
        float DrawScale { get; }
        DrawStyles Styles { get; }
        Control Target { get; }
    }

    public static class CanvasConversions
    {
        public static PointF GetPixel(this ICanvas canvas, Point2 point)
            => canvas.Frame.GetPixel(point);
        public static PointF[] GetPixels(this ICanvas canvas, params Point2[] points)
            => canvas.Frame.GetPixels(points);
        public static PointF[] GetPixels(this ICanvas canvas, params Vector2[] coordinates)
            => canvas.Frame.GetPixels(coordinates);
        public static Point2 GetPoint(this ICanvas canvas, PointF pixel)
            => canvas.Frame.GetPoint(pixel);
        public static Vector2 GetVector(this ICanvas canvas, PointF pixel)
            => canvas.Frame.GetVector(pixel);
    }

    public static class CanvasRendering
    {
        public static void DrawLine(this ICanvas canvas, Line2 line)
        {
            float size = (float)Sqrt( canvas.Target.Width*canvas.Target.Width
                + canvas.Target.Height*canvas.Target.Height )/canvas.DrawScale;

            float d = line.ParallelDistanceTo(line.GetCenter());

            var from = line.GetPointAlong(d - size);
            var to = line.GetPointAlong(d + size);

            canvas.LastGraphics.DrawLine(canvas.Styles, canvas.Frame.GetPixel(from), canvas.Frame.GetPixel(to));
        }
        public static void DrawLine(this ICanvas canvas, Point2 point1, Point2 point2)
            => canvas.LastGraphics.DrawLine(canvas.Styles, canvas.Frame.GetPixel(point1), canvas.Frame.GetPixel(point2));
        public static void DrawPoint(this ICanvas canvas, Point2 point, float size = -1)
            => canvas.LastGraphics.DrawPoint(canvas.Styles, canvas.Frame.GetPixel(point), size);
        public static void FillPoint(this ICanvas canvas, Point2 point, float size = -1)
            => canvas.LastGraphics.FillPoint(canvas.Styles, canvas.Frame.GetPixel(point), size);
        public static void DrawPoints(this ICanvas canvas, Point2[] nodes, float size = -1)
            => canvas.LastGraphics.DrawPoints(canvas.Styles, canvas.Frame.GetPixels(nodes), size);
        public static void FillPoints(this ICanvas canvas, Point2[] nodes, float size = -1)
            => canvas.LastGraphics.FillPoints(canvas.Styles, canvas.Frame.GetPixels(nodes), size);
        public static void DrawCircle(this ICanvas canvas, Circle2 circle)
            => DrawCircle(canvas, circle.Center, circle.Radius);
        public static void DrawCircle(this ICanvas canvas, Point2 center, float radius)
            => canvas.LastGraphics.DrawCircle(canvas.Styles, canvas.Frame.GetPixel(center), canvas.Frame.Scale*radius);
        public static void DrawEllipse(this ICanvas canvas, Ellipse2 ellipse, float tiltAngleDegrees = 0)
            => DrawEllipse(canvas, ellipse.Center, ellipse.MajorAxis, ellipse.MinorAxis);
        public static void DrawEllipse(this ICanvas canvas, Point2 center, float radius1, float radius2, float tiltAngleDegrees = 0)
            => canvas.LastGraphics.DrawEllipse(canvas.Styles, canvas.Frame.GetPixel(center), canvas.Frame.Scale*radius1, canvas.Frame.Scale*radius2, tiltAngleDegrees);
        public static void DrawCircleArc(this ICanvas canvas, Point2 center, float radius, float startAngle, float sweepAngleDegrees)
            => canvas.LastGraphics.DrawEllipseArc(canvas.Styles, canvas.Frame.GetPixel(center), canvas.Frame.Scale*radius, canvas.Frame.Scale*radius, startAngle, sweepAngleDegrees);
        public static void DrawEllipseArc(this ICanvas canvas, Point2 center, float radius1, float radius2, float startAngle, float sweepAngleDegrees, float tiltAngleDegrees = 0)
            => canvas.LastGraphics.DrawEllipseArc(canvas.Styles, canvas.Frame.GetPixel(center), canvas.Frame.Scale*radius1, canvas.Frame.Scale*radius2, startAngle, sweepAngleDegrees, tiltAngleDegrees);
        public static void FillCircle(this ICanvas canvas, Point2 center, float radius)
            => canvas.LastGraphics.FillCircle(canvas.Styles, canvas.Frame.GetPixel(center), canvas.Frame.Scale*radius);
        public static void FillEllipse(this ICanvas canvas, Point2 center, float radius1, float radius2, float tiltAngleDegrees = 0)
            => canvas.LastGraphics.FillEllipse(canvas.Styles, canvas.Frame.GetPixel(center), canvas.Frame.Scale*radius1, canvas.Frame.Scale*radius2, tiltAngleDegrees);
        public static void DrawArcBetweenTwoPoints(this ICanvas canvas, Point2 a, Point2 b, float radius, bool flip = false)
            => canvas.LastGraphics.DrawArcBetweenTwoPoints(canvas.Styles, canvas.Frame.GetPixel(a), canvas.Frame.GetPixel(b), canvas.Frame.Scale*radius, flip);
        public static void DrawArcFromCenter(this ICanvas canvas, Point2 center, Point2 point, float sweepAngleDegrees)
            => canvas.LastGraphics.DrawArcFromCenter(canvas.Styles, canvas.Frame.GetPixel(center), canvas.Frame.GetPixel(point), sweepAngleDegrees);
        public static void DrawLineArrow(this ICanvas canvas, Point2 from, Vector2 delta, float arrowSize, bool bothSides = false)
            => canvas.LastGraphics.DrawLineArrow(canvas.Styles, canvas.Frame.GetPixel(from), canvas.Frame.GetPixel(from+delta), arrowSize, bothSides);
        public static void DrawLineArrow(this ICanvas canvas, Point2 from, Point2 to, float arrowSize, bool bothSides = false)
            => canvas.LastGraphics.DrawLineArrow(canvas.Styles, canvas.Frame.GetPixel(from), canvas.Frame.GetPixel(to), arrowSize, bothSides);
        public static void DrawLineArrow(this ICanvas canvas, Point2 from, float offset_x, float offset_y, float arrowSize, bool bothSides = false)
            => canvas.LastGraphics.DrawLineArrow(canvas.Styles, canvas.Frame.GetPixel(from), offset_x, offset_y, arrowSize, bothSides);
        public static void DrawText(this ICanvas canvas, string text, Point2 anchor, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
            => canvas.LastGraphics.DrawText(canvas.Styles, text, canvas.Frame.GetPixel(anchor), alignment, offset_x, offset_y, padding);
        public static void DrawTextBetweenTwoPoints(this ICanvas canvas, string text, Point2 start, Point2 end, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
            => canvas.LastGraphics.DrawTextBetweenTwoPoints(canvas.Styles, text, canvas.Frame.GetPixel(start), canvas.Frame.GetPixel(end), alignment, offset_x, offset_y, padding);
        public static void DrawCaption(this ICanvas canvas, Point2 anchor, Size area, string text, ContentAlignment alingment, float offset_x = 0, float offset_y = 0)
            => canvas.LastGraphics.DrawCaption(canvas.Styles, canvas.Frame.GetPixel(anchor), area, text, alingment, offset_x, offset_y);
        public static void HorizontalArrow(this ICanvas canvas, Point2 start, float length, string text, bool bothSides = false)
            => canvas.LastGraphics.HorizontalArrow(canvas.Styles, canvas.Frame.GetPixel(start), canvas.Frame.Scale*length, text, bothSides);
        public static void VerticalArrow(this ICanvas canvas, Point2 start, float length, string text, bool bothSides = false)
            => canvas.LastGraphics.VerticalArrow(canvas.Styles, canvas.Frame.GetPixel(start), canvas.Frame.Scale*length, text, bothSides);

    }
}
