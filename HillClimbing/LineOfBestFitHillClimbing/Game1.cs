using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace LineOfBestFitHillClimbing
{
    public class Line
    {
        public float slope;
        public int b;

        public Line(float slope, int b)
        {
            this.slope = slope;
            this.b = b;
        }
    }

    public class Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private Texture2D pixel;
        private List<Point> points;
        private MouseState previousMouseState;
        private Line line;

        const int LINENUM = 30;
        const int XLENGTH = 450;
        const int YLENGTH = 450;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public float CalculateFit(List<Point> points, float slope, int b)
        {
            float result = 0;
            for (int i = 0; i < points.Count; i++)
            {
                float lineResult = (slope * points[i].x) + b;
                result += Math.Abs(points[i].y - lineResult);
            }

            return Math.Abs(result);
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

            points = new List<Point>();

            Random random = new Random();

            line = new Line(random.Next(0, 2), 100);

            // TODO: use this.Content to load your game content here
        }

        public void MutateLine(ref float slope, ref int b)
        {
            Random random = new Random();
            float newSlope;
            int newB;
            if (random.Next(0, 2) == 0)
            {
                newSlope = slope + (float)(random.Next(0, 2) * 2 - 1) / 50;
                newB = b;
            }
            else
            {
                newB = b + ((random.Next(0, 2) * 2 - 1) * 5);
                newSlope = slope;
            }

            if (CalculateFit(points, slope, b) <= CalculateFit(points, newSlope, newB)) return;

            slope = newSlope;
            b = newB;

            Window.Title = $"B: {b}, Slope: {slope}";
        }

        #region UpdateLogic

        public void UpdateMouse()
        {
            MouseState currentMouseState = Mouse.GetState();

            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                previousMouseState.LeftButton == ButtonState.Released)
            {
                AddPoints(currentMouseState.X, currentMouseState.Y);
                if(currentMouseState.X < )
            }

            previousMouseState = currentMouseState;
        }

        public void AddPoints(int x, int y)
        {
            points.Add(new Point(x, y));
        }

        #endregion

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MutateLine(ref line.slope, ref line.b);

            UpdateMouse();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        #region Drawing Logic

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

        public void DrawPoints()
        {
            for (int i = 0; i < points.Count; i++)
            {
                spriteBatch.Draw(pixel,
                                 new Rectangle(points[i].x, points[i].y, 15, 15),
                                 Color.IndianRed);
            }
        }

        public void DrawLine()
        {
            for (int i = 0; i <= XLENGTH; i++)
            {
                spriteBatch.Draw(pixel,
                                 new Rectangle(i, (int)((line.slope * i) + line.b), 5, 5),
                                 Color.DarkBlue);
            }
        }

        #endregion

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.DarkOliveGreen);

            DrawGraph(450, 450);
            DrawPoints();
            DrawLine();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
            spriteBatch.End();
        }
    }
}
