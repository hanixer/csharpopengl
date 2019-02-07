using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GlmNet;

namespace _014_DrawTriangle
{
    public partial class Form1 : Form
    {
        static int w = 800;
        static int h = 600;
        Bitmap bitmap = new Bitmap(w, h);

        public Form1()
        {
            InitializeComponent();

            Width = w;
            Height = h;

            Actionf();


            vec2 v0 = new vec2(0, 0);
            vec2 v1 = new vec2(100, 100);
            vec2 v2 = new vec2(100, 0);
            vec2 p = new vec2(30, 50);
            float w0 = edgeFunction(v1, v2, p);
            float w1 = edgeFunction(v2, v0, p);
            float w2 = edgeFunction(v0, v1, p);

            Console.WriteLine(w0);
            Console.WriteLine(w1);
            Console.WriteLine(w2);

            Paint += Form1_Paint;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bitmap, 0, 0);
        }

        static float edgeFunction(vec2 a, vec2 b, vec2 p)
        {
            return (p.x - a.x) * (b.y - a.y) - (p.y - a.y) * (b.x - a.x);
        }

        private void Actionf()
        {
            vec2 v0 = new vec2(0, 0);
            vec2 v1 = new vec2(100, 100);
            vec2 v2 = new vec2(100, 0);
            vec3 c0 = new vec3(1, 0, 0);
            vec3 c1 = new vec3(1, 1, 0);
            vec3 c2 = new vec3(1, 1, .5f);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    vec2 p = new vec2(x + 0.5f, y + 0.5f);
                    float w0 = edgeFunction(v1, v2, p);
                    float w1 = edgeFunction(v2, v0, p);
                    float w2 = edgeFunction(v0, v1, p);
                    if (w0 >= 0 && w1 >= 0 && w2 >= 0)
                    {
                        Color color = Color.FromArgb(255, 255, 255);
                        bitmap.SetPixel(x, y, color);
                    }
                    else
                    {
                        Color color = Color.FromArgb(0, 0, 0);
                        bitmap.SetPixel(x, y, color);
                        bitmap.SetPixel(x, y, Color.Red);
                    }
                }
            }
        }
    }
}
