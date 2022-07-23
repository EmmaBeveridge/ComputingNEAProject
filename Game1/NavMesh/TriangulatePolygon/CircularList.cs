using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.NavMesh.TriangulatePolygon
{
    class CircularList<T>:List<T>
    {
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
        
        
        public new void RemoveAt (int index)
        {
            Remove(this[index]);
        }




        public CircularList() { }

        public CircularList(IEnumerable<T> collection) : base(collection)
        {

        }

       



    }
}
