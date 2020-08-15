﻿// <auto-generated>
// THIS FILE (HtmlCases.cs) IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE .tt FILE INSTEAD.
// </auto-generated>

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NMeCab;
using NMeCab.Alissa;
using JapaneseStyleChanger;

namespace JapaneseStyleChangerTest
{
    [TestClass]
    public class HtmlCases
    {
		// Generated cases.


		[TestMethod]
		public void Html_001_CG()
		{
			var result = ConvertText("赤い花が咲いています。", CombineMode.CG);
			Assert.AreEqual("赤い花が咲いている。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_001_MS()
        {
			var result = ConvertText("赤い花が咲いています。", CombineMode.MS);
			Assert.AreEqual("赤い花が咲いている。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_002_CG()
		{
			var result = ConvertText("<i>赤い花が咲いています。</i>", CombineMode.CG);
			Assert.AreEqual("<i>赤い花が咲いている。</i>", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_002_MS()
        {
			var result = ConvertText("<i>赤い花が咲いています。</i>", CombineMode.MS);
			Assert.AreEqual("<i>赤い花が咲いている。</i>", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_003_CG()
		{
			var result = ConvertText("<i>赤い</i>花が咲いています。", CombineMode.CG);
			Assert.AreEqual("<i>赤い</i>花が咲いている。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_003_MS()
        {
			var result = ConvertText("<i>赤い</i>花が咲いています。", CombineMode.MS);
			Assert.AreEqual("<i>赤い</i>花が咲いている。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_004_CG()
		{
			var result = ConvertText("赤い<i>花が</i>咲いています。", CombineMode.CG);
			Assert.AreEqual("赤い<i>花が</i>咲いている。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_004_MS()
        {
			var result = ConvertText("赤い<i>花が</i>咲いています。", CombineMode.MS);
			Assert.AreEqual("赤い<i>花が</i>咲いている。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_005_CG()
		{
			var result = ConvertText("赤い花が<i>咲いています</i>。", CombineMode.CG);
			Assert.AreEqual("赤い花が<i>咲いている</i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_005_MS()
        {
			var result = ConvertText("赤い花が<i>咲いています</i>。", CombineMode.MS);
			Assert.AreEqual("赤い花が<i>咲いている</i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_006_CG()
		{
			var result = ConvertText("赤い花が<i>咲いて</i>います。", CombineMode.CG);
			Assert.AreEqual("赤い花が<i>咲いて</i>いる。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_006_MS()
        {
			var result = ConvertText("赤い花が<i>咲いて</i>います。", CombineMode.MS);
			Assert.AreEqual("赤い花が<i>咲いて</i>いる。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_007_CG()
		{
			var result = ConvertText("赤い花が咲いて<i>います</i>。", CombineMode.CG);
			Assert.AreEqual("赤い花が咲いて<i>いる</i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_007_MS()
        {
			var result = ConvertText("赤い花が咲いて<i>います</i>。", CombineMode.MS);
			Assert.AreEqual("赤い花が咲いて<i>いる</i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_008_CG()
		{
			var result = ConvertText("赤い花が咲いて<i>い</i>ます。", CombineMode.CG);
			Assert.AreEqual("赤い花が咲いて<i>いる</i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_008_MS()
        {
			var result = ConvertText("赤い花が咲いて<i>い</i>ます。", CombineMode.MS);
			Assert.AreEqual("赤い花が咲いて<i>いる</i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_009_CG()
		{
			var result = ConvertText("赤い花が咲いてい<i>ます</i>。", CombineMode.CG);
			Assert.AreEqual("赤い花が咲いている<i></i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_009_MS()
        {
			var result = ConvertText("赤い花が咲いてい<i>ます</i>。", CombineMode.MS);
			Assert.AreEqual("赤い花が咲いている<i></i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_011_CG()
		{
			var result = ConvertText("赤い花が咲いています。", CombineMode.CG);
			Assert.AreEqual("赤い花が咲いている。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_011_MS()
        {
			var result = ConvertText("赤い花が咲いています。", CombineMode.MS);
			Assert.AreEqual("赤い花が咲いている。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_012_CG()
		{
			var result = ConvertText("<i><b>赤い花が咲いています。</b></i>", CombineMode.CG);
			Assert.AreEqual("<i><b>赤い花が咲いている。</b></i>", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_012_MS()
        {
			var result = ConvertText("<i><b>赤い花が咲いています。</b></i>", CombineMode.MS);
			Assert.AreEqual("<i><b>赤い花が咲いている。</b></i>", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_013_CG()
		{
			var result = ConvertText("<i><b>赤い</b></i>花が咲いています。", CombineMode.CG);
			Assert.AreEqual("<i><b>赤い</b></i>花が咲いている。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_013_MS()
        {
			var result = ConvertText("<i><b>赤い</b></i>花が咲いています。", CombineMode.MS);
			Assert.AreEqual("<i><b>赤い</b></i>花が咲いている。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_014_CG()
		{
			var result = ConvertText("赤い<i><b>花が</b></i>咲いています。", CombineMode.CG);
			Assert.AreEqual("赤い<i><b>花が</b></i>咲いている。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_014_MS()
        {
			var result = ConvertText("赤い<i><b>花が</b></i>咲いています。", CombineMode.MS);
			Assert.AreEqual("赤い<i><b>花が</b></i>咲いている。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_015_CG()
		{
			var result = ConvertText("赤い花が<i><b>咲いています</b></i>。", CombineMode.CG);
			Assert.AreEqual("赤い花が<i><b>咲いている</b></i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_015_MS()
        {
			var result = ConvertText("赤い花が<i><b>咲いています</b></i>。", CombineMode.MS);
			Assert.AreEqual("赤い花が<i><b>咲いている</b></i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_021_CG()
		{
			var result = ConvertText("<i>花です</i>。", CombineMode.CG);
			Assert.AreEqual("<i>花だ</i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_021_MS()
        {
			var result = ConvertText("<i>花です</i>。", CombineMode.MS);
			Assert.AreEqual("<i>花だ</i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_022_CG()
		{
			var result = ConvertText("花<i>です</i>。", CombineMode.CG);
			Assert.AreEqual("花<i>だ</i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_022_MS()
        {
			var result = ConvertText("花<i>です</i>。", CombineMode.MS);
			Assert.AreEqual("花<i>だ</i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_023_CG()
		{
			var result = ConvertText("花で<i>す</i>。", CombineMode.CG);
			Assert.AreEqual("花だ<i></i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_023_MS()
        {
			var result = ConvertText("花で<i>す</i>。", CombineMode.MS);
			Assert.AreEqual("花だ<i></i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_024_CG()
		{
			var result = ConvertText("<i>花でした</i>。", CombineMode.CG);
			Assert.AreEqual("<i>花だった</i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_024_MS()
        {
			var result = ConvertText("<i>花でした</i>。", CombineMode.MS);
			Assert.AreEqual("<i>花だった</i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_025_CG()
		{
			var result = ConvertText("花<i>でした</i>。", CombineMode.CG);
			Assert.AreEqual("花<i>だった</i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_025_MS()
        {
			var result = ConvertText("花<i>でした</i>。", CombineMode.MS);
			Assert.AreEqual("花<i>だった</i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_026_CG()
		{
			var result = ConvertText("花で<i>した</i>。", CombineMode.CG);
			Assert.AreEqual("花だっ<i>た</i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_026_MS()
        {
			var result = ConvertText("花で<i>した</i>。", CombineMode.MS);
			Assert.AreEqual("花だっ<i>た</i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_027_CG()
		{
			var result = ConvertText("花で<i>し</i>た。", CombineMode.CG);
			Assert.AreEqual("花だっ<i></i>た。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_027_MS()
        {
			var result = ConvertText("花で<i>し</i>た。", CombineMode.MS);
			Assert.AreEqual("花だっ<i></i>た。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_028_CG()
		{
			var result = ConvertText("花でし<i>た</i>。", CombineMode.CG);
			Assert.AreEqual("花だっ<i>た</i>。", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_028_MS()
        {
			var result = ConvertText("花でし<i>た</i>。", CombineMode.MS);
			Assert.AreEqual("花だっ<i>た</i>。", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_031_CG()
		{
			var result = ConvertText("<i>その2日後</i>", CombineMode.CG);
			Assert.AreEqual("<i>その2日後</i>", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_031_MS()
        {
			var result = ConvertText("<i>その2日後</i>", CombineMode.MS);
			Assert.AreEqual("<i>その 2 日後</i>", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_032_CG()
		{
			var result = ConvertText("<i>その 2 日後</i>", CombineMode.CG);
			Assert.AreEqual("<i>その2日後</i>", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_032_MS()
        {
			var result = ConvertText("<i>その 2 日後</i>", CombineMode.MS);
			Assert.AreEqual("<i>その 2 日後</i>", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_033_CG()
		{
			var result = ConvertText("その<i>2</i>日後", CombineMode.CG);
			Assert.AreEqual("その<i>2</i>日後", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_033_MS()
        {
			var result = ConvertText("その<i>2</i>日後", CombineMode.MS);
			Assert.AreEqual("その <i>2</i> 日後", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_034_CG()
		{
			var result = ConvertText("その <i>2</i> 日後", CombineMode.CG);
			Assert.AreEqual("その<i>2</i>日後", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_034_MS()
        {
			var result = ConvertText("その <i>2</i> 日後", CombineMode.MS);
			Assert.AreEqual("その <i>2</i> 日後", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_035_CG()
		{
			var result = ConvertText("その<i> 2 </i>日後", CombineMode.CG);
			Assert.AreEqual("その<i>2</i>日後", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_035_MS()
        {
			var result = ConvertText("その<i> 2 </i>日後", CombineMode.MS);
			Assert.AreEqual("その<i> 2 </i>日後", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_036_CG()
		{
			var result = ConvertText("その<i>2日</i>後", CombineMode.CG);
			Assert.AreEqual("その<i>2日</i>後", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_036_MS()
        {
			var result = ConvertText("その<i>2日</i>後", CombineMode.MS);
			Assert.AreEqual("その <i>2 日</i>後", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_037_CG()
		{
			var result = ConvertText("その <i>2 日</i>後", CombineMode.CG);
			Assert.AreEqual("その<i>2日</i>後", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_037_MS()
        {
			var result = ConvertText("その <i>2 日</i>後", CombineMode.MS);
			Assert.AreEqual("その <i>2 日</i>後", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_038_CG()
		{
			var result = ConvertText("その<i> 2 日</i>後", CombineMode.CG);
			Assert.AreEqual("その<i>2日</i>後", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_038_MS()
        {
			var result = ConvertText("その<i> 2 日</i>後", CombineMode.MS);
			Assert.AreEqual("その<i> 2 日</i>後", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_100_CG()
		{
			var result = ConvertText("試してみましょう<i>", CombineMode.CG);
			Assert.AreEqual("試してみよう<i>", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_100_MS()
        {
			var result = ConvertText("試してみましょう<i>", CombineMode.MS);
			Assert.AreEqual("試してみよう<i>", result, "Unexpected MS text.");
		}

		[TestMethod]
		public void Html_101_CG()
		{
			var result = ConvertText("とても素敵です</i>", CombineMode.CG);
			Assert.AreEqual("とても素敵だ</i>", result, "Unexpected CG text.");
        }

        [TestMethod]
        public void Html_101_MS()
        {
			var result = ConvertText("とても素敵です</i>", CombineMode.MS);
			Assert.AreEqual("とても素敵だ</i>", result, "Unexpected MS text.");
		}

		// Postamble

		private static TextStyleChanger Changer;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
			Changer = new TextStyleChanger
			{
				ChangeToJotai = true,
				HtmlSyntax = true,
			};
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
			Changer.Dispose();
			Changer = null;
        }

        private static string ConvertText(string text, CombineMode mode)
        {
	        Changer.CombineMode = mode;
			return Changer.ChangeText(text);
        }
    }
}