using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphicsModeler.Parser;

namespace GraphicsModeler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObjectFileParser _parser = new ObjectFileParser(@"C:\Users\Kamar\Desktop\Model.obj");
        private float _deltaX = 0;
        private float _deltaY = 0;
        private float _deltaZ = 0;

        public MainWindow()
        {
            InitializeComponent();

            var polygons = _parser.GetPolygons();
            GeometryGroup filledPolygon = new GeometryGroup();
            Parallel.ForEach(polygons, (p) =>
            {
                p.DeltaOffsets(_deltaX, _deltaY, _deltaZ);
                p.ScalePolygon(150f, _deltaX, _deltaY, _deltaZ);
                filledPolygon = new GeometryGroup();

                foreach (var line in GetPointsBetweenTwo(
                    new Point(p.Vectors[0].X, p.Vectors[0].Y),
                    new Point(p.Vectors[1].X, p.Vectors[1].Y)))
                {
                    filledPolygon.Children.Add(new LineGeometry(line, new Point(line.X + 0.1, line.Y + 0.1)));
                }

                foreach (var line in GetPointsBetweenTwo(
                    new Point(p.Vectors[2].X, p.Vectors[2].Y),
                    new Point(p.Vectors[1].X, p.Vectors[1].Y)))
                {
                    filledPolygon.Children.Add(new LineGeometry(line, new Point(line.X + 0.1, line.Y + 0.1)));
                }

                foreach (var line in GetPointsBetweenTwo(
                    new Point(p.Vectors[0].X, p.Vectors[0].Y),
                    new Point(p.Vectors[2].X, p.Vectors[2].Y)))
                {
                    filledPolygon.Children.Add(new LineGeometry(line, new Point(line.X + 0.1, line.Y + 0.1)));
                }
            });

            _canvas.Children.Add(new Path()
            {
                Data = filledPolygon,
                Stroke = Brushes.Black
            });
        }


        private IEnumerable<Point> GetPointsBetweenTwo(Point p1, Point p2)
        {
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            var L = Math.Max(Math.Abs(dx), Math.Abs(dy));
            var xInc = dx / L;
            var yInc = dy / L;
            var x = p1.X;
            var y = p1.Y;

            for (int i = 0; i <= L; i++)
            {
                yield return new Point(x, y);
                x += xInc;
                y += yInc;
            }
        }
        
    }
}
