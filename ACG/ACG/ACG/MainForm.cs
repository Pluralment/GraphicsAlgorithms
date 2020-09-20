using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ACG.Extensions;
using ACG.Vectors;
using ACG.Parser;
using ACG.Shapes;

namespace ACG
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            parser = new ObjectFileParser(@"C:\Users\Kamar\Desktop\Model.obj");
            Canvas.Size = this.ClientSize;
            this.Canvas.Image = new Bitmap(Canvas.Width, Canvas.Height);
            Graphics g = Graphics.FromImage(Canvas.Image);
            g.Clear(Color.Black);
            g.Dispose();
        }

        private float deltaX;
        private float deltaY;
        private float deltaZ;
        private bool updated = false;
        private ObjectFileParser parser;
        private Polygon[] polygons;
        private Pen pen = new Pen(Brushes.Red, 1);
        private void MainForm_Load(object sender, EventArgs e)
        {
            deltaX = Canvas.Width / 2;
            deltaY = Canvas.Height / 2;
            deltaZ = 0;

            Graphics g = Graphics.FromImage(Canvas.Image);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            polygons = parser.GetPolygons();
            foreach (var p in polygons)
            {
                p.DeltaOffsets(deltaX, deltaY, deltaZ);
                p.ScalePolygon(150f, deltaX, deltaY, deltaZ);
                g.DrawPath(pen, p.GetGraphicsPath());
            }

            g.Dispose();

        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.KeyCode == Keys.W)
            {
                Graphics g = Graphics.FromImage(Canvas.Image);
                g.Clear(Color.Black);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                foreach (var p in polygons)
                {
                    p.ScalePolygon(1.2f, deltaX, deltaY, deltaZ);
                    g.DrawPath(pen, p.GetGraphicsPath());
                }

                //Canvas.Invalidate();
                g.Dispose();

            }
            else if (e.KeyCode == Keys.S)
            {
                Graphics g = Graphics.FromImage(Canvas.Image);
                g.Clear(Color.Black);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;


                foreach (var p in polygons)
                {
                    p.ScalePolygon(0.8f, deltaX, deltaY, deltaZ);
                    g.DrawPath(pen,p.GetGraphicsPath());
                }

                //Canvas.Invalidate();
                g.Dispose();
            }
            else if (e.KeyCode == Keys.Up)
            {

                Graphics g = Graphics.FromImage(Canvas.Image);
                g.Clear(Color.Black);

                foreach (var p in polygons)
                {
                    p.RotateVectorX(0.3f, deltaX, deltaY, deltaZ);
                    g.DrawPath(pen, p.GetGraphicsPath());
                }

                //Canvas.Invalidate();
                g.Dispose();
            }
            else if (e.KeyCode == Keys.Down)
            {
                Graphics g = Graphics.FromImage(Canvas.Image);
                g.Clear(Color.Black);

                foreach (var p in polygons)
                {
                    p.RotateVectorX(-0.3f, deltaX, deltaY, deltaZ);
                    g.DrawPath(pen, p.GetGraphicsPath());
                }

                
                //Canvas.Invalidate();
                g.Dispose();
            }
            else if (e.KeyCode == Keys.Left)
            {
                Graphics g = Graphics.FromImage(Canvas.Image);
                g.Clear(Color.Black);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;


                foreach (var p in polygons)
                {
                    p.RotateVectorY(0.3f, deltaX, deltaY, deltaZ);
                    g.DrawPath(pen, p.GetGraphicsPath());
                }

                
                //Canvas.Invalidate();
                g.Dispose();
            }
            else if (e.KeyCode == Keys.Right)
            {
                Graphics g = Graphics.FromImage(Canvas.Image);
                g.Clear(Color.Black);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;


                foreach (var p in polygons)
                {
                    p.RotateVectorY(-0.3f, deltaX, deltaY, deltaZ);
                    g.DrawPath(pen, p.GetGraphicsPath());
                }

                
                //Canvas.Invalidate();
                g.Dispose();
            }
            else if (e.KeyCode == Keys.A)
            {
                Graphics g = Graphics.FromImage(Canvas.Image);
                g.Clear(Color.Black);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;


                foreach (var p in polygons)
                {
                    p.RotateVectorZ(0.3f, deltaX, deltaY, deltaZ);
                    g.DrawPath(pen, p.GetGraphicsPath());
                }

                //Canvas.Invalidate();
                g.Dispose();
            }
            else if (e.KeyCode == Keys.D)
            {
                Graphics g = Graphics.FromImage(Canvas.Image);
                g.Clear(Color.Black);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;


                foreach (var p in polygons)
                {
                    p.RotateVectorZ(-0.3f, deltaX, deltaY, deltaZ);
                    g.DrawPath(pen, p.GetGraphicsPath());
                }

                
                //Canvas.Invalidate();
                g.Dispose();
            }

            Canvas.Invalidate();
        }

        
    }
}
