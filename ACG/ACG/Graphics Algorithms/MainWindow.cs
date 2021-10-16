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
        private ObjectFileParser parser;
        private Model model;
        private Vector3 modelPosition;
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
            model = parser.CreateModel(@"gun.obj");
            modelPosition = model.Position;
            modelPosition.X = (float)_canvas.Width / 2;
            modelPosition.Y = (float)_canvas.Height / 2;
            modelPosition.Z = -(float)_canvas.Width;
            _drawTimer.Enabled = true;

            camera = new Camera(
                new Vector3(0, 0, 10),
                new Vector3(0, 0, -10f),
                new Vector3(0f , 1f, 0f)
                );
            
            // MOVE TO A CLASS.
            model.Mesh.Vertices.ToWorld(modelPosition);
            model.Mesh.Vertices.ScaleVectors(150f, modelPosition);
            
            model.Mesh.Vertices.ToView(camera.Position, camera.Target, camera.Up);
            model.Mesh.Vertices.ToPerspective((float)(Math.PI / 2), _canvas.Width, _canvas.Height);
            model.Mesh.Vertices.ToViewport(_canvas.Width, _canvas.Height);
            //
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                model.Mesh.Vertices.RotateVectors(new Vector3(0.2f, 0f, 0f), modelPosition);
            }
            else if (e.KeyCode == Keys.Up)
            {
                model.Mesh.Vertices.RotateVectors(new Vector3(-0.2f, 0f, 0f), modelPosition);
            }
            else if (e.KeyCode == Keys.D)
            {
                model.Mesh.Vertices.RotateVectors(new Vector3(0f, 0f, 0.2f), modelPosition);
            }
            else if (e.KeyCode == Keys.A)
            {
                model.Mesh.Vertices.RotateVectors(new Vector3(0f, 0f, -0.2f), modelPosition);
            }
            else if (e.KeyCode == Keys.Right)
            {
                model.Mesh.Vertices.RotateVectors(new Vector3(0f, 0.2f, 0f), modelPosition);
            }
            else if (e.KeyCode == Keys.Left)
            {
                model.Mesh.Vertices.RotateVectors(new Vector3(0f, -0.2f, 0f), modelPosition);
            }
            else if (e.KeyCode == Keys.W)
            {
                model.Mesh.Vertices.ScaleVectors(1.2f, modelPosition);
            }
            else if (e.KeyCode == Keys.S)
            {
                model.Mesh.Vertices.ScaleVectors(0.8f, modelPosition);
            }
        }


        private void MainWindow_Resize(object sender, EventArgs e)
        {
            _canvas.Size = this.ClientSize;
            if (model != null)
            {
                model.Mesh.Vertices.Translate(new Vector3(-modelPosition.X, -modelPosition.Y, -modelPosition.Z));
                modelPosition = new Vector3((float)_canvas.Width / 2, (float)_canvas.Height / 2, 0);
                model.Mesh.Vertices.Translate(modelPosition);
            }
        }

        private void _drawTimer_Tick(object sender, EventArgs e)
        {
            // modelTransformator.Transform(model, camera);
            // renderer.Render(bmp);
            var bmp = new ExtendedBitmap(_canvas.Width, _canvas.Height);
            model.Draw(bmp);
            _canvas.Image = bmp.Source;
        }

        private void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                model.Mesh.Vertices.RotateVectors(new Vector3(0.2f, 0f, 0f), modelPosition);
            }
        }
    }
}
