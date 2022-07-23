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

        
        Model model;
        public Matrix worldMatrix;





        public Avatar (Model _model, Vector3 _position)
        {
            model = _model;
            worldMatrix = Matrix.CreateTranslation(_position);
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
