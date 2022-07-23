using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Game1.NavMesh
{
    class Pylon
    {

        public BoundingSphere boundingSphere;
        public Vector3 position;
        public Vector3 scale;
        public float radius;
        public BoundingBox boundingBox;



        public Pylon (float argRadius)
        {
            position = Vector3.Zero;
            scale = Vector3.One;
            radius = argRadius;
            boundingSphere = new BoundingSphere(position, radius);
            boundingBox = new BoundingBox();
        }

        public void Update (GameTime gameTime)
        {
            position = (Vector3.UnitX + Vector3.UnitZ) * position + Vector3.UnitY * scale.Y * 0.5f;
            boundingSphere.Center = position;
            boundingSphere.Radius = radius;
            boundingBox.Min = position;
            boundingBox.Max = position + scale;

        }




    }
}
