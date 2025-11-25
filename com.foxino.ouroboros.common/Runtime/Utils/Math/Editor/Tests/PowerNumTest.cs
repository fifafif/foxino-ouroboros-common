using NUnit.Framework;
using UnityEngine;

namespace Ouroboros.Common.Utils.Math
{
    public static class PowerMathTest
    {
        [TestCase(1, 5.0, 1, 2.0, 1, 3.0)]
        [TestCase(2, 1.0, 2, 5.0, 2, -4.0)]
        [TestCase(1, 5.0, 3, 2, 3, -1.95)]
        public static void TestMinus(int p1, double s1, int p2, double s2, int p3, double s3)
        {
            var n1 = new PowerNum(p1, s1);
            var n2 = new PowerNum(p2, s2);
            var expected = new PowerNum(p3, s3);

            var n = n1 - n2;

            Debug.Log("Result=" + n);
            Debug.Log("Expected=" + expected);

            Assert.True(PowerNum.AreSimilar(n, expected));
        }

        [TestCase]
        public static void TestPlus()
        {
            var n1 = new PowerNum(10, 6f);
            var n2 = new PowerNum(10, 6f);

            var n = n1 + n2;

            Debug.Log(n);

            Assert.AreEqual(n.power, 11);
        }

        [Test]
        public static void TestDivision()
        {
            var n1 = new PowerNum(10, 6f);
            var n2 = new PowerNum(10, 6f);

            var n = n1 / n2;

            Debug.Log(n);

            Assert.AreEqual(n.power, 0);
        }

        [Test]
        public static void TestDivision2()
        {
            var n1 = new PowerNum(10, 4f);
            var n2 = new PowerNum(5, 7f);

            var n = n1 / n2;

            Debug.Log(n);

            Assert.AreEqual(n.power, 4);
        }

        [Test]
        public static void TestMultiplication()
        {
            var n1 = new PowerNum(10, 5f);
            var n2 = new PowerNum(10, 5f);

            var n = n1 * n2;

            Debug.Log(n);

            Assert.AreEqual(n.power, 21);
        }

        [Test]
        [TestCase(10, 1, 10, 9, 10, 5, 0.5)]
        [TestCase(9, 1, 10, 1, 9, 5.5, 0.5)]
        [TestCase(9, 1, 10, 1, 9, 1, 0.0)]
        [TestCase(9, 1, 10, 1, 10, 1, 1.0)]
        public static void TestLerp(int p1, double s1, int p2, double s2, int p3, double s3, double f)
        {
            var n1 = new PowerNum(p1, s1);
            var n2 = new PowerNum(p2, s2);
            var expected = new PowerNum(p3, s3);

            var n = PowerNum.Lerp(n1, n2, f);

            Debug.Log("Result=" + n);
            Debug.Log("Expected=" + expected);

            Assert.True(PowerNum.AreSimilar(n, expected));
        }
    }
}