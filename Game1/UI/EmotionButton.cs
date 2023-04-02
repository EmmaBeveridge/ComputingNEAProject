using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    public class EmotionButton:Button
    {
        public static Texture2D PositiveEmotionTexture;
        public static Texture2D NegativeEmotionTexture;

        public static List<PeopleEmotionalState> positiveEmotions = new List<PeopleEmotionalState> { PeopleEmotionalState.Comfortable, PeopleEmotionalState.Playful, PeopleEmotionalState.Energised };
        public static List<PeopleEmotionalState> negativeEmotions = new List<PeopleEmotionalState> { PeopleEmotionalState.Uncomfortable, PeopleEmotionalState.Tense, PeopleEmotionalState.Lonely };

        public EmotionPanel panel;


        public Player Player;

        /// <summary>
        /// Constructor for new EmotionButton 
        /// </summary>
        /// <param name="argLabel"></param>
        /// <param name="argPosition"></param>
        /// <param name="argTexture"></param>
        /// <param name="player"></param>
        public EmotionButton(string argLabel, Vector2 argPosition, Texture2D argTexture, Player player) : base(argLabel, argPosition, argTexture)
        {
            Player = player;
            panel = new EmotionPanel(Player, argPosition);

        }


        /// <summary>
        /// Establishes if player’s current emotion is positive or negative and assigns button texture accordingly 
        /// </summary>
        public void SetTexture()
        {
            if (positiveEmotions.Contains(Player.emotionalState)) { this.buttonTexture = PositiveEmotionTexture; }
            else { this.buttonTexture = NegativeEmotionTexture; }

        }
        


    }
}
