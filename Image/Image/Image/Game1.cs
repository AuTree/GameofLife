using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Image
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState prevKeyboardState;
        int screenWidth, screenHeight;
        Texture2D imageA;
        Texture2D imageB;
        Texture2D currentImage;
        Vector2 pos;
        Vector2 mouseStart;
        Vector2 mouseEnd;
        bool pixA;
        bool needNewImage;
        bool drawn;
        bool mouseButtonPressed;
        bool[,] currentState;
        bool[,] nextState;
        bool startSim;
        int xCount, yCount;
        Random rand;

        Image rasterImage;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            IsMouseVisible = true;
            IsFixedTimeStep = false;

            screenHeight = GraphicsDevice.Viewport.Height;
            screenWidth = GraphicsDevice.Viewport.Width;

            imageA = new Texture2D(GraphicsDevice, screenWidth, screenHeight);
            imageB = new Texture2D(GraphicsDevice, screenWidth, screenHeight);
            pixA = false;
            needNewImage = true;
            drawn = true;
            xCount = 200;
            yCount = 200;
            startSim = true;
            rand = new Random();
            currentState = new bool[xCount, yCount];
            nextState = new bool[xCount, yCount];

            rasterImage = new Image(screenWidth, screenHeight);

            pos.X = 0.0f;
            pos.Y = 0.0f;

            initCells();
            DieHard();

        }

  
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        void updateTexture()
        {
            if (drawn && needNewImage)
            {
                rasterImage.calculateTexture(currentState, xCount, yCount);
                bool[,] tempState = currentState;
                currentState = GetNextFrame(currentState);
                nextState = tempState;
                pixA = !pixA;

                if (pixA)
                {
                    imageA.SetData<Color>(rasterImage.pixelData);
                    currentImage = imageA;
                }
                else
                {
                    imageB.SetData<Color>(rasterImage.pixelData);
                    currentImage = imageB;
                }

                needNewImage = false;
                drawn = false;
            }

        }

        protected override void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            //if (startSim == true)
            //    startSim = false;

            if(mouseState.RightButton == ButtonState.Pressed && !mouseButtonPressed)
            {
               mouseStart.X = mouseState.X;
               mouseStart.Y = mouseState.Y;
               mouseButtonPressed = true;
            }

            if(mouseState.RightButton == ButtonState.Released && mouseButtonPressed)
            {
                mouseEnd.X = mouseState.X;
                mouseEnd.Y = mouseState.Y;
                rasterImage.TargetZoom(mouseStart, mouseEnd);
                mouseStart = Vector2.Zero;
                mouseEnd = Vector2.Zero;
                mouseButtonPressed = false;
                needNewImage = true;
            }

            if(mouseState.MiddleButton == ButtonState.Pressed)
            {
                rasterImage.GetMouseCoord(mouseState.X, mouseState.Y);
                needNewImage = true;
            }

            if(keyboardState.IsKeyDown(Keys.C) && (!prevKeyboardState.IsKeyDown(Keys.C)))
            {
                //rasterImage.resetZoom();
                //prevKeyboardState = keyboardState;
                //needNewImage = true;

                startSim = !startSim;
            }
            else if(keyboardState.IsKeyDown(Keys.OemMinus))
            {
                rasterImage.ZoomOut();
                prevKeyboardState = keyboardState;
                needNewImage = true;
            }
            else if (keyboardState.IsKeyDown(Keys.OemPlus))
            {
                rasterImage.ZoomIn();
                prevKeyboardState = keyboardState;
                needNewImage = true;
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Change this!
            if (startSim)
                needNewImage = true;

            //needNewImage = true;
            updateTexture();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            // TODO: Add your drawing code here                      
            spriteBatch.Begin();

            spriteBatch.Draw(currentImage, pos, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);

            drawn = true;
        }

        void initCells()
        {
            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    currentState[i, j] = nextState[i, j] = false;
                }
            }
        }

        public void DieHard()//first state
        {
            int ist, jst;

            for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    currentState[i, j] = nextState[i, j] = false;
                    int chance = (int)rand.Next(1, 100);

                    if(chance > 90)
                    {
                        currentState[i, j] = true;
                    }
                }
            }

            ist = xCount / 2 - 4;
            jst = yCount / 2 - 2;

            currentState[ist + 6, jst + 2] = true;
            currentState[ist + 0, jst + 1] = true;
            currentState[ist + 1, jst + 0] = true;
            currentState[ist + 1, jst + 1] = true;
            currentState[ist + 5, jst + 0] = true;
            currentState[ist + 6, jst + 0] = true;
            currentState[ist + 7, jst + 0] = true;
        }

        bool[,] GetNextFrame(bool[,] currentState)
        {
            bool[,]newState = currentState;
			for (int i = 0; i < xCount; i++)
            {
                for (int j = 0; j < yCount; j++)
                {
                    int cellsClose = 0;
                    //Check cells
                    if (i >= 1 && j >= 1)
                    {
                        if (newState[i - 1, j - 1] == true)
                            cellsClose++;
                    }

                    if (j >= 1)
                    {
                        if (newState[i, j - 1] == true)
                            cellsClose++;
                    }

                    if (i >= 1)
                    {
                        if (newState[i - 1, j] == true)
                            cellsClose++;
                    }

                    if (i + 1 < xCount)
                    {
                        if(j != 0)
                        {
                            if (newState[i + 1, j - 1] == true)
                                cellsClose++;
                        }


                        if (newState[i + 1, j] == true)
                            cellsClose++;
                    }

                    if (j + 1 <= yCount -1)
                    {
                        if (newState[i, j + 1] == true)
                            cellsClose++;

                        if(i!=0)
                        {
                            if (newState[i - 1, j + 1] == true)
                                cellsClose++;
                        }
                        
                    }

                    if (i + 1 <= xCount - 1 && j + 1 <= yCount -1)
                    {
                        if (newState[i + 1, j + 1] == true)
                            cellsClose++;
                    }

                    if (newState[i, j] == true)
                    {
                        if (cellsClose == 2 || cellsClose == 3)
                            nextState[i, j] = true;
                        else
                            nextState[i, j] = false;
                    }
                    else
                    {
                        if (cellsClose == 3)
                            nextState[i, j] = true;
                        else
                            nextState[i, j] = false;
                    }
                    
                }     
            }

            return nextState;
        }
    }
}
