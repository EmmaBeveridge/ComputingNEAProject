using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Game1.NavMesh
{
    class NavMeshVertex
    {
        public List<NavMeshVertex> linkedVertices;
        public Vector3 position;
        public NavMeshVertex nextVertex;
        public NavMeshVertex previousVertex;
        public int edgeIndex = -1;

        public int id;
        public static int idCount; 

        public NavMeshVertex()
        {
            linkedVertices = new List<NavMeshVertex>();
            position = Vector3.Zero;
            id = idCount;
            idCount++;
        }

    }
}
