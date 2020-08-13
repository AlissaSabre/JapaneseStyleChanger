using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using NMeCab;
using NMeCab.Alissa;
using JapaneseStyleChanger;

namespace JapaneseStyleChangerTest
{
    public class BaseClass
    {
        protected static Tagger<WNode> Tagger;

        protected static LanguageStyleChanger Changer;

        protected static TokenCombiner<WNode> Combiner;

        protected static void ClassInitialize(TestContext context)
        {
            var dir = Path.GetDirectoryName(typeof(BaseClass).Assembly.Location);
            var dic_dir = Path.Combine(dir, "UniDic-CWJ");
            Tagger = Tagger<WNode>.Create(() => new WNode(), dic_dir);
            Changer = new LanguageStyleChanger(Tagger);
            Combiner = new TokenCombiner<WNode>(n => n.Surface, n => n.RLength != n.Length);
        }

        protected static void ClassCleanup()
        {
            Combiner = null;
            Changer = null;
            Tagger?.Dispose();
            Tagger = null;
        }
    }
}
