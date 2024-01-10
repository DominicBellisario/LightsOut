using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.Audio;

/*
 * Level Class
 * 2/26/23
 * 
 * Manages all level related systems
 * Handles: 
 * Environment, Collectibles, Monsters, and
 * Player collision with walls and collectibles
 * 
 * 
 * No Known Issues
 */
namespace GroupProject_Game_TeamC
{
    public delegate bool HitDetection(Rectangle tile, double playerPosX, double playerPosY);

    public delegate bool Overcharge();

    internal class Level
    {
        //level
        private string fileName;
        private string layeredFileName;
        private Dictionary<string, Texture2D> textures;
        private House[,] levelBlueprint;
        private List<House> layeredArt;
        private Wall winCondition; 
        private Stool stool;
        private bool lightsOn;

        private Texture2D currentLightSwitch;

        //Junior the Rat
        private Rat junior;

        //collectibles
        private List<Collectible> levelCollectible;
        private int batteryCount;

        //monster
        List<Monster> monsterList;

        //player
        private const int pSpeed = 8;

        //player collision
        private Rectangle playerRectangle; //collectibles hitbox
        private Rectangle pXRectangle; //wall collision hitbox for x direction
        private Rectangle pYRectangle; //wall collision hitbox for y direction

        //player's pixel position
        private double pScreenPosX;
        private double pScreenPosY;

        //the players position in terms of which tile they are on
        private int pTileX;
        private int pTileY;

        //booleans for if the player is intersecting a wall in a certain direction
        private bool intersectsUp;
        private bool intersectsLeft;
        private bool intersectsDown;
        private bool intersectsRight;

        //Rat delegates
        private HitDetection onHit;
        private Overcharge flashlight;

        //screen bounds
        private int width;
        private int height;

        private SpriteFont font;
        KeyboardState prevKBS;

        //sounds
        private List<SoundEffect> soundEffects;
        bool stoolCollected;
        private SoundEffectInstance collect;
        private SoundEffectInstance lightSwitch;
        private bool soundPlayed;

        /// <summary>
        /// How many batteries the player collected
        /// Will be passed into the battery field in Game1
        /// </summary>
        public int BatteryCount
        {
            get { return batteryCount; }                 
        }

        /// <summary>
        /// Stool public property so it can be drawn when collected
        /// </summary>
        public Stool Stool
        {
            get { return stool; }
        }

        /// <summary>
        /// gets the player hitbox
        /// </summary>
        public Rectangle PlayerRectangle
        {
            get { return playerRectangle; }
        }

        /// <summary>
        /// returns wether or not the player was hit
        /// </summary>
        public HitDetection OnHit
        {
            get { return onHit; }
            set { onHit = value; }
        }

        /// <summary>
        /// gets a flashlight object
        /// </summary>
        public Overcharge Flashlight
        {
            get { return flashlight; }
            set { flashlight = value; }
        }

        /// <summary>
        /// gets the rat object
        /// </summary>
        public Rat Junior
        {
            get { return junior; }
        }

        /// <summary>
        /// returns wther or not the lights are on
        /// </summary>
        public bool LightsOn
        {
            get { return lightsOn; }
        }
        
               
        /// <summary>
        /// Level constructor
        /// </summary>
        /// <param name="textures"></param>
        /// <param name="fileName"></param>
        /// <param name="layeredFileName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="monsterList"></param>
        public Level(Dictionary<string, Texture2D> textures, string fileName, string layeredFileName,
            int width, int height, List<Monster> monsterList, SpriteFont font, List<SoundEffect> soundEffects)
        {
            //game textures dictionary
            this.textures = textures;

            //default font
            this.font = font;

            //turns the lights off
            lightsOn = false;
            currentLightSwitch = textures["LightOff"];

            //screen bounds
            this.width = width;
            this.height = height;

            //monsters for level
            this.monsterList = monsterList;

            //Rat
            junior = null;

            //player hitboxes
            playerRectangle = new Rectangle((width / 2) - 50, (height / 2) - 50, 96, 96);
            pXRectangle = new Rectangle((width/2) - 50, (height / 2) - 42, 96, 80);
            pYRectangle = new Rectangle((width/2) - 42, (height/2) - 50, 80, 96);

            //player starts at the center of the viewable screen
            pScreenPosX = width / 2;
            pScreenPosY = height / 2;

            //directional intersection variables
            intersectsUp = false;
            intersectsLeft = false;
            intersectsDown = false;
            intersectsRight = false;

            //level collectibles (temporary for play test)
            levelCollectible = new List<Collectible>();
            layeredArt = new List<House>();

            //loads level
            this.fileName = fileName;
            this.layeredFileName = layeredFileName;
            LoadLevel(fileName);
            LoadCollectibles(layeredFileName);

            //sound effects
            this.soundEffects = soundEffects;
            stoolCollected = false;
            collect = soundEffects[4].CreateInstance();
            lightSwitch = soundEffects[10].CreateInstance();
            collect.Volume = 0.5f;
            soundPlayed = false;
        }


        /// <summary>
        /// Loads the level's layout from a text file and creates a 2D array that represents the level
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="Exception"></exception>
        public void LoadLevel(string fileName)
        {
            //declare variables
            StreamReader input = null;
            string currentLine;
            int rows;
            int columns;
            List<string> lines = new List<string>();
            string[] data;
            string[] tileData;

            try
            {
                input = new StreamReader("..\\..\\..\\" + fileName);

                //read through text file and store it in List
                while ((currentLine = input.ReadLine()) != null)
                {
                    lines.Add(currentLine);
                }

                //initialize levelBlueprint
                rows = lines.Count;
                columns = lines[0].Split(',').Length;
                levelBlueprint = new House[rows, columns];

                //read through list and convert it into a level blueprint
                for (int i = 0; i < rows; i++)
                {
                    data = lines[i].Split(',');
                    for (int j = 0; j < columns - 1; j++)
                    {
                        //System.Diagnostics.Debug.WriteLine((i+1) + " " + (j+1)); //used for debugging

                        //checks what House object to make
                        if (data[j] == "E")
                        {
                            levelBlueprint[i, j] = null;
                        }
                        else if (data[j].Substring(0, 1) == "F")
                        {
                            tileData = data[j].Split(':');
                            levelBlueprint[i, j] = new House(new Rectangle(j * 96, i * 96, 96, 96), textures[tileData[1]]);
                        }
                        else if (data[j].Substring(0, 1) == "W")
                        {
                            tileData = data[j].Split(':');
                            levelBlueprint[i, j] = new Wall(new Rectangle(j * 96, i * 96, 96, 96), textures[tileData[1]]);
                        }
                        else if (data[j].Substring(0,1) == "L")
                        {
                            tileData = data[j].Split(':');
                            levelBlueprint[i, j] = new Wall(new Rectangle(j * 96, i * 96, 96, 96), textures[tileData[1]]);
                            winCondition = (Wall)levelBlueprint[i, j];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            //if file was opened close it
            if (input != null)
            {
                input.Close();
            }
        }

        /// <summary>
        /// Loads the Collectibles and layered art from a text file and stores them in lists
        /// </summary>
        /// <param name="fileName"></param>
        /// <exception cref="Exception"></exception>
        public void LoadCollectibles(string fileName)
        {
            StreamReader input = null;
            List<string> lines = new List<string>();
            string currentLine;
            int rows;
            int columns;
            string[] data;
            string[] tileData;

            try
            {
                input = new StreamReader("..\\..\\..\\" + fileName);

                //reads the file into a list
                while ((currentLine = input.ReadLine()) != null)
                {
                    lines.Add(currentLine);
                }

                //determines the amount of rows and columns
                rows = lines.Count;
                columns = lines[0].Split(',').Length;

                //stores the layered art and collectibles into lists
                for(int i=0; i < rows; i++)
                {
                    data = lines[i].Split(',');
                    for(int j=0; j < columns-1; j++)
                    {
                        //System.Diagnostics.Debug.WriteLine((i+1) + " " + (j+1)); //used for debugging

                        if (data[j].Substring(0, 1) == "B")
                        {
                            levelCollectible.Add(new Battery(new Rectangle((j * 96) + 30, (i * 96) + 30, 45, 30), textures["Battery"], false, 1));
                        }
                        else if (data[j].Substring(0, 1) == "F")
                        {
                            tileData = data[j].Split(':');
                            layeredArt.Add(new House(new Rectangle(j*96, i*96, 96 * int.Parse(tileData[3]), 96 * int.Parse(tileData[2]) ), textures[tileData[1]] ) );
                        }
                        else if (data[j].Substring(0, 1) == "S")
                        {
                            levelCollectible.Add(new Stool(new Rectangle(j * 96, i * 96, 96, 96), textures["Stool"], false));
                            stool = (Stool)(levelCollectible[levelCollectible.Count - 1]);
                        }
                        else if (data[j].Substring(0, 1) == "R")
                        {
                            junior = new Rat(new Rectangle(j * 96, i * 96, 96, 96), textures["Junior"], textures["Toilet"], "", font);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }

            //if file was opened close it
            if(input != null)
            {
                input.Close();
            }
        }



        /// <summary>
        /// Updates the level based on player movement and checks for intersections
        /// It handles changing level position, collectibles positions, layered art positions, and monster path and position
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();

            if (winCondition != null)
            {
                //checks if the player has won
                if (playerRectangle.Intersects(winCondition.Rect) && stool.WasCollected)
                {
                    lightsOn = true;
                    currentLightSwitch = textures["LightOn"];

                    //sound used when the player touches the light switch
                    if (!soundPlayed)
                    {
                        lightSwitch.Play();
                        soundPlayed = true;
                    }
                }
            }


            //checks if tiles above the player are being intersected
            if (kbState.IsKeyDown(Keys.W))
            {
                if (Intersect(Keys.W))
                {
                    intersectsUp = true;
                }
                else
                {
                    intersectsUp = false;
                }
            }
            //checks if tiles to the left of the player are being intersected
            if (kbState.IsKeyDown(Keys.A))
            {
                if (Intersect(Keys.A))
                {
                    intersectsLeft = true;
                }
                else
                {
                    intersectsLeft = false;
                }
            }
            //checks if tiles below the player are being intersected
            if (kbState.IsKeyDown(Keys.S))
            {
                if (Intersect(Keys.S))
                {
                    intersectsDown = true;
                }
                else
                {
                    intersectsDown = false;
                }
            }
            //checks if tiles to the right of the player are being intersected
            if (kbState.IsKeyDown(Keys.D))
            {
                if (Intersect(Keys.D))
                {
                    intersectsRight = true;
                }
                else
                {
                    intersectsRight = false;
                }
            }


            //moves the level
            for (int i = 0; i < levelBlueprint.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < levelBlueprint.GetLength(1) - 1; j++)
                {
                    if (levelBlueprint[i, j] != null)
                    {
                        //moves the level down
                        if (kbState.IsKeyDown(Keys.W))
                        {
                            if (!intersectsUp)
                            {
                                levelBlueprint[i, j].RectY += pSpeed;
                            }
                        }
                        //moves the level to the right
                        if (kbState.IsKeyDown(Keys.A))
                        {
                            if (!intersectsLeft)
                            {
                                levelBlueprint[i, j].RectX += pSpeed;
                            }
                        }
                        //moves the level up
                        if (kbState.IsKeyDown(Keys.S))
                        {
                            if (!intersectsDown)
                            {
                                levelBlueprint[i, j].RectY -= pSpeed;
                            }
                        }
                        //moves the level to the left
                        if (kbState.IsKeyDown(Keys.D))
                        {
                            if (!intersectsRight)
                            {
                                levelBlueprint[i, j].RectX -= pSpeed;
                            }
                        }
                    }
                }
            }


            //updates monsters
            if (monsterList != null)
            {
                foreach (Monster m in monsterList)
                {
                    m.Update(gameTime);
                }
            }


            //moves player, collectibles, monster, layered art, and junior the Rat UP
            if (kbState.IsKeyDown(Keys.W))
            {
                if (!intersectsUp)
                {
                    //changes player's relative level position
                    pScreenPosY -= pSpeed;

                    //changes the monsters' path and position
                    if (monsterList != null)
                    {
                        foreach (Monster m in monsterList)
                        {
                            m.UpdatePath(Keys.W);
                        }
                    }

                    //changes the Rat's position
                    if(junior != null)
                    {
                        junior.RectY += pSpeed;
                    }

                    //changes collectible positions
                    foreach(Collectible b in levelCollectible)
                    {
                        b.RectY += pSpeed;
                    }

                    //changes layered art positions
                    foreach(House h in layeredArt)
                    {
                        h.RectY += pSpeed;
                    }
                }
            }
            //moves player, collectibles, monster, layered art, and junior the Rat LEFT
            if (kbState.IsKeyDown(Keys.A))
            {
                if (!intersectsLeft)
                {
                    //changes player's relative level position
                    pScreenPosX -= pSpeed;

                    //changes the monsters' path and position
                    if (monsterList != null)
                    {
                        foreach (Monster m in monsterList)
                        {
                            m.UpdatePath(Keys.A);
                        }
                    }

                    //changes the Rat's position
                    if (junior != null)
                    {
                        junior.RectX += pSpeed;
                    }

                    //changes collectible positions
                    foreach (Collectible b in levelCollectible)
                    {
                        b.RectX += pSpeed;
                    }

                    //changes layered art positions
                    foreach(House h in layeredArt)
                    {
                        h.RectX += pSpeed;
                    }
                }
            }
            //moves player, collectibles, monster, layered art, and junior the Rat DOWN
            if (kbState.IsKeyDown(Keys.S))
            {
                if (!intersectsDown)
                {
                    //changes player's relative level position
                    pScreenPosY += pSpeed;

                    //changes the monsters' path and position
                    if (monsterList != null)
                    {
                        foreach (Monster m in monsterList)
                        {
                            m.UpdatePath(Keys.S);
                        }
                    }

                    //changes the Rat's position
                    if (junior != null)
                    {
                        junior.RectY -= pSpeed;
                    }

                    //changes collectibles position
                    foreach (Collectible b in levelCollectible)
                    {
                        b.RectY -= pSpeed;
                    }

                    //changes layered art positions
                    foreach(House h in layeredArt)
                    {
                        h.RectY -= pSpeed;
                    }
                }
            }
            //moves player, collectibles, monster, layered art, and junior the Rat RIGHT
            if (kbState.IsKeyDown(Keys.D))
            {
                if (!intersectsRight)
                {
                    //changes player's relative level position
                    pScreenPosX += pSpeed;

                    //changes the monsters' path and position
                    if (monsterList != null)
                    {
                        foreach (Monster m in monsterList)
                        {
                            m.UpdatePath(Keys.D);
                        }
                    }

                    //changes the Rat's position
                    if (junior != null)
                    {
                        junior.RectX -= pSpeed;
                    }

                    //changes collectibles position
                    foreach (Collectible b in levelCollectible)
                    {
                        b.RectX -= pSpeed;
                    }

                    //changes layered art positions
                    foreach(House h in layeredArt)
                    {
                        h.RectX -= pSpeed;
                    }
                }
            }
            

            //checks if a collectible is being collected
            for (int i = 0; i < levelCollectible.Count; i++)
            {
                if (levelCollectible[i].Collect(playerRectangle))
                {
                    //if a battery is collected update count
                    if (levelCollectible[i] is Battery)
                    {
                        batteryCount++;

                        //sound played when picking up a battery
                        soundEffects[2].Play();
                    }
                    else if (levelCollectible[i] is Stool)
                    {
                        levelCollectible[i].WasCollected = true;
                        stool.WasCollected = true;
                    }
                }
            }

            //when stool is collected, play the sound
            if (stool != null)
            {
                if (stool.WasCollected && !stoolCollected)
                {
                    collect.Play();
                    stoolCollected = true;  
                }
            }
            

            //checks if a player has batteries and if they are attempting to use them
            if (batteryCount > 0)
            {
                if (kbState.IsKeyDown(Keys.Space) &&
                    prevKBS.IsKeyUp(Keys.Space))
                {
                    batteryCount--;
                }
            }

            //checks if the player has stunned the Rat Junior
            if (junior != null && !Junior.IsStunned)
            {
                if (onHit(Junior.RatRectangle, width / 2, height / 2) && flashlight())
                {
                    junior.RevealRat();
                    Junior.IsStunned = true;
                }
            }

            //checks if the monsters are being stunned by the player
            if(monsterList != null)
            {
                foreach (Monster m in monsterList)
                {
                    if (!lightsOn)
                    {
                        if (onHit(m.MRec, width / 2, height / 2) && flashlight())
                        {
                            m.Stunned = true;
                        }
                    }
                    else
                    {
                        m.Stunned = true;
                        m.GameOver = true;
                    }
                }
                
            }


            // Gets previous keyboard state
            prevKBS = kbState;
        }

        /// <summary>
        /// Checks if the player is intersecting a Wall object
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Intersect(Keys key)
        {
            //determines the players position in terms of the levelBlueprint 2D array
            pTileX = (int)(pScreenPosX / 96);
            pTileY = (int)(pScreenPosY / 96);

            //determine the direction the player is going in based off of a key
            switch (key)
            {
                case Keys.W:

                    //checks if the block is within appropriate y range
                    if (pTileY - 1 >= 0)
                    {
                        //read through the three blocks above the player
                        for (int i = pTileX - 1; i < pTileX + 2; i++)
                        {
                            //checks if the block is within appropriate x range
                            if (i >= 0 && i < levelBlueprint.GetLength(1))
                            {
                                if (levelBlueprint[pTileY - 1, i] is Wall)
                                {
                                    //checks if the player is toucking a wall
                                    if (pYRectangle.Intersects(levelBlueprint[pTileY - 1, i].Rect))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }

                    break;

                case Keys.S:

                    //checks if the block is within appropriate y range
                    if (pTileY + 1 < levelBlueprint.GetLength(0))
                    {
                        //read through the three blocks below the player
                        for (int i = pTileX - 1; i < pTileX + 2; i++)
                        {
                            //checks if the block is within appropriate x range
                            if (i >= 0 && i < levelBlueprint.GetLength(1))
                            {
                                if (levelBlueprint[pTileY + 1, i] is Wall)
                                {
                                    //checks if the player is touching a wall
                                    if (pYRectangle.Intersects(levelBlueprint[pTileY + 1, i].Rect))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }

                    break;

                case Keys.A:

                    //checks if the blocks are within appropriate x range
                    if (pTileX - 1 >= 0)
                    {
                        //read through the three blocks to the left of the player
                        for (int i = pTileY - 1; i < pTileY + 2; i++)
                        {
                            //checks if the block is within appropriate y range
                            if (i >= 0 && i < levelBlueprint.GetLength(0))
                            {
                                if (levelBlueprint[i, pTileX - 1] is Wall)
                                {
                                    //checks if the player is touching a wall
                                    if (pXRectangle.Intersects(levelBlueprint[i, pTileX - 1].Rect))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }

                    break;

                case Keys.D:

                    //checks if the blocks are within appropriate x range
                    if (pTileX + 1 < levelBlueprint.GetLength(1))
                    {
                        //read through the three blocks to the right of the player
                        for (int i = pTileY - 1; i < pTileY + 2; i++)
                        {
                            //checks if the block is within appropriate y range
                            if (i >= 0 && i < levelBlueprint.GetLength(0))
                            {
                                if (levelBlueprint[i, pTileX + 1] is Wall)
                                {
                                    //checks if the player is touching a wall
                                    if (pXRectangle.Intersects(levelBlueprint[i, pTileX + 1].Rect))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }


        /// <summary>
        /// Draws the level
        /// Handles the level, collectibles, layered art, and monster
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        public void Draw(SpriteBatch sb)
        {
            //draws level tiles that are in a spsecific range from player
            for (int i = pTileY - 15; i < pTileY + 16; i++)
            {
                for (int j = pTileX - 20; j < pTileX + 21; j++)
                {
                    if (i >= 0 && j >= 0 && j < levelBlueprint.GetLength(1) && i < levelBlueprint.GetLength(0))
                    {
                        if (levelBlueprint[i, j] != null)
                        {
                            levelBlueprint[i, j].Draw(sb);
                        }
                    }
                }
            }

            //draws layered art
            foreach(House h in layeredArt)
            {
                h.Draw(sb);
            }

            if(winCondition != null)
            {
                sb.Draw(currentLightSwitch, new Rectangle(winCondition.RectX + 28, winCondition.RectY - 96 + 28, 40, 40), Color.White);
            }
            


            //draws collectibles
            foreach (Collectible b in levelCollectible)
            {
                b.Draw(sb);
            }
        }

        /// <summary>
        /// Draws the level's monsters
        /// </summary>
        /// <param name="sb"></param>
        public void DrawMonster(SpriteBatch sb)
        {
            foreach(Monster m in monsterList)
            {
                m.Draw(sb);
            }
        }

        /// <summary>
        /// Draw Junior the Rat
        /// </summary>
        /// <param name="sb"></param>
        public void DrawRat(SpriteBatch sb)
        {
            Junior.Draw(sb);
        }
        

        /// <summary>
        /// Resets the level
        /// </summary>
        public void Reset()
        {
            //resets lights
            lightsOn = false;
            currentLightSwitch = textures["LightOff"];
            soundPlayed = false;

            //resets the player's psuedo level position
            pScreenPosX = width / 2;
            pScreenPosY = height / 2;

            //reset battery count
            batteryCount = 0;
            stool = null;
            stoolCollected = false;

            //reloads the level
            LoadLevel(fileName);

            //reloads the collectibles and layered objects
            layeredArt.Clear();
            levelCollectible.Clear();
            LoadCollectibles(layeredFileName);

            //reloads the monsters
            if (monsterList != null)
            {
                foreach (Monster m in monsterList)
                {
                    m.Reset();
                }
            }
        }
    }
}