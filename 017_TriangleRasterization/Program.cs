using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace _014_DrawTriangle
{
    static class Program
    {
        static int w = 100;
        static int h = 100;

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
            string text = @"
v -1.0 2.0 -0.433
v -1.0 2.0 -0.433
v -1.0 2.0 -0.433
f 0/1/2 0/1/2 0/1/1";
            Model m = Model.Parse(text);
            
            Console.WriteLine(m);
        }
    }
}
