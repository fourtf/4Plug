using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public class PrimitiveList<T> : IEnumerable<T>
    {
        // Fields
        protected T[] arr;

        public T[] GetArray() { return arr; }

        public T this[int index]
        {
            get { return arr[index]; }
            set { arr[index] = value; }
        }


        // Properties
        public int Count { get { return arr.Length; } }

        public T Last
        {
            get
            {
                return arr[arr.Length - 1];
            }
        }



        // Constructor
        public PrimitiveList()
        {
            arr = new T[0];
        }


        // Add
        public void Add(T item)
        {
            T[] _ = new T[arr.Length + 1];
            arr.CopyTo(_, 0);
            _[_.Length - 1] = item;
            arr = _;
        }

        public void AddRange(PrimitiveList<T> list)
        {
            AddRange(list.GetArray());
        }

        public void AddRange(T[] array)
        {
            T[] _ = new T[arr.Length + array.Length];
            arr.CopyTo(_, 0);
            array.CopyTo(_, arr.Length);
            arr = _;
        }

        public void AddFromTo(PrimitiveList<T> list, int from, int to)
        {
            AddFromTo(list.GetArray(), from, to);
        }

        public void AddFromTo(T[] array, int from, int to)
        {
            T[] _ = new T[arr.Length + to - from];
            arr.CopyTo(_, 0);
            Array.Copy(array, from, _, arr.Length, to - from);
            arr = _;
        }


        // Insert
        public void Insert(T item, int index)
        {
            
        }

        public void InsertRange(PrimitiveList<T> list, int index)
        {
            InsertRange(list.GetArray(), index);
        }

        public void InsertRange(T[] items, int index, int offsetLeft = 0)
        {
            string s;
            T[] _ = new T[Count + items.Length - offsetLeft];
            Array.Copy(arr, 0, _, 0, index);
            Array.Copy(items, offsetLeft, _, index, items.Length - offsetLeft);
            Array.Copy(arr, index, _, index + items.Length - offsetLeft, _.Length - index - items.Length + offsetLeft);
            arr = _;
        }

        public void InsertEmpty(int index, int count)
        {
            T[] _ = new T[Count + count];
            Array.Copy(arr, 0, _, 0, index);
            Array.Copy(arr, 0, _, index + count, arr.Length - index);
            arr = _;
        }


        // Remove
        public void RemoveAt(int i)
        {
            RemoveFromTo(i, i + 1);
        }

        public void RemoveRange(int i, int count)
        {
            RemoveFromTo(i, i + count);
        }

        public void RemoveFromTo(int from, int to)
        {
            T[] _ = new T[Count - (to - from)];
            Array.Copy(arr, 0, _, 0, from);
            Array.Copy(arr, to, _, from, Count - to);
            arr = _;
        }

        public void RemoveTo(int to)
        {
            RemoveFromTo(0, to);
        }

        public void RemoveFrom(int from)
        {
            RemoveFromTo(from, arr.Length);
        }


        // Clear
        public void Clear()
        {
            arr = new T[0];
        }


        // IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)arr).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return arr.GetEnumerator();
        }
    }
}
