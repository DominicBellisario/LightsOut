using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Flashlight_Demo
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont testText;

        //player object
        private Player player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // sets the width of the screen
            _graphics.PreferredBackBufferWidth = 1900;

            // sets the height of the screen
            _graphics.PreferredBackBufferHeight = 1400;

            this.IsMouseVisible = true;

            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            // TODO: use this.Content to load your game content here
            //loads player position
            Rectangle position = new Rectangle(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2, 340, 160);

            testText = Content.Load<SpriteFont>("testText");

            //load spritesheet, rectangles, and make player object
            Texture2D spritesheet = Content.Load<Texture2D>("spritesheet");
            Texture2D redRect = Content.Load<Texture2D>("redRect");
            player = new Player(position, spritesheet, redRect, redRect, testText);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            player.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //draws level
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            player.Draw(_spriteBatch, GraphicsDevice);
            _spriteBatch.End();
            //draws shapes to cover the level
            player.DrawShapes(_spriteBatch, GraphicsDevice);
            //draws player above shapes
            _spriteBatch.Begin();
            player.DrawPlayer(_spriteBatch, GraphicsDevice);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}