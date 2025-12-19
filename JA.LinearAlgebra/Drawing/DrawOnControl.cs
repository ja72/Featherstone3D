using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JA.Geometry.Planar;

using static System.Math;

namespace JA.Drawing
{
    public delegate void PaintEventHandler(DrawOnControl draw);
    public delegate void MouseEventHandler(DrawOnControl draw);

    public interface IDrawOnControl : IFrame
    {
        Graphics LastGraphics { get; }
        Control OnControl { get; }
    }

    public class DrawOnControl : IDrawOnControl
    {
        public event PaintEventHandler Paint;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseMove;
        public DrawOnControl(Control onControl)
        {
            OnControl=onControl??throw new ArgumentNullException(nameof(onControl));
            DrawScale=1f;
            DrawOrigin=new Point(ClientSize.Width/2, ClientSize.Height/2);

            onControl.ParentChanged+=(s, e) =>
            {
                onControl.FindForm().MouseWheel+=(s2, ev2) =>
                {
                    DrawScale*=(float)Pow(1.2, ev2.Delta/120);
                };
            };

            onControl.Paint+=(s, ev) =>
            {
                LastClip=ev.ClipRectangle;
                LastGraphics=ev.Graphics;
                //LastGraphics.Clear(OnControl.BackColor);
                LastGraphics.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Paint?.Invoke(this);
            };
            onControl.SizeChanged+=(s, ev) =>
            {
                var rel = GetRelativeOrigin();
                RelativeOriginWithScale(DrawScale, rel.X, rel.Y);
                onControl.Invalidate();
            };
            onControl.MouseDown+=(s, ev) =>
            {
                MouseDownPoint=GetPoint(ev.Location);
                Buttons=ev.Button;
                MouseDown?.Invoke(this);
                onControl.Invalidate();
            };
            onControl.MouseMove+=(s, ev) =>
            {
                MouseMovePoint=GetPoint(ev.Location);
                Buttons=ev.Button;
                MouseMove?.Invoke(this);
                onControl.Invalidate();
            };
            onControl.MouseUp+=(s, ev) =>
            {
                MouseUpPoint=GetPoint(ev.Location);
                Buttons=ev.Button;
                MouseUp?.Invoke(this);
                onControl.Invalidate();
            };
        }

        #region SetFrame
        public Vector2 GetRelativeOrigin()
        {
            return new Vector2(
                DrawOrigin.X/( ClientSize.Width-1 ),
                DrawOrigin.Y/( ClientSize.Height-1 ));
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
            var orign = new PointF((ClientSize.Width-1) * relx, (ClientSize.Height-1) * rely);
            AtOriginWithSize(orign, size);
        }
        public void RelativeOriginWithScale(float scale, float relx, float rely)
        {
            var orign = new PointF((ClientSize.Width-1)  * relx, (ClientSize.Height-1) * rely);
            AtOriginWithScale(orign, scale);
        }
        public void AtOriginWithSize(PointF origin, float size)
        {
            if (size<=0)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be positive.");
            int pixels = Max(1, Min(ClientSize.Height-1, ClientSize.Width-1));
            float scale = pixels/size;
            AtOriginWithScale(origin, scale);
        }
        public void AtOriginWithScale(PointF origin, float scale)
        {
            DrawOrigin=origin;
            DrawScale=scale;
            OnControl.Invalidate();
        }

        #endregion

        #region Properties
        public Control OnControl { get; }
        public Size ClientSize => OnControl.ClientSize;
        public Rectangle LastClip { get; private set; }
        public Graphics LastGraphics { get; private set; }
        public float DrawScale { get; private set; }
        public PointF DrawOrigin { get; private set; }
        public Point2 MouseDownPoint { get; private set; }
        public Point2 MouseMovePoint { get; private set; }
        public Point2 MouseUpPoint { get; private set; }
        public MouseButtons Buttons { get; private set; }
        public Point2 TopLeft => Point2.FromVector(
            GetVector(new PointF(0, 0)));
        public Point2 TopRight => Point2.FromVector(
            GetVector(new PointF(ClientSize.Width-1, 0)));
        public Point2 BottomLeft => Point2.FromVector(
            GetVector(new PointF(0, ClientSize.Height-1)));
        public Point2 BottomRight => Point2.FromVector(
            GetVector(new PointF(ClientSize.Width-1, ClientSize.Height-1)));

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
