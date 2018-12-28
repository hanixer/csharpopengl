
using OpenTK.Graphics.OpenGL4;
using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class PointShape : Shape
    {
        public PointShape(vec2 point, vec3 color) : base(new List<vec2>(new vec2[] {point}), color)
        {
            primitiveType = PrimitiveType.Points;
        }
    }
}
