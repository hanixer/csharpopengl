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
        vec3 cameraPos = new vec3(0.0f, 0.0f, 3.0f);
        float cameraSpeed = 0.05f * 4;
        vec3 cameraFront = new vec3(0.0f, 0.0f, -1.0f);
        vec3 cameraUp = new vec3(0.0f, 1.0f, 0.0f);
        Timer timer = new Timer();

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

            timer.Interval = 100;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space)
            {
                cameraPos.y += 0.1f;
                glControl.Invalidate();
            }
            if (keyData == Keys.Z)
            {
                cameraPos.y -= 0.1f;
                glControl.Invalidate();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var state = OpenTK.Input.Keyboard.GetState();

            if (state.IsKeyDown(OpenTK.Input.Key.W))
            {
                cameraPos += cameraSpeed * cameraFront;
                glControl.Invalidate();
            }
            if (state.IsKeyDown(OpenTK.Input.Key.S))
            {
                cameraPos -= cameraSpeed * cameraFront;
                glControl.Invalidate();
            }
            if (state.IsKeyDown(OpenTK.Input.Key.A))
            {
                cameraPos -= glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;
                glControl.Invalidate();
            }
            if (state.IsKeyDown(OpenTK.Input.Key.D))
            {
                cameraPos += glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;
                glControl.Invalidate();
            }
        }

        //private void Form1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    var state = OpenTK.Input.Keyboard.GetState();
            
        //    if (state.IsKeyDown(OpenTK.Input.Key.W))
        //    {
        //        cameraPos += cameraSpeed * cameraFront;
        //    }
        //    if (e.KeyChar == 's')
        //    {
        //        cameraPos -= cameraSpeed * cameraFront;
        //    }
        //    if (state.IsKeyDown(OpenTK.Input.Key.A))
        //    {
        //        cameraPos -= glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;
        //    }
        //    if (state.IsKeyDown(OpenTK.Input.Key.D))
        //    {
        //        cameraPos += glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;
        //    }
        //}

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
                //projection = projection * lookAt;
                //projection = projection * glm.translate(mat4.identity(), new vec3(0, 0, -2));
                //projection = mat4.identity();
                Shaders.shader.Set("projection", projection);
                mat4 view = glm.lookAt(cameraPos, cameraPos + cameraFront, cameraUp);
                //view = mat4.identity();
                Shaders.shader.Set("view", view);

                vec4 p = new vec4(plane.Points[0], 1.0f);
                vec4 v = p;
                Console.WriteLine("0) " + v.x + " " + v.y + " " + v.z + " " + v.w);
                v = plane.Model * p;
                Console.WriteLine("1) " + v.x + " " + v.y + " " + v.z + " " + v.w);
                v = view * plane.Model * p;
                Console.WriteLine("2) " + v.x + " " + v.y + " " + v.z + " " + v.w);
                //v = projection * plane.Model * new vec4(-0.05f, -0.05f, 0.0f, 1);
                //Console.WriteLine("2) " + v.x + " " + v.y + " " + v.z + " " + v.w);
                //v = projection * glm.translate(mat4.identity(), new vec3(0, 0, z)) * plane.Model * new vec4(-0.05f, -0.05f, 0.0f, 1);
                //Console.WriteLine("3) " + v.x + " " + v.y + " " + v.z + " " + v.w);
            }

            //plane2.Draw();
            plane.Draw();
            line.Draw();
            //ray.Draw();
            //cube.Draw();

            glControl.SwapBuffers();
        }

        private void MakePlaneWithNorm(vec3 point, vec3 normal, Rectangle3D plane, Rectangle3D normShape)
        {
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
