using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.UI
{
    public class UIHandler
    {

        public List<Button> buttons;
        public List<ToolbarButton> toolbarButtons;
        public bool displayTextbox = false;
        Textbox currentTextbox;


       


        public UIHandler()
        {
            buttons = new List<Button>();
            toolbarButtons = new List<ToolbarButton>();
        }


        public void ClearButtons()
        {
            buttons.Clear();
        }


        public void AddRangeButtons(List<Button> _buttons)
        {
            buttons.AddRange(_buttons);
        }


        public void AddToolbarButton(ToolbarButton toolbarButton)
        {
            toolbarButtons.Add(toolbarButton);
        }


        public void ClosePanels()
        {
            foreach (ToolbarButton button in toolbarButtons)
            {
                button.panel.IsDisplayed = false;
            }
        }

        public void BuildToolbarButtons(GraphicsDeviceManager graphicsManager, Player player)
        {


            List<NeedBar> needBars = new List<NeedBar>(); 
            
            foreach (Need need in player.Needs.Values.ToList())
            {
               
                needBars.Add(need.NeedBar);
            }


            AddToolbarButton(new NeedsButton(null, new Vector2((graphicsManager.GraphicsDevice.Viewport.Width - 4 * ToolbarButton.toolbarButtonTextureDim.X), (graphicsManager.GraphicsDevice.Viewport.Height - ToolbarButton.toolbarButtonTextureDim.Y)), needBars));

            AddToolbarButton(new RelationshipsButton(null, new Vector2((graphicsManager.GraphicsDevice.Viewport.Width - 3 * ToolbarButton.toolbarButtonTextureDim.X), (graphicsManager.GraphicsDevice.Viewport.Height - ToolbarButton.toolbarButtonTextureDim.Y)), player.Relationships));
            AddToolbarButton(new SkillsButton(null, new Vector2((graphicsManager.GraphicsDevice.Viewport.Width - 2 * ToolbarButton.toolbarButtonTextureDim.X), (graphicsManager.GraphicsDevice.Viewport.Height - ToolbarButton.toolbarButtonTextureDim.Y))));
            AddToolbarButton(new CareerButton(null, new Vector2((graphicsManager.GraphicsDevice.Viewport.Width - ToolbarButton.toolbarButtonTextureDim.X), (graphicsManager.GraphicsDevice.Viewport.Height - ToolbarButton.toolbarButtonTextureDim.Y))));
        }


        public void Update(GameTime gameTime, GraphicsDeviceManager graphicsManager, Player player )
        {
            if (displayTextbox)
            {
                if (currentTextbox == null)
                {
                    currentTextbox = new Textbox(new Vector2(graphicsManager.GraphicsDevice.Viewport.Width / 2 - Textbox.defaultTextboxTexture.Width / 2, graphicsManager.GraphicsDevice.Viewport.Height / 3));

                }
                currentTextbox.Update();
                TextboxInputHandler.HandleInput(gameTime, currentTextbox);

                if (currentTextbox.Submitted)
                {
                    player.ReceiveConversationData(currentTextbox.CurrentText);
                    displayTextbox = false;
                    currentTextbox = null;

                }




            }
        }



        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {

            foreach (Button button in buttons)
            {

                spriteBatch.Draw(button.buttonTexture, button.position);

                if (button.buttonLabel != null)
                {
                    spriteBatch.DrawString(spriteFont, button.buttonLabel, new Vector2(button.position.X + button.buttonTexture.Width / 4, button.position.Y + button.buttonTexture.Height / 20), Color.Black);

                }

            }

            foreach (ToolbarButton button in toolbarButtons)
            {
                spriteBatch.Draw(button.buttonTexture, button.position, layerDepth: 1f);

                if (button.panel.IsDisplayed)
                {
                    button.panel.Draw(spriteBatch, spriteFont);
                }


            }

            if (displayTextbox)
            {
                currentTextbox.Draw(spriteBatch);
            }


        }



    }
}
