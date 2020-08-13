using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMeCab.Alissa
{
    /// <summary>
    /// A generic version of MeCab tagger.
    /// </summary>
    /// <typeparam name="TNode">Type of nodes that this tagger handles.</typeparam>
    public class Tagger<TNode> : MeCabTaggerBase<TNode> where TNode : MeCabNodeBase<TNode>
    {
        /// <summary>
        /// Creates an instance of this tagger.
        /// </summary>
        /// <remarks>
        /// Use <see cref="Create(Func{TNode}, string, string[])"/> instead if you need to create an instance.
        /// </remarks>
        private Tagger() { }

        /// <summary>
        /// Create an instance of this tagger.
        /// </summary>
        /// <param name="allocator">A function to create a new instance of <typeparamref name="TNode"/>.</param>
        /// <param name="dic_dir">Pathname of the folder that contain dictionary files.</param>
        /// <param name="user_dics">Filenames (without any directories) of user dictionaries.</param>
        /// <returns>An instance of a tagger.</returns>
        public static Tagger<TNode> Create(Func<TNode> allocator, string dic_dir, params string[] user_dics)
        {
            return Create(dic_dir, user_dics, () => new Tagger<TNode>(), allocator, null);
        }
    }
}
