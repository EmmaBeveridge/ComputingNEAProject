using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    public class EmotionPanel : ToolbarPanel
    {
        Player Player;


        /// <summary>
        /// Constructor for new EmotionPanel object, setting panel scales and positioning. 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="position"></param>
        public EmotionPanel(Player player, Vector2 position)
        {
            Player = player;
            panelPosition.Y = position.Y;
            panelPosition.X = position.X+120;
            panelScale.X -= 40;
            panelScale.Y -= 40;
            
        }

        /// <summary>
        /// Draws string of player’s current emotion 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            //spriteBatch.Draw(texture: whiteRectangle, position: panelPosition, scale: panelScale, layerDepth: 0.5f);
            spriteBatch.DrawString(spriteFont, Player.emotionalState.ToString(), new Vector2(panelPosition.X , panelPosition.Y + 20), Color.Black);



        }

        public override void InitialisePanel()
        {
            
        }
    }
}
