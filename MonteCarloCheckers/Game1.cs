using System;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonteCarloCheckers;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch spriteBatch;

    private Texture2D square; 
    private Square[][] board;
    private MouseState mouse;
    private Square selected;
    private Square target;
    private bool isMousePressed = false;
    private bool userIsRed = true;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public void SetupBoard()
    {
        board = new Square[Dimensions.SquareCount][];
        for(int i = 0; i < Dimensions.SquareCount; i++)
        {
            board[i] = new Square[Dimensions.SquareCount];
            for(int j = 0; j < Dimensions.SquareCount; j++)
            {
                board[i][j] = new Square(i * Dimensions.SquareSize, j * Dimensions.SquareSize, Dimensions.SquareSize);
                board[i][j].state = (j <= 2 && Math.Abs(i - j) % 2 == 1) ? CellState.Red : board[i][j].state;
                board[i][j].state = (j >= 5 && Math.Abs(i - j) % 2 == 1) ? CellState.White : board[i][j].state;
            }
        }
    }

    protected override void Initialize()
    {
        square = new Texture2D(GraphicsDevice, 1, 1);
        square.SetData(new Color[] { Color.LightCyan });

        _graphics.PreferredBackBufferWidth = Dimensions.WindowWidth; // Set your desired width
        _graphics.PreferredBackBufferHeight = Dimensions.WindowHeight; // Set your desired height
        _graphics.ApplyChanges();

        SetupBoard();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    public Square CheckForMouse()
    {
        if(mouse.LeftButton == ButtonState.Pressed || isMousePressed == false) return null;

        isMousePressed = false;
        for(int i = 0; i < board.Length; i++)
        {
            for(int j = 0; j < board[i].Length; j++)
            {
                if(board[i][j].IsWithinBounds(mouse.Position.X, mouse.Position.Y)) return board[i][j];
            }
        }

        return null;
    }

    public bool CanMovePiece()
    {
        if(target is null || selected is null) return false;
        if(selected.state == CellState.None)
        {
            selected = null;
            return false;
        }
        if(target.state != CellState.None)
        {
            target = null;
            return false;
        }

        return true;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        selected ??= CheckForMouse();
        target = (selected is null) ? target : CheckForMouse();
        mouse = Mouse.GetState();

        isMousePressed = (mouse.LeftButton == ButtonState.Pressed) ? true : isMousePressed;

        if(!CanMovePiece()) return;

        target.state = selected.state;
        selected.state = CellState.None;
        target = null;
        selected = null;

        base.Update(gameTime);
    }

    public void DrawBoard()
    {
        for(int i = 0; i < Dimensions.SquareCount; i++)
        {
            for(int j = 0; j < Dimensions.SquareCount; j ++)
            {
                if(Math.Abs(i - j) % 2 == 1)
                {
                    spriteBatch.Draw(square, new Rectangle(i * Dimensions.SquareSize,
                                                           j * Dimensions.SquareSize,
                                                           Dimensions.SquareSize, 
                                                           Dimensions.SquareSize), Color.BlueViolet);
                }

                if(board[i][j].state == CellState.None) continue;

                Color color = (board[i][j].state == CellState.Red) ? Color.Red : Color.White;
                spriteBatch.Draw(square, new Rectangle(i * Dimensions.SquareSize + 8,
                                                       j * Dimensions.SquareSize + 8,
                                                       40, 
                                                       40), color);
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin();

        // TODO: Add your drawing code here

        DrawBoard();

        base.Draw(gameTime);
        spriteBatch.End();
    }
}
