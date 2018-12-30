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
    class Shape2D : ShapeBase
    {
        public List<vec2> points;

        public List<vec2> Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
                vertices = new float[points.Count * 3];
                for (int i = 0; i < points.Count; ++i)
                {
                    vertices[i * 3] = points[i].x;
                    vertices[i * 3 + 1] = points[i].y;
                }

            }
        }

        protected PrimitiveType primitiveType = PrimitiveType.Triangles;

        public Shape2D(List<vec2> points, vec3 color) : base(color)
        {
            Points = points;
        }

        public virtual void Draw()
        {
            shader.Use();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float),
                vertices, BufferUsageHint.StaticDraw);
            GL.DrawArrays(primitiveType, 0, points.Count);
        }
    }
}
