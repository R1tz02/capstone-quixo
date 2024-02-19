using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.Rendering.DebugUI.Table;

public class Player
{
    public char piece;
    public bool won = false;
}

public class GameCore : MonoBehaviour
{
    public GameObject piecePrefab;
    public GameObject winnerText;
    public Material playerOneSpace;
    public Material playerTwoSpace;
    public ButtonHandler buttonHandler;
    private PieceLogic pieceLogic;
    private PieceLogic chosenPiece;
    private GameObject[,] gameBoard = new GameObject[5, 6];
    private Renderer rd;
    public Player currentPlayer = new Player();
    public Player p1 = new Player();
    public Player p2 = new Player();
    public int counter = 0;
    public bool gamePaused;
    public Canvas winScreen;

    // Start is called before the first frame update
    void Start()
    {
        winScreen.enabled = false;
        p1.piece = 'X'; //F: assign X to player one
        currentPlayer = p1; //F: make X the first player/move
        p2.piece = 'O'; //F: assign O to player two
        buttonHandler = GameObject.FindObjectOfType<ButtonHandler>();
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
            baseSymbol = gameBoard[row, 0].GetComponent<PieceLogic>().player; //F: first value of every row is base
            for (int col = 0; col < 5; col++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<PieceLogic>().player; //F: assigned to a variable instead of callind GetComponent twice in the if
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
            baseSymbol = gameBoard[0, col].GetComponent<PieceLogic>().player; ;
            for (int row = 0; row < 5; row++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<PieceLogic>().player;
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
        baseSymbol = gameBoard[0, 0].GetComponent<PieceLogic>().player; ;
        for (int i = 1; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, i].GetComponent<PieceLogic>().player;
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
        char baseSymbol = gameBoard[0, 4].GetComponent<PieceLogic>().player;
        bool success = true;
        for (int i = 0; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, 4 - i].GetComponent<PieceLogic>().player;
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

    public bool won()
    {
        if (horizontalWin()) return true;
        if (verticalWin()) return true;
        if (leftDiagonalWin()) return true; //separated checkDiagonalWin into two separate functions
        if (rightDiagonalWin()) return true;
        return false;
    }


    private void shiftBoard(char dir, char currentPiece)
    {
        Debug.Log(dir);
        gameBoard[0, 5] = gameBoard[chosenPiece.row, chosenPiece.col]; // Store the selected piece temporarily

        Material pieceColor;
        switch (currentPiece)
        {
            case 'X':
                pieceColor = playerOneSpace;
                break;
            default:
                pieceColor = playerTwoSpace;
                break;
        }

        if (dir == 'u')
        {
            for (int i = chosenPiece.row; i > 0; i--)
            {
                GameObject currentPieceObject = gameBoard[i - 1, chosenPiece.col];
                currentPieceObject.GetComponent<PieceLogic>().row = i;
                Vector3 newPosition = currentPieceObject.transform.position + new Vector3(20, 0, 0);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                gameBoard[i, chosenPiece.col] = currentPieceObject;
            }
            moveChosenPiece(0, chosenPiece.col, pieceColor, currentPiece, -40, 100f, gameBoard[1, chosenPiece.col].transform.position.z);
        }
        else if (dir == 'd')
        {
            for (int i = chosenPiece.row; i < 4; i++)
            {
                GameObject currentPieceObject = gameBoard[i + 1, chosenPiece.col];
                currentPieceObject.GetComponent<PieceLogic>().row = i;
                Vector3 newPosition = currentPieceObject.transform.position - new Vector3(20, 0, 0);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                gameBoard[i, chosenPiece.col] = currentPieceObject;
            }
            moveChosenPiece(4, chosenPiece.col, pieceColor, currentPiece, 40, 100f, gameBoard[1, chosenPiece.col].transform.position.z);
        }
        else if (dir == 'r')
        {
            for (int i = chosenPiece.col; i < 4; i++)
            {
                GameObject currentPieceObject = gameBoard[chosenPiece.row, i + 1];
                currentPieceObject.GetComponent<PieceLogic>().col = i;
                Vector3 newPosition = currentPieceObject.transform.position - new Vector3(0, 0, 20);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                gameBoard[chosenPiece.row, i] = currentPieceObject;
            }
            moveChosenPiece(chosenPiece.row, 4, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, 40);
        }
        else if (dir == 'l')
        {
            for (int i = chosenPiece.col; i > 0; i--)
            {
                GameObject currentPieceObject = gameBoard[chosenPiece.row, i - 1];
                currentPieceObject.GetComponent<PieceLogic>().col = i;
                Vector3 newPosition = currentPieceObject.transform.position + new Vector3(0, 0, 20);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                gameBoard[chosenPiece.row, i] = currentPieceObject;
            }
            moveChosenPiece(chosenPiece.row, 0, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, -40);
        }
    }

    private IEnumerator MovePieceSmoothly(GameObject piece, Vector3 targetPosition)
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
  
    private void moveChosenPiece(int row, int col, Material pieceColor, char currentPiece, float x, float y, float z)
    {
        gameBoard[row, col] = gameBoard[0, 5]; //F: set the selected piece to its new position in the array
        gameBoard[row, col].GetComponent<PieceLogic>().player = currentPiece; //F: changing the moved piece's symbol to the current
        gameBoard[row, col].GetComponent<Renderer>().material = pieceColor; //F: changing the moved piece's material (color) 
        Vector3 target = new Vector3(x, y + 15, z);
        StartCoroutine(MovePieceSmoothly(gameBoard[row, col], target));
        gameBoard[row, col].GetComponent<PieceLogic>().row = row; //F: changing the moved piece's row
        gameBoard[row, col].GetComponent<PieceLogic>().col = col; //F: changing the moved piece's col
        gameBoard[row, col].GetComponent<Rigidbody>().mass = 100.0f;
        gameBoard[row, col].GetComponent<Rigidbody>().useGravity = true;
    }


    public void makeMove(char c)
    {
        if (gamePaused)
        {
            return;
        }
        if (validPiece(chosenPiece.row, chosenPiece.col))
        {
            shiftBoard(c, currentPlayer.piece);
            counter++;
            buttonHandler.changeArrowsBack(); //F: change arrows back for every new piece selected
            if (counter > 8 && won()) //F: TODO add counter 
            {
                winScreen.enabled = true;
                Time.timeScale = 0;
                gamePaused = true;
                Debug.Log(currentPlayer.piece + " won!");
                return;
            }
            //F: TODO - work on validmove error handling
            else if (currentPlayer.piece == 'X') { currentPlayer = p2; } else { currentPlayer = p1; }; //F: if not won, we change the currentPlayer
        }
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
        PieceLogic piece = gameBoard[row, col].GetComponent<PieceLogic>();
        if ((row == 0 || row == 4) || (col == 0 || col == 4))
        {
            if (piece.player == '-' || currentPlayer.piece == piece.player)
            {
                chosenPiece = piece;
                return true;
            }
        }
        return false;
    }

    //fills the board with GamePiece Objects and sets the important feilds
    void populateBoard()
    {
        int x = -40;
        int z = -40;
        for (int i = 0; i < 5; i++)
        {
            z = -40;
            for (int j = 0; j < 5; j++)
            {
                gameBoard[i, j] = Instantiate(piecePrefab, new Vector3(x, 100f, z), Quaternion.identity);
                gameBoard[i, j].GetComponent<PieceLogic>().row = i;
                gameBoard[i, j].GetComponent<PieceLogic>().col = j;
                gameBoard[i, j].GetComponent<PieceLogic>().player = '-';
                gameBoard[i, j].GetComponent<PieceLogic>().game = this;
                gameBoard[i, j].GetComponent<Rigidbody>().useGravity = true;
                gameBoard[i, j].GetComponent<Rigidbody>().mass = 100.0f;
                z += 20;
            }
            x += 20;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}