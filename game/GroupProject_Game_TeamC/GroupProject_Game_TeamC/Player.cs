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
using Microsoft.Xna.Framework.Audio;

namespace GroupProject_Game_TeamC
{
    /// <summary>
    /// the different directions the player can be facing
    /// </summary>
    enum AnimationState
    {
        FaceNorth,
        FaceEast,
        FaceSouth,
        FaceWest,
        IdleFront,
        IdleBack
    }


    internal class Player
    {
        private double windowWidth;
        private double windowHeight;

        private KeyboardState prevKBS;
        private MouseState prevMS;

        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        private Random rng;

        private AnimationState state;

        //entire spritesheet
        private Texture2D spriteSheet;

        // The current animation frame
        private int frame;
        private double dFrame;
        // The amount of time that has passed
        private double timeCounter;
        // The speed of the animation
        private double fps;
        // The amount of time (in fractional seconds) per frame
        private double timePerFrame;
        // The number of frames in the animation 
        private int WalkFrameCount = 5;
        // the time it takes to switch to idle animation
        private const int idleTimer = 8000;

        // How far down in the image are the frames?       
        private const int PlayerRectOffsetY = 100;
        //how far across in the image are the frames?   
        private const int PlayerRectOffsetX = 20;
        // The height of a single frame    
        private const int PlayerRectHeight = 100;
        // The width of a single frame     
        private const int PlayerRectWidth = 100;

        //NOTE: increase or decrease to change the length of the beam (does not change collisions)
        private const int beamLength = 900;
        //NOTE: increase or decrease to change the width of the beam (radians) (does not change collisions)
        private const double beamWidth = 0.4;
        //scalar that determines the size of the blinders
        private const int blinderSize = 5000;

        //the x and y lengths of the points of the flashlight triangle
        private float playerCenterX;
        private float playerCenterY;
        private float beamX2;
        private float beamY2;
        private float beamX3;
        private float beamY3;
        private const double backTriDistance = 1666.5;
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
        //far triangle
        private float farTriPointX;
        private float farTriPointY;

        //used to calculate lengths
        private double angle;
        private double angleRad;

        //used to switch between idle and moving animations
        private bool isMoving;
        private bool isMouseMoving;

        //battery mechanics (oC = overCharge)
        private bool canOvercharge;
        private bool overCharged;
        private bool overChargedEffect;
        private double barLength;
        private double oCBarLength;

        private Color barColor;
        private Color oCBarColor;
        private Color beamColor;
        private Color barBorderColor;

        private int rgb;
        private float blinderOpacity;
        private float beamOpacity;
        private float gradientOpacity;
        private int pulse;

        //changes the frquency of flickers (1/value chance per tick)
        private const int flickerSpeed = 21;
        //time for the charge bar to pulse colors (value/gameSpeed of a second)
        private const int pulseSpeed = 10;

        //how fast the battery drains normally (pixel amount/tick)
        private const float normalDrainSpeed = 0.12f;
        //how fast the battery drains while overcharged (pixel amount/tick)
        private const float oCDrainSpeed = .5f;
        //how fast the overcharge meter charges(pixel amount/tick)
        private const float oCMeterChargeSpeed = 5f;
        //amount (in pixels) a battery can recharge (max charge is 450
        private const int rechargeAmount = 110;
        //how much battery is used when stunning the monster
        private const int oCBlastAmount = 30;

        //the opacity of the blinders and gradient normally
        private const float normalBlinderOpacity = 0.93f;
        //the opacity of the blinders and gradient while overcharged
        private const float oCBlinderOpacity = 1;
        //the opacity of the blinders, gradient, and beam while flashlight is off
        private const float offOpacity = 0.93f;

        //the opacity of the flashlight beam normally
        private const float normalBeamOpacity = 0.1f;
        //the opacity of the flashlight beam while overcharged
        private const float oCBeamOpacity = 1;
        //the opacity of the flashlight beam while flickering
        private const float flickerBeamOpacity = 0.93f;

        //sounds
        private List<SoundEffect> soundEffects;
        private SoundEffectInstance overcharge;
        private SoundEffectInstance shoot;
        private SoundEffectInstance walk1;
        private SoundEffectInstance walk2;

        private bool godMode;

        /// <summary>
        /// Returns/Sets Barlength for battery bar
        /// </summary>
        public double BarLength
        {
            get { return barLength; }
            set { barLength = value; }
        }

        /// <summary>
        /// Returns the length of the overcharge bar
        /// </summary>
        public double OCBarLenth
        {
            get { return oCBarLength; }
        }

        /// <summary>
        /// returns the players x coord
        /// </summary>
        public float PlayerCenterX
        {
            get { return playerCenterX; }
        }
        /// <summary>
        /// returns the players y coord
        /// </summary>
        public float PlayerCenterY
        {
            get { return playerCenterY; }
        }

        public bool OverCharged()
        {
            return overChargedEffect;
        }

        public bool GodMode
        {
            get { return godMode; }
            set { godMode = value; }
        }
        
        //paramaterized consturctor
        public Player(Texture2D spriteSheet, double windowWidth, double windowHeight, List<SoundEffect> soundEffects)  
        {
            this.spriteSheet = spriteSheet;
            //player starts by facing north and not moving
            state = AnimationState.FaceNorth;
            isMoving = false;

            //player will walk at 8 animations per second
            fps = 8.0;
            timePerFrame = 1.0 / fps;

            //flashlight is not overcharged
            canOvercharge = true;
            overChargedEffect = false;
            overCharged = false;

            //battery charge begins at 100%
            barLength = 450;

            //overcharge meter is not charged at all
            oCBarLength = 0;
            barColor = Color.Green;
            oCBarColor = Color.Turquoise;
            barBorderColor = Color.White;
            beamColor = Color.Black;
            rgb = 255;

            //flashlight effects are set to normal
            blinderOpacity = 0.93f;
            beamOpacity = 0.1f;
            gradientOpacity = 0.93f;
            pulse = 0;

            //player is positioned at the center of the screen
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            playerCenterX = (float)windowWidth / 2;
            playerCenterY = (float)windowHeight / 2;
            
            //brings in the sound effect list and makes instances of sounds player uses
            this.soundEffects = soundEffects;
            overcharge = this.soundEffects[1].CreateInstance();
            shoot = this.soundEffects[3].CreateInstance();
            walk1 = this.soundEffects[8].CreateInstance();
            walk2 = this.soundEffects[9].CreateInstance();

            //godmode is off by default
            godMode = false;

            rng = new Random();
        }

        /// <summary>
        /// manages the direction the player is facing, the position and effects of the flashlight, battery logic, and player animation timing
        /// only updates if game is in game state
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="battery"></param>
        public void Update(GameTime gameTime, Battery battery)
        {
            //gets mouse states
            MouseState ms = Mouse.GetState();

            //manages player animations (movement is in level loader)
            //the player is moving if W, A, S, or D are pressed
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                isMoving = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                isMoving = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                isMoving = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
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
            //if the mouse if not moving, the timer for the idle animation can start
            if (prevMS.X == ms.X && prevMS.Y == ms.Y)
            {
                isMouseMoving = false;
            }
            else
            {
                isMouseMoving = true;
            }

            //player faces in the direction of the mouse
            double xDis = Math.Abs(ms.X - playerCenterX);
            double yDis = Math.Abs(ms.Y - playerCenterY);
            angle = Math.Atan2(xDis, yDis) * 180 / Math.PI;
            angleRad = Math.Atan2(xDis, yDis);
            //player stands still while the idle timer is counting down
            if (watch.ElapsedMilliseconds < idleTimer)
            {
                //mouse is north of the player
                if (angle <= 45 && ms.Y < (float)windowHeight / 2)
                {
                    state = AnimationState.FaceNorth;
                }
                //mouse is east of the player
                else if (angle >= 45 && ms.X > (float)windowWidth / 2)
                {
                    state = AnimationState.FaceEast;
                }
                //mouse is south of the player
                else if (angle <= 45 && ms.Y > (float)windowHeight / 2)
                {
                    state = AnimationState.FaceSouth;
                }
                //mouse is west of the player
                else if (angle >= 45 && ms.X < (float)windowWidth / 2)
                {
                    state = AnimationState.FaceWest;
                }
            }
            //player switches to idle animation when 
            else
            {
                if (isMoving == false && isMouseMoving == false)
                {
                    //Mouse is not north and character hasn't moved
                    if (angle >= 45 && ms.Y > (float)windowHeight / 2
                        ||
                        angle <= 45 && ms.Y > (float)windowHeight / 2)
                    {
                        state = AnimationState.IdleFront;
                    }
                    //Mouse is north and character hasn't moved
                    else if (angle <= 45 && ms.Y < (float)windowHeight / 2)
                    {
                        state = AnimationState.IdleBack;
                    }
                }
            }

            //restart the idle timer if the player looks somewhere else or moves
            if (isMoving == true || isMouseMoving == true)
            {
                watch.Stop();
                watch.Reset();
            }
            //otherwise, start it
            else
            {
                watch.Start();
            }

            


            // Handle animation timing
            // - Add to the time counter
            // - Check if we have enough "time" to advance the frame

            // How much time has passed?  
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            // If enough time has passed:
            if (state == AnimationState.FaceSouth || state == AnimationState.FaceNorth)
            {
                if (timeCounter >= timePerFrame)
                {
                    WalkFrameCount = 5;
                    frame += 1;                     // Adjust the frame to the next image

                    if (frame > WalkFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                        frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                    timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                    // This keeps the time passed 
                }
            }
            else if (state == AnimationState.FaceWest || state == AnimationState.FaceEast)
            {
                // If enough time has passed:
                if (timeCounter >= timePerFrame)
                {
                    WalkFrameCount = 7;
                    frame += 1;                     // Adjust the frame to the next image

                    if (frame > WalkFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                        frame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                    timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                    // This keeps the time passed 
                }
            }
            else if (state == AnimationState.IdleBack || state == AnimationState.IdleFront)
            {
                if (timeCounter >= timePerFrame)
                {
                    WalkFrameCount = 7;
                    dFrame += .5;                     // Adjust the frame to the next image

                    if (dFrame > WalkFrameCount)     // Check the bounds - have we reached the end of walk cycle?
                        dFrame = 1;                  // Back to 1 (since 0 is the "standing" frame)

                    timeCounter -= timePerFrame;    // Remove the time we "used" - don't reset to 0
                                                    // This keeps the time passed 
                }
            }

            //gets mouse
            MouseState mState = Mouse.GetState();

            //calculates the distance each point will be from the player
            //mouse is in the 1st quadrant : +x, +y : bottom right
            if (playerCenterX < mState.X && playerCenterY < mState.Y)
            {
                beamX2 = (float)(beamLength * Math.Sin(angleRad + beamWidth) + playerCenterX);
                beamY2 = (float)(beamLength * Math.Cos(angleRad + beamWidth) + playerCenterY);
                beamX3 = (float)(beamLength * Math.Sin(angleRad - beamWidth) + playerCenterX);
                beamY3 = (float)(beamLength * Math.Cos(angleRad - beamWidth) + playerCenterY);

                t1X2 = (float)(blinderSize * Math.Sin(angleRad + beamWidth) + playerCenterX);
                t1Y2 = (float)(blinderSize * Math.Cos(angleRad + beamWidth) + playerCenterY);
                t1X3 = (float)(blinderSize * Math.Sin(angleRad + Math.PI / 2) + playerCenterX);
                t1Y3 = (float)(blinderSize * Math.Cos(angleRad + Math.PI / 2) + playerCenterY);

                t2X2 = (float)(blinderSize * Math.Sin(angleRad - beamWidth) + playerCenterX);
                t2Y2 = (float)(blinderSize * Math.Cos(angleRad - beamWidth) + playerCenterY);
                t2X3 = (float)(blinderSize * Math.Sin(angleRad - Math.PI / 2) + playerCenterX);
                t2Y3 = (float)(blinderSize * Math.Cos(angleRad - Math.PI / 2) + playerCenterY);

                t3X = (float)(backTriDistance * Math.Sin(angleRad + Math.PI) + playerCenterX);
                t3Y = (float)(backTriDistance * Math.Cos(angleRad + Math.PI) + playerCenterY);
                triAngle = (float)(-angleRad + (Math.PI * 2 / 3));

                farTriPointX = (float)(blinderSize * Math.Sin(angleRad) + playerCenterX);
                farTriPointY = (float)(blinderSize * Math.Cos(angleRad) + playerCenterY);
            }
            //mouse is in the 2nd quadrant : +x, -y : top right
            else if (playerCenterX < mState.X && playerCenterY > mState.Y)
            {
                beamX2 = (float)(beamLength * Math.Sin(angleRad - beamWidth) + playerCenterX);
                beamY2 = (float)-(beamLength * Math.Cos(angleRad - beamWidth) - playerCenterY);
                beamX3 = (float)(beamLength * Math.Sin(angleRad + beamWidth) + playerCenterX);
                beamY3 = (float)-(beamLength * Math.Cos(angleRad + beamWidth) - playerCenterY);

                t1X2 = (float)(blinderSize * Math.Sin(angleRad - beamWidth) + playerCenterX);
                t1Y2 = (float)-(blinderSize * Math.Cos(angleRad - beamWidth) - playerCenterY);
                t1X3 = (float)(blinderSize * Math.Sin(angleRad - Math.PI / 2) + playerCenterX);
                t1Y3 = (float)-(blinderSize * Math.Cos(angleRad - Math.PI / 2) - playerCenterY);

                t2X2 = (float)(blinderSize * Math.Sin(angleRad + beamWidth) + playerCenterX);
                t2Y2 = (float)-(blinderSize * Math.Cos(angleRad + beamWidth) - playerCenterY);
                t2X3 = (float)(blinderSize * Math.Sin(angleRad + Math.PI / 2) + playerCenterX);
                t2Y3 = (float)-(blinderSize * Math.Cos(angleRad + Math.PI / 2) - playerCenterY);

                t3X = (float)(backTriDistance * Math.Sin(angleRad + Math.PI) + playerCenterX);
                t3Y = (float)-(backTriDistance * Math.Cos(angleRad + Math.PI) - playerCenterY);
                triAngle = (float)(angleRad + (Math.PI * 1 / 3));

                farTriPointX = (float)(blinderSize * Math.Sin(angleRad) + playerCenterX);
                farTriPointY = (float)-(blinderSize * Math.Cos(angleRad) - playerCenterY);
            }
            //mouse is in the 3rd quadrant : -x, -y : top left
            else if (playerCenterX > mState.X && playerCenterY > mState.Y)
            {
                beamX2 = (float)-(beamLength * Math.Sin(angleRad + beamWidth) - playerCenterX);
                beamY2 = (float)-(beamLength * Math.Cos(angleRad + beamWidth) - playerCenterY);
                beamX3 = (float)-(beamLength * Math.Sin(angleRad - beamWidth) - playerCenterX);
                beamY3 = (float)-(beamLength * Math.Cos(angleRad - beamWidth) - playerCenterY);

                t1X2 = (float)-(blinderSize * Math.Sin(angleRad + beamWidth) - playerCenterX);
                t1Y2 = (float)-(blinderSize * Math.Cos(angleRad + beamWidth) - playerCenterY);
                t1X3 = (float)-(blinderSize * Math.Sin(angleRad + Math.PI / 2) - playerCenterX);
                t1Y3 = (float)-(blinderSize * Math.Cos(angleRad + Math.PI / 2) - playerCenterY);

                t2X2 = (float)-(blinderSize * Math.Sin(angleRad - beamWidth) - playerCenterX);
                t2Y2 = (float)-(blinderSize * Math.Cos(angleRad - beamWidth) - playerCenterY);
                t2X3 = (float)-(blinderSize * Math.Sin(angleRad - Math.PI / 2) - playerCenterX);
                t2Y3 = (float)-(blinderSize * Math.Cos(angleRad - Math.PI / 2) - playerCenterY);

                t3X = (float)-(backTriDistance * Math.Sin(angleRad + Math.PI) - playerCenterX);
                t3Y = (float)-(backTriDistance * Math.Cos(angleRad + Math.PI) - playerCenterY);
                triAngle = (float)(-angleRad + (Math.PI * 1 / 3));

                farTriPointX = (float)-(blinderSize * Math.Sin(angleRad) - playerCenterX);
                farTriPointY = (float)-(blinderSize * Math.Cos(angleRad) - playerCenterY);
            }
            //mouse is in the 4th quadrant : -x, +y : bottom left
            else if (playerCenterX > mState.X && playerCenterY < mState.Y)
            {
                beamX2 = (float)-(beamLength * Math.Sin(angleRad - beamWidth) - playerCenterX);
                beamY2 = (float)(beamLength * Math.Cos(angleRad - beamWidth) + playerCenterY);
                beamX3 = (float)-(beamLength * Math.Sin(angleRad + beamWidth) - playerCenterX);
                beamY3 = (float)(beamLength * Math.Cos(angleRad + beamWidth) + playerCenterY);

                t1X2 = (float)-(blinderSize * Math.Sin(angleRad - beamWidth) - playerCenterX);
                t1Y2 = (float)(blinderSize * Math.Cos(angleRad - beamWidth) + playerCenterY);
                t1X3 = (float)-(blinderSize * Math.Sin(angleRad - Math.PI / 2) - playerCenterX);
                t1Y3 = (float)(blinderSize * Math.Cos(angleRad - Math.PI / 2) + playerCenterY);

                t2X2 = (float)-(blinderSize * Math.Sin(angleRad + beamWidth) - playerCenterX);
                t2Y2 = (float)(blinderSize * Math.Cos(angleRad + beamWidth) + playerCenterY);
                t2X3 = (float)-(blinderSize * Math.Sin(angleRad + Math.PI / 2) - playerCenterX);
                t2Y3 = (float)(blinderSize * Math.Cos(angleRad + Math.PI / 2) + playerCenterY);

                t3X = (float)-(backTriDistance * Math.Sin(angleRad + Math.PI) - playerCenterX);
                t3Y = (float)(backTriDistance * Math.Cos(angleRad + Math.PI) + playerCenterY);
                triAngle = (float)(angleRad + (Math.PI * 2 / 3));

                farTriPointX = (float)-(blinderSize * Math.Sin(angleRad) - playerCenterX);
                farTriPointY = (float)(blinderSize * Math.Cos(angleRad) + playerCenterY);
            }

            //battery logic
            //player releases a full charge, overcharging the flashlight
            if (overCharged)
            {
                //turns off the stun (stun only lasts for one frame)
                overChargedEffect = false;
                barBorderColor = Color.Red;
                //quickly resets overcharge meter
                if (oCBarLength > 0)
                {
                    oCBarLength -= 25;
                }
                //effect is still in progress until value is back to normal
                if (blinderOpacity > normalBlinderOpacity)
                {
                    blinderOpacity -= 0.001f;
                    beamOpacity -= 0.013f;
                    gradientOpacity -= 0.001f;
                    rgb -= 4;
                    beamColor = new Color(rgb, rgb, rgb);
                }
                //ends the effect
                else
                {
                    beamColor = Color.Black;
                    rgb = 255;
                    overCharged = false;
                }
            }
            else
            {
                //pressing and holding the right mouse key will charge an attack
                if (mState.LeftButton == ButtonState.Pressed && canOvercharge)
                {
                    if (!godMode)
                    {
                        //battery decreases faster
                        barLength -= oCDrainSpeed;
                    }
                    else
                    {
                        oCBarLength = 449;
                    }
                    //surroundings dim as charge continues
                    blinderOpacity += .001f;
                    gradientOpacity += .001f;
                    
                    //the effects while attack is charging
                    if (oCBarLength < 450)
                    {
                        oCBarLength += oCMeterChargeSpeed;
                        barBorderColor = Color.Yellow;
                    }
                    //the effects while charge is at max
                    else
                    {
                        //caps bar
                        oCBarLength = 450;
                        
                        //increments pulse
                        pulse++;
                        //show one color for half of the time
                        if (pulse < pulseSpeed / 2)
                        {
                            barBorderColor = Color.Red;
                        }
                        //show another for the remainder
                        else if (pulse >= pulseSpeed / 2 && pulse < pulseSpeed)
                        {
                            barBorderColor = Color.Yellow;
                        }
                        //loop the effect
                        else
                        {
                            pulse = 0;
                        }
                    }
                    //plays overcharge sound once when player begins to charge the attack
                    if (prevMS.LeftButton == ButtonState.Released)
                    {
                        overcharge.Play();
                    }
                }
                //releasing the attack while fully charged fires the attack
                else if (mState.LeftButton == ButtonState.Released && oCBarLength == 450)
                {
                    if (oCBarLength == 450)
                    {
                        //stops the overcharge noise
                        overcharge.Stop();
                        //take a chunk off of the battery
                        barLength -= oCBlastAmount;
                        //dims the surroundings
                        blinderOpacity = oCBlinderOpacity;
                        beamOpacity = oCBeamOpacity;
                        gradientOpacity = oCBeamOpacity;
                        //flashlght is currently overcharged
                        overCharged = true;
                        //for one tick, the monster can be stunned
                        overChargedEffect = true;
                        //plays the shooting sound
                        shoot.Play();
                    }
                }
                //the light flickers if low battery
                else if (barLength < 112 && canOvercharge)
                {
                    //slowly lowers any charge built up
                    if (oCBarLength > 0)
                    {
                        oCBarLength -= 0.5f;
                    }
                    //normal effects
                    barLength -= normalDrainSpeed;
                    barBorderColor = Color.White;
                    blinderOpacity = normalBlinderOpacity;
                    gradientOpacity = normalBlinderOpacity;

                    //flashlight has a random chance to turn off for a brief time, creating a flickering effect
                    if (rng.Next(flickerSpeed) == 1 && barLength > 0)
                    {
                        beamOpacity = flickerBeamOpacity;
                    }
                    else
                    {
                        beamOpacity = normalBeamOpacity;
                    }
                }
                //no beam is visible when flashlight is out of battery
                else if (barLength < 0)
                {
                    //slowly lowers any charge built up
                    if (oCBarLength > 0)
                    {
                        oCBarLength -= 0.5f;
                    }
                    //no light effects
                    barBorderColor = Color.White;
                    blinderOpacity = offOpacity;
                    beamOpacity = offOpacity;
                    gradientOpacity = offOpacity;
                }
                //if no other conditions are met, shine the flashlight normally
                else
                {
                    //stops the overcharge noise
                    overcharge.Stop();
                    //slowly lowers any charge built up
                    if (oCBarLength > 0)
                    {
                        oCBarLength -= 0.5f;
                    }
                    //normal effects
                    barLength -= normalDrainSpeed;
                    barBorderColor = Color.White;
                    blinderOpacity = normalBlinderOpacity;
                    beamOpacity = normalBeamOpacity;
                    gradientOpacity = normalBlinderOpacity;
                }
            }
            //bar is green if 50% or more
            if (barLength >= 225)
            {
                barColor = Color.Green;
                canOvercharge = true;
            }
            //bar is yellow if between 25 and 50%
            else if (barLength >= 112)
            {
                barColor = Color.Yellow;
                canOvercharge = true;
            }
            //bar is red if below 25%
            else if (barLength < 112 && barLength > 0)
            {
                barColor = Color.Red;
                canOvercharge = true;
            }
            //disables the ability to overcharge if battery is out
            else if (barLength <= 0)
            {
                canOvercharge = false;
            }

            // Gets keyboard state
            KeyboardState kbState = Keyboard.GetState();

            // If there are batteries to use then you can recharge
            if (battery.Count > 0)
            {
                //pressing space uses a battery           
                if (kbState.IsKeyDown(Keys.Space) &&
                    prevKBS.IsKeyUp(Keys.Space))
                {
                    //plays battery sound
                    soundEffects[5].Play();
                    // uses up 1 battery
                    battery.Count--;

                    //adds an amount to the charge
                    if (barLength + rechargeAmount <= 450)
                    {
                        barLength += rechargeAmount;
                    }
                    //charge can not go past 100%
                    else
                    {
                        barLength = 450;
                    }
                }
            }

            if(godMode)
            {
                barLength = 450;
            }

            prevKBS = kbState;
            prevMS = ms;
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
                new Vector2((float)windowWidth / 2, (float)windowHeight / 2),
                new Vector2(t1X2, t1Y2),
                new Vector2(t1X3, t1Y3),
                new Color(Color.Black, blinderOpacity));
            //right triangle
            ShapeBatch.Triangle(
                new Vector2((float)windowWidth / 2, (float)windowHeight / 2),
                new Vector2(t2X2, t2Y2),
                new Vector2(t2X3, t2Y3),
                new Color(Color.Black, blinderOpacity));
            //back triangle
            ShapeBatch.Triangle(
                new Vector2(t3X, t3Y), 5000, triAngle, new Color(Color.Black, blinderOpacity));
            //beam
            ShapeBatch.Triangle(
            new Vector2((float)windowWidth / 2, (float)windowHeight / 2), //player point
            new Vector2(beamX2, beamY2), //left point
            new Vector2(beamX3, beamY3), //right point
            new Color(beamColor, beamOpacity),
            new Color(beamColor, gradientOpacity),
            new Color(beamColor, gradientOpacity));
            //far left triangle
            ShapeBatch.Triangle(
                //uses beam triangle for points
                new Vector2(beamX2, beamY2),
                new Vector2(t1X2, t1Y2),
                new Vector2(farTriPointX, farTriPointY),
                new Color(Color.Black, blinderOpacity));
            //far center triangle
            ShapeBatch.Triangle(
                //uses beam triangle for points
                new Vector2(beamX2, beamY2),
                new Vector2(beamX3, beamY3),
                new Vector2(farTriPointX, farTriPointY),
                new Color(Color.Black, blinderOpacity));
            //far right triangle
            ShapeBatch.Triangle(
                //uses beam triangle for points
                new Vector2(beamX3, beamY3),
                new Vector2(t2X2, t2Y2),
                new Vector2(farTriPointX, farTriPointY),
                new Color(Color.Black, blinderOpacity));

            //outline for battery bar
            ShapeBatch.Line(new Vector2((float)windowWidth - 499, 50), new Vector2((float)windowWidth - 49, 50), 2, barBorderColor);
            ShapeBatch.Line(new Vector2((float)windowWidth - 49, 50), new Vector2((float)windowWidth - 49, 100), 2, barBorderColor);
            ShapeBatch.Line(new Vector2((float)windowWidth - 49, 100), new Vector2((float)windowWidth - 499, 100), 2, barBorderColor);
            ShapeBatch.Line(new Vector2((float)windowWidth - 499, 100), new Vector2((float)windowWidth - 499, 50), 2, barBorderColor);
            //outline for overcharge bar
            ShapeBatch.Line(new Vector2((float)windowWidth - 499, 100), new Vector2((float)windowWidth - 499, 115), 2, barBorderColor);
            ShapeBatch.Line(new Vector2((float)windowWidth - 499, 115), new Vector2((float)windowWidth - 49, 115), 2, barBorderColor);
            ShapeBatch.Line(new Vector2((float)windowWidth - 49, 115), new Vector2((float)windowWidth - 49, 100), 2, barBorderColor);
            //battery bar (length begins at 450 px)
            ShapeBatch.Box((float)windowWidth - 499, 51, (float)barLength, 47, barColor);
            //overcharge meter
            ShapeBatch.Box((float)windowWidth - 499, 101, (float)oCBarLength, 13, oCBarColor);
            

            ShapeBatch.End();
        }

        /// <summary>
        /// draws player animations
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="graphics"></param>
        public void DrawPlayer(SpriteBatch spriteBatch)
        {
            //manages player animations.
            switch (state)
            {

                //player faces north
                case AnimationState.FaceNorth:
                    if (isMoving)
                    {
                        DrawWalkingNorth(SpriteEffects.None, spriteBatch);
                        if (frame == 3)
                        {
                            walk1.Play();
                        }
                        if (frame == 5)
                        {
                            walk2.Play();
                        }
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
                        if (frame == 1)
                        {
                            walk1.Play();
                        }
                        if (frame == 5)
                        {
                            walk2.Play();
                        }
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
                        if (frame == 3)
                        {
                            walk1.Play();
                        }
                        if (frame == 5)
                        {
                            walk2.Play();
                        }
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
                        if (frame == 1)
                        {
                            walk1.Play();
                        }
                        if (frame == 5)
                        {
                            walk2.Play();
                        }
                    }
                    else
                    {
                        DrawFacingHorizontal(SpriteEffects.None, spriteBatch);
                    }
                    break;

                //Idle animation front facing
                case AnimationState.IdleFront:
                    DrawIdleFront(SpriteEffects.None, spriteBatch);
                    break;

                //Idle animation back facing
                case AnimationState.IdleBack:
                    DrawIdleBack(SpriteEffects.None, spriteBatch);
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
            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX - 50, playerCenterY - 50),
                new Rectangle(200, 0, 100, 100), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player walking north
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawWalkingNorth(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX - 50, playerCenterY - 50),
                new Rectangle((PlayerRectWidth * frame) + PlayerRectWidth * 8, 0,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player facing east or west
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawFacingHorizontal(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX - 50, playerCenterY - 50),
                new Rectangle(PlayerRectOffsetX * 3 + (PlayerRectWidth * 2) + 40, PlayerRectOffsetY,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player walking east or west
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawWalkingHorizontal(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX - 50, playerCenterY - 50),
                new Rectangle((PlayerRectWidth * frame) + PlayerRectWidth * 2, PlayerRectOffsetY,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player facing south
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawFacingSouth(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX - 50, playerCenterY - 50),
                new Rectangle(PlayerRectOffsetX - 15, PlayerRectOffsetY,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// draws the player walking south
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawWalkingSouth(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX - 50, playerCenterY - 50),
                new Rectangle((PlayerRectWidth * frame) + PlayerRectWidth * 8, PlayerRectOffsetY,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);
        }

        /// <summary>
        /// Draws the player during idle animation
        /// </summary>
        /// <param name="flipSprite"></param>
        /// <param name="spriteBatch"></param>
        private void DrawIdleFront(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX - 50, playerCenterY - 50),
                new Rectangle(PlayerRectWidth, PlayerRectOffsetY,
                PlayerRectWidth, PlayerRectHeight), Color.White, 0, Vector2.Zero, 1.0f, flipSprite, 0);

            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX - 100, playerCenterY -200),
                new Rectangle((PlayerRectWidth * 10) - (int)(PlayerRectWidth * dFrame) , PlayerRectOffsetY * 2,
                PlayerRectWidth/2, PlayerRectHeight*3), Color.Aqua, 0, Vector2.Zero, 2.0f, flipSprite, 0);
        }

        private void DrawIdleBack(SpriteEffects flipSprite, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX - 50, playerCenterY - 50),
                new Rectangle(100, 0, PlayerRectWidth, PlayerRectHeight), Color.White);

            spriteBatch.Draw(spriteSheet, new Vector2(playerCenterX -100, playerCenterY - 200),
                new Rectangle((PlayerRectWidth * 10) - (int)(PlayerRectWidth * dFrame), PlayerRectOffsetY * 2,
                PlayerRectWidth/2, PlayerRectHeight * 3), Color.Aqua, 0, Vector2.Zero, 2.0f, flipSprite, 0);
        }
    }
}
