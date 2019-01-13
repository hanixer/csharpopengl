using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    public class Helper
    {
        public static float Length(vec3 v)
        {
            return (float)Math.Sqrt(glm.dot(v, v));
        }
    }
}
