using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Image
{
    class Image
    {
        public Color[] pixelData;
        double xmin, xlen, ymin, ylen, asp;
        double xTrans, xScale, yTrans, yScale;
        int screenWidth, screenHeight;


        public Image(int Width, int Height)
        {
            screenWidth = Width;
            screenHeight = Height;

            pixelData = new Color[screenWidth * screenHeight];

            asp = (float)screenHeight / (float)screenWidth;

            resetZoom();
        }


        public void resetZoom()
        {
            double windowWidth = 4.5;

            xmin = -windowWidth * 0.5;
            ymin = -windowWidth * 0.5 * asp;
            xlen = windowWidth;
            ylen = windowWidth * asp;

            xTrans = xmin;
            xScale = windowWidth / (double)screenWidth;
            yTrans = ymin;
            yScale = xScale;
        }

        public void ZoomIn()
        {
            xmin = xmin + (.25 * xlen);
            ymin = ymin + (.25 * ylen);
            xlen *= .5;
            ylen *= .5;

            xTrans = xmin;
            xScale = xlen / (double)screenWidth;
            yTrans = ymin;
            yScale = xScale;
        }

        public void ZoomOut()
        {

            xmin = xmin - (.25 * xlen);
            ymin = ymin - (.25 * ylen);
            xlen *= 1.5;
            ylen *= 1.5;

            xTrans = xmin;
            xScale = xlen / (double)screenWidth;
            yTrans = ymin;
            yScale = xScale;
        }

        public void TargetZoom(Vector2 mouseStart, Vector2 mouseEnd)
        {
            float tempWorldX1 = (float)((mouseStart.X * xScale) + xTrans);
            float tempWorldX2 = (float)((mouseEnd.X * xScale) + xTrans);
            float tempWorldY1 = (float)((mouseStart.Y * yScale) + yTrans);
            float tempWorldY2 = (float)((mouseEnd.Y * yScale) + yTrans);
            double xMax = 0.0f;
            double yMax = 0.0f;

            if (tempWorldX1 < tempWorldX2)
            {
                xmin = tempWorldX1;
                xMax = tempWorldX2;
            }
            else
            {
                xmin = tempWorldX2;
                xMax = tempWorldX1;
            }

            if (tempWorldY1 < tempWorldY2)
            {
                ymin = tempWorldY1;// *asp;
                yMax = tempWorldY2;// *asp;
            }
            else
            {
                ymin = tempWorldY2;// *asp;
                yMax = tempWorldY1;// *asp;
            }
                

            xlen =  xMax - xmin;
            ylen = yMax- ymin; //*asp

            xTrans = xmin;
            xScale = xlen / (double)screenWidth;
            yTrans = ymin;
            yScale = xScale;
        }

        Color calculateColor(double x, double y)
        {
            double cReal = x;
            double cImag = y;
            double tReal = x;
            double tImag = y;
            double nReal;
            double nImag;
            int i;
            int maxTimes = 100;
            Color result = new Color();

            for (i = 0; i < maxTimes; i++)
            {
                nReal = (tReal * tReal) - (tImag * tImag);
                nImag = 2 * tReal * tImag;
                tReal = nReal + cReal;
                tImag = nImag + cImag;

                if (tReal * tReal + tImag * tImag > 4.0)
                    break;
            }

            if (i == maxTimes)
                result = Color.Black;
            else
            {
                result.R = result.B = (byte)(255.0 * i / maxTimes);
                result.G = 0;
                result.A = 255;
            }

            return result;
        }

        public void GetMouseCoord(float x, float y)
        {
            
            xlen = x;
            ylen = y; 

            xTrans = x;
            xScale = xlen / (double)screenWidth;
            yTrans = y;
            yScale = xScale;
        }

        public void calculateTexture(bool [,] state, int ni, int nj)
        {
            for (int i = 0; i < screenWidth; i++)
            {
                for (int j = 0; j < screenHeight; j++)
                {
                    int ind = j * screenWidth + i;
                    int isnd = i * ni / screenWidth;
                    int jsnd = j * ni / screenHeight;

                    if (state[isnd, jsnd])
                        pixelData[ind] = Color.White;
                    else
                        pixelData[ind] = Color.Black;
                }
            }
        }


    }
}
