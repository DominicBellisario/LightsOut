using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Path
 * Singly-Linked List
 * represents the monster's path
 */
namespace GroupProject_Game_TeamC
{
    internal class Path
    {
        private Tile head = null;
        private Tile tail = null;

        /// <summary>
        /// returns the head of the monster path linked list
        /// </summary>
        public Tile Head
        {
            get { return head; }
            set { head = value; }
        }

        public int Count { get; set; }

        /// <summary>
        /// Adds a tile to the Monster's path
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Add(int x, int y)
        {
            if(head != null)
            {
                tail.Next = new Tile(x, y);
                tail = tail.Next;
                Count++;
            }
            else
            {
                head = new Tile(x, y);
                tail = head;
                Count++;
            }
        }

        /// <summary>
        /// Links the head and the tail of the monster's path
        /// </summary>
        public void Link()
        {
            tail.Next = head;
        }
    }
}
