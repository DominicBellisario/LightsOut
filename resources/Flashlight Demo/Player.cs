using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Flashlight_Demo
{

    //holds the directions the player can be in
    enum AnimationState
    {
        FaceNorth,
        FaceEast,
        FaceSouth,
        FaceWest
    }


    internal class Player
    {
        //player hitbox and position
        private Rectangle position;

        private AnimationState state;

        //entire spritesheet
        private Texture2D spriteSheet;

        private SpriteFont testText;

        // Walking animation
        // The current animation frame
        int frame;
        // The amount of time that has passed
        double timeCounter;
        // The speed of the animation
        double fps;
        // The amount of time (in fractional seconds) per frame
        double timePerFrame;
        // The number of frames in the animation 
        const int WalkFrameCount = 4;

        // How far down in the image are the frames?       
        private const int PlayerRectOffsetY = 100;
        //how far across in the image are the frames?   
        private const int PlayerRectOffsetX = 20;
        // The height of a single frame    
        private const int PlayerRectHeight = 460;
        // The width of a single frame     
        private const int PlayerRectWidth = 360;
        //length of the flashlight beam
        private const int beamLength = 500;
        //width of the flashlight beam (radians)
        private const double beamWidth = .4;

        //the x and y lengths of the points of the flashlight triangle
        private float playerCenterX;
        private float playerCenterY;
        //private float beamX2;
        //private float beamY2;
        //private float beamX3;
        //private float beamY3;
        //left triangle
        private float t1X2;
        private float t1Y2;
        private float t1X3;
        private float t1Y3;
        //right triangle
        private float t2X2;
        private float t2Y2;
        private float t2X3;
        private float t2Y3;
        //back triangle
        private float t3X;
        private float t3Y;
        private float triAngle;

        //used to calculate lengths
        private double angle;
        private double angleRad;

        //used to switch between idle and moving animations
        private bool isMoving;

        //red and white rectangle
        private Texture2D redRect;
        private Texture2D whiteRect;

        //collision detection
        private bool isColliding;
        private Rectangle rectangle;
        private const int rectX = 300;
        private const int rectY = 300;
        private const int rectWidth = 100;
        private int rectHeight = 100;


        //paramaterized consturctor
        public Player(Rectangle position, Texture2D spriteSheet, Texture2D redRect, Texture2D whiteRect, SpriteFont testText)
        {
            //places player in the middle of the screen
            this.position = position;
            this.spriteSheet = spriteSheet;
            //player starts by facing north and not moving
            state = AnimationState.FaceNorth;
            isMoving = false;
            //player will walk at 4 animations per second
            fps = 8.0;
            timePerFrame = 1.0 / fps;
            //rectangles
            this.redRect = redRect;
            this.whiteRect = whiteRect;
            //nothing is hitting the flashlight at first
            isColliding = false;
            rectangle = new Rectangle(rectX, rectY, rectWidth, rectHeight);

            this.testText = testText;
        }

        /// <summary>
        /// Draws the player onto the given sprite batch based on his current state (and the current frame in the animation)
        /// Draws flashlight beam
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            //draws a white or red rectangle depending on if the flashlight beam collided with the block
            if (isColliding)
            {
                spriteBatch.Draw(redRect, new Vector2(300, 300), rectangle, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1);
            }
            else
            {
                spriteBatch.Draw(whiteRect, new Vector2(300, 300), rectangle, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 1);
            }

            //debug stuff
            //spriteBatch.DrawString(testText, playerCenterX + ", " + playerCenterY +
            //   ",   " + beamX2 + ", " + beamY2 + ",   " + beamX3 + ", " + beamY3 + ",  " + angleRad, new Vector2(30, 30), Color.Black);
        }

        /// <summary>
        /// manages player movement, the direction the player is facing, and the position of the flashlight
        /// also manages player animation timing
        /// should only run if game is in game state
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //gets keyboard and mouse states
            KeyboardState kbs = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            //player movement is 10 px / tick
            //user can press two keys together to move diagonaly
            //W = move north
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                position.Y -= 10;
                isMoving = true;
            }
            //D = move east
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.X += 10;
                isMoving = true;
            }
            //S = move south
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                position.Y += 10;
                isMoving = true;
            }
            //A = move west
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.X -= 10;
                isMoving = true;
            }
            //if none of these keys are pressed, the player is not moving
            if (Keyboard.GetState().IsKeyUp(Keys.W) &&
               Keyboard.GetState().IsKeyUp(Keys.A) &&
               Keyboard.GetState().IsKeyUp(Keys.S) &&
               Keyboard.GetState().IsKeyUp(Keys.D))
            {
                isMoving = false;
            }

            //player faces in the direction of the mouse
            double xDis = Math.Abs(ms.X - position.X - (PlayerRectWidth / 2));
            double yDis = Math.Abs(ms.Y - position.Y - (PlayerRectHeight / 2));
            angle = Math.Atan2(xDis, yDis) * 180 / Math.PI;
            angleRad = Math.Atan2(xDis, yDis);
            //calculates the center of the player
            playerCenterX = position.X + (PlayerRectWidth / 2);
            playerCenterY = position.Y + (PlayerRectHeight / 2);

            //mouse is north of the player
            if (angle <= 45 && ms.Y < playerCenterY)
            {
                state = AnimationState.FaceNorth;
            }
            //mouse is east of the player
            else if (angle >= 45 && ms.X > playerCenterX)
            {
                state = AnimationState.FaceEast;
            }
            //mouse is south of the player
            else if (angle <= 45 && ms.Y > playerCenterY)
            {
                state = AnimationState.FaceSouth;
            }
            //mouse is west of the player
            else if (angle >= 45 && ms.X < playerCenterX)
            {
                state = AnimationState.FaceWest;
            }

            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame

            // How much time has passed?  
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (timeCounter >= timePerFrame)
            {
                frame += 1;                     // Adjust the frame to the next image

                if (frame > WalkFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                    frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                // This keeps the time passed 
            }

            //gets mouse
            MouseState mState = Mouse.GetState();

            //calculates the distance each point will be from the player
            //mouse is in the 1st quadrant : +x, +y : bottom right
            //unused triangle commented out
            if (playerCenterX < mState.X && playerCenterY < mState.Y)
            {
                //beamX2 = (float)(beamLength * Math.Sin(angleRad + beamWidth) + playerCenterX);
                //beamY2 = (float)(beamLength * Math.Cos(angleRad + beamWidth) + playerCenterY);
                //beamX3 = (float)(beamLength * Math.Sin(angleRad - beamWidth) + playerCenterX);
                //beamY3 = (float)(beamLength * Math.Cos(angleRad - beamWidth) + playerCenterY);
                t1X2 = (float)(10000 * Math.Sin(angleRad + beamWidth) + playerCenterX);
                t1Y2 = (float)(10000 * Math.Cos(angleRad + beamWidth) + playerCenterY);
                t1X3 = (float)(10000 * Math.Sin(angleRad + 1.57) + playerCenterX);
                t1Y3 = (float)(10000 * Math.Cos(angleRad + 1.57) + playerCenterY);

                t2X2 = (float)(10000 * Math.Sin(angleRad - beamWidth) + playerCenterX);
                t2Y2 = (float)(10000 * Math.Cos(angleRad - beamWidth) + playerCenterY);
                t2X3 = (float)(10000 * Math.Sin(angleRad - 1.57) + playerCenterX);
                t2Y3 = (float)(10000 * Math.Cos(angleRad - 1.57) + playerCenterY);

                t3X = (float)(1300 * Math.Sin(angleRad + Math.PI) + playerCenterX);
                t3Y = (float)(1300 * Math.Cos(angleRad + Math.PI) + playerCenterY);
                triAngle = (float)(-angleRad + (Math.PI * 2 / 3));
            }
            //mouse is in the 2nd quadrant : +x, -y : top right
            else if (playerCenterX < mState.X && playerCenterY > mState.Y)
            {
                //beamX2 = (float)(beamLength * Math.Sin(angleRad - beamWidth) + playerCenterX);
                //beamY2 = (float)-(beamLength * Math.Cos(angleRad - beamWidth) - playerCenterY);
                //beamX3 = (float)(beamLength * Math.Sin(angleRad + beamWidth) + playerCenterX);
                //beamY3 = (float)-(beamLength * Math.Cos(angleRad + beamWidth) - playerCenterY);
                t1X2 = (float)(10000 * Math.Sin(angleRad - beamWidth) + playerCenterX);
                t1Y2 = (float)-(10000 * Math.Cos(angleRad - beamWidth) - playerCenterY);
                t1X3 = (float)(10000 * Math.Sin(angleRad - 1.57) + playerCenterX);
                t1Y3 = (float)-(10000 * Math.Cos(angleRad - 1.57) - playerCenterY);

                t2X2 = (float)(10000 * Math.Sin(angleRad + beamWidth) + playerCenterX);
                t2Y2 = (float)-(10000 * Math.Cos(angleRad + beamWidth) - playerCenterY);
                t2X3 = (float)(10000 * Math.Sin(angleRad + 1.57) + playerCenterX);
                t2Y3 = (float)-(10000 * Math.Cos(angleRad + 1.57) - playerCenterY);

                t3X = (float)(1300 * Math.Sin(angleRad + Math.PI) + playerCenterX);
                t3Y = (float)-(1300 * Math.Cos(angleRad + Math.PI) - playerCenterY);
                triAngle = (float)(angleRad + (Math.PI * 1 / 3));
            }
            //mouse is in the 3rd quadrant : -x, -y : top left
            else if (playerCenterX > mState.X && playerCenterY > mState.Y)
            {
                //beamX2 = (float)-(beamLength * Math.Sin(angleRad + beamWidth) - playerCenterX);
                //beamY2 = (float)-(beamLength * Math.Cos(angleRad + beamWidth) - playerCenterY);
                //beamX3 = (float)-(beamLength * Math.Sin(angleRad - beamWidth) - playerCenterX);
                //beamY3 = (float)-(beamLength * Math.Cos(angleRad - beamWidth) - playerCenterY);
                t1X2 = (float)-(10000 * Math.Sin(angleRad + beamWidth) - playerCenterX);
                t1Y2 = (float)-(10000 * Math.Cos(angleRad + beamWidth) - playerCenterY);
                t1X3 = (float)-(10000 * Math.Sin(angleRad + 1.57) - playerCenterX);
                t1Y3 = (float)-(10000 * Math.Cos(angleRad + 1.57) - playerCenterY);

                t2X2 = (float)-(10000 * Math.Sin(angleRad - beamWidth) - playerCenterX);
                t2Y2 = (float)-(10000 * Math.Cos(angleRad - beamWidth) - playerCenterY);
                t2X3 = (float)-(10000 * Math.Sin(angleRad - 1.57) - playerCenterX);
                t2Y3 = (float)-(10000 * Math.Cos(angleRad - 1.57) - playerCenterY);

                t3X = (float)-(1300 * Math.Sin(angleRad + Math.PI) - playerCenterX);
                t3Y = (float)-(1300 * Math.Cos(angleRad + Math.PI) - playerCenterY);
                triAngle = (float)(-angleRad + (Math.PI * 1 / 3));
            }
            //mouse is in the 4th quadrant : -x, +y : bottom left
            else if (playerCenterX > mState.X && playerCenterY < mState.Y)
            {
                //beamX2 = (float)-(beamLength * Math.Sin(angleRad - beamWidth) - playerCenterX);
                //beamY2 = (float)(beamLength * Math.Cos(angleRad - beamWidth) + playerCenterY);
                //beamX3 = (float)-(beamLength * Math.Sin(angleRad + beamWidth) - playerCenterX);
                //beamY3 = (float)(beamLength * Math.Cos(angleRad + beamWidth) + playerCenterY);
                t1X2 = (float)-(10000 * Math.Sin(angleRad - beamWidth) - playerCenterX);
                t1Y2 = (float)(10000 * Math.Cos(angleRad - beamWidth) + playerCenterY);
                t1X3 = (float)-(10000 * Math.Sin(angleRad - 1.57) - playerCenterX);
                t1Y3 = (float)(10000 * Math.Cos(angleRad - 1.57) + playerCenterY);

                t2X2 = (float)-(10000 * Math.Sin(angleRad + beamWidth) - playerCenterX);
                t2Y2 = (float)(10000 * Math.Cos(angleRad + beamWidth) + playerCenterY);
                t2X3 = (float)-(10000 * Math.Sin(angleRad + 1.57) - playerCenterX);
                t2Y3 = (float)(10000 * Math.Cos(angleRad + 1.57) + playerCenterY);

                t3X = (float)-(1300 * Math.Sin(angleRad + Math.PI) - playerCenterX);
                t3Y = (float)(1300 * Math.Cos(angleRad + Math.PI) + playerCenterY);
                triAngle = (float)(angleRad + (Math.PI * 2 / 3));
            }

            //collision detection between flashlight and rectangle
            //checks each point of the rectangle to see if it is in between the x distance of the player and flashlight
            /*
            //left side
            if((beamX2 <= rectangle.X && rectangle.X <= playerCenterX) ||
               (beamX2 >= rectangle.X && rectangle.X >= playerCenterX) ||
               (beamX3 <= rectangle.X && rectangle.X <= playerCenterX) ||
               (beamX3 >= rectangle.X && rectangle.X >= playerCenterX))
            {
                //top left point
                if((beamY2 <= rectangle.Y && rectangle.Y <= playerCenterY) ||
                   (beamY2 >= rectangle.Y && rectangle.Y >= playerCenterY) ||
                   (beamY3 <= rectangle.Y && rectangle.Y <= playerCenterY) ||
                   (beamY3 >= rectangle.Y && rectangle.Y >= playerCenterY))
                {
                    isColliding = true;
                }
                //bottom left point
                else if((beamY2 <= rectangle.Y + rectangle.Height && rectangle.Y + rectangle.Height <= playerCenterY) ||
                       (beamY2 >= rectangle.Y + rectangle.Height && rectangle.Y + rectangle.Height >= playerCenterY)  ||
                       (beamY3 <= rectangle.Y + rectangle.Height && rectangle.Y + rectangle.Height <= playerCenterY)  ||
                       (beamY3 >= rectangle.Y && rectangle.Y >= playerCenterY))
                {
                    isColliding = true;
                }
                else
                {
                    isColliding = false;
                }
                
            }
            //right side
            else if ((beamX2 <= rectangle.X + rectangle.Width && rectangle.X + rectangle.Width <= playerCenterX) ||
                    (beamX2 >= rectangle.X + rectangle.Width && rectangle.X + rectangle.Width >= playerCenterX) ||
                    (beamX3 <= rectangle.X + rectangle.Width && rectangle.X + rectangle.Width <= playerCenterX) ||
                    (beamX3 >= rectangle.X + rectangle.Width && rectangle.X + rectangle.Width >= playerCenterX))
            {
                //top right point
                if ((beamY2 <= rectangle.Y && rectangle.Y <= playerCenterY) ||
                   (beamY2 >= rectangle.Y && rectangle.Y >= playerCenterY) ||
                   (beamY3 <= rectangle.Y && rectangle.Y <= playerCenterY) ||
                   (beamY3 >= rectangle.Y && rectangle.Y >= playerCenterY))
                {
                    isColliding = true;
                }
                //bottom right point
                else if ((beamY2 <= rectangle.Y + rectangle.Height && rectangle.Y + rectangle.Height <= playerCenterY) ||
                       (beamY2 >= rectangle.Y + rectangle.Height && rectangle.Y + rectangle.Height >= playerCenterY) ||
                       (beamY3 <= rectangle.Y + rectangle.Height && rectangle.Y + rectangle.Height <= playerCenterY) ||
                       (beamY3 >= rectangle.Y && rectangle.Y >= playerCenterY))
                {
                    isColliding = true;
                }
                else
                {
                    isColliding = false;
                }
            }
            else
            {
                isColliding = false;
            }
            */
        }

        /// <summary>
        /// draws shapes from shapeBatch 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="graphics"></param>
        public void DrawShapes(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            //generates triangles with specified points
            ShapeBatch.Begin(graphics);
            //left triangle
            ShapeBatch.Triangle(
                new Vector2(playerCenterX, playerCenterY),
                new Vector2(t1X2, t1Y2),
                new Vector2(t1X3, t1Y3),
                Color.Black);
            //right triangle
            ShapeBatch.Triangle(
                new Vector2(playerCenterX, playerCenterY),
                new Vector2(t2X2, t2Y2),
                new Vector2(t2X3, t2Y3),
                Color.Black);
            //back triangle
            ShapeBatch.Triangle(
                new Vector2(t3X, t3Y), 4000, triAngle, Color.Black);

            //unused triangle
            //ShapeBatch.Triangle(
            //new Vector2(playerCenterX, playerCenterY), //player point
            //new Vector2(beamX2, beamY2), //left point
            //new Vector2(beamX3, beamY3), //right point
            //Color.White);
            ShapeBatch.End();
        }

        /// <summary>
        /// draws player animations
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="graphics"></param>
        public void DrawPlayer(SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            //manages player animations.  Walking states are a work in progress and are commented out
            //                                                      (he goes invisible when he moves right now)
            switch (state)
            {

                //player faces north
                case AnimationState.FaceNorth:
                    if (isMoving)
                    {
                        DrawWalkingNorth(SpriteEffects.None, spriteBatch);
                    }
                    else
                    {
                        DrawFacingNorth(SpriteEffects.None, spriteBatch);
                    }
                    break;

                //player faces east
                case AnimationState.FaceEast:
                    if (isMoving)
                    {
                        DrawWalkingHorizontal(SpriteEffects.FlipHorizontally, spriteBatch);
                    }
                    else
                    {
                        DrawFacingHorizontal(SpriteEffects.FlipHorizontally, spriteBatch);
                    }
                    break;

                //player faces south
                case AnimationState.FaceSouth:
                    if (isMoving)
                    {
                        DrawWalkingSouth(SpriteEffects.None, spriteBatch);
                    }
                    else
                    {
                        DrawFacingSouth(SpriteEffects.None, spriteBatch);
                    }
                    break;

                //player faces west
                case AnimationState.FaceWest:
                    if (isMoving)
                    {
                        DrawWalkingHorizontal(SpriteEffects.None, spriteBatch);
                    }
                    else
                    {
                        DrawFacingHorizontal(SpriteEffects.None, spriteBatch);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// draws the player facing north
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawFacingNorth(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(position.X, position.Y),
                new Rectangle(PlayerRectOffsetX, (PlayerRectOffsetY * 2) + PlayerRectHeight + 20,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player walking north
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawWalkingNorth(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(position.X, position.Y),
                new Rectangle((PlayerRectOffsetX * frame) + (PlayerRectWidth * (frame - 1)), (PlayerRectOffsetY * 2) + PlayerRectHeight + 20,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player facing east or west
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawFacingHorizontal(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(position.X, position.Y),
                new Rectangle(PlayerRectOffsetX, (PlayerRectOffsetY * 3) + (PlayerRectHeight * 2) + 60,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player walking east or west
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawWalkingHorizontal(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(position.X, position.Y),
                new Rectangle((PlayerRectOffsetX * frame) + (PlayerRectWidth * (frame - 1)), (PlayerRectOffsetY * 3) + (PlayerRectHeight * 2) + 60,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player facing south
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawFacingSouth(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(position.X, position.Y),
                new Rectangle(PlayerRectOffsetX, PlayerRectOffsetY,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player walking south
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawWalkingSouth(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(position.X, position.Y),
                new Rectangle((PlayerRectOffsetX * frame) + (PlayerRectWidth * (frame - 1)), PlayerRectOffsetY,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }
    }
}
