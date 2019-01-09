using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace _014_DrawTriangle
{
    public class Transformations
    {
        public static mat4 MakeViewportTransformation(int width, int height)
        {
            int w = width - 1;
            int h = height - 1;
            mat4 m = mat4.identity();
            m[0] = new vec4(w / 2, 0, 0, 0);
            m[1] = new vec4(0, h / 2, 0, 0);
            m[3] = new vec4(w / 2 + 0.5f, h / 2 + 0.5f, 0, 1);

            return m;
        }

        public static vec3 Vec4ToVec3(vec4 v4) => new vec3(v4.x, v4.y, v4.z);
    }
}
