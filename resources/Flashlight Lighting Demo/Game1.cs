using Flashlight_Demo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Flashlight_Lighting_Demo
{
    //Currently draws a line to mouse position, will change to column/triangle later.
    //Window is 160x160 so it fits 5x5 tiles at 32x32.
    //Player class created, but waiting to finish flashlight mechanics before implementing.
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D tAsset;

        private Tile tile1;

        //used to draw line
        private Vector2 lineStart;
        private Vector2 lineEnd;
        private Rectangle LineCollision;
        
        //list of tiles
        private List<Tile> tiles;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            //sets window size to accomodate tile sizes
            _graphics.PreferredBackBufferWidth = 160;
            _graphics.PreferredBackBufferHeight = 160;

            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //line starts in middle of screen
            lineStart.X = _graphics.PreferredBackBufferWidth / 2; 
            lineStart.Y = _graphics.PreferredBackBufferHeight / 2;

            tiles = new List<Tile>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            

            tAsset = Content.Load<Texture2D>("FloorTile");
            tile1 = new Tile(tAsset, 0, 0);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            MouseState mState = Mouse.GetState();
            //line end always where mouse cursor is.
            lineEnd.X = mState.Position.X;
            lineEnd.Y = mState.Position.Y;

            LineCollision.X = (int)lineEnd.X;
            LineCollision.Y = (int)lineEnd.Y;


            tile1.Collides(LineCollision);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            ShapeBatch.Begin(GraphicsDevice);

            //draws line
            ShapeBatch.Line(lineStart, lineEnd, Color.White);
            tile1.Draw(_spriteBatch);


            ShapeBatch.End();

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}