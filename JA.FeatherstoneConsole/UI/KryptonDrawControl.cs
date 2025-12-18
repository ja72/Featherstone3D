using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

using static System.Math;

using JA.Drawing.Geometry.Planar;

namespace JA.UI
{
    public partial class KryptonDrawControl : Krypton.Toolkit.KryptonPictureBox
    {
        public KryptonDrawControl()
        {
            InitializeComponent();

            BorderStyle = BorderStyle.FixedSingle;
            DrawScale = 1f;
            DrawOrigin = new Point(Target.Width/2, Target.Height/2);
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            FindForm().MouseWheel+=(s, ev) =>
            {
                DrawScale*=(float)Pow(1.2, ev.Delta/120);
            };
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            LastClip=pe.ClipRectangle;
            LastGraphics=pe.Graphics;
            LastGraphics.Clear(BackColor);
            LastGraphics.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            base.OnPaint(pe);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            var rel = GetRelativeOrigin();
            RelativeOriginWithScale(DrawScale, rel.X, rel.Y);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseDownPoint=GetPoint(e.Location);
            Buttons=e.Button;
            base.OnMouseDown(e);
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseUpPoint = GetPoint(e.Location);
            Buttons=e.Button;
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseMovePoint = GetPoint(e.Location);
            Buttons=e.Button;
            base.OnMouseMove(e);
        }

        #region SetFrame
        public Vector2 GetRelativeOrigin()
        {
            return new Vector2(
                DrawOrigin.X/( Target.Width-1 ), 
                DrawOrigin.Y/( Target.Height-1 ));
        }
        public void CenteredWithSize(float size)
        {
            RelativeOriginWithSize(size, 0.5f, 0.5f);
        }
        public void CenteredWithScale(float scale)
        {
            RelativeOriginWithScale(scale, 0.5f, 0.5f);
        }
        public void RelativeOriginWithSize(float size, float relx, float rely)
        {
            var orign = new PointF((Target.Width-1) * relx, (Target.Height-1) * rely);
            AtOriginWithSize(orign, size);
        }
        public void RelativeOriginWithScale(float scale, float relx, float rely)
        {
            var orign = new PointF((Target.Width-1)  * relx, (Target.Height-1) * rely);
            AtOriginWithScale(orign, scale);
        }
        public void AtOriginWithSize(PointF origin, float size)
        {
            if (size<=0)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be positive.");
            int pixels = Max(1, Min(Target.Height-1, Target.Width-1));
            float scale = pixels/size;
            AtOriginWithScale(origin, scale);
        }
        public void AtOriginWithScale(PointF origin, float scale)
        {
            DrawOrigin=origin;
            DrawScale=scale;
            Invalidate();
        }

        #endregion

        #region Properties
        public Rectangle LastClip { get; private set; }
        public Graphics LastGraphics { get; private set; }
        public float DrawScale { get; private set; }
        public PointF DrawOrigin { get; private set; }
        public Size Target { get => base.ClientSize; }
        public Point2 MouseDownPoint { get; private set; }
        public Point2 MouseMovePoint { get; private set; }
        public Point2 MouseUpPoint { get; private set; }
        public MouseButtons Buttons { get; private set; }
        public Point2 TopLeft => Point2.FromVector(
            GetVector(new PointF(0, 0)));
        public Point2 TopRight => Point2.FromVector(
            GetVector(new PointF(Target.Width-1, 0)));
        public Point2 BottomLeft => Point2.FromVector(
            GetVector(new PointF(0, Target.Height-1)));
        public Point2 BottomRight => Point2.FromVector(
            GetVector(new PointF(Target.Width-1, Target.Height-1)));

        #endregion

        #region Conversions
        public PointF GetPixel(Point2 point)
            => GetPixel(point.AsVector());

        public PointF GetPixel(Vector2 coordinate)
        {
            var origin = DrawOrigin;

            return new PointF(
                origin.X+DrawScale*coordinate.X,
                origin.Y-DrawScale*coordinate.Y);
        }

        public PointF[] GetPixels(params Point2[] points)
            => GetPixels(points.Select(pt => pt.AsVector()).ToArray());

        public PointF[] GetPixels(params Vector2[] coordinates)
        {
            var origin = DrawOrigin;

            var points = new PointF[coordinates.Length];
            for (var i = 0; i<points.Length; i++)
            {
                points[i]=new PointF(
                origin.X+DrawScale*coordinates[i].X,
                origin.Y-DrawScale*coordinates[i].Y);
            }
            return points;
        }
        public Point2 GetPoint(PointF pixel)
        {
            var origin = DrawOrigin;

            return new Point2(
                 ( pixel.X-origin.X )/DrawScale,
                -( pixel.Y-origin.Y )/DrawScale,
                1);
        }
        public Vector2 GetVector(PointF pixel)
        {
            var origin = DrawOrigin;

            return new Vector2(
                 ( pixel.X-origin.X )/DrawScale,
                -( pixel.Y-origin.Y )/DrawScale);
        }
        #endregion

    }
}
