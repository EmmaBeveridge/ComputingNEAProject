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
        public List<Button> characterSelectionButtons;
        public List<Button> traitSelectionButtons;

        public ExitButton exitButton;
        public EmotionButton emotionButton;
        public bool displayTextbox = false;
        Textbox currentTextbox;


       


        public UIHandler()
        {
            buttons = new List<Button>();
            toolbarButtons = new List<ToolbarButton>();
            mainMenuButtons = new List<Button>();
            characterSelectionButtons = new List<Button>();
            traitSelectionButtons = new List<Button>();
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



        public void BuildTraitSelectionButtons(GraphicsDeviceManager graphicsManager, List<Texture2D> traitButtonsTextures, Texture2D selectedCharacter)
        {

            float yPosition = 80;
            int buttonsPerRow = 2;
            float screenWidth = graphicsManager.GraphicsDevice.Viewport.Width - selectedCharacter.Width;

            float xSpacing = (screenWidth - traitButtonsTextures[0].Width * buttonsPerRow) / (buttonsPerRow + 1);
            float ySpacing = traitButtonsTextures.Max(t => t.Height);

            float initialX =  0.5f*xSpacing + selectedCharacter.Width;

            Vector2 position = new Vector2( initialX, yPosition);

            for (int i = 0; i < traitButtonsTextures.Count; i++)
            {
                TraitSelectionButton button = new TraitSelectionButton(null, position, traitButtonsTextures[i]);
                traitSelectionButtons.Add(button);


                if ((i+1) % buttonsPerRow == 0)
                {
                    position = new Vector2(initialX, position.Y + ySpacing);
                }
                else
                {
                    position.X += traitButtonsTextures[i].Width + xSpacing;
                  

                }
               
            }






        }




        public void BuildCharacterSelectionButtons(GraphicsDeviceManager graphicsManager, List<Texture2D> characterTextures)
        {
            
            float yPosition = 80;
            float screenWidth = graphicsManager.GraphicsDevice.Viewport.Width;

            float xSpacing = (screenWidth - characterTextures[0].Width * characterTextures.Count) / (characterTextures.Count + 1);

            Vector2 position = new Vector2(xSpacing, yPosition);

            for (int i = 0; i < characterTextures.Count; i++)
            {
                CharacterSelectionButton button = new CharacterSelectionButton(null, position, characterTextures[i]);
                characterSelectionButtons.Add(button);

                position.X += characterTextures[i].Width + xSpacing;



            }






        }


        public void BuildMainMenuButtons(GraphicsDeviceManager graphicsManager, Texture2D exitButtonTexture, Texture2D resumeGameTexture, Texture2D newGameTexture)
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


        public void BuildEmotionButton(GraphicsDeviceManager graphicsManager, Player player) 
        {
            emotionButton = new EmotionButton(null, new Vector2(10, graphicsManager.GraphicsDevice.Viewport.Height-EmotionButton.PositiveEmotionTexture.Height), EmotionButton.PositiveEmotionTexture, player);
            emotionButton.SetTexture();
        
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
            AddToolbarButton(new CareerButton(null, new Vector2((graphicsManager.GraphicsDevice.Viewport.Width - ToolbarButton.toolbarButtonTextureDim.X), (graphicsManager.GraphicsDevice.Viewport.Height - ToolbarButton.toolbarButtonTextureDim.Y)), player));
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


        /// <summary>
        /// Returns texture of character selected, texture used in game.characterNameDictionary to convert to name to get icon/model, null if none selected
        /// </summary>
        /// <returns></returns>
        public Texture2D HandleCharacterSelection()
        {
            if (!MouseInput.WasLeftClicked())
            {
                return null;
            }

            Button buttonPressed = MouseInput.GetButtonPressed(characterSelectionButtons);

            if (buttonPressed == null) { return null; }

            else { return buttonPressed.buttonTexture; }
            
        }


        public Texture2D HandleTraitSelection()
        {
            if (!MouseInput.WasLeftClicked())
            {
                return null;
            }

            Button buttonPressed = MouseInput.GetButtonPressed(traitSelectionButtons);

            if (buttonPressed == null) { return null; }

            else { return buttonPressed.buttonTexture; }


        }








        public Button HandleMainMenuInput()
        {
            

            if (!MouseInput.WasLeftClicked())
            {
                return null;
            }
            
            Button buttonPressed = MouseInput.GetButtonPressed(mainMenuButtons);

            return buttonPressed;


            //if( buttonPressed == null)
            //{
            //    return;
            //}
            //else if (buttonPressed.GetType() == typeof(ExitButton))
            //{
            //    ExitButton.Exit();
            //}
            //else  if (buttonPressed.GetType() == typeof(NewGameButton))
            //{
            //    //NewGameButton.CreateNewGame();
            //}

            //return true;






        }


        



        public void DrawMainMenuButtons(SpriteBatch spriteBatch)
        {
            foreach (Button mainMenuButton in mainMenuButtons)
            {

                spriteBatch.Draw(mainMenuButton.buttonTexture, mainMenuButton.position);

            }
        }


        public void DrawCharacterSelectionButtons(SpriteBatch spriteBatch)
        {
            foreach (Button characterButton in characterSelectionButtons)
            {

                spriteBatch.Draw(characterButton.buttonTexture, characterButton.position, scale: new Vector2(1f, 1f));

            }
        }



        public void DrawTraitSelectionButtons(SpriteBatch spriteBatch)
        {
            foreach (Button traitButton in traitSelectionButtons)
            {
               

                if (traitButton.isSelected)
                {
                    spriteBatch.Draw(traitButton.buttonTexture, traitButton.position, color: Color.Gray);

                }
                else
                {
                    spriteBatch.Draw(traitButton.buttonTexture, traitButton.position);

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

            spriteBatch.Draw(exitButton.buttonTexture, exitButton.position);
            
            emotionButton.SetTexture();
            spriteBatch.Draw(emotionButton.buttonTexture, emotionButton.position);

            if (emotionButton.panel.IsDisplayed) { emotionButton.panel.Draw(spriteBatch, spriteFont); }

            if (displayTextbox)
            {
                currentTextbox.Draw(spriteBatch);
            }


        }



    }
}
