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
                Tagger = Tagger<WNode>.Create(() => new WNode(), Path.Combine(dir, "UniDic-CWJ"));
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

        public JotaiPreferences JotaiPreferences { get; set; }

        public bool HtmlSyntax { get; set; }

        public CombineMode CombineMode { get; set; }

        public WidthPreferences WidthPreferences { get; set; }

        public IEnumerable<char> CustomFullwidthSet { get; set; }

        public IEnumerable<char> CustomHalfwidthSet { get; set; }

        public string ChangeText(string input)
        {
            string text = input;

            HtmlHandler html = null;
            if (HtmlSyntax)
            {
                html = new HtmlHandler();
                html.OriginalHtml = text;
                text = html.GetCleanText();
            }

            IList<WNode> nodes = Tagger.Parse(text);
            if (ChangeToJotai)
            {
                Changer.PreferDearu = (JotaiPreferences & JotaiPreferences.PreferDearu) != 0;
                var buffer = new EditBuffer(nodes);
                Changer.ToJotai(buffer);
                nodes = buffer;
            }
            if (HtmlSyntax)
            {
                html.UpdatedNodes = nodes;
            }
            Combiner.CombineMode = CombineMode;
            switch (WidthPreferences
                & (WidthPreferences.HalfwidthParentheses | WidthPreferences.FullwidthParentheses))
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
            if (WidthPreferences.None != (WidthPreferences
                & (WidthPreferences.FullwidthAlphabets
                 | WidthPreferences.FullwidthDigits
                 | WidthPreferences.FullwidthSymbols
                 | WidthPreferences.CustomFullwidthSet)))
            {
                var p = TokenCombiner.SimpleFullwidthPostprocess(
                    WidthPreferences.HasFlag(WidthPreferences.FullwidthAlphabets) ? TokenCombiner.AsciiAlphabets : null,
                    WidthPreferences.HasFlag(WidthPreferences.FullwidthDigits) ? TokenCombiner.AsciiDigits : null,
                    WidthPreferences.HasFlag(WidthPreferences.FullwidthSymbols) ? TokenCombiner.OtherAsciiSymbols : null,
                    WidthPreferences.HasFlag(WidthPreferences.CustomFullwidthSet) ? CustomFullwidthSet : null);
                var q = Combiner.Postprocess;
                Combiner.Postprocess = (q == null) ? p : sb => p(q(sb));
            }
            if (WidthPreferences.None != (WidthPreferences
                & WidthPreferences.CustomHalfwidthSet))
            {
                var p = TokenCombiner.SimpleHalfwidthPostprocess(
                    WidthPreferences.HasFlag(WidthPreferences.CustomHalfwidthSet) ? CustomHalfwidthSet : null);
                var q = Combiner.Postprocess;
                Combiner.Postprocess = (q == null) ? p : sb => p(q(sb));
            }
            var result = Combiner.Combine(nodes);
            if (HtmlSyntax)
            {
                html.UpdatedText = result;
                result = html.GetUpdatedHtml();
            }
            return result;
        }
    }

    [Flags]
    public enum JotaiPreferences
    {
        None = 0,
        PreferDearu = 1,
    }

    [Flags]
    public enum WidthPreferences
    {
        None = 0,

        HalfwidthParentheses = 1,
        FullwidthParentheses = 2,

        FullwidthAlphabets = 16,
        FullwidthDigits = 64,
        FullwidthSymbols = 256,

        CustomHalfwidthSet = 16384,
        CustomFullwidthSet = 32768,
    }
}
