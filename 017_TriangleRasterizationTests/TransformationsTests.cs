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
        mat4 m = Transformations.MakeViewportTransformation(10, 10);
        [TestMethod()]
        public void MakeViewportTransformationTest()
        {
            float x = -1;
            float y = -1;
            float ex = 0;
            float ey = 0;

            TestIt(m, x, y, ex, ey);
        }
        [TestMethod()]
        public void MakeViewportTransformationTest2()
        {
            TestIt(m, 0, 0, 4, 4);
        }
        [TestMethod()]
        public void MakeViewportTransformationTest3()
        {
            m = Transformations.MakeViewportTransformation(10000, 10000);
            //TestIt(m, 1, 1, 2, 2);
        }
        [TestMethod()]
        public void MakeViewportTransformationTest4()
        {
            m = Transformations.MakeViewportTransformation(3, 3);
            TestIt(m, 0, -1, 1, 0);
        }

        private static void TestIt(mat4 m, float x, float y, float ex, float ey)
        {
            vec4 v = new vec4(x, y, 0, 1);
            vec4 r = m * v;
            Assert.AreEqual(ex + 0.5f, r.x);
            Assert.AreEqual(ey + 0.5f, r.y);
        }
    }
}