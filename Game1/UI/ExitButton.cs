using Game1.DataClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    public class ExitButton:Button
    {

        public static Texture2D defaultTexture;

        
        
        public ExitButton(string argLabel, Vector2 argPosition):base(argLabel, argPosition, defaultTexture)
        {


        }

        public ExitButton(string argLabel, Vector2 argPosition, Texture2D argTexture) : base(argLabel, argPosition, argTexture)
        {


        }

        /// <summary>
        /// Handles saving and exiting of game using SQLiteDBHandler.SaveGame
        /// </summary>
        /// <param name="people">List of people to save state for</param>
        public static void SaveAndExit(List<People> people, Game1 game)
        {
            SQLiteDBHandler handler = new SQLiteDBHandler();

            handler.SaveGame(people);
            Exit(game);
            


        }


        public static void Exit(Game1 game)
        {
            game.Exit();
        }


    }
}
