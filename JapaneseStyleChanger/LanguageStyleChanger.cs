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

        private static readonly WNode DummyNode_21642_da = new WNode()
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

        private static readonly WNode DummyNode_27438 = new WNode()
        {
            Feature = "助動詞,*,*,*,助動詞-ナイ,終止形,,,ない,,ない,,,,,,,,,,,,,,,,,0,27438"
        };

        private static readonly WNode DummyNode_27442 = new WNode()
        {
            Feature = "形容詞,非自立可能,*,*,形容詞,終止形-一般,,,ない,,ない,,,,,,,,,,,,,,,,,0,27442"
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

        /// <summary>List of cTypes that vocalize some particular words that follow.</summary>
        private static readonly string[] VocalizingCTypes =
        {
            "五段-ガ行",
            "五段-ナ行",
            "五段-マ行",
            "五段-バ行",
        };

        /// <summary>Lemma IDs that are vocalized when used after <see cref="VocalizingCTypes"/> and their vocalized versions.</summary>
        private static Dictionary<int, WNode> VocalizedWords = new Dictionary<int, WNode>
        {
            { 21642, new WNode { Feature = "助動詞,*,*,*,助動詞-タ,終止形,,た,だ,,だ,,,,,,,,,,,,,,,,,0,21642" } },
            { 24874, new WNode { Feature = "助詞,接続助詞,*,*,*,*,,て,で,,で,,,,,,,,,,,,,,,,,0,24874" } },
            { 22727, new WNode { Feature = "助詞,副助詞,*,*,*,*,,たり,だり,,だり,,,,,,,,,,,,,,,,,0,22727" } },
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
                                    Conjugator.ConjugateStrictly(DummyNode_22916, "連用形-一般"), // XXX
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
                            // Remove this ます, conjugating the previous 用言 into a same conjugation form as this ます,
                            // as well as conjugating or otherwise changing nearby words where necessary.
                            var conjugations1 = Conjugator.ConjugateLoosely(current.Prev, current.CForm) ?? new[] { current.Prev };
                            var conjugations2 = Conjugator.ConjugateLoosely(current.Next, current.Next.CForm) ?? new[] { current.Next };

                            // some special cases.
                            if (VocalizedWords.TryGetValue(current.Next.Lemma_id, out var vocalized_node))
                            {
                                // UniDic considers difference between た and だ, for example, is the difference in orth.
                                // In UniDic, orth differences for a same word are usually differences in
                                // meaning (sentiment), so we usually try to preserve orth when conjugating.
                                // However, the distinction of た and だ is purely a grammatical phenomenon (IMHO),
                                // and we need to handle it in an exceptional way.
                                if (VocalizingCTypes.Contains(current.Prev.CType))
                                {
                                    conjugations2 = Conjugator.ConjugateLoosely(vocalized_node, current.Next.CForm) ?? new[] { vocalized_node };
                                }
                            }
                            else if (current.Next.Lemma_id == 19587) // 助動詞「ず」
                            {
                                // replace ず (most likely in its ん form) following ます (in its ませ form) with ない,
                                // so that 読みません, for example, as a whole is replaced by 読まない
                                conjugations2 = Conjugator.ConjugateLoosely(DummyNode_27438, current.Next.CForm) ?? conjugations2;

                                // Note that the above call to ConjugateLoosely can return a null for an unusual input, 
                                // such as 読みませざれ which is parsed to 読み/ませ/ざれ with ざれ considered a 命令形 of 文語助動詞-ズ.
                                // Having ?? operator is essential to live with such an input.
                            }

                            // Find the best conjugations for up to two nodes each before and after ます.
                            var possibilities = new List<IList<WNode>>(4);
                            if (!current.Prev.IsBos) possibilities.Add(new[] { current.Prev.Prev });
                            possibilities.Add(conjugations1);
                            possibilities.Add(conjugations2);
                            if (!current.Next.IsEos) possibilities.Add(new[] { current.Next.Next });
                            var best_conjugations = Conjugator.ChooseBest(possibilities);

                            // Updated the buffer, substituting nearby nodes with best ones.
                            buffer.RemoveAt(p);
                            if (!current.Prev.IsBos) buffer[p - 1] = best_conjugations[1];
                            if (!current.Next.IsEos) buffer[p - 0] = best_conjugations[current.Prev.IsBos ? 1 : 2];
                            if (!current.Prev.IsBos) --p;
                        }
                        break;
                    case 1216: // 動詞「ある」
                        {
                            // If ある is followed by a ない, whether 27442 (形容詞) or 27438 (助動詞),
                            // remove the ある, possibly adjusting the pos of ない.
                            // the case may be produced by a rewriting of ません to ない.
                            if (current.Next.Lemma_id == 27438 || current.Next.Lemma_id == 27442)
                            {
                                var possibilities = new IList<WNode>[]
                                {
                                    Enumerable.Concat(
                                        Conjugator.ConjugateLoosely(DummyNode_27438, current.Next.CForm),
                                        Conjugator.ConjugateLoosely(DummyNode_27442, current.Next.CForm)).ToList(),
                                    new[] { current.Next.Next },
                                };
                                var best_conjugations = Conjugator.ChooseBest(possibilities);
                                buffer.RemoveAt(p);
                                buffer[p] = best_conjugations[0];
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
