
using OpenTK.Graphics.OpenGL4;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class LineShape : Shape2D
    {
        public LineShape(vec2 point1, vec2 point2, vec3 color) : base(new List<vec2>(new vec2[] {point1, point2}), color)
        {
            primitiveType = PrimitiveType.Lines;
        }

        public void Set(vec2 point1, vec2 point2)
        {
            Points = new List<vec2>(new vec2[] { point1, point2 });
        }
    }
}
