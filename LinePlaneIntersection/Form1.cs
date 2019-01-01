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
        Rectangle3D plane2;
        Line3D line;
        Line3D ray;
        Cube cube;
        vec3 direction = new vec3(0, 0, 1);
        float z;
        bool isLeftMouseDown = false;
        bool isRightMouseDown = false;
        float mouseX;
        float mouseY;
        float cameraAngleX;
        float cameraAngleY;
        float cameraDistance = 3.0f;

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
            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseMove += GlControl_MouseMove;
            Controls.Add(glControl);
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
            {
                isLeftMouseDown = false;
            }
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                isRightMouseDown = false;
            }
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!glControl.ClientRectangle.Contains(glControl.PointToClient(MousePosition)))
            {
                return;
            }

            if (isLeftMouseDown)
            {
                cameraAngleY += (e.X - mouseX) / 100;
                cameraAngleX += (e.Y - mouseY) / 100;
                mouseX = e.X;
                mouseY = e.Y;

                glControl.Invalidate();
            }
            else if (isRightMouseDown)
            {
                cameraDistance += (e.Y - mouseY) / 50;

                mouseX = e.X;
                mouseY = e.Y;

                glControl.Invalidate();
            }
        }
        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button.HasFlag(MouseButtons.Left))
            {
                isLeftMouseDown = true;
            }
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                isRightMouseDown = true;
            }
        }

        private void GlControl_OnLoadEvent(object sender, EventArgs e)
        {
            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(3.0f);

            Shaders.Init();

            plane = new Rectangle3D(new List<vec3> {
                new vec3(-0.5f, -0.5f, 0.0f),
                new vec3(-0.5f, 0.5f, 0.0f),
                new vec3(0.5f, 0.5f, 0.0f),
                new vec3(0.5f, -0.5f, 0.0f),
            });
            plane2 = new Rectangle3D(new List<vec3> {
                new vec3(-0.5f, -0.5f, -.0f),
                new vec3(-0.5f, 0.5f, -.0f),
                new vec3(0.5f, 0.5f, -.0f),
                new vec3(0.5f, -0.5f, -.0f),
            });
            plane2.Color = new vec3(0.5f, 0.5f, 0.95f);
            line = new Line3D(new vec3(0, 0, 0), direction);
            ray = new Line3D(new vec3(-0.5f, -0.5f, 0), new vec3(100, 100, 0));
            ray.Color = new vec3(0.5f, 0.5f, 0.75f);
            cube = new Cube();
        }

        private void GlControl_OnPaintEvent(object sender, PaintEventArgs e)
        {
            GL.ClearColor(0.0f, 0.2f, 0.2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            if (Height != 0)
            {
                float fov = 45;
                float near = 0.1f;
                float far = 100;

                mat4 projection = glm.perspective(glm.radians(fov), Width / Height, near, far);
                Shaders.shader.Set("projection", projection);

                mat4 view = mat4.identity();
                view = glm.translate(view, new vec3(0, 0, -cameraDistance));
                view = glm.rotate(view, cameraAngleX, new vec3(1, 0, 0));
                view = glm.rotate(view, cameraAngleY, new vec3(0, 1, 0));
                Shaders.shader.Set("view", view);
            }

            //plane2.Draw();
            //plane.Draw();
            //line.Draw();
            //ray.Draw();
            cube.Draw();

            glControl.SwapBuffers();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            direction.x = trackBar1.Value / 100.0f;
            UpdateRotation();
            glControl.Invalidate();
        }

        private void UpdateRotation()
        {
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
            plane2.Translate(new vec3(0, 0, direction.z));
            glControl.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            z += 0.1f;
            glControl.Invalidate();
        }
    }
}
