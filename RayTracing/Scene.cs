using GlmNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class Scene
    {
        ViewPort viewport = new ViewPort();
        Color background = Color.Black;
        public Bitmap Bitmap;
        List<GeometricObject> objects = new List<GeometricObject>();
        float planeZ = 45;
        static vec3 lightDirection = glm.normalize(new vec3(0, 0, -1));
        Triangle triangle;
        Light light;
        Camera camera = new Camera();

        public Scene()
        {
            viewport.Width = 800;
            viewport.Height = 600;
            viewport.PixelSize = 1;
            viewport.Gamma = 1;
            viewport.SamplesCount = 16;

            Bitmap = new Bitmap(viewport.Width, viewport.Height);

            vec3 v1 = new vec3(0, 1, -2);
            vec3 v2 = new vec3(-1.9f, -1, -2);
            vec3 v3 = new vec3(1.6f, -0.5f, -2);

            vec3 n1 = glm.normalize(new vec3(0, 0.6f, 1));
            vec3 n2 = glm.normalize(new vec3(-0.4f, -0.4f, 1));
            vec3 n3 = glm.normalize(new vec3(0.4f, -0.4f, 1));

            triangle = new Triangle(new vec3[] { v1, v2, v3 }, new vec3[] { n1, n2, n3 });

            light = new Light(new vec3(1, 3, 1), new vec3(10, 10, 10));                
        }

        public void RenderScene()
        {
            float distance = float.PositiveInfinity;

            for (int r = 0; r < viewport.Height; r++)
            {
                for (int c = 0; c < viewport.Width; c++)
                {
                    Ray ray = ComputeEyeRay(c + 0.5f, r + 0.5f, viewport.Width, viewport.Height, camera);

                    vec3 color;

                    if (Sample(ref distance, ray))
                    {
                        color = Colors.White;
                    }
                    else
                    {
                        color = Colors.Black;
                    }

                    DisplayPixel(r, c, color);
                }
            }
            
            Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        private bool Sample(ref float distance, Ray ray)
        {
            float d = ComputeIntersection(ray, triangle);
            if (d >= distance)
            {
                return false;
            }

            distance = d;
            return true;
        }

        static Ray ComputeEyeRay(float x, float y, int width, int height, Camera camera)
        {
            float aspect = (float)height / width;
            float side = (float)(-2.0f * Math.Tan(camera.fieldOfViewX * 0.5));
            float originX = (x / width - 0.5f) * side;
            float originY = -(y / height - 0.5f) * side * aspect;
            var origin = new vec3(originX, originY, 1.0f) * camera.nearZ;

            return new Ray(origin, glm.normalize(origin));
        }

        static float ComputeIntersection(Ray ray, Triangle triangle)
        {
            vec3 e1 = triangle.Point(1) - triangle.Point(0);
            vec3 e2 = triangle.Point(2) - triangle.Point(0);
            vec3 triNormal = glm.cross(e1, e2);

            float d = -glm.dot(triNormal, ray.Origin - triangle.Point(0)) / glm.dot(triNormal, ray.Direction);

            return d;
        }

        public void AddPlaneZ(float add)
        {
            planeZ += add;
        }

        private vec3 Trace(Ray ray)
        {
            float tmin = float.PositiveInfinity;
            float t = 0;
            vec3 color = Colors.Black;

            foreach (var obj in objects)
            {
                if (obj.Hit(ray, ref t))
                {
                    if (t < tmin)
                    {
                        tmin = t;
                        color = ComputeColorWithNormal(ray, t, obj);
                    }
                }
            }

            return color;
        }


        private static vec3 ComputeColorWithNormal(Ray ray, float t, GeometricObject obj)
        {
            vec3 point = ray.GetPoint(t);
            vec3 normal = obj.GetNormal(point);
            float dot = glm.dot(normal, lightDirection);

            if (dot > 0)
            {
                return obj.Color * dot;
            }
            else
            {
                return Colors.Black;
            }            
        }

        void DisplayPixel(int row, int column, vec3 color)
        {
            Bitmap.SetPixel(column, row, Color.FromArgb((int)(color.x * 255), (int)(color.y * 255), (int)(color.z * 255)));
        }

        void AddObject(GeometricObject obj) => objects.Add(obj);

        private vec3 TraceFunctionF(float x, float y)
        {
            x /= 100;
            y /= 100;
            float value = (float)(1 + Math.Sin(x * x * y * y)) / 2;
            //Console.WriteLine("{0}, {1}, {2}", x, y, value);
            return new vec3(value, value, value);
        }
    }
}
