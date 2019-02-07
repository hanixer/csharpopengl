using System.Collections.Generic;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace WindowsFormsApplication1
{
    class Shape3D : ShapeBase
    {
        public List<vec3> points;

        public List<vec3> Points
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

        public Shape3D(List<vec3> points, vec3 color) : base(color)
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
            GL.DrawArrays(PrimitiveType.Triangles, 0, points.Count);
        }
    }
}
