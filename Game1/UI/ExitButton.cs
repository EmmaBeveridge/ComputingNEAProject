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

        public static Action Exit;


        //NEED TO DO SOMETHING ABOUT SAVING GAME HERE??
        public ExitButton(string argLabel, Vector2 argPosition):base(argLabel, argPosition, defaultTexture)
        {


        }

        public ExitButton(string argLabel, Vector2 argPosition, Texture2D argTexture) : base(argLabel, argPosition, argTexture)
        {


        }




    }
}
