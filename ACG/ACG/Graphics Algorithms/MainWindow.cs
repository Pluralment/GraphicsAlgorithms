using System;
using System.Drawing;
using System.Windows.Forms;
using GraphicsModeler.Parser;
using System.Numerics;
using GraphicsModeler.Extensions;
using GraphicsModeler.Helper;
using GraphicsModeler.Scene;

namespace GraphicsModeler.MainWindow
{
    public partial class MainWindow : Form
    {
        private readonly ObjectFileParser parser;
        private Drawer _drawer = new Drawer();
        
        private Model model;
        private Vector3 modelPosition;
        private Vector3 modelRotation = Vector3.Zero;
        private Camera camera;

        private Vector3 viewCamera = new Vector3(0, 0, 10);
        private Vector3 viewTarget = new Vector3(0, 0, -10f);
        private Vector3 viewUp = new Vector3(0f , 1f, 0f);

        public MainWindow()
        {
            InitializeComponent();
            _canvas.Size = this.ClientSize;
            _canvas.Image = new Bitmap(_canvas.Width, _canvas.Height);
            parser = new ObjectFileParser();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            model = parser.CreateModel(@"Model.obj");
            model.Scale = 200f;
            model.Rotation = Vector3.Zero;
            
            modelPosition = new Vector3(
                (float)_canvas.Width / 2,
                (float)_canvas.Height / 2,
                _canvas.Width
            );
            
            model.Position = modelPosition;


            _drawTimer.Enabled = true;

            camera = new Camera
            {
                Position = new Vector3(0, 0, 50),
                Target = new Vector3(0, 0, -1f),
                Up = new Vector3(0f , 1f, 0f),
                Width = _canvas.Width,
                Height = _canvas.Height
            };
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                modelPosition.Y += 10f;
                model.Position = modelPosition;
            }
            else if (e.KeyCode == Keys.Up)
            {
                modelPosition.Y -= 10f;
                model.Position = modelPosition;
            }
            else if (e.KeyCode == Keys.D)
            {
                modelRotation.Y -= 0.2f;
                model.Rotation = modelRotation;
            }
            else if (e.KeyCode == Keys.A)
            {
                modelRotation.Y += 0.2f;
                model.Rotation = modelRotation;
            }
            else if (e.KeyCode == Keys.Right)
            {
                modelPosition.X += 10f;
                model.Position = modelPosition;
            }
            else if (e.KeyCode == Keys.Left)
            {
                modelPosition.X -= 10f;
                model.Position = modelPosition;
            }
            else if (e.KeyCode == Keys.W)
            {
                model.Scale += 5f;
            }
            else if (e.KeyCode == Keys.S)
            {
                model.Scale -= 5f;
            }
        }
        
        private void MainWindow_Resize(object sender, EventArgs e)
        {
            _canvas.Size = this.ClientSize;
            
            if (camera != null)
            {
                camera.Width = _canvas.Width;
                camera.Height = _canvas.Height;
            }

            if (model != null)
            {
                modelPosition = new Vector3((float)_canvas.Width / 2, (float)_canvas.Height / 2, modelPosition.Z);
                model.Position = modelPosition;
            }
        }
        
        private void _drawTimer_Tick(object sender, EventArgs e)
        {
            var bmp = new ExtendedBitmap(_canvas.Width, _canvas.Height);
            _drawer.Draw(bmp, model, VertexTransformator.Transform(model, camera));
            _canvas.Image = bmp.Source;
        }

        private void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                
            }
        }
    }
}
