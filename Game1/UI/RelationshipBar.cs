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
        /// <summary>
        /// Creates white rectangle to be scaled and coloured depending on relationship score. 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public static void GenerateTexture(GraphicsDevice graphicsDevice)
        {
            whiteRectangle = new Texture2D(graphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

        }

        /// <summary>
        /// Draws relationship bar where length and colour of bar represents relationship score.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        /// <param name="Score"></param>
        public static void Draw(SpriteBatch spriteBatch, Vector2 position, float Score )
        {

            float length = (Score - 50) * 10;

            if (length < 0) { position.X = position.X + length; length = -length; }

            spriteBatch.Draw(texture: whiteRectangle, position: position, scale: new Vector2(length, width), color: Score>60? Color.Green : (Score>40 ? Color.Orange : Color.Red), layerDepth: 1f);

        }





    }
}
