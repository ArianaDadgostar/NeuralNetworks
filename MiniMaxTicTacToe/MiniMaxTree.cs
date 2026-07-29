using System.Collections.Generic;
using System.Data;
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
            CellState a = current.Board[row][line[0] - row].state;
            row = line[1]/3;
            CellState b = current.Board[row][line[1] - row].state;
            row = line[2]/3;
            CellState c = current.Board[row][line[2] - row].state;

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
                current.Board = (Square[][])current.Board.Clone();

                node.Board[i][j].state = isCross ? CellState.X : CellState.O;
                node = DefineTree(node, !isCross);
                children.Add(node);
            }
        }

        current.Children = children.ToArray();
        return current;
    }
}

public class TicTacToeTree
{
    TicTacToeNode head;

    public TicTacToeTree(bool isCross)
    {
        head = new TicTacToeNode();
        head = TicTacToeNode.DefineTree(head, isCross);
    }
}