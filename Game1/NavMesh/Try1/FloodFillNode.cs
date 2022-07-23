using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.NavMesh
{

   

    class FloodFillNode
    { 
        public BoundingBox boundingBox;
        public Vector3 position;
        public static float size = 1.00f;

        public List<FloodFillNode> neighbours;
        public List<NavMeshVertex> corners;

        public FloodFillNode()
        {
            boundingBox = new BoundingBox();
            neighbours = new List<FloodFillNode>();
            position = Vector3.Zero;


            corners = new List<NavMeshVertex>();
            corners.Add(new NavMeshVertex());
            corners.Add(new NavMeshVertex());
            corners.Add(new NavMeshVertex());
            corners.Add(new NavMeshVertex());

            Update();


        }

        public void Update()
        {
            boundingBox.Min = position - (Vector3.UnitX * size * 0.420f) - (Vector3.UnitY * 0.1f) - (Vector3.UnitZ * size * 0.420f);
            boundingBox.Max = position + (Vector3.UnitX * size * 0.420f) + (Vector3.UnitY * 0.1f) + (Vector3.UnitZ * size * 0.420f);

            corners[0].position = position - (Vector3.UnitX * size * 0.5f) + (Vector3.UnitY * 0.1f) - (Vector3.UnitZ * size * 0.5f);
            corners[1].position = position + (Vector3.UnitX * size * 0.5f) + (Vector3.UnitY * 0.1f) - (Vector3.UnitZ * size * 0.5f);
            corners[2].position = position + (Vector3.UnitX * size * 0.5f) + (Vector3.UnitY * 0.1f) + (Vector3.UnitZ * size * 0.5f);
            corners[3].position = position - (Vector3.UnitX * size * 0.5f) + (Vector3.UnitY * 0.1f) + (Vector3.UnitZ * size * 0.5f);
        }
    }
}

     