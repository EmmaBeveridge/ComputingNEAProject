using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Game1
{
    public class Avatar
    {
       
        
        public Model model;
        public Matrix worldMatrix;
        public int id;
        static int idCount;



        /// <summary>
        /// Constructor for Avatar object. Sets worldMatrix as matrix encoding translation to _position argument. Sets model and id, increments static idCount.
        /// </summary>
        /// <param name="_model"></param>
        /// <param name="_position"></param>
        public Avatar (Model _model, Vector3 _position)
        {
            model = _model;
            worldMatrix = Matrix.CreateTranslation(_position);
            id = idCount;
            idCount++;
        }


        /// <summary>
        /// Uses model’s meshes to determine a minimum and maximum vector to be used to build a BoundingBox object for the avatar. 
        /// </summary>
        /// <returns></returns>
        public BoundingBox UpdateBoundingBox()
        {
            Vector3 minVector = new Vector3(float.MaxValue);
            Vector3 maxVector = new Vector3(float.MinValue);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    for (int i = 0; i < (vertexBufferSize / sizeof(float)); i += (vertexStride / sizeof(float)))
                    {

                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldMatrix);

                        minVector = Vector3.Min(minVector, transformedPosition);
                        maxVector = Vector3.Max(maxVector, transformedPosition);
                    }
                    




                }

            }


            return new BoundingBox(minVector, maxVector);

        }



        /// <summary>
        /// Iterates over meshes in 3D model, applying view, world, projection matrices to each effect in the mesh’s effect. It then draws the model.  
        /// </summary>
        /// <param name="viewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting(); //causing problem with rendering exterior walls
                    effect.PreferPerPixelLighting = true;
                    effect.World = worldMatrix;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }

                mesh.Draw();

            }


        }

    }
}
