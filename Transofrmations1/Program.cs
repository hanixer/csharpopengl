﻿using System;
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
        float[] vertices1 =
        {
            // first triangle   // colors
            .7f, 0.0f, 0.0f,  1.0f, 0.0f, 0.0f,
            -0.5f, 1.0f, 0.0f,  0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f,   0.0f, 0.0f, 1.0f,
        };
        float[] vertices = 
        {
            // positions            // colors           // texture coords
            0.5f,  0.5f, 0.0f,      1.0f, 0.0f, 0.0f,   1.0f, 1.0f,   // top right
            0.5f, -0.5f, 0.0f,      0.0f, 1.0f, 0.0f,   1.0f, 0.0f,   // bottom right
            -0.5f, -0.5f, 0.0f,     0.0f, 0.0f, 1.0f,   0.0f, 0.0f,   // bottom left
            -0.5f,  0.5f, 0.0f,     1.0f, 1.0f, 0.0f,   0.0f, 1.0f    // top left 
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

        Shader shader;

        Program()
        {
            RenderFrame += OnRenderFrame;
            Load += OnLoad;
            Closing += OnClosing;
            Height = Width;
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

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

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
        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(0.0f, 0.2f, .2f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // drawing code

            shader.Use();
            var deltaTime = (float)(startTime - DateTime.Now).TotalSeconds;

            mat4 m4 = glm.rotate(glm.radians(deltaTime * 16.0f), new vec3(0.0f, 0.0f, 1.0f));
            //m4 = glm.translate(m4, new vec3(0.5f, 0.0f, 0.0f));
            GL.UniformMatrix4(GL.GetUniformLocation(shader.ID, "transform"), 1, false, m4.to_array());
            shader.Set("transform", m4);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture1);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, texture2);

            GL.BindVertexArray(VAO1);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            m4 = glm.scale(mat4.identity(), new vec3(Math.Abs((float)Math.Sin(deltaTime))));
            m4 = glm.translate(m4, new vec3(0.5f));
            shader.Set("transform", m4);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

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
