using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace MonteCarloCheckers;

public class MonteCarloNode
{
    private double C = 1.5;
    public Square changed;
    public bool isTerminal;
    public CellState state;
    public bool expanded;


    public int value;
    public int simCount;
    public List<double> UCTVals; 
    public List<MonteCarloNode> children;

    public MonteCarloNode(Square changed, 
                          bool isTerminal, 
                          CellState state, 
                          int value)
    {
        this.changed = changed;
        this.isTerminal = isTerminal;
        this.state = state;
        this.value = value;
        expanded = false;
        simCount = 0;
    }

    public void AddPossibleMove(List<Square> possible, CellState current, Square[][] board, int x, int y, int extensionX, int extensionY)
    {
        if(board[x][y].state == current) return;
        else if(extensionY >= board.Length || extensionY <= 0) return;
        else if(extensionX >= board[0].Length || extensionX <= 0) return;

        possible.Add(board[x][y]);
    }

    public void AssessMoveOptions(List<Square> possible, Square[][] board, int x, int y, CellState current)
    {
        possible.Add(board[x][y]);

        int newY = (current == CellState.Red) ? y - 1 : y + 1;
        int extensionY = (current == CellState.Red) ? newY - 1 : newY + 1;

        AddPossibleMove(possible, current, board, x - 1, newY, x - 2, extensionY);
        AddPossibleMove(possible, current, board, x + 1, newY, x + 2, extensionY);

        if(!board[x][y].isQueen) return;

        newY = (current == CellState.Red) ? y + 1 : y - 1;
        extensionY = (current == CellState.Red) ? newY + 1 : newY - 1;

        AddPossibleMove(possible, current, board, x - 1, newY, x - 2, extensionY);
        AddPossibleMove(possible, current, board, x + 1, newY, x + 2, extensionY);
    }

    public List<Square>[] PossibleMoves(MonteCarloNode node, Square[][] board)
    {
        List<Square>[] moves = new List<Square>[Dimensions.ChipCount];
        int count = 0;
        CellState current = (node.state == CellState.Red) ? CellState.White : CellState.Red;
        int x, y;

        for(int i = 0; i < board.Length * board[0].Length; i++)
        {
            x = i / board[0].Length;
            y = i - x;
            if(board[x][y].state != current) continue;

            AssessMoveOptions(moves[count], board, x, y, current);

            count++;
        }
        return moves;
    }

    public void CalculateUCTVals()
    {
        for(int i = 0; i < children.Count; i++)
        {
            UCTVals[i] = children[i].value/children[i].simCount;
            UCTVals[i] += C * Math.Pow(Math.Log(simCount) / children[i].simCount, 0.5);
        }
    }

    public int FindIndex(double UCTVal)
    {
        for(int i = 0; i < UCTVals.Count; i++)
        {
            if(UCTVals[i] == UCTVal) return i;
        }
        return -1;
    }

    public int TerminalStateStatus(MonteCarloNode node, Square[][] board)
    {
        //RED is the maximizing player, WHITE is the minimizing player
        if(PossibleMoves(node, board).Length > 0) return 0;

        int value = (node.state == CellState.Red) ? 1 : -1;
        return value;
    }
}

public class MonteCarloTree
{
    public MonteCarloNode head;

    public int SimulateTree(MonteCarloNode node, Square[][] board, bool isRed)
    {
        if(node.isTerminal) return node.TerminalStateStatus(node, board);
        
        if(node.expanded)
        {
            node.CalculateUCTVals();
            double wantedVal = node.UCTVals.Max();
            return SimulateTree(node.children[node.FindIndex(wantedVal)], board, !isRed);
        }
        List<Square>[] possible = node.PossibleMoves(node, board);
        Random random = new Random();

        int index = random.Next(possible.Length);
        Square chosen = possible[index][random.Next(possible[index].Count)];
        CellState state = isRed ? CellState.Red : CellState.White;
        bool nextState = state != CellState.Red;
        
        return SimulateTree(new MonteCarloNode(chosen, false, state, 0), board, nextState);
    }

    public MonteCarloNode FindExpandedNode()
    {
        MonteCarloNode current = head;
        Random random = new Random();
        int index = 0;
        while(current.expanded)
        {
            for(int i = 0; i < current.UCTVals.Count; i++)
            {
                if(current.UCTVals[i] > current.UCTVals[index]) continue;

                index = i;
            }
            current = current.children[index];
        }
        return current;
    }
}