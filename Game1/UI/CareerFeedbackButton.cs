using Game1.GOAP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    /// <summary>
    /// Enumeration to store possible career feedback sentiments.
    /// </summary>
    public enum FeedbackScore 
    { 
        Good,
        Bad,
        Average

    }



    public class CareerFeedbackButton : Button
    {

        /// <summary>
        /// Constructor for new CareerFeedbackButton. Sets buttonLabel appropriately given a FeedbackScore parameter.
        /// </summary>
        /// <param name="feedback"></param>
        /// <param name="argPosition"></param>
        public CareerFeedbackButton(FeedbackScore feedback, Vector2 argPosition) : base(argPosition)
        {
            switch (feedback)
            {
                case FeedbackScore.Good:
                    buttonLabel = "You have had a good day at work.\nGreat job!";
                    break;
                case FeedbackScore.Bad:
                    buttonLabel = "It's been a bad day at work.\nHopefully tomorrow is better.";
                    break;
                case FeedbackScore.Average:
                    buttonLabel = "Today was fine.\nNot great but not bad.";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Draws button texture and career feedback text using SpriteBatch and SpriteFont parameters.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(this.buttonTexture, this.position);


            spriteBatch.DrawString(spriteFont, this.buttonLabel, new Vector2(this.position.X + this.buttonTexture.Width / 20, this.position.Y + this.buttonTexture.Height / 20), Color.Black);

        }


    }
}
