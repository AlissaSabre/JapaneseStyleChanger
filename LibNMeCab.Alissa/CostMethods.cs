using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMeCab.Alissa
{
    /// <summary>
    /// Provides extension methods to calculate several types of cost.
    /// </summary>
    public static class CostMethods
    {


        /// <summary>
        /// Calculates the total cost of a series of nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes.</typeparam>
        /// <param name="bundle">The dictionary bundle to get cost parameters from.</param>
        /// <param name="array">An array of nodes.</param>
        /// <returns>The total cost of the array of nodes, i.e., the sum of word costs (生起コスト) and path costs (遷移コスト).</returns>
        /// <remarks>
        /// <para>
        /// This method uses nodes' <see cref="MeCabNodeSuperBase.WCost"/> but
        /// doesn't use <see cref="MeCabNodeBase{TNode}.LPath"/>, <see cref="MeCabNodeBase{TNode}.RPath"/>, or their <see cref="MeCabPath{TNode}.Cost"/>,
        /// so <paramref name="array"/> doesn't need to be from a parsed lattice.
        /// Path costs are looked anew up in <paramref name="bundle"/>
        /// (in particular in <see cref="DictionaryBundle{TNode}.Connector"/>).
        /// </para>
        /// <para>
        /// This method doesn't count the path costs from BOS to the first node and from the last node to EOS,
        /// so that it works better if applied to a sequence of nodes in a middle of a sentence.
        /// </para>
        /// </remarks>
        public static int TotalCost<TNode>(this DictionaryBundle<TNode> bundle, params TNode[] array)
            where TNode : MeCabNodeBase<TNode>, new()
        {
            if (array.Length == 0) return 0;
            var connector = bundle.Connector;
            TNode node, prev;
            node = prev = array[0];
            int cost = node.WCost;
            for (int i = 1; i < array.Length; i++)
            {
                node = array[i];
                cost += connector.Cost(prev, node);
                prev = node;
            }
            return cost;
        }

        /// <summary>
        /// Calculates the sum of word costs (生起コスト) of nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes.</typeparam>
        /// <param name="bundle">Not used for calculation.</param>
        /// <param name="array">An array of nodes.</param>
        /// <returns>The sum of word costs.</returns>
        /// <remarks>
        /// This method uses nodes' <see cref="MeCabNodeSuperBase.WCost"/> to calculate the sum.
        /// <paramref name="bundle"/> parameter is actually not needed.
        /// </remarks>
        public static int WordsCost<TNode>(this DictionaryBundle<TNode> bundle, params TNode[] array)
            where TNode : MeCabNodeBase<TNode>, new()
        {
            return array.Sum(n => n.WCost);
        }

        /// <summary>
        /// Calculates the sum of path costs (遷移コスト) between nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes.</typeparam>
        /// <param name="bundle">The dictionary bundle to get cost parameters from.</param>
        /// <param name="array">An array of nodes.</param>
        /// <returns>The sum of path costs.</returns>
        /// <remarks>
        /// This method doesn't use any cost information stored in or linked from nodes.
        /// Path costs are looked anew up in <paramref name="bundle"/>
        /// (in particular in <see cref="DictionaryBundle{TNode}.Connector"/>).
        /// </remarks>
        public static int PathsCost<TNode>(this DictionaryBundle<TNode> bundle, params TNode[] array)
            where TNode : MeCabNodeBase<TNode>, new()
        {
            return TotalCost(bundle, array) - WordsCost(bundle, array);
        }

        /// <summary>
        /// Calculates a weighted mix of word costs (生起コスト) and path costs (遷移コスト) over a series of nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes.</typeparam>
        /// <param name="bundle">The dictionary bundle to get cost parameters from.</param>
        /// <param name="weight">A factor between 0.0f and 1.0f to determine the weights of word and path costs.</param>
        /// <param name="array">An array of nodes.</param>
        /// <returns>The sum of a mix of word costs and path costs.</returns>
        /// <remarks>
        /// Use <paramref name="weight"/> to control how much of word and path costs contribute to the cost.
        /// If it is 0.0f, the resulting cost equals to the word cost (<see cref="WordsCost{TNode}"/>).
        /// If it is 1.0f, the resulting cost equals to the path cost (<see cref="PathsCost{TNode}"/>).
        /// If it is in between, the resulting cost is a weighted mix of word and path costs.
        /// In particular, <paramref name="weight"/> of 0.5f gives exactly a half of the total cost
        /// (<see cref="TotalCost{TNode}"/>).
        /// <paramref name="weight"/> can be below 0 or above 1, though the returned value may be meaningless.
        /// </remarks>
        public static int MixedCost<TNode>(this DictionaryBundle<TNode> bundle, double weight, params TNode[] array)
            where TNode : MeCabNodeBase<TNode>, new()
        {
            // Note that f * P + (1 - f) * W == f * (P + W) - (f * 2 - 1) * W,
            // and the right side is easier to calculate in this case.
            return -(int)Math.Round(weight * TotalCost(bundle, array) - (weight * 2 - 1) * WordsCost(bundle, array));
        }
    }
}
