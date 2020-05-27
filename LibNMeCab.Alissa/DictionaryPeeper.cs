using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NMeCab.Core;

namespace NMeCab.Alissa
{
    /// <summary>
    /// Provides <i>extension</i> methods to get information from <see cref="DictionaryBundle{TNode}"/>.
    /// </summary>
    public static class DictionaryPeeper
    {
        /// <summary>
        /// Gets all tokens from a dictionary as isolated nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes that <paramref name="bundle"/> is for.</typeparam>
        /// <param name="bundle">The dictionary bundle containing <paramref name="dictionary"/>.</param>
        /// <param name="dictionary">The dictionary to get nodes from.</param>
        /// <returns>Iteration of nodes.</returns>
        /// <remarks>
        /// Iterating over all nodes may require some significant time, depending on the size of the dictionary.  Please be careful.
        /// </remarks>
        public static IEnumerable<TNode> GetNodes<TNode>(this DictionaryBundle<TNode> bundle, MeCabDictionary dictionary) where TNode: MeCabNodeBase<TNode>, new()
        {
            var mmva = Hack.GetFieldValue(dictionary, "mmva") as MemoryMappedViewAccessor;
            var h = mmva.SafeMemoryMappedViewHandle;
            uint dsize = h.Read<uint>(24);
            uint tsize = h.Read<uint>(28);
            ulong token_table_starts = 72UL + dsize;
            ulong token_table_ends = token_table_starts + tsize;
            for (ulong t = token_table_starts; t < token_table_ends; t += 16)
            {
                var node = new TNode();
                node.LCAttr = h.Read<ushort>(t + 0);
                node.RCAttr = h.Read<ushort>(t + 2);
                node.PosId = h.Read<ushort>(t + 4);
                node.WCost = h.Read<short>(t + 6);
                node.SetFeature(h.Read<uint>(t + 8), dictionary);
                node.Stat = MeCabNodeStat.Nor;
                yield return node;
            }
        }

        /// <summary>
        /// Gets all tokens defined in all dictionaries as isolated nodes.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes that <paramref name="bundle"/> is for.</typeparam>
        /// <param name="bundle">The dictionary bundle.</param>
        /// <returns>Iteration of nodes.</returns>
        /// <remarks>
        /// Iterating over all nodes usually requires some significant time.  Please be careful.
        /// </remarks>
        public static IEnumerable<TNode> GetAllNodes<TNode>(this DictionaryBundle<TNode> bundle) where TNode: MeCabNodeBase<TNode>, new()
        {
            return bundle.Dictionaries.SelectMany(dictionary => GetNodes(bundle, dictionary));
        }

        /// <summary>
        /// Raw header information from a MeCab dic file.
        /// </summary>
        /// <remarks>
        /// Use <see cref="GetHeader{TNode}(DictionaryBundle{TNode}, MeCabDictionary)"/> to get a <see cref="Header"/> instance.
        /// Many of the members correspond to those available in <see cref="MeCabDictionary"/>,
        /// but this class provides a raw view of the header.
        /// </remarks>
        public class Header
        {
            /// <summary>Magic number combined with file size.</summary>
            public uint Magic;
            
            /// <summary>Dictionary file format version.</summary>
            public uint Version;

            /// <summary>Dictionary Type.</summary>
            public uint Type;

            /// <summary>Number of entries included in the seed dictionary file.</summary>
            public uint LexSize;

            /// <summary>Size of left attributes.</summary>
            public uint LSize;

            /// <summary>Size of right attributes.</summary>
            public uint RSize;

            /// <summary>Size of double array.</summary>
            public uint DSize;

            /// <summary>Size of Token table.</summary>
            public uint TSize;

            /// <summary>Size of Features table.</summary>
            public uint FSize;

            /// <summary>Four unused bytes.</summary>
            public uint Dummy;

            /// <summary>Character encoding used in the dictionary file.</summary>
            public byte[] Charset;
        }

        /// <summary>
        /// Gets the header information of a MeCab dic file.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes that <paramref name="bundle"/> is for.</typeparam>
        /// <param name="bundle">The dictionary bundle containing <paramref name="dictionary"/>.</param>
        /// <param name="dictionary">A dictionary object to get the header from.</param>
        /// <returns>A header.</returns>
        public static Header GetHeader<TNode>(this DictionaryBundle<TNode> bundle, MeCabDictionary dictionary) where TNode: MeCabNodeBase<TNode>, new()
        {
            var mmva = Hack.GetFieldValue(dictionary, "mmva") as MemoryMappedViewAccessor;
            var h = mmva.SafeMemoryMappedViewHandle;
            var charset = new byte[32];
            h.ReadArray(40, charset, 0, charset.Length);
            return new Header()
            {
                Magic = h.Read<uint>(0),
                Version = h.Read<uint>(4),
                Type = h.Read<uint>(8),
                LexSize = h.Read<uint>(12),
                LSize = h.Read<uint>(16),
                RSize = h.Read<uint>(20),
                DSize = h.Read<uint>(24),
                TSize = h.Read<uint>(28),
                FSize = h.Read<uint>(32),
                Dummy = h.Read<uint>(36),
                Charset = charset,
            };
        }
    }
}
