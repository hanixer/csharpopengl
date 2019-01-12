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
        public Sphere sphere;
        Tracer tracer;

        public World()
        {
            viewport.HorResolution = 200;
            viewport.VertResolution = 200;
            viewport.PixelSize = 1;
            viewport.Gamma = 1;

            Bitmap = new Bitmap(viewport.HorResolution, viewport.VertResolution);

            sphere = new Sphere(new vec3(0), 80);

            tracer = new SphereTracer(this);
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
                    float z = 1;
                    ray.Origin = new vec3(x, y, z);

                    vec3 color = tracer.TraceRay(ray);

                    DisplayPixel(r, c, color);
                }
            }

            Bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        void DisplayPixel(int row, int column, vec3 color)
        {
            Bitmap.SetPixel(column, row, Color.FromArgb((int)color.x * 255, (int)color.y * 255, (int)color.z * 255));
        }
    }
}
