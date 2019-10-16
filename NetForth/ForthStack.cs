using System.Collections;
using System.Collections.Generic;

namespace NetForth
{
    internal class ForthStack<T> : IEnumerable<T>
    {
		#region Data and Properties
		internal List<T> Data { get; } = new List<T>();
        internal int Count => Data.Count;
		#endregion

		#region Stack Properties
		internal T Pop()
        {
            var c = Data.Count;
            if (c == 0)
            {
				throw new NfException("Stack underflow");
            }

            var ret = Data[c - 1];
            Data.RemoveAt(c - 1);
            return ret;
        }

        internal void Push(T val)
        {
            Data.Add(val);
        }

        internal T Peek()
        {
            var c = Data.Count;
            if (c == 0)
            {
                throw new NfException("Stack underflow");
            }

            return Data[c - 1];
        }

        internal void Clear()
        {
            Data.Clear();
        }
		#endregion

		#region Indexing
		internal T this[int i]
        {
            get
            {
                if (i < 0)
                {
                    i = Data.Count + i;
                }

                if (i < 0 || i >= Data.Count)
                {
                    throw new NfException("Index out of range");
                }
                return Data[i];
			}
            set
            {
                if (i < 0)
                {
                    i = Data.Count + i;
                }
                if (i < 0 || i >= Data.Count)
                {
                    throw new NfException("Index out of range");
                }
                Data[i] = value;
            }
        }
		#endregion

		#region IEnumerable implementation
		public IEnumerator<T> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }
		#endregion
	}
}
