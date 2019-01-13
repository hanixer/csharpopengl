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
        World world;
        float pixelSize = 1;

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            KeyPress += Form1_KeyPress;
            Paint += Form1_Paint;
            world = new World();
            Width = 1000;
            Height = 1000;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //RenderScene(e);
            world.RenderScene();

            DrawBitmapWithZoom(e, world.Bitmap);
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
            world.RenderScene();
            e.Graphics.DrawImage(world.Bitmap, 0, 0);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 's')
            {
                world.Bitmap.Save("output.bmp");
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
                world.AddPlaneZ(1);
                Invalidate();
            }
            else if (e.KeyChar == 'd')
            {
                world.AddPlaneZ(-1);
                Invalidate();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
