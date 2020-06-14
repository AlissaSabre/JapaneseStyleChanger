﻿using System;
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
        /// <summary>
        /// To follow the original spacing as much as possible.
        /// </summary>
        /// <remarks>
        /// This is the default value.
        /// </remarks>
        Default = 0,

        /// <summary>
        /// To try to conform to the way insisted by a company that dominated PC software market in '90s.
        /// </summary>
        /// <remarks>
        /// With this mode, spaces are inserted
        /// between a half-width (ASCII) letter and a full-width letter,
        /// before and after a number, and
        /// between two katakana words,
        /// in general.
        /// (There is a more complicated set of rules.)
        /// </remarks>
        MS = 1,

        /// <summary>
        /// Try to follow the traditional way of writing Japanese texts.
        /// </summary>
        /// <remarks>
        /// Less spaces are inserted in this mode than in <see cref="MS"/> mode.
        /// </remarks>
        CG = 2,
    }

    public class TokenCombiner<TToken>
    {
        private readonly Func<TToken, string> Renderer;

        private readonly Func<TToken, bool> Spacer;

        public TokenCombiner(Func<TToken, string> renderer = null, Func<TToken, bool> spacer = null)
        {
            Renderer = renderer;
            Spacer = spacer;
        }

        public CombineMode CombineMode { get; set; }

        public string Space { get; set; } = " ";

        public string Combine(IEnumerable<TToken> tokens)
        {
            return TokenCombiner.Combine(tokens, CombineMode, Space, Renderer, Spacer);
        }
    }

    public static class TokenCombiner
    {
        #region Token types

        /// <summary>
        /// Type of a token for the purpose of judging presence/absence of a space between two tokens.
        /// </summary>
        /// <remarks>
        /// Although it is called <i>token</i> type, each token type is based on a single character.
        /// You need to use <see cref="GetTokenTypeLeft(string)"/> and <see cref="GetTokenTypeRight(string)"/> appropriately
        /// to get an appropriate type for a token.
        /// </remarks>
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

        /// <summary>
        /// A partial mapping of a character to a token type.
        /// </summary>
        private static readonly Dictionary<char, TokenType> TokenDictionary;

        /// <summary>
        /// This static constructor fills <see cref="TokenDictionary"/>.
        /// </summary>
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

        /// <summary>
        /// Gets a token type of the leftmost character of a token text.
        /// </summary>
        /// <param name="token_text">Text that a token represents.</param>
        /// <returns>
        /// Token type appropriate for considering a placement of another token
        /// to the left of <paramref name="token_text"/>.
        /// </returns>
        private static TokenType GetTokenTypeLeft(string token_text)
        {
            if (token_text is null) throw new ArgumentNullException(nameof(token_text));
            if (token_text.Length == 0) return TokenType.NN;
            return GetTokenType(token_text[0]);
        }

        /// <summary>
        /// Gets a token type of the rightmost character of a token text.
        /// </summary>
        /// <param name="token_text">Text that a token represents.</param>
        /// <returns>
        /// Token type appropriate for considerting a placement of another token
        /// to the right of <paramref name="token_text"/>.
        /// </returns>
        private static TokenType GetTokenTypeRight(string token_text)
        {
            if (token_text is null) throw new ArgumentNullException(nameof(token_text));
            if (token_text.Length == 0) return TokenType.NN;
            return GetTokenType(token_text[token_text.Length - 1]);
        }

        /// <summary>
        /// Returns a <see cref="TokenType"/> for a character. 
        /// </summary>
        /// <param name="ch">A character.</param>
        /// <returns>The <see cref="TokenType"/> for <paramref name="ch"/>.</returns>
        /// <remarks>The current implementaiton is just a quick hack.  We need to revise it soon.  XXX</remarks>
        private static TokenType GetTokenType(char ch)
        {
            if (TokenDictionary.TryGetValue(ch, out var type)) return type;
            if (ch < 0x2000) return TokenType.HH;
            if (ch < 0x2010) return TokenType.NN;
            return TokenType.ZZ;
        }

        #endregion

        #region Spacing tables

        /// <summary>
        /// Value of spacing tables.
        /// </summary>
        /// <remarks>
        /// A spacing table is a two-dimensional array to help deciding
        /// whether a space should be inserted between two tokens.
        /// Its indexes for both dimensions are (integral values of) <see cref="TokenType"/> enumeration.
        /// s[x, y] gives a hint of spacing when a token X is followed by Y,
        /// where x == GetTokenTypeRight(X) and y == GetTokenTypeLeft(Y).
        /// </remarks>
        private enum Spacing : byte
        {
            /// <summary>A spacing should follow Spacer delegate.</summary>
            /// <remarks>This is the default value.</remarks>
            DD = 0,

            /// <summary>A space should be present.</summary>
            SS,

            /// <summary>No space should be present.</summary>
            __,
        }

        private const Spacing __ = Spacing.__;

        private const Spacing SS = Spacing.SS;

        private const Spacing DD = Spacing.DD;

        /// <summary>
        /// The default spacing table.
        /// </summary>
        /// <remarks>
        /// This table indicates to always follow Spacer delegate to reproduce original spacing.
        /// </remarks>
        private static readonly Spacing[,] SpacingTableDefault = new Spacing[9, 9];

        /// <summary>
        /// Spacing table for MS combining mode.
        /// </summary>
        private static readonly Spacing[,] SpacingTableMS =
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

        /// <summary>Spacing table for CG combining mode.</summary>
        private static readonly Spacing[,] SpacingTableCG =
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

        #endregion

        public static string Combine<TToken>(IEnumerable<TToken> tokens,
            CombineMode mode = CombineMode.Default,
            string space = " ",
            Func<TToken, string> renderer = null,
            Func<TToken, bool> spacer = null,
            Func<StringBuilder, StringBuilder> postprocess = null)
        {
            Spacing[,] table;
            switch (mode)
            {
                case CombineMode.Default:
                    table = SpacingTableDefault;
                    break;
                case CombineMode.MS:
                    table = SpacingTableMS;
                    break;
                case CombineMode.CG:
                    table = SpacingTableCG;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (renderer == null) renderer = token => token.ToString();

            var sb = new StringBuilder();
            TokenType prev_type = TokenType.NN;
            foreach (var token in tokens)
            {
                var s = renderer(token);
                var t = GetTokenTypeLeft(s);
                switch (table[(int)prev_type, (int)t])
                {
                    case Spacing.__:
                        break;
                    case Spacing.SS:
                        sb.Append(space);
                        break;
                    case Spacing.DD:
                        if (spacer?.Invoke(token) == true) sb.Append(space);
                        break;
                    default:
                        throw new ApplicationException("Internal error");
                }
                sb.Append(s);
                prev_type = GetTokenTypeRight(s);
            }
            if (postprocess != null) sb = postprocess(sb);
            return sb.ToString();
        }

        public static StringBuilder AsciiParentheses(StringBuilder sb)
        {
            throw new NotImplementedException();
        }

        public static StringBuilder FullWidthParentheses(StringBuilder sb)
        {
            throw new NotImplementedException();
        }
    }
}
