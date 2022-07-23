using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Game1.AStar;

namespace Game1.NavMesh
{
    public class Vertex : IMapPosition
    {
        public Vector3 position;
        public int index;
        static int indexCounter = 0;


        public Vertex(Vector3 argPosition, int argIndex = -1)
        {
            position = argPosition;
            

            if (argIndex == -1)
            {
                index = indexCounter;
            }
            else
            {
                index = argIndex;
            }

            indexCounter++;


        }



        public float Cost(IMapPosition position)
        {
            if (position == null)
            {
                return 0;
            }

            return Heuristic(this, (Vertex)position);
        }

        public bool Equals(IMapPosition b)
        {
           if (b is Vertex)
           {
                return Equals((Vertex) b);
           }

            return false;

        }

        public bool Equals (Vertex b)
        {
            return (position == b.position);


        }


        public static bool operator == (Vertex v1, Vertex v2)
        {
            if (object.ReferenceEquals(v1, null))
            {
                return object.ReferenceEquals(v2, null);
            }

            else if (object.ReferenceEquals(v2, null))
            {
                return false;
            }

            else
            {
                return v1.position == v2.position;
            }

        }
        public static bool operator != (Vertex v1, Vertex v2)
        {
            return !(v1 == v2);
        }



        public static float Heuristic(Vertex current, Vertex goal)
        {
            return (current.position - goal.position).Length();
        }



        public static implicit operator Vector3(Vertex vertex)
        {
            return vertex.position;
        }

        public static implicit operator Vertex(Vector3 vector)
        {
            return new Vertex(vector);
        }



    }
}
