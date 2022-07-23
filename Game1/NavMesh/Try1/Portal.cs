using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace Game1.NavMesh
{
    class Portal
    {
        public NavMeshVertex vertex1;
        public NavMeshVertex vertex2;
        public NavMeshVertex centre;

        public int searched = 0;

        public Portal(NavMeshVertex argVertex1, NavMeshVertex argVertex2)
        {
            vertex1 = argVertex1;
            vertex2 = argVertex2;

            centre = new NavMeshVertex();
            centre.position = (vertex1.position = vertex2.position) * 0.5f;
        }





    }
}
