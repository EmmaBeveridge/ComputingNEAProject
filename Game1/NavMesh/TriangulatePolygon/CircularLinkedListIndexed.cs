using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.NavMesh.TriangulatePolygon
{
    class CircularLinkedListIndexed<T>: LinkedList<T>
    {

        /// <summary>
        /// get method: First brings specified index into range of list indices by repeatedly adding the value of Count (list length) until the index is positive or by calculating the modulus of the index divided by Count. Then the linked list is followed through until the item at the calculated index is returned.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Removes the item at the specified index, using this[] get method to ensure index is within list range. 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            Remove(this[index]);
        }

        /// <summary>
        /// Performs linear search for item and returns index index of item if found, otherwise returns –1. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
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
