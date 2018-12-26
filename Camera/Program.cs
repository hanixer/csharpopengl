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

namespace Camera
{
    class Program : GameWindow
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
        vec3[] cubePositions = {
  new vec3( 0.0f,  0.0f,  0.0f),
  new vec3( 2.0f,  5.0f, -15.0f),
  new vec3(-1.5f, -2.2f, -2.5f),
  new vec3(-3.8f, -2.0f, -12.3f),
  new vec3( 2.4f, -0.4f, -3.5f),
  new vec3(-1.7f,  3.0f, -7.5f),
  new vec3( 1.3f, -2.0f, -2.5f),
  new vec3( 1.5f,  2.0f, -2.5f),
  new vec3( 1.5f,  0.2f, -1.5f),
  new vec3(-1.3f,  1.0f, -1.5f)
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
        float fov = 45.0f;
        vec3 cameraPos = new vec3(0.0f, 0.0f, 3.0f);
        vec3 cameraFront = new vec3(0.0f, 0.0f, -1.0f);
        vec3 cameraUp = new vec3(0.0f, 1.0f, 0.0f);
        float cameraSpeed = 0.05f;
        float yaw = 0.0f;
        float pitch = 0.0f;
        bool isFirstMove = true;
        Shader shader;

        Program()
        {
            RenderFrame += OnRenderFrame;
            Load += OnLoad;
            Closing += OnClosing;
            KeyPress += OnKeyPress;
            KeyUp += OnKeyUp;
            Height = Width;
            UpdateFrame += OnUpdateFrame;
            MouseMove += OnMouseMove;
            //CursorVisible = false;
            FocusedChanged += OnFocusedChanged;
            
        }

        private void OnFocusedChanged(object sender, EventArgs e)
        {

        }

        private void OnMouseMove(object sender, OpenTK.Input.MouseMoveEventArgs e)
        {
            if (isFirstMove)
            {
                isFirstMove = false;
                return;
            }
            //throw new NotImplementedException();
            //yaw += e.XDelta;
            //pitch += e.YDelta;

            Console.WriteLine("{0}, {1}", cameraFront.x, e.XDelta);

            cameraFront.x += e.XDelta * 0.01f / 2 / 2;

            
        }

        private void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            var state = OpenTK.Input.Keyboard.GetState();

            if (state.IsKeyDown(OpenTK.Input.Key.W))
            {
                cameraPos += cameraSpeed * cameraFront;
            }
            if (state.IsKeyDown(OpenTK.Input.Key.S))
            {
                cameraPos -= cameraSpeed * cameraFront;
            }
            if (state.IsKeyDown(OpenTK.Input.Key.A))
            {
                cameraPos -= glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;
            }
            if (state.IsKeyDown(OpenTK.Input.Key.D))
            {
                cameraPos += glm.normalize(glm.cross(cameraFront, cameraUp)) * cameraSpeed;
            }

        }

        private void OnKeyUp(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
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

            Images.LoadTexture("../../Textures/container.jpg", texture1);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            texture2 = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texture2);
            Images.LoadTexture("../../Textures/cloud.jpg", texture2);
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
            shader.Set("uniformColor", new vec3(1.0f)); // Reset box color

            float dt = (float)(DateTime.Now - startTime).TotalMilliseconds;

            mat4 view = glm.lookAt(cameraPos, cameraPos + cameraFront, cameraUp);
            shader.Set("view", view);

            if (Height != 0)
            {
                mat4 projection = glm.perspective(glm.radians(fov), Width / Height, 0.1f, 100.0f);
                shader.Set("projection", projection);
            }

            // draw
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture1);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, texture2);

            GL.BindVertexArray(VAO1);
            //GL.DrawElements(BeginMode.Triangles, vertices.Length / 5, DrawElementsType.UnsignedInt, 0);

            int i = 0;
            foreach (var pos in cubePositions)
            {
                i++;
                mat4 model = glm.translate(mat4.identity(), pos);

                if (i == 1 || i == 3)
                    model = glm.rotate(model, i * glm.radians(dt / 8.0f), new vec3(1.0f, 0.5f, 0.0f));

                shader.Set("model", model);
                GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 5);
            }

            shader.Set("model", mat4.identity());
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 5);

            for (int j = 0; j < 5; ++j)
            {
                shader.Set("model",
                    glm.scale(
                        glm.translate(mat4.identity(), new vec3(0.25f * j, 0.0f * 0.25f * j, +2.0f)),
                        new vec3(1.0f / 16.0f)));
                shader.Set("uniformColor", new vec3(1.0f, 0.0f, 0.0f));
                GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 5);
                shader.Set("model",
                    glm.scale(
                        glm.translate(mat4.identity(), new vec3(0.25f * -j, 0.0f * 0.25f * j, +2.0f)),
                        new vec3(1.0f / 16.0f)));
                shader.Set("uniformColor", new vec3(1.0f, 0.0f, 0.0f));
                GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 5);
            }
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
