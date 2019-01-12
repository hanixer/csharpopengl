using GlmNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    abstract class GeometricObject
    {
        protected vec3 color;
        public abstract bool Hit(Ray ray, ref float tmin);
    }
}
