using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    class RelationshipBar
    {
        
        static int width = 25;
        static Texture2D whiteRectangle;

        public static void GenerateTexture(GraphicsDevice graphicsDevice)
        {
            whiteRectangle = new Texture2D(graphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

        }

        public static void Draw(SpriteBatch spriteBatch, Vector2 position, float Score )
        {

            float length = (Score - 50) * 10;

            if (length < 0) { position.X = position.X + length; length = -length; }

            spriteBatch.Draw(texture: whiteRectangle, position: position, scale: new Vector2(length, width), color: Score>60? Color.Green : (Score>40 ? Color.Orange : Color.Red), layerDepth: 1f);

        }





    }
}
