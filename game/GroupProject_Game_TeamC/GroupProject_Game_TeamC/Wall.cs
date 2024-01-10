using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
 * Wall
 * Inherits from a House Object and represents tiles that have collision
 * 
 * Was planned to have more differences to its parent,
 * but in practice the only difference is that it IS a Wall
 * which means it has collision.
 */
namespace GroupProject_Game_TeamC
{
    internal class Wall : House
    {
        /// <summary>
        /// Wall constructor
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="asset"></param>
        public Wall(Rectangle rect, Texture2D asset):base(rect, asset)
        {
        }
    }
}
