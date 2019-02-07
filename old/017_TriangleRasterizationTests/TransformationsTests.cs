using Microsoft.VisualStudio.TestTools.UnitTesting;
using _014_DrawTriangle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace _014_DrawTriangle.Tests
{
    [TestClass()]
    public class TransformationsTests
    {

        [TestMethod()]
        public void LookAtTest6()
        {
            TestLookAt(new vec3(1, 0, 0), new vec3(0, 0, 0), new vec3(0, 1, 0), new vec3(1, 1, 0), 0, 1, 0);
        }

        [TestMethod()]
        public void LookAtTestZero()
        {
            TestLookAt(new vec3(0, 0, 1), new vec3(0, 0, 0), new vec3(0, 1, 0), new vec3(0, 0, 0), 0, 0, -1);
        }

        [TestMethod()]
        public void LookAtTestRightOnX()
        {
            TestLookAt(new vec3(0, 0, 1), new vec3(0, 0, 0), new vec3(0, 1, 0), new vec3(1, 0, 0), 1, 0, -1);
        }

        [TestMethod()]
        public void StandOnXPointZero()
        {
            TestLookAt(new vec3(1, 0, 0), new vec3(0, 0, 0), new vec3(0, 1, 0), new vec3(0, 0, 0), 0, 0, -1);
        }

        [TestMethod()]
        public void StandOnXPointOneOneOne()
        {
            TestLookAt(new vec3(1, 0, 0), new vec3(0, 0, 0), new vec3(0, 1, 0), new vec3(1, 1, 1), -1, 1, 0);
        }

        private static void TestLookAt(vec3 eye, vec3 target, vec3 up, vec3 point, float ex, float ey, float ez)
        {
            mat4 m = Transformations.LookAt(eye, target, up);
            
            vec4 result = m * new vec4(point, 1);

            Assert.AreEqual(ex, result.x, 0.0001f);
            Assert.AreEqual(ey, result.y, 0.0001f);
            Assert.AreEqual(ez, result.z, 0.0001f);
        }

        private static void TestIt(mat4 m, float x, float y, float ex, float ey)
        {
            vec4 v = new vec4(x, y, -0.5f, 1);
            vec4 r = m * v;
            Assert.AreEqual(ex + 0.5f, r.x);
            Assert.AreEqual(ey + 0.5f, r.y);
        }
    }
}