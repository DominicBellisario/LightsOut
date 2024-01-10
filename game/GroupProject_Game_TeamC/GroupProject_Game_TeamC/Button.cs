using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GroupProject_Game_TeamC
{
    /// <summary>
    /// Delegate for left mouse click
    /// </summary>
    public delegate void ButtonClicked();

    internal class Button
    {
        // Fields

        // Mousestates
        protected MouseState prevMS;

        // Button fields
        private Rectangle buttonRect;
        private Texture2D buttonTexture;

        // Text fields
        private string text;
        private Vector2 textLocation;
        private SpriteFont font;

        // Checks if a button was clicked
        public event ButtonClicked OnButtonClicked;

        /// <summary>
        /// Used to check if the mouse is on the mutton
        /// </summary>
        public Rectangle ButtonRectangle
        {
            get { return buttonRect; }
        }


        /// <summary>
        /// Parameterized constructor for buttons
        /// </summary>
        /// <param name="graphics"> graphics device </param>
        /// <param name="buttonRect"> The buttons position and size </param>
        /// <param name="buttonTexture"> The buttons texture </param>
        /// <param name="text"> The text of the button </param>
        /// <param name="font"> The font </param>
        public Button(GraphicsDevice graphics, Rectangle buttonRect, Texture2D buttonTexture, string text, SpriteFont font)
        {
            this.buttonRect = buttonRect;
            this.buttonTexture = buttonTexture;
            this.text = text;
            this.font = font;

            // Puts the text in the middle of the button
            textLocation = new Vector2(
                (buttonRect.X + buttonRect.Width / 2) - font.MeasureString(text).X / 2,
                (buttonRect.Y + buttonRect.Height / 2) - font.MeasureString(text).Y / 2);
        }

        /// <summary>
        /// Updates and checks if any of the buttons are clicked
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // Checks if a button is being clicked or not
            MouseState mState = Mouse.GetState();
            if (buttonRect.Contains(mState.Position) &&
                mState.LeftButton == ButtonState.Released &&
                prevMS.LeftButton == ButtonState.Pressed)
            {
                if (OnButtonClicked != null)
                {
                    OnButtonClicked();
                }
            }
           
            prevMS = mState;
        }

        /// <summary>
        /// Draws the buttons and their text
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draws the button
            spriteBatch.Draw(buttonTexture, buttonRect, Color.Black);

            // Draws the text in the button
            spriteBatch.DrawString(font, text, textLocation, Color.White);
        }
    }
}
