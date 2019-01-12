using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class Colors
    {
        public static vec3 Black
        {
            get { return new vec3(0); }
        }
        public static vec3 White
        {
            get { return new vec3(1); }
        }
        public static vec3 Red
        {
            get { return new vec3(1, 0, 0); }
        }
    }
}
