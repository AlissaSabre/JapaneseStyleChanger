using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMeCab.Alissa
{
    /// <summary>
    /// Constrains lattices to produce desired parsing.
    /// </summary>
    /// <typeparam name="TNode">Type of nodes the constraints appliy to.</typeparam>
    /// <remarks>
    /// <para>
    /// Constraints class works with <see cref="MeCabLattice{TNode}"/> to provide
    /// a similar functionality to partial analysis (部分解析) feature, 
    /// also known as constrained parsing (制約付き解析),
    /// of original MeCab.
    /// </para>
    /// <para>
    /// To use this class,
    /// you need to use a method like <see cref="MeCabTaggerBase{TNode}.ParseToLattice(string, MeCabParam)"/>
    /// to get a full lattice.
    /// Then, create an instance of this class,
    /// add several constraints to it through <see cref="Boundaries"/> and/or <see cref="Conditions"/>,
    /// and invoke <see cref="ApplyTo(MeCabLattice{TNode})"/> to plune the nodes in lattice.
    /// You can then use methods like <see cref="MeCabLattice{TNode}.GetBestNodes()"/>
    /// to get the result of constrained analysis.
    /// </para>
    /// <para>
    /// Please note an important limitation of this class.
    /// Unlike the original MeCab's partial analysis feature,
    /// this class works only after a tagger produced a full lattice.
    /// As a result, this class can't introduce a new node that was not produced by the set of dictionaries.
    /// </para>
    /// </remarks>
    public class Constraints<TNode> where TNode : MeCabNodeBase<TNode>
    {
        /// <summary>
        /// A collection of constraints on boundaries.
        /// </summary>
        /// <remarks>
        /// Use indexer of BoundaryConstraints to add a new boundary constraint to this Constraints object.
        /// </remarks>
        public BoundaryConstraints Boundaries { get; } = new BoundaryConstraints();

        /// <summary>
        /// A collection of general conditinal constraints.
        /// </summary>
        /// <remarks>
        /// Use <see cref="ConditionConstraints.Add(Func{TNode, bool})"/> to add a new conditinal constraint to this Constraints object.
        /// </remarks>
        public ConditionConstraints Conditions { get; } = new ConditionConstraints();

        /// <summary>
        /// Applies constraints to a lattice to plune it.
        /// </summary>
        /// <param name="lattice">The lattice constraints are applied to.</param>
        public void ApplyTo(MeCabLattice<TNode> lattice)
        {
            var remover = new NodeRemover<TNode>(lattice);
            foreach (var pair in Boundaries)
            {
                var index = pair.Key;
                switch (pair.Value)
                {
                    case BoundaryType.Boundary:
                        remover.RemoveUnsatisfied(node => node.BPos < index && node.EPos > index);
                        break;

                    case BoundaryType.Insdie:
                        remover.RemoveBeginningAt(index);
                        remover.RemoveEndingAt(index);
                        break;

                    default:
                        throw new ApplicationException("Internal error.");
                }
            }
            foreach (var condition in Conditions)
            {
                remover.RemoveUnsatisfied(condition);
            }
            remover.DoRemoves();
        }

        public class BoundaryConstraints : IEnumerable<KeyValuePair<int, BoundaryType>>
        {
            private readonly SortedDictionary<int, BoundaryType> Dictionary = new SortedDictionary<int, BoundaryType>();

            public BoundaryType this[int index]
            {
                get
                {
                    if (index < 0) throw new IndexOutOfRangeException();
                    if (Dictionary.TryGetValue(index, out var value)) return value;
                    return BoundaryType.Any;
                }
                set
                {
                    if (index < 0) throw new IndexOutOfRangeException();
                    switch (value)
                    {
                        case BoundaryType.Any:
                            Dictionary.Remove(index);
                            break;
                        case BoundaryType.Boundary:
                        case BoundaryType.Insdie:
                            Dictionary[index] = value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid value");
                    }
                }
            }

            public void Clear()
            {
                Dictionary.Clear();
            }

            public IEnumerator<KeyValuePair<int, BoundaryType>> GetEnumerator()
            {
                return Dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (Dictionary as IEnumerable).GetEnumerator();
            }
        }

        public class ConditionConstraints : IEnumerable<Func<MeCabNodeSuperBase, bool>>
        {
            private readonly List<Func<MeCabNodeSuperBase, bool>> Conditions = new List<Func<MeCabNodeSuperBase, bool>>();

            public void Add(Func<TNode, bool> condition)
            {
                Conditions.Add(node => condition(node as TNode));
            }

            public void AddRange(IEnumerable<Func<TNode, bool>> conditions)
            {
                Conditions.AddRange(conditions.Select<Func<TNode, bool>, Func<MeCabNodeSuperBase, bool>>(condition => node => condition(node as TNode)));
            }

            public void Clear()
            {
                Conditions.Clear();
            }

            public IEnumerator<Func<MeCabNodeSuperBase, bool>> GetEnumerator()
            {
                return Conditions.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (Conditions as IEnumerable).GetEnumerator();
            }
        }
    }

    /// <summary>
    /// Types of a boundary constraint.
    /// </summary>
    /// <remarks>
    /// This enum is used by <see cref="Constraints{TNode}.Boundaries"/>.
    /// </remarks>
    public enum BoundaryType
    {
        ///<summary>The token boundary is not specified.</summary>
        MECAB_ANY_BOUNDARY = 0,

        ///<summary>The position is a strong token boundary.</summary>
        MECAB_TOKEN_BOUNDARY = 1,

        ///<summary>The position is not a token boundary.</summary>
        MECAB_INSIDE_TOKEN = 2,

        /// <summary>Constraints nothing on token boundaries.</summary>
        /// <remarks>This is an alias to <see cref="MECAB_ANY_BOUNDARY"/>.</remarks>
        Any = MECAB_ANY_BOUNDARY,

        /// <summary>Insists a token boundary.</summary>
        /// <remarks>This is an alias to <see cref="MECAB_TOKEN_BOUNDARY"/>.</remarks>
        Boundary = MECAB_TOKEN_BOUNDARY,

        /// <summary>Prohibits a token boundary.</summary>
        /// <remarks>This is an alias to <see cref="MECAB_INSIDE_TOKEN"/>.</remarks>
        Insdie = MECAB_INSIDE_TOKEN,
    }
}
