using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using Utilities;

namespace Project2_Shaders
{
    class Program : OpenTK.GameWindow
    {
        float[] vertices1 =
        {
            // first triangle   // colors
            -1.0f, 0.0f, 0.0f,  1.0f, 0.0f, 0.0f,
            -0.5f, 1.0f, 0.0f,  0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f,   0.0f, 0.0f, 1.0f,
        };
        float[] vertices2 =
        {
            // second triangle
            0.5f, 1.0f, 0.0f,
            1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.0f,
        };
        int[] indices =
        {
            0, 1, 2,
            3, 4, 5
        };
        int VBO1;
        int VBO2;
        int VAO1;
        int VAO2;

        Shader shader;
        Shader shader2;

        Program()
        {
            RenderFrame += OnRenderFrame;
            Load += OnLoad;
            Closing += OnClosing;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Close();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            // compile vertex shader
            shader = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");
            shader2 = new Shader("../../Shaders/shader.vert", "../../Shaders/shader2.frag");

            // initialize buffers
            VBO1 = GL.GenBuffer();
            VBO2 = GL.GenBuffer();
            VAO1 = GL.GenVertexArray();
            VAO2 = GL.GenVertexArray();

            GL.BindVertexArray(VAO1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO1);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices1.Length * sizeof(float), vertices1, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(VAO2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO2);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices2.Length * sizeof(float), vertices2, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(0.0f, 0.2f, .2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // drawing code
            shader.Use();

            GL.BindVertexArray(VAO1);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            shader2.Use();
            // setting uniform
            double time = (double)DateTime.Now.ToBinary();
            double value = Math.Sin(time) / 2.0 + 0.5;

            GL.BindVertexArray(VAO2);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            SwapBuffers();
        }

        static void Main(string[] args)
        {
            using (Program program = new Program())
            {
                program.Run();
            }

        }

        private static void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("Closing...");
        }
    }
}
