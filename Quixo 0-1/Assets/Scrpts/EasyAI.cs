using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EasyAI : MonoBehaviour
{

    private GameObject[,] gameBoard = new GameObject[5, 6];
    public GameCore gameCore;
    public GameObject[,] newGameBoard;
    // Start is called before the first frame update
    void Start()
    {
        newGameBoard = (GameObject[,])gameCore.gameBoard.Clone();
    }

    private int Evaluate(GameObject[,] board)
    {
        return 0;
    }
    private IEnumerable<(int row, int col, Material pieceColor, char currentPiece, float x, float y, float z)> PossibleMoves(GameCore copyModel, bool v)
    {
        throw new NotImplementedException();
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
            foreach ((Piece, char) move in PossibleMoves(copyModel, !aiTurn))
            {
                copyModel.makeMove(move.Item1, move.Item2);
                int eval = Minimax(copyModel, depth - 1, false);
                copyModel.board = (char[,])model.board.Clone();
                maxEval = Math.Max(maxEval, eval);
            }
            return maxEval;

        }
        else
        {
            int minEval = int.MaxValue;
            foreach ((int row, int col, Material pieceColor, char currentPiece, float x, float y, float z) move in PossibleMoves(copyModel, aiTurn))
            {
                copyModel.makeMove(move.Item1, move.Item2);
                int eval = Math.Min(minEval, Minimax(copyModel, depth - 1, true));
                copyModel.board = (char[,])model.board.Clone();
                minEval = Math.Min(minEval, eval);
            }
            return minEval;
        }
    }







    // Update is called once per frame
}
