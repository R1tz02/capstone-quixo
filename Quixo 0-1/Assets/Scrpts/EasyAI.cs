using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class EasyAI : MonoBehaviour
{

    private GameObject[,] gameBoard = new GameObject[5, 6];
    public GameCore gameCore;
    public GameObject[,] newGameBoard;
    private PieceLogic chosenPiece;

    // Start is called before the first frame update
    void Start()
    {}

    private int Evaluate(GameObject[,] board)
    {
        //TODO
    }
    private List<(PieceLogic, char)> PossibleMoves(GameCore copyModel, bool v)
    {
        PieceLogic pieceLogic = null;
        List<(PieceLogic, char)> moves = new List<(PieceLogic, char)>();
        List<char> moveDirs;
        char pieceChar = '-';
        if (v == true)
        {
            pieceChar = 'O';
        }
        else
        {
            pieceChar = 'X';
        }

        for (int col = 0; col < 5; col++)
        {
            for (int row = 0; row < 5; row++)
            {
                char piece = copyModel.gameBoard[row, col].GetComponent<PieceLogic>().player;
                if ((row == 0 || row == 4) || (col == 0 || col == 4))
                {
                    if (piece == '-' || piece == pieceChar)
                    {
                        pieceLogic.player = pieceChar;
                        pieceLogic.row = row;
                        pieceLogic.col = col;

                        moveDirs = copyModel.moveOptions(row, col);
                        foreach(char dirs in moveDirs)
                        {
                            moves.Add((pieceLogic, dirs));
                        }
                    }
                }
            }
        }
        return moves; 
    }
    public int Minimax(GameObject[,] newBoard, int depth, bool aiTurn)
    {
        GameCore copyModel = new GameCore();
        copyModel.gameBoard = (GameObject[,])newBoard.Clone();
        if (copyModel.won() || depth == 0)
        {
            return Evaluate(copyModel.gameBoard);
        }
        if (aiTurn)
        {
            int maxEval = int.MinValue;
            foreach ((PieceLogic, char) move in PossibleMoves(copyModel, !aiTurn))
            {
                copyModel.chosenPiece = move.Item1;
                copyModel.makeMove(move.Item2);
                int eval = Minimax(copyModel.gameBoard, depth - 1, false);
                copyModel.gameBoard = (GameObject[,])newBoard.Clone();
                maxEval = Math.Max(maxEval, eval);
            }
            return maxEval;

        }
        else
        {
            int minEval = int.MaxValue;
            foreach ((PieceLogic, char) move in PossibleMoves(copyModel, aiTurn))
            {
                copyModel.chosenPiece = move.Item1;
                copyModel.makeMove(move.Item2);
                int eval = Math.Min(minEval, Minimax(copyModel.gameBoard, depth - 1, true));
                copyModel.gameBoard = (GameObject[,])newBoard.Clone();
                minEval = Math.Min(minEval, eval);
            }
            return minEval;
        }
    }

    public (PieceLogic, char) FindBestMove(int depth)
    {
        (PieceLogic, char) bestMove = (null, ' ');
        int bestEval = int.MinValue;
        GameCore newBoard = new GameCore();
        newBoard.gameBoard = (GameObject[,])gameCore.gameBoard.Clone();
        foreach ((PieceLogic, char) move in PossibleMoves(newBoard, true)){
            newBoard.chosenPiece = move.Item1;
            newBoard.makeMove(move.Item2);
            int evalScore = Minimax(newBoard.gameBoard, depth, true);

            if (evalScore > bestEval)
            {
                bestEval = evalScore;
                bestMove = move;
            }
            newBoard.gameBoard = (GameObject[,])gameCore.gameBoard.Clone();
        }
        return bestMove; 
    }







    // Update is called once per frame
}
