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
        static int w = 400;
        static int h = 400;
        Bitmap mainImage = new Bitmap(w, h);
        Model model = Model.FromFile("head.obj");
        vec3 p1 = new vec3(20, 34, 50.0f);
        vec3 p2 = new vec3(744, 400, 50.0f);
        vec3 p3 = new vec3(120, 434, 50.0f);
        float[,] zbuffer = new float[w, h];
        bool isRandomColor = false;

        public Form1()
        {
            InitializeComponent();
            ClearBitmap();
            Width = 1000;
            Height = 1000;

            InitZBuffer();

            KeyPress += Form1_KeyPress;

            Paint += Form1_Paint;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'c')
            {
                isRandomColor = !isRandomColor;
                Invalidate();
            }
            else if (e.KeyChar == 's')
            {
                mainImage.Save("output.png");
            }
        }

        private void InitZBuffer()
        {
            for (int x = 0; x < zbuffer.GetLength(0); x++)
            {
                for (int y = 0; y < zbuffer.GetLength(1); y++)
                {
                    zbuffer[x, y] = -1.0f;
                }
            }
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
            InitZBuffer();

            DrawModel();

            //DrawZBuffer();

            mainImage.RotateFlip(RotateFlipType.RotateNoneFlipY);

            e.Graphics.DrawImage(mainImage, 0, 0);
        }

        private void DrawZBuffer()
        {
            ClearBitmap();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float zb = zbuffer[i, j];
                    int v = (int)((zbuffer[i, j] + 1.0f) / 2 * 255);
                    mainImage.SetPixel(i, j, Color.FromArgb(v, v, v));
                }
            }
        }

        static Random random = new Random();

        private void DrawModel()
        {
            vec3 lightDirection = new vec3(0, 0, -1);
            int count = 0;
            for (int i = 0; i < model.Faces.Count; i++)
            {
                var worldCoords = new vec3[3];
                var screenCoords = new vec3[3];

                for (int j = 0; j < 3; j++)
                {
                    int w = Form1.w - 1;
                    int h = Form1.h - 1;
                    vec3 v0 = model.GetVertex(i, j);
                    if (Math.Abs(v0.x) <= 1 && Math.Abs(v0.y) <= 1)
                    {
                        worldCoords[j] = v0;
                        screenCoords[j] = WorldToScreen(v0, w, h);
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
                    if (isRandomColor)
                    {
                        color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    }

                    Renderer.Triangle(screenCoords[0], screenCoords[1], screenCoords[2], color, mainImage, zbuffer);
                }
            }
            Console.WriteLine(count);
        }

        private static vec3 WorldToScreen(vec3 point, int w, int h)
        {
            return new vec3((point.x + 1.0f) * w / 2 + 0.5f, (point.y + 1.0f) * h / 2 + 0.5f, point.z);
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
