using System.Collections;
using System.Collections.Generic;
<<<<<<< Updated upstream
=======
using System.Threading;
using TMPro;
>>>>>>> Stashed changes
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameCore : MonoBehaviour
{
    public GameObject piecePrefab;
    public Material playerOneSpace;
    public Material playerTwoSpace;
<<<<<<< Updated upstream

    private PieceLogic pieceLogic;
    public GameObject[,] gameBoard = new GameObject[5, 5];
    private bool playerOneTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        populateBoard();
=======
    public ButtonHandler buttonHandler;
    private PieceLogic pieceLogic;
    private PieceLogic chosenPiece;
    private GameObject[,] gameBoard = new GameObject[5, 6];
    private Renderer rd;
    public Player currentPlayer = new Player();
    public Player p1 = new Player();
    public Player p2 = new Player();
    public int counter = 0;


    // Start is called before the first frame update
    void Start()
    {
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

<<<<<<< Updated upstream
=======
    /*private void moveChosenPiece(int row, int col, Material pieceColor, char currentPiece, float x, float y, float z)
    {
        GameObject piece = gameBoard[row, col]; // Get the game object representing the piece
        Rigidbody rb = piece.AddComponent<Rigidbody>(); // Add Rigidbody component to the piece
        rb.useGravity = false; // Disable gravity for smooth movement
        rb.isKinematic = true; // Make the Rigidbody kinematic to control movement manually

        // Set other properties of the piece
        piece.GetComponent<PieceLogic>().player = currentPiece;
        piece.GetComponent<Renderer>().material = pieceColor;
        piece.transform.position = new Vector3(x, y, z);
        piece.GetComponent<PieceLogic>().row = row;
        piece.GetComponent<PieceLogic>().col = col;
    }*/

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

        if (dir == 'U')
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
        else if (dir == 'D')
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
        else if (dir == 'R')
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
        else if (dir == 'L')
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
  
>>>>>>> Stashed changes
    private void moveChosenPiece(int row, int col, Material pieceColor, char currentPiece, float x, float y, float z)
    {
        gameBoard[row, col] = gameBoard[0, 5]; //F: set the selected piece to its new position in the array
        gameBoard[row, col].GetComponent<PieceLogic>().player = currentPiece; //F: changing the moved piece's symbol to the current
        gameBoard[row, col].GetComponent<Renderer>().material = pieceColor; //F: changing the moved piece's material (color) 
        gameBoard[row, col].transform.position = new Vector3(x, y, z); //F: physically move the selected piece
        gameBoard[row, col].GetComponent<PieceLogic>().row = row; //F: changing the moved piece's row
        gameBoard[row, col].GetComponent<PieceLogic>().col = col; //F: changing the moved piece's col
    }
    
    private void shiftBoard(char dir, char currentPiece)
    {
        Debug.Log(dir);
        gameBoard[0,5] = gameBoard[chosenPiece.row, chosenPiece. col]; //F: [0,5] is permanently used as a temp index in which we hold the selected piece
        Material pieceColor; //F: Made a variable to change the material of the piece depending on the currentPlayer
        switch (currentPiece) 
        {
            case 'X': pieceColor = playerOneSpace;
                break;
            default: pieceColor = playerTwoSpace; break;
        }

        if (dir == 'u') 
        {
            for (int i = chosenPiece.row; i > 0; i--)
            {
                gameBoard[i - 1, chosenPiece.col].GetComponent<PieceLogic>().row = i; //F: Change the piece's underlying variable holding its row
                gameBoard[i - 1, chosenPiece.col].transform.position = new Vector3(gameBoard[i - 1, chosenPiece.col].transform.position.x + 20, 5, gameBoard[i - 1, chosenPiece.col].transform.position.z); //F: Physically move every piece in the row/col
                gameBoard[i, chosenPiece.col] = gameBoard[i - 1, chosenPiece.col]; //F: updating the array (low level implementation of the game)
            }
            moveChosenPiece(0, chosenPiece.col, pieceColor, currentPiece, -40, 5, gameBoard[1, chosenPiece.col].transform.position.z);
        }
        else if (dir == 'd')
        {
            for (int i = chosenPiece.row; i < 4; i++)
            {
                gameBoard[i + 1, chosenPiece.col].GetComponent<PieceLogic>().row = i;
                gameBoard[i + 1, chosenPiece.col].transform.position = new Vector3(gameBoard[i + 1, chosenPiece.col].transform.position.x - 20, 5, gameBoard[i + 1, chosenPiece.col].transform.position.z);
                gameBoard[i, chosenPiece.col] = gameBoard[i + 1, chosenPiece.col];
            }
            moveChosenPiece(4, chosenPiece.col, pieceColor, currentPiece, 40, 5, gameBoard[1, chosenPiece.col].transform.position.z);
        }
        else if (dir == 'r')
        {
            for (int i = chosenPiece.col; i < 4; i++)
            {
                gameBoard[chosenPiece.row, i + 1].GetComponent<PieceLogic>().col = i;
                gameBoard[chosenPiece.row, i + 1].transform.position = new Vector3(gameBoard[chosenPiece.row, i + 1].transform.position.x, 5, gameBoard[chosenPiece.row, i + 1].transform.position.z - 20);
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i + 1];
            }
            moveChosenPiece(chosenPiece.row, 4, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 5, 40);
        }
        else if (dir == 'l')
        {
            for (int i = chosenPiece.col; i > 0; i--)
            {
                gameBoard[chosenPiece.row, i - 1].GetComponent<PieceLogic>().col = i;
                gameBoard[chosenPiece.row, i - 1].transform.position = new Vector3(gameBoard[chosenPiece.row, i - 1].transform.position.x, 5, gameBoard[chosenPiece.row, i - 1].transform.position.z + 20);
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i - 1];
            }
            moveChosenPiece(chosenPiece.row, 0, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 5, -40);
        }
    }

    public void makeMove(char c)
    {
<<<<<<< Updated upstream
        if (validPiece(chosenPiece.row, chosenPiece.col))
=======
        if (gamePaused)
        {
            return;
        }
        if (validPiece(chosenPiece.row, chosenPiece.col) && moveOptions(chosenPiece.row, chosenPiece.col).Contains(c))
>>>>>>> Stashed changes
        {
            shiftBoard(c, currentPlayer.piece);
            counter++;
            Debug.Log("make move called");
            buttonHandler.changeArrowsBack(); //F: change arrows back for every new piece selected
            if (counter > 8 && won()) //F: TODO add counter 
            {
                GameObject instantiatedPrefab = Instantiate(winnerText, Vector3.zero, Quaternion.identity); //F: instantiating the winner text pop up, this is temporal
                instantiatedPrefab.GetComponent<TextMesh>().text = currentPlayer.piece + " won!"; //F: setting its output content
                instantiatedPrefab.SetActive(true); //F: make it appear
                instantiatedPrefab.transform.position = new Vector3(0, 16, 50); //F: adjust it so it is visible
                instantiatedPrefab.transform.rotation = Quaternion.Euler(90, -90, 0); //F: ^
                Debug.Log(currentPlayer.piece + " won!");
                return;
            }
            //F: TODO - work on validmove error handling
            else if (currentPlayer.piece == 'X') { currentPlayer = p2; } else { currentPlayer = p1; }; //F: if not won, we change the currentPlayer
        }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
        
>>>>>>> Stashed changes
    }

    //Returns a list of Directions a piece can move based on the locations of the passes piece
    public List<char> moveOptions(int row, int col)
    {
        List<char> moveList = new List<char>();
        if (row > 0)
        {
            moveList.Add('U');
<<<<<<< Updated upstream
=======
            buttonHandler.changeArrowColor('U');
>>>>>>> Stashed changes
        }
        if (row < 4)
        {
            moveList.Add('D');
<<<<<<< Updated upstream
=======
            buttonHandler.changeArrowColor('D');
>>>>>>> Stashed changes
        }
        if (col > 0)
        {
            moveList.Add('L');
<<<<<<< Updated upstream
=======
            buttonHandler.changeArrowColor('L');
>>>>>>> Stashed changes
        }
        if (col < 4)
        {
            moveList.Add('R');
<<<<<<< Updated upstream
=======
            buttonHandler.changeArrowColor('R');
>>>>>>> Stashed changes
        }
        return moveList;
    }

    //goes through the board and resets the y cooridinate to be level with the base
    public void lowerPiece()
    {
        GameObject piece;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                piece = gameBoard[i, j];
                piece.transform.position = new Vector3(piece.transform.position.x, 5f, piece.transform.position.z);
            }
        }
    }

    //checks to see if the passed piece is a selectable piece for the player to choose
    public bool validPiece(int row, int col)
    {
        PieceLogic piece = gameBoard[row, col].GetComponent<PieceLogic>();
        if ((row == 0 || row == 4) || (col == 0 || col == 4))
        { 
            if((playerOneTurn == true && (piece.player == '-' || piece.player == 'X')) ||
                (playerOneTurn == false && (piece.player == '-' || piece.player == 'O')))
            {
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
                gameBoard[i, j] = Instantiate(piecePrefab, new Vector3(x, 5f ,z), Quaternion.identity);
                gameBoard[i, j].GetComponent<PieceLogic>().row = i;
                gameBoard[i, j].GetComponent<PieceLogic>().col = j;
                gameBoard[i, j].GetComponent<PieceLogic>().player = '-';
                gameBoard[i, j].GetComponent<PieceLogic>().game = this;
                z += 20;
            }
            x += 20;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
