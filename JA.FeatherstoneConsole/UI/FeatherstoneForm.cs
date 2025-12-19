using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using JA.Drawing;
using JA.Geometry.Planar;

namespace JA.UI
{
    public partial class FeatherstoneForm : Krypton.Toolkit.KryptonForm
    {
        List<Point2> points=new List<Point2>();
        DrawStyles styles=DrawStyles.CreateDarkScheme();
        public FeatherstoneForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var draw=new DrawOnControl(this);

            draw.Paint      +=Draw_Paint; 
            draw.MouseDown  +=Draw_MouseDown;
        }

        private void Draw_MouseDown(DrawOnControl draw)
        {
            points.Add(draw.MouseDownPoint);
        }

        private void Draw_Paint(DrawOnControl draw)
        {
            foreach (var item in points)
            {
                draw.DrawPoint(styles, item);
            }
        }
    }
}
