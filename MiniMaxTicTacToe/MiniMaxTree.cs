using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace MiniMaxTicTacToe;

public interface IGameState<T> where T : IGameState<T>
{
    bool IsWin { get; }
    bool IsLoss { get; }
    bool IsTie { get; }
    bool IsTerminal { get; }
    T[] GetChildren();
}

public class TicTacToeNode : IGameState<TicTacToeNode>
{
    public bool IsWin { get; set; }
    public bool IsLoss { get; set; }
    public bool IsTie { get; set; }
    public bool IsTerminal { get; set; }
    public Square[][] Board { get; set; }
    public Square change { get; set; }
    public TicTacToeNode[] Children;

    public bool IsCross { get; set; }

    private static readonly int[][] Lines = new int[][]
    {
        new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8}, // rows
        new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8}, // columns
        new[] {0,4,8}, new[] {2,4,6}                 // diagonals
    };

    public TicTacToeNode[] GetChildren()
    {
        return Children;
    }

    public int EmptyCells()
    {
        int count = 0;
        for(int i = 0; i < Board.Length; i ++)
        {
            for(int j = 0; j < Board[i].Length; j++)
            {
                if(Board[i][j].state != CellState.None) continue;

                count++;
            }
        }
        return count;
    }

    public static TicTacToeNode EvaluateNode(TicTacToeNode current, bool isCross)
    {
        int row;
        foreach (var line in Lines)
        {
            row = line[0]/3;
            CellState a = current.Board[row][line[0] - (row*3)].state;
            row = line[1]/3;
            CellState b = current.Board[row][line[1] - (row*3)].state;
            row = line[2]/3;
            CellState c = current.Board[row][line[2] - (row*3)].state;

            if (a == CellState.None || a != b || b != c) continue;

            current.IsTerminal = true;
            current.IsWin = (a == CellState.X) ? true : false;
            current.IsLoss = !current.IsWin;
            current.IsTie = false;
            return current;
        }

        if(current.EmptyCells() != 0) return current;

        current.IsTerminal = true;
        current.IsTie = true;
        current.IsWin = false;
        current.IsLoss = false;
        return current;
    }

    public static TicTacToeNode DefineTree(TicTacToeNode current, bool isCross)
    {
        current = EvaluateNode(current, isCross);
        if(current.IsTerminal) return current;

        List<TicTacToeNode> children = new List<TicTacToeNode>();

        for(int i = 0; i < current.Board.Length; i++)
        {
            for(int j = 0; j < current.Board[i].Length; j++)
            {
                if(current.Board[i][j].state != CellState.None) continue;

                TicTacToeNode node = new();
                node.Board = new Square[3][];
                for(int k = 0; k < current.Board.Length; k++)
                {
                    node.Board[k] = new Square[3];
                    for(int l = 0; l < current.Board[k].Length; l++)
                    {
                        node.Board[k][l] = new Square(current.Board[k][l].minX, current.Board[k][l].maxX, current.Board[k][l].minY, current.Board[k][l].maxY);
                        node.Board[k][l].state = current.Board[k][l].state;
                    }
                }

                node.Board[i][j].state = isCross ? CellState.X : CellState.O;
                node.change = node.Board[i][j];
                node = DefineTree(node, !isCross);
                children.Add(node);
            }
        }

        for(int i = 0; i < children.Count; i++)
        {
            if(children[i].IsWin && isCross) current.IsWin = true;
            if(children[i].IsLoss && !isCross) current.IsLoss = true;
            if(!current.IsWin && !current.IsLoss && children[i].IsTie) current.IsTie = true;
        }

        current.Children = children.ToArray();
        return current;
    }

    public bool Contains(Square possible)
    {
        for(int i = 0; i < Board.Length; i++)
        {
            for(int j = 0; j < Board[i].Length; j++)
            {
                if(possible.state != Board[i][j].state) continue;
                if(possible.minX != Board[i][j].minX || possible.maxX != Board[i][j].maxX) continue;
                if(possible.minY != Board[i][j].minY || possible.maxY != Board[i][j].maxY) continue;
                return true;
            }
        }

        return false;
    }
}

public class TicTacToeTree
{
    TicTacToeNode head;
    TicTacToeNode current;
    bool isMaximizer;

    public TicTacToeTree(bool isCross, Square[][] board)
    {
        head = new TicTacToeNode();
        head.Board = new Square[3][];
        for(int i = 0; i < board.Length; i++)
        {
            head.Board[i] = new Square[3];
            for(int j = 0; j < board[i].Length; j++)
            {
                head.Board[i][j] = new Square(board[i][j].minX, board[i][j].maxX, board[i][j].minY, board[i][j].maxY);
                head.Board[i][j].state = board[i][j].state;
            }
        }
        head = TicTacToeNode.DefineTree(head, !isCross);
        isMaximizer = isCross;
        current = head;
    }

    public Square[][] DefineNextMove(Square chosen)
    {
        if(current.IsTerminal) return current.Board;
        foreach(TicTacToeNode child in current.Children)
        {
            if(child.change.maxX != chosen.maxX || child.change.maxY != chosen.maxY) continue;

            current = child;
            if(current.IsTerminal) return current.Board;
            foreach(TicTacToeNode grandchild in current.Children)
            {
                if(grandchild.IsWin && isMaximizer) current = grandchild;
                if(grandchild.IsLoss && !isMaximizer) current = grandchild;
                else if(grandchild.IsTie && current == child) current = grandchild;
            }
        }
        return current.Board;
    }
    
    public Square[][] TravelDownTree(Square chosen)
    {
        if(current.IsTerminal) return current.Board;
        foreach(TicTacToeNode child in current.Children)
        {
            if(child.change.maxX != chosen.maxX || child.change.maxY != chosen.maxY) continue;

            current = child;
        }
        return current.Board;
    }
}