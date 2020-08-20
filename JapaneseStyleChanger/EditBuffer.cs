using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NMeCab;

namespace JapaneseStyleChanger
{
    public class EditBuffer : IList<WNode>
    {
        protected readonly List<WNode> List;

        public bool Changed { get; set; }

        public readonly WNode BOS;

        public readonly WNode EOS;

        public EditBuffer(IEnumerable<WNode> analysis)
        {
            if (analysis is null) throw new ArgumentNullException(nameof(analysis));

            List = new List<WNode>();
            List.AddRange(analysis);
            if (List.Count == 0)
            {
                BOS = new WNode();
                EOS = new WNode();
                BOS.Next = EOS;
                EOS.Prev = BOS;
            }
            else
            {
                BOS = List[0].Prev;
                EOS = List[List.Count - 1].Next;
                for (int i = 0; i < List.Count; i++)
                {
                    List[i].Prev = i > 0 ? List[i - 1] : BOS;
                    List[i].Next = i < List.Count - 1 ? List[i + 1] : EOS;
                }
            }

            Changed = true;
        }

        public WNode this[int index]
        {
            get
            {
                if (index < 0) return BOS;
                if (index < List.Count) return List[index];
                return EOS;
            }
            set
            {
                if (index < 0 || index >= List.Count) throw new IndexOutOfRangeException();
                if (!ReferenceEquals(value, List[index]))
                {
                    var before = List[index];
                    var prev = before.Prev;
                    var next = before.Next;
                    List[index] = value;
                    value.Prev = prev;
                    value.Next = next;
                    prev.Next = value;
                    next.Prev = value;
                    Changed = true;
                }
            }
        }

        public int Count => List.Count;

        public bool IsReadOnly => false;

        public void Add(WNode item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            var last = EOS.Prev;
            List.Add(item);
            item.Prev = last;
            item.Next = EOS;
            last.Next = item;
            EOS.Prev = item;
            Changed = true;
        }

        public void Clear()
        {
            List.Clear();
            EOS.Prev = BOS;
            BOS.Next = EOS;
            Changed = true;
        }

        public bool Contains(WNode item)
        {
            return List.Contains(item);
        }

        public void CopyTo(WNode[] array, int array_index)
        {
            List.CopyTo(array, array_index);
        }

        public IEnumerator<WNode> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public int IndexOf(WNode item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, WNode item)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), index, "index must be zero or positive");
            if (index > List.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "index exceeds Count");
            if (item is null) throw new ArgumentNullException(nameof(item));

            var next = (index < List.Count) ? List[index] : EOS;
            var prev = next.Prev;
            List.Insert(index, item);
            item.Next = next;
            item.Prev = prev;
            prev.Next = item;
            next.Prev = item;
            Changed = true;
        }

        public bool Remove(WNode item)
        {
            if (item == null) return false;

            var index = List.IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                Changed = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0) throw new ArgumentNullException(nameof(index));
            if (index >= List.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "index exceeds or equals to Count");

            var item = List[index];
            List.RemoveAt(index);
            var prev = item.Prev;
            var next = item.Next;
            prev.Next = next;
            next.Prev = prev;
            Changed = true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (List as IEnumerable).GetEnumerator();
        }

        /// <summary>Returns a string representation primarily for debugging.</summary>
        /// <returns>A series of <see cref="WNode.Surface"/> strings.</returns>
        public override string ToString()
        {
            return string.Join("/", List.Select(n => n.Surface));
        }
    }
}
