using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public class LimitedStack<T>
    {
        private List<T> list;

        private int index;

        private int limit;

        public int Limit
        {
            get { return limit; }
            set { value = value - 1; if (value < index) { list.RemoveRange(0, index - value); index = value; }; limit = value; }
        }

        public LimitedStack(int limit)
        {
            list = new List<T>(limit - 1);
            this.limit = limit - 1;
            index = -1;
        }

        public T Pop()
        {
            T t = list[index];
            list.RemoveAt(index);
            index--;
            return t;
        }

        public T Seek()
        {
            return list[index];
        }

        public void Push(T item)
        {
            if (index < limit)
            {
                list.Add(item);
                index++;
            }
            else
            {
                list.RemoveAt(0);
                list.Add(item);
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public T[] ToArray()
        {
            return list.ToArray();
        }
    }
}
