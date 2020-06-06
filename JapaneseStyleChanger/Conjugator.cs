﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NMeCab;
using NMeCab.Alissa;

namespace JapaneseStyleChanger
{
    public class Conjugator
    {
        private readonly DictionaryBundle<WNode> Dictionaries;

        private readonly Dictionary<WNode, List<WNode>> ConjugationTable;

        public float CostMixFactor = 0.5f;

        public Conjugator(Tagger<WNode> tagger)
        {
            Dictionaries = Hack.GetDictionaries(tagger);
            ConjugationTable = BuildConjugationTable(Dictionaries);
        }

        public IList<WNode> ConjugateStrictly(WNode node, string cform)
        {
            return Conjugate(node, n => n.CForm == cform);
        }

        public IList<WNode> ConjugateLoosely(WNode node, string cform)
        {
            var i = cform.IndexOf('-');
            var loose_cform = (i >= 0) ? cform.Substring(0, i) : cform;
            return Conjugate(node, n =>
            {
                var f = n.CForm;
                return f.StartsWith(loose_cform)
                && (f.Length == loose_cform.Length || f[loose_cform.Length] == '-');
            });
        }

        public IList<WNode> Conjugate(WNode node, Func<WNode, bool> chooser)
        {
            List<WNode> list;
            if (!ConjugationTable.TryGetValue(node, out list)) return null;
            list = list.Where(n => n.Surface != null && chooser(n)).ToList();
            if (list.Count == 0) return null;
            return list;
        }

        public IEnumerable<WNode> GetConjugations(WNode node)
        {
            if (node == null) return null;

            if (ConjugationTable.TryGetValue(node, out var list))
            {
                return list;
            }
            else
            {
                return Enumerable.Empty<WNode>();
            }
        }

        public int TotalCost(WNode[] text)
        {
            return Dictionaries.TotalCost(text);
        }

        private struct Path
        {
            public int Cost;
            public int Previous;
        }

        public WNode[] ChooseBest(IList<IList<WNode>> nodes)
        {
            var paths = new Path[nodes.Count][];
            paths[0] = new Path[nodes[0].Count];
            for (int p = 1; p < nodes.Count; p++)
            {
                paths[p] = new Path[nodes[p].Count];
                for (int i = 0; i < paths[p].Length; i++)
                {
                    var min_cost = int.MaxValue;
                    for (int j = 0; j < paths[p - 1].Length; j++)
                    {
                        var cost = paths[p - 1][j].Cost + Dictionaries.MixedCost(CostMixFactor, nodes[p - 1][j], nodes[p][i]); // XXX XXX XXX
                        if (cost < min_cost)
                        {
                            paths[p][i].Previous = j;
                            min_cost = cost;
                        }
                    }
                    paths[p][i].Cost = min_cost;
                }
            }
            var result = new WNode[nodes.Count];
            var best_path = default(Path); // XXX
            int best_index = 0;
            for (int i = 0; i < paths[paths.Length - 1].Length; i++)
            {
                if (paths[paths.Length - 1][i].Cost < best_path.Cost)
                {
                    best_path = paths[paths.Length - 1][i];
                    best_index = i;
                }
            }
            result[paths.Length - 1] = nodes[nodes.Count - 1][best_index];
            int previous = paths[paths.Length - 1][best_index].Previous;
            for (int p = paths.Length - 2; p >= 0; --p)
            {
                result[p] = nodes[p][previous];
                previous = paths[p][previous].Previous;
            }
            return result;
        }

        private Dictionary<WNode, List<WNode>> BuildConjugationTable(DictionaryBundle<WNode> dictionaries)
        {
            // Group nodes per stems.
            var table = new Dictionary<WNode, List<WNode>>(new ConjugationTableComparer());
            foreach (var node in dictionaries.GetAllNodes())
            {
                if (node.CType != "*")
                {
                    node.Surface = node.Orth;
                    List<WNode> list;
                    if (!table.TryGetValue(node, out list))
                    {
                        list = new List<WNode>();
                        table.Add(node, list);
                    }
                    list.Add(node);
                }
            }

            // Remove non-preferred (for the purpose of this app) entries.
            var redundant = new List<WNode>();
            foreach (var list in table.Values)
            {
                redundant.Clear();
                foreach (var g in list.GroupBy(n => n.CForm))
                {
                    if (g.Count() > 1)
                    {
                        redundant.AddRange(g.OrderBy(n =>
                        {
                            var lid = n.Lid;
                            if ((lid & 0x01E0) != 0)
                            {
                                return (int)lid & 0x01FF;
                            }
                            else
                            {
                                return int.MaxValue;
                            }
                        }).Skip(1));
                    }
                }
                foreach (var n in redundant)
                {
#if true
                    // Surface == null indicates the particular node should be excluded.
                    // See Conjugate method.
                    // XXX: we need a better way to flag it.
                    n.Surface = null;
#else
                    // Alternatively, we could just remove them from the ConjugationTable.
                    // If we do so, users can't see it even in GetConjugations. 
                    list.Remove(n);
#endif
                }
            }
            return table;
        }

        private class ConjugationTableComparer : IEqualityComparer<WNode>
        {
            public bool Equals(WNode x, WNode y)
            {
                return x.Lemma_id == y.Lemma_id
                    && x.OrthBase == y.OrthBase
                    && x.CType == y.CType;
            }

            public int GetHashCode(WNode node)
            {
                return node.Lemma_id.GetHashCode()
                    + (node.OrthBase.GetHashCode() ^ 0x5CCBF78E)
                    + (node.CType.GetHashCode() ^ 0x5FF47E32);
            }
        }
    }
}