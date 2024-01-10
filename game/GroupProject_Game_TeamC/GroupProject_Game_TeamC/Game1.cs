//using Flashlight_Demo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;


namespace GroupProject_Game_TeamC
{
    enum GameState
    {
        Title,
        Game,
        Pause,
        GameOver,
        LevelSelect
    }

    //nothing is being done with this currently, added 4/9/23
    enum Levels
    {
        Test,
        Tutorial,
        Level1
    }

    public class Game1 : Game
    {
        private bool godmode;
        private bool lights;

        private GameState state;
        private Levels levelState;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //levels
        private Level levelOne;
        private Level testLevel;
        private Level tutorial;

        private string[] tutorialInstructions;
        private int tutorialStep;
        private int tutorialSpecialAnimation;

        VectorLight hitDetection;

        //monster lists
        private List<Monster> monstersTestLevel;
        private List<Monster> monstersLevelOne;


        private SpriteFont defaultFont;

        //screen bounds
        private int width;
        private int height;

        private KeyboardState prevKBS;
        private MouseState mState;
        private MouseState prevMS;

        //texture dictionary
        private Dictionary<string, Texture2D> assets;

        //player object
        private Player player;
        Texture2D spritesheet;

        //CoverArt + Screen Art
        private Texture2D coverArt;
        private Texture2D monsterFace;

        // Tile Textures
        private Texture2D stoneWall;
        private Texture2D woodWall;
        private Texture2D woodFloor;

        //Living Room Textures
        private Texture2D chair;
        private Texture2D window;
        private Texture2D table;
        private Texture2D bigChair;
        private Texture2D couch;
        private Texture2D television;

        //Kitchen Textures
        private Texture2D fridge;
        private Texture2D cabinet;

        //Bathroom Textures
        private Texture2D toilet;
        private Texture2D tub;
        private Texture2D sink;
        private Texture2D mirror;

        // Button texture and button positions
        private List<Button> buttons = new List<Button>();
        private Texture2D buttonTexture;
        private Texture2D drawer;
        private Texture2D drawerTop;
        private Texture2D drawerMiddle;
        private Texture2D drawerBottom;
        private Texture2D drawerSmall;
        private Texture2D drawerSmallTop;
        private Texture2D drawerSmallBottom;
        private Texture2D teddyBear;
        private Texture2D teddyBearSelected;
        private Texture2D halo;



        //Light Switch
        private Texture2D lightSwitchBase;
        private Texture2D lightSwitchRed;
        private Texture2D lightSwitchGreen;

        // Collectible objects
        private Battery battery; //battery used for UI
        private Texture2D batteryTexture;

        private Texture2D stoolTexture;

        //MonsterSpritesheet
        Rectangle monPosition; 
        Texture2D monsterSheet;
        Texture2D coatRack;

        //Junioor the Rat
        private Texture2D juniorTheRat;

        //test level monsters
        private Monster monster;

        //level 1 monsters
        private Monster monsterLevel1;
        private Monster monster2Level1;

        //Rug and Carpet
        private Texture2D carpet;
        private Texture2D rug;

        //Sounds
        private List<SoundEffect> soundEffects;
        private SoundEffectInstance buzz;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            state = GameState.Title;

            //size based on screen
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();

            width = _graphics.GraphicsDevice.Viewport.Width;
            height = _graphics.GraphicsDevice.Viewport.Height;

            godmode = false;
            lights = false;

            assets = new Dictionary<string, Texture2D>();

            hitDetection = new VectorLight();

            monstersTestLevel = new List<Monster>();
            monstersLevelOne = new List<Monster>();

            soundEffects = new List<SoundEffect>();
            SoundEffect.MasterVolume = 0.2f;

            tutorialStep = 0;
            tutorialInstructions = new string[] 
            { 
              "Hey! \nstun me with your flashlight \nby holding down left click \nand then releasing it!",
              "Hello! \nNice to meet you now that you aren't hallucinating.\nMy name is Junior the Rat!\n        (Press ENTER to continue)",
              "But before we continue, \nYou should go recharge your flashlight \nThere are some batteries at the bottom right of the room" +
              " \n        (Press SPACE to use a Battery)",
              "You know, \nIt'd be easier to see if you turn on the lights \nIt's a little too high for you though, " +
              "\nIf you see a stool grab it and bring it to the light switch"
            };


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            defaultFont = this.Content.Load<SpriteFont>("defaultFont");

            //Cover Art + Screens
            coverArt = Content.Load<Texture2D>("Lights Out - Covee Art");
            monsterFace = Content.Load<Texture2D>("Lights Out - GameOver");


            //loads spritesheet for player
            spritesheet = Content.Load<Texture2D>("playerSpriteSheetv4");

            //Load Monster Spritesheet
            monsterSheet = Content.Load<Texture2D>("Lights Out - Coat Rack Monster SpriteSheet V1");

            //Light Switch 
            lightSwitchBase = this.Content.Load<Texture2D>("LightSwitchBase");
            lightSwitchGreen = this.Content.Load<Texture2D>("LightSwitchGreen");
            lightSwitchRed = this.Content.Load<Texture2D>("LightSwitchRed");
            assets.Add("LightBase", lightSwitchBase);
            assets.Add("LightOn", lightSwitchGreen);
            assets.Add("LightOff", lightSwitchRed);

            // Stone wall texture
            stoneWall = this.Content.Load<Texture2D>("StoneFloor");
            assets.Add("StoneWall", stoneWall);

            //wood wall texture
            woodWall = this.Content.Load<Texture2D>("ceiling");
            assets.Add("WoodWall", woodWall);

            //chair texture
            //Chair 1
            chair = this.Content.Load<Texture2D>("LargeChair1");
            assets.Add("Chair", chair);

            //window
            //Window Filled
            window = this.Content.Load<Texture2D>("WindowLargeDark");
            assets.Add("Window", window);

            //table
            table = this.Content.Load<Texture2D>("Table");
            assets.Add("Table", table);

            //Rugs and Carpets
            rug = this.Content.Load<Texture2D>("Rug");
            carpet = this.Content.Load<Texture2D>("MountainRug");
            assets.Add("Rug", rug);
            assets.Add("Carpet", carpet);

            //Living Room Assets
            couch = this.Content.Load<Texture2D>("Couch");
            bigChair = this.Content.Load<Texture2D>("BigRedChair");
            television = this.Content.Load<Texture2D>("Television");
            assets.Add("Couch", couch);
            assets.Add("BigChair", bigChair);
            assets.Add("Tv", television);


            //Bathroom
            sink = this.Content.Load<Texture2D>("Sink'");
            tub = this.Content.Load<Texture2D>("Tub");
            mirror = this.Content.Load<Texture2D>("Mirror");
            toilet = this.Content.Load<Texture2D>("Toilet");
            assets.Add("Sink", sink);
            assets.Add("Tub", tub);
            assets.Add("Mirror", mirror);
            assets.Add("Toilet", toilet);


            //Kitchen
            fridge = this.Content.Load<Texture2D>("Fidge");
            cabinet = this.Content.Load<Texture2D>("Cabinet");
            assets.Add("Cabinet", cabinet);
            assets.Add("Fridge", fridge);

            // Wood Floor Texture
            woodFloor = this.Content.Load<Texture2D>("floor");
            assets.Add("WoodFloor", woodFloor);

            // Makes battery
            batteryTexture = Content.Load<Texture2D>("Battery");
            assets.Add("Battery", batteryTexture);
            battery = new Battery(new Rectangle(25, 25, 45, 30), batteryTexture, false, 0);

            // Makes stool
            stoolTexture = Content.Load<Texture2D>("Stool");
            assets.Add("Stool", stoolTexture);

            // Button texture and buttons positions/sizes
            drawer = Content.Load<Texture2D>("Drawer");
            drawerTop = Content.Load<Texture2D>("Drawer top");
            drawerMiddle = Content.Load<Texture2D>("Drawer middle");
            drawerBottom = Content.Load<Texture2D>("Drawer bottom");
            buttonTexture = Content.Load<Texture2D>("button");
            drawerSmall = Content.Load<Texture2D>("Drawer #2 pixel");
            drawerSmallTop = Content.Load<Texture2D>("Drawer #2 Top pixel");
            drawerSmallBottom = Content.Load<Texture2D>("Drawer #2 Bottom pixel");
            teddyBear = Content.Load<Texture2D>("Teddy Bear");
            teddyBearSelected = Content.Load<Texture2D>("Teddy Bear Selected");
            halo = Content.Load<Texture2D>("halo");



            int buttonWidth = 1240;
            int buttonHeight = 315;

            // Continue button
            buttons.Add(new Button(_graphics.GraphicsDevice,
                new Rectangle(width / 2 - buttonWidth / 2,
                height / 2 - buttonHeight,
                buttonWidth, buttonHeight),
                buttonTexture, "Continue", defaultFont));
            buttons[0].OnButtonClicked += SwitchToGame;

            // Quit button
            buttons.Add(new Button(_graphics.GraphicsDevice,
               new Rectangle(width / 2 - buttonWidth / 2,
               height / 2 - buttonHeight / 10,
               buttonWidth, buttonHeight),
               buttonTexture, "Quit", defaultFont));
            buttons[1].OnButtonClicked += SwitchToGameOver;

            // Level select buttons
            // Tutorial button
            buttons.Add(new Button(_graphics.GraphicsDevice,
                new Rectangle(width / 2 - buttonWidth / 2,
                height / 3 - buttonHeight + 142,
                buttonWidth, buttonHeight),
                buttonTexture, "Test Level", defaultFont));
            buttons[2].OnButtonClicked += Tutorial;
            buttons[2].OnButtonClicked += SwitchToGame;


            // Level 1 button
            buttons.Add(new Button(_graphics.GraphicsDevice,
               new Rectangle(width / 2 - buttonWidth / 2,
               height / 3 - buttonHeight / 20 + 100,
               buttonWidth, buttonHeight),
               buttonTexture, "Level 1", defaultFont));
            buttons[3].OnButtonClicked += PlayLevel1;
            buttons[3].OnButtonClicked += SwitchToGame;

            // Test Level button
            buttons.Add(new Button(_graphics.GraphicsDevice,
            new Rectangle(width / 2 - buttonWidth / 2,
               height - buttonHeight - 100,
               buttonWidth, buttonHeight),
               buttonTexture, "Tutorial", defaultFont));
            buttons[4].OnButtonClicked += PlayTestLevel;
            buttons[4].OnButtonClicked += SwitchToGame;

            // God mode button
            buttons.Add(new Button(_graphics.GraphicsDevice,
            new Rectangle(width / 6 - 200,
               height - buttonHeight - 100,
               buttonWidth / 4 - 100, buttonHeight - 100),
               buttonTexture, "God Mode", defaultFont));
            buttons[5].OnButtonClicked += GodMode;
            

            coatRack = this.Content.Load<Texture2D>("CoatRack");
            assets.Add("CoatRack", coatRack);
            
            //sounds
            //ambient sound
            soundEffects.Add(Content.Load<SoundEffect>("FlashlightBuzzingLoop"));
            buzz = soundEffects[0].CreateInstance();
            buzz.Volume = 0.4f;
            //overcharge sound
            soundEffects.Add(Content.Load<SoundEffect>("Overcharging"));
            //battery pickup sound
            soundEffects.Add(Content.Load<SoundEffect>("PickUpBattery"));
            //shooting the charge sound
            soundEffects.Add(Content.Load<SoundEffect>("ShootCharge"));
            //picking up the stepstool sound
            soundEffects.Add(Content.Load<SoundEffect>("StepStoolPickup"));
            //use battery sound
            soundEffects.Add(Content.Load<SoundEffect>("UseBattery"));
            //monster sound 1
            soundEffects.Add(Content.Load<SoundEffect>("MoVoice1"));
            //monster sound 2
            soundEffects.Add(Content.Load<SoundEffect>("MoVoice2"));
            //foot one
            soundEffects.Add(Content.Load<SoundEffect>("Walk1"));
            //foot two
            soundEffects.Add(Content.Load<SoundEffect>("Walk2"));
            //player flips the switch
            soundEffects.Add(Content.Load<SoundEffect>("lightswitch"));

            //makes player
            player = new Player(spritesheet, width, height, soundEffects);

            //test level Monster
            monster = new Monster(monsterSheet, SpriteEffects.None, monPosition, "TestMonsterPath.txt", width, height, coatRack, soundEffects);
            monstersTestLevel.Add(monster);

            //Level 1 Monster
            monsterLevel1 = new Monster(monsterSheet, SpriteEffects.None, monPosition, "MPL1.txt", width, height, coatRack, soundEffects);
            monstersLevelOne.Add(monsterLevel1);

            monster2Level1 = new Monster(monsterSheet, SpriteEffects.None, monPosition, "L1M2P.txt", width, height, coatRack, soundEffects);
            monstersLevelOne.Add(monster2Level1);

            juniorTheRat = Content.Load<Texture2D>("Junior the RatEdged");
            assets.Add("Junior", juniorTheRat);

            //Tutorial Level
            tutorial = new Level(assets,
                                 "Tutorial.txt",
                                 "TutorialLayered.txt",
                                 width, height,
                                 null,
                                 defaultFont,
                                 soundEffects);
            tutorial.OnHit += hitDetection.Intersects;
            tutorial.Flashlight += player.OverCharged;

            tutorial.Junior.SignText = tutorialInstructions[0];

            //Level 1
            levelOne = new Level(assets, 
                                 "Level1.txt", 
                                 "Level1Layered.txt", 
                                 width, height,
                                 monstersLevelOne,
                                 defaultFont,
                                 soundEffects);
            levelOne.OnHit += hitDetection.Intersects;
            levelOne.Flashlight += player.OverCharged;

            // Test Level
            testLevel = new Level(assets, 
                                  "TestLevel.txt", 
                                  "TestLevelLayered.txt", 
                                  width, height,
                                  monstersTestLevel,
                                  defaultFont,
                                  soundEffects);
            testLevel.OnHit += hitDetection.Intersects;
            testLevel.Flashlight += player.OverCharged;


            
        }

        protected override void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here
            KeyboardState kbs = Keyboard.GetState();
            MouseState mState = Mouse.GetState();
            switch (state)
            {
                case GameState.Title:
                    if (SingleKeyPress(Keys.Enter))
                    {
                        

                        state = GameState.LevelSelect;
                    }
                    break;

                case GameState.LevelSelect:
                    {
                        // Buutons that can be pressed to choose level
                        buttons[2].Update(gameTime);
                        buttons[3].Update(gameTime);
                        buttons[4].Update(gameTime);
                        buttons[5].Update(gameTime);
                    }
                    break;

                case GameState.Game:

                    //player update logic                   
                    player.Update(gameTime, battery);

                    hitDetection.Direction(width/2, height/2);

                    
                    

                    switch (levelState)
                    {
                        // Only updates level 1 if it was selected
                        case Levels.Level1:

                            levelOne.Update(gameTime);

                            //loops ambient sound
                            buzz.Play();

                            // sets the battery count equal to the amount of batteries collected in the
                            // level class
                            battery.Count = levelOne.BatteryCount;

                            if (levelOne.LightsOn)
                            {
                                lights = true;
                            }

                            if (monsterLevel1.MRec.Intersects(levelOne.PlayerRectangle) && !godmode)
                            {
                                state = GameState.GameOver;
                                levelOne.Reset();
                            }

                            if (monster2Level1.MRec.Intersects(levelOne.PlayerRectangle) && !godmode)
                            {
                                state = GameState.GameOver;
                                levelOne.Reset();
                            }

                            break;

                        // Only updates test level if it was selected
                        case Levels.Test:

                            testLevel.Update(gameTime);

                            //loops ambient sound
                            buzz.Play();

                            // sets the battery count equal to the amount of batteries collected in the
                            // level class
                            battery.Count = testLevel.BatteryCount;

                            if(monster.MRec.Intersects(testLevel.PlayerRectangle))
                            {
                                state = GameState.GameOver;
                                testLevel.Reset();
                            }
                            break;

                        case Levels.Tutorial:

                            tutorial.Update(gameTime);

                            //loops ambient sound
                            buzz.Play();

                            battery.Count = tutorial.BatteryCount;

                            if(tutorial.Junior.IsStunned && tutorialStep == 0)
                            {
                                tutorialStep++;
                            }

                            

                            if(tutorialStep == 1 && kbs.IsKeyDown(Keys.Enter))
                            {
                                tutorialStep++;
                            }

                            if(tutorialStep == 2 && tutorial.BatteryCount > 0 && kbs.IsKeyUp(Keys.Space) && prevKBS.IsKeyDown(Keys.Space))
                            {
                                tutorialStep++;
                            }

                            if (tutorial.LightsOn)
                            {
                                lights = true;
                            }

                            if (tutorialStep == 3 && kbs.IsKeyDown(Keys.F1))
                            {
                                player.BarLength = 450;
                                tutorial.Reset();
                                lights = false;
                                levelState = Levels.Level1;
                            }

                            tutorial.Junior.SignText = tutorialInstructions[tutorialStep];
                            break;
                    }
               

                    if (SingleKeyPress(Keys.E))
                    {
                        state = GameState.Pause;
                    }

                    break;

                case GameState.Pause:
                    //pauses ambient sound
                    buzz.Pause();
                    // Checks if the continue or quit buttons have been clicked to change the state                   
                    buttons[0].Update(gameTime);
                    buttons[1].Update(gameTime);

                    break;

                case GameState.GameOver:

                    //pauses ambient sound
                    buzz.Pause();

                    if (SingleKeyPress(Keys.Enter))
                    {
                        state = GameState.Title;

                        //resets the player's flashlight charge
                        player.BarLength = 450;

                        lights = false;

                        switch (levelState)
                        {
                            case Levels.Tutorial:

                                tutorial.Reset();
                                tutorialStep = 0;

                                break;

                            case Levels.Level1:

                                levelOne.Reset();

                                break;

                            case Levels.Test:

                                testLevel.Reset();

                                break;
                        }

                    }

                    break;
            }
            prevKBS = kbs;
            prevMS = mState;
            base.Update(gameTime);
        }

        /// <summary>
        /// Takes a key and checks if that key has been pressed once
        /// Checks if the key is pressed and released
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool SingleKeyPress(Keys key)
        {
            KeyboardState kbs = Keyboard.GetState();
            if (prevKBS.IsKeyDown(key) && kbs.IsKeyUp(key))
            {
                return true;
            }
            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            switch (state)
            {
                case GameState.Title:
                    _spriteBatch.Draw(coverArt, new Rectangle(width/5,-50, height, height), Color.White);
                   _spriteBatch.DrawString(defaultFont, "Press ENTER to Start", new Vector2(width/2 - 170, height/2+280), Color.White);
                    
                    break;

                case GameState.LevelSelect:
                    {
                        MouseState mState = Mouse.GetState();

                        //instructions

                        buttons[2].Draw(_spriteBatch);
                        buttons[3].Draw(_spriteBatch);
                        buttons[4].Draw(_spriteBatch);
                        buttons[5].Draw(_spriteBatch);

                        // Draws the top drawer open if the mouse is over it
                        if (buttons[2].ButtonRectangle.Contains(mState.Position))
                        {
                            _spriteBatch.Draw(drawerTop, new Rectangle(width / 2 - 500, height / 2 - 412, 1000, 850), Color.White);
                        }
                        // Draws te middle drawer open if the mouse is over it
                        else if (buttons[3].ButtonRectangle.Contains(mState.Position))
                        {
                            _spriteBatch.Draw(drawerMiddle, new Rectangle(width / 2 - 500, height / 2 - 412, 1000, 850), Color.White);
                        }
                        // Draws te bottom drawer open if the mouse is over it
                        else if (buttons[4].ButtonRectangle.Contains(mState.Position))
                        {
                            _spriteBatch.Draw(drawerBottom, new Rectangle(width / 2 - 500, height / 2 - 412, 1000, 850), Color.White);
                        }
                        // Draws the default drawer if the mouse is not over any of the drawers
                        else
                        {
                            _spriteBatch.Draw(drawer, new Rectangle(width / 2 - 500, height / 2 - 412, 1000, 850), Color.White);

                        }

                        // Draws teddy bear button with selected outline
                        if (buttons[5].ButtonRectangle.Contains(mState.Position))
                        {
                            _spriteBatch.Draw(teddyBearSelected, new Rectangle(width / 6 - 330, height - 540, 460, 480), Color.White);
                        }
                        // Draws normal teddy bear
                        else
                        {
                            _spriteBatch.Draw(teddyBear, new Rectangle(width / 6 - 330, height - 540, 460, 480), Color.White);

                        }

                        // Shows if god mode is on or off
                        if(godmode)
                        {
                            _spriteBatch.Draw(halo, new Rectangle(width / 6 - 330, height - 610, 460, 480), Color.White);
                        }

                        // Draws the label for the drawers ( need to change the font to fit the drawer)
                        _spriteBatch.DrawString(defaultFont, "Tutorial", new Vector2(width / 2 - 110, height / 3 - 15), Color.White);
                        _spriteBatch.DrawString(defaultFont, "Level 1", new Vector2(width / 2 - 110, height / 2 - 15), Color.White);
                        _spriteBatch.DrawString(defaultFont, "Test Level", new Vector2(width / 2 - 110, height - 300), Color.White);
                        _spriteBatch.DrawString(defaultFont, "God Mode", new Vector2(width / 6 - 130, height - 340), Color.White);


                    }
                    break;

                //Draws (in order) Level, Flashligt, Player, Item UI, Monster
                case GameState.Game:

                    //draws the current level
                    switch (levelState)
                    {
                        // Draws level 1
                        case Levels.Level1:

                            levelOne.Draw(_spriteBatch);

                            if (!lights)
                            {
                                levelOne.DrawRat(_spriteBatch);
                            }

                            break;

                        // Draws test level
                        case Levels.Test:

                            testLevel.Draw(_spriteBatch);

                            break;

                        case Levels.Tutorial:
                            tutorial.Draw(_spriteBatch);
                            break;
                    }               

                    _spriteBatch.End();
                    if (!lights)
                    {
                        player.DrawShapes(_spriteBatch, GraphicsDevice);
                    }
                    _spriteBatch.Begin();

                    player.DrawPlayer(_spriteBatch);

                    //UI
                    battery.Draw(_spriteBatch); //this is the battery used for UI
                    _spriteBatch.DrawString(defaultFont, "(" + battery.Count + ")", new Vector2(80, 23), Color.White);
                    switch (levelState)
                    {
                        case Levels.Level1:

                            // If the stool was collected it can be seen in the top left corner
                            if (levelOne.Stool.WasCollected)
                            {
                                _spriteBatch.Draw(stoolTexture, new Vector2(128, -40), Color.White);
                            }

                            //draws the monsters
                            levelOne.DrawMonster(_spriteBatch);

                            break;

                        case Levels.Test:

                            //draws the monsters
                            testLevel.DrawMonster(_spriteBatch);

                            break;

                        case Levels.Tutorial:
                            if (!lights)
                            {
                                tutorial.DrawRat(_spriteBatch);
                            }

                            // If the stool was collected it can be seen in the top left corner
                            if (tutorial.Stool.WasCollected)
                            {
                                _spriteBatch.Draw(stoolTexture, new Vector2(128, -40), Color.White);
                            }

                            if (tutorialStep == 3 && lights == true)
                            {
                                _spriteBatch.DrawString(defaultFont, "Press F1 to Leave the Basement and Continue", new Vector2(width/2 - 250, height-200), Color.Black);
                            }

                            if(player.OCBarLenth > 0 && player.OverCharged() == false)
                            {
                                if (tutorialSpecialAnimation == 15)
                                {
                                    _spriteBatch.Draw(batteryTexture, new Rectangle(width - 350, 125, 50, 25), Color.DeepSkyBlue);
                                    _spriteBatch.Draw(batteryTexture, new Rectangle(width - 300, 125, 50, 25), Color.DeepSkyBlue);
                                    tutorialSpecialAnimation-=10;
                                }
                                else if (tutorialSpecialAnimation >= 0)
                                {
                                        _spriteBatch.Draw(batteryTexture, new Rectangle(width - 350, 125, 50, 25), Color.White);
                                        _spriteBatch.Draw(batteryTexture, new Rectangle(width - 300, 125, 50, 25), Color.White);
                                        tutorialSpecialAnimation++;
                                }
                            }

                            break;
                    }
                    
                    
                    break;

                // Draws the contents of the game but the player is unable to interact with them
                case GameState.Pause:

                    mState = Mouse.GetState();

                    // Draws the level 
                    switch (levelState)
                    {
                        case Levels.Level1:

                            levelOne.Draw(_spriteBatch);

                            break;

                        case Levels.Test:

                            testLevel.Draw(_spriteBatch);

                            break;

                        case Levels.Tutorial:

                            tutorial.Draw(_spriteBatch);

                            break;
                    }               

                    // player and flashlight draw logic
                    _spriteBatch.End();
                    //draws flashlight shapes to cover the level
                    if (!lights)
                    {
                        player.DrawShapes(_spriteBatch, GraphicsDevice);
                    }
                    //draws player above shapes
                    _spriteBatch.Begin();
                    player.DrawPlayer(_spriteBatch);

                    // Draws the monster for what ever level your on
                    switch (levelState)
                    {
                        case Levels.Level1:

                            levelOne.DrawMonster(_spriteBatch);

                            break;

                        case Levels.Test:

                            testLevel.DrawMonster(_spriteBatch);

                            break;
                        case Levels.Tutorial:
                            break;
                    }

                    // Draws the quit and continue buttons 
                    buttons[0].Draw(_spriteBatch);
                    buttons[1].Draw(_spriteBatch);

                    // Draws the top drawer open if the mouse is over it
                    if (buttons[0].ButtonRectangle.Contains(mState.Position))
                    {
                        _spriteBatch.Draw(drawerSmallTop, new Rectangle(width / 2 - 500, height / 2 - 300, 1000, 650), Color.White);
                    }
                    // Draws the Bottom drawer open if the mouse is over it
                    else if (buttons[1].ButtonRectangle.Contains(mState.Position))
                    {
                        _spriteBatch.Draw(drawerSmallBottom, new Rectangle(width / 2 - 500, height / 2 - 300, 1000, 650), Color.White);
                    }
                    
                    // Draws the default drawer if the mouse is not over any of the drawers
                    else
                    {
                        _spriteBatch.Draw(drawerSmall, new Rectangle(width / 2 - 500, height / 2 - 300, 1000, 650), Color.White);

                    }

                    // Draws the label for the drawers ( need to change the font to fit the drawer)
                    _spriteBatch.DrawString(defaultFont, "Continue", new Vector2(width / 2 - 110, height / 3 - 15), Color.White);
                    _spriteBatch.DrawString(defaultFont, "Quit", new Vector2(width / 2 - 110, height / 2 + 100), Color.White);

                    break;

                case GameState.GameOver:
                    if (!levelOne.LightsOn)
                    {
                        _spriteBatch.Draw(monsterFace, new Rectangle(width/2 - height/2, 0, height, height), Color.White);
                        _spriteBatch.DrawString(defaultFont, "Game Over", new Vector2(width / 2 - 100, 400), Color.White);
                    }
                    else
                    {
                        _spriteBatch.DrawString(defaultFont, "You Win !", new Vector2(width / 2, height / 2), Color.White);
                    }
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Switches to the game state
        /// </summary>
        public void SwitchToGame()
        {
            state = GameState.Game;
        }

        /// <summary>
        /// Switches to the title state
        /// </summary>
        public void SwitchToGameOver()
        {
            state = GameState.GameOver;
        }

        /// <summary>
        /// Plays test level only
        /// </summary>
        public void PlayTestLevel()
        {
            levelState = Levels.Test;
        }

        /// <summary>
        /// Plays level 1 only
        /// </summary>
        public void PlayLevel1()
        {
            levelState = Levels.Level1;
        }

        /// <summary>
        /// Plays tutorial only
        /// </summary>
        public void Tutorial()
        {
            levelState = Levels.Tutorial;
        }

        /// <summary>
        /// Turn on and off god mode
        /// </summary>
        public void GodMode()
        {
            godmode = !godmode;
            player.GodMode = godmode;
        }

    }
}