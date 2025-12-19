using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

using JA.Geometry.Planar;

using static System.Math;

namespace JA.Drawing
{

    public static class DrawRendering
    {
        
        public static void DrawLine(this IDrawOnControl draw, DrawStyles styles, Line2 line)
        {
            float size = (float)Sqrt( draw.ClientSize.Width*draw.ClientSize.Width
        + draw.ClientSize.Height*draw.ClientSize.Height )/draw.DrawScale;

            float d = line.ParallelDistanceTo(line.GetCenter());

            var from = line.GetPointAlong(d - size);
            var to = line.GetPointAlong(d + size);

            draw.LastGraphics.DrawLine(styles, draw.GetPixel(from), draw.GetPixel(to));
        }
        public static void DrawLine(this IDrawOnControl draw, DrawStyles styles, Point2 point1, Point2 point2)
            => draw.LastGraphics.DrawLine(styles, draw.GetPixel(point1), draw.GetPixel(point2));
        public static void DrawPoint(this IDrawOnControl draw, DrawStyles styles, Point2 point, float size = -1)
            => draw.LastGraphics.DrawPoint(styles, draw.GetPixel(point), size);
        public static void FillPoint(this IDrawOnControl draw, DrawStyles styles, Point2 point, float size = -1)
            => draw.LastGraphics.FillPoint(styles, draw.GetPixel(point), size);
        public static void DrawPoints(this IDrawOnControl draw, DrawStyles styles, Point2[] nodes, float size = -1)
            => draw.LastGraphics.DrawPoints(styles, draw.GetPixels(nodes), size);
        public static void FillPoints(this IDrawOnControl draw, DrawStyles styles, Point2[] nodes, float size = -1)
            => draw.LastGraphics.FillPoints(styles, draw.GetPixels(nodes), size);
        public static void DrawCircle(this IDrawOnControl draw, DrawStyles styles, Circle2 circle)
            => DrawCircle(draw, styles, circle.Center, circle.Radius);
        public static void DrawCircle(this IDrawOnControl draw, DrawStyles styles, Point2 center, float radius)
            => draw.LastGraphics.DrawCircle(styles, draw.GetPixel(center), draw.DrawScale*radius);
        public static void DrawEllipse(this IDrawOnControl draw, DrawStyles styles, Ellipse2 ellipse, float tiltAngleDegrees = 0)
            => DrawEllipse(draw, styles, ellipse.Center, ellipse.MajorAxis, ellipse.MinorAxis);
        public static void DrawEllipse(this IDrawOnControl draw, DrawStyles styles, Point2 center, float radius1, float radius2, float tiltAngleDegrees = 0)
            => draw.LastGraphics.DrawEllipse(styles, draw.GetPixel(center), draw.DrawScale*radius1, draw.DrawScale*radius2, tiltAngleDegrees);
        public static void DrawCircleArc(this IDrawOnControl draw, DrawStyles styles, Point2 center, float radius, float startAngle, float sweepAngleDegrees)
            => draw.LastGraphics.DrawEllipseArc(styles, draw.GetPixel(center), draw.DrawScale*radius, draw.DrawScale*radius, startAngle, sweepAngleDegrees);
        public static void DrawEllipseArc(this IDrawOnControl draw, DrawStyles styles, Point2 center, float radius1, float radius2, float startAngle, float sweepAngleDegrees, float tiltAngleDegrees = 0)
            => draw.LastGraphics.DrawEllipseArc(styles, draw.GetPixel(center), draw.DrawScale*radius1, draw.DrawScale*radius2, startAngle, sweepAngleDegrees, tiltAngleDegrees);
        public static void FillCircle(this IDrawOnControl draw, DrawStyles styles, Point2 center, float radius)
            => draw.LastGraphics.FillCircle(styles, draw.GetPixel(center), draw.DrawScale*radius);
        public static void FillEllipse(this IDrawOnControl draw, DrawStyles styles, Point2 center, float radius1, float radius2, float tiltAngleDegrees = 0)
            => draw.LastGraphics.FillEllipse(styles, draw.GetPixel(center), draw.DrawScale*radius1, draw.DrawScale*radius2, tiltAngleDegrees);
        public static void DrawArcBetweenTwoPoints(this IDrawOnControl draw, DrawStyles styles, Point2 a, Point2 b, float radius, bool flip = false)
            => draw.LastGraphics.DrawArcBetweenTwoPoints(styles, draw.GetPixel(a), draw.GetPixel(b), draw.DrawScale*radius, flip);
        public static void DrawArcFromCenter(this IDrawOnControl draw, DrawStyles styles, Point2 center, Point2 point, float sweepAngleDegrees)
            => draw.LastGraphics.DrawArcFromCenter(styles, draw.GetPixel(center), draw.GetPixel(point), sweepAngleDegrees);
        public static void DrawLineArrow(this IDrawOnControl draw, DrawStyles styles, Point2 from, Vector2 delta, float arrowSize, bool bothSides = false)
            => draw.LastGraphics.DrawLineArrow(styles, draw.GetPixel(from), draw.GetPixel(from+delta), arrowSize, bothSides);
        public static void DrawLineArrow(this IDrawOnControl draw, DrawStyles styles, Point2 from, Point2 to, float arrowSize, bool bothSides = false)
            => draw.LastGraphics.DrawLineArrow(styles, draw.GetPixel(from), draw.GetPixel(to), arrowSize, bothSides);
        public static void DrawLineArrow(this IDrawOnControl draw, DrawStyles styles, Point2 from, float offset_x, float offset_y, float arrowSize, bool bothSides = false)
            => draw.LastGraphics.DrawLineArrow(styles, draw.GetPixel(from), offset_y, offset_y, arrowSize, bothSides);
        public static void DrawText(this IDrawOnControl draw, DrawStyles styles, string text, Point2 anchor, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
            => draw.LastGraphics.DrawText(styles, text, draw.GetPixel(anchor), alignment, offset_x, offset_y, padding);
        public static void DrawTextBetweenTwoPoints(this IDrawOnControl draw, DrawStyles styles, string text, Point2 start, Point2 end, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
            => draw.LastGraphics.DrawTextBetweenTwoPoints(styles, text, draw.GetPixel(start), draw.GetPixel(end), alignment, offset_x, offset_y, padding);
        public static void DrawCaption(this IDrawOnControl draw, DrawStyles styles, Point2 anchor, Size area, string text, ContentAlignment alingment, float offset_x = 0, float offset_y = 0)
            => draw.LastGraphics.DrawCaption(styles, draw.GetPixel(anchor), area, text, alingment, offset_x, offset_y);
        public static void HorizontalArrow(this IDrawOnControl draw, DrawStyles styles, Point2 start, float length, string text, bool bothSides = false)
            => draw.LastGraphics.HorizontalArrow(styles, draw.GetPixel(start), draw.DrawScale*length, text, bothSides);
        public static void VerticalArrow(this IDrawOnControl draw, DrawStyles styles, Point2 start, float length, string text, bool bothSides = false)
            => draw.LastGraphics.VerticalArrow(styles, draw.GetPixel(start), draw.DrawScale*length, text, bothSides);

        
    }
}