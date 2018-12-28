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
    class Shape
    {
        List<vec2> points;
        int vao;
        int vbo;
        vec3 color;
        Shader shader;

        public Shape(List<vec2> points, vec3 color)
        {
            this.points = points;
            this.color = color;

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Set("uniformColor", color);

            vbo = GL.GenBuffer();
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindVertexArray(0);
        }

        public void Draw()
        {
            float[] vertices = new float[points.Count * 3];
            for (int i = 0; i < points.Count; ++i)
            {
                vertices[i * 3] = points[i].x;
                vertices[i * 3 + 1] = points[i].y;
            }

            shader.Use();
            shader.Set("uniformColor", color);
            GL.BindVertexArray(vao);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length,
                vertices, BufferUsageHint.StaticDraw);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }
    }
}
