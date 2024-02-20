using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;


public class Piece {
   public int row;
   public int col; 
}
public class quixoModel
{
    Piece piece = new Piece();

    public char[,] board = {    { '-', '-', '-', '-', '-' },
                                    { '-', '-', '-', '-', '-' },
                                    { '-', '-', '-', '-', '-' },
                                    { '-', '-', '-', '-', '-' },
                                    { '-', '-', '-', '-', '-' }
        };
    public bool playerOneTurn = true;




    public void movePiece(Piece piece, char dir)
    {
        if (validPiece(piece))
        {
            makeMove(piece, dir);
            if (checkForWin() == '-')
            {
                playerOneTurn = !playerOneTurn;
            }
        }
    }

    public void makeMove(Piece piece, char dir)
    {
        List<char> moveList = moveOptions(piece);
        char currentPiece;
        if (playerOneTurn == true)
        {
            currentPiece = 'X';
        }
        else { currentPiece = 'O'; }
        foreach (char move in moveList)
        {
            if (dir == move)
            {
                shiftBoard(piece, dir, currentPiece);
                break;
            }
        }
    }

    private void shiftBoard(Piece piece, char dir, char currentTurn)
    {
        board[piece.row, piece.col] = 'e';
        if (dir == 'U')
        {
            for (int i = piece.row; i > 0; i--)
            {
                char temp = board[i, piece.col];
                board[i, piece.col] = board[i - 1, piece.col];
                board[i - 1, piece.col] = temp;
            }
        }
        else if (dir == 'D')
        {
            for (int i = piece.row; i < 4; i++)
            {
                char temp = board[i, piece.col];
                board[i, piece.col] = board[i + 1, piece.col];
                board[i + 1, piece.col] = temp;
            }
        }
        else if (dir == 'R')
        {
            for (int i = piece.col; i < 4; i++)
            {
                char temp = board[piece.row, i];
                board[piece.row, i] = board[piece.row, i + 1];
                board[piece.row, i + 1] = temp;
            }
        }
        else if (dir == 'L')
        {
            for (int i = piece.col; i > 0; i--)
            {
                char temp = board[piece.row, i];
                board[piece.row, i] = board[piece.row, i - 1];
                board[piece.row, i - 1] = temp;
            }
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (board[i, j] == 'e')
                {
                    board[i, j] = currentTurn;
                    break;
                }
            }
        }

    }

    public List<char> moveOptions(Piece piece)
    {
        List<char> moveList = new List<char>();
        if (piece.row > 0)
        {
            moveList.Add('u');
        }
        if (piece.row < 4)
        {
            moveList.Add('d');
        }
        if (piece.col > 0)
        {
            moveList.Add('l');
        }
        if (piece.col < 4)
        {
            moveList.Add('r');
        }
        return moveList;
    }

    private char checkHorizontalWin(char winnerSymbol)
    {
        int count = 0;
        char baseSymbol = '-';
        for (int i = 0; i < 5; i++)
        {
            count = 0;
            baseSymbol = (board[i, 0] == '-') ? 'e' : board[i, 0];
            for (int j = 0; j < 5; j++)
            {
                if (baseSymbol == board[i, j])
                {
                    count++;
                }
            }
            if (count == 5)
            {
                //Check who wins on player 1s turn
                if (playerOneTurn == true && baseSymbol == 'O')
                {
                    winnerSymbol = 'O';
                }
                else if (playerOneTurn == true && baseSymbol == 'X' && winnerSymbol != 'O')
                {
                    winnerSymbol = 'X';
                }

                //Check who wins on player 2s turn
                if (playerOneTurn == false && baseSymbol == 'X')
                {
                    winnerSymbol = 'X';
                }
                else if (playerOneTurn == false && baseSymbol == 'O' && winnerSymbol != 'X')
                {
                    winnerSymbol = 'O';
                }
            }
        }
        return winnerSymbol;
    }


    private char checkVerticalWin(char winnerSymbol)
    {
        int count = 0;
        char baseSymbol = '-';
        for (int i = 0; i < 5; i++)
        {
            count = 0;
            baseSymbol = (board[0, i] == '-') ? 'e' : board[0, i];
            for (int j = 0; j < 5; j++)
            {
                if (baseSymbol == board[j, i])
                {
                    count++;
                }
            }
            if (count == 5)
            {
                //Check who wins on player 1s turn
                if (playerOneTurn == true && baseSymbol == 'O')
                {
                    winnerSymbol = 'O';
                }
                else if (playerOneTurn == true && baseSymbol == 'X' && winnerSymbol != 'O')
                {
                    winnerSymbol = 'X';
                }

                //Check who wins on player 2s turn
                if (playerOneTurn == false && baseSymbol == 'X')
                {
                    winnerSymbol = 'X';
                }
                else if (playerOneTurn == false && baseSymbol == 'O' && winnerSymbol != 'X')
                {
                    winnerSymbol = 'O';
                }
            }
        }
        return winnerSymbol;
    }

    private char checkDiagonalWin(char winnerSymbol)
    {
        int count = 0;
        char baseSymbol = '-';
        //check for top left to bottom right win
        count = 1;
        baseSymbol = (board[0, 0] == '-') ? 'e' : board[0, 0];
        for (int i = 0; i < 5; i++)
        {

            if (baseSymbol == board[i, i])
            {
                count++;
            }
        }
        if (count == 5)
        {
            //Check who wins on player 1s turn
            if (playerOneTurn == true && baseSymbol == 'O')
            {
                winnerSymbol = 'O';
            }
            else if (playerOneTurn == true && baseSymbol == 'X' && winnerSymbol != 'O')
            {
                winnerSymbol = 'X';
            }

            //Check who wins on player 2s turn
            if (playerOneTurn == false && baseSymbol == 'X')
            {
                winnerSymbol = 'X';
            }
            else if (playerOneTurn == false && baseSymbol == 'O' && winnerSymbol != 'X')
            {
                winnerSymbol = 'O';
            }
        }

        //check for bottom left to top right win
        count = 0;
        baseSymbol = (board[4, 0] == '-') ? 'e' : board[4, 0];

        for (int i = 0; i < 5; i++)
        {

            if (baseSymbol == board[4 - i, i])
            {
                count++;
            }
        }
        if (count == 5)
        {
            //Check who wins on player 1s turn
            if (playerOneTurn == true && baseSymbol == 'O')
            {
                winnerSymbol = 'O';
            }
            else if (playerOneTurn == true && baseSymbol == 'X' && winnerSymbol != 'O')
            {
                winnerSymbol = 'X';
            }

            //Check who wins on player 2s turn
            if (playerOneTurn == false && baseSymbol == 'X')
            {
                winnerSymbol = 'X';
            }
            else if (playerOneTurn == false && baseSymbol == 'O' && winnerSymbol != 'X')
            {
                winnerSymbol = 'O';
            }
        }
        return winnerSymbol;
    }

    public char checkForWin()
    {
        char winnerSymbol = '-';
        winnerSymbol = checkHorizontalWin(winnerSymbol);
        winnerSymbol = checkVerticalWin(winnerSymbol);
        winnerSymbol = checkDiagonalWin(winnerSymbol);
        return winnerSymbol;
    }

    public bool validPiece(Piece piece)
    {

        if (piece.row == 0 || piece.row == 4)
        {
            if ((playerOneTurn == true && (board[piece.row, piece.col] == '-' || board[piece.row, piece.col] == 'X')) ||
            (playerOneTurn == false && (board[piece.row, piece.col] == '-' || board[piece.row, piece.col] == 'O')))
            {
                return true;
            }
        }
        else if (piece.col == 0 || piece.col == 4)
        {
            if ((playerOneTurn == true && (board[piece.row, piece.col] == '-' || board[piece.row, piece.col] == 'X')) ||
            (playerOneTurn == false && (board[piece.row, piece.col] == '-' || board[piece.row, piece.col] == 'O')))
            {
                return true;
            }
        }
        return false;
    }
}

public class EasyAI : MonoBehaviour
{

    // Start is called before the first frame update



    //evaluates the state of the board using (# of AI pieces in a row ^ 2) - (# of user pieces in a row ^ 2) + number of pieces on board to prioritze adding a new piece
    public int Evaluate(char[,] board)
    {
        int score = 0;
        score += (getHorizontalScore(board) + getVerticalScore(board) + getDiagonalScore(board) + getPieceCount(board));
        return score;
    }


    private int getPieceCount(char[,] board)
    {
        int count = 0;
        int opponentCount = 0;
        int score = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (board[i, j] == 'O')
                {
                    count++;
                }
                if (board[i, j] == '-')
                {
                    count = count;
                }
                else
                {
                    opponentCount++;
                }
            }
        }
        score = count - opponentCount;
        return score / 2;
    }
    private int getHorizontalScore(char[,] board)
    {
        int count = 0;
        int opponentCount = 0;
        int score = 0;
        for (int i = 0; i < 5; i++)
        {
            count = 0;
            for (int j = 0; j < 5; j++)
            {
                if ('O' == board[i, j])
                {
                    count++;
                    score += count ^ 2;
                }
                if (board[i, j] == '-')
                {
                    score += 0;
                }
                else
                {
                    opponentCount++;
                    score += -(opponentCount ^ 2);
                }
            }
        }
        return score;
    }
    private int getVerticalScore(char[,] board)
    {
        int count = 0;
        int opponentCount = 0;
        int score = 0;
        for (int i = 0; i < 5; i++)
        {
            count = 0;
            for (int j = 0; j < 5; j++)
            {
                if ('O' == board[j, i])
                {
                    count++;
                    score += count ^ 2;
                }
                if (board[j, i] == '-')
                {
                    score += 0;
                }
                else
                {
                    opponentCount++;
                    score += -(opponentCount ^ 2);
                }
            }
        }
        return score;
    }
    private int getDiagonalScore(char[,] board)
    {
        int count = 0;
        int opponentCount = 0;
        int score = 0;
        for (int i = 0; i < 5; i++)
        {
            count = 0;
            if ('O' == board[i, i])
            {
                count++;
                score += count ^ 2;
            }
            if (board[i, i] == '-')
            {
                score += 0;
            }
            else
            {
                opponentCount++;
                score += -(opponentCount ^ 2);
            }
        }
        count = 0;

        for (int i = 0; i < 5; i++)
        {

            if ('O' == board[i, i])
            {
                count++;
                score += count ^ 2;
            }
            if (board[i, i] == '-')
            {
                score += 0;
            }
            else
            {
                opponentCount++;
                score += -(opponentCount ^ 2);
            }
        }
        return score;
    }






    //need to create function that creates a list of all possible moves
    public List<(Piece, char)> PossibleMoves(quixoModel model)
    {
        List<(Piece, Char)> PosMoves = new List<(Piece, Char)>();

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Piece move = new Piece();
                move.row = i; move.col = j;
                if (model.validPiece(move))
                {
                    List<char> moveOptions = model.moveOptions(move);
                    foreach (char moves in moveOptions)
                    {
                        PosMoves.Add((move, moves));
                    }
                }

            }
        }
        return PosMoves;
    }



    //
    public int Minimax(quixoModel model, int depth, bool aiTurn)
    {
        quixoModel copyModel = new quixoModel();
        copyModel.board = (char[,])model.board.Clone();
        if (copyModel.checkForWin() != '-' || depth == 0)
        {
            return Evaluate(copyModel.board);
        }
        if (aiTurn)
        {
            int maxEval = int.MinValue;
            foreach ((Piece, char) move in PossibleMoves(copyModel))
            {
                copyModel.makeMove(move.Item1, move.Item2);
                maxEval = Math.Max(maxEval, Minimax(copyModel, depth - 1, false));
            }
            return maxEval;

        }
        else
        {
            int minEval = int.MaxValue;
            foreach ((Piece, char) move in PossibleMoves(copyModel))
            {
                copyModel.makeMove(move.Item1, move.Item2);
                minEval = Math.Min(minEval, Minimax(copyModel, depth - 1, true));
            }
            return minEval;
        }
    }


    //looks through all possible moves and finds the one that has will end with the highest possible score, whent he opponent
    //is also trying to maximize their score

    public (Piece, char) FindBestMove(char[,] model, int depth)
    {

        (Piece, char) bestMove = (null, ' ');
        int bestEval = int.MinValue;

        quixoModel newBoard = new quixoModel();
        newBoard.board = model;
        newBoard.playerOneTurn = false;
        foreach ((Piece, char) move in PossibleMoves(newBoard))
        {

            newBoard.makeMove(move.Item1, move.Item2);

            int evalScore = Minimax(newBoard, depth, true);

            if (evalScore > bestEval)
            {
                bestEval = evalScore;
                bestMove = move;
            }
            newBoard.board = model; 
        }

        return bestMove;

        // Update is called once per frame
    }

}


