using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Flashlight_Lighting_Demo
{
    /// <summary>
    /// Contains directions for player to face.
    /// </summary>
    enum PlayerState
    {
        FaceNorth,
        FaceSouth,
        FaceEast,
        FaceWest
    }

    internal class Player
    {
        //Player position with getters.
        private Rectangle pos;
        public int xPos
        {
            get
            {
                return pos.X;
            }
        }
        public int yPos
        {
            get
            {
                return pos.Y;
            }
        }

        private PlayerState state;
        private Texture2D asset;

        public Player(Rectangle position, Texture2D asset)
        {
            this.pos = position;
            this.asset = asset;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            
        }

    }
}
