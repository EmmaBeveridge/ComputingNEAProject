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
    class NewGameButton:Button
    {
        /// <summary>
        ///  string name of character model selected by player in character selection menu to be stored in database 
        /// </summary>
        public static string selectedCharacterName;


        /// <summary>
        /// string names of traits selected by player in trait selection menu to be stored in database 
        /// </summary>
        public static List<string> selectedTraitNames = new List<string>();


        /// <summary>
        /// Constructor for new NewGameButton 
        /// </summary>
        /// <param name="argLabel"></param>
        /// <param name="argPosition"></param>
        /// <param name="argTexture"></param>
        public NewGameButton(string argLabel, Vector2 argPosition, Texture2D argTexture) : base(argLabel, argPosition, argTexture)
        {


        }





        /// <summary>
        /// Creates a new SQLiteDBHandler obect and uses to create database tables and adds people in initial state to tables. 
        /// </summary>
        public static void CreateNewGame()
        {

            SQLiteDBHandler handler = new SQLiteDBHandler();

            handler.CreateTables();
            handler.CreateTraitLookupTable();
            handler.CreateSkillLookupTable();
            handler.AddPeople(selectedCharacterName, selectedTraitNames);






        }


    }
}
