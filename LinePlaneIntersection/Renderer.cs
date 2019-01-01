using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using GlmNet;

namespace WindowsFormsApplication1
{
    class Renderer
    {
        int vao;
        int vbo;
        float[] positions;

        public void Init()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void DrawLine(vec3 start, vec3 end, vec3 color)
        {
            positions = new float[]
            {
                start.x, start.y, start.z,
                end.x, end.y, end.z,
            };

            Shaders.shader.Set("color", color);
            Shaders.shader.Set("model", mat4.identity());
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * positions.Length, positions, BufferUsageHint.StaticDraw);
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
        }
    }
}
