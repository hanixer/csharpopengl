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
        Shape triangle;
        PointShape point;
        PointShape pointR;
        PointShape pointQ;
        float s;
        float t;
        vec2 a = new vec2(-0.5f, -0.25f);
        vec2 b = new vec2(0.5f, -0.5f);
        vec2 c = new vec2(0.0f, 0.5f);
        vec2 r;
        vec2 q;
        LineShape lineCQ;

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
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(3.0f);
            var points = new List<vec2>();
            points.Add(a);
            points.Add(b);
            points.Add(c);
            triangle = new Shape(points, new vec3(0.25f, 0.35f, 0.15f));
            point = new PointShape(new vec2(0.0f, 0.0f), new vec3(1.0f, 0.0f, 0.0f));
            pointQ = new PointShape(new vec2(0.0f, 0.0f), new vec3(1.0f, 0.0f, 0.0f));
            points = new List<vec2>();
            points.Add(a);
            points.Add(b);
            lineCQ = new LineShape(a, b, new vec3(0.25f, 0.35f, 0.65f));
        }

        private void GlControl_OnPaintEvent(object sender, PaintEventArgs e)
        {
            GL.ClearColor(0.0f, 0.2f, 0.2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            triangle.Draw();
            pointQ.Draw();
            lineCQ.Draw();

            glControl.SwapBuffers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            glControl.Invalidate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float t = (float)trackBar1.Value / 100;
            q = (1 - t) * a + t * b;
            pointQ.Set(q);
            lineCQ.Set(c, q);
            glControl.Invalidate();
        }
    }
}
