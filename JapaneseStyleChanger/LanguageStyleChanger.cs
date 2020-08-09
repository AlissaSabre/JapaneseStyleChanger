using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NMeCab;
using NMeCab.Alissa;

namespace JapaneseStyleChanger
{
    public class LanguageStyleChanger
    {
        public bool PreferDearu;

        private readonly Conjugator Conjugator;

        public LanguageStyleChanger(Tagger<WNode> tagger)
        {
            Conjugator = new Conjugator(tagger);
        }

        private static readonly WNode DummyNode_21642 = new WNode()
        {
            Feature = "助動詞,*,*,*,助動詞-タ,終止形,,,だ,,だ,,,,,,,,,,,,,,,,,0,21642"
        };

        private static readonly WNode DummyNode_22916 = new WNode()
        {
            Feature = "助動詞,*,*,*,助動詞-ダ,終止形,,,だ,,だ,,,,,,,,,,,,,,,,,0,22916"
        };

        private static readonly WNode DummyNode_1216 = new WNode()
        {
            Feature = "動詞,非自立可能,*,*,五段-ラ行,終止形,,,ある,,ある,,,,,,,,,,,,,,,,,0,1216"
        };

        private static readonly WNode DummyNode_27442 = new WNode()
        {
            Feature = "形容詞,非自立可能,*,*,形容詞,終止形,,,ない,,ない,,,,,,,,,,,,,,,,,0,27442"
        };

        /// <summary>List of Lemma IDs of ending particles (終助詞) that don't need だ when rewriting a preceding です.</summary>
        /// <remarks></remarks>
        private static readonly int[] NoDaEndingParticles =
        {
            5569,   // か
            6446,   // かしら
            7175,   // かも
            13520,  // さ
        };

        public void ToJotai(EditBuffer buffer)
        {
            int p = 0;
            while (p < buffer.Count)
            {
                buffer.Changed = false;
                var current = buffer[p];
                switch (current.Lemma_id)
                {
                    case 25653: // 助動詞「です」
                        {
                            // Replace this です with だ (or である depending on the option
                            // and if it apparently terminates a sentence).
                            // OR, if the です follows a 形容詞 or is followed by some particular 終助詞,
                            // just remove です, regardless of the である preference, unless it is in 意思推量形.
                            IList<WNode>[] possibilities;
                            if (current.CForm != "意志推量形"
                                && (current.Prev.Pos1 == "動詞" ||
                                    current.Prev.Pos1 == "形容詞" || 
                                    current.Prev.Pos1 == "助動詞" ||
                                    NoDaEndingParticles.Contains(current.Next.Lemma_id) ||
                                    current.Next.Lemma_id == 57 /* question mark */))
                            {
                                // The case to remove です.
                                var prev_conjugations = Conjugator.ConjugateLoosely(current.Prev, current.CForm);
                                if (prev_conjugations == null)
                                {
                                    // The preceding word to です was a non-conjugating word.
                                    // We simply remove the です without changing any preceding/following words.
                                }
                                else
                                {
                                    // The preceding word to です was a conjugating word.
                                    // We should find a best conjugation.
                                    possibilities = new IList<WNode>[2]
                                    {
                                        prev_conjugations,
                                        new[] { current.Next },
                                    };
                                    var best_conjugations = Conjugator.ChooseBest(possibilities);
                                    buffer[p - 1] = best_conjugations[0];
                                }
                                buffer.RemoveAt(p);
                            }
                            else if (PreferDearu
                                && current.CForm.StartsWith("終止形")
                                && (current.Next.IsEos || current.Next.Pos1 == "補助記号"))
                            {
                                // The case to change です to である.
                                possibilities = new IList<WNode>[4]
                                {
                                    new[] { current.Prev },
                                    Conjugator.ConjugateLoosely(DummyNode_22916, "連用形"),
                                    Conjugator.ConjugateLoosely(DummyNode_1216, current.CForm),
                                    new[] { current.Next },
                                };
                                var best_conjugations = Conjugator.ChooseBest(possibilities);
                                buffer[p] = best_conjugations[1];
                                buffer.Insert(p + 1, best_conjugations[2]);
                            }
                            else
                            {
                                // The case to change です to だ.
                                possibilities = new IList<WNode>[3]
                                {
                                    new[] { current.Prev },
                                    Conjugator.ConjugateLoosely(DummyNode_22916, current.CForm),
                                    new[] { current.Next },
                                };
                                var best_conjugations = Conjugator.ChooseBest(possibilities);
                                buffer[p] = best_conjugations[1];
                            }
                        }
                        break;
                    case 35697: // 助動詞「ます」
                        {
                            // Remove this ます, conjugating the previous 用言 into a same conjugation form.
                            var conjugations = Conjugator.ConjugateLoosely(current.Prev, current.CForm);
                            if (conjugations is null)
                            {
                                // We can do nothing.
                            }
                            else
                            {
                                var conjugations2 = Conjugator.ConjugateLoosely(current.Next, current.Next.CForm);
                                if (conjugations2 is null)
                                {
                                    // ます was at the end of a sentence or before a non-conjugating word.
                                    // Let the preceeding word conjugate alone.
                                    var possibilities = new IList<WNode>[3]
                                    {
                                        new[] { buffer[p - 2] },
                                        conjugations,
                                        new[] { current.Next },
                                    };
                                    var best_conjugations = Conjugator.ChooseBest(possibilities);
                                    buffer[p - 1] = best_conjugations[1];
                                }
                                else
                                {
                                    // Try to find the best conjugation along with the word following ます.
                                    // We handle several special cases manually.
                                    if (current.Next.Lemma_id == 21642) // 助動詞「た/だ」
                                    {
                                        // consider both た forms and だ forms.
                                        conjugations2 = conjugations2.Concat(Conjugator.ConjugateLoosely(DummyNode_21642, current.Next.CForm)).ToList();
                                    }
                                    else if (current.Next.Lemma_id == 19587) // 助動詞「ず」
                                    {
                                        // substitute ません with ない
                                        conjugations2 = Conjugator.ConjugateLoosely(DummyNode_27442, current.Next.CForm);
                                    }
                                    var possibilities = new IList<WNode>[4]
                                    {
                                        new[] { buffer[p - 2] },
                                        conjugations,
                                        conjugations2,
                                        new[] { current.Next.Next },
                                    };
                                    var best_conjugations = Conjugator.ChooseBest(possibilities);
                                    buffer[p - 1] = best_conjugations[1];
                                    buffer[p + 1] = best_conjugations[2];
                                }
                            }
                            buffer.RemoveAt(p);
                            --p;
                        }
                        break;
                    case 1216: // 動詞「ある」
                        {
                            // If ある is followed by ない, just remove ある.
                            // the case may be produced by a rewriting of ません to ない.
                            if (current.Next.Lemma_id == 27442)
                            {
                                buffer.RemoveAt(p);
                            }
                        }
                        break;
#if false
                    case 22916: // 助動詞「だ」
                        {
                            // If だ is placed after a 形容詞 (or a 助動詞 with 形容詞 conjugation type)
                            // remove this だ.
                            // Also remove だ if it is placed after 助動詞「た」
                            // If such a だ was followed by ある, remove that ある, too.
                            // The cases are often produced by rewriting of です to だ or である.
                            // (Note that we are talking about 助動詞「だ」 but だ form of 助動詞「た」.)
                            // 
                            // However, we don't remove だ if it is in 意志推量形, because, for example,
                            // 青いだろう is preferred over 青かろう or 見ただろう is over 見たろう these days (IMHO)
                            if (current.CForm != "意志推量形"
                                && (current.Prev.Pos1 == "形容詞"
                                    || current.Prev.Pos1 == "助動詞" && current.Prev.Lemma.EndsWith("い")
                                    || current.Prev.Lemma_id == 21642))
                            {
                                var conjugations = Conjugator.ConjugateLoosely(current.Prev, current.CForm);
                                if (!(conjugations is null))
                                {
                                    var possibilities = new IList<WNode>[3]
                                    {
                                        new[] { current.Prev.Prev },
                                        conjugations,
                                        new[] { current.Next },
                                    };
                                    var best_conjugations = Conjugator.ChooseBest(possibilities);
                                    buffer[p - 1] = best_conjugations[1];
                                    buffer.RemoveAt(p);
                                }
                            }
                        }
                        break;
#endif
                    default:
                        break;
                }

                if (!buffer.Changed)
                {
                    ++p;
                }
            }
        }
    }
}
