using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    public abstract class ToolbarPanel
    {

        private bool isDisplayed = false;

        public bool IsDisplayed
        {
            get { return isDisplayed; }
            set { if (value == true) { InitialisePanel(); } isDisplayed = value; }
        }

        public abstract void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont);

        static protected Texture2D whiteRectangle;

        protected Vector2 panelPosition = new Vector2(545, 230);
        protected Vector2 panelScale = new Vector2(400, 200);

        public static void GenerateTexture(GraphicsDevice graphicsDevice)
        {
            whiteRectangle = new Texture2D(graphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

        }

        public abstract void InitialisePanel();
    }



    public class NeedsPanel : ToolbarPanel
    {





        List<NeedBar> needsBars;


        public NeedsPanel(List<NeedBar> _bars)
        {
            needsBars = _bars;

        }



        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {

            spriteBatch.Draw(texture: whiteRectangle, position: panelPosition, scale: panelScale, layerDepth: 0.5f);

            foreach (NeedBar bar in needsBars)
            {
                bar.Draw(spriteBatch, spriteFont);
            }
        }

        public override void InitialisePanel()
        {

        }
    }


    public class RelationshipsPanel : ToolbarPanel
    {
        Dictionary<People, int> relationships;
        List<List<KeyValuePair<People, int>>> screens = new List<List<KeyValuePair<People, int>>>();
        public int currentScreen = 0;
        

        public RelationshipsPanel(Dictionary<People, int> argRelationships)
        {
            relationships = argRelationships;
            panelPosition.X = 450;
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(texture: whiteRectangle, position: panelPosition, scale: panelScale, layerDepth: 0.5f);
            
            if (screens.Count >= 1)
            {
                for (int i = 0; i < screens[currentScreen].Count; i++)
                {
                    KeyValuePair<People, int> relationship = screens[currentScreen][i];

                    float X = panelPosition.X + i * relationship.Key.icon.Width + (i + 1) * 20;

                    spriteBatch.Draw(texture: relationship.Key.icon,  scale: new Vector2(1.2f, 1.5f), position: new Vector2(X, panelPosition.Y + 20));

                    RelationshipBar.Draw(spriteBatch, new Vector2(X + relationship.Key.icon.Width / 2, panelPosition.Y + 25 + relationship.Key.icon.Height), relationship.Value);
                }

            }
            else
            {
                spriteBatch.DrawString(spriteFont, "Go get some friends, loser", new Vector2(panelPosition.X + 15, panelPosition.Y + 85), Color.Black);
            }

            
        }

        public void SplitIntoScreens()
        {
            screens.Clear();

            for (int i = 0; i < (relationships.Count / 3); i++)
            {
                List<KeyValuePair<People, int>> screen = new List<KeyValuePair<People, int>>();

                screen.Add(relationships.ElementAt(i));
                screen.Add(relationships.ElementAt(i + 1));
                screen.Add(relationships.ElementAt(i + 2));
                screens.Add(screen);
            }
            if (relationships.Count % 3 != 0)
            {
                List<KeyValuePair<People, int>> finalScreen = new List<KeyValuePair<People, int>>();
                finalScreen.Add(relationships.ElementAt(relationships.Count - 1));

                if ((relationships.Count - 1) % 3 != 0)
                {
                    finalScreen.Add(relationships.ElementAt(relationships.Count - 2));
                }

                screens.Add(finalScreen);

            }





        }

        public override void InitialisePanel()
        {
            SplitIntoScreens();
        }
    }

    public class CareerPanel : ToolbarPanel
    {
        public CareerPanel() { }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            throw new NotImplementedException();
        }

        public override void InitialisePanel()
        {

        }
    }
    public class SkillsPanel : ToolbarPanel
    {
        public SkillsPanel() { }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            throw new NotImplementedException();
        }

        public override void InitialisePanel()
        {

        }
    }


}

