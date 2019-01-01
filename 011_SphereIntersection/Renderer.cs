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

            Draw(color, PrimitiveType.Lines);
        }

        public void DrawPoint(vec3 point, vec3 color)
        {
            positions = new float[]
            {
                point.x, point.y, point.z,
            };

            Draw(color, PrimitiveType.Points);
        }

        public void DrawRectangle(vec3 position, vec2 size, vec3 direction, vec3 color)
        {
            positions = new float[]
            {
                -1, 1, 0,
                -1, -1, 0,
                1, -1, 0,
                1, 1, 0,
            };

            direction = glm.normalize(direction);
            vec3 directionOld = new vec3(0, 0, 1);
            float angle = (float)Math.Acos(glm.dot(glm.normalize(directionOld), glm.normalize(direction)));
            vec3 axis = glm.cross(directionOld, direction);
            mat4 rotation = glm.rotate(angle, axis);
            mat4 model = mat4.identity();
            model = glm.scale(model, new vec3(size, 1));
            model *= rotation;
            model = glm.translate(model, position);
            Shaders.shader.Set("color", color);
            Shaders.shader.Set("model", model);
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * positions.Length, positions, BufferUsageHint.StaticDraw);
            GL.DrawArrays(PrimitiveType.Quads, 0, positions.Length / 3);
        }

        public void DrawCircle(vec3 center, float radius, vec3 color)
        {
            float pi = (float)Math.PI;
            int pieces = 64;
            positions = new float[pieces * 3];
            for (int i = 0; i < pieces; ++i)
            {
                float v = pi * 2 / pieces * i;
                float x = (float)Math.Cos(v);
                float y = (float)Math.Sin(v);
                positions[i * 3] = x * radius;
                positions[i * 3 + 1] = y * radius;
            }

            Draw(color, PrimitiveType.LineLoop);
        }

        private void Draw(vec3 color, PrimitiveType primitiveType)
        {
            Shaders.shader.Set("color", color);
            Shaders.shader.Set("model", mat4.identity());
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * positions.Length, positions, BufferUsageHint.StaticDraw);
            GL.DrawArrays(primitiveType, 0, positions.Length / 3);
        }
    }
}
