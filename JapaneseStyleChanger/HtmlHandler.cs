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
                                // this case also covers no semicolon was found.
                            }
                            else if (html[q + 1] != '#')
                            {
                                // a named character reference.
                                HtmlCharacterReferences.NameToString.TryGetValue(html.Substring(q + 1, r - q - 1), out s);
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
                            if (r < q + 2)
                            {
                                // the minimum length of a valid tag is 3, e.g., <i>,
                                // so this is invalid.
                                // this case also covers no '>' was found.
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
            var tags = Tags;
            int tindex = 0;

            var text = UpdatedText;

            var sb = new StringBuilder(text.Length + tags.Count * 8);

            // By the following loop, we merge subtexts in text (= UpdatedText)
            // and tags in tags (= Tags) appropriately, considering the positions of
            // each tag in the original html text AND the correspondence between the
            // text and original html via nodes in UpdatedNodes.  The result will be
            // in sb.

            // To do so, we examine node in UpdatedNodes one by one in the following
            // loop.

            int p = 0;
            int qpos, bpos, epos = 0;
            foreach (var node in UpdatedNodes)
            {
                int q = text.IndexOfIgnoreWidth(node.Surface, p);
                if (q < 0)
                {
                    // This case is unexpected, but if it happened,
                    // we just ignore the node to keep going.
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
                    // The node was not from the original node list.
                    // We use the most passable estimation
                    // for the purpose of tag placement in the case.
                    qpos = epos;
                    bpos = epos;
                    // epos = epos;
                }

                // At this moment, we assume that the subtexts in text that correspond
                // to any node before the current node have been put in sb, being merged with 
                // tags[0] to tags[tindex - 1] already.
                // We are about to put the subtext that corresponds to the current node
                // into sb.
                // If there are any tags that were before the node in the original
                // HTML text (i.e., Tag.Pos < bpos), put them in sb before the text that
                // corresponds to the current node.
                // IF there were some whitespaces between the previous node and the current node,
                // place each of those tags in an appropriate position in the whitespace block.
                // We use two values original_preceding_padding and original_succeeding_padding
                // to find the best positions of tags; the original_preceding_padding is the number
                // of whitespace characters after the previous node and before the tag,
                // and the original_succeeding_padding is the number of whitespace characters 
                // after the tag and before the current node, both counted in the original HTML text.
                // Remember, bpos points to the beginning of such whitespace block and qpos points
                // to the end of it.
                // We try to more respect original_preceding_padding than original_succeeding_padding
                // if it is a close tag, so that the extra spaces added to the updated text
                // go outside of elements as much as possible.
                // We assume texts between text[p] and text[q - 1], inclusive, are all whitespace
                // (or anything that are not important), and distribute them in between 
                // the tags appropriately.
                // the variable r points in a middle of p and q, where text[p] to text[r - 1]
                // are already put into sb, and text[r] to text[q - 1] are not (i.e., ready for
                // further consumption.)

                int r = p;
                while (tindex < tags.Count && tags[tindex].Pos <= qpos && tags[tindex].IsCloseTag)
                {
                    int original_preceding_padding = tags[tindex].Pos - bpos;
                    if (original_preceding_padding > r - p)
                    {
                        var n = Math.Min(q - r, original_preceding_padding);
                        sb.AppendEscaped(text.Substring(r, n));
                        r += n;
                    }
                    sb.Append(tags[tindex].TagText);
                    tindex++;
                }
                while (tindex < tags.Count && tags[tindex].Pos <= qpos)
                {
                    int original_succeeding_padding = qpos - tags[tindex].Pos;
                    if (original_succeeding_padding < q - r)
                    {
                        var n = q - r - original_succeeding_padding;
                        sb.AppendEscaped(text.Substring(r, n));
                        r += n;
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

            // handle any remaining tags and texts.
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

    /// <summary>
    /// Provides some extension methods to be used by the internals of HtmlHandler.
    /// </summary>
    /// <remarks>
    /// This class is marked public only because the C# language grammar requires it.
    /// </remarks>
    public static class HtmlHandlerHelperMethods
    {
        /// <summary>
        /// Appends a text, after escaping characters in it as HTML character references where needed.
        /// </summary>
        /// <param name="sb">A <see cref="StringBuilder"/> instance.</param>
        /// <param name="text">The (unescaped) string to be appended.</param>
        public static void AppendEscaped(this StringBuilder sb, string text)
        {
            int p = 0;
            for (; ; )
            {
                int q = text.IndexOfAny(HtmlCharacterReferences.CharsToBeEscaped, p);
                if (q < 0) break;
                sb.Append(text.Substring(p, q - p));
                sb.Append(HtmlCharacterReferences.CharToRef[text[q]]);
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

        /// <summary>
        /// Reports the index of the specified string,
        /// starting the search at the specified index, 
        /// and matching the (sub)strings in a width-insensitve mannar.
        /// </summary>
        /// <param name="s">The string to search <paramref name="text"/> in.</param>
        /// <param name="text">The string to find.</param>
        /// <param name="index">The index in <paramref name="s"/> to start searching at.</param>
        /// <returns>
        /// The first index in the whole <paramref name="s"/> after <paramref name="index"/> where <paramref name="text"/> is at,
        /// or a negative value if no matching string was found after <paramref name="index"/>.
        /// </returns>
        /// <remarks>
        /// For the purpose of this function,
        /// <i>width-insensive</i> means that an ASCII character (i.e., U+0021 thru U+007E)
        /// is considered equivalent to its FULLWIDTH version (i.e., those in U+FF01 thru U+FF5E)
        /// as well as ASCII SPACE (U+0020) is to IDEOGRAPHIC SPACE (U+3000) (aka fullwidth space).
        /// We never consider other special equivalence, 
        /// especially for other FULLWIDTH and HALFWIDTH forms in U+FF5E thru U+FFEE. 
        /// </remarks>
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
            // Because EPos is a base-zero index, and there is no
            // token with length zero, MeCab tokenizer never assigns a 0 to EPos,
            // while JapaneseStyleChanger.Conjugator never assigns a value to it,
            // leaving its default value, i.e., 0.
            return node.EPos > 0;
        }
    }
}
