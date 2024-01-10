using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShapeUtils;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;
using System.IO;
using Microsoft.Xna.Framework.Audio;

/*
 * Monster
 * creates a monster
 * 
 * handles animation and states such as
 * Wandering by following a set path which is loaded from a text file
 * Attacking the player where it goes directly to the player's position
 */
namespace GroupProject_Game_TeamC
{
    

    enum MonsterStates
    {
        Attack,
        Visible,
        Transformation,
        Wander
    }

    internal class Monster
    {


        private string fileName;

        //Fields
        private Rectangle mRec;
        private Texture2D spriteSheet;
        private SpriteEffects flipSprite;
        private MonsterStates monsterState;
        private bool isVisible;
        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        private Texture2D coatRack;

        // Consts for Drawing
        private const int MonsterOffSet = 190;
        private const int MonsterRectWidth = 100;
        private const int MonsterRectHeight = 200;

        // Animation variables
        // The current animation frame
        private double d2Frame;
        private double dFrame = 0;
        private int frame;
        private int f2rame;
        // The amount of time that has passed
        private double timeCounter;
        // The amount of time (in fractional seconds) per frame
        private double timePerFrame;
        // The number of frames in the animation 
        private double WalkFrameCount = 3;

        // Stuff for LoadPath and Wander
        public String[,] pathArray;
        private int currentX;
        private int currentY;
        private Tile currentMonsterTile;

        // Stuff for monster movement
        private Path monsterPath;
        private const int mSpeed = 8;

        // Stun timer
        private double timer = 6;
        private bool stunned = false;

        private int screenWidth;
        private int screenHeight;

        private bool gameOver;

        //sounds
        private List<SoundEffect> sounds;
        private Random rng;

        //Properties

        /// <summary>
        /// returns wether or not the monster is stunned
        /// </summary>
        public bool Stunned
        {
            get { return stunned; }
            set { stunned = value; }
        }

        /// <summary>
        /// returns if the game is over
        /// </summary>
        public bool GameOver
        {
            get { return gameOver; }
            set { gameOver = value; }
        }

        /// <summary>
        /// Get/Set property for isVisible field
        /// Returns isVisible bool value
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        /// <summary>
        /// Allows the monsters rectangle to be used for collison
        /// </summary>
        public Rectangle MRec
        {
            get
            {
                // If the monster is stunned then its collision rectangle will be uncolidable
                if (stunned)
                {
                    return new Rectangle(5000, 5000, 1, 1);
                }

                // Else returns the regular monster rectangle
                else
                {
                    return mRec;
                }
            }
        }


        /// <summary>
        /// Monster constructor
        /// </summary>
        /// <param name="spriteSheet"></param>
        /// <param name="flipSprite"></param>
        /// <param name="mRec"></param>
        /// <param name="fileName"></param>
        public Monster(Texture2D spriteSheet, SpriteEffects flipSprite, Rectangle mRec,
            string fileName, int width, int height, Texture2D coatRack, List<SoundEffect> sounds)
        {
            this.fileName = fileName;

            this.spriteSheet = spriteSheet;
            this.flipSprite = flipSprite;
            this.mRec = mRec;
            monsterState = MonsterStates.Wander;
            LoadPath(fileName);
            screenWidth = width;
            screenHeight = height;

            this.coatRack = coatRack;

            gameOver = false;


            this.sounds = sounds;
            rng = new Random();
        }


        /// <summary>
        /// Update method for monster
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            MouseState mState = Mouse.GetState();
            // Monster's rectangle for collision
            mRec = new Rectangle(currentX, currentY - 192, 200, 300);

            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;
            if (timeCounter >= timePerFrame)
            {
                WalkFrameCount = 2;
                // Adjust the frame to the next image
                dFrame += .05;

                // Check the bounds - have we reached the end of walk cycle?
                if (dFrame > WalkFrameCount)
                {
                    dFrame = 0;
                    // Back to 1 (since 0 is the "standing" frame)
                    frame += 1;
                }

                // Remove the time we "used" - don't reset to 0
                // This keeps the time passed 
                timeCounter -= timePerFrame;

                if (frame > WalkFrameCount)
                    frame = 0;
            }
            if (timeCounter >= timePerFrame)
            {
                WalkFrameCount = 5;
                // Adjust the frame to the next image
                d2Frame += .35;

                // Check the bounds - have we reached the end of walk cycle?
                if (d2Frame > WalkFrameCount)
                {
                    d2Frame = 0;
                    // Back to 1 (since 0 is the "standing" frame)
                    f2rame += 1;
                }

                // Remove the time we "used" - don't reset to 0
                // This keeps the time passed
                timeCounter -= timePerFrame;

                if (f2rame > WalkFrameCount)
                    f2rame = 0;
            }

            

            // If the monster is stunned it will need to wait a certain amount of time before going back into wander
            if (stunned)
            {
                timer -= gameTime.ElapsedGameTime.TotalSeconds;

                // Monster is not drawn
                if (timer > 0)
                {
                    monsterState = MonsterStates.Visible;
                }

                // Monster is unstunned
                else
                {
                    if (!gameOver)
                    {
                        stunned = false;
                        monsterState = MonsterStates.Wander;
                        timer = 4;
                    }
                }

            }

            // Monster FSM
            switch (monsterState)
            {
                case MonsterStates.Wander:
                    // Follows the set path
                    PatrolMode();
                    if (InMonsterRange())
                    {
                        // Monster begins animation before starting to hunt player
                        monsterState = MonsterStates.Transformation;
                        if(rng.Next(2) == 1)
                        {
                            sounds[6].Play();
                        }
                        else
                        {
                            sounds[7].Play();
                        }
                        
                        watch.Restart();
                    }


                    break;

                case MonsterStates.Transformation:
                    // Functions as attack mode but does the animation


                    break;

                case MonsterStates.Attack:
                    // Purely seeking the player
                    AttackMode();
                    break;
            }
        }

        /// <summary>
        /// Updates the monster's path and position based on player movement
        /// should only be called within the Level class
        /// </summary>
        /// <param name="key"></param>
        public void UpdatePath(Keys key)
        {
            Tile currentTile = monsterPath.Head;
            KeyboardState kbState = Keyboard.GetState();

            // Displaces the monster based on the player movement
            if (key == Keys.W)
            {
                currentY += mSpeed;
            }
            if (key == Keys.A)
            {
                currentX += mSpeed;
            }
            if (key == Keys.S)
            {
                currentY -= mSpeed;
            }
            if (key == Keys.D)
            {
                currentX -= mSpeed;
            }

            // Moves the path based on player movement
            do
            {
                if (key == Keys.W)
                {
                    currentTile.Y += mSpeed;
                }

                if (key == Keys.A)
                {
                    currentTile.X += mSpeed;
                }

                if (key == Keys.S)
                {
                    currentTile.Y -= mSpeed;
                }

                if (key == Keys.D)
                {
                    currentTile.X -= mSpeed;
                }

                // Goes to the next tile to be displaced
                currentTile = currentTile.Next;

            } while (currentTile != monsterPath.Head);
        }


        /// <summary>
        /// Default method of movement, follows set path of tiles.
        /// Active when AttackMode = False
        /// </summary>
        public void PatrolMode()
        {
            // Checks if the monster has reached the next tile's position
            if (currentX == currentMonsterTile.Next.X && currentY == currentMonsterTile.Next.Y)
            {
                currentMonsterTile = currentMonsterTile.Next;
            }

            if (monsterState == MonsterStates.Wander)
            {
                // Updates X position until reaches next node
                if (currentX != currentMonsterTile.Next.X)
                {
                    if (currentX > currentMonsterTile.Next.X)
                    {
                        currentX -= mSpeed;
                    }
                    else
                    {
                        currentX += mSpeed;
                    }
                }

                // Updates Y position until reaches next node
                if (currentY != currentMonsterTile.Next.Y)
                {
                    if (currentY > currentMonsterTile.Next.Y)
                    {
                        currentY -= mSpeed;
                    }
                    else
                    {
                        currentY += mSpeed;
                    }
                }

                // Checks if the monster has reached the next tile's position
                if (currentX == currentMonsterTile.Next.X && currentY == currentMonsterTile.Next.Y)
                {
                    currentMonsterTile = currentMonsterTile.Next;
                }
            }
        }

        /// <summary>
        /// Checks if the monster is close to the player
        /// </summary>        
        /// <returns> True if the player is in range </returns>
        public bool InMonsterRange()
        {
            return Vector2.Distance(new Vector2(screenWidth / 2, screenHeight / 2), new Vector2(currentX, currentY)) < 725;
        }

        /// <summary>
        /// Checks if the monster is close enough to catch the player
        /// </summary>
        /// <returns> If the monster catches the player or not </returns>
        public bool MonsterCatch()
        {
            return Vector2.Distance(new Vector2(screenWidth / 2 - 50, screenHeight / 2 - 50), new Vector2(currentX, currentY)) < 150;
        }

        /// <summary>
        /// Secondary method of movement, follows player once within range.
        /// Active when AttackMode = True
        /// </summary>
        public void AttackMode()
        {
            // Checks the monster's current X to see how close it is to the center of the screen
            // Adjusts speed accordingly in order to reach player
            if (currentX + 100 > (screenWidth / 2) - 50)
            {
                currentX -= mSpeed;
            }
            else
            {
                currentX += mSpeed;
            }

            // Checks the monster's current Y to see how close it is to the center of the screen
            // Adjusts speed accordingly in order to reach player
            if (currentY + 150 > (screenHeight / 2) - 50 + 192)
            {
                currentY -= mSpeed;
            }
            else
            {
                currentY += mSpeed;
            }

        }

        public void DrawTransformation(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(currentX, currentY - 192),
                new Rectangle((int)(MonsterOffSet * frame), 10,
                MonsterRectWidth * 2, MonsterRectHeight + 100),
                Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);

            spriteBatch.Draw(spriteSheet, new Vector2(currentX, currentY - 192),
            new Rectangle((int)(65 * f2rame) + 200,
            MonsterOffSet * 2, 65, 90),
            Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
            if (watch.ElapsedMilliseconds > 2000)
            {
                monsterState = MonsterStates.Attack;
                watch.Stop();
                watch.Reset();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            switch (monsterState)
            {
                case MonsterStates.Transformation:
                    DrawTransformation(spriteBatch);
                    break;

                case MonsterStates.Wander:
                    spriteBatch.Draw(spriteSheet, new Vector2(currentX, currentY - 192),
                    new Rectangle(0, 0,
                    MonsterRectWidth * 2, MonsterRectHeight * 2)
                    , Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);

                    break;

                case MonsterStates.Attack:
                    spriteBatch.Draw(spriteSheet, new Vector2(currentX, currentY - 192),
                        new Rectangle(MonsterRectWidth * 6, 0,
                        MonsterRectWidth * 2, MonsterRectHeight + 100),
                        Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
                    break;

                // Draws the monster as a coat rack
                case MonsterStates.Visible:

                    spriteBatch.Draw(coatRack, new Rectangle(currentX + 50, currentY - 192, 100, 250), Color.White);

                    break;

            }
        }

        /// <summary>
        /// loads the monster's path from a text file and creates a singly linked list from it
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="Exception"></exception>
        public void LoadPath(string fileName)
        {
            // Declare variables
            StreamReader input = null;
            string currentLine;
            int rows;
            int columns;
            List<string> lines = new List<string>();
            string[] data;
            string currentPath;
            int currentI = 0;
            int currentJ = 0;


            try
            {
                input = new StreamReader("..\\..\\..\\" + fileName);

                // Read through text file and store it in List
                while ((currentLine = input.ReadLine()) != null)
                {
                    lines.Add(currentLine);
                }

                rows = lines.Count;
                columns = lines[0].Split(',').Length;
                pathArray = new string[rows, columns];

                // Read through list and convert it into a level blueprint
                for (int i = 0; i < rows; i++)
                {
                    data = lines[i].Split(',');
                    for (int j = 0; j < columns - 1; j++)
                    {
                        pathArray[i, j] = data[j];
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }


            // Searches for the head of the singly linked list from a 2D array and saves the index
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns - 1; j++)
                {
                    // System.Diagnostics.Debug.WriteLine((i+1) + " " + (j+1));
                    if (pathArray[i, j].Substring(0, 1) == "H")
                    {
                        currentI = i;
                        currentJ = j;
                    }
                }
            }

            // Moves through the 2D array creating a singly linked list
            currentPath = pathArray[currentI, currentJ];
            monsterPath = new Path();
            do
            {
                monsterPath.Add(currentJ * 96, currentI * 96);
                switch (currentPath.Substring(2))
                {
                    case "U":
                        currentI -= 1;
                        break;

                    case "D":
                        currentI += 1;
                        break;

                    case "L":
                        currentJ -= 1;
                        break;

                    case "R":
                        currentJ += 1;
                        break;
                }

                currentPath = pathArray[currentI, currentJ];

            } while (currentPath.Substring(0, 1) != "H");

            // Link the head and tail of the singly linked list
            monsterPath.Link();

            // Saves the starting the position of the monster
            currentMonsterTile = monsterPath.Head;
            currentX = monsterPath.Head.X;
            currentY = monsterPath.Head.Y;
        }

        /// <summary>
        /// Reloads the monster
        /// </summary>
        public void Reset()
        {
            LoadPath(fileName);
            monsterState = MonsterStates.Wander;
            gameOver = false;
        }


    }
}