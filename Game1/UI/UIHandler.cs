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
    public class UIHandler
    {

        public List<Button> buttons;
        public List<ToolbarButton> toolbarButtons;

        public List<Button> mainMenuButtons;

        public ExitButton exitButton;
        public bool displayTextbox = false;
        Textbox currentTextbox;


       


        public UIHandler()
        {
            buttons = new List<Button>();
            toolbarButtons = new List<ToolbarButton>();
            mainMenuButtons = new List<Button>();
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


        public void BuildMainMenuButton(GraphicsDeviceManager graphicsManager, Texture2D exitButtonTexture, Texture2D resumeGameTexture, Texture2D newGameTexture)
        {

            float screenWidth = graphicsManager.GraphicsDevice.Viewport.Width;
            float screenHeight= graphicsManager.GraphicsDevice.Viewport.Height;

            

            
            if (SQLiteDBHandler.DBExists())
            {
                //mainMenuButtons.Add(new ResumeGameButton(null, new Vector2(screenWidth / 2 - resumeGameTexture.Width / 2, 2 * screenHeight / 3), resumeGameTexture));
                mainMenuButtons.Add(new ResumeGameButton(null, new Vector2(screenWidth / 4 , screenHeight / 4), resumeGameTexture));
            }

            mainMenuButtons.Add(new NewGameButton(null, new Vector2(screenWidth / 80, ( screenHeight / 1.8f) ), newGameTexture));



            mainMenuButtons.Add(new ExitButton(null, new Vector2((screenWidth / 2), (screenHeight / 1.8f)), exitButtonTexture));





        }
       



        public void BuildExitButton(GraphicsDeviceManager graphicsManager)
        {
            exitButton = new ExitButton(null, new Vector2(graphicsManager.GraphicsDevice.Viewport.Width - ExitButton.defaultTexture.Width, 0));
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



        public void HandleCharacterSelection()
        {
            throw new NotImplementedException();
        }


        public bool HandleMainMenuInput()
        {
            

            if (!MouseInput.WasLeftClicked())
            {
                return false;
            }
            
            Button buttonPressed = MouseInput.GetButtonPressed(mainMenuButtons);

            if( buttonPressed == null)
            {
                return false;
            }
            else if (buttonPressed.GetType() == typeof(ExitButton))
            {
                ExitButton.Exit();
            }
            else  if (buttonPressed.GetType() == typeof(NewGameButton))
            {
                NewGameButton.CreateNewGame();
            }

            return true;






        }


        



        public void DrawMainMenuButtons(SpriteBatch spriteBatch)
        {
            foreach (Button mainMenuButton in mainMenuButtons)
            {

                spriteBatch.Draw(mainMenuButton.buttonTexture, mainMenuButton.position);

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

            spriteBatch.Draw(exitButton.buttonTexture, exitButton.position);

            if (displayTextbox)
            {
                currentTextbox.Draw(spriteBatch);
            }


        }



    }
}
