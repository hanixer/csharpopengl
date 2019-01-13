using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class Light
    {
        public vec3 Position;
        public vec3 Power;

        public Light(vec3 position, vec3 power)
        {
            Position = position;
            Power = power;
        }
    }
}
