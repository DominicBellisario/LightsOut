using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
 * Battery
 * a collectible that restores the player's flashlight charge
 */
namespace GroupProject_Game_TeamC
{
    internal class Battery : Collectible
    {
        private int count;

        /// <summary>
        /// The amount of batteries the player has
        /// </summary>
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="rect"> The hitbox of the collectible </param>
        /// <param name="asset"> The texture of the battery </param>
        /// <param name="wasCollected"> If the battery was collected or not </param>
        /// <param name="count"> How many batteries the player has </param>
        public Battery(Rectangle rect, Texture2D asset, bool wasCollected, int count) : base(rect, asset, wasCollected)
        {
            this.count = count;            
        }

        /// <summary>
        /// Overriden Collect method that only allows a battery to be picked up once
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public override bool Collect(Rectangle player)
        {
            // Batteries cannot be collected twice
            if (wasCollected)
            {
                return false;
            }

            return base.Collect(player);
        }

    }
}
