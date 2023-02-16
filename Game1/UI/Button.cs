using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.GOAP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1.UI
{
    public class Button
    {
        public Texture2D buttonTexture;
        public string buttonLabel;
        public Vector2 position;
        public int id;
        static int idCount=0;
        public static Texture2D defaultTexture;
        public static readonly float circleRadius = 200f;
        public GOAPAction buttonAction;


        public bool isSelected = false;

        public Button( string argLabel, Vector2 argPosition, Texture2D argTexture)
        {
            buttonLabel = argLabel;
            position = argPosition;
            buttonTexture = argTexture;
            id = idCount;
            idCount++;

        }

        public Button(string argLabel, GOAPAction argAction, Vector2 argPosition)
        {
            buttonLabel = argLabel;
            buttonAction = argAction;
            position = argPosition;
            buttonTexture = defaultTexture;
            id = idCount;
            idCount++;
        }



    }
}
