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
        //[TestMethod()]
        //public void LookAtTest1()
        //{
        //    vec3 eye = new vec3(0, 0, 3);
        //    vec3 target = new vec3(0);
        //    vec3 up = new vec3(0, 1, 0);
        //    vec3 point = new vec3(1, 1, 0);

        //    float ex = 1;
        //    float ey = 1;

        //    TestLookAt(eye, target, up, point, ex, ey);
        //}

        //[TestMethod()]
        //public void LookAtTest2()
        //{
        //    TestLookAt(new vec3(1, 0, 1), new vec3(1, 0, 0), new vec3(0, 1, 0), new vec3(0), -1, 0);
        //}

        //[TestMethod()]
        //public void LookAtTest3()
        //{
        //    TestLookAt(new vec3(1, 1, 1), new vec3(0, 0, 0), new vec3(0, 1, 0), new vec3(0), 0, 0);
        //}

        //[TestMethod()]
        //public void LookAtTest4()
        //{
        //    TestLookAt(new vec3(1, 0, 1), new vec3(0, 0, 0), new vec3(0, 1, 0), new vec3(0), 0, -1);
        //}

        //[TestMethod()]
        //public void LookAtTest5()
        //{
        //    TestLookAt(new vec3(1, 0, 1), new vec3(0, 0, 0), new vec3(0, 1, 0), new vec3(1, 0, 0), 0.7071068f, 0);
        //}

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
            vec4 v = new vec4(x, y, 0, 1);
            vec4 r = m * v;
            Assert.AreEqual(ex + 0.5f, r.x);
            Assert.AreEqual(ey + 0.5f, r.y);
        }
    }
}