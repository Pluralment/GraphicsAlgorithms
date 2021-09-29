using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GraphicsModeler.Parser;
using GraphicsModeler.Shapes;
using System.Drawing.Drawing2D;
using GraphicsModeler.Extensions;


namespace GraphicsModeler.MainWindow
{
    public partial class MainWindow : Form
    {
        private float deltaX;
        private float deltaY;
        private float deltaZ;
        private ObjectFileParser parser;
        private List<Polygon> polygons;
        private Graphics hdc;
        private Bitmap _bitmap;

        public MainWindow()
        {
            InitializeComponent();
            _canvas.Size = this.ClientSize;
            _bitmap = new Bitmap(_canvas.Width, _canvas.Width);
            _canvas.Image = _bitmap;
            parser = new ObjectFileParser(@"C:\Users\Kamar\Desktop\Model2.obj");
            hdc = Graphics.FromImage(_canvas.Image);
            hdc.SmoothingMode = SmoothingMode.HighSpeed;
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            deltaX = _canvas.Width / 2;
            deltaY = _canvas.Height / 2;
            deltaZ = 0;

            polygons = parser.GetPolygons();
            hdc.Clear(Color.White);
            foreach(var p in polygons)
            {
                p.Translate(deltaX, deltaY, deltaZ);
                p.ScalePolygon(150f, deltaX, deltaY, deltaZ);
                foreach (var point in p.GetDDALine(0, 1))
                {
                    _bitmap.SetPixel((int)point.X, (int)point.Y, Color.Black);
                }
                foreach (var point in p.GetDDALine(1, 2))
                {
                    _bitmap.SetPixel((int)point.X, (int)point.Y, Color.Black);
                }
                foreach (var point in p.GetDDALine(2, 0))
                {
                    _bitmap.SetPixel((int)point.X, (int)point.Y, Color.Black);
                }
            }

            _canvas.Refresh();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            var bitmap = (Bitmap)_canvas.Image;
            if (e.KeyCode == Keys.W)
            {
                Parallel.ForEach(polygons, p =>
                {
                    p.ScalePolygon(1.1f, deltaX, deltaY, deltaZ);
                });
            }
            else if (e.KeyCode == Keys.S)
            {
                Parallel.ForEach(polygons, p =>
                {
                    p.ScalePolygon(0.9f, deltaX, deltaY, deltaZ);
                });
            }
            else if (e.KeyCode == Keys.Up)
            {
                Parallel.ForEach(polygons, p =>
                {
                    p.RotateVectorX(0.1f, deltaX, deltaY, deltaZ);
                });
            }
            else if (e.KeyCode == Keys.Down)
            {
                Parallel.ForEach(polygons, p =>
                {
                    p.RotateVectorX(-0.1f, deltaX, deltaY, deltaZ);
                });
            }
            else if (e.KeyCode == Keys.A)
            {
                Parallel.ForEach(polygons, p =>
                {
                    p.RotateVectorZ(-0.1f, deltaX, deltaY, deltaZ);
                });
            }
            else if (e.KeyCode == Keys.D)
            {
                Parallel.ForEach(polygons, p =>
                {
                    p.RotateVectorZ(0.1f, deltaX, deltaY, deltaZ);
                });
            }
            else if (e.KeyCode == Keys.Left)
            {
                Parallel.ForEach(polygons, p =>
                {
                    p.RotateVectorY(-0.1f, deltaX, deltaY, deltaZ);
                });
            }
            else if (e.KeyCode == Keys.Right)
            {
                Parallel.ForEach(polygons, p =>
                {
                    p.RotateVectorY(0.1f, deltaX, deltaY, deltaZ);
                });
            }

            hdc.Clear(Color.White);
            foreach (var p in polygons)
            {
                //hdc.DrawLine(Pens.Black, new PointF(p.Vectors[0].X, p.Vectors[0].Y), new PointF(p.Vectors[1].X, p.Vectors[1].Y));
                //hdc.DrawLine(Pens.Black, new PointF(p.Vectors[1].X, p.Vectors[1].Y), new PointF(p.Vectors[2].X, p.Vectors[2].Y));
                //hdc.DrawLine(Pens.Black, new PointF(p.Vectors[0].X, p.Vectors[0].Y), new PointF(p.Vectors[2].X, p.Vectors[2].Y));
                foreach (var point in p.GetDDALine(0, 1))
                {
                    _bitmap.SetPixel((int)point.X, (int)point.Y, Color.Black);
                }
                foreach (var point in p.GetDDALine(1, 2))
                {
                    _bitmap.SetPixel((int)point.X, (int)point.Y, Color.Black);
                }
                foreach (var point in p.GetDDALine(2, 0))
                {
                    _bitmap.SetPixel((int)point.X, (int)point.Y, Color.Black);
                }
            }

            _canvas.Refresh();
        }

        
        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (polygons != null)
            {
                _canvas.Size = this.ClientSize;
                _bitmap = new Bitmap(_canvas.Width, _canvas.Height);
                _canvas.Image = _bitmap;
                hdc.Dispose();
                hdc = Graphics.FromImage(_canvas.Image);

                Parallel.ForEach(polygons, p =>
                {
                    p.Translate(-deltaX, -deltaY, -deltaZ);
                });

                deltaX = _canvas.Width / 2;
                deltaY = _canvas.Height / 2;

                Parallel.ForEach(polygons, p =>
                {
                    p.Translate(deltaX, deltaY, deltaZ);
                });

                hdc.Clear(Color.White);
                foreach (var p in polygons)
                {
                    foreach (var point in p.GetDDALine(0, 1))
                    {
                        _bitmap.SetPixel((int)point.X, (int)point.Y, Color.Black);
                    }
                    foreach (var point in p.GetDDALine(1, 2))
                    {
                        _bitmap.SetPixel((int)point.X, (int)point.Y, Color.Black);
                    }
                    foreach (var point in p.GetDDALine(2, 0))
                    {
                        _bitmap.SetPixel((int)point.X, (int)point.Y, Color.Black);
                    }
                }

                _canvas.Refresh();
            }
        }
    }
}
