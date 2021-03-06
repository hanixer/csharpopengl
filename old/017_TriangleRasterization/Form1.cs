﻿using System;
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
        static int w = 1000;
        static int h = 1000;
        Bitmap mainImage = new Bitmap(w, h);
        Model model = Model.FromFile("head.obj");
        float[,] zbuffer = new float[w, h];
        bool isRandomColor = false;
        float userValue = 0.0f;

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
                mainImage.Save("output.bmp");
            }
            else if (e.KeyChar == 'w')
            {
                userValue += 0.05f;
                Invalidate();
            }
            else if (e.KeyChar == 'q')
            {
                userValue -= 0.05f;
                Invalidate();
            }
            else if (e.KeyChar == 'r')
            {
                isRandomColor = !isRandomColor;
                Invalidate();
            }
        }

        private void InitZBuffer()
        {
            for (int x = 0; x < zbuffer.GetLength(0); x++)
            {
                for (int y = 0; y < zbuffer.GetLength(1); y++)
                {
                    zbuffer[x, y] = float.MaxValue;
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

        static string Str(vec4 v) => string.Format("[{0}, {1}, {2}, {3}]", v.x, v.y, v.z, v.w);
        static string Str(vec3 v) => string.Format("[{0}, {1}, {2}]", v.x, v.y, v.z);

        private void DrawModel()
        {
            vec3 lightDirection = new vec3(0, 0, -1);
            int count = 0;
            mat4 viewport = Transformations.ViewportTransformation(w, h);
            mat4 perspective = Transformations.PerspectiveTransformation(0.5f, 0.5f, 1, 10);
            mat4 cameraTransf = Transformations.LookAt(new vec3(1.5f + userValue, 0, 1.5f), new vec3(0, 0, 0));
            for (int i = 0; i < model.Faces.Count; i++)
            {
                var worldCoords = new vec3[3];
                var screenCoords = new vec3[3];

                for (int j = 0; j < 3; j++)
                {
                    int w = Form1.w - 1;
                    int h = Form1.h - 1;
                    vec3 v = model.GetVertex(i, j);
                    vec3 screen = ToScreen(viewport, perspective, cameraTransf, v);
                    if (Math.Abs(v.x) <= 1 && Math.Abs(v.y) <= 1)
                    {
                        worldCoords[j] = v;
                        screenCoords[j] = screen;
                    }
                }

                vec3 norm = glm.normalize(glm.cross(worldCoords[2] - worldCoords[0], worldCoords[1] - worldCoords[0]));

                float intensity = glm.dot(norm, lightDirection);

                if (intensity > 0 || isRandomColor)
                {
                    var value = (int)(intensity * 255);
                    count++;
                    Color color;
                    if (isRandomColor)
                    {
                        color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
                    }
                    else
                    {
                        color = Color.FromArgb(value, value, value);
                    }
                    
                    Renderer.Triangle(screenCoords[0], screenCoords[1], screenCoords[2], color, mainImage, zbuffer);
                }
            }
            Console.WriteLine(count);
        }

        private static vec3 ToScreen(mat4 viewport, mat4 perspective, mat4 cameraTransf, vec3 v)
        {
            vec4 obj = new vec4(v, 1);
            vec4 eye = cameraTransf * obj;
            vec4 clip = perspective * eye;
            vec4 screen = viewport * clip;
            vec3 final = Transformations.Vec4ToVec3(screen);

            //Console.WriteLine("{0} => {1} - eye", Str(obj), Str(eye));
            //Console.WriteLine("{0} => {1} - clip", Str(eye), Str(clip));
            //Console.WriteLine("{0} - norma", Str(Transformations.Vec4ToVec3(clip)));
            //Console.WriteLine("{0} => {1} - screen", Str(clip), Str(screen));
            //Console.WriteLine("{0} => {1} - final", Str(screen), Str(final));
            //Console.WriteLine();

            return final;
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
