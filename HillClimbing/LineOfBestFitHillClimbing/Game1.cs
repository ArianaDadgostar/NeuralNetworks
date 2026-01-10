using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Transactions;

namespace LineOfBestFitHillClimbing
{
    public class Line
    {
        public int slope;
        public int b;
    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private Texture2D pixel;
        private (int x, int y)[] points;
        private int index;
        private MouseState previousMouseState;

        const int LINENUM = 30;
        const int XLENGTH = 450;
        const int YLENGTH = 450;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public int CalculateFit(int[] points, int slope, int b)
        {
            int result = 0;
            for (int i = 0; i < points.Length; i++)
            {
                int line = (slope * points[i]) + b;
                result += points[i] - line;
            }

            return result / points.Length;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            points = new (int x, int y)[100];
            index = 0;

            // TODO: use this.Content to load your game content here
        }

        public void MutateLine(ref int slope, ref int b)
        {
            Random random = new Random();
            slope += random.Next(-1, 2);
            b += random.Next(-1, 2);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        public void DrawGraph(int xLength, int yLength)
        {
            int width = xLength / LINENUM;
            int height = yLength / LINENUM;

            for (int i = 0; i <= LINENUM; i++)
            {
                spriteBatch.Draw(pixel,
                                 new Rectangle(width * (i + 1), height, 2, yLength),
                                 Color.Black);

                spriteBatch.Draw(pixel,
                                 new Rectangle(width, height * (i + 1), xLength, 2),
                                 Color.Black);
            }
        }

        public void AddPoints(int x, int y)
        {
            spriteBatch.Draw(pixel,
                             new Rectangle(x, y, 15, 15),
                             Color.IndianRed);
            points[index] = (x, y);
            index++;
        }

        public void DrawPoints((int x, int y)[] points)
        {
            for (int i = 0; i < index; i ++)
            {
                spriteBatch.Draw(pixel,
                                 new Rectangle(points[i].x, points[i].y, 15, 15),
                                 Color.IndianRed);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.DarkOliveGreen);


            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                AddPoints(currentMouseState.X, currentMouseState.Y);
            }

            previousMouseState = currentMouseState;

            DrawGraph(450, 450);
            DrawPoints(points);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
