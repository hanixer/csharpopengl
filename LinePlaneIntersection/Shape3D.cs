using System;
using System.Collections.Generic;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace WindowsFormsApplication1
{
    class Rectangle3D : ShapeBase
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
                    vertices[i * 3 + 2] = points[i].z;
                }
            }
        }

        public Rectangle3D(List<vec3> points) : base(new vec3(1.0f))
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
            GL.DrawArrays(PrimitiveType.Quads, 0, points.Count);
            shader.Set("color", new vec3(1.0f, 0.0f, 1.0f));

            GL.DrawArrays(PrimitiveType.LineLoop, 0, points.Count);
            shader.Set("color", color);
        }

        public void Rotate(float angle)
        {
            shader.Set("model", glm.rotate(angle, new vec3(0, 0, 1)));
        }

        public void Rotate(vec3 direction)
        {
            vec3 directionOld = new vec3(0, 0, 1);
            float angle = (float)Math.Acos(glm.dot(glm.normalize(directionOld), glm.normalize(direction)));
            vec3 axis = glm.cross(directionOld, direction);
            shader.Use();
            shader.Set("model", glm.rotate(angle, axis));
        }
    }
    class Line3D : ShapeBase
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
                    vertices[i * 3 + 2] = points[i].z;
                }
            }
        }

        public Line3D(vec3 point1, vec3 point2) : base(new vec3(0.5f))
        {
            Points = new List<vec3> { point1, point2 };
        }

        public virtual void Draw()
        {
            shader.Use();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float),
                vertices, BufferUsageHint.StaticDraw);
            GL.DrawArrays(PrimitiveType.Lines, 0, points.Count);
        }

        public void Rotate(vec3 direction)
        {
            vec3 directionOld = new vec3(0, 0, 1);
            float angle = (float)Math.Acos(glm.dot(glm.normalize(directionOld), glm.normalize(direction)));
            vec3 axis = glm.cross(directionOld, direction);
            shader.Use();
            shader.Set("model", glm.rotate(angle, axis));
        }
    }
}
