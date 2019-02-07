using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace WindowsFormsApplication1
{
    class Shaders
    {
        public static Shader shader;
        public static void Init()
        {
            shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            shader.Use();
            shader.Set("model", mat4.identity());
        }
    }
}
