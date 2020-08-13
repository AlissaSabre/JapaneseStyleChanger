using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NMeCab.Alissa
{
    /// <summary>
    /// Provides static methods to peep into intenal data structures of NMeCab library.
    /// </summary>
    /// <remarks>
    /// This class relies on implementation details of NMeCab library.
    /// Use with care at your own risk.
    /// </remarks>
    public static class Hack
    {
        /// <summary>
        /// Creates an instance of dictionary bundle upon a tagger.
        /// </summary>
        /// <typeparam name="TNode">The type of nodes returned by the tagger that the dictionary bundle is built upon.</typeparam>
        /// <param name="tagger">A tagger instance.</param>
        /// <returns></returns>
        public static DictionaryBundle<TNode> GetDictionaries<TNode>(MeCabTaggerBase<TNode> tagger) where TNode : MeCabNodeBase<TNode>
        {
            return new DictionaryBundle<TNode>(tagger);
        }

        /// <summary>
        /// Gets the value of a private field of an object.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <param name="name">Name of a field.</param>
        /// <returns>The value of the field.</returns>
        public static object GetFieldValue(object obj, string name)
        {
            // Sometimes GetField fails to return a non-hidden field defined in a base class of an object.
            // I'm not sure why...  Anyway, a workaround is easy.
            for (var type = obj.GetType(); type != null; type = type.BaseType)
            {
                var f = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
                if (f != null)
                {
                    return f.GetValue(obj);
                }
            }
            throw new ArgumentException($"field named \"{name}\" not found", nameof(name));
        }
    }
}
