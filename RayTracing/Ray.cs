using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    public class Ray
    {
        public vec3 Origin;
        public vec3 Direction;

        public Ray(vec3 origin, vec3 direction)
        {
            Origin = new vec3(origin);
            Direction = glm.normalize(direction);
        }

        public Ray()
        {
            Origin = new vec3(0);
            Direction = new vec3(0);
        }
    }
}
