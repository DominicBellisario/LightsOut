using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
 * House
 * Object used to represent the level and its art.
 * Contains a position and a texture.
 */
namespace GroupProject_Game_TeamC
{
    internal class House
    {
        private Rectangle rect;
        private Texture2D asset;

        /// <summary>
        /// returns the House object's Rectangle
        /// </summary>
        public Rectangle Rect
        {
            get { return rect; }
        }

        /// <summary>
        /// returns and sets the House's Rectangle's X position
        /// </summary>
        public int RectX
        {
            get { return rect.X; }
            set { rect.X = value; }
        }

        /// <summary>
        /// returns and sets the House's Rectangle's Y position
        /// </summary>
        public int RectY
        {
            get { return rect.Y; }
            set { rect.Y = value; }
        }

        /// <summary>
        /// returns the texture of the asset
        /// </summary>
        public Texture2D Asset
        {
            get { return asset; }
        }

        /// <summary>
        /// House constructor
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="asset"></param>
        public House(Rectangle rect, Texture2D asset)
        {
            this.rect = rect;
            this.asset = asset;
        }


        /// <summary>
        /// Draws the House object
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(Asset, Rect, Color.White);
        }
    }
}