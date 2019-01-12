using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class Tracer
    {
        protected World world;

        public Tracer(World world)
        {
            this.world = world;
        }

        public virtual vec3 TraceRay(Ray ray)
        {
            return Colors.Black;
        }
    }
}
