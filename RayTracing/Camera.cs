using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class Camera
    {
        public float nearZ = -0.1f;
        public float farZ = -100;
        public float fieldOfViewX = (float)Math.PI / 2;
    }
}
