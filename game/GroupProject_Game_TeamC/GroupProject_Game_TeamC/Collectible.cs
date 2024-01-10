using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
 * Collectible
 * parent class for a collectible
 * contains basic functionality such as 
 * position, texture, drawing, and checks if it was collected.
 */
namespace GroupProject_Game_TeamC
{
    internal class Collectible
    {
        protected Rectangle rect;
        protected Texture2D asset;
        protected bool wasCollected;

        public Rectangle Rect
        {
            get { return rect; }
        }

        public int RectX
        {
            get { return rect.X; }
            set { rect.X = value; }
        }

        public int RectY
        {
            get { return rect.Y; }
            set { rect.Y = value; }
        }

        /// <summary>
        /// Returns if an object was collected or not
        /// </summary>
        public bool WasCollected
        {
            get { return wasCollected; }
            set { wasCollected = value; }
        }

        public Collectible(Rectangle rect, Texture2D asset, bool wasCollected)
        {
            this.rect = rect;
            this.asset = asset;
            this.wasCollected = false;
        }

        public void Draw(SpriteBatch sb)
        {
            if (!wasCollected)
            {
                sb.Draw(asset, rect, Color.White);
            }
        }

        /// <summary>
        /// Checks if the player collides with a collectible
        /// </summary>
        /// <param name="obj"> The object being collected </param>
        /// <param name="player"> the players hitbox </param>
        /// <returns> True if the player collides with collectible </returns>
        public virtual bool Collect(Rectangle player)
        {
            
            // If an object is collected then wasCollected field will be true
            if(rect.Intersects(player))
            {
                wasCollected = true;
            }

            return (rect.Intersects(player));
        }

    }
}
