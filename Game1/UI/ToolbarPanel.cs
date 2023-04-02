using Game1.Careers;
using Game1.Skills;
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

        /// <summary>
        /// Accessor for private isDisplayed attribute. Mutator for attribute calls InitialisePanel method first if setting isDisplayed to true, then modifies attribute. 
        /// </summary>
        public bool IsDisplayed
        {
            get { return isDisplayed; }
            set { if (value == true) { InitialisePanel(); } isDisplayed = value; }
        }


        /// <summary>
        /// Abstract method to be implemented in child classes.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
        public abstract void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont);

        static protected Texture2D whiteRectangle;

        protected Vector2 panelPosition = new Vector2(545, 230);
        protected Vector2 panelScale = new Vector2(400, 200);

        /// <summary>
        ///  Creates white rectangle to be scaled as background for derived panel classes. 
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public static void GenerateTexture(GraphicsDevice graphicsDevice)
        {
            whiteRectangle = new Texture2D(graphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });

        }


        /// <summary>
        ///  Abstract method to be implemented in child classes. 
        /// </summary>
        public abstract void InitialisePanel();
    }



    public class NeedsPanel : ToolbarPanel
    {





        List<NeedBar> needsBars;

        /// <summary>
        /// Constructor for new NeedsPanel object, supplied with list of needs bars as parameter, setting panel scales and positioning. 
        /// </summary>
        /// <param name="_bars"></param>
        public NeedsPanel(List<NeedBar> _bars)
        {
            needsBars = _bars;
            panelPosition.Y -= 100;
            panelPosition.X -= 15;
            panelScale.Y += 100;
        }


        /// <summary>
        ///  Draws panel background and calls draw method on each need bar. 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
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
        Dictionary<People, float> relationships;
        List<List<KeyValuePair<People, float>>> screens = new List<List<KeyValuePair<People, float>>>();
        public int currentScreen = 0;

        /// <summary>
        ///  Constructor for new RelationshipsPanel object, supplied with dictionary of relationships as parameter, setting panel scales and positioning. 
        /// </summary>
        /// <param name="argRelationships"></param>
        public RelationshipsPanel(Dictionary<People, float> argRelationships)
        {
            relationships = argRelationships;
            panelPosition.X = 450;
        }

        /// <summary>
        ///  Draws panel background and current screen of relationships. Relationships depicted by an icon of the person and a bar displaying degree of postive/negative relationship.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(texture: whiteRectangle, position: panelPosition, scale: panelScale, layerDepth: 0.5f);
            
            if (screens.Count >= 1)
            {
                for (int i = 0; i < screens[currentScreen].Count; i++)
                {
                    KeyValuePair<People, float> relationship = screens[currentScreen][i];

                    float X = panelPosition.X + i * relationship.Key.icon.Width + (i + 1) * 20;

                    spriteBatch.Draw(texture: relationship.Key.icon,  scale: new Vector2(1f, 1f), position: new Vector2(X, panelPosition.Y + 20));

                    RelationshipBar.Draw(spriteBatch, new Vector2(X + relationship.Key.icon.Width / 2, panelPosition.Y + 25 + relationship.Key.icon.Height), relationship.Value);
                }

            }
            else
            {
                spriteBatch.DrawString(spriteFont, "Go get some friends, loser", new Vector2(panelPosition.X + 15, panelPosition.Y + 85), Color.Black);
            }

            
        }


        /// <summary>
        /// divides relationships into screens to display in panel. 
        /// </summary>
        public void SplitIntoScreens()
        {
            screens.Clear();

            for (int i = 0; i < (relationships.Count / 3); i++)
            {
                List<KeyValuePair<People, float>> screen = new List<KeyValuePair<People, float>>();

                screen.Add(relationships.ElementAt(i));
                screen.Add(relationships.ElementAt(i + 1));
                screen.Add(relationships.ElementAt(i + 2));
                screens.Add(screen);
            }
            if (relationships.Count % 3 != 0)
            {
                List<KeyValuePair<People, float>> finalScreen = new List<KeyValuePair<People, float>>();
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
        Player player;

        /// <summary>
        /// Constructor for new CareerPanel object.
        /// </summary>
        /// <param name="argPlayer"></param>
        public CareerPanel(Player argPlayer) { player = argPlayer; panelPosition.X = 400; }

        /// <summary>
        /// Draws panel background and career information 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(texture: whiteRectangle, position: panelPosition, scale: panelScale, layerDepth: 0.5f);


            if (player.Career == null) { spriteBatch.DrawString(spriteFont, "You are unemployed", new Vector2(panelPosition.X + 15, panelPosition.Y + 85), Color.Black); }
            else { spriteBatch.DrawString(spriteFont, player.Career.CareerName, new Vector2(panelPosition.X + 15, panelPosition.Y + 15), Color.Black); 
                spriteBatch.DrawString(spriteFont, player.Career.CareerDescription, new Vector2(panelPosition.X + 15, panelPosition.Y + 85), Color.Black); }
        }




        public override void InitialisePanel()
        {

        }
    }
    public class SkillsPanel : ToolbarPanel
    {
        Player player;

        /// <summary>
        ///  Constructor for new SkillsPanel object.
        /// </summary>
        /// <param name="argPlayer"></param>
        public SkillsPanel(Player argPlayer) { player = argPlayer; panelPosition.X = 450; }

        /// <summary>
        /// Draws panel background and each skill player has and level of skill.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="spriteFont"></param>
        public override void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(texture: whiteRectangle, position: panelPosition, scale: panelScale, layerDepth: 0.5f);

            if (player.Skills.Count() == 0) {
                spriteBatch.DrawString(spriteFont, "You have no skills", new Vector2(panelPosition.X + 15, panelPosition.Y + 85), Color.Black);

            }
            else
            {
                float yHeight = panelPosition.Y + 15;
                foreach (Skill skill in player.Skills)
                {
                    spriteBatch.DrawString(spriteFont, skill.GetSkillString(), new Vector2(panelPosition.X + 15, yHeight), Color.Black);
                    spriteBatch.DrawString(spriteFont, skill.Level.ToString(), new Vector2(panelPosition.X + 200, yHeight), Color.Black);


                    

                    yHeight += 30;

                }

            }

            


        }

        public override void InitialisePanel()
        {

        }
    }


}

