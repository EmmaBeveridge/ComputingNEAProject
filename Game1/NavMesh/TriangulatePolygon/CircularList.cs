using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.NavMesh.TriangulatePolygon
{
    class CircularList<T>:List<T>
    {

        /// <summary>
        /// get method: First brings specified index into range of list indices by repeatedly adding the value of Count (list length) until the index is positive or by calculating the modulus of the index divided by Count. The method then returns the item at the in-range index in the list. 
        /// set method: Brings the supplied index into the range of the list and will then assign the value at the in-range index. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new T this[int index]
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
                return base[index];


            }

            set
            {

                while (index < 0)
                {
                    index = Count + index;
                }
                if (index >= Count)
                {
                    index = index % Count;
                }
                base[index] = value;

            }

        }

        /// <summary>
        /// Removes the item at the specified index, using this[] get method to ensure index is within list range. 
        /// </summary>
        /// <param name="index"></param>
        public new void RemoveAt (int index)
        {
            Remove(this[index]);
        }



        /// <summary>
        /// Constructor for new circular list object 
        /// </summary>
        public CircularList() { }

        public CircularList(IEnumerable<T> collection) : base(collection)
        {

        }

       



    }
}
