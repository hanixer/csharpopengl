using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace ConsoleApplication1
{
    class Program : OpenTK.GameWindow
    {
        string vertexShaderSource =
            @"
#version 330 core

layout (location = 0) in vec3 aPos;

out vec4 vertexColor;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
    vertexColor = vec4(aPos.xzy, 1.0);
}
";
        string fragmentShaderSource =
            @"
#version 330 core

in vec4 vertexColor;
out vec4 FragColor;
uniform vec4 ourColor;

void main()
{
    FragColor = ourColor + vertexColor;
}
";
        string fragmentShaderSource2 =
            @"
#version 330 core

out vec4 FragColor;

void main()
{
    FragColor = vec4(0.1, 0.5, 1.0, 1.0);
}
";
        float[] verticesOld =
        {
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f,
                0.0f, 0.5f, 0.0f,
            };
        float[] verticesOld2 =
        {
            -1.0f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            0.5f, -0.5f, 0.0f,
        };
        float[] vertices =
        {
            // first triangle
            -1.0f, 0.0f, 0.0f,
            -0.5f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f,
            // second triangle
            0.5f, 1.0f, 0.0f,
            1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.0f,
        };
        float[] vertices1 =
        {
            // first triangle
            -1.0f, 0.0f, 0.0f,
            -0.5f, 1.0f, 0.0f,
            0.0f, 0.0f, 0.0f,
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
        int shaderProgram;
        int shaderProgram2;
        int VBO1;
        int VBO2;
        int VAO1;
        int VAO2;

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
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            string buildInfo = GL.GetShaderInfoLog(vertexShader);
            Console.WriteLine(buildInfo);
            // compile fragment shader
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            buildInfo = GL.GetShaderInfoLog(fragmentShader);
            int fragmentShader2 = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader2, fragmentShaderSource2);
            GL.CompileShader(fragmentShader2);
            buildInfo = GL.GetShaderInfoLog(fragmentShader2);
            Console.WriteLine(buildInfo);
            // compile program
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            buildInfo = GL.GetProgramInfoLog(shaderProgram);
            shaderProgram2 = GL.CreateProgram();
            GL.AttachShader(shaderProgram2, vertexShader);
            GL.AttachShader(shaderProgram2, fragmentShader2);
            GL.LinkProgram(shaderProgram2);
            buildInfo = GL.GetProgramInfoLog(shaderProgram2);


            // initialize buffers
            VBO1 = GL.GenBuffer();
            VBO2 = GL.GenBuffer();
            VAO1 = GL.GenVertexArray();
            VAO2 = GL.GenVertexArray();

            GL.BindVertexArray(VAO1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO1);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices1.Length * sizeof(float), vertices1, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

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
            GL.UseProgram(shaderProgram);

            GL.BindVertexArray(VAO1);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);


            GL.UseProgram(shaderProgram2);
            // setting uniform
            double time = (double)DateTime.Now.ToBinary();
            double value = Math.Sin(time) / 2.0 + 0.5;
            int uniformLocation = GL.GetUniformLocation(shaderProgram2, "ourColor");
            GL.Uniform4(uniformLocation, 0.0f, value, 0.0f, 1.0f);


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
