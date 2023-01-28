using IronXL.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    public class NeedBar
    {



        static float HungerBarHeight = 150;
        static float ToiletBarHeight = 250;
        static float SleepBarHeight = 300;
        static float HygieneBarHeight = 350;
        static float FunBarHeight = 400;
        static float SocialBarHeight = 200;
        static int BarX = 640;

        
        static int width = 25;
        static Texture2D whiteRectangle;

        private Need need;
        private Vector2 position;
        


        public static void GenerateTexture(GraphicsDevice graphicsDevice)
        {
            whiteRectangle = new Texture2D(graphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });
            
        }



        public NeedBar(Need _need)
        {
            need = _need;
            position = new Vector2(BarX, 0);
            switch (need.Name)
            {
                case NeedNames.Hunger:
                    position.Y = HungerBarHeight;
                    break;
                case NeedNames.Sleep:
                    position.Y = SleepBarHeight;
                    break;
                case NeedNames.Toilet:
                    position.Y = ToiletBarHeight;
                    break;
                case NeedNames.Social:
                    position.Y = SocialBarHeight;
                    break;
                case NeedNames.Hygiene:
                    position.Y = HygieneBarHeight;
                    break;
                case NeedNames.Fun:
                    position.Y = FunBarHeight;
                    break;
            }

        }


        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.DrawString(spriteFont, need.Name.ToString(), new Vector2(position.X-100, position.Y-1), Color.Black);


            spriteBatch.Draw(texture: whiteRectangle, position: position, scale: new Vector2(need.Percent*150, width), color: need.Level == NeedLevel.Low ? Color.Red : (need.Level == NeedLevel.Mid ? Color.Orange : Color.Green), layerDepth:1f);
            
        }








    }
}
