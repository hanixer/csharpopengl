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
        protected float[] vertices;
        public ShapeBase(vec3 color)
        {
            Color = color;
            Model = mat4.identity();

            vbo = GL.GenBuffer();
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public vec3 Color
        {
            set
            {
                color = value;
            }
        }

        public mat4 Model
        {
            get;
            set;
        }

        vec3 position = new vec3(0, 0, 0);
        public vec3 Position
        {
            set
            {
                position = value;
                UpdateModel();
            }
        }

        mat4 rotation = mat4.identity();
        public mat4 Rotation
        {
            set
            {
                rotation = value;

            }
        }

        public void Rotate(vec3 direction)
        {
            vec3 directionOld = new vec3(0, 0, 1);
            float angle = (float)Math.Acos(glm.dot(glm.normalize(directionOld), glm.normalize(direction)));
            vec3 axis = glm.cross(directionOld, direction);
            rotation = glm.rotate(angle, axis);
            UpdateModel();
        }

        protected void UpdateModel()
        {
            Model = rotation;//* glm.translate(mat4.identity(), position);
        }
    }
}
