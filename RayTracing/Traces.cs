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
        protected Scene scene;

        public Tracer(Scene scene)
        {
            this.scene = scene;
        }

        public virtual vec3 TraceRay(Ray ray)
        {
            return Colors.Black;
        }
    }
}
