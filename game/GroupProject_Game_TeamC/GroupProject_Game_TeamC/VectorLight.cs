using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/*
 * Robinson Ruddock
 * Vector Light
 * Creates 5 vectors and handles Logic for if a Rectangle is touched by one of the vectors
 * These 5 lines are meant to represent a flashlight and handle hit detection
 */
namespace GroupProject_Game_TeamC
{
    internal class VectorLight
    {
        //side lengths of triangles
        private const double directionLineLength = 850;
        private const double displacementLength = 300;
        private const double magnitudeRatio = displacementLength / directionLineLength;

        //mouse position
        private double mouseY;
        private double mouseX;

        //vector of the flashlight direction
        private double directionAngle;
        private double directionX;
        private double directionY;

        //arctan has limitations and the direction of the x and y values for the triangle would need to be corrected
        private int flipX = 1;
        private int flipY = 1;

        //vectors for the flashlight borders
        private double borderOneX;
        private double borderOneY;
        private double borderTwoX;
        private double borderTwoY;

        //vectors in between a border and the direction vector
        private double fillingOneX;
        private double fillingOneY;
        private double fillingTwoX;
        private double fillingTwoY;

        //tile corner x positions
        private double tileXOne;
        private double tileXTwo;

        //tile corner y positions
        private double tileYOne;
        private double tileYTwo;

        //line points
        double lineOne;
        double lineTwo;

        //line max and min ranges
        double graphXMin;
        double graphXMax;
        double graphYMin;
        double graphYMax;

        //stores the max and mins of the tiles
        double tileXMin;
        double tileXMax;
        double tileYMin;
        double tileYMax;

        //flashlight range
        double rangeMinX;
        double rangeMaxX;
        double rangeMinY;
        double rangeMaxY;

        /// <summary>
        /// Determines the direction of the 5 vector used in the flashlight's hit detection
        /// </summary>
        /// <param name="flashPosX"></param>
        /// <param name="flashPosY"></param>
        public void Direction(double flashPosX, double flashPosY)
        {
            MouseState mState = Mouse.GetState();

            //mouses's position is considered its distance from the player
            mouseY = mState.Position.Y - flashPosY;
            mouseX = mState.Position.X - flashPosX;

            if (mouseY < 0)
            {
                flipY = -1;
            }
            if (mouseX < 0)
            {
                flipX = -1;
            }

            //determines the angle of the mouse
            directionAngle = Math.Atan(Math.Abs(mouseY / mouseX));

            //x and y values of the flashlight direction line
            directionX = flipX * directionLineLength * Math.Cos(directionAngle);
            directionY = flipY * directionLineLength * Math.Sin(directionAngle);

            //determines the vectors of the flashlight's borders
            borderOneX = directionX + (magnitudeRatio * -1 * directionY);
            borderOneY = directionY + (magnitudeRatio * directionX);
            borderTwoX = directionX + (magnitudeRatio * directionY);
            borderTwoY = directionY + (magnitudeRatio * -1 * directionX);

            //determines the vectors in between a border and the direction vector
            fillingOneX = directionX + (magnitudeRatio / 2 * -1 * directionY);
            fillingOneY = directionY + (magnitudeRatio / 2 * directionX);
            fillingTwoX = directionX + (magnitudeRatio / 2 * directionY);
            fillingTwoY = directionY + (magnitudeRatio / 2 * -1 * directionX);

            //reset flip values
            flipX = 1;
            flipY = 1;
        }


        /// <summary>
        /// determines the y value at a given x point and slope
        /// </summary>
        /// <param name="tileX"></param>
        /// <param name="cornerX"></param>
        /// <param name="cornerY"></param>
        /// <returns></returns>
        public double YValue(double tileX, double cornerX, double cornerY)
        {
            return (cornerY / cornerX) * tileX;
        }

        /// <summary>
        /// determines the x value at a given y point and slope
        /// </summary>
        /// <param name="tileY"></param>
        /// <param name="cornerX"></param>
        /// <param name="cornerY"></param>
        /// <returns></returns>
        public double XValue(double tileY, double cornerX, double cornerY)
        {
            return (cornerX / cornerY) * tileY;
        }

        /// <summary>
        /// Checks if the flashlight intersects with a Rectangle
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="flashPosX"></param>
        /// <param name="flashPosY"></param>
        /// <returns></returns>
        public bool Intersects(Rectangle tile, double flashPosX, double flashPosY)
        {
            //displace x bounds of the tile
            tileXOne = tile.X - flashPosX;
            tileXTwo = tile.X + tile.Width - flashPosX;

            //displace y bounds of tile
            tileYOne = tile.Y - flashPosY;
            tileYTwo = tile.Y + tile.Height - flashPosY;

            //tile range
            tileXMin = Math.Min(tileXOne, tileXTwo);
            tileXMax = Math.Max(tileXOne, tileXTwo);
            tileYMin = Math.Min(tileYOne, tileYTwo);
            tileYMax = Math.Max(tileYOne, tileYTwo);

            //determines the range a Rectangle has to be in to be checked for intersection
            rangeMinX = Math.Min(Math.Min(0, borderOneX), borderTwoX);
            rangeMaxX = Math.Max(Math.Max(0, borderOneX), borderTwoX);
            rangeMinY = Math.Min(Math.Min(0, borderOneY), borderTwoY);
            rangeMaxY = Math.Max(Math.Max(0, borderOneY), borderTwoY);

            //checks if the Rectangle is within appropriate range, if not don't even consider the Rectangle
            if ((rangeMinX <= tileXOne && rangeMaxX >= tileXOne && rangeMinY <= tileYOne && rangeMaxY >= tileYOne)
                || (rangeMinX <= tileXTwo && rangeMaxX >= tileXTwo && rangeMinY <= tileYOne && rangeMaxY >= tileYOne)
                || (rangeMinX <= tileXOne && rangeMaxX >= tileXOne && rangeMinY <= tileYTwo && rangeMaxY >= tileYTwo)
                || (rangeMinX <= tileXTwo && rangeMaxX >= tileXTwo && rangeMinY <= tileYTwo && rangeMaxY >= tileYTwo))
            {
                //determines the max ranges of the direction vector
                graphXMin = Math.Min(0, directionX);
                graphXMax = Math.Max(0, directionX);
                graphYMin = Math.Min(0, directionY);
                graphYMax = Math.Max(0, directionY);

                //checks if the direction vector intersects with the top and bottom sides of the square
                lineOne = XValue(tileYOne, directionX, directionY);
                lineTwo = XValue(tileYTwo, directionX, directionY);
                if (((lineOne >= tileXMin && lineOne <= tileXMax) || (lineTwo >= tileXMin && lineTwo <= tileXMax)) &&
                    ((tileYOne <= graphYMax && tileYOne >= graphYMin) || tileYTwo <= graphYMax && tileYTwo >= graphYMin))
                {
                    return true;
                }

                //checks if the direction vector intersects with the left and right sides of the square
                lineOne = YValue(tileXOne, directionX, directionY);
                lineTwo = YValue(tileXTwo, directionX, directionY);
                if (((lineOne >= tileYMin && lineOne <= tileYMax) || (lineTwo >= tileYMin && lineTwo <= tileYMax)) &&
                    ((tileXOne <= graphXMax && tileXOne >= graphXMin) || tileXTwo <= graphXMax && tileXTwo >= graphXMin))
                {
                    return true;
                }


                //determines the max ranges of the 1st filling vector
                graphYMin = Math.Min(0, fillingOneY);
                graphYMax = Math.Max(0, fillingOneY);
                graphXMin = Math.Min(0, fillingOneX);
                graphXMax = Math.Max(0, fillingOneX);

                //checks if filling 1 intersects the top and bottom square of the square
                lineOne = XValue(tileYOne, fillingOneX, fillingOneY);
                lineTwo = XValue(tileYTwo, fillingOneX, fillingOneY);
                if (((lineOne >= tileXMin && lineOne <= tileXMax) || (lineTwo >= tileXMin && lineTwo <= tileXMax)) &&
                    ((tileYOne <= graphYMax && tileYOne >= graphYMin) || tileYTwo <= graphYMax && tileYTwo >= graphYMin))
                {
                    return true;
                }

                //checks if filling 1 intersects with the left and right sides of the square
                lineOne = YValue(tileXOne, fillingOneX, fillingOneY);
                lineTwo = YValue(tileXTwo, fillingOneX, fillingOneY);
                if (((lineOne >= tileYMin && lineOne <= tileYMax) || (lineTwo >= tileYMin && lineTwo <= tileYMax)) &&
                    ((tileXOne <= graphXMax && tileXOne >= graphXMin) || tileXTwo <= graphXMax && tileXTwo >= graphXMin))
                {
                    return true;
                }


                //determines the max ranges of filling vector 2
                graphYMin = Math.Min(0, fillingTwoY);
                graphYMax = Math.Max(0, fillingTwoY);
                graphXMin = Math.Min(0, fillingTwoX);
                graphXMax = Math.Max(0, fillingTwoX);

                //checks if filling 2 intersects the top and bottom square of the square
                lineOne = XValue(tileYOne, fillingTwoX, fillingTwoY);
                lineTwo = XValue(tileYTwo, fillingTwoX, fillingTwoY);
                if (((lineOne >= tileXMin && lineOne <= tileXMax) || (lineTwo >= tileXMin && lineTwo <= tileXMax)) &&
                    ((tileYOne <= graphYMax && tileYOne >= graphYMin) || tileYTwo <= graphYMax && tileYTwo >= graphYMin))
                {
                    return true;
                }

                //checks if filling 2 intersects with the left and right sides of the square
                lineOne = YValue(tileXOne, fillingTwoX, fillingTwoY);
                lineTwo = YValue(tileXTwo, fillingTwoX, fillingTwoY);
                if (((lineOne >= tileYMin && lineOne <= tileYMax) || (lineTwo >= tileYMin && lineTwo <= tileYMax)) &&
                    ((tileXOne <= graphXMax && tileXOne >= graphXMin) || tileXTwo <= graphXMax && tileXTwo >= graphXMin))
                {
                    return true;
                }


                //determines the max ranges of border 1
                graphXMin = Math.Min(0, borderOneX);
                graphXMax = Math.Max(0, borderOneX);
                graphYMin = Math.Min(0, borderOneY);
                graphYMax = Math.Max(0, borderOneY);

                //checks if border 1 intersects the top and bottom sides of the square
                lineOne = XValue(tileYOne, borderOneX, borderOneY);
                lineTwo = XValue(tileYTwo, borderOneX, borderOneY);
                if (((lineOne >= tileXMin && lineOne <= tileXMax) || (lineTwo >= tileXMin && lineTwo <= tileXMax)) &&
                    ((tileYOne <= graphYMax && tileYOne >= graphYMin) || tileYTwo <= graphYMax && tileYTwo >= graphYMin))
                {
                    return true;
                }

                //checks if border 1 intersects with the left and right sides of the square
                lineOne = YValue(tileXOne, borderOneX, borderOneY);
                lineTwo = YValue(tileXTwo, borderOneX, borderOneY);
                if (((lineOne >= tileYMin && lineOne <= tileYMax) || (lineTwo >= tileYMin && lineTwo <= tileYMax)) &&
                    ((tileXOne <= graphXMax && tileXOne >= graphXMin) || tileXTwo <= graphXMax && tileXTwo >= graphXMin))
                {
                    return true;
                }


                //determines the max ranges of border 2
                graphYMin = Math.Min(0, borderTwoY);
                graphYMax = Math.Max(0, borderTwoY);
                graphXMin = Math.Min(0, borderTwoX);
                graphXMax = Math.Max(0, borderTwoX);

                //checks if border 2 intersects the top and bottom square of the square
                lineOne = XValue(tileYOne, borderTwoX, borderTwoY);
                lineTwo = XValue(tileYTwo, borderTwoX, borderTwoY);
                if (((lineOne >= tileXMin && lineOne <= tileXMax) || (lineTwo >= tileXMin && lineTwo <= tileXMax)) &&
                    ((tileYOne <= graphYMax && tileYOne >= graphYMin) || tileYTwo <= graphYMax && tileYTwo >= graphYMin))
                {
                    return true;
                }

                //checks if border 2 intersects with the left and right sides of the square
                lineOne = YValue(tileXOne, borderTwoX, borderTwoY);
                lineTwo = YValue(tileXTwo, borderTwoX, borderTwoY);
                if (((lineOne >= tileYMin && lineOne <= tileYMax) || (lineTwo >= tileYMin && lineTwo <= tileYMax)) &&
                    ((tileXOne <= graphXMax && tileXOne >= graphXMin) || tileXTwo <= graphXMax && tileXTwo >= graphXMin))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
