using GlmNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _014_DrawTriangle
{
    public struct vec3i
    {
        public int x, y, z;
        public vec3i(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public struct vec2i
    {
        public int x, y;
        public vec2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public override string ToString()
        {
            return string.Format("[{0}, {1}]", x, y);
        }
    }
    class Renderer
    {
        static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        public static void Line(int x0, int y0, int x1, int y1, Color color, Bitmap bitmap)
        {
            bool steep = false;

            if (Math.Abs(x1 - x0) < Math.Abs(y1 - y0))
            {
                steep = true;
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }

            if (x0 >= x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            if (x0 == x1)
                return;

            for (int x = x0; x <= x1; x++)
            {
                float t = (float)(x - x0) / (x1 - x0);
                int y = (int)(y0 + t * (y1 - y0));

                if (steep)
                {
                    SetPixel(y, x, color, bitmap);
                }
                else
                {
                    SetPixel(x, y, color, bitmap);
                }
            }
        }

        public static void Line(vec2i p0, vec2i p1, Color color, Bitmap bitmap)
        {
            Line(p0.x, p0.y, p1.x, p1.y, color, bitmap);
        }

        public static void LineWithBuffer(vec2i p0, vec2i p1, Color color, Bitmap bitmap, int[] buffer)
        {
            if (p0.x > p1.x)
            {
                vec2i temp = p0;
                p0 = p1;
                p1 = temp;
            }

            for (int x = p0.x; x <= p1.x; x++)
            {
                float t = (float)(x - p0.x) / (p1.x - p0.x);
                int y = (int)(p0.y + t * (p1.y - p0.y));
                int previousX = buffer[x];

                if (buffer[x] < y)
                {
                    buffer[x] = y;
                    for (int yy = 0; yy < bitmap.Height; yy++)
                    {
                        SetPixel(x, yy, color, bitmap);
                    }                    
                }
            }
        }

        static Tuple<vec2i, vec2i> GetBoundingBox(vec3i v0, vec3i v1, vec3i v2)
        {
            var xs = new List<int> { v0.x, v1.x, v2.x };
            var ys = new List<int> { v0.y, v1.y, v2.y };

            return new Tuple<vec2i, vec2i>(new vec2i(xs.Min(), ys.Min()), new vec2i(xs.Max(), ys.Max()));
        }

        static Tuple<vec2i, vec2i> GetBoundingBox(vec2i v0, vec2i v1, vec2i v2)
        {
            var xs = new List<int> { v0.x, v1.x, v2.x };
            var ys = new List<int> { v0.y, v1.y, v2.y };

            return new Tuple<vec2i, vec2i>(new vec2i(xs.Min(), ys.Min()), new vec2i(xs.Max(), ys.Max()));
        }

        public static void Triangle(vec2i v0, vec2i v1, vec2i v2, Color color, Bitmap bitmap)
        {
            var boundingBox = GetBoundingBox(v0, v1, v2);
            //Console.WriteLine("boundingBox = {0}, {1}", boundingBox.Item1, boundingBox.Item2);
            //Console.WriteLine("v0 = {0}, v1 = {1}, v2 = {2}", v0, v1, v2);

            //boundingBox = new Tuple<vec2i, vec2i>(new vec2i(0, 0), new vec2i(bitmap.Width, bitmap.Height));
            for (int y = boundingBox.Item1.y; y < boundingBox.Item2.y; y++)
            {
                for (int x = boundingBox.Item1.x; x < boundingBox.Item2.x; x++)
                {
                    vec2i point = new vec2i(x, y);
                    int w0 = EdgeFunction(v0, v1, point);
                    int w1 = EdgeFunction(v1, v2, point);
                    int w2 = EdgeFunction(v2, v0, point);
                    //Console.WriteLine("v0 = {3}, v1 = {4}, v2 = {5}, point = {6}, w0 = {0}, w1 = {1}, w2 = {2}", w0, w1, w2, v0, v1, v2, point);
                    if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                    {
                        SetPixel(x, y, color, bitmap);
                    }
                }
            }
        }

        public static int EdgeFunction(vec2i start, vec2i end, vec2i point) => (point.y - start.y) * (end.x - start.x) - (point.x - start.x) * (end.y - start.y);

        private static void SetPixel(int x, int y, Color color, Bitmap bitmap)
        {
            //bitmap.SetPixel(x, bitmap.Height - y - 1, color);
            bitmap.SetPixel(x, y, color);
        }
    }
}
