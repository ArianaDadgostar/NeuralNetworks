namespace MiniMaxTicTacToe;

public enum CellState
{
    X,
    O,
    None
};

public class Square
{
    public CellState state { get; set; }
    public int minX;
    public int maxX;
    public int minY;
    public int maxY;

    public Square(int minX, int maxX, int minY, int maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        state = CellState.None;
    }

    public bool IsWithinSquare(int x, int y)
    {
        if(x > maxX || x < minX) return false;

        if(y > maxY || y < minY) return false;

        return true;
    }
}