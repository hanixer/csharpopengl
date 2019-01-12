using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class Sphere : GeometricObject
    {
        vec3 center;
        float radius;

        public Sphere(vec3 center, float radius)
        {
            this.center = new vec3(center);
            this.radius = radius;
        }

        public override bool Hit(Ray ray, ref float tmin)
        {
            vec3 offset = ray.Origin - center;
            float a = glm.dot(ray.Direction, ray.Direction);
            float b = 2 * glm.dot(ray.Direction, offset);
            float c = glm.dot(offset, offset) - radius * radius;
            float discriminant = b * b - 4 * a * c;
            
            if (discriminant > 0)
            {
                float t1 = (-b + discriminant) / (2 * a);
                float t2 = (-b - discriminant) / (2 * a);
                tmin = Math.Min(t1, t2);
                return true;
            }
            else if (discriminant == 0)
            {
                float t1 = -b / (2 * a);
                tmin = t1;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
