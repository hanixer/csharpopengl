using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace RayTracing
{
    class SphereTracer : Tracer
    {
        public SphereTracer(World world) : base(world)
        {

        }

        public override vec3 TraceRay(Ray ray)
        {
            float nothing = 0;
            if (world.sphere.Hit(ray, ref nothing))
            {
                return Colors.Red;
            }
            else
            {
                return Colors.Black;
            }
        }
    }
}
