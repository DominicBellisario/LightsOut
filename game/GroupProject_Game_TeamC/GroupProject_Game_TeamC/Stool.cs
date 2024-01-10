using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
 * Stool
 * stool is a collectible that the player needs to turn on the light.
 * Only difference to the Collectible parent class is that it is a stool.
 */
namespace GroupProject_Game_TeamC
{
    internal class Stool : Collectible
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect"> The hitbox of the stepstool</param>
        /// <param name="asset"> The asset texture</param>
        /// <param name="wasCollected"> If stool has been picked up or not</param>
        /// <param name="count"> The amount of stool's</param>
        public Stool(Rectangle rect, Texture2D asset, bool wasCollected) : base(rect, asset, wasCollected)
        {
        }

    }
}