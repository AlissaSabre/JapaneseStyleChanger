using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using JapaneseStyleChanger;
using System.Collections.Generic;

namespace JapaneseStyleChangerFuzzTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Empty()
        {
            Changer.ChangeText("");
        }

        [TestMethod]
        public void Hira1()
        {
            foreach (var c in AllKana)
            {
                Changer.ChangeText(c.ToString());
            }
        }

        [TestMethod]
        public void Hira2()
        {
            foreach (var c in AllKana)
            {
                foreach (var d in AllKana)
                {
                    Changer.ChangeText(new string(new[] { c, d }));
                }
            }
        }

        [TestMethod]
        public void Random3()
        {
            TestRandom(3);
        }

        [TestMethod]
        public void Random4()
        {
            TestRandom(4);
        }

        [TestMethod]
        public void Random6()
        {
            TestRandom(6);
        }

        [TestMethod]
        public void Random10()
        {
            TestRandom(10);
        }

        [TestMethod]
        public void Random16()
        {
            TestRandom(16);
        }

        private static void TestRandom(int length)
        {
            var text = new char[length];
            var rand = new Random(1780378116 + 82142 * length); // two numbers are chosen arbitrarily.
            for (int i = 0; i < 100000; i++)
            {
                for (int p = 0; p < length; p++)
                {
                    text[p] = AllKana[rand.Next(AllKana.Length)];
                }
                Changer.ChangeText(new string(text));
            }
        }

        private static char[] AllKana;

        private static TextStyleChanger Changer;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            {
                var list = new List<char>();
                for (int c = 0x3041; c <= 0x3093; c++) list.Add((char)c);
                for (int c = 0x3099; c <= 0x309E; c++) list.Add((char)c);
                AllKana = list.ToArray();
            }

            Changer = new TextStyleChanger
            {
                ChangeToJotai = true,
            };
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Changer.Dispose();
            Changer = null;
            AllKana = null;
        }
    }
}
