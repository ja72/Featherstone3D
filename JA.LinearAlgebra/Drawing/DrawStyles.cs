using System;
using System.Drawing;

namespace JA.Drawing
{
    public enum DefaultColorScheme
    {
        Light,
        Dark
    }

    /// <summary>
    /// Holds a set of GDI+ styles for drawing.
    /// </summary>
    /// <remarks>Supports the <code>using(GdiStyle style = new GdiStyle() ){ }</code> pattern</remarks>
    public class DrawStyles : IDisposable
    {
        #region Factory
        public DrawStyles(DefaultColorScheme colorScheme)
        {
            switch (colorScheme)
            {
                case DefaultColorScheme.Light:
                {
                    Stroke=new Pen(Color.Black, 0);
                    Fill=new SolidBrush(Color.White);
                    break;
                }
                case DefaultColorScheme.Dark:
                {
                    Stroke=new Pen(Color.White, 0);
                    Fill=new SolidBrush(Color.Black);
                    break;
                }
                default:
                throw new NotSupportedException(colorScheme.ToString());
            }
            Font=SystemFonts.CaptionFont.Clone() as Font;
            PointSize=8f;
        }
        public static DrawStyles CreateLightScheme()
        {
            return new DrawStyles(DefaultColorScheme.Light);
        }
        public static DrawStyles CreateDarkScheme()
        {
            return new DrawStyles(DefaultColorScheme.Dark);
        }
        public DrawStyles(Pen stroke, SolidBrush fill, Font font, float pointSize)
        {
            Stroke=stroke??throw new ArgumentNullException(nameof(stroke));
            Fill=fill??throw new ArgumentNullException(nameof(fill));
            Font=font??throw new ArgumentNullException(nameof(font));
            PointSize=pointSize;
        } 
        #endregion

        #region Properties
        public Pen Stroke { get; set; }
        public SolidBrush Fill { get; set; }
        public Font Font { get; set; }
        public float PointSize { get; set; }
        #endregion

        #region Arrows
        /// <summary>
        /// Adds an arrow to the start of a <see cref="Pen"/> object.
        /// </summary>
        /// <remarks>Intended to be used with <see cref="Stroke"/></remarks>
        /// <param name="pen">The pen to modify.</param>
        /// <param name="arrowSize">Size of the arrow in pixels.</param>
        public void AddStartArrow(float arrowSize)
        {
            Stroke.AddStartArrow(arrowSize);
        }
        /// <summary>
        /// Removes the start arrow from a <see cref="Stroke"/> object.
        /// </summary>
        public void RemoveStartArrow()
        {
            Stroke.RemoveEndArrow();
        }

        /// <summary>
        /// Adds an arrow to the end of a <see cref="Stroke"/> object.
        /// </summary>
        /// <remarks>Intended to be used with <see cref="Stroke"/></remarks>
        /// <param name="arrowSize">Size of the arrow in pixels.</param>
        public void AddEndArrow(float arrowSize)
        {
            Stroke.AddEndArrow(arrowSize);
        }

        /// <summary>
        /// Removes the end arrow from a <see cref="Stroke"/> object.
        /// </summary>
        public void RemoveEndArrow()
        {
            Stroke.RemoveEndArrow();
        }
        #endregion

        #region Dispose
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Stroke.Dispose();
                    Fill.Dispose();
                    Font.Dispose();
                    PointSize=0;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue=true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GdiStyles()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }

}
