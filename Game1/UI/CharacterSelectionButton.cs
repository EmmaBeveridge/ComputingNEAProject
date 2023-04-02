﻿using Game1.GOAP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    class CharacterSelectionButton : Button
    {


        /// <summary>
        /// Constructor for new character selection button 
        /// </summary>
        /// <param name="argLabel"></param>
        /// <param name="argPosition"></param>
        /// <param name="argTexture"></param>
        public CharacterSelectionButton(string argLabel, Vector2 argPosition, Texture2D argTexture) : base(argLabel, argPosition, argTexture)
        {




        }

    }
}
