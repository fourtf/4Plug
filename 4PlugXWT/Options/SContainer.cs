using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Options
{
    public class SChildChangedEventArgs : EventArgs
    {
        public SChild Child { get; set; }
        public SContainer Parent { get; set; }
        public int NewIndex { get; set; }
    }

    public abstract class SContainer : SChild, System.Collections.IEnumerable
    {
        public event EventHandler<SChildChangedEventArgs> ChildAdded;
        //public event EventHandler<SChildChangedEventArgs> ChildRemoved;
        //public event EventHandler<SChildChangedEventArgs> ChildMoved;

        public List<SChild> children = new List<SChild>();

        protected abstract void OnAddChild(SChild child);
        protected abstract void OnRemoveChild(SChild child);
        protected abstract void OnMoveChild(SChild child, int index);

        public virtual int IndexOf(SChild child)
        {
            return children.IndexOf(child);
        }

        /*public virtual void Remove()
        {
            if (children.Count > 0)
                foreach (SChild b in children)
                    b.Parent.RemoveChild(b);
            Parent.RemoveChild(this);
        }
        */

        public SChild GetChildByID(string ID)
        {
            foreach (var child in children)
            {
                if (child.ID == ID)
                    return child;
                if (child is SContainer)
                {
                    SChild c;
                    if ((c = ((SContainer)child).GetChildByID(ID)) != null)
                        return c;
                }
            }
            return null;
        }

        public SContainer AddChild(SChild child)
        {
            child.Parent = this;
            child.Window = Window;
            children.Add(child);

            OnAddChild(child);

            if (ChildAdded != null)
                ChildAdded(this, new SChildChangedEventArgs() { Child = child, Parent = this });
            return this;
        }

        /*public void RemoveChild(SChild child)
        {
            children.Remove(child);
            child.Parent = null;
            child.Window = null;

            OnRemoveChild(child);

            if (ChildRemoved != null)
                ChildRemoved(this, new SChildChangedEventArgs() { Child = child, Parent = this });
        }

        public void MoveChild(SChild child, int index)
        {
            //if (children.IndexOf(child) > index)
            //    index--;

            OnMoveChild(child, index);

            if (ChildMoved != null)
                ChildMoved(this, new SChildChangedEventArgs() { Child = child, Parent = this, NewIndex = index });
        }*/

        public virtual new void Clear()
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                var w = children[i];
                var c = w as SContainer;
                if (c != null)
                    c.Clear();
                RemoveChild(w);
                children.RemoveAt(i);
                //w.Dispose();
            }
        }

        public virtual string CanAddChild(SChild child)
        {
            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return children.GetEnumerator();
        }
    }
}
