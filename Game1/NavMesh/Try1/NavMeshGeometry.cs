using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.NavMesh
{
    class NavMeshGeometry
    {
        public Vector3 position;
        public Vector3 scale;

        public BoundingBox boundingBox;

        public NavMeshGeometry(Vector3 argPosition, Vector3 argScale)
        {
            position = argPosition;
            scale = argScale;
            boundingBox = new BoundingBox(position, position + scale);

        }




    }
}
