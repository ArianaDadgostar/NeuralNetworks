using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace AlphaBetaTicTacToe;

public interface IAlphaBetaState<T> where T : IAlphaBetaState<T>
{
    int Value { get; set; }
    int Alpha { get; set; }
    int Beta { get; set; }
    bool IsTerminal { get; }
    T[] GetChildren();
}

public class Node : IAlphaBetaState<Node>
{
    public int Alpha { get; set; }
    public int Beta { get; set; }
    public int Value { get; set; }
    public bool IsTerminal { get; set; }
    public Square[][] Board { get; set; }
    public Square change { get; set; }
    public Node[] Children;

    public Node()
    {
        Value = 0;
        Alpha = int.MinValue;
        Beta = int.MaxValue;
        IsTerminal = false;
    }

    public bool IsCross { get; set; }

    private static readonly int[][] Lines = new int[][]
    {
        new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8}, // rows
        new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8}, // columns
        new[] {0,4,8}, new[] {2,4,6}                 // diagonals
    };

    public Node[] GetChildren()
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

    public static Node EvaluateNode(Node current, bool isCross)
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
            current.Alpha = (isCross && current.Alpha < 1) ? 1 : current.Alpha;
            return current;
        }

        if(current.EmptyCells() != 0) return current;

        current.IsTerminal = true;
        current.Alpha = (current.Alpha < 0) ? current.Alpha : 0;
        return current;
    }

    public static Node DefineCurrent(Node current, bool isCross, List<Node> children)
    {
        current.Alpha = int.MinValue;
        current.Beta = int.MaxValue;

        for(int i = 0; i < children.Count; i++)
        {
            if(isCross && children[i].Value > current.Alpha)
            {
                current.Alpha = children[i].Value;
                current.Value = current.Alpha;
            }
            else if(!isCross && children[i].Value < current.Beta)
            {
                current.Beta = children[i].Value;
                current.Value = current.Beta;
            }
        }

        current.Children = children.ToArray();

        return current;
    }

    public static Node DefineBoard(Node node, Node current)
    {
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

        node.Alpha = current.Alpha;
        node.Beta = current.Beta;

        return node;
    }

    public static Node DefineTree(Node current, bool isCross, bool ignore = false)
    {
        current = EvaluateNode(current, !isCross); // is a not because is a child of previous node
        if(current.IsTerminal) return current;

        List<Node> children = new List<Node>();

        for(int i = 0; i < current.Board.Length; i++)
        {
            for(int j = 0; j < current.Board[i].Length; j++)
            {
                if(current.Board[i][j].state != CellState.None) continue;

                Node node = new();
                node = DefineBoard(node, current);

                node.Board[i][j].state = isCross ? CellState.X : CellState.O;
                node.change = node.Board[i][j];

                node = DefineTree(node, !isCross, ignore);
                if(isCross && node.Value > current.Alpha) current.Alpha = node.Value;
                else if(!isCross && node.Value < current.Beta) current.Beta = node.Value;

                children.Add(node);

                if(current.Beta <= current.Alpha && !ignore) return current;
            }
        }

        current = DefineCurrent(current, isCross, children);

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

public class AlphaBetaTree
{
    Node head;
    Node current;
    bool isMaximizer;

    public AlphaBetaTree(bool isCross, Square[][] board)
    {
        head = new Node();
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
        head = Node.DefineTree(head, !isCross);
        isMaximizer = isCross;
        current = head;
    }

    public Square[][] DefineNextMove(Square chosen)
    {
        if(current.IsTerminal) return current.Board;
        for(int i = 0; i < current.Children.Length; i++)
        {
            Node child = current.Children[i];
            if(child.change.maxX != chosen.maxX || child.change.maxY != chosen.maxY) continue;

            if(child.IsTerminal) return current.Board;
            if(child.Children == null)
            {
                child = Node.DefineTree(child, isMaximizer, true);
            }
            current = child.Children[0];
            foreach(Node grandchild in child.Children)
            {
                if(grandchild.Value > current.Value && isMaximizer) current = grandchild;
                if(grandchild.Value < current.Value && !isMaximizer) current = grandchild;
            }
        }
        return current.Board;
    }
    
    public Square[][] TravelDownTree(Square chosen)
    {
        if(current.IsTerminal) return current.Board;
        foreach(Node child in current.Children)
        {
            if(child.change.maxX != chosen.maxX || child.change.maxY != chosen.maxY) continue;

            current = child;
        }
        return current.Board;
    }
}