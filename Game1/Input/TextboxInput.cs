
using Game1.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    class TextboxKeyboard
    {
        static KeyboardState currentKeyState;
        static KeyboardState previousKeyState;

        /// <summary>
        /// Constructor for new TextboxKeyboard object 
        /// </summary>
        public TextboxKeyboard() { }

        /// <summary>
        /// Sets previousKeyState to currentKeyState and resets and returns currentKeyState to the state of the keyboard. 
        /// </summary>
        /// <returns></returns>
        public static KeyboardState GetState()
        {
            previousKeyState = currentKeyState;
            currentKeyState = Keyboard.GetState();
            return currentKeyState;

        }
        /// <summary>
        /// Returns if the specified key is currently held down. 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns if the specified key was pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasNotBeenPressed(Keys key)
        {
            return currentKeyState.IsKeyDown(key) && !previousKeyState.IsKeyDown(key);
        }

        


    }

    



    class TextboxInputHandler
    {

        static int timeBeforeNextDelete = 10;

        public static void HandleInput(GameTime gameTime, Textbox textbox)
        {
            
            KeyboardState kbs = TextboxKeyboard.GetState();
            MouseState ms = MouseInput.GetState();

            Keys[] keys = kbs.GetPressedKeys();
            string value = string.Empty;

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (MouseInput.HasNotBeenPressed(true)) { HandleLeftMouseClick(textbox); }

            }

            if (kbs.IsKeyUp(Keys.Back) && kbs.IsKeyUp(Keys.Delete))
            {
                timeBeforeNextDelete = 10;
            }

            if (keys.Count() > 0)
            {
                if (keys.Count() > 1)
                {
                    keys[0] = ExtractSingleChar(keys);
                }

                if (kbs.IsKeyDown(Keys.Back) || kbs.IsKeyDown(Keys.Delete))
                {
                    if (timeBeforeNextDelete == 0)
                    {
                        timeBeforeNextDelete = 10;
                    }

                    if (timeBeforeNextDelete == 10)
                    {
                        if (textbox.Selected) { textbox.AddText('\b'); }
                       
                    } 
                    
                    timeBeforeNextDelete--;

                    return;
                    

                }

                if (TextboxKeyboard.HasNotBeenPressed(Keys.Enter))
                {
                    textbox.Submit();
                }

                if (textbox.Selected && (((int)keys[0] >= 48 && (int)keys[0] <= 105)|| keys[0] == Keys.RightShift||keys[0] == Keys.LeftShift||keys[0] == Keys.Space))
                {
                    if (keys[0] == Keys.RightShift || keys[0] == Keys.LeftShift)
                    {
                        if (TextboxKeyboard.HasNotBeenPressed(keys[0])) { return; }
                    }

                    if (TextboxKeyboard.HasNotBeenPressed(keys[0]))
                    {
                        if ((int)keys[0] >= 96 && (int)keys[0] <= 105) //number pad key
                        {
                            value = keys[0].ToString().Substring(keys[0].ToString().Length - 1);
                            textbox.AddText(value.ToCharArray()[0]);
                        }
                        else { textbox.AddText((char)keys[0]); }
                    }


                }




            }



        }

        /// <summary>
        /// Checks if mouse clicked over textbox or not and sets selected property accordingly
        /// </summary>
        /// <param name="textbox"></param>
        private static void HandleLeftMouseClick(Textbox textbox)
        {
            Rectangle textboxRectangle = new Rectangle((int)textbox.Position.X, (int)textbox.Position.Y, textbox.CellWidth, textbox.CellHeight);
            if (MouseInput.MouseOverRectangle(textboxRectangle)) { textbox.Selected = true; }
            else { textbox.Selected = false; }

        }


        /// <summary>
        ///  returns character key from pressed keys. 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private static Keys ExtractSingleChar(Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if((int)key >= 48 && (int)key <= 105)
                {
                    return key;
                }
                
            }
            
            return Keys.None;
        }

    }

    



}
