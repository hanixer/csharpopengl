using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using OpenTK.Graphics.OpenGL4;
using GlmNet;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        MyControl glControl;
        Shape shape;
        PointShape point;

        public Form1()
        {
            InitializeComponent();

            Width = 800;
            Height = 600;

            glControl = new MyControl();
            glControl.Location = new Point(Width / 4, Height / 4);
            glControl.Size = new Size(Width / 2, Height / 2);
            glControl.OnPaintEvent += GlControl_OnPaintEvent;
            glControl.OnLoadEvent += GlControl_OnLoadEvent;
            Controls.Add(glControl);
        }

        private void GlControl_OnLoadEvent(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.ProgramPointSize);
            var points = new List<vec2>();
            points.Add(new vec2(-0.5f, -0.25f));
            points.Add(new vec2(0.5f, -0.5f));
            points.Add(new vec2(0.0f, 0.5f));
            shape = new Shape(points, new vec3(0.25f, 0.35f, 0.15f));
            point = new PointShape(new vec2(0.0f, 0.0f), new vec3(1.0f, 0.0f, 0.0f));
        }

        private void GlControl_OnPaintEvent(object sender, PaintEventArgs e)
        {
            GL.ClearColor(0.0f, 0.2f, 0.2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            shape.Draw();
            point.Draw();


            glControl.SwapBuffers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            glControl.Invalidate();
        }
    }
}
