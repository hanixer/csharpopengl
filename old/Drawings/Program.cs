using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using Utilities;
using System.Drawing;
using GlmNet;

namespace Project2_Shaders
{
    class Program : OpenTK.GameWindow
    {
        DateTime startTime = DateTime.Now;
        float cameraZ = -3.0f;
        float cameraX = 0.0f;
        float cameraY = 0.0f;
        float fov = 45.0f;

        int VBO;
        int VAO;

        Shader shader;

        float[] vertices =
        {
            -1.0f, 0.0f, 0.0f,  1.0f, 0.0f, 0.0f,
            0.0f, -1.0f, 0.0f,  0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, -1.0f,  0.0f, 0.0f, 1.0f
        };

        Program()
        {
            RenderFrame += OnRenderFrame;
            Load += OnLoad;
            Closing += OnClosing;
            KeyPress += OnKeyPress;
            KeyUp += OnKeyUp;
            Height = Width;
        }

        private void OnKeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            float diff = 1.0f;
            if (e.Key == OpenTK.Input.Key.S)
            {
                cameraZ -= diff;
            }
            else if (e.Key == OpenTK.Input.Key.W)
            {
                cameraZ += diff;
            }
            else if (e.Key == OpenTK.Input.Key.A)
            {
                cameraX += diff;
            }
            else if (e.Key == OpenTK.Input.Key.D)
            {
                cameraX -= diff;
            }
            else if (e.Key == OpenTK.Input.Key.Q)
            {
                cameraY += diff;
            }
            else if (e.Key == OpenTK.Input.Key.E)
            {
                cameraY -= diff;
            }
            else if (e.Key == OpenTK.Input.Key.KeypadPlus)
            {
                fov += 10.0f;
            }
            else if (e.Key == OpenTK.Input.Key.KeypadMinus)
            {
                fov -= 10.0f;
            }
            Console.WriteLine("camera = {0}, fov = {1}", cameraZ, fov);
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Close();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            // compile vertex shader
            shader = new Shader("../../Shaders/shader.vert", "../../Shaders/shader.frag");

            VBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertices.Length, vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.Enable(EnableCap.DepthTest);
        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(0.0f, 0.2f, .2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();
            shader.Set("uniformColor", new vec3(0.5f, 0.5f, 0.0f));
            float dt = (float)(DateTime.Now - startTime).TotalMilliseconds;

            GL.BindVertexArray(VAO);
            GL.LineWidth(3.0f);
            GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Length / 2);
            
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
