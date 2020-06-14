using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseStyleChanger
{
    /// <summary>
    /// How spaces are added and/or deleted from the original text.
    /// </summary>
    public enum CombineMode
    {
        /// <summary>Follow the original spacing as much as possible.</summary>
        /// <remarks>This is the default.</remarks>
        Default = 0,

        /// <summary>The way insisted by a company that dominated PC software market in '90s.</summary>
        MS = 1,

        /// <summary>The way adopted by most of genuine Japanese console games in '90s.</summary>
        CG = 2,
    }

    public class TokenCombiner<TToken>
    {
        private readonly Func<TToken, string> Renderer;

        private readonly Func<TToken, bool> Spacer;

        public TokenCombiner(Func<TToken, string> renderer = null, Func<TToken, bool> spacer = null)
        {
            Renderer = renderer ?? (token => token.ToString());
            Spacer = spacer;
        }

        public CombineMode CombineMode { get; set; }

        public string Space { get; set; } = " ";

        //public string Combine(IList<TToken> tokens)
        //{
        //    var sb = new StringBuilder();
        //    for (int i = 0; i < tokens.Count; i++)
        //    {
        //        // We need to consider spaces between/inside _words_ written in alphabets. FIXME.
        //        sb.Append(R(tokens[i]));
        //    }
        //    return sb.ToString();
        //}

        private enum TokenType
        {
            /// <summary>an empty token that works like ZWJ.</summary>
            /// <remarks>This is the default value.</remarks>
            NN = 0,

            /// <summary>Japanese full-width characters, e.g., "あ"</summary>
            ZZ,

            /// <summary>Katakana (both normal and half-width)</summary>
            ZK,

            /// <summary>full-width symbols that contain a space on the left and never require a space to the right, e.g., "（".</summary>
            ZL,

            /// <summary>full-width symbols that contain a space on the right and never require a space to the left, e.g., "）".</summary>
            ZR,

            /// <summary>half-width letters, e.g., "A".</summary>
            HH,

            /// <summary>half-width digits, e.g., "9".</summary>
            H9,

            /// <summary>half-width symbols that usually require a space to the left and never to the right, e.g., "(".</summary>
            SL,

            /// <summary>half-width symbols that usually require a space to the right and never to the left, e.g., ")".</summary>
            SR,
        }

        private static readonly Dictionary<char, TokenType> TokenDictionary;

        static TokenCombiner()
        {
            var d = new Dictionary<char, TokenType>();
            for (var ch = '\u30A0'; ch <= '\u30FA'; ch++) d.Add(ch, TokenType.ZK);
            for (var ch = '\u30FC'; ch <= '\u30FE'; ch++) d.Add(ch, TokenType.ZK);
            for (var ch = '\u31F0'; ch <= '\u31FF'; ch++) d.Add(ch, TokenType.ZK);
            for (var ch = '\uFF66'; ch <= '\uFF9F'; ch++) d.Add(ch, TokenType.ZK);
            foreach (var ch in "‘“（〔［｛〈《「『【") d.Add(ch, TokenType.ZL);
            foreach (var ch in "’”）〕］｝〉》」』】、。，．°′″") d.Add(ch, TokenType.ZR);
            for (var ch = '0'; ch <= '9'; ch++) d.Add(ch, TokenType.H9);
            foreach (var ch in "([{") d.Add(ch, TokenType.SL);
            foreach (var ch in ")]},.!?") d.Add(ch, TokenType.SR);
            TokenDictionary = d;
        }

        private enum Spacing : byte
        {
            DD = 0,
            SS,
            __,
        }

        private const Spacing __ = Spacing.__;

        private const Spacing SS = Spacing.SS;

        private const Spacing DD = Spacing.DD;

        private static readonly Spacing[,] SpacingTableDefault = new Spacing[9, 9]; // i.e., all DD.

        private static readonly Spacing[,] SpaceTableMS =
        {   //         NN  ZZ  ZK  ZL  ZR  HH  H9  SL  SR
            /* NN */ { __, __, __, __, __, __, __, __, __ },
            /* ZZ */ { __, __, __, __, __, SS, SS, SS, __ },
            /* ZK */ { __, __, SS, __, __, SS, SS, SS, __ },
            /* ZL */ { __, __, __, __, __, __, __, __, __ },
            /* ZR */ { __, __, __, __, __, __, __, __, __ },
            /* HH */ { __, SS, SS, __, __, DD, DD, DD, __ },
            /* H9 */ { __, SS, SS, __, __, DD, DD, DD, __ },
            /* SL */ { __, __, __, __, __, __, __, __, __ },
            /* SR */ { __, SS, SS, __, __, DD, DD, DD, __ },
        };

        private static readonly Spacing[,] SpaceTableCG =
        {   //         NN  ZZ  ZK  ZL  ZR  HH  H9  SL  SR
            /* NN */ { __, __, __, __, __, __, __, __, __ },
            /* ZZ */ { __, __, __, __, __, __, __, SS, __ },
            /* ZK */ { __, __, __, __, __, __, __, SS, __ },
            /* ZL */ { __, __, __, __, __, __, __, __, __ },
            /* ZR */ { __, __, __, __, __, __, __, __, __ },
            /* HH */ { __, __, __, __, __, DD, DD, DD, __ },
            /* H9 */ { __, __, __, __, __, DD, DD, DD, __ },
            /* SL */ { __, __, __, __, __, __, __, __, __ },
            /* SR */ { __, SS, SS, __, __, DD, DD, DD, __ },
        };

        private static TokenType GetTokenTypeLeft(string token_text)
        {
            if (token_text is null) throw new ArgumentNullException(nameof(token_text));
            if (token_text.Length == 0) return TokenType.NN;
            return GetTokenType(token_text[0]);
        }

        private static TokenType GetTokenTypeRight(string token_text)
        {
            if (token_text is null) throw new ArgumentNullException(nameof(token_text));
            if (token_text.Length == 0) return TokenType.NN;
            return GetTokenType(token_text[token_text.Length - 1]);
        }

        /// <summary>
        /// Returns a <see cref="TokenType"/> for a character, assuming it solely comprises a token. 
        /// </summary>
        /// <param name="ch"></param>
        /// <returns>The <see cref="TokenType"/>.</returns>
        /// <remarks>The current implementaiton is just a quick hack.  We need to revise it soon.  XXX</remarks>
        private static TokenType GetTokenType(char ch)
        {
            if (TokenDictionary.TryGetValue(ch, out var type)) return type;
            if (ch < 0x2000) return TokenType.HH;
            if (ch < 0x2010) return TokenType.NN;
            return TokenType.ZZ;
        }

        public string Combine(IEnumerable<TToken> tokens)
        {
            Spacing[,] table;
            switch (CombineMode)
            {
                case CombineMode.Default:
                    table = SpacingTableDefault;
                    break;
                case CombineMode.MS:
                    table = SpaceTableMS;
                    break;
                case CombineMode.CG:
                    table = SpaceTableCG;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var sb = new StringBuilder();
            TokenType prev_type = TokenType.NN;
            foreach (var token in tokens)
            {
                var s = Renderer(token);
                var t = GetTokenTypeLeft(s);
                switch (table[(int)prev_type, (int)t])
                {
                    case Spacing.__:
                        break;
                    case Spacing.SS:
                        sb.Append(Space);
                        break;
                    case Spacing.DD:
                        if (Spacer?.Invoke(token) == true) sb.Append(Space);
                        break;
                    default:
                        throw new ApplicationException("Internal error");
                }
                sb.Append(s);
                prev_type = GetTokenTypeRight(s);
            }
            return sb.ToString();
        }
    }
}
