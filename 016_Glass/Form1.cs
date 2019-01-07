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
        vec3 rayPoint;
        vec3 rayDirection;
        float sphereRadius = 5.0f;
        vec3 spherePosition = new vec3(0);
        bool isLeftMouseDown = false;
        bool isRightMouseDown = false;
        float mouseX;
        float mouseY;
        float cameraAngleX = 44;
        float cameraAngleY = -45;
        float cameraDistance = 235;
        int sectorsCount = 3;
        float[] data = new float[]
        {
            15, 60,
            17, 54,
            18, 48,
            17.5f, 41,
            14, 36,
            10, 32,
            4, 30,
            2.5f, 25,
            2.16f, 17,
            2.72f, 9.84f,
            3.8f, 6.3f,
            7.6f, 3.6f,
            13, 2,
        };

        public Form1()
        {
            InitializeComponent();

            rayPoint = new vec3(5, 4, 3);
            rayDirection = new vec3(0) - rayPoint;

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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F)
            {
                renderer.PolygonMode = PolygonMode.Fill;
                glControl.Invalidate();
            }
            else if (keyData == Keys.D)
            {
                renderer.PolygonMode = PolygonMode.Point;
                glControl.Invalidate();
            }
            else if (keyData == Keys.E)
            {
                renderer.PolygonMode = PolygonMode.Line;
                glControl.Invalidate();
            }
            else if (keyData == Keys.W)
            {
                sectorsCount++;
                glControl.Invalidate();
            }
            else if (keyData == Keys.S)
            {
                sectorsCount--;
                glControl.Invalidate();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Init()
        {
            GL.Enable(EnableCap.ProgramPointSize);
            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.DepthTest);
            //GL.LineWidth(3.0f);

            Shaders.Init();
            renderer.Init();

            var result = Renderer.GeneratePositions(1, 4, 8);

            UpdateRotation();
        }

        private void Draw()
        {
            GL.ClearColor(0.0f, 0.2f, 0.2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (Height != 0)
            {
                SetViewAndProjection();
            }

            renderer.DrawLine(new vec3(0), new vec3(10, 0, 0), new vec3(1, 0, 0));
            renderer.DrawLine(new vec3(0), new vec3(0, 10, 0), new vec3(0, 1, 0));
            renderer.DrawLine(new vec3(0), new vec3(0, 0, 10), new vec3(0, 0, 1));
            //renderer.DrawGlass(data, sectorsCount);
            //renderer.DrawGlass(new float[] { 10, 10, 5, 5, 10, 0 }, sectorsCount);
            renderer.DrawSphere(50, new vec3(0.4f, 0.6f, 0.7f), sectorsCount);

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

        string Str(vec3 v) => String.Format("{0:0.00}, {1:0.00}, {2:0.00}", v.x, v.y, v.z);

        Tuple<vec3, vec3?> ComputeIntersection()
        {
            vec3 v = rayPoint - spherePosition;
            float a = glm.dot(rayDirection, rayDirection);
            float b = 2 * glm.dot(rayDirection, v);
            float c = glm.dot(v, v) - sphereRadius * sphereRadius;
            float underRoot = b * b - 4 * a * c;

            if (underRoot == 0)
            {
                float t = (float)(-b + Math.Sqrt(underRoot)) / (2 * a);
                return new Tuple<vec3, vec3?>(ParameterToPoint(t), null);
            }
            else if (underRoot > 0)
            {
                float t1 = (float)(-b + Math.Sqrt(underRoot)) / (2 * a);
                float t2 = (float)(-b - Math.Sqrt(underRoot)) / (2 * a);
                return new Tuple<vec3, vec3?>(ParameterToPoint(t1), ParameterToPoint(t2));
            }
            else
            {
                return null;
            }
        }

        vec3 ParameterToPoint(float t) => rayPoint + rayDirection * t;

        vec3? ComputeLinePlaneIntersectionPoint()
        {
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
