using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapaneseStyleChanger
{
    /// <summary>
    /// Handles HTML syntax (tags and entities).
    /// </summary>
    /// <remarks>
    /// An instance should be used in the following steps:
    /// <list type="number">
    /// <description>Instanciates.</description>
    /// <description>Sets the original HTML text to <see cref="OriginalHtml"/>.</description>
    /// <description>Calls <see cref="GetCleanText"/> to get a plain text version of <see cref="OriginalHtml"/>.</description>
    /// <description>Parses the clean text into a list of <see cref="WNode"/>.  (An external operation.)</description>
    /// <description>Makes appropriate updates to the list.  (An external operation.)</description>
    /// <description>Sets the updated list of <see cref="WNode"/> to <see cref="UpdatedNodes"/>.</description>
    /// <description>Combine the nodes into a single string.  (An external operation.)</description>
    /// <description>Sets the combined string to <see cref="UpdatedText"/>.</description>
    /// <description>Calls <see cref="GetUpdatedHtml"/> to get an HTML version of the updated nodes/text containings tags from <see cref="OriginalHtml"/>.</description>
    /// </list>
    /// </remarks>
    public class HtmlHandler
    {
        private struct Tag
        {
            /// <summary>Raw tag string.</summary>
            public string TagText;

            /// <summary>Character index in the <em>clean</em> text at which this tag was inserted.</summary>
            public int Pos;

            /// <summary>This tag is an opening or empty tag.</summary>
            public bool IsCloseTag;
        }

        private List<Tag> Tags = new List<Tag>();

        public string OriginalHtml { get; set; }

        static readonly char[] Specials = { '<', '&' };

        public string GetCleanText()
        {
            var html = OriginalHtml;
            if (html.IndexOfAny(Specials) < 0)
            {
                // A shortcut.
                // If the original _html_ text contained no special character,
                // it means it has no tags and its text is already clean.
                return html;
            }

            // We parse the given html text in our own loose way, which behaves
            // differently from the one mandated by the recent HTML standards.
            // The following code may behave incorrectly for unusual HTML inputs.
            // You have been warned.

            var sb = new StringBuilder(html.Length);
            for (int p = 0, q; p < html.Length; p = q)
            {
                // Find the next special character and handle any preceding texts.
                q = html.IndexOfAny(Specials, p);
                if (q < 0)
                {
                    sb.Append(html.Substring(p));
                    // this _preceding_ text covered the whole remaining text.
                    // we can stop looping now.
                    break;
                }
                else if (q > p)
                {
                    sb.Append(html.Substring(p, q - p));
                }

                switch (html[q])
                {
                    case '&':
                        {
                            // Try to decode a character entity reference.
                            string s = null;
                            uint c = default(uint);
                            var r = html.IndexOf(';', q + 1);
                            if (r < q + 3)
                            {
                                // the minimum length of a valid character entity reference is 4 (e.g., &#9; or &lt;),
                                // so this is invalid.
                            }
                            else if (html[q + 1] != '#')
                            {
                                // a named character reference.
                                HtmlEntities.NameToString.TryGetValue(html.Substring(q + 1, r - q - 1), out s);
                            }
                            else if (html[q + 2] != 'x' && html[q + 2] != 'X')
                            {
                                // a decimal character reference.
                                uint.TryParse(html.Substring(q + 2, r - q - 2), NumberStyles.None, CultureInfo.InvariantCulture, out c);
                            }
                            else
                            {
                                // a hexadecimal character reference.
                                uint.TryParse(html.Substring(q + 3, r - q - 3), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out c);
                            }
                            if (s != null)
                            {
                                sb.Append(s);
                                q = r + 1;
                            }
                            else if (c != default(uint))
                            {
                                // Note that we consider a numeric character reference to U+0000 is invalid.
                                // (HTML5 spec prohibits it, too.)
                                sb.AppendScalar(c);
                                q = r + 1;
                            }
                            else
                            {
                                sb.Append('&');
                                q = q + 1;
                            }
                        }
                        break;
                    case '<':
                        {
                            // Grab a tag.
                            var r = html.IndexOf('>', q + 1);
                            if (r < q + 2) // the minimum length of a valid tag is 3, e.g., <i>.
                            {
                                sb.Append('<');
                                q = q + 1;
                            }
                            else
                            {
                                Tags.Add(new Tag
                                {
                                    TagText = html.Substring(q, r - q + 1),
                                    IsCloseTag = html[q + 1] == '/',
                                    Pos = sb.Length,
                                });
                                q = r + 1;
                            }
                        }
                        break;
                    default:
                        throw new ApplicationException();
                }
            }
            return sb.ToString();
        }

        public IList<WNode> UpdatedNodes { get; set; }

        public string UpdatedText { get; set; }

        public string GetUpdatedHtml()
        {
            var tags = Tags; int tindex = 0;
            var text = UpdatedText;

            var sb = new StringBuilder(text.Length + tags.Count * 8);

            int p = 0;
            int qpos, bpos, epos = 0;
            foreach (var node in UpdatedNodes)
            {
                int q = text.IndexOfIgnoreWidth(node.Surface, p);
                if (q < 0)
                {
                    // This case is unexpected,
                    // though we can safely keep going by simply ignoring it.
                    continue;
                }
                if (node.IsOriginalNode())
                {
                    // BPos points to the beginning of any preceding whitespaces
                    // before the token.
                    // EPos *may* point to the next character after this token
                    // regardless it is a whitespace, but it *could* point to
                    // the first non-whitespace character after this token.
                    // I'm not sure under what condition either of the two cases occurs.
                    // So, we just avoid using EPos.
                    // Our epos always points to the next character even if
                    // it is a whitespace.  qpos points to the beginning of a token
                    // excluding any preceding whitespaces.
                    qpos = node.BPos + (node.RLength - node.Length);
                    bpos = node.BPos;
                    epos = node.BPos + node.RLength;
                }
                else
                {
                    // The node u is not from the original node list.
                    // We use the most passable estimation
                    // for the purpose of tag placement in the case.
                    qpos = epos;
                    bpos = epos;
                    // epos = epos;
                }

                int r = p;
                while (tindex < tags.Count && tags[tindex].Pos <= qpos && tags[tindex].IsCloseTag)
                {
                    int original_preceding_padding = tags[tindex].Pos - bpos;
                    if (original_preceding_padding > r - p)
                    {
                        sb.AppendEscaped(text.Substring(r, Math.Min(q - r, original_preceding_padding)));
                        r += Math.Min(q - r, original_preceding_padding);
                    }
                    sb.Append(tags[tindex].TagText);
                    tindex++;
                }
                //sb.AppendEscaped(utext.Substring(p, q - p));
                //p = q;
                while (tindex < tags.Count && tags[tindex].Pos <= qpos)
                {
                    int original_succeeding_padding = qpos - tags[tindex].Pos;
                    if (original_succeeding_padding < q - r)
                    {
                        sb.AppendEscaped(text.Substring(r, q - r - original_succeeding_padding));
                        r = q - original_succeeding_padding;
                    }
                    sb.Append(tags[tindex].TagText);
                    tindex++;
                }
                if (r < q)
                {
                    sb.AppendEscaped(text.Substring(r, q - r));
                }
                sb.AppendEscaped(text.Substring(q, node.Length));
                p = q + node.Length;
            }
            {
                int q = text.Length;
                int r = p;
                while (tindex < tags.Count)
                {
                    int original_preceding_padding = tags[tindex].Pos - epos;
                    if (original_preceding_padding > r - p)
                    {
                        sb.AppendEscaped(text.Substring(r, Math.Min(q - r, original_preceding_padding)));
                        r += Math.Min(q - r, original_preceding_padding);
                    }
                    sb.Append(tags[tindex].TagText);
                    tindex++;
                }
                sb.AppendEscaped(text.Substring(r));
            }

            return sb.ToString();
        }
    }

    public static class HtmlHandlerHelperMethods
    {
        public static void AppendEscaped(this StringBuilder sb, string text)
        {
            int p = 0;
            for (; ; )
            {
                int q = text.IndexOfAny(HtmlEntities.EscapeChars, p);
                if (q < 0) break;
                sb.Append(text.Substring(p, q - p));
                sb.Append(HtmlEntities.CharToRef[text[q]]);
                p = q + 1;
            }
            sb.Append(text.Substring(p));
        }

        /// <summary>
        /// Appends a Unicode character represented in a scalar value to <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="StringBuilder"/> instance.</param>
        /// <param name="scalar">The Unicode scalar value of a character to be appended.</param>
        public static void AppendScalar(this StringBuilder sb, uint scalar)
        {
            if (scalar <= char.MaxValue)
            {
                sb.Append((char)scalar);
            }
            else
            {
                sb.Append((char)(((scalar - char.MaxValue - 1) >> 10) + 0xD800));
                sb.Append((char)(((scalar - char.MaxValue - 1) & 1023) + 0xDC00));
            }
        }

        public static int IndexOfIgnoreWidth(this string s, string text, int index)
        {
            const int FullwidthShift = 0xFEE0;
            int max = s.Length - text.Length + 1;
            for (int p = index; p < max; p++)
            {
                for (int t = 0; t < text.Length; t++)
                {
                    var c = s[p + t];
                    var d = text[t];
                    if (c == d) continue;
                    if (c > '\u0020' && c < '\u007F' && c + FullwidthShift == d) continue;
                    if (d > '\u0020' && d < '\u007F' && d + FullwidthShift == c) continue;
                    if (c == '\u0020' && d == '\u3000') continue;
                    if (d == '\u0020' && c == '\u3000') continue;
                    goto NEXT;
                }
                return p;
            NEXT:;
            }
            return -1;
        }

        public static int IndexOf(this IList<WNode> list, WNode node, int index)
        {
            for (int i = index; i < list.Count; i++)
            {
                if (ReferenceEquals(node, list[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// Checks if a node is from the original list returned by Tagger.Parse method.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsOriginalNode(this WNode node)
        {
            return node.EPos > 0;
        }
    }
}
