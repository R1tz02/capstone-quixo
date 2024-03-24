using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Fusion;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Threading;
using System.Threading.Tasks;

public class StoryGameCore : MonoBehaviour
{
    public GameObject piecePrefab;

    public Material playerOneSpace;
    public Material playerTwoSpace;
    public StoryButtonHandler buttonHandler;
    public GameObject AI;
    private StoryPieceLogic storyPieceLogic;
    public StoryPieceLogic chosenPiece;
    public GameObject[,] gameBoard = new GameObject[5, 6];
    private Renderer rd;
    public IPlayer currentPlayer;
    public IPlayer p1;
    public IPlayer p2;
    public int SMLvl = 0;
    public bool gamePaused;

    public Canvas loseScreen;
    public Canvas winScreen;
    public Canvas SMLvl2;
    public Canvas SMLvl3;
    public Canvas SMLvl4;
    private EasyAI easyAI;
    private bool playAI = false;

    //Event for sending chosen piece to the NetworkingManager
    public delegate void ChosenPieceEvent(int row, int col);
    public static event ChosenPieceEvent OnChosenPiece;

    void Start()
    {
        GameObject curPlayerVisual;
        SMLvl2.enabled = false;
        SMLvl3.enabled = false;
        SMLvl4.enabled = false;
        winScreen.enabled = false;
        loseScreen.enabled = false;
    }

    public void StartStoryGame()
    {
        playAI = true;

        GameObject player1Object = new GameObject("Player1");
        p1 = player1Object.AddComponent<LocalPlayer>();
        p1.Initialize('X');

        GameObject player2Object = new GameObject("Player2");
        p2 = player2Object.AddComponent<LocalPlayer>();
        p2.Initialize('O');

        currentPlayer = p1; //F: make X the first player/move
        buttonHandler = GameObject.FindObjectOfType<StoryButtonHandler>();
        easyAI = AI.AddComponent(typeof(EasyAI)) as EasyAI;
        populateBoard(); //Initialize board
    }

    private bool horizontalWin()
    {
        Debug.Log("checking for horizontal win");
        bool success;
        char baseSymbol = '-';
        char pieceToCheck = '-';
        for (int row = 0; row < 5; row++)
        {
            success = true;
            baseSymbol = gameBoard[row, 0].GetComponent<StoryPieceLogic>().player; //F: first value of every row is base
            for (int col = 0; col < 5; col++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<StoryPieceLogic>().player; //F: assigned to a variable instead of callind GetComponent twice in the if
                if (pieceToCheck != baseSymbol || pieceToCheck == '-') //F: compare every item to the baseSymbol, ignore immediately if it is blank
                {
                    success = false; //F: if changed, not same symbols
                    break; //F: get out if not same symbol or blank, and try the next
                }
            }
            if (success) //F: If unchanged, we have a win
            {
                if (p1.piece == baseSymbol)
                {
                    p1.won = true;
                    currentPlayer = p1;
                }
                else
                {
                    p2.won = true;
                    currentPlayer = p2;
                }
                return true;
            }
        }
        return false;
    }

    private bool verticalWin()
    {
        Debug.Log("checking for vertical win");
        bool success;
        char baseSymbol = '-';
        char pieceToCheck = '-';
        for (int col = 0; col < 5; col++)
        {
            success = true;
            baseSymbol = gameBoard[0, col].GetComponent<StoryPieceLogic>().player; ;
            for (int row = 0; row < 5; row++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<StoryPieceLogic>().player;
                if (pieceToCheck != baseSymbol || pieceToCheck == '-')
                {
                    success = false;
                    break;
                }
            }

            if (success)
            {
                if (p1.piece == baseSymbol)
                {
                    p1.won = true;
                    currentPlayer = p1;
                }
                else
                {
                    p2.won = true;
                    currentPlayer = p2;
                }
                return true;
            }
        }
        return false;
    }

    private bool leftDiagonalWin()
    {
        Debug.Log("check leftdiagonal win");
        char baseSymbol = '-';
        char pieceToCheck = '-';
        bool success = true;
        //check for top left to bottom right win
        baseSymbol = gameBoard[0, 0].GetComponent<StoryPieceLogic>().player; ;
        for (int i = 1; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, i].GetComponent<StoryPieceLogic>().player;
            if (pieceToCheck != baseSymbol || pieceToCheck == '-')
            {
                success = false;
                break;
            }
        }
        if (success)
        {
            if (p1.piece == baseSymbol)
            {
                p1.won = true;
                currentPlayer = p1;
            }
            else
            {
                p2.won = true;
                currentPlayer = p2;
            }
            return true;
        }

        return false;
    }

    private bool rightDiagonalWin()
    {
        //check for bottom left to top right 
        char pieceToCheck = '-';
        char baseSymbol = gameBoard[0, 4].GetComponent<StoryPieceLogic>().player;
        bool success = true;
        for (int i = 0; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, 4 - i].GetComponent<StoryPieceLogic>().player;
            if (pieceToCheck != baseSymbol || pieceToCheck == '-')
            {
                success = false;
                break;
            }
        }

        if (success)
        {
            if (p1.piece == baseSymbol)
            {
                p1.won = true;
                currentPlayer = p1;
            }
            else
            {
                p2.won = true;
                currentPlayer = p2;
            }
            return true;
        }
        return false;
    }

    private bool helmetWin()
    {
        Debug.Log("check helmet win");

        //check for top left to bottom right win
        char helmetPart1 = gameBoard[3, 1].GetComponent<StoryPieceLogic>().player;
        char helmetPart2 = gameBoard[2, 1].GetComponent<StoryPieceLogic>().player;
        char helmetPart3 = gameBoard[1, 2].GetComponent<StoryPieceLogic>().player;
        char helmetPart4 = gameBoard[2, 3].GetComponent<StoryPieceLogic>().player;
        char helmetPart5 = gameBoard[3, 3].GetComponent<StoryPieceLogic>().player;

        if (helmetPart1 == helmetPart2 && helmetPart2 == helmetPart3 && helmetPart3 == helmetPart4 && helmetPart4 == helmetPart5)
        {
            if (p1.piece == helmetPart1)
            {
                p1.won = true;
                currentPlayer = p1;
            }
            else
            {
                p2.won = true;
                currentPlayer = p2;
            }
            return true;
        }

        return false;
    }

    private void chooseCanvasAndWinner(ref Canvas canvasToShow)
    {
         //AI game, either SM or normal
        if (currentPlayer == p1)
        {
            canvasToShow.enabled = true;
        }
        else
        {
            loseScreen.enabled = true;
        }
    }

    public bool won()
    {        
        switch (SMLvl)
        {
            case 1: if (verticalWin()) { chooseCanvasAndWinner(ref SMLvl2); return true; } break;
            case 2: if (horizontalWin()) { chooseCanvasAndWinner(ref SMLvl3); return true; } break;
            case 3: if (leftDiagonalWin() || rightDiagonalWin()) { chooseCanvasAndWinner(ref SMLvl4); return true; } break;
            case 4: if (helmetWin()) { chooseCanvasAndWinner(ref winScreen); return true; } break;
            default: return false;
        }
        
        return false;
    }


    public void shiftBoard(char dir, char currentPiece)
    {
        Debug.Log(dir);
        gameBoard[0, 5] = gameBoard[chosenPiece.row, chosenPiece.col]; // Store the selected piece temporarily
        Material pieceColor;
        gamePaused = true;
        switch (currentPiece)
        {
            case 'X':
                pieceColor = playerOneSpace;
                break;
            default:
                pieceColor = playerTwoSpace;
                break;
        }

        if (dir == 'U')
        {
            for (int i = chosenPiece.row; i > 0; i--)
            {
                StoryPieceLogic currentPieceObject = gameBoard[i - 1, chosenPiece.col].GetComponent<StoryPieceLogic>();
                currentPieceObject.GetComponent<StoryPieceLogic>().row = i;
                Vector3 newPosition = currentPieceObject.transform.position + new Vector3(20, 0, 0);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                gameBoard[i, chosenPiece.col] = gameBoard[i - 1, chosenPiece.col];
            }
            StartCoroutine(moveChosenPiece(0, chosenPiece.col, pieceColor, currentPiece, (-40 + -2856), 100f, gameBoard[1, chosenPiece.col].transform.position.z));
        }
        else if (dir == 'D')
        {
            for (int i = chosenPiece.row; i < 4; i++)
            {
                StoryPieceLogic currentPieceObject = gameBoard[i + 1, chosenPiece.col].GetComponent<StoryPieceLogic>();
                currentPieceObject.GetComponent<StoryPieceLogic>().row = i;
                Vector3 newPosition = currentPieceObject.transform.position - new Vector3(20, 0, 0);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                gameBoard[i, chosenPiece.col] = gameBoard[i + 1, chosenPiece.col];
            }
            StartCoroutine(moveChosenPiece(4, chosenPiece.col, pieceColor, currentPiece, (40 + -2856), 100f, gameBoard[1, chosenPiece.col].transform.position.z));
        }
        else if (dir == 'R')
        {
            for (int i = chosenPiece.col; i < 4; i++)
            {
                StoryPieceLogic currentPieceObject = gameBoard[chosenPiece.row, i + 1].GetComponent<StoryPieceLogic>();
                currentPieceObject.GetComponent<StoryPieceLogic>().col = i;
                Vector3 newPosition = currentPieceObject.transform.position - new Vector3(0, 0, 20);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i + 1];
            }
            StartCoroutine(moveChosenPiece(chosenPiece.row, 4, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, 40));
        }
        else if (dir == 'L')
        {
            for (int i = chosenPiece.col; i > 0; i--)
            {
                StoryPieceLogic currentPieceObject = gameBoard[chosenPiece.row, i - 1].GetComponent<StoryPieceLogic>();
                currentPieceObject.GetComponent<StoryPieceLogic>().col = i;
                Vector3 newPosition = currentPieceObject.transform.position + new Vector3(0, 0, 20);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i - 1];
            }
            StartCoroutine(moveChosenPiece(chosenPiece.row, 0, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, -40));
        }
    }

    public System.Collections.IEnumerator MovePieceSmoothly(StoryPieceLogic piece, Vector3 targetPosition)
    {
        float duration = 0.5f; // Adjust as needed
        Vector3 startPosition = piece.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            piece.transform.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        piece.transform.position = targetPosition; // Ensure it reaches the target position precisely
    }

    private System.Collections.IEnumerator moveChosenPiece(int row, int col, Material pieceColor, char currentPiece, float x, float y, float z)
    {
        gameBoard[row, col] = gameBoard[0, 5]; //F: set the selected piece to its new position in the array
        gameBoard[row, col].GetComponent<StoryPieceLogic>().player = currentPiece; //F: changing the moved piece's symbol to the current
        gameBoard[row, col].GetComponent<Renderer>().material = pieceColor; //F: changing the moved piece's material (color) 
        Vector3 target = new Vector3(x, y + 15, z);
        yield return StartCoroutine(MovePieceSmoothly(gameBoard[row, col].GetComponent<StoryPieceLogic>(), target));
        gameBoard[row, col].GetComponent<StoryPieceLogic>().row = row; //F: changing the moved piece's row
        gameBoard[row, col].GetComponent<StoryPieceLogic>().col = col; //F: changing the moved piece's col
        yield return StartCoroutine(MovePieceSmoothly(gameBoard[row, col].GetComponent<StoryPieceLogic>(), new Vector3(target.x, 96f, target.z)));
        gamePaused = false;

    }
    public bool makeMove(char c)
    {
        if (gamePaused)
        {
            return false;
        }
        if (validPiece(chosenPiece.row, chosenPiece.col) && moveOptions(chosenPiece.row, chosenPiece.col).Contains(c))
        {
            shiftBoard(c, currentPlayer.piece);
            buttonHandler.changeArrowsBack(); //F: change arrows back for every new piece selected
            if (won())
            {
                Time.timeScale = 0;
                gamePaused = true;
                Debug.Log(currentPlayer.piece + " won!");
                return true;
            }
            //F: if not won, we change the currentPlayer
            else if (currentPlayer.piece == 'X')
            {
                currentPlayer = p2;
            }
            else
            {
                currentPlayer = p1;
            }

            if (playAI)
            {
                if (easyAI)
                {
                    AIMove(easyAI);
                }
            }

            return true;
        }
        return false;
    }

    async void AIMove(EasyAI easyAI)
    {
        Debug.Log("Fernando's mother");
        char[,] board = translateBoard();

        (Piece, char) move = await Task.Run(() => easyAI.FindBestMove(board, 2));
        await WaitFor();
        validPiece(move.Item1.row, move.Item1.col);
        shiftBoard(move.Item2, currentPlayer.piece);
        Debug.Log("Row: " + move.Item1.row + "Col: " + move.Item1.col + ":" + move.Item2);
        if (won())
        {
            Time.timeScale = 0;
            gamePaused = true;
            Debug.Log(currentPlayer.piece + " won!");
        }
        else if (currentPlayer.piece == 'X')
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
        }
    }



    public System.Collections.IEnumerator waitAI(EasyAI easyAI)
    {
        yield return new WaitForSeconds(2);
        AIMove(easyAI);

    }
    private async Task WaitFor()
    {
        await Task.Delay(1000);
    }

    public List<char> moveOptions(int row, int col)
    {
        buttonHandler.changeArrowsBack();
        List<char> moveList = new List<char>();
        if (row > 0)
        {
            moveList.Add('U');
            buttonHandler.changeArrowColor('U');
        }
        if (row < 4)
        {
            moveList.Add('D');
            buttonHandler.changeArrowColor('D');
        }
        if (col > 0)
        {
            moveList.Add('L');
            buttonHandler.changeArrowColor('L');
        }
        if (col < 4)
        {
            moveList.Add('R');
            buttonHandler.changeArrowColor('R');
        }
        return moveList;
    }

    //checks to see if the passed piece is a selectable piece for the player to choose
    public bool validPiece(int row, int col)
    {
        if (gamePaused)
        {
            return false;
        }
        StoryPieceLogic piece = gameBoard[row, col].GetComponent<StoryPieceLogic>();
        if ((row == 0 || row == 4) || (col == 0 || col == 4))
        {
            if (piece.player == '-' || currentPlayer.piece == piece.player)
            {
                chosenPiece = piece;

                OnChosenPiece?.Invoke(row, col);

                return true;
            }
        }
        return false;
    }

    //fills the board with GamePiece Objects and sets the important fields
    public void populateBoard()
    {
        int x = -40;
        int z = -40;
        for (int i = 0; i < 5; i++)
        {
            z = -40;
            for (int j = 0; j < 5; j++)
            {
                gameBoard[i, j] = Instantiate(piecePrefab, new Vector3((-2856 + x), 100f, z), Quaternion.identity);
                gameBoard[i, j].GetComponent<StoryPieceLogic>().row = i;
                gameBoard[i, j].GetComponent<StoryPieceLogic>().col = j;
                gameBoard[i, j].GetComponent<StoryPieceLogic>().player = '-';
                gameBoard[i, j].GetComponent<StoryPieceLogic>().game = this;
                z += 20;
            }
            x += 20;
        }
    }

    public char[,] translateBoard()
    {
        char[,] aiBoard = new char[5, 5];
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                aiBoard[i, j] = gameBoard[i, j].GetComponent<StoryPieceLogic>().player;
            }
        }

        return aiBoard;
    }
}