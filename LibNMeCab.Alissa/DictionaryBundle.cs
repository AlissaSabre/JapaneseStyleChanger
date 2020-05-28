using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using NMeCab.Core;

namespace NMeCab.Alissa
{
    /// <summary>
    /// Bundle of dictionary-related objects extracted from a MeCab tagger.
    /// </summary>
    /// <typeparam name="TNode">The type of nodes returned by the tagger that this class is built upon.</typeparam>
    /// <remarks>
    /// <para>
    /// This class depends on implementation details of NMeCab library.
    /// Use with care at your own risk.
    /// </para>
    /// <para>
    /// An instance of this class holds references to MeCab's internal objects
    /// that are owned by the tagger and are disposed as the tagger is disposed.
    /// You need to keep the tagger object undisposed as long as you want to use an instance of this class.
    /// Also, you should not try to dispose directly any of the objects that this class provides;
    /// You should dispose the tagger instead after finished using it.
    /// </para>
    /// </remarks>
    public class DictionaryBundle<TNode> where TNode : MeCabNodeBase<TNode>, new()
    {
        /// <summary>
        /// Vierbi analysis engine.
        /// </summary>
        public readonly Viterbi<TNode> Viterbi;

        /// <summary>
        /// Path cost matrix manager.
        /// </summary>
        public readonly Connector<TNode> Connector;

        /// <summary>
        /// Token recognizer.
        /// </summary>
        public readonly Tokenizer<TNode> Tokenizer;

        /// <summary>
        /// A list of dictionaries in use.
        /// </summary>
        public readonly MeCabDictionary[] Dictionaries;

        /// <summary>
        /// The <i>system</i> dictionary.
        /// </summary>
        /// <remarks>
        /// The system dictionary is the one in <see cref="Dictionaries"/> at index 0.
        /// If you modify the array (not recommended), <see cref="SystemDictionary"/> may be changed.
        /// </remarks>
        public MeCabDictionary SystemDictionary => Dictionaries.Length > 0 ? Dictionaries[0] : null;

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <param name="tagger">A tagger that this instnace peeps in.</param>
        /// <remarks>
        /// It is usually easier to use <see cref="Hack.GetDictionaries{TNode}(MeCabTaggerBase{TNode})"/> than this constructor.
        /// </remarks>
        public DictionaryBundle(MeCabTaggerBase<TNode> tagger)
        {
            Viterbi = Hack.GetFieldValue(tagger, "viterbi") as Viterbi<TNode>;
            Connector = Hack.GetFieldValue(Viterbi, "connector") as Connector<TNode>;
            Tokenizer = Hack.GetFieldValue(Viterbi, "tokenizer") as Tokenizer<TNode>;
            Dictionaries = Hack.GetFieldValue(Tokenizer, "dic") as MeCabDictionary[];
        }
    }
}
