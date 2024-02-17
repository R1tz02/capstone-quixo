using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameCore : MonoBehaviour
{
    public GameObject piecePrefab;
    public GameObject winnerText;
    public Material playerOneSpace;
    public Material playerTwoSpace;
    public ButtonHandler buttonHandler;
    private PieceLogic pieceLogic;
    public PieceLogic chosenPiece;
    public GameObject[,] gameBoard = new GameObject[5, 6];
    private Renderer rd;
    public IPlayer currentPlayer;
    public IPlayer p1;
    public IPlayer p2;
    public int counter = 0;
    public bool gamePaused;
    public Canvas winScreen;

    //Event for sending chosen piece to the NetworkingManager
    public delegate void ChosenPieceEvent(int row, int col);
    public static event ChosenPieceEvent OnChosenPiece;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public async void StartNetworkedGame(string gameType)
    {
        winScreen.enabled = false;

        if (gameType != "Host" && gameType != "Client")
        {
            throw new System.Exception("Not a valid game type");
        }

        NetworkingManager networkingManager = GameObject.Find("NetworkManager").GetComponent<NetworkingManager>();

        if (gameType == "Host")
        {
            await networkingManager.StartGame(GameMode.Host);
        }
        else
        {
            await networkingManager.StartGame(GameMode.Client);
        }
    }

    public void StartLocalGame()
    {
        winScreen.enabled = false;
        
        GameObject player1Object = new GameObject("Player1");
        p1 = player1Object.AddComponent<LocalPlayer>();
        p1.Initialize('X');

        GameObject player2Object = new GameObject("Player2");
        p2 = player2Object.AddComponent<LocalPlayer>();
        p2.Initialize('O');

        currentPlayer = p1;

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

    private void moveChosenPiece(int row, int col, Material pieceColor, char currentPiece, float x, float y, float z)
    {
        gameBoard[row, col] = gameBoard[0, 5]; //F: set the selected piece to its new position in the array
        gameBoard[row, col].GetComponent<PieceLogic>().player = currentPiece; //F: changing the moved piece's symbol to the current
        gameBoard[row, col].GetComponent<Renderer>().material = pieceColor; //F: changing the moved piece's material (color) 
        gameBoard[row, col].transform.position = new Vector3(x, y, z); //F: physically move the selected piece
        gameBoard[row, col].GetComponent<PieceLogic>().row = row; //F: changing the moved piece's row
        gameBoard[row, col].GetComponent<PieceLogic>().col = col; //F: changing the moved piece's col
    }

    public void shiftBoard(char dir, char currentPiece)
    {
        Debug.Log(dir);
        gameBoard[0, 5] = gameBoard[chosenPiece.row, chosenPiece.col]; //F: [0,5] is permanently used as a temp index in which we hold the selected piece
        Material pieceColor; //F: Made a variable to change the material of the piece depending on the currentPlayer
        switch (currentPiece)
        {
            case 'X':
                pieceColor = playerOneSpace;
                break;
            default: pieceColor = playerTwoSpace; break;
        }

        if (dir == 'u')
        {
            for (int i = chosenPiece.row; i > 0; i--)
            {
                gameBoard[i - 1, chosenPiece.col].GetComponent<PieceLogic>().row = i; //F: Change the piece's underlying variable holding its row
                gameBoard[i - 1, chosenPiece.col].transform.position = new Vector3(gameBoard[i - 1, chosenPiece.col].transform.position.x + 20, 100f, gameBoard[i - 1, chosenPiece.col].transform.position.z); //F: Physically move every piece in the row/col
                gameBoard[i, chosenPiece.col] = gameBoard[i - 1, chosenPiece.col]; //F: updating the array (low level implementation of the game)
            }
            moveChosenPiece(0, chosenPiece.col, pieceColor, currentPiece, -40, 100f, gameBoard[1, chosenPiece.col].transform.position.z);
        }
        else if (dir == 'd')
        {
            for (int i = chosenPiece.row; i < 4; i++)
            {
                gameBoard[i + 1, chosenPiece.col].GetComponent<PieceLogic>().row = i;
                gameBoard[i + 1, chosenPiece.col].transform.position = new Vector3(gameBoard[i + 1, chosenPiece.col].transform.position.x - 20, 100f, gameBoard[i + 1, chosenPiece.col].transform.position.z);
                gameBoard[i, chosenPiece.col] = gameBoard[i + 1, chosenPiece.col];
            }
            moveChosenPiece(4, chosenPiece.col, pieceColor, currentPiece, 40, 100f, gameBoard[1, chosenPiece.col].transform.position.z);
        }
        else if (dir == 'r')
        {
            for (int i = chosenPiece.col; i < 4; i++)
            {
                gameBoard[chosenPiece.row, i + 1].GetComponent<PieceLogic>().col = i;
                gameBoard[chosenPiece.row, i + 1].transform.position = new Vector3(gameBoard[chosenPiece.row, i + 1].transform.position.x, 100f, gameBoard[chosenPiece.row, i + 1].transform.position.z - 20);
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i + 1];
            }
            moveChosenPiece(chosenPiece.row, 4, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, 40);
        }
        else if (dir == 'l')
        {
            for (int i = chosenPiece.col; i > 0; i--)
            {
                gameBoard[chosenPiece.row, i - 1].GetComponent<PieceLogic>().col = i;
                gameBoard[chosenPiece.row, i - 1].transform.position = new Vector3(gameBoard[chosenPiece.row, i - 1].transform.position.x, 100f, gameBoard[chosenPiece.row, i - 1].transform.position.z + 20);
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i - 1];
            }
            moveChosenPiece(chosenPiece.row, 0, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, -40);
        }
    }

    public bool makeMove(char c)
    {
        if (gamePaused)
        {
            return false;
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
                return true;
            }
            //F: TODO - work on validmove error handling

            currentPlayer = currentPlayer == p1 ? p2 : p1;

            return true;
        }

        else
        {
            return false;
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
                gameBoard[i, j] = Instantiate(piecePrefab, new Vector3(x, 100f, z), Quaternion.identity);
                gameBoard[i, j].GetComponent<PieceLogic>().row = i;
                gameBoard[i, j].GetComponent<PieceLogic>().col = j;
                gameBoard[i, j].GetComponent<PieceLogic>().player = '-';
                gameBoard[i, j].GetComponent<PieceLogic>().game = this;
                gameBoard[i, j].GetComponent<Rigidbody>().useGravity = true;
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