using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
 * Rat
 * Used for the tutorial and as an easter egg in a level
 */
namespace GroupProject_Game_TeamC
{
    internal class Rat
    {
        private Rectangle ratRectangle;

        //Rat textures
        private Texture2D visible;
        private Texture2D monster;
        private Texture2D currentTexture;

        string signText;

        //text font
        private SpriteFont defaultFont;


        private bool isStunned;

        public Rectangle RatRectangle
        {
            get { return ratRectangle; }
            set { ratRectangle = value; }
        }

        public int RectX
        {
            get { return ratRectangle.X; }
            set { ratRectangle.X = value; }
        }

        public int RectY
        {
            get { return ratRectangle.Y; }
            set { ratRectangle.Y = value; }
        }

        public string SignText
        {
            get { return signText; }
            set { signText = value; }
        }

        public bool IsStunned
        {
            get { return isStunned; }
            set { isStunned = true; }
        }

        /// <summary>
        /// Sets Rat's texture to the visible texture
        /// </summary>
        public void RevealRat()
        {
            currentTexture = visible;
        }

        /// <summary>
        /// Rat Constructor
        /// </summary>
        /// <param name="ratRectangle"></param>
        /// <param name="visible"></param>
        /// <param name="monster"></param>
        /// <param name="signText"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="defaultFont"></param>
        public Rat(Rectangle ratRectangle, Texture2D visible, Texture2D monster, string signText, SpriteFont defaultFont)
        {
            this.ratRectangle = ratRectangle;

            this.visible = visible;
            this.monster = monster;
            currentTexture = monster;

            this.signText = signText;
            this.defaultFont = defaultFont;


            isStunned = false;
        }

       /// <summary>
       /// Draws the Rat and it's associated text
       /// </summary>
       /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(currentTexture, ratRectangle, Color.White);
            sb.DrawString(defaultFont, signText, new Vector2(ratRectangle.X + ratRectangle.Width + 30, ratRectangle.Y  - 40), Color.White) ;
        }
    }
}
