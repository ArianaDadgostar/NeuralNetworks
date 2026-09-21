namespace MonteCarloCheckers;

public enum CellState
{
    Red,
    White,
    None
};

public class Square
{
    public int x;
    public int y;
    public int side;
    public CellState state;
    public bool isQueen;

    public Square(int x, int y, int side)
    {
        this.x = x;
        this.y = y;
        this.side = side;
        state = CellState.None;
        isQueen = false;
    }

    public bool IsWithinBounds(int userX, int userY)
    {
        if(userX >= x + side || userX < x) return false;

        if(userY >= y + side || userY < y) return false;

        return true;
    }
}