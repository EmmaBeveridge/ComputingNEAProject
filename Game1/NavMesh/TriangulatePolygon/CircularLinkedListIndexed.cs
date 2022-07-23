using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.NavMesh.TriangulatePolygon
{
    class CircularLinkedListIndexed<T>: LinkedList<T>
    {

        public LinkedListNode<T> this[int index]
        {
            get
            {
                while (index < 0)
                {
                    index = Count + index;
                   
                }

                if (index >= Count)
                {
                    index = index % Count;
                }

                LinkedListNode<T> node = First;
                for (int i = 0; i < index; i++)
                {
                    node = node.Next;
                }

                return node;
            }
        }

        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        public int IndexOf (T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Value.Equals(item))
                {
                    return i;
                }

            }

            return -1;



        }








    }
}
