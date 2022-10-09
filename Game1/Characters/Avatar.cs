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



        public Avatar (Model _model, Vector3 _position)
        {
            model = _model;
            worldMatrix = Matrix.CreateTranslation(_position);
            id = idCount;
            idCount++;
        }


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
