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
    public Square[] move;
    public bool isTerminal;
    public CellState state;
    public bool expanded;


    public int value;
    public int simCount;
    public List<double> UCTVals; 
    public List<MonteCarloNode> children;

    public MonteCarloNode(Square previous,
                          Square changed, 
                          bool isTerminal, 
                          CellState state, 
                          int value)
    {
        move = new Square[2] {previous, changed};

        this.isTerminal = isTerminal;
        this.state = state;
        this.value = value;
        expanded = false;
        simCount = 0;
    }

    public void AddPossibleMove(List<MonteCarloNode> possible, CellState current, Square[][] board, int x, int y, int extensionX, int extensionY, CellState state)
    { // FIX ADDPOSSIBLE MOVE LOGIC TO INCORPORATE CAPTURES PROPERLY
        if(board[x][y].state == current) return;
        else if(extensionY >= board.Length || extensionY <= 0) return;
        else if(extensionX >= board[0].Length || extensionX <= 0) return;

        possible.Add(new MonteCarloNode(board[x][y], board[extensionX][extensionY], false, state, int.MaxValue));
    }

    public void AssessMoveOptions(List<MonteCarloNode> possible, Square[][] board, int x, int y, CellState current)
    {
        int newY = (current == CellState.Red) ? y - 1 : y + 1;
        int extensionY = (current == CellState.Red) ? newY - 1 : newY + 1;

        AddPossibleMove(possible, current, board, x - 1, newY, x - 2, extensionY, state);
        AddPossibleMove(possible, current, board, x + 1, newY, x + 2, extensionY, state);

        if(!board[x][y].isQueen) return;

        newY = (current == CellState.Red) ? y + 1 : y - 1;
        extensionY = (current == CellState.Red) ? newY + 1 : newY - 1;

        AddPossibleMove(possible, current, board, x - 1, newY, x - 2, extensionY, state);
        AddPossibleMove(possible, current, board, x + 1, newY, x + 2, extensionY, state);
    }

    public List<MonteCarloNode>[] PossibleMoves(MonteCarloNode node, Square[][] board)
    {
        List<MonteCarloNode>[] moves = new List<MonteCarloNode>[Dimensions.ChipCount];
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
    public MonteCarloNode current;
    public bool isMaximizer; // AKA is RED

    public int SimulateAndBackProp(MonteCarloNode node, Square[][] board, bool isRed)
    {
        if(node.isTerminal) return node.TerminalStateStatus(node, board);
        
        if(node.expanded)
        {
            node.CalculateUCTVals();
            double wantedVal = node.UCTVals.Max();
            int value = SimulateAndBackProp(node.children[node.FindIndex(wantedVal)], board, !isRed);
            node.value += value;
            node.simCount++;
            return value;
        }
        List<MonteCarloNode>[] possible = node.PossibleMoves(node, board);
        Random random = new Random();

        int x = random.Next(possible.Length);
        int y = random.Next(possible[x].Count);
        CellState state = isRed ? CellState.Red : CellState.White;
        bool nextState = state != CellState.Red;
        
        return SimulateAndBackProp(possible[x][y], board, nextState);
    }

    public MonteCarloNode FindExpandedNode(MonteCarloNode startNode)
    {
        MonteCarloNode current = startNode;
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

    public void EvaluateTree(MonteCarloNode start)
    {
        
    }
    
    public Square[][] DefineNextMove(Square[][] board, Square newMove)
    {
        foreach(MonteCarloNode child in current.children)
        {
            if(newMove != child.move[1]) continue;

            current = child;
            break;
        }

        if(current == null) return null;

        else if(current.children == null)
        {
            
        }
    }
}