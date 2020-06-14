using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NMeCab.Alissa;

namespace JapaneseStyleChanger
{
    public class TextStyleChanger : IDisposable
    {
        private Tagger<WNode> Tagger;

        private LanguageStyleChanger Changer;

        private TokenCombiner<WNode> Combiner;

        public TextStyleChanger()
        {
            try
            {
                var dir = Path.GetDirectoryName(GetType().Assembly.Location);
                Tagger = Tagger<WNode>.Create(Path.Combine(dir, "UniDic-CWJ"));
                Changer = new LanguageStyleChanger(Tagger);
                Combiner = new TokenCombiner<WNode>(n => n.Surface, n => n.RLength != n.Length);
            }
            catch (Exception)
            {
                Tagger?.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            Combiner = null;
            Changer = null;
            Tagger?.Dispose();
            Tagger = null;
        }

        public bool ChangeToJotai { get; set; }

        public CombineMode CombineMode { get; set; }

        public WidthPreferences WidthPreferences { get; set; }

        public string ChangeText(string text)
        {
            IEnumerable<WNode> nodes = Tagger.Parse(text);
            if (ChangeToJotai)
            {
                var buffer = new EditBuffer(nodes);
                Changer.ToJotai(buffer);
                nodes = buffer;
            }
            Combiner.CombineMode = CombineMode;
            switch (WidthPreferences)
            {
                case WidthPreferences.HalfwidthParentheses:
                    Combiner.Postprocess = TokenCombiner.AsciiParentheses;
                    break;
                case WidthPreferences.FullwidthParentheses:
                    Combiner.Postprocess = TokenCombiner.FullWidthParentheses;
                    break;
                default:
                    Combiner.Postprocess = null;
                    break;
            }
            var result = Combiner.Combine(nodes);
            return result;
        }
    }

    [Flags]
    public enum WidthPreferences
    {
        None = 0,

        HalfwidthParentheses = 1,
        FullwidthParentheses = 2,
    }
}
