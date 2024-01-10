using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GroupProject_Game_TeamC
{
    internal class Nightlight : House
    {
        //UNUSED IN FINAL RELEASE
        //used to get coordinates of respawn point
        private Rectangle spawn;

        public Nightlight(Rectangle rect, Texture2D asset) : base(rect, asset)
        {
        }

        /// <summary>
        /// Sets X and Y for spawn, and returns spawn. Use to set new spawn point.
        /// </summary>
        /// <returns></returns>
        public Rectangle SpawnPoint()
        {
            spawn.X = Rect.X;
            spawn.Y = Rect.Y;

            return spawn;
        }
    }
}
