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

        public static mat4 LookAt(vec3 eye, vec3 target)
        {
            return LookAt(eye, target, new vec3(0, 1, 0));
        }

        public static mat4 LookAt(vec3 eye, vec3 target, vec3 up)
        {
            mat4 lookAt = mat4.identity();
            vec3 eyeMinus = eye * -1;
            vec3 toUs = glm.normalize(eye - target);
            vec3 right = glm.cross(glm.normalize(up), toUs);
            vec3 upNew = glm.cross(toUs, right);

            lookAt[0, 0] = right.x;
            lookAt[1, 0] = right.y;
            lookAt[2, 0] = right.z;
            lookAt[0, 1] = upNew.x;
            lookAt[1, 1] = upNew.y;
            lookAt[2, 1] = upNew.z;
            lookAt[0, 2] = toUs.x;
            lookAt[1, 2] = toUs.y;
            lookAt[2, 2] = toUs.z;
            lookAt[3, 0] = glm.dot(right, eyeMinus);
            lookAt[3, 1] = glm.dot(upNew, eyeMinus);
            lookAt[3, 2] = glm.dot(toUs, eyeMinus);
            lookAt[3, 3] = 1;

            return lookAt;
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
