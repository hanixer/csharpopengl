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
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            KeyPress += Form1_KeyPress;
            Paint += Form1_Paint;
            world = new World();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
