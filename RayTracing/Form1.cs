using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RayTracing
{
    public partial class Form1 : Form
    {
        Scene scene;
        float pixelSize = 1;

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            KeyPress += Form1_KeyPress;
            Paint += Form1_Paint;
            scene = new Scene();
            Width = 1000;
            Height = 1000;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //RenderScene(e);
            scene.RenderScene();

            DrawBitmapWithZoom(e, scene.Bitmap);
        }

        private void DrawBitmapWithZoom(PaintEventArgs e, Bitmap bitmap)
        {
            var brush = new SolidBrush(Color.Black);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    brush.Color = bitmap.GetPixel(x, y);
                    e.Graphics.FillRectangle(brush, x * pixelSize, y * pixelSize, pixelSize, pixelSize);
                }
            }
        }

        private void RenderScene(PaintEventArgs e)
        {
            scene.RenderScene();
            e.Graphics.DrawImage(scene.Bitmap, 0, 0);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 's')
            {
                scene.Bitmap.Save("output.bmp");
            }
            else if (e.KeyChar == 'w')
            {
                pixelSize += 1;
                Invalidate();
            }
            else if (e.KeyChar == 'e')
            {
                pixelSize -= 1;
                Invalidate();
            }
            else if (e.KeyChar == 'q')
            {
                pixelSize *= 2;
                Invalidate();
            }
            else if (e.KeyChar == 'a')
            {
                pixelSize /= 2;
                Invalidate();
            }
            else if (e.KeyChar == 'f')
            {
                scene.AddPlaneZ(1);
                Invalidate();
            }
            else if (e.KeyChar == 'd')
            {
                scene.AddPlaneZ(-1);
                Invalidate();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
