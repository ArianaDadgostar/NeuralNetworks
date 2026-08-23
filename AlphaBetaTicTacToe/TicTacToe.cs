using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AlphaBetaTicTacToe;

/*
    CROSS is MAXIMIZER
    CIRCLE goes FIRST
*/

public class TicTacToe : Game
{
    #region Definitions
    private GraphicsDeviceManager _graphics;
    private SpriteBatch spriteBatch;

    private Texture2D line;
    private Texture2D cross;
    private Texture2D circle;

    private Texture2D shape;
    private Color color;

    public bool isCross;

    Square[][] board;

    AlphaBetaTree tree;

    #endregion

    public TicTacToe()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    #region GraphicDefinitions

    void DefineTextures()
    {
        line = new Texture2D(GraphicsDevice, 1, 1);
        line.SetData(new Color[] { Color.White });

        cross = new Texture2D(GraphicsDevice, 1, 1);
        cross.SetData(new Color[] { Color.Red });

        circle = new Texture2D(GraphicsDevice, 1, 1);
        circle.SetData(new Color[] { Color.Green });
    }

    void DefineBoard()
    {
        board = new Square[3][];
        for(int i = 0; i < board.Length; i ++)
        {
            board[i] = new Square[3];
            for(int j = 0; j < board[i].Length; j ++)
            {
                board[i][j] = new Square(i * Dimensions.offset + Dimensions.xMin,
                                         (i + 1) * Dimensions.offset + Dimensions.xMin, 
                                         j * Dimensions.offset + Dimensions.yMin,
                                         (j + 1) * Dimensions.offset + Dimensions.yMin);
            }
        }
    }

    #endregion 

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        isCross = false;

        DefineTextures();
        DefineBoard();
        tree = new(true, board);

        // TODO: use this.Content to load your game content here
    }

    public Square SquareSelected()
    {
        MouseState mouseState = Mouse.GetState();
        if(mouseState.LeftButton != ButtonState.Pressed) return null;

        foreach(Square[] row in board)
        {
            foreach(Square cell in row)
            {
                if(!cell.IsWithinSquare(mouseState.Position.X, mouseState.Position.Y)) continue;

                return cell;
            }
        }

        return null;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        if(Mouse.GetState().LeftButton != ButtonState.Pressed || isCross) return; // CHANGE TO ALLOW USER TO PLAY AS CROSS OR CIRCLE 

        Square chosen = SquareSelected();
        if(chosen == null || chosen.state != CellState.None) return;

        chosen.state = isCross ? CellState.X : CellState.O;
        isCross = !isCross;

        board = tree.DefineNextMove(chosen);
        isCross = !isCross;
        
        base.Update(gameTime);
    }

    public void DrawTicTacToeBoard()
    {
        spriteBatch.Draw(line, new Rectangle(Dimensions.xMin + Dimensions.offset, 
                                             Dimensions.yMin, 
                                             Dimensions.width, 
                                             Dimensions.length), Color.White);
        spriteBatch.Draw(line, new Rectangle(Dimensions.xMin + Dimensions.length - Dimensions.offset, 
                                             Dimensions.yMin, 
                                             Dimensions.width, 
                                             Dimensions.length), Color.White);
        spriteBatch.Draw(line, new Rectangle(Dimensions.xMin, 
                                             Dimensions.yMin + Dimensions.offset, 
                                             Dimensions.length, 
                                             Dimensions.width), Color.White);
        spriteBatch.Draw(line, new Rectangle(Dimensions.xMin, 
                                             Dimensions.yMin + Dimensions.length - Dimensions.offset, 
                                             Dimensions.length, 
                                             Dimensions.width), Color.White);

        foreach(Square[] row in board)
        {
            foreach(Square cell in row)
            {
                if(cell.state == CellState.None) continue;

                shape = (cell.state == CellState.X) ? cross : circle;
                color = (cell.state == CellState.X) ? Color.Red : Color.Green;

                spriteBatch.Draw(shape, new Rectangle((cell.minX + cell.maxX) / 2, 
                                                      (cell.minY + cell.maxY) / 2, 10, 10), color);
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.BlueViolet);
        spriteBatch.Begin();

        DrawTicTacToeBoard();

        base.Draw(gameTime);
        spriteBatch.End();
    }
}
