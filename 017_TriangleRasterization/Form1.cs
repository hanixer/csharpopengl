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
        static int w = 800;
        static int h = 600;
        Bitmap mainImage = new Bitmap(w, h);
        Bitmap lineImage = new Bitmap(w, 16);
        Model model = Model.FromFile("head.obj");
        vec2i p1 = new vec2i(20, 34);
        vec2i p2 = new vec2i(744, 400);
        vec2i p3 = new vec2i(120, 434);
        vec2i p4 = new vec2i(444, 400);
        vec2i p5 = new vec2i(330, 463);
        vec2i p6 = new vec2i(594, 200);
        int[] ybuffer = new int[w];

        public Form1()
        {
            InitializeComponent();
            ClearBitmap();
            Width = w + 20;
            Height = h + 30 + lineImage.Height * 4;

            for (int i = 0; i < ybuffer.Length; i++)
            {
                ybuffer[i] = int.MinValue;
                for (int j = 0; j < lineImage.Height; j++)
                {
                    lineImage.SetPixel(i, j, Color.Black);
                }
            }

            Paint += Form1_Paint;
        }

        private void ClearBitmap()
        {
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    mainImage.SetPixel(i, j, Color.Black);
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            ClearBitmap();

            Renderer.Line(p1, p2, Color.Red, mainImage);
            Renderer.Line(p3, p4, Color.Green, mainImage);
            Renderer.Line(p5, p6, Color.Blue, mainImage);

            Renderer.LineWithBuffer(p1, p2, Color.Red, lineImage, ybuffer);
            Renderer.LineWithBuffer(p3, p4, Color.Green, lineImage, ybuffer);
            Renderer.LineWithBuffer(p5, p6, Color.Blue, lineImage, ybuffer);


            mainImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            e.Graphics.DrawImage(mainImage, 0, 0);
            e.Graphics.DrawImage(lineImage, 0, mainImage.Height + 20);
        }

        static Random random = new Random();

        private void DrawModelNew()
        {
            vec3 lightDirection = new vec3(0, 0, -1);
            int count = 0;
            for (int i = 0; i < model.Faces.Count; i++)
            {
                var worldCoords = new vec3[3];
                var screenCoords = new vec2i[3];
                for (int j = 0; j < 3; j++)
                {
                    int w = Form1.w - 1;
                    int h = Form1.h - 1;
                    vec3 v0 = model.GetVertex(i, j);
                    if (Math.Abs(v0.x) <= 1 && Math.Abs(v0.y) <= 1)
                    {
                        worldCoords[j] = v0;
                        screenCoords[j] = new vec2i(Convert(w, v0.x), Convert(h, v0.y));
                    }
                }

                vec3 norm = glm.normalize(glm.cross(worldCoords[2] - worldCoords[0], worldCoords[1] - worldCoords[0]));

                float intensity = glm.dot(norm, lightDirection);
                //intensity = 1;
                if (intensity > 0)
                {
                    var value = (int)(intensity * 255);
                    count++;
                    var color = Color.FromArgb(value, value, value);
                    //color = Color.Bisque;
                    //color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    Renderer.Triangle(screenCoords[0], screenCoords[1], screenCoords[2], color, mainImage);
                }
            }
            Console.WriteLine(count);
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
    }
}
