using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL4;
using Utilities;

namespace WindowsFormsApplication1
{
    public class MyControl : GLControl
    {
         float[] verticesPosition = new float[]{
    -0.5f, -0.25f, 0.0f,
     0.5f, -0.5f, 0.0f,
     0.0f,  0.5f, 0.0f
  };

        int vbo;
        int vao;
        Shader shader;

        public MyControl()
        {
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            shader.Use();
            GL.Enable(EnableCap.LineSmooth);
            GL.ClearColor(0.0f, 0.2f, 0.2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.LineLoop, 0, verticesPosition.Length / 3);
            GL.Disable(EnableCap.LineSmooth);

            SwapBuffers();
        }
    }
}
