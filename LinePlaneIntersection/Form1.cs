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
        Rectangle3D plane;
        Line3D line;
        vec3 direction = new vec3(0, 0, 1);

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

            plane = new Rectangle3D(new List<vec3> {
                new vec3(-0.5f, -0.5f, 0.0f),
                new vec3(-0.5f, 0.5f, 0.0f),
                new vec3(0.5f, 0.5f, 0.0f),
                new vec3(0.5f, -0.5f, 0.0f),
            });
            line = new Line3D(new vec3(0, 0, 0), direction);
        }

        private void GlControl_OnPaintEvent(object sender, PaintEventArgs e)
        {
            GL.ClearColor(0.0f, 0.2f, 0.2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            plane.Draw();
            line.Draw();

            glControl.SwapBuffers();
        }

        private void MakePlaneWithNorm(vec3 point, vec3 normal, Rectangle3D plane, Rectangle3D normShape)
        {


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            float angle;
            if (float.TryParse(textBox1.Text, out angle))
            {
                plane.Rotate(angle);
                glControl.Invalidate();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            direction.x = trackBar1.Value / 100.0f;
            UpdateRotation();
            glControl.Invalidate();
        }

        private void UpdateRotation()
        {
            Console.WriteLine("{0},{1},{2}",direction.x, direction.y, direction.z);
            plane.Rotate(direction);
            line.Rotate(direction);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            direction.y = trackBar2.Value / 100.0f;
            UpdateRotation();
            glControl.Invalidate();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            direction.z = trackBar3.Value / 100.0f;
            UpdateRotation();
            glControl.Invalidate();

        }
    }
}
