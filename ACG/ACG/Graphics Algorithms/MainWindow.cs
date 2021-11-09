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
        
        private Vector3 cameraPosition;
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
            model.Scale = 1f;
            model.Rotation = Vector3.Zero;

            modelPosition = Vector3.Zero;

            model.Position = modelPosition;

            _drawTimer.Enabled = true;
            
            cameraPosition = new Vector3(0, 0, 5);

            camera = new Camera
            {
                Position = cameraPosition,
                Target = new Vector3(0, 0, 0),
                Up = new Vector3(0f , 1f, 0f),
                Width = _canvas.Width,
                Height = _canvas.Height,
                Fov = (float)Math.PI / 4
            };
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                modelPosition.Y += 0.1f;
                model.Position = modelPosition;
            }
            else if (e.KeyCode == Keys.Up)
            {
                modelPosition.Y -= 0.1f;
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
                modelPosition.X -= 0.1f;
                model.Position = modelPosition;
            }
            else if (e.KeyCode == Keys.Left)
            {
                modelPosition.X += 0.1f;
                model.Position = modelPosition;
            }
            else if (e.KeyCode == Keys.W)
            {
                modelPosition.Z += 0.1f;
                model.Position = modelPosition;
            }
            else if (e.KeyCode == Keys.S)
            {
                modelPosition.Z -= 0.1f;
                model.Position = modelPosition;
            }
            else if (e.KeyCode == Keys.B)
            {
                modelRotation.X -= 0.1f;
                model.Rotation = modelRotation;
            }
            else if (e.KeyCode == Keys.N)
            {
                modelRotation.X += 0.1f;
                model.Rotation = modelRotation;
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
        }
        
        private void _drawTimer_Tick(object sender, EventArgs e)
        {
            var bmp = new ExtendedBitmap(_canvas.Width, _canvas.Height);
            _drawer.Draw(bmp, VertexTransformator.Transform(model, camera), camera);
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
