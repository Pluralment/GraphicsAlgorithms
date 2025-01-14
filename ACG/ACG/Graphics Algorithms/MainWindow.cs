﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GraphicsModeler.Parser;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Numerics;
using GraphicsModeler.Extensions;
using GraphicsModeler.Helper;

namespace GraphicsModeler.MainWindow
{
    public partial class MainWindow : Form
    {
        private Vector3 translation;
        private ObjectFileParser parser;
        private Model model;

        public MainWindow()
        {
            InitializeComponent();
            _canvas.Size = this.ClientSize;
            _canvas.Image = new Bitmap(_canvas.Width, _canvas.Height);
            parser = new ObjectFileParser(@"C:\Users\Kamar\Desktop\Airplane.obj");
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            translation.X = _canvas.Width / 2;
            translation.Y = _canvas.Height / 2;
            model = parser.GetModel();
            _drawTimer.Enabled = true;
            model.Vertexes.Translate(translation);
            model.Vertexes.ScaleVectors(150f, translation);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                model.Vertexes.RotateVectors(new Vector3(0.2f, 0f, 0f), translation);
            }
            else if (e.KeyCode == Keys.Up)
            {
                model.Vertexes.RotateVectors(new Vector3(-0.2f, 0f, 0f), translation);
            }
            else if (e.KeyCode == Keys.D)
            {
                model.Vertexes.RotateVectors(new Vector3(0f, 0f, 0.2f), translation);
            }
            else if (e.KeyCode == Keys.A)
            {
                model.Vertexes.RotateVectors(new Vector3(0f, 0f, -0.2f), translation);
            }
            else if (e.KeyCode == Keys.Right)
            {
                model.Vertexes.RotateVectors(new Vector3(0f, 0.2f, 0f), translation);
            }
            else if (e.KeyCode == Keys.Left)
            {
                model.Vertexes.RotateVectors(new Vector3(0f, -0.2f, 0f), translation);
            }
            else if (e.KeyCode == Keys.W)
            {
                model.Vertexes.ScaleVectors(1.2f, translation);
            }
            else if (e.KeyCode == Keys.S)
            {
                model.Vertexes.ScaleVectors(0.8f, translation);
            }





        }


        private void MainWindow_Resize(object sender, EventArgs e)
        {
            _canvas.Size = this.ClientSize;
            if (model != null)
            {
                model.Vertexes.Translate(new Vector3(-translation.X, -translation.Y, -translation.Z));
                translation = new Vector3(_canvas.Width / 2, _canvas.Height / 2, 0);
                model.Vertexes.Translate(translation);
            }
        }

        private void _drawTimer_Tick(object sender, EventArgs e)
        {
            var bmp = new ExtendedBitmap(_canvas.Width, _canvas.Height);
            model.DrawToBitmap(bmp);
            _canvas.Image = bmp.Source;
        }

        private void _canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                model.Vertexes.RotateVectors(new Vector3(0.2f, 0f, 0f), translation);
            }
        }
    }
}
