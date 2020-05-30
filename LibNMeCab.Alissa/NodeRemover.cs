using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMeCab.Alissa
{
    /// <summary>
    /// Facilitates removing several nodes from a lattice.
    /// </summary>
    /// <typeparam name="TNode">Type of nodes to be removed.</typeparam>
    /// <remarks>
    /// <para>
    /// Removing nodes with this class is a two-step operation.
    /// First, you request removal of several nodes using methods like <see cref="RemoveNode(TNode)"/>,
    /// then invoke <see cref="DoRemoves"/> to actually remove the requested nodes from lattice.
    /// </para>
    /// <para>
    /// You can't <i>cancel</i>, <i>revert</i>, or <i>reset</i> a remove request,
    /// but lattice is not updated in any way before invoking <see cref="DoRemoves"/>,
    /// and you can always stop invoking it if you don't want to remove already requested nodes.
    /// </para>
    /// </remarks>
    public class NodeRemover<TNode> where TNode : MeCabNodeBase<TNode>
    {
        private readonly MeCabLattice<TNode> Lattice;

        private readonly HashSet<TNode> List;

        /// <summary>
        /// Creates an instance to remove nodes from a lattice.
        /// </summary>
        /// <param name="lattice">Lattice to remove nodes from.</param>
        public NodeRemover(MeCabLattice<TNode> lattice)
        {
            Lattice = lattice;
            List = new HashSet<TNode>(ReferenceEqualityComparer.Instance);
        }

        /// <summary>
        /// A trivial implementation of IEqualityComparer based on reference equality for use by <see cref="List"/>.
        /// </summary>
        private class ReferenceEqualityComparer : IEqualityComparer<TNode>
        {
            private ReferenceEqualityComparer() { }
            public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();
            public bool Equals(TNode x, TNode y) => ReferenceEquals(x, y);
            public int GetHashCode(TNode x) => x.GetHashCode();
        }

        /// <summary>
        /// Request to remove all nodes that begin at a specific index.
        /// </summary>
        /// <param name="index">The index (character position).</param>
        /// <remarks>
        /// Index values beyond the practical range,
        /// e.g., a negative value or a value larger than the length of the source text,
        /// are silently ignored.
        /// </remarks>
        public void RemoveBeginningAt(int index)
        {
            if (index >= Lattice.BeginNodeList.Length - 1)
            {
                // Note that EOS is the only node that begins at BeginNodeList.Length - 1,
                // and we should never remove it from the lattice.
                // Also NOTE that we do allow removing nodes beginning at 0,
                // though it empties the whole lattice.
                return;
            }
            for (var node = Lattice.BeginNodeList[index]; node != null; node = node.BNext)
            {
                List.Add(node);
            }
        }

        /// <summary>
        /// Requests to remove all nodes that end at a specific index.
        /// </summary>
        /// <param name="index">The index (character position).</param>
        /// <remarks>
        /// Index values beyond the practical range,
        /// e.g., a negative value or a value larger than the length of the source text,
        /// are silently ignored.
        /// </remarks>
        public void RemoveEndingAt(int index)
        {
            if (index <= 0 || index > Lattice.EndNodeList.Length - 1)
            {
                // 0 could be considered a valid index value, but
                // BOS is the only node that ends at 0, and we should never remove it.
                return;
            }
            for (var node = Lattice.EndNodeList[index]; node != null; node = node.ENext)
            {
                List.Add(node);
            }
        }

        /// <summary>
        /// Requests to remove all nodes that doesn't satisfy the specified condition.
        /// </summary>
        /// <param name="condition">Delegate that returns true for a node that satisfies the condition and false otherwise.</param>
        /// <remarks>
        /// The condition is evaluated during the execution of this method,
        /// though those unsatisfied nodes are not actually removed until <see cref="DoRemoves"/> is invoked.
        /// </remarks>
        public void RemoveUnsatisfied(Func<TNode, bool> condition)
        {
            // NOTE again that BOS is the only node ending at 0,
            // and we should never remove it under any circumstances.
            for (int i = 1; i < Lattice.EndNodeList.Length; i++)
            {
                for (var node = Lattice.EndNodeList[i]; node != null; node = node.ENext)
                {
                    if (!condition(node)) List.Add(node);
                }
            }
        }

        /// <summary>
        /// Requests to remove a particular node in the lattice.
        /// </summary>
        /// <param name="node">The node to be removed.</param>
        /// <remarks>
        /// The node must be from the lattice that this instance is created for.
        /// Otherwise, the result is unpredicatable.
        /// </remarks>
        public void RemoveNode(TNode node)
        {
            List.Add(node);
        }

        /// <summary>
        /// Actually removes all nodes that were requested to remove.
        /// </summary>
        /// <remarks>
        /// Removing of a node may cause some other nodes to be disjointed from a lattice.
        /// This method checks and removes all those disjointed nodes, too.
        /// </remarks>
        public void DoRemoves()
        {
            while (List.Count > 0)
            {
                // Remove all nodes registered in the List.
                foreach (var node in List)
                {
                    DoRemoveNode(node);
                }
                List.Clear();

                // Removing of a node may cause some other nodes to be disjointed from the lattice.
                // Find all of them for the next removal pass.
                RemoveUnsatisfied(node => node.LPath == null || node.RPath == null);
            }
        }

        private void DoRemoveNode(TNode node)
        {
            RemoveFromList(node, Lattice.BeginNodeList[node.BPos], n => n.BNext, n => Lattice.BeginNodeList[node.BPos] = n, (n, m) => n.BNext = m);
            RemoveFromList(node, Lattice.EndNodeList[node.EPos], n => n.ENext, n => Lattice.EndNodeList[node.EPos] = n, (n, m) => n.ENext = m);
            for (var path = node.LPath; path != null; path = path.LNext)
            {
                RemoveFromList(path, path.LNode.RPath, p => p.RNext, p => path.LNode.RPath = p, (p, q) => p.RNext = q);
            }
            for (var path = node.RPath; path != null; path = path.RNext)
            {
                RemoveFromList(path, path.RNode.LPath, p => p.LNext, p => path.RNode.LPath = p, (p, q) => p.LNext = q);
            }
        }

        /// <summary>
        /// Removes an item from a linked list in a <i>generic</i> way.
        /// </summary>
        /// <typeparam name="T">The type of items that form a linked list.</typeparam>
        /// <param name="item">The item to remove from a linked list.</param>
        /// <param name="first">The first item in the linked list, or null if the linked list is empty.</param>
        /// <param name="next">Delegate that returns the next item in the linked list or null if there is no more item.</param>
        /// <param name="change_first">Delegate to change the first item in the linked list.</param>
        /// <param name="change_next">Delegate to change the next item of an item.</param>
        private static void RemoveFromList<T>(T item, T first, Func<T, T> next, Action<T> change_first, Action<T, T> change_next) where T : class
        {
            T p = first;
            if (ReferenceEquals(p, item))
            {
                change_first(next(item));
                return;
            }
            while (!ReferenceEquals(p, null))
            {
                T q = next(p);
                if (ReferenceEquals(q, item))
                {
                    change_next(p, next(item));
                    return;
                }
                p = q;
            }
        }
    }

    /// <summary>
    /// Provides a static method to create an instance of <see cref="NodeRemover{TNode}"/>.
    /// </summary>
    public static class NodeRemover
    {
        /// <summary>
        /// Creates a new instance of <see cref="NodeRemover{TNode}"/>.
        /// </summary>
        /// <typeparam name="TNode">Type of nodes the created <see cref="NodeRemover{TNode}"/> removes.</typeparam>
        /// <param name="lattice">A lattic that the created <see cref="NodeRemover{TNode}"/> instance removes nodes from.</param>
        /// <returns>An instance of <see cref="NodeRemover{TNode}"/>.</returns>
        public static NodeRemover<TNode> Create<TNode>(MeCabLattice<TNode> lattice) where  TNode : MeCabNodeBase<TNode>
        {
            return new NodeRemover<TNode>(lattice);
        }
    }
}
