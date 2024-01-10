using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Flashlight_Lighting_Demo
{
    internal class Tile
    {
        private Texture2D asset;
        private Rectangle pos;
        private bool dark;

        /// <summary>
        /// Takes asset, x and y coordinates.
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="pos"></param>
        public Tile(Texture2D asset, int x, int y)
        {
            this.asset = asset;
            //32 is size of each tile
            this.pos.X = 0;
            this.pos.Y = 0;
            this.pos.Height = asset.Height;
            this.pos.Width = asset.Width;

            dark = true;
        }

        public int X
        {
            get { return pos.X; }
        }

        public int Y
        {
            get { return pos.Y; }
        }

        /// <summary>
        /// If passed object touches tile, sets dark bool to false.
        /// </summary>
        /// <param name="other"></param>
        public void Collides(Rectangle other)
        {
            if (this.pos.Intersects(other))
            {
                dark = false;
            } else
            {
                dark = true;
            }
        }

        /// <summary>
        /// Draws object based on dark bool. If dark = true then draws normal, else draws with yellow tint.
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            if (dark == true)
            {
                sb.Draw(asset, pos, Color.White);
            } else
            {
                sb.Draw(asset, pos, Color.Blue);
            }
        }

    }
}
