using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Utilities;

namespace WindowsFormsApplication1
{
    class ShapeBase
    {
        protected int vao;
        protected int vbo;
        protected vec3 color;
        protected Shader shader;
        protected float[] vertices;
        public ShapeBase(vec3 color)
        {
            this.color = color;

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();
            shader.Set("uniformColor", color);

            vbo = GL.GenBuffer();
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

    }
}
