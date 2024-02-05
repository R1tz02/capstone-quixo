using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Player //F: Holds the symbol of the particular player and whether they have won
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
        bool success = true;
        char baseSymbol = '-';
        char pieceToCheck = '-';
        for (int row = 0; row < 5; row++) 
        {
            baseSymbol = gameBoard[row, 0].GetComponent<PieceLogic>().player; //F: first value of every row is base
            for (int col = 0; col < 5; col++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<PieceLogic>().player; //F: assigned to a variable instead of callind GetComponent twice in the if
                if ( pieceToCheck== '-' || pieceToCheck != baseSymbol) //F: compare every item to the baseSymbol, ignore immediately if it is blank
                {
                    success = false; //F: if changed, not same symbols
                    break; //F: get out if not same symbol or blank, and try the next
                }
            }
           
            if (success) //F: If unchanged, we have a win
            {
                if (currentPlayer.piece == baseSymbol) //F: means that the current player is the one who won
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool verticalWin()
    {
        Debug.Log("checking for vertical win");
        bool success = true;
        char baseSymbol = '-';
        char pieceToCheck = '-';
        for (int col = 0; col < 5; col++)
        {
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
                if (currentPlayer.piece == baseSymbol)
                {
                    return true;
                }    
            }
        }
        return false;
    }

    private bool leftDiagonalWin()
    {
        char baseSymbol = '-';
        char pieceToCheck = '-';
        bool success = true;
        //check for top left to bottom right win
        baseSymbol = gameBoard[0, 0].GetComponent<PieceLogic>().player; ;
        for (int i = 0; i < 5; i++)
        {
            pieceToCheck= gameBoard[i, i].GetComponent<PieceLogic>().player;
            if (pieceToCheck != baseSymbol || pieceToCheck == '-')
            {
                success = false;
                break;
            }
        }
       
        if (success)
        {
            if (currentPlayer.piece == baseSymbol)
            {
                return true;
            }
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
            if (pieceToCheck!= baseSymbol || pieceToCheck == '-')
            {
                success = false;
                break;
            }
        }
        
        if (success)
        {
            if (currentPlayer.piece == baseSymbol)
            {
                return true;
            }
        }
        return false;
    }

    public bool won()
    { 
        Debug.Log("checking for all wins");
        if (horizontalWin()) return true;
        if (verticalWin()) return true;
        if (leftDiagonalWin()) return true; //separated checkDiagonalWin into two separate functions
        if (rightDiagonalWin()) return true;
        return false;
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
                gameBoard[i-1, chosenPiece.col].transform.position = new Vector3(gameBoard[i-1, chosenPiece.col].transform.position.x + 20, 5, gameBoard[i-1, chosenPiece.col].transform.position.z); //F: Physically move every piece in the row/col
                gameBoard[i, chosenPiece.col] = gameBoard[i - 1, chosenPiece.col]; //F: updating the array (low level implementation of the game)
            }
            //F: This could probably be better, I'll improve it this week
            gameBoard[0, chosenPiece.col] = gameBoard[0, 5]; //F: set the selected piece to its new position in the array
            gameBoard[0, chosenPiece.col].GetComponent<PieceLogic>().player = currentPiece; //F: changing the moved piece's symbol to the current
            gameBoard[0, chosenPiece.col].GetComponent<Renderer>().material = pieceColor; //F: ^ same thing but material (color) of the piece
            gameBoard[0, chosenPiece.col].transform.position = new Vector3(-40, 5, gameBoard[1, chosenPiece.col].transform.position.z); //F: physically move the selected piece
            gameBoard[0, chosenPiece.col].GetComponent<PieceLogic>().row = 0; //F: ^^ same but with row
        }
        else if (dir == 'd')
        {
            for (int i = chosenPiece.row; i < 4; i++)
            {
                gameBoard[i + 1, chosenPiece.col].GetComponent<PieceLogic>().row = i;
                gameBoard[i + 1, chosenPiece.col].transform.position = new Vector3(gameBoard[i + 1, chosenPiece.col].transform.position.x - 20, 5, gameBoard[i + 1, chosenPiece.col].transform.position.z);
                gameBoard[i, chosenPiece.col] = gameBoard[i + 1, chosenPiece.col];
            }
            gameBoard[4, chosenPiece.col] = gameBoard[0, 5];
            gameBoard[4, chosenPiece.col].GetComponent<Renderer>().material = pieceColor;
            gameBoard[4, chosenPiece.col].GetComponent<PieceLogic>().player = currentPiece;
            gameBoard[4, chosenPiece.col].transform.position = new Vector3(40, 5, gameBoard[1, chosenPiece.col].transform.position.z);
            gameBoard[4, chosenPiece.col].GetComponent<PieceLogic>().row = 4;
        }
        else if (dir == 'r')
        {
            for (int i = chosenPiece.col; i < 4; i++)
            {
                gameBoard[chosenPiece.row, i + 1].GetComponent<PieceLogic>().col = i;
                gameBoard[chosenPiece.row, i + 1].transform.position = new Vector3(gameBoard[chosenPiece.row, i + 1].transform.position.x, 5, gameBoard[chosenPiece.row, i + 1].transform.position.z - 20);
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i + 1];
            }
            gameBoard[chosenPiece.row, 4] = gameBoard[0, 5];
            gameBoard[chosenPiece.row, 4].GetComponent<Renderer>().material = pieceColor;
            gameBoard[chosenPiece.row, 4].GetComponent<PieceLogic>().player = currentPiece; 
            gameBoard[chosenPiece.row, 4].transform.position = new Vector3(gameBoard[chosenPiece.row, 1].transform.position.x, 5, 40);
            gameBoard[chosenPiece.row, 4].GetComponent<PieceLogic>().col = 4;
        }
        else if (dir == 'l')
        {
            for (int i = chosenPiece.col; i > 0; i--)
            {
                gameBoard[chosenPiece.row, i - 1].GetComponent<PieceLogic>().col = i;
                gameBoard[chosenPiece.row, i - 1].transform.position = new Vector3(gameBoard[chosenPiece.row, i - 1].transform.position.x, 5, gameBoard[chosenPiece.row, i - 1].transform.position.z + 20);
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i - 1];
            }
            gameBoard[chosenPiece.row, 0] = gameBoard[0, 5];
            gameBoard[chosenPiece.row, 0].GetComponent<Renderer>().material = pieceColor;
            gameBoard[chosenPiece.row, 0].GetComponent<PieceLogic>().player = currentPiece; 
            gameBoard[chosenPiece.row, 0].transform.position = new Vector3(gameBoard[chosenPiece.row, 1].transform.position.x, 5, -40);
            gameBoard[chosenPiece.row, 0].GetComponent<PieceLogic>().col = 0;
        }
    }

    public void makeMove(char c)
    {
        Debug.Log("make move called");
        shiftBoard(c, currentPlayer.piece);
        buttonHandler.changeArrowsBack(); //F: change arrows back for every new piece selected
        if (won()) //F: TODO add counter 
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
        if (currentPlayer.piece == 'X') { currentPlayer = p2; } else { currentPlayer = p1; }; //F: if not won, we change the currentPlayer
    }

    //Returns a list of Directions a piece can move based on the locations of the passes piece
    public List<char> moveOptions(int row, int col)
    {
        buttonHandler.changeArrowsBack();
        List<char> moveList = new List<char>();
        if (row > 0)
        {
            moveList.Add('U');
            buttonHandler.changeUpArrowColor();
        }
        if (row < 4)
        {
            moveList.Add('D');
            buttonHandler.changeDownArrowColor();
        }
        if (col > 0)
        {
            moveList.Add('L');
            buttonHandler.changeLeftArrowColor();
        }
        if (col < 4)
        {
            moveList.Add('R');
            buttonHandler.changeRightArrowColor();
        }

        return moveList;

    }

    public bool validPiece(int row, int col)
    {
        chosenPiece = gameBoard[row, col].GetComponent<PieceLogic>();
        if ((row == 0 || row == 4) || (col == 0 || col == 4)) //edge pieces
        {
            if(chosenPiece.player == '-' || chosenPiece.player == currentPlayer.piece) //F: Checks if it is currentPlayer's piece or not selected yet
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
}
    



    

