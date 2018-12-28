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
        public List<vec2> points;
        int vao;
        int vbo;
        vec3 color;
        Shader shader;
        protected PrimitiveType primitiveType = PrimitiveType.Triangles;
        float[] vertices;

        public Shape(List<vec2> points, vec3 color)
        {
            this.points = points;
            this.color = color;
            vertices = new float[points.Count * 3];

            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();
            shader.Set("uniformColor", color);
            Console.WriteLine(GL.GetError().ToString());

            vbo = GL.GenBuffer();
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.BindVertexArray(0);
        }

        public virtual void Draw()
        {
            for (int i = 0; i < points.Count; ++i)
            {
                vertices[i * 3] = points[i].x;
                vertices[i * 3 + 1] = points[i].y;
            }

            shader.Use();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float),
                vertices, BufferUsageHint.StaticDraw);
            GL.DrawArrays(primitiveType, 0, points.Count);
        }
    }
}
