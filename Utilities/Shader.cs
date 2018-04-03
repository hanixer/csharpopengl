using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace Utilities
{
    public class Shader
    {
        public int ID { set; get; }
        public Shader(string vertexPath, string fragmentPath)
        {
            string vertexCode = File.ReadAllText(vertexPath);
            string fragmentCode = File.ReadAllText(fragmentPath);

            string infoLog;

            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexCode);
            GL.CompileShader(vertex);
            infoLog = GL.GetShaderInfoLog(vertex);
            if (!string.IsNullOrEmpty(infoLog))
            {
                Console.WriteLine("Error! Vertex shader compilation failed!");
                Console.WriteLine(infoLog);
            }

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentCode);
            GL.CompileShader(fragment);
            infoLog = GL.GetShaderInfoLog(fragment);
            if (!string.IsNullOrEmpty(infoLog))
            {
                Console.WriteLine("Error! Fragment shader compilation failed!");
                Console.WriteLine(infoLog);
            }


            ID = GL.CreateProgram();
            GL.AttachShader(ID, vertex);
            GL.AttachShader(ID, fragment);
            GL.LinkProgram(ID);

            infoLog = GL.GetProgramInfoLog(ID);
            if (!string.IsNullOrEmpty(infoLog))
            {
                Console.WriteLine("Error! Program compilation failed!");
                Console.WriteLine(infoLog);
            }

            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
        }

        public void Use()
        {
            GL.UseProgram(ID);
        }

        public void setBool(string name, bool value)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), value ? 1 : 0);
        }
        public void Set(string name, int value)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), value);
        }
        public void Set(string name, float value)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), value);
        }

        public void Set(string v, GlmNet.mat4 m4)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ID, v), 1, false, m4.to_array());
        }

        public void Set(string v, vec3 vec3)
        {
            GL.Uniform3(GL.GetUniformLocation(ID, v), vec3.x, vec3.y, vec3.z);
        }
    }
}
