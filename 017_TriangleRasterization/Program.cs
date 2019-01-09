using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;
using GlmNet;

namespace _014_DrawTriangle
{
    static class Program
    {
        static int w = 100;
        static int h = 100;

        static void Doo(float x, float y)
        {
            vec4 v1 = new vec4(x, y, 0, 1);
            vec4 r1 = Transformations.MakeViewportTransformation(19, 19) * v1;
            
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Environment.CurrentDirectory = Environment.CurrentDirectory + "..\\..\\..\\";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
