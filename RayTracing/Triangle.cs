using GlmNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracing
{
    class Triangle
    {
        private vec3[] points;
        private vec3[] normals;

        public Triangle(vec3[] points, vec3[] normals)
        {
            Debug.Assert(points.Length == 3);
            Debug.Assert(normals.Length == 3);

            this.points = points;
            this.normals = normals;
        }

        public vec3 Point(int i)
        {
            return points[i];
        }

        public vec3 Normal(int i)
        {
            return normals[i];
        }
    }
}
