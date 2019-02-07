using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL4;
using Utilities;

namespace WindowsFormsApplication1
{
    public class MyControl : GLControl
    {

        public MyControl()
        {
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            OnLoadEvent(this, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            OnPaintEvent(this, e);
        }

        public event EventHandler<PaintEventArgs> OnPaintEvent;
        public event EventHandler<EventArgs> OnLoadEvent;
    }
}
