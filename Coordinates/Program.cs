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
        float[] vertices = {
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
};

        int[] indices = {
        0, 3, 1, // first triangle
        1, 2, 3  // second triangle
    };

        int VBO1;
        int VAO1;
        int EBO;
        int texture1;
        int texture2;
        DateTime startTime = DateTime.Now;
        float cameraZ = -3.0f;

        Shader shader;

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
            if (e.Key == OpenTK.Input.Key.Down)
            {
                cameraZ -= diff;
            }
            else if (e.Key == OpenTK.Input.Key.Up)
            {
                cameraZ += diff;
            }
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

            // initialize buffers
            VBO1 = GL.GenBuffer();
            VAO1 = GL.GenVertexArray();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO1);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            // prepare texture
            texture1 = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture1);

            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            Images.LoadTexture("../../Textures/container.jpg", texture1, false);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            texture2 = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture2);
            Images.LoadTexture("../../Textures/cloud.jpg", texture2, false);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // set texture variables
            shader.Use();
            shader.Set("texture1", 0);
            shader.Set("texture2", 1);

            GL.Enable(EnableCap.DepthTest);
        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(0.0f, 0.2f, .2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            shader.Use();

            float dt = (float)(DateTime.Now - startTime).TotalMilliseconds;
            // set coordinates
            mat4 model = glm.rotate(glm.radians(dt / 8.0f), new vec3(1.0f, 1.0f, 1.0f));
            shader.Set("model", model);
            Console.WriteLine("dt / = {0}, radians = {1}, dt = {2}", dt / 8.0f, glm.radians(dt / 8.0f), dt);

            mat4 view = glm.translate(mat4.identity(), new vec3(0.0f, 0.0f, cameraZ));
            shader.Set("view", view);

            mat4 projection = glm.perspective(glm.radians(45.0f), Width / Height, 0.1f, 100.0f);
            shader.Set("projection", projection);

            // draw
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture1);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, texture2);

            GL.BindVertexArray(VAO1);
            //GL.DrawElements(BeginMode.Triangles, vertices.Length / 5, DrawElementsType.UnsignedInt, 0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 5);

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
