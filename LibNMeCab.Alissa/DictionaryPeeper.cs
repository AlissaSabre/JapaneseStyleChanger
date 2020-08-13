using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
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
        public static IEnumerable<TNode> GetNodes<TNode>(this DictionaryBundle<TNode> bundle, MeCabDictionary dictionary) where TNode: MeCabNodeBase<TNode>
        {
            var address = GetSafeMemoryMappedViewAddress(dictionary);
            ulong token_table_starts;
            ulong token_table_ends;
            GetTokenTableLocations(address, out token_table_starts, out token_table_ends);
            for (ulong t = token_table_starts; t < token_table_ends; t += 16)
            {
                var node = bundle.NodeAllocator();
                LoadNodeData(t, node);
                node.Feature = GetFeature(t, bundle, dictionary);
                node.Stat = MeCabNodeStat.Nor;
                yield return node;
            }
        }

        private unsafe static void GetTokenTableLocations(ulong address, out ulong starts, out ulong ends)
        {
            var h = (byte*)address;
            uint dsize = *(uint*)(h + 24);
            uint tsize = *(uint*)(h + 28);
            starts = address + 72UL + dsize;
            ends = starts + tsize;
        }

        private unsafe static void LoadNodeData<TNode>(ulong address, TNode node) where TNode : MeCabNodeBase<TNode>
        {
            var h = (byte*)address;
            node.LCAttr = *(ushort*)(h + 0);
            node.RCAttr = *(ushort*)(h + 2);
            node.PosId = *(ushort*)(h + 4);
            node.WCost = *(short*)(h + 6);
        }

        private unsafe static string GetFeature<TNode>(ulong address, DictionaryBundle<TNode> bundle, MeCabDictionary dic) where TNode : MeCabNodeBase<TNode>
        {
            var h = (byte*)address;
            return StrUtils.GetString(dic.GetFeature(*(uint*)(h + 8)), bundle.Tokenizer.Encoding);
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
        public static IEnumerable<TNode> GetAllNodes<TNode>(this DictionaryBundle<TNode> bundle) where TNode: MeCabNodeBase<TNode>
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
        public unsafe static Header GetHeader<TNode>(this DictionaryBundle<TNode> bundle, MeCabDictionary dictionary) where TNode : MeCabNodeBase<TNode>
        {
            var h = (byte*)GetSafeMemoryMappedViewAddress(dictionary);
            var charset = new byte[32];
            for (int i = 0; i < charset.Length; i++) charset[i] = h[40 + i];
            return new Header()
            {
                Magic = *(uint*)(h + 0),
                Version = *(uint*)(h + 4),
                Type = *(uint*)(h + 8),
                LexSize = *(uint*)(h + 12),
                LSize = *(uint*)(h + 16),
                RSize = *(uint*)(h + 20),
                DSize = *(uint*)(h + 24),
                TSize = *(uint*)(h + 28),
                FSize = *(uint*)(h + 32),
                Dummy = *(uint*)(h + 36),
                Charset = charset,
            };
        }

        private unsafe static ulong GetSafeMemoryMappedViewAddress(MeCabDictionary dictionary)
        {
            var mmfLoader = Hack.GetFieldValue(dictionary, "mmfLoader") as MemoryMappedFileLoader;
            var mmva = Hack.GetFieldValue(mmfLoader, "mmva") as MemoryMappedViewAccessor;
            byte* pointer = null;
            mmva.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
            return (ulong)pointer;
        }
    }
}
