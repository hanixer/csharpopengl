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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        MyControl glControl;
        float[] verticesPosition = new float[]{
    -0.5f, -0.25f, 0.0f,
     0.5f, -0.5f, 0.0f,
     0.0f,  0.5f, 0.0f
  };
        int vbo;
        int vao;
        Shader shader;

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
            vbo = GL.GenBuffer();
            vao = GL.GenVertexArray();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * verticesPosition.Length, verticesPosition, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.Enable(EnableCap.VertexProgramPointSize);

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();
        }
        float color = 0.1f;

        private void GlControl_OnPaintEvent(object sender, PaintEventArgs e)
        {
            shader.Use();
            GL.Enable(EnableCap.LineSmooth);
            GL.ClearColor(0.0f, 0.2f, color, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, verticesPosition.Length / 3);
            GL.Disable(EnableCap.LineSmooth);

            glControl.SwapBuffers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            verticesPosition[3] += 0.10f;
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * verticesPosition.Length, verticesPosition, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            color += 0.1f;
            glControl.Invalidate();
        }
    }
}
