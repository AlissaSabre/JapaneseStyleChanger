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
    /// <description>Sets the parsed list of <see cref="WNode"/> to <see cref="CleanNodes"/>.</description>
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

            /// <summary>Character index in the clean text at which this tag was inserted.</summary>
            public int Pos;

            /// <summary>This tag is an opening or empty tag.</summary>
            public bool IsOpen;
        }

        private List<Tag> Tags = new List<Tag>();

        public string OriginalHtml { get; set; }

        static readonly char[] Specials = { '<', '&' };

        public string GetCleanText()
        {
            var s = OriginalHtml;
            if (s.IndexOfAny(Specials) < 0) return s;

            // We parse the given html text in our own loose way, which behaves
            // differently from the one mandated by the recent HTML standards.
            // You have been warned.

            var sb = new StringBuilder(s.Length);
            for (int p = 0, q; p >= 0 && p < s.Length; p = q)
            {
                q = s.IndexOfAny(Specials, p);
                if (q < 0)
                {
                    sb.Append(s.Substring(p));
                }
                else
                {
                    sb.Append(s.Substring(p, q - p));
                }
                if (q < 0)
                {

                }
                else if (s[q] == '&')
                {
                    // Decode a character entity reference.
                    var r = s.IndexOf(';', q + 1);
                    if (r < q + 3) // the minimum length of a valid character entity reference is 4 (e.g., &#9; or &lt;).
                    {
                        sb.Append('&');
                        q = q + 1;
                    }
                    else if (s[q + 1] != '#')
                    {
                        if (HtmlEntities.NameToString.TryGetValue(s.Substring(q + 1, r - q - 1), out var t))
                        {
                            sb.Append(t);
                            q = r + 1;
                        }
                        else
                        {
                            sb.Append('&');
                            q = q + 1;
                        }
                    }
                    else if (s[q + 2] != 'x' && s[q + 2] != 'X')
                    {
                        if (uint.TryParse(s.Substring(q + 2, r - q - 2), NumberStyles.None, CultureInfo.InvariantCulture, out var c))
                        {
                            if (c <= char.MaxValue)
                            {
                                sb.Append((char)c);
                            }
                            else
                            {
                                sb.Append((char)(((c - char.MaxValue) >> 10)  + 0xD800));
                                sb.Append((char)(((c - char.MaxValue) & 1023) + 0xDC00));
                            }
                            q = r + 1;
                        }
                        else
                        {
                            sb.Append('&');
                            q = q + 1;
                        }
                    }
                    else
                    {
                        if (uint.TryParse(s.Substring(q + 3, r - q - 3), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var c))
                        {
                            if (c <= char.MaxValue)
                            {
                                sb.Append((char)c);
                            }
                            else
                            {
                                sb.Append((char)(((c - char.MaxValue) >> 10) + 0xD800));
                                sb.Append((char)(((c - char.MaxValue) & 1023) + 0xDC00));
                            }
                            q = r + 1;
                        }
                        else
                        {
                            sb.Append('&');
                            q = q + 1;
                        }
                    }
                }
                else if (s[q] == '<')
                {
                    // Grab a tag.
                    var r = s.IndexOf('>', q + 1);
                    if (r < q + 2) // the minimum length of a valid tag is 3, e.g., <i>.
                    {
                        sb.Append('<');
                        q = q + 1;
                    }
                    else
                    {
                        Tags.Add(new Tag
                        {
                            TagText = s.Substring(q, r - q + 1),
                            IsOpen = s[q + 1] != '/',
                            Pos = sb.Length,
                        });
                        q = r + 1;
                    }
                }
                else
                {
                    throw new ApplicationException();
                }
            }
            return sb.ToString();
        }

        public IList<WNode> CleanNodes { get; set; }

        public IList<WNode> UpdatedNodes { get; set; }

        public string UpdatedText { get; set; }

        public string GetUpdatedHtml()
        {
            // I'm sorry, it is difficult to explain what the following code does... :(
            // or, more precisely, the following code DOES insert tags
            // separated from OriginalHtml into UpdatedText
            // (as well as escaping some characters) to rebuild the
            // corresponding HTML text.  What is difficult is how we decide the position
            // in UpdatedText to insert each tag.

            var tags = Tags; int tindex = 0;
            var cnodes = CleanNodes; int cindex = 0;
            var unodes = UpdatedNodes;
            var utext = UpdatedText;

            var sb = new StringBuilder(utext.Length + tags.Count * 8);

            int p = 0;
            foreach (var u in unodes)
            {
                int d = cnodes.IndexOf(u, cindex);
                if (d < 0) continue;
                int q = utext.IndexOfIgnoreWidth(u.Surface, p);
                if (q < 0) continue;
                int pos = u.BPos;
                while (tindex < tags.Count && tags[tindex].Pos <= pos)
                {
                    sb.Append(tags[tindex].TagText);
                    tindex++;
                }
                sb.AppendEscaped(utext.Substring(p, q - p));
                sb.AppendEscaped(utext.Substring(q, u.Surface.Length));
                p = q + u.Surface.Length;
            }
            while (tindex < tags.Count)
            {
                sb.Append(tags[tindex++].TagText);
            }
            sb.AppendEscaped(utext.Substring(p));

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
    }
}
