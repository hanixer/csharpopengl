using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using GlmNet;
using Utilities;

namespace WindowsFormsApplication1
{
    class Renderer
    {
        int vao;
        int vbo;
        int ibo;
        float[] positions;
        int[] indices;

        public void Init()
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ibo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        
        PolygonMode polygonMode;
        public PolygonMode PolygonMode
        {
            get
            {
                return polygonMode;
            }
            set
            {
                polygonMode = value;
                GL.PolygonMode(MaterialFace.FrontAndBack, polygonMode);
            }
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

        public void DrawSphere(float radius, vec3 color, int count)
        {
            int sectorsCount = count * 2;
            positions = GeneratePositions(radius, count, sectorsCount);
            indices = GenerateIndices(positions, count, sectorsCount);

            Shaders.shader.Set("color", color);
            Shaders.shader.Set("model", mat4.identity());
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * positions.Length, positions, BufferUsageHint.StaticCopy);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticCopy);
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void DrawCylinder(float radius, float height, int sectorsCount)
        {
            vec3 color = new vec3(0.5f, 0.2f, 0.2f);

            positions = GenerateCylinderPositions(radius, height, sectorsCount);

            List<int> list = new List<int>();
            int a = 0;
            int b = sectorsCount + 1;
            for (int j = 0; j < sectorsCount; ++j)
            {
                int c = a + j + 1;
                int d = c < sectorsCount ? c + 1 : a + 1;
                int e = b + j + 1;
                int f = c < sectorsCount ? e + 1 : b + 1;

                list.Add(a);
                list.Add(c);
                list.Add(d);

                list.Add(c);
                list.Add(d);
                list.Add(f);

                list.Add(f);
                list.Add(e);
                list.Add(c);

                list.Add(b);
                list.Add(e);
                list.Add(f);
            }

            indices = list.ToArray();
            //Console.WriteLine("[{0}]", string.Join(", ", indices));

            Shaders.shader.Set("color", color);
            Shaders.shader.Set("model", mat4.identity());
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * positions.Length, positions, BufferUsageHint.StaticCopy);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticCopy);
            
            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
            //Shaders.shader.Set("color", new vec3(0));
            //GL.DrawElements(BeginMode.LineStrip, indices.Length, DrawElementsType.UnsignedInt, 0);

            Draw(color, PrimitiveType.Points);
        }

        public void DrawHeightfield(float[,] heightfield, float scaleXY)
        {
            float offsetX = heightfield.GetLength(1) * scaleXY / 2;
            float offsetZ = heightfield.GetLength(0) * scaleXY / 2;
            List<float> list = new List<float>();
            for (int z = 0; z < heightfield.GetLength(0); z++)
            {
                for (int x = 0; x < heightfield.GetLength(1); x++)
                {
                    list.Add(x * scaleXY - offsetX);
                    list.Add(heightfield[z, x] / 1.0f);
                    list.Add(z * scaleXY - offsetZ);
                }
            }
            positions = list.ToArray();

            List<int> indicesList = new List<int>();
            int zCount = heightfield.GetLength(0);
            int xCount = heightfield.GetLength(1);
            for (int i = 0; i < zCount - 1; i++)
            {
                for (int j = 0; j < xCount - 1; j++)
                {
                    int a = i * xCount + j;
                    int b = (i + 1) * xCount + j;
                    int c = b + 1;
                    int d = a + 1;

                    indicesList.Add(a);
                    indicesList.Add(b);
                    indicesList.Add(c);
                    indicesList.Add(d);
                }
            }

            indices = indicesList.ToArray();

            Shaders.shader.Set("color", new vec3(1));
            Shaders.shader.Set("model", mat4.identity());
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * positions.Length, positions, BufferUsageHint.StaticCopy);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticCopy);

            GL.DrawElements(BeginMode.Quads, indices.Length, DrawElementsType.UnsignedInt, 0);
            Shaders.shader.Set("color", new vec3(0));
            GL.DrawElements(BeginMode.Points, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void DrawGlass(float[] points, int sectorsCount)
        {
            List<float> positionList = new List<float>();
            float sectorsStep = Constants.PI * 2 / sectorsCount;
            for (int i = 0; i < points.Length; i += 2)
            {
                float xOrigin = points[i];
                float yOrigin = points[i + 1];

                for (int j = 0; j < sectorsCount; j++)
                {
                    float angle = j * sectorsStep;
                    float x = (float)Math.Sin(angle) * xOrigin;
                    float z = (float)Math.Cos(angle) * xOrigin;
                    positionList.Add(x);
                    positionList.Add(yOrigin);
                    positionList.Add(z);
                }
            }
            positions = positionList.ToArray();

            List<int> indicesList = new List<int>();
            for (int i = 0; i < points.Length / 2; i++)
            {
                for (int j = 0; j < sectorsCount; j++)
                {
                    int a = i * sectorsCount + j;
                    int b = (i + 1) * sectorsCount + j;
                    int c = (i + 1) * sectorsCount + (j + 1) % sectorsCount;
                    int d = i * sectorsCount + (j + 1) % sectorsCount;

                    indicesList.Add(a);
                    indicesList.Add(b);
                    indicesList.Add(c);
                    indicesList.Add(d);
                }
            }
            indices = indicesList.ToArray();

            Shaders.shader.Set("color", new vec3(0));
            Shaders.shader.Set("model", mat4.identity());
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * positions.Length, positions, BufferUsageHint.StaticCopy);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * indices.Length, indices, BufferUsageHint.StaticCopy);
            GL.DrawElements(BeginMode.Quads, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private static float[] GenerateCylinderPositions(float radius, float height, int sectorsCount)
        {
            List<float> positionsList = new List<float>();
            float y = 0;
            for (int i = 0; i < 2; ++i)
            {
                positionsList.Add(0);
                positionsList.Add(y);
                positionsList.Add(0);

                float step = Constants.PI * 2 / sectorsCount;
                for (int j = 0; j < sectorsCount; ++j)
                {
                    float angle = j * step;
                    float x = (float)Math.Sin(angle) * radius;
                    float z = (float)Math.Cos(angle) * radius;

                    positionsList.Add(x);
                    positionsList.Add(y);
                    positionsList.Add(z);
                }

                y += height;
            }

            return positionsList.ToArray();
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

        public static float[] GeneratePositions(float radius, int stacksCount, int sectorsCount)
        {
            List<float> positions = new List<float>();
            float stacksStep = (float)Math.PI / stacksCount;
            float sectorsStep = (float)Math.PI * 2 / sectorsCount;
            for (int i = 0; i <= stacksCount; ++i)
            {
                float stacksAngle = (float)Math.PI / 2 - i * stacksStep;
                float y = radius * (float)Math.Sin(stacksAngle);
                float xz = radius * (float)Math.Cos(stacksAngle);

                for (int j = 0; j < sectorsCount; ++j)
                {
                    float sectorsAngle = j * sectorsStep;
                    float x = xz * (float)Math.Sin(sectorsAngle);
                    float z = xz * (float)Math.Cos(sectorsAngle);
                    if (Math.Abs(x) < 0.0001)
                    {
                        x = 0;
                    }
                    if (Math.Abs(z) < 0.0001)
                    {
                        z = 0;
                    }
                    positions.Add(x);
                    positions.Add(y);
                    positions.Add(z);
                }
            }
            return positions.ToArray();
        }

        static int[] GenerateIndices(float[] positions, int stacksCount, int sectorsCount)
        {
            List<int> result = new List<int>();
            for (int stack = 0; stack < stacksCount; stack++)
            {
                for (int sector = 0; sector < sectorsCount; sector++)
                {
                    int a = stack * sectorsCount + sector;
                    int b = (stack + 1) * sectorsCount + sector;
                    int c = stack * sectorsCount + ((sector + 1) % sectorsCount);
                    int d = (stack + 1) * sectorsCount + ((sector + 1) % sectorsCount);
                    result.Add(a);
                    result.Add(b);
                    result.Add(c);
                    result.Add(c);
                    result.Add(b);
                    result.Add(d);
                }
            }

            return result.ToArray();
        }
    }
}
