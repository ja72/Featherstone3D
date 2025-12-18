using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

using static System.Math;

using JA.Drawing.Geometry.Planar;

namespace JA.Drawing.Geometry
{
    public partial class DrawControl : PictureBox, IDrawControl
    {
        public DrawControl()
        {
            InitializeComponent();
            DrawStyles = new DrawStyles(DefaultColorScheme.Dark);
            BorderStyle=BorderStyle.FixedSingle;
            DrawScale=1f;
            DrawOrigin=new Point(DrawSize.Width/2, DrawSize.Height/2);
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
            //LastGraphics.Clear(BackColor);
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
            MouseDownPoint=this.GetPoint(e.Location);
            Buttons=e.Button;
            base.OnMouseDown(e);
            Invalidate();
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseUpPoint=this.GetPoint(e.Location);
            Buttons=e.Button;
            base.OnMouseUp(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            MouseMovePoint=this.GetPoint(e.Location);
            Buttons=e.Button;
            base.OnMouseMove(e);
        }

        #region SetFrame
        public Vector2 GetRelativeOrigin()
        {
            return new Vector2(
                DrawOrigin.X/( DrawSize.Width-1 ),
                DrawOrigin.Y/( DrawSize.Height-1 ));
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
            var orign = new PointF((DrawSize.Width-1) * relx, (DrawSize.Height-1) * rely);
            AtOriginWithSize(orign, size);
        }
        public void RelativeOriginWithScale(float scale, float relx, float rely)
        {
            var orign = new PointF((DrawSize.Width-1)  * relx, (DrawSize.Height-1) * rely);
            AtOriginWithScale(orign, scale);
        }
        public void AtOriginWithSize(PointF origin, float size)
        {
            if (size<=0)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be positive.");
            int pixels = Max(1, Min(DrawSize.Height-1, DrawSize.Width-1));
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
        public DrawStyles DrawStyles { get; set; }
        public Rectangle LastClip { get; private set; }
        public Graphics LastGraphics { get; private set; }
        public float DrawScale { get; private set; }
        public PointF DrawOrigin { get; private set; }
        public Size DrawSize { get => base.ClientSize; }
        public Point2 MouseDownPoint { get; private set; }
        public Point2 MouseMovePoint { get; private set; }
        public Point2 MouseUpPoint { get; private set; }
        public MouseButtons Buttons { get; private set; }
        public Point2 TopLeft => Point2.FromVector(
            this.GetVector(new PointF(0, 0)));
        public Point2 TopRight => Point2.FromVector(
            this.GetVector(new PointF(DrawSize.Width-1, 0)));
        public Point2 BottomLeft => Point2.FromVector(
            this.GetVector(new PointF(0, DrawSize.Height-1)));
        public Point2 BottomRight => Point2.FromVector(
            this.GetVector(new PointF(DrawSize.Width-1, DrawSize.Height-1)));

        #endregion

    }
}
