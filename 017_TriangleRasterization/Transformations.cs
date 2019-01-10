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
            mat4 rotation = mat4.identity();
            vec3 offset = target - eye;
            vec3 forward = glm.normalize(offset);
            vec3 right = glm.cross(glm.normalize(up), forward);
            vec3 upNew = glm.cross(forward, right);

            rotation[0, 0] = right.x;
            rotation[1, 0] = right.y;
            rotation[2, 0] = right.z;
            rotation[0, 1] = upNew.x;
            rotation[1, 1] = upNew.y;
            rotation[2, 1] = upNew.z;
            rotation[0, 2] = forward.x;
            rotation[1, 2] = forward.y;
            rotation[2, 2] = forward.z;

            return rotation * glm.translate(mat4.identity(), -1 * eye);
        }

        public static mat4 LookAt2(vec3 eye, vec3 target, vec3 up)
        {
            mat4 lookAt = mat4.identity();
            vec3 offset = eye - target;
            vec3 forward = glm.normalize(-1 * offset);
            vec3 right = glm.cross(forward, glm.normalize(up));
            vec3 upNew = glm.cross(right, forward);

            var what = glm.translate(mat4.identity(), -1 * eye);

            return lookAt;
        }

        public static vec3 Vec4ToVec3(vec4 v4) => new vec3(v4.x, v4.y, v4.z);
    }
}
