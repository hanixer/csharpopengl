using GlmNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class World
    {
        ViewPort viewport = new ViewPort();
        Color background = Color.Black;
        public Bitmap Bitmap;
        List<GeometricObject> objects = new List<GeometricObject>();

        public World()
        {
            viewport.HorResolution = 200;
            viewport.VertResolution = 200;
            viewport.PixelSize = 1;
            viewport.Gamma = 1;

            Bitmap = new Bitmap(viewport.HorResolution, viewport.VertResolution);

            Sphere sphere = new Sphere(new vec3(0, -25, 0), 80);
            sphere.Color = Colors.Red;            
            AddObject(sphere);

            sphere = new Sphere(new vec3(0, 30, 0), 60);
            sphere.Color = Colors.Yellow;
            AddObject(sphere);

            Plane plane = new Plane(new vec3(0), new vec3(0, 1, 1));
            plane.Color = Colors.Green;
            AddObject(plane);
        }

        public void RenderScene()
        {
            Ray ray = new Ray();
            ray.Direction = new vec3(0, 0, -1);

            for (int r = 0; r < viewport.VertResolution; r++)
            {
                for (int c = 0; c < viewport.HorResolution; c++)
                {
                    float x = viewport.PixelSize * (c - (viewport.HorResolution - 1.0f) / 2);
                    float y = viewport.PixelSize * (r - (viewport.VertResolution - 1.0f) / 2);
                    float z = 100;
                    ray.Origin = new vec3(x, y, z);

                    vec3 color = Trace(ray);
                    DisplayPixel(r, c, color);
                }
            }

            Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        private vec3 Trace(Ray ray)
        {
            float tmin = float.MaxValue;
            float t = 0;
            vec3 color = Colors.Black;

            foreach (var obj in objects)
            {
                if (obj.Hit(ray, ref t))
                {
                    if (t < tmin)
                    {
                        tmin = t;
                        color = obj.Color;
                    }
                }
            }
            return color;
        }

        void DisplayPixel(int row, int column, vec3 color)
        {
            Bitmap.SetPixel(column, row, Color.FromArgb((int)(color.x * 255), (int)(color.y * 255), (int)(color.z * 255)));
        }

        void AddObject(GeometricObject obj) => objects.Add(obj);
    }
}
