// See https://aka.ms/new-console-template for more information


using System.Runtime.CompilerServices;

public class Piece
{
    public int row;
    public int col;
}

public class quixoModel
{

    public char[,] board = {    { '-', '-', '-', '-', '-' },
                                { '-', '-', '-', '-', '-' },
                                { '-', '-', '-', '-', '-' },
                                { '-', '-', '-', '-', '-' },
                                { '-', '-', '-', '-', '-' }
    };
    private bool playerOneTurn = true;

    public void movePiece(Piece piece, char dir, int counter)
    {
        if (validPiece(piece))
        {
            makeMove(piece, dir);
            if (counter < 10 || checkForWin() == '-')  //check for win until it is actually possible
            {
                playerOneTurn = !playerOneTurn;
            }
        }
    }

    private void makeMove(Piece piece, char dir)
    {
        //F: changed to check if it is a valid option
        char currentPiece;
        if (playerOneTurn == true)
        {
            currentPiece = 'X';
        }
        else { currentPiece = 'O'; }

        if (isValidMove(piece, dir))         //F: instead of checking for the move options, check if valid move
        {
            shiftBoard(piece, dir, currentPiece);
        }

    }

    private void shiftBoard(Piece piece, char dir, char currentPiece)
    {
        if (dir == 'U') //no need for temp as it will either be our piece or blank
        {
            for (int i = piece.row; i > 0; i--)
            {
                board[i, piece.col] = board[i - 1, piece.col]; //shifted every value down 
            }
            board[0, piece.col] = currentPiece; //set the value on top
        }
        else if (dir == 'D')
        {
            for (int i = piece.row; i < 4; i++)
            {
                board[i, piece.col] = board[i + 1, piece.col];
            }
            board[4, piece.col] = currentPiece;
        }
        else if (dir == 'R')
        {
            for (int i = piece.col; i < 4; i++)
            {
                board[piece.row, i] = board[piece.row, i + 1];
            }
            board[piece.row, 4] = currentPiece;
        }
        else if (dir == 'L')
        {
            for (int i = piece.col; i > 0; i--)
            {
                board[piece.row, i] = board[piece.row, i - 1];
            }
            board[piece.row, 0] = currentPiece;
        }
    }

    private bool isValidMove(Piece piece, char dir)
    {
        switch (dir) //switch is faster + saves the iteration in makeMove
        {
            case 'L':
                if (piece.col > 0)
                    return true;
                else
                    return false;

            case 'R':
                if (piece.col < 4)
                    return true;
                else
                    return false;

            case 'U':
                if (piece.row > 0)
                    return true;
                else
                    return false;

            case 'D':
                if (piece.row < 4)
                    return true;
                else
                    return false;

            default:
                return false;
        }

    }

    private char checkHorizontalWin(char winnerSymbol)
    {
        bool success = true;
        char baseSymbol = '-'; 
        for (int row = 0; row < 5; row++) //changed the name of for loop variables
        {
            baseSymbol = board[row, 0]; //first value of every row is base
            for (int col = 0; col < 5; col++)
            {
                if (board[row,col] == '-' || board[row, col] != baseSymbol) //compare every item to the baseSymbol, ignore immediately if it is blank
                {
                    success = false; //if changed, not same symbols
                    break; //get out if not same symbol or blank, and try the next
                }
            }

            //Check who wins on player 1s turn
            if (success) 
            {
                if (playerOneTurn && baseSymbol == 'O')
                {
                    winnerSymbol = 'O';
                }
                else if (playerOneTurn && baseSymbol == 'X' && winnerSymbol != 'O')
                {
                    winnerSymbol = 'X';
                }

                //Check who wins on player 2s turn
                if (!playerOneTurn && baseSymbol == 'X')
                {
                    winnerSymbol = 'X';
                }
                else if (!playerOneTurn && baseSymbol == 'O' && winnerSymbol != 'X')
                {
                    winnerSymbol = 'O';
                }
                break;
            }
        }
        return winnerSymbol;
    }

    private char checkVerticalWin(char winnerSymbol)
    {
        bool success = true;
        char baseSymbol = '-';
        for (int col = 0; col < 5; col++)
        {
            baseSymbol = board[0, col];
            for (int row = 0; row < 5; row++)
            {
                if (board[row, col] != baseSymbol || board[row, col] == '-')
                {
                    success = false;
                    break;
                }
            }

            if (success)
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

    private char checkLeftDiagonalWin(char winnerSymbol)
    {
        char baseSymbol = '-';
        bool success = true;
        //check for top left to bottom right win
        baseSymbol = board[0, 0];
        for (int i = 0; i < 5; i++)
        {
            if (board[i, i] != baseSymbol || board[i, i] == '-')
            {
                success = false;
                break;
            }
        }
        if (success)
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

    private char checkRightDiagonalWin(char winnerSymbol)
    {
        //check for bottom left to top right 
        char baseSymbol = board[0, 4];
        bool success = true;
        for (int i = 0; i < 5; i++)
        {
            if (board[i, 4 - i] != baseSymbol || board[i, 4-i] == '-')
            {
                success = false;
                break;
            }
        }
        if (success)
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

        //if we find the win, we exit
        if ((winnerSymbol = checkHorizontalWin(winnerSymbol)) != '-') return winnerSymbol; 
        if ((winnerSymbol = checkVerticalWin(winnerSymbol)) != '-') return winnerSymbol;
        if ((winnerSymbol = checkLeftDiagonalWin(winnerSymbol)) != '-') return winnerSymbol; //separated checkDiagonalWin into two separate functions
        if ((winnerSymbol = checkRightDiagonalWin(winnerSymbol)) != '-') return winnerSymbol;

        return winnerSymbol;
    }

    private bool validPiece(Piece piece)
    {
        if (piece.row == 0 || piece.row == 4 || piece.col == 0 || piece.col == 4)  //added everything to one if
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

class TestQuixo
{
    public static void printBoard(char[,] board)
    {
        Console.WriteLine("    0 1 2 3 4");
        Console.WriteLine("  +-----------+");
        for (int i = 0; i < 5; i++)
        {
            Console.Write(i + " | ");
            for (int j = 0; j < 5; j++)
            {
                Console.Write(board[i, j]);
                if(j<4) Console.Write(' ');
            }
            Console.Write(" | " + i);
            Console.WriteLine();
        }
        Console.WriteLine("  +-----------+");
        Console.WriteLine("    0 1 2 3 4");
        Console.WriteLine();
    }

    static void Main(string[] args)
    {
        quixoModel Quixo = new quixoModel();

        Piece piece = new Piece();
        int row = -1;
        int col = -1;
        char dir = 'e';
        printBoard(Quixo.board);
        int counter = 0;
        char winner = ' ';
        bool playerOne = true;

        while (winner == '-' || counter < 9) 
        {
            if (playerOne) { Console.WriteLine("      X's turn"); } else { Console.WriteLine("      O's turn"); }
            counter++; 
            Console.Write("Please Select a Row: ");
            row = Convert.ToInt32(Console.ReadLine());

            Console.Write("Please Select a Col: ");
            col = Convert.ToInt32(Console.ReadLine());

            Console.Write("Please Select a Direction: ");
            dir = Convert.ToChar(Console.ReadLine());
            piece.row = row;
            piece.col = col;

            Quixo.movePiece(piece, dir, counter);
            Console.Clear();
            printBoard(Quixo.board);
            if (counter > 8) winner = Quixo.checkForWin();
            playerOne = !playerOne; //make sure to be in sync with the turn
        }

        Console.WriteLine(winner + " Wins!!!");
    }
}
