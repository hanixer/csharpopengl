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
        float cameraAngleX = 89.5f;
        float cameraAngleY = 0.0f;
        float cameraDistance = 50;
        float scaleXY = 0.1f;
        float heightFactor = 0.05f;

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
            trackBar1.ValueChanged += trackBar1_Scroll;
            trackBar2.ValueChanged += trackBar2_Scroll;
            trackBar3.ValueChanged += trackBar3_Scroll;
            KeyPress += Form1_KeyPress;
            
            Controls.Add(glControl);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
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
            //GL.LineWidth(3.0f);

            Shaders.Init();
            renderer.Init();

            var result = Renderer.GeneratePositions(1, 4, 8);

            UpdateRotation();
            LoadImage();
        }

        private void LoadImage()
        {
            Bitmap bitmap = new Bitmap("map.jpg");
            heightfield2 = bitmapToHeightfield(bitmap);
        }

        float[,] heightfield2;
        float[,] bitmapToHeightfield(Bitmap bitmap)
        {
            float[,] result = new float[bitmap.Height, bitmap.Width];
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    Color c = bitmap.GetPixel(j, i);
                    result[i, j] = (c.R + c.G + c.B) / 3 * heightFactor;
                }
            }

            return result;
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

            float[,] heightfield = new float[,]
            {
                {1, 2, 3, 4, 4, 1, 1, 3, 4,},
                {1, 3, 6, 4, 4, 4, 4, 5, 1,},
                {2, 3, 4, 4, 3, 4, 4, 1, 1,},
                {2, 3, 4, 4, 3, 4, 4, 1, 1,},
                {2, 3, 4, 4, 3, 4, 4, 1, 1,},
                {2, 3, 4, 4, 3, 4, 4, 1, 1,},
                {2, 3, 4, 4, 3, 4, 4, 1, 1,},
                {2, 3, 3, 3, 3, 3, 3, 1, 1,},
                {2, 1, 1, 1, 1, 1, 1, 1, 1,},
                {1, 1, 1, 1, 1, 1, 1, 1, 1,},
                {1, 1, 1, 1, 1, 1, 1, 1, 1,},
                {1, 1, 1, 1, 1, 1, 1, 1, 1,},
                {1, 1, 1, 1, 1, 1, 1, 1, 1,},
                {1, 1, 1, 1, 1, 1, 1, 1, 1,},
            };
            renderer.DrawHeightfield(heightfield2, scaleXY);

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

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float value = ConvertTrackbarValue(trackBar1.Value);
            label1.Text = value.ToString();
            rayDirection.x = value;
            glControl.Invalidate();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            float value = ConvertTrackbarValue(trackBar2.Value);
            label2.Text = value.ToString();
            rayDirection.y = value;
            glControl.Invalidate();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            float value = ConvertTrackbarValue(trackBar3.Value);
            label3.Text = value.ToString();
            rayDirection.z = value;
            glControl.Invalidate();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                scaleXY = float.Parse(textBox1.Text);
                glControl.Invalidate();
            }
            catch(Exception)
            {

            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                heightFactor = float.Parse(textBox2.Text);
                LoadImage();
                glControl.Invalidate();
            }
            catch (Exception)
            {

            }

        }
    }
}
