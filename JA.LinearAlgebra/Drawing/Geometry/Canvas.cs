using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;
using System.Numerics;

using static System.Math;

using Point2   = JA.Drawing.Geometry.Planar.Point2;

namespace JA.Drawing.Geometry
{
    
    public abstract class Canvas : ICanvas
    {
        #region Factory
        public Canvas(Control control, float drawScale)
        {
            Styles=new DrawStyles(DefaultColorScheme.Dark);
            Target=control;
            DrawScale=drawScale;
            control.Paint+=(s, ev) =>
            {
                LastGraphics=ev.Graphics;
                LastGraphics.SmoothingMode=SmoothingMode.AntiAlias;
                OnPaint();
            };
            control.MouseDown+=(s, ev) =>
            {
                Frame=Frame.CenteredWithScale(control.ClientSize, DrawScale);
                var point=Frame.GetPoint(ev.Location);
                OnMouseDown(Frame, ev.Button, point);
                control.Invalidate();
            };
            control.MouseMove+=(s, ev) =>
            {
                Frame=Frame.CenteredWithScale(control.ClientSize, DrawScale);
                var point=Frame.GetPoint(ev.Location);
                if (ev.Button!=MouseButtons.None)
                {
                    OnMouseDrag(Frame, ev.Button, point);
                }
                else
                {
                    OnMouseMove(Frame, point);

                }
                control.Invalidate();
            };
            control.MouseUp+=(s, ev) =>
            {
                Frame=Frame.CenteredWithScale(control.ClientSize, DrawScale);
                var point=Frame.GetPoint(ev.Location);
                OnMouseUp(Frame, ev.Button, point);
                control.Invalidate();
            };
            control.FindForm().MouseWheel+=(s, ev) =>
            {
                DrawScale*=(float)Pow(1.2, ev.Delta/120);
                control.Invalidate();
            };
        }

        #endregion
        #region Property
        public Frame Frame { get; private set; }
        public Graphics LastGraphics { get; private set; }
        public float DrawScale { get; private set; }
        public DrawStyles Styles { get; private set; }

        public Control Target { get; }
        #endregion

        #region Events
        protected abstract void OnPaint();
        protected virtual void OnMouseDown(Frame frame, MouseButtons buttons, Point2 mousePoint) { }
        protected virtual void OnMouseMove(Frame frame, Point2 mousePoint) { }
        protected virtual void OnMouseDrag(Frame frame, MouseButtons buttons, Point2 mousePoint) { }
        protected virtual void OnMouseUp(Frame frame, MouseButtons buttons, Point2 mousePoint) { } 
        #endregion

        //#region Rendering
        //public void DrawLine(Line2 line)
        //{
        //    float size = (float)Sqrt( Frame.Target.Width*Frame.Target.Width
        //        + Frame.Target.Height*Frame.Target.Height )/Frame.Scale;

        //    float d = line.ParallelDistanceTo(line.GetCenter());

        //    var from = line.GetPointAlong(d - size);
        //    var to = line.GetPointAlong(d + size);

        //    LastGraphics.DrawLine(Styles, Frame.GetPixel(from), Frame.GetPixel(to));
        //}
        //public void DrawLine(Point2 point1, Point2 point2)
        //    => LastGraphics.DrawLine(Styles, Frame.GetPixel(point1), Frame.GetPixel(point2));
        //public void DrawPoint(Point2 point, float size = -1)
        //    => LastGraphics.DrawPoint(Styles, Frame.GetPixel(point), size);
        //public void FillPoint(Point2 point, float size = -1)
        //    => LastGraphics.FillPoint(Styles, Frame.GetPixel(point), size);
        //public void DrawPoints(Point2[] nodes, float size = -1)
        //    => LastGraphics.DrawPoints(Styles, Frame.GetPixels(nodes), size);
        //public void FillPoints(Point2[] nodes, float size = -1)
        //    => LastGraphics.FillPoints(Styles, Frame.GetPixels(nodes), size);
        //public void DrawCircle(Circle2 circle)
        //    => DrawCircle(circle.Center, circle.Radius);
        //public void DrawCircle(Point2 center, float radius)
        //    => LastGraphics.DrawCircle(Styles, Frame.GetPixel(center), Frame.Scale*radius);
        //public void DrawEllipse(Ellipse2 ellipse, float tiltAngleDegrees = 0)
        //    => DrawEllipse(ellipse.Center, ellipse.MajorAxis, ellipse.MinorAxis);
        //public void DrawEllipse(Point2 center, float radius1, float radius2, float tiltAngleDegrees = 0)
        //    => LastGraphics.DrawEllipse(Styles, Frame.GetPixel(center), Frame.Scale*radius1, Frame.Scale*radius2, tiltAngleDegrees);
        //public void DrawCircleArc(Point2 center, float radius, float startAngle, float sweepAngleDegrees)
        //    => LastGraphics.DrawEllipseArc(Styles, Frame.GetPixel(center), Frame.Scale*radius, Frame.Scale*radius, startAngle, sweepAngleDegrees);
        //public void DrawEllipseArc(Point2 center, float radius1, float radius2, float startAngle, float sweepAngleDegrees, float tiltAngleDegrees = 0)
        //    => LastGraphics.DrawEllipseArc(Styles, Frame.GetPixel(center), Frame.Scale*radius1, Frame.Scale*radius2, startAngle, sweepAngleDegrees, tiltAngleDegrees);
        //public void FillCircle(Point2 center, float radius)
        //    => LastGraphics.FillCircle(Styles, Frame.GetPixel(center), Frame.Scale*radius);
        //public void FillEllipse(Point2 center, float radius1, float radius2, float tiltAngleDegrees = 0)
        //    => LastGraphics.FillEllipse(Styles, Frame.GetPixel(center), Frame.Scale*radius1, Frame.Scale*radius2, tiltAngleDegrees);
        //public void DrawArcBetweenTwoPoints(Point2 a, Point2 b, float radius, bool flip = false)
        //    => LastGraphics.DrawArcBetweenTwoPoints(Styles, Frame.GetPixel(a), Frame.GetPixel(b), Frame.Scale*radius, flip);
        //public void DrawArcFromCenter(Point2 center, Point2 point, float sweepAngleDegrees)
        //    => LastGraphics.DrawArcFromCenter(Styles, Frame.GetPixel(center), Frame.GetPixel(point), sweepAngleDegrees);
        //public void DrawLineArrow(Point2 from, Vector2 delta, float arrowSize, bool bothSides = false)
        //    => LastGraphics.DrawLineArrow(Styles, Frame.GetPixel(from), Frame.GetPixel(from+delta), arrowSize, bothSides);
        //public void DrawLineArrow(Point2 from, Point2 to, float arrowSize, bool bothSides = false)
        //    => LastGraphics.DrawLineArrow(Styles, Frame.GetPixel(from), Frame.GetPixel(to), arrowSize, bothSides);
        //public void DrawLineArrow(Point2 from, float offset_x, float offset_y, float arrowSize, bool bothSides = false)
        //    => LastGraphics.DrawLineArrow(Styles, Frame.GetPixel(from), offset_y, offset_y, arrowSize, bothSides);
        //public void DrawText(string text, Point2 anchor, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
        //    => LastGraphics.DrawText(Styles, text, Frame.GetPixel(anchor), alignment, offset_x, offset_y, padding);
        //public void DrawTextBetweenTwoPoints(string text, Point2 start, Point2 end, ContentAlignment alignment, float offset_x = 0, float offset_y = 0, int padding = 4)
        //    => LastGraphics.DrawTextBetweenTwoPoints(Styles, text, Frame.GetPixel(start), Frame.GetPixel(end), alignment, offset_x, offset_y, padding);
        //public void DrawCaption(Point2 anchor, Size area, string text, ContentAlignment alingment, float offset_x = 0, float offset_y = 0)
        //    => LastGraphics.DrawCaption(Styles, Frame.GetPixel(anchor), area, text, alingment, offset_x, offset_y);
        //public void HorizontalArrow(Point2 start, float length, string text, bool bothSides = false)
        //    => LastGraphics.HorizontalArrow(Styles, Frame.GetPixel(start), Frame.Scale*length, text, bothSides);
        //public void VerticalArrow(Point2 start, float length, string text, bool bothSides = false)
        //    => LastGraphics.VerticalArrow(Styles, Frame.GetPixel(start), Frame.Scale*length, text, bothSides);

        //#endregion
    }
}
