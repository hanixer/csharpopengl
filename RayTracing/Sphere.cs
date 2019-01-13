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

        public Sphere(vec3 center, float radius, vec3 color) : this(center, radius)
        {
            Color = color;
        }

        public Sphere(vec3 center, float radius)
        {
            this.center = new vec3(center);
            this.radius = radius;
        }

        public override bool Hit(Ray ray, ref float tmin)
        {
            vec3 v = ray.Origin - center;
            float a = glm.dot(ray.Direction, ray.Direction);
            float b = 2 * glm.dot(ray.Direction, v);
            float c = glm.dot(v, v) - radius * radius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant > 0)
            {
                float root = (float)Math.Sqrt(discriminant);
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

        public bool Hit2(Ray ray, ref float tmin)
        {
            vec3 offset = center - ray.Origin;
            vec3 toCenter = glm.normalize(offset);
            float sin = glm.dot(toCenter, ray.Direction);
            float hipotenuse = Helper.Length(offset);
            float height = sin * hipotenuse;
            float triangleBase = (float)Math.Sqrt(hipotenuse * hipotenuse - height * height);
            float diff = radius * radius - height * height;

            if (diff > 0)
            {
                float root = (float)Math.Sqrt(diff);
                float t0 = triangleBase - root;
                float t1 = triangleBase + root;

                if (t0 <= 0)
                {
                    tmin = t1;
                }
                else if (t1 <= 0)
                {
                    tmin = t0;
                }
                else
                {
                    tmin = Math.Min(t0, t1);
                }

                return true;
            }
            else if (diff == 0)
            {
                tmin = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        public override vec3 GetNormal(vec3 point)
        {
            return glm.normalize(point - center);
        }
    }
}
