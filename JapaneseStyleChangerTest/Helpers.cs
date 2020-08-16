using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NMeCab.Alissa;
using JapaneseStyleChanger;

namespace JapaneseStyleChangerTest
{
    static class Helpers
    {
        public static Tagger<WNode> CreateTagger()
        {
            var dir = Path.GetDirectoryName(typeof(Helpers).Assembly.Location);
            var dic_dir = Path.Combine(dir, "UniDic-CWJ");
            return Tagger<WNode>.Create(() => new WNode(), dic_dir);
        }

        public static LanguageStyleChanger CreateLanguageStyleChanger(Tagger<WNode> tagger)
        {
            return new LanguageStyleChanger(tagger);
        }

        public static TokenCombiner<WNode> CreateTokenCombiner()
        {
            return new TokenCombiner<WNode>(n => n.Surface, n => n.RLength != n.Length);
        }
    }
}
