using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
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
        UCTVals = new List<double>();
    }

    public void AddPossibleMove(List<MonteCarloNode> possible, CellState current, Square[][] board, int x, int y, int extensionX, int extensionY, CellState state)
    { // FIX ADDPOSSIBLE MOVE LOGIC TO INCORPORATE CAPTURES PROPERLY
        if(x >= board.Length || x < 0) return;
        else if(y >= board.Length || y < 0) return;
        else if(board[x][y].state == current) return;
        else if(extensionY >= board.Length || extensionY <= 0) return;
        else if(extensionX >= board[0].Length || extensionX <= 0) return;

        possible.Add(new MonteCarloNode(board[x][y], board[extensionX][extensionY], false, state, int.MaxValue));
    }

    public void AssessMoveOptions(List<MonteCarloNode> possible, Square[][] board, int x, int y, CellState current)
    {
        int newY = (current == CellState.Red) ? y + 1 : y - 1;
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
            y = i - (x * board[0].Length);
            if(board[x][y].state != current) continue;

            moves[count] = new List<MonteCarloNode>();
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

    public void AssessMove(MonteCarloNode node, Square[][] board)
    {
        board[node.move[0].x][node.move[0].y].state = CellState.None;
        board[node.move[1].x][node.move[1].y].state = node.state;

        bool isCapture = Math.Abs(node.move[0].y) > Math.Abs(node.move[1].y) + 2;

        if(isCapture)
        {
            int x = (node.move[0].x + node.move[1].x) / 2;
            int y = (node.move[0].y + node.move[1].y) / 2;
            board[x][y].state = CellState.None;
        }
    }
}

public class MonteCarloTree
{
    public MonteCarloNode head;
    public MonteCarloNode current;
    public bool isMaximizer; // AKA is RED

    public MonteCarloTree(CellState initial)
    {
        head = new MonteCarloNode(null, null, false, initial, 0);
        current = head;
    }

    public int SimulateAndBackProp(MonteCarloNode node, Square[][] board, bool isRed, int runCount)
    {
        int status = node.TerminalStateStatus(node, board);
        node.isTerminal = status != 0;
        if(node.isTerminal || runCount >= 30) 
            return status;

        if(runCount >= 30) return 0; // Prevents infinite recursion in case of a bug
        
        if(node.expanded)
        {
            node.CalculateUCTVals();
            double wantedVal = node.UCTVals.Max();
            int value = SimulateAndBackProp(node.children[node.FindIndex(wantedVal)], board, !isRed, runCount + 1);
            node.value += value;
            node.simCount++;
            return value;
        }
        List<MonteCarloNode>[] possible = node.PossibleMoves(node, board);
        Random random = new Random();

        int x = random.Next(possible.Length);
        while(possible[x] == null || possible[x].Count == 0)
        {
            x = random.Next(possible.Length);
        }
        int y = random.Next(possible[x].Count);
        Square[][] newBoard = board;
        //X & Y below are positions on screen, not index 10/18
        int oldX = possible[x][y].move[0].x;
        int oldY = possible[x][y].move[0].y;
        int newX = possible[x][y].move[1].x;
        int newY = possible[x][y].move[1].y;

        newBoard[oldX][oldY].state = CellState.None;
        newBoard[newX][newY].state = isRed ? CellState.Red : CellState.White;

        CellState state = isRed ? CellState.Red : CellState.White;
        return SimulateAndBackProp(possible[x][y], newBoard, !isRed, runCount + 1);
    }
    
    public Square[][] DefineNextMove(Square[][] board, Square previous, Square newMove)
    {
        if(current == null) return board;
        current.children ??= new List<MonteCarloNode>();

        foreach(MonteCarloNode child in current.children)
        {
            if(previous != child.move[0] || newMove != child.move[1]) continue;

            current = child;
            break;
        }

        if(current == null) return board;
        
        int runCount = 0;
        while(!current.expanded && runCount < 100)
        {
            SimulateAndBackProp(current, board, true, 0);
            runCount++;
        }

        MonteCarloNode chosen = current.children[current.FindIndex(current.UCTVals.Max())];
        chosen.AssessMove(chosen, board);

        return board;
    }
}