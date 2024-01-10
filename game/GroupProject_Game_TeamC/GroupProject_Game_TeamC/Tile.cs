using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Tile
 * Nodes for a Singly-Linked List that represent parts of a monster's path
 * Stores X and Y positions that represent the path and the Next tile in the path
 */
namespace GroupProject_Game_TeamC
{
    internal class Tile
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Tile Next { get; set; }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
