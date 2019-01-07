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
using Utilities;

namespace _014_DrawTriangle
{
    public partial class Form1 : Form
    {
        static int w = 500;
        static int h = 500;
        Bitmap bitmap = new Bitmap(w, h);
        Model model = Model.FromFile("head.obj");

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    bitmap.SetPixel(i, j, Color.Black);
                }
            }
            Width = w + 20;
            Height = h + 30;

            Paint += Form1_Paint;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //Renderer.Triangle(new vec2i(10, 70), new vec2i(50, 160), new vec2i(70, 80), Color.Red, bitmap);
            //Renderer.Triangle(new vec2i(180, 50), new vec2i(150, 1), new vec2i(70, 180), Color.White, bitmap);
            //Renderer.Triangle(new vec2i(180, 150), new vec2i(120, 160), new vec2i(130, 180), Color.Green, bitmap);
            //Renderer.Line(0, 0, 100, 100, Color.Gold, bitmap);
            //Renderer.Line(100, 100, 50, 125, Color.Goldenrod, bitmap);

            DrawModelNew();
            e.Graphics.DrawImage(bitmap, 0, 0);
        }

        private void DrawModelOld()
        {
            for (int i = 0; i < model.Faces.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int w = Form1.w - 1;
                    int h = Form1.h - 1;
                    vec3 v0 = model.GetVertex(i, j);
                    vec3 v1 = model.GetVertex(i, (j + 1) % 3);
                    int x0 = Convert(w, v0.x);
                    int y0 = Convert(h, v0.y);
                    int x1 = (int)((v1.x + 1.0f) * w / 2);
                    int y1 = (int)((v1.y + 1.0f) * h / 2);
                    Renderer.Line(x0, y0, x1, y1, Color.White, bitmap);
                }
            }
        }

        static Random random = new Random();

        private void DrawModelNew()
        {
            for (int i = 0; i < model.Faces.Count; i++)
            {
                var screenCoords = new vec2i[3];
                for (int j = 0; j < 3; j++)
                {
                    int w = Form1.w - 1;
                    int h = Form1.h - 1;
                    vec3 v0 = model.GetVertex(i, j);
                    screenCoords[j] = new vec2i(Convert(w, v0.x), Convert(h, v0.y));
                }



                Renderer.Triangle(screenCoords[0], screenCoords[1], screenCoords[2], Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)), bitmap);
            }
        }

        private static int Convert(int w, float r)
        {
            return (int)((r + 1.0f) * w / 2);
        }

        static float edgeFunction(vec2 a, vec2 b, vec2 p)
        {
            return (p.x - a.x) * (b.y - a.y) - (p.y - a.y) * (b.x - a.x);
        }

        static float Interpolate(vec3 v, float w0, float w1, float w2)
        {
            return v.x * w0 + v.y * w1 + v.z * w2;
        }

        private void Actionf()
        {
            vec2 v0 = new vec2(0, 0);
            vec2 v1 = new vec2(400, 400);
            vec2 v2 = new vec2(400, 0);
            vec3 c0 = new vec3(0.5f, 0, 0);
            vec3 c1 = new vec3(0, 0.5f, 0);
            vec3 c2 = new vec3(0, 0, .5f);

            float area = edgeFunction(v0, v1, v2);

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
                        w0 /= area;
                        w1 /= area;
                        w2 /= area;
                        int r = (int)(Interpolate(c0, w0, w1, w2) * 255);
                        int g = (int)(Interpolate(c1, w0, w1, w2) * 255);
                        int b = (int)(Interpolate(c2, w0, w1, w2) * 255);
                        Color color = Color.FromArgb(r, g, b);
                        bitmap.SetPixel(x, y, color);
                    }
                    else
                    {
                        Color color = Color.FromArgb(0, 0, 0);
                        bitmap.SetPixel(x, y, color);
                    }
                }
            }
        }
    }
}
