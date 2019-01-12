using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class Plane : GeometricObject
    {
        const float EPSILON = 0.00001f;
        vec3 point;
        vec3 normal;

        public Plane(vec3 point, vec3 normal)
        {
            this.point = new vec3(point);
            this.normal = new vec3(normal);
        }

        public override bool Hit(Ray ray, ref float tmin)
        {
            float t = -glm.dot(normal, ray.Origin - point) / glm.dot(normal, ray.Direction);

            if (t > EPSILON)
            {
                tmin = t;
                return true;
            }

            return false;
        }
    }
}
