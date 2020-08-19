using System;
using System.Collections.Concurrent;
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

        private readonly IDictionary<int, List<WNode>> ConjugationTable;

        public const double CostMixFactor = 0.5; // 0.7; // So, finally, I'm stopping it... soon...

        public Conjugator(Tagger<WNode> tagger)
        {
            Dictionaries = Hack.GetDictionaries(tagger);
            ConjugationTable = BuildConjugationTable();
        }

        public IList<WNode> ConjugateStrictly(WNode node, string cform)
        {
            return Conjugate(node, n => n.CForm == cform && n.CType == node.CType && n.OrthBase == node.OrthBase);
        }

        public IList<WNode> ConjugateLoosely(WNode node, string cform)
        {
            var i = cform.IndexOf('-');
            var loose_cform = (i >= 0) ? cform.Substring(0, i) : cform;
            return Conjugate(node, n =>
            {
                var f = n.CForm;
                return f.StartsWith(loose_cform)
                && (f.Length == loose_cform.Length || f[loose_cform.Length] == '-')
                && n.CType == node.CType && n.OrthBase == node.OrthBase;
            });
        }

        private IList<WNode> Conjugate(WNode node, Func<WNode, bool> chooser)
        {
            if (!ConjugationTable.TryGetValue(node.Lemma_id, out var list)) return null;
            list = list.Where(n => n.Surface != null && chooser(n)).ToList();
            if (list.Count == 0) return null;
            return list;
        }

        private struct Path
        {
            public double Cost;
            public int Previous;
        }

        public WNode[] ChooseBest(IList<IList<WNode>> nodes)
        {
#if DEBUG
            var cacs = DumpAllCosts(nodes);
#endif
            var paths = new Path[nodes.Count][];
            paths[0] = new Path[nodes[0].Count];
            for (int i = 0; i < paths[0].Length; i++)
            {
                paths[0][i].Cost = nodes[0][i].WCost * (1 - CostMixFactor);
            }
            for (int p = 1; p < nodes.Count; p++)
            {
                paths[p] = new Path[nodes[p].Count];
                for (int i = 0; i < paths[p].Length; i++)
                {
                    double min_cost = double.MaxValue;
                    for (int j = 0; j < paths[p - 1].Length; j++)
                    {
                        double cost = paths[p - 1][j].Cost + Dictionaries.MixedCostIncrease(CostMixFactor, nodes[p - 1][j], nodes[p][i]);
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

#if DEBUG

        public class CostAndCandidate
        {
            public double Cost;
            public WNode[] Nodes;

            public override string ToString()
            {
                return string.Format("{0,12:F2} {1}", Cost, string.Join("|", Nodes.Select(n => n.Surface)));
            }
        }

        public IList<CostAndCandidate> DumpAllCosts(IList<IList<WNode>> candidate_nodes, int index = -1)
        {
            if (index < 0)
            {
                return DumpAllCosts(candidate_nodes, candidate_nodes.Count - 1).OrderBy(cap => cap.Cost).ToList();
            }
            else if (index == 0)
            {
                var list = new List<CostAndCandidate>(candidate_nodes[index].Count);
                foreach (var n in candidate_nodes[index])
                {
                    var cost = n.WCost * (1 - CostMixFactor);
                    var nodes = new WNode[] { n };
                    list.Add(new CostAndCandidate { Cost = cost, Nodes = nodes });
                }
                return list;
            }
            else
            {
                var p = DumpAllCosts(candidate_nodes, index - 1);
                var list = new List<CostAndCandidate>(p.Count * candidate_nodes[index].Count);
                foreach (var n in candidate_nodes[index])
                {
                    foreach (var cac in p)
                    {
                        var cost = cac.Cost + Dictionaries.MixedCostIncrease(CostMixFactor, cac.Nodes[cac.Nodes.Length - 1], n);
                        var nodes = new WNode[cac.Nodes.Length + 1];
                        Array.Copy(cac.Nodes, nodes, cac.Nodes.Length);
                        nodes[cac.Nodes.Length] = n;
                        list.Add(new CostAndCandidate { Cost = cost, Nodes = nodes });
                    }
                }
                return list;
            }
        }

#endif

        private IDictionary<int, List<WNode>> BuildConjugationTable()
        {
            // Group conjugating nodes per their Lemma_id
            var table = new ConcurrentDictionary<int, List<WNode>>();
            Parallel.ForEach(Dictionaries.GetAllNodes(), node =>
            {
                if (node.CType != "*")
                {
                    node.Surface = node.Orth;
                    node.Length = node.RLength = node.Surface.Length;
                    var list = table.GetOrAdd(node.Lemma_id, key => new List<WNode>());
                    lock (list)
                    {
                        list.Add(node);
                    }
                }
            });

            // Remove non-preferred (for the purpose of this app) entries.
            Parallel.ForEach(table.Values, list =>
            {
                var redundant = new List<WNode>();

                // Choose one preferred node among those sharing OrthBase, CType, and CForm.
                foreach (var g in list.GroupBy(n => n.CType + ":" + n.CForm + ":" + n.OrthBase))
                {
                    if (g.Count() > 1)
                    {
                        redundant.AddRange(g.OrderBy(n =>
                        {
                            var lid = n.Lid;
                            if ((lid & 0x01E0) == 0)
                            {
                                // Those with none of the bits at 5 thru 8 set in Lid are least preferred.
                                return int.MaxValue;
                            }
                            else
                            {
                                // Otherwise, the preference is induced by the lowest 9 bits of Lid.
                                return (int)lid & 0x01FF;
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
            });

            return table;
        }
    }
}
