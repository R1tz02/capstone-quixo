using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


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


    public quixoModel Clone()
    {
            quixoModel copy = new quixoModel();
            char[,] clonedBoard = {    { '-', '-', '-', '-', '-' },
                                    { '-', '-', '-', '-', '-' },
                                    { '-', '-', '-', '-', '-' },
                                    { '-', '-', '-', '-', '-' },
                                    { '-', '-', '-', '-', '-' }
        };
        for(int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                clonedBoard[i, j] = board[i, j];

            }
        }
        copy.board = clonedBoard;
        copy.playerOneTurn = playerOneTurn;
        return copy;
}


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
        for (int i = 0; i< 5; i++)
        {
            score += EvalLine(board[i, 0], board[i, 1], board[i, 2], board[i, 3], board[i, 4]);
            score += EvalLine(board[0, i], board[1, i], board[2, i], board[3, i], board[4, i]);
        }
        score += EvalLine(board[0, 0], board[1, 1], board[2, 2], board[3, 3], board[4, 4]);
        score += EvalLine(board[0, 4], board[1, 3], board[2, 2], board[3, 2], board[4, 0]);

        return score; 
    }


    private int EvalLine(params char[] pieces)
    {
        int count = 0;
        int opponentCount= 0;
        

        foreach(char piece in pieces)
        {
            if(piece == 'O')
            {
                count++;
            }
            else if(piece == 'X')
            {
                opponentCount++;
            }
        }
        if (opponentCount > 0 && count < opponentCount)
        {
            return -(int)Math.Pow(10, opponentCount);
        }
        else if (count > 0 && opponentCount < count)
        {
            return (int)Math.Pow(10, count);
        }
        else
            return 0;
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
    public int Minimax(quixoModel board, int depth, bool maximizing, int alpha, int beta)
    {
    
        if (board.checkForWin() != '-' || depth == 0)
        {
            return Evaluate(board.board);
        }
        if (maximizing)
        {
            
            int maxEval = int.MinValue;
            foreach ((Piece, char) move in PossibleMoves(board))
            {
                quixoModel copy = board.Clone();
                copy.makeMove(move.Item1, move.Item2);
                                Debug.Log("Check");

                maxEval = Math.Max(maxEval, Minimax(copy.Clone(), depth - 1, false, alpha, beta));
                alpha = Math.Max(alpha, maxEval);
                if(beta <= alpha)
                {
                    break;
                }
            }
            return maxEval;

        }
        else if(!maximizing)
        {
            int minEval = int.MaxValue;
            foreach ((Piece, char) move in PossibleMoves(board))
            {
                quixoModel copy = board.Clone();
                copy.makeMove(move.Item1, move.Item2);
                Debug.Log("Check");
                minEval = Math.Min(minEval, Minimax(copy.Clone(), depth - 1, true, alpha, beta));
                beta = Math.Min(beta, minEval);
                if(beta <= alpha)
                {
                    break;
                }
            }
            return minEval;
        }
        else
        {
            return 0;
        }
    }

    public int MinimaxAlphaBeta(quixoModel board, int depth, bool aiTurn)
    {
        return Minimax(board, depth, aiTurn, int.MinValue, int.MaxValue);
    }
    //looks through all possible moves and finds the one that has will end with the highest possible score, whent he opponent
    //is also trying to maximize their score

    public (Piece, char) FindBestMove(char[,] model, int depth)
    {

        (Piece, char) bestMove = (null, ' ');
        int bestEval = int.MinValue;
        quixoModel newBoard = new quixoModel();
        newBoard.board = (char[,])model.Clone();
        newBoard.playerOneTurn = false;
        foreach ((Piece, char) move in PossibleMoves(newBoard))
        {
            quixoModel copy = newBoard.Clone();
            copy.makeMove(move.Item1, move.Item2);

            int evalScore = MinimaxAlphaBeta(copy, depth, false);

            if (evalScore > bestEval)
            {
                bestEval = evalScore;
                bestMove = move;
            }
        }
        return bestMove;

        // Update is called once per frame
    }

}


