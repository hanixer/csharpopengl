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

        public Scene()
        {
            viewport.HorResolution = 200;
            viewport.VertResolution = 200;
            viewport.PixelSize = 1;
            viewport.Gamma = 1;
            viewport.SamplesCount = 16;

            Bitmap = new Bitmap(viewport.HorResolution, viewport.VertResolution);

            Sphere sphere = new Sphere(new vec3(0, -50, 0), 20);
            sphere.Color = Colors.Red;
            //AddObject(sphere);

            sphere = new Sphere(new vec3(-0, 50, 0), 20);
            sphere.Color = Colors.Yellow;
            //AddObject(sphere);

            Plane plane = new Plane(new vec3(0), new vec3(0, 1, 1));
            //plane.Color = Colors.Green;

            AddObject(new Sphere(new vec3(0, 0, 0), 10, new vec3(1, 0, 1)));
            AddObject(new Sphere(new vec3(20, 0, 0), 10, new vec3(1, 0, 1)));
            AddObject(new Sphere(new vec3(40, 0, 0), 10, new vec3(1, 0, 1)));
            AddObject(new Sphere(new vec3(60, 0, 0), 10, new vec3(1, 0, 1)));
            AddObject(new Sphere(new vec3(80, 0, 0), 10, new vec3(1, 0, 1)));
            AddObject(new Sphere(new vec3(100, 0, 0), 10, new vec3(1, 0, 1)));
            AddObject(new Sphere(new vec3(-20, 0, 0), 10, new vec3(0, 1, 1)));
            AddObject(new Sphere(new vec3(-40, 0, 0), 10, new vec3(0, 1, 1)));
            AddObject(new Sphere(new vec3(-60, 0, 0), 10, new vec3(0, 1, 1)));

            AddObject(new Sphere(new vec3(0, 20, 20), 10, new vec3(0.5f, 0.5f, 1)));
            AddObject(new Sphere(new vec3(0, -20, -20), 10, new vec3(0.5f, 0.5f, 1)));

            Random rand = new Random();
            for (int i = 0; i < 100; i++)
            {
                float x = rand.Next(80, 200);
                float y = rand.Next(80, 200);
                float z = rand.Next(-10, 0);
                float r = rand.Next(5, 15);
                float re = (float)rand.NextDouble();
                float g = (float)rand.NextDouble();
                float b = (float)rand.NextDouble();
                //AddObject(new Sphere(new vec3(x, y, z), r, new vec3(re, g, b)));
            }

            //AddObject(plane);
        }

        public void RenderScene()
        {
            Ray ray = new Ray();
            float z = 50;

            for (int r = 0; r < viewport.VertResolution; r++)
            {
                for (int c = 0; c < viewport.HorResolution; c++)
                {
                    float x = viewport.PixelSize * (c - (viewport.HorResolution - 1.0f) / 2);
                    float y = viewport.PixelSize * (r - (viewport.VertResolution - 1.0f) / 2);

                    ray.Origin = new vec3(0, 0, z);
                    ray.Direction = glm.normalize(new vec3(x, y, planeZ));

                    vec3 color = Trace(ray);

                    DisplayPixel(c, r, color);
                }
            }

            Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
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
            //return obj.Color;
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
