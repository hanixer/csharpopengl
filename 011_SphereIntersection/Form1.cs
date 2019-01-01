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
        Renderer renderer = new Renderer();
        Cube cube;
        vec3 rayPoint = new vec3(0, 5, 5);
        vec3 rayDirection;
        vec3 planePoint = new vec3(0);
        vec3 planeDirection = new vec3(0, 0, 1);
        vec2 planeSize = new vec2(5, 5);
        float z;
        bool isLeftMouseDown = false;
        bool isRightMouseDown = false;
        float mouseX;
        float mouseY;
        float cameraAngleX = 45.0f;
        float cameraAngleY = -45.0f;
        float cameraDistance = 25;

        public Form1()
        {
            InitializeComponent();

            rayDirection = glm.normalize(new vec3(0) - rayPoint);

            glControl = new MyControl();
            glControl.Location = new Point(Width / 4, Height / 4);
            glControl.Size = new Size(Width / 2, Width / 2);
            glControl.OnPaintEvent += GlControl_OnPaintEvent;
            glControl.OnLoadEvent += GlControl_OnLoadEvent;
            glControl.MouseDown += GlControl_MouseDown;
            glControl.MouseUp += GlControl_MouseUp;
            glControl.MouseMove += GlControl_MouseMove;
            Controls.Add(glControl);
        }

        private void LinkTrackBarWithLabel(TrackBar trackBar, Label label)
        {
            trackBar.Scroll += (sender, e) =>
            {
                label.Text = ConvertTrackbarValue(trackBar.Value).ToString();
            };
        }

        private float ConvertTrackbarValue(int value) => value / 10.0f;

        private void Init()
        {
            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.DepthTest);
            GL.LineWidth(3.0f);

            Shaders.Init();
            renderer.Init();

            cube = new Cube();
            planeDirection = new vec3(1, 1, 1);
            UpdateRotation();
        }

        private void Draw()
        {
            GL.ClearColor(0.0f, 0.2f, 0.2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            var intersectionPoint = ComputeLinePlaneIntersectionPoint();           
            

            if (Height != 0)
            {
                SetViewAndProjection();
            }

            renderer.DrawLine(new vec3(0), new vec3(10, 0, 0), new vec3(1, 0, 0));
            renderer.DrawLine(new vec3(0), new vec3(0, 10, 0), new vec3(0, 1, 0));
            renderer.DrawLine(new vec3(0), new vec3(0, 0, 10), new vec3(0, 0, 1));
            renderer.DrawCircle(new vec3(0), 5, new vec3(0));

            glControl.SwapBuffers();
        }

        private void GlControl_OnLoadEvent(object sender, EventArgs e)
        {
            Init();
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
                cameraDistance += (e.Y - mouseY) / 5;
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

        private void GlControl_OnPaintEvent(object sender, PaintEventArgs e)
        {
            Draw();
        }

        string Str(vec3 v) => String.Format("{0}, {1}, {2}", v.x, v.y, v.z);

        vec3? ComputeLinePlaneIntersectionPoint()
        {
            float t = glm.dot(planePoint - rayPoint, planeDirection) / glm.dot(rayDirection, planeDirection);
            if (t >= 0)
                return rayPoint + t * rayDirection;
            else
                return null;
            //var result = rayPoint + z * glm.normalize(rayDirection);
            //return result;
        }

        void DrawAxes()
        {
            
        }

        private void SetViewAndProjection()
        {
            float fov = 45;
            float near = 0.1f;
            float far = 100;

            mat4 view = mat4.identity();
            view = glm.translate(view, new vec3(0, 0, -cameraDistance));
            view = glm.rotate(view, cameraAngleX, new vec3(1, 0, 0));
            view = glm.rotate(view, cameraAngleY, new vec3(0, 1, 0));
            Shaders.shader.Set("view", view);

            mat4 projection = glm.perspective(glm.radians(fov), Width / Height, near, far);
            Shaders.shader.Set("projection", projection);
        }

        private void UpdateRotation()
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            glControl.Invalidate();
        }
    }
}
