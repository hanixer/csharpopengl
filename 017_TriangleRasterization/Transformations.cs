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

        public static mat4 LookAt(vec3 eye, vec3 target, vec3 up)
        {
            mat4 lookAt = mat4.identity();
            vec3 offset = target - eye;
            vec3 forward = glm.normalize(offset);
            vec3 right = glm.cross(forward, glm.normalize(up));
            vec3 upNew = glm.cross(right, forward);

            lookAt[0] = new vec4(right, 0);
            lookAt[1] = new vec4(upNew, 0);
            lookAt[2] = new vec4(forward, 0);
            lookAt[3] = new vec4(offset, 1);

            return lookAt;
        }

        public static vec3 Vec4ToVec3(vec4 v4) => new vec3(v4.x, v4.y, v4.z);
    }
}
