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


namespace GraphicsModeler.MainWindow
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            _canvas.Size = this.ClientSize;
            _canvas.Image = new Bitmap(_canvas.Width, _canvas.Width);
            parser = new ObjectFileParser(@"C:\Users\Kamar\Desktop\Model.obj");
        }

        private float deltaX;
        private float deltaY;
        private float deltaZ;
        private ObjectFileParser parser;
        private List<Polygon> polygons;

        private void MainWindow_Load(object sender, EventArgs e)
        {
            deltaX = _canvas.Width / 2;
            deltaY = _canvas.Height / 2;
            deltaZ = 0;

            polygons = parser.GetPolygons();
            Graphics g = Graphics.FromImage(_canvas.Image);
            g.Clear(Color.White);
            GraphicsPath path = new GraphicsPath();
            foreach (var p in polygons)
            {
                p.DeltaOffsets(deltaX, deltaY, deltaZ);
                p.ScalePolygon(150f, deltaX, deltaY, deltaZ);

                foreach (var point in p.GetDDALine(0, 1))
                {
                    path.AddLine(point, new PointF(point.X + 0.2f, point.Y + 0.2f));
                }
                foreach (var point in p.GetDDALine(1, 2))
                {
                    path.AddLine(point, new PointF(point.X + 0.2f, point.Y + 0.2f));
                }
                foreach (var point in p.GetDDALine(2, 0))
                {
                    path.AddLine(point, new PointF(point.X + 0.2f, point.Y + 0.2f));
                }

            }

            g.DrawPath(Pens.Black, path);
            _canvas.Refresh();
            g.Dispose();
        }
    }
}
