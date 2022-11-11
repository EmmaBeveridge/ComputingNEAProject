﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1.UI
{
    class Textbox
    {
        public string CurrentText;
        public Vector2 CurrentTextPosition;
        public Vector2 CursorPosition;
        public int AnimationTime;
        public bool Visible;
        public Vector2 Position;
        public bool Selected;
        public int CellWidth;
        public int CellHeight;
        private int cursorWidth;
        private int cursorHeight;
        private int length;
        private Texture2D texture;
        private Texture2D cursorTexture;
        private Point cursorDims;
        private SpriteFont font;


        private Stack<int> lineEndXValue = new Stack<int>();


        public bool Submitted { get; private set; }



        public static Texture2D defaultTextboxTexture; 
        public static Texture2D defaultCursorTexture;
        public static int defaultLength = 100;
        public static SpriteFont defaultFont;

        public static int lineLength = 25;
        public static int lineHeight = 5;

        public Textbox(Texture2D _texture, Texture2D _cursorTexture, Point _dims, Point _cursorDims, Vector2 _position, int _length, bool _visible, SpriteFont _font, string _text)
        {
            texture = _texture;
            CellWidth = _dims.X;
            CellHeight = _dims.Y;
            cursorHeight = _cursorDims.Y;
            cursorWidth = _cursorDims.X;
            length = _length;
            AnimationTime = 0;
            Visible = _visible;
            Position = _position;
            CursorPosition = new Vector2(Position.X, Position.Y);
            CurrentTextPosition = new Vector2(Position.X, Position.Y);
            cursorTexture = _cursorTexture;
            cursorDims = _cursorDims;
            Selected = false;
            font = _font;
            CurrentText = _text;
            Submitted = false;
        }

        public Textbox(Vector2 _position)
        {
            texture = defaultTextboxTexture;
            CellWidth = defaultTextboxTexture.Width;
            CellHeight = defaultTextboxTexture.Height;
            cursorTexture = defaultCursorTexture;
            cursorHeight = defaultCursorTexture.Height;
            cursorWidth = defaultCursorTexture.Width;
            length = defaultLength;
            AnimationTime = 0;
            Visible = true;
            Position = _position;
            CursorPosition = new Vector2(Position.X, Position.Y);
            CurrentTextPosition = new Vector2(Position.X, Position.Y);
            Selected = false;
            font = defaultFont;
            CurrentText = string.Empty;
            Submitted = false;
        }


        public void Submit() { Submitted = true; }



        public void Update()
        {
            AnimationTime++;
        }

        public bool IsFlashingCursorVisible()
        {
            int time = AnimationTime % 60;
            if (time >= 0 && time < 31) //cursor flashes every 0.5s
            {
                return true;
            }
            else { return false; }
        }


        public void AddText(char text)
        {
            Vector2 spacing = new Vector2();
            KeyboardState keyboardState = Keyboard.GetState();
            bool isLower = true;

            if (keyboardState.CapsLock || keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
            {
                isLower = false;
            }

            if (text != '\b')
            {
                if (CurrentText.Length < length)
                {
                    if (isLower) { text = Char.ToLower(text); }
                    CurrentText += text;
                    spacing = font.MeasureString(text.ToString());
                    CursorPosition = new Vector2(CursorPosition.X + spacing.X, CursorPosition.Y);
                    if (CurrentText.Length % lineLength == 0)
                    {
                        CurrentText += "\n";
                        lineEndXValue.Push((int)CursorPosition.X);
                        CursorPosition = new Vector2(Position.X, Position.Y + lineHeight);

                    }

                }
                
            }

            else
            {
                if (CurrentText.Length > 0)
                {
                    spacing = font.MeasureString(CurrentText[CurrentText.Length - 1].ToString());

                    if (CurrentText[CurrentText.Length - 1] == '\n') 
                    {
                        CurrentText.Remove(CurrentText.Length-2, 2);
                        CursorPosition = new Vector2(lineEndXValue.Pop(), CursorPosition.Y- lineHeight); //problem with stack underflow
                    }

                    else
                    {
                        CurrentText = CurrentText.Remove(CurrentText.Length - 1, 1);
                        CursorPosition = new Vector2(CursorPosition.X - spacing.X, CursorPosition.Y);

                    }
                    
                }
            }
            


        }

        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(texture, Position, Color.White);
                spriteBatch.DrawString(font, CurrentText, CurrentTextPosition, Color.Black);
                if (Selected && IsFlashingCursorVisible())
                {
                    Rectangle sourceRectangle = new Rectangle(0, 0, cursorWidth, cursorHeight);
                    Rectangle destinationRectangle = new Rectangle((int)CursorPosition.X, (int)CursorPosition.Y, cursorWidth, cursorHeight);
                    spriteBatch.Draw(cursorTexture, destinationRectangle, sourceRectangle, Color.White);

                }
            }
        }
    }
}