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

        /// <summary>
        /// Constructor for new vertex object, supplied with position of vertex. Unique index assigned to vertex index attribute if no index supplied in parameter. 
        /// </summary>
        /// <param name="argPosition"></param>
        /// <param name="argIndex"></param>
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


        /// <summary>
        /// Returns heuristic cost of moving from position supplied in parameter to position of vertex. 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public float Cost(IMapPosition position)
        {
            if (position == null)
            {
                return 0;
            }

            return Heuristic(this, (Vertex)position);
        }


        /// <summary>
        /// Overloaded method returning if the position the object supplied in parameter is equal to the position of the vertex 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Equals(IMapPosition b)
        {
           if (b is Vertex)
           {
                return Equals((Vertex) b);
           }

            return false;

        }

        /// <summary>
        /// Overloaded method returning if the position the object supplied in parameter is equal to the position of the vertex 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Equals (Vertex b)
        {
            return (position == b.position);


        }

        /// <summary>
        ///  Returns if positions of vertex parameters are equal. Handles null arguments by returning true if both verticies are null, otherwise returning false. 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Returns Euclidean distance as heuristic for A* pathfinding.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static float Heuristic(Vertex current, Vertex goal)
        {
            return (current.position - goal.position).Length();
        }


        /// <summary>
        /// Allows implicit conversion of vertex to Vector3 object. Returns vertex position when vertex object is used as a Vector3 type object.
        /// </summary>
        /// <param name="vertex"></param>
        public static implicit operator Vector3(Vertex vertex)
        {
            return vertex.position;
        }


        /// <summary>
        /// Allows implicit conversion of Vector3 to vertex object. Returns new vertex object with position of Vector3 parameter.
        /// </summary>
        /// <param name="vector"></param>
        public static implicit operator Vertex(Vector3 vector)
        {
            return new Vertex(vector);
        }



    }
}
