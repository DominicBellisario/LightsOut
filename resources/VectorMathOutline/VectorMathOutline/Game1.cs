using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VectorMathOutline
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Rectangle[,] house;
        private Texture2D squareTexture;
        private VectorLight flashLight;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();

            flashLight = new VectorLight();
            house = new Rectangle[100, 100];

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            squareTexture = this.Content.Load<Texture2D>("Square");

            for(int i=0; i < 100; i++)
            {
                for(int j=0; j < 100; j++)
                {
                    house[i, j] = new Rectangle(i * 32, j * 32, 32, 32);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            Color currentColor = Color.White;
            _spriteBatch.Begin();

            flashLight.Direction(700, 700);
            for(int i=0; i < 100; i++)
            {
                for(int j=0; j < 100; j++)
                {
                    if (flashLight.Intersects(house[i, j], 700, 700))
                    {
                        currentColor = Color.Red;
                        _spriteBatch.Draw(squareTexture, house[i, j], currentColor);
                    }
                    
                    currentColor = Color.White;
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}