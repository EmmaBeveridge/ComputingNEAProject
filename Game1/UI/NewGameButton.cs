﻿using Game1.DataClasses;
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


        public NewGameButton(string argLabel, Vector2 argPosition, Texture2D argTexture) : base(argLabel, argPosition, argTexture)
        {


        }


        public static void CreateNewGame()
        {

            SQLiteDBHandler handler = new SQLiteDBHandler();

            handler.CreateTables();
            handler.AddPeople();






        }


    }
}