using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.UI;
using System.IO;


public enum GameType
{ 
    AIEasy,
    AIHard,
    AIMedium,
    Local,
    Online
};

public enum WinType
{
    horizontal,
    vertical,
    Leftdiagonal,
    Rightdiagonal,
    helmet
};


public class GameCore : MonoBehaviour
{
    public GameObject piecePrefab;
    public WinType winType;
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
    public int SMLvl = 0;
    public int counter = 0;
    public bool gamePaused;
    public bool gameOver = false;

    [SerializeField] public AudioClip pieceClickSound;

    [SerializeField] private AudioClip hotPieceMoveSound;
    [SerializeField] private AudioClip coldPieceMoveSound;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip defeat;
    [SerializeField] private AudioClip growl;

    Image vikingWeapon;

    public SpriteRenderer spear;
    public SpriteRenderer sword;
    public SpriteRenderer axe;
    public SpriteRenderer lose;

    public GameObject swordPrefab;
    public GameObject axePrefab;
    public GameObject spearPrefab;

    private GameObject swordInstance;
    private GameObject axeInstance;
    private GameObject spearInstance;

    public Canvas loseScreen;
    public Canvas winScreen;
    public Button drawButton;
    public Canvas errorScreen;
    public Canvas buttonsCanvas;
    public Text errorText;
    public Camera CameraPosition;
    public Button restartButton;
    private PauseButton pauseButton;
    public List<(int, int)> winnerPieces = new List<(int, int)>();

    public GameType currentGameMode;


    //Event for sending chosen piece to the NetworkingManager
    public delegate void ChosenPieceEvent(int row, int col);
    public static event ChosenPieceEvent OnChosenPiece;
    
    void Start()
    {
        GameObject curPlayerVisual;
        winScreen.enabled = false;
        loseScreen.enabled = false;
        CameraPosition = Camera.main;
        errorScreen.enabled = false;
        restartButton.gameObject.SetActive(true);
        pauseButton = GameObject.Find("Menu Manager").GetComponent<PauseButton>();

        vikingWeapon = winScreen.transform.Find("Background/vikingWeapon").GetComponent<Image>();
    }

    void SetSprite(string spriteName, Image image)
    {
        // Load the sprite from the Resources folder
        Sprite sprite = Resources.Load<Sprite>(spriteName);

        // Assign the sprite to the Image component
        image.sprite = sprite;
    }

    public void showError(string error)
    { 
        errorText.text = error;
        errorScreen.enabled = true;
        gamePaused = true;
        if (winScreen.enabled == true || loseScreen.enabled == true)
        {
            winScreen.enabled = false;
            loseScreen.enabled = false;
        }

        pauseButton.gameObject.SetActive(false);
        Time.timeScale = 0;
    }
    public void closeError()
    { 
        errorScreen.enabled = false;
        gamePaused = false;
        Time.timeScale = 1;
        pauseButton.gameObject.SetActive(true);
    }

    IEnumerator RotateCamera()
    {
        float timeelapsed = 0;

        Quaternion currentRotation = CameraPosition.transform.rotation;

        // Define the target rotation
        Quaternion targetRotation = Quaternion.Euler(-25f, 270f, 0f);

        GameObject congrats = winScreen.transform.Find("Background/Header/Congrats").gameObject;

        TMP_Text text = congrats.GetComponent<TMP_Text>();
        if (currentPlayer.piece == 'O')
        {

            // One second delay before rotation starts
            yield return new WaitForSeconds(2.5f);

        SoundFXManage.Instance.PlaySoundFXClip(growl, transform, 1f);

            yield return new WaitForSeconds(1.0f);

            while (timeelapsed < 1)
        {
            // Smoothly rotate the camera towards the target rotation
            CameraPosition.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, timeelapsed / 1);
            timeelapsed += Time.deltaTime;
            yield return null;
        }

        CameraPosition.transform.rotation = targetRotation;
            SoundFXManage.Instance.PlaySoundFXClip(defeat, transform, 1f);
            // One second delay after rotation ends
            yield return new WaitForSeconds(2.75f);

           // text.text = "You have forged through the fury!";
            
            SetSprite("graveLose", vikingWeapon);
            winScreen.enabled = true;
        }
        else 
        {
            //vikingWeapon.sprite = lose;
            yield return new WaitForSeconds(3.5f);

           // text.text = "The dragons fire consumes all!";
            
            SoundFXManage.Instance.PlaySoundFXClip(victory, transform, 1f);
            winScreen.enabled = true;
        }
        
    }

    public async void StartNetworkedGame(string gameType, string code = null)
    {
        currentGameMode = GameType.Online;
        restartButton.gameObject.SetActive(false);
        drawButton.gameObject.SetActive(false);
        buttonsCanvas.enabled = false;
        GameObject.Find("Menu Manager").GetComponent<PauseButton>().pauseButton.gameObject.SetActive(false);

        if (gameType != "Host" && gameType != "Client" && gameType != "AutoHostOrClient")
        {
            throw new System.Exception("Not a valid game type");
        }

        NetworkingManager networkingManager = GameObject.Find("NetworkManager").GetComponent<NetworkingManager>();

        if (gameType == "Host")
        {
            await networkingManager.StartGame(GameMode.Host, code);
        }
        else if (gameType == "AutoHostOrClient")
        {
            await networkingManager.StartGame(GameMode.AutoHostOrClient, code);
        }
        else
        {
            await networkingManager.StartGame(GameMode.Client, code);
        }
    }
    public void StartLocalGame()
    {
        buttonsCanvas.enabled = true;
        drawButton.gameObject.SetActive(true);
        //GameObject.Find("Menu Manager").GetComponent<PauseButton>().pauseButton.gameObject.SetActive(true);
        currentGameMode = GameType.Local;
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

    private System.Collections.IEnumerator winAnimation()
    {
        List<int> verPos = new List<int> { -2866, -2876, -2856, -2846, -2836 };
        List<int> horPos = new List<int> { -10, -20, 0, 10, 20 };
        List<(int, int)> leftDiagPos = new List<(int, int)> { (-2866, -10), (-2876, -20), (-2856, 0), (-2846, 10), (-2836, 20) };
        List<(int, int)> rightDiagPos = new List<(int, int)> { (-2866, 10), (-2876, 20), (-2856, 0), (-2846, -10), (-2836, -20) };
        List<PieceLogic> listOfPieces = new List<PieceLogic>();
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitUntil(() => gamePaused == false);
            PieceLogic curPiece = gameBoard[winnerPieces[i].Item1, winnerPieces[i].Item2].GetComponent<PieceLogic>();
            listOfPieces.Add(curPiece);
            if (winType == WinType.vertical)
            {
                yield return StartCoroutine(MovePieceSmoothly(curPiece, new Vector3(verPos[i], 140, 0)));
            }
            else if (winType == WinType.horizontal)
            {
                yield return StartCoroutine(MovePieceSmoothly(curPiece, new Vector3(-2856, 140, horPos[i])));
            }
            else
            {
                if (winnerPieces.Contains((0, 0))) //means it is left diagonal
                {
                    yield return StartCoroutine(MovePieceSmoothly(curPiece, new Vector3(leftDiagPos[i].Item1, 140, leftDiagPos[i].Item2)));
                }
                else //right diagonal
                {
                    yield return StartCoroutine(MovePieceSmoothly(curPiece, new Vector3(rightDiagPos[i].Item1, 140, rightDiagPos[i].Item2)));
                }
            }
        }
        foreach (PieceLogic piece in listOfPieces)
        {
            piece.gameObject.SetActive(false);
        }
        if (winType == WinType.vertical)
        {
            swordInstance = Instantiate(swordPrefab, new Vector3(-2800, 140, 0), Quaternion.identity);
            Vector3 scale = swordInstance.transform.localScale;
            scale.y = 100f;
            scale.x = 100f;
            scale.z = 100f;
            swordInstance.transform.localScale = scale;
            swordInstance.transform.Rotate(90.0f, 0f, 90.0f, Space.Self);
        }
        if (winType == WinType.Leftdiagonal)
        {
            axeInstance = Instantiate(axePrefab, new Vector3(-2800, 140, 45), Quaternion.identity);
            Vector3 scale = axeInstance.transform.localScale;
            scale.y = 80;
            scale.x = 80;
            scale.z = 80;
            axeInstance.transform.localScale = scale;
            axeInstance.transform.Rotate(90.0f, 0, 135.0f, Space.Self);
        }
        if (winType == WinType.horizontal)
        {
            spearInstance = Instantiate(spearPrefab, new Vector3(-2850, 140, 45), Quaternion.identity);
            Vector3 scale = spearInstance.transform.localScale;
            scale.y = 50f;
            scale.x = 50f;
            scale.z = 50f;
            spearInstance.transform.localScale = scale;
            spearInstance.transform.Rotate(0f, 0, 0, Space.Self);
        }
        if (winType == WinType.Rightdiagonal)
        {
            axeInstance = Instantiate(axePrefab, new Vector3(-2800, 140, -45), Quaternion.identity);
            Vector3 scale = axeInstance.transform.localScale;
            scale.y = 80;
            scale.x = 80;
            scale.z = 80;
            axeInstance.transform.localScale = scale;
            axeInstance.transform.Rotate(-90.0f, 0, 135.0f, Space.Self);
        }
        gameOver = true;
    }

    private void highlightPieces()
    {
        (int, int) temp = winnerPieces[0];
        winnerPieces[0] = winnerPieces[1];
        winnerPieces[1] = temp;
        for (int i = 0; i< 5; i++)
        {
            gameBoard[winnerPieces[i].Item1, winnerPieces[i].Item2].AddComponent<Outline>();
            gameBoard[winnerPieces[i].Item1, winnerPieces[i].Item2].GetComponent<Outline>().OutlineWidth = 10;
        }
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
                winnerPieces.Add((row,col));
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
            winnerPieces.Clear();
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
                winnerPieces.Add((row, col));
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
            winnerPieces.Clear();
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
        baseSymbol = gameBoard[0, 0].GetComponent<PieceLogic>().player;
        winnerPieces.Add((0, 0));
        for (int i = 1; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, i].GetComponent<PieceLogic>().player;
            winnerPieces.Add((i, i));
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
        winnerPieces.Clear();

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
            winnerPieces.Add((i, 4-i));
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
        winnerPieces.Clear();
        return false;
    }


    public bool won()
    {
        if (horizontalWin())    {
            SetSprite("spearWin", vikingWeapon);
            winType = WinType.horizontal;
            return true;
        };
        if (verticalWin())      {
            SetSprite("swordWin", vikingWeapon);
            winType = WinType.vertical;
            return true;
        };
        if (leftDiagonalWin() || rightDiagonalWin())  {
            SetSprite("axeWin", vikingWeapon);
            winType = WinType.Leftdiagonal;
            //vikingWeapon.sprite = axe;
            return true;
        }; //separated checkDiagonalWin into two separate functions
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
                PieceLogic currentPieceObject = gameBoard[i - 1, chosenPiece.col].GetComponent<PieceLogic>();
                currentPieceObject.GetComponent<PieceLogic>().row = i;
                Vector3 newPosition = currentPieceObject.transform.position + new Vector3(20, 0, 0);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                currentPlayerSFX();
                gameBoard[i, chosenPiece.col] = gameBoard[i - 1, chosenPiece.col];
            }
            StartCoroutine(moveChosenPiece(0, chosenPiece.col, pieceColor, currentPiece, (-40 + -2856), 100f, gameBoard[1, chosenPiece.col].transform.position.z));
        }
        else if (dir == 'D')
        {
            for (int i = chosenPiece.row; i < 4; i++)
            {
                PieceLogic currentPieceObject = gameBoard[i + 1, chosenPiece.col].GetComponent<PieceLogic>();
                currentPieceObject.GetComponent<PieceLogic>().row = i;
                Vector3 newPosition = currentPieceObject.transform.position - new Vector3(20, 0, 0);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                currentPlayerSFX();
                gameBoard[i, chosenPiece.col] = gameBoard[i + 1, chosenPiece.col]; 
            }
             StartCoroutine(moveChosenPiece(4, chosenPiece.col, pieceColor, currentPiece, (40 + -2856), 100f, gameBoard[1, chosenPiece.col].transform.position.z));
        }
        else if (dir == 'R')
        {
            for (int i = chosenPiece.col; i < 4; i++)
            {
                PieceLogic currentPieceObject = gameBoard[chosenPiece.row, i + 1].GetComponent<PieceLogic>();
                currentPieceObject.GetComponent<PieceLogic>().col = i;
                Vector3 newPosition = currentPieceObject.transform.position - new Vector3(0, 0, 20);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                currentPlayerSFX();
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i + 1];
            }
             StartCoroutine(moveChosenPiece(chosenPiece.row, 4, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, 40));
        }
        else if (dir == 'L')
        {
            for (int i = chosenPiece.col; i > 0; i--)
            {
                PieceLogic currentPieceObject = gameBoard[chosenPiece.row, i - 1].GetComponent<PieceLogic>();
                currentPieceObject.GetComponent<PieceLogic>().col = i;
                Vector3 newPosition = currentPieceObject.transform.position + new Vector3(0, 0, 20);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                currentPlayerSFX();
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i - 1];
            }
             StartCoroutine(moveChosenPiece(chosenPiece.row, 0, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, -40));
        }
    }

    public void currentPlayerSFX()
    {
        if (currentPlayer == p1)
        {
            SoundFXManage.Instance.PlaySoundFXClip(hotPieceMoveSound, transform, 1f);
        }
        else
        {
            SoundFXManage.Instance.PlaySoundFXClip(coldPieceMoveSound, transform, 1f);
        }
    }
    public System.Collections.IEnumerator MovePieceSmoothly(PieceLogic piece, Vector3 targetPosition)
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
        gameBoard[row, col].GetComponent<PieceLogic>().player = currentPiece; //F: changing the moved piece's symbol to the current
        gameBoard[row, col].GetComponent<Renderer>().material = pieceColor; //F: changing the moved piece's material (color) 
        Vector3 target = new Vector3(x, y + 15, z);
        yield return StartCoroutine(MovePieceSmoothly(gameBoard[row, col].GetComponent<PieceLogic>(), target));
        gameBoard[row, col].GetComponent<PieceLogic>().row = row; //F: changing the moved piece's row
        gameBoard[row, col].GetComponent<PieceLogic>().col = col; //F: changing the moved piece's col
        yield return StartCoroutine(MovePieceSmoothly(gameBoard[row, col].GetComponent<PieceLogic>(), new Vector3(target.x, 96f, target.z)));
        gamePaused = false;
    }

    // force is used to force a move, even if the game is paused. Used for networking
    public bool makeMove(char c, bool force = false)
    {
        if (gamePaused && !force)
        {
            return false;
        }
        if (validPiece(chosenPiece.row, chosenPiece.col, force) && moveOptions(chosenPiece.row, chosenPiece.col).Contains(c))
        {

            shiftBoard(c, currentPlayer.piece);
            buttonHandler.changeArrowsBack(); //F: change arrows back for every new piece selected
            if (won()) 
            {
                if(currentPlayer.piece == 'X')
                    StartCoroutine(winAnimation());
                gameOver = true;
                highlightPieces();

                if (currentGameMode == GameType.Online)
                {
                    NetworkingManager networkingManager = GameObject.Find("NetworkManager").GetComponent<NetworkingManager>();

                    networkingManager._runner.SessionInfo.IsOpen = false;

                    GameObject.Find("PlayerIndicatorCanvas").gameObject.SetActive(false);
                }

                buttonsCanvas.enabled = false;
                GameObject.Find("Menu Manager").GetComponent<PauseButton>().pauseButton.gameObject.SetActive(false);
                StartCoroutine(RotateCamera());
                Debug.Log(currentPlayer.piece + " won!");
                return true;
            }
            //F: if not won, we change the currentPlayer
            else if (currentPlayer.piece == 'X') {
                currentPlayer = p2; 
            }
            else {
                currentPlayer = p1; 
            } 

            return true;
        }
        return false;
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
    // force is used to force a move, even if the game is paused. Used for networking
    public bool validPiece(int row, int col, bool force = false)
    {
        if ((gamePaused || gameOver) && !force)
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
                gameBoard[i, j] = Instantiate(piecePrefab, new Vector3((-2856 + x), 100f, z), Quaternion.identity);
                gameBoard[i, j].GetComponent<PieceLogic>().row = i;
                gameBoard[i, j].GetComponent<PieceLogic>().col = j;
                gameBoard[i, j].GetComponent<PieceLogic>().player = '-';
                gameBoard[i, j].GetComponent<PieceLogic>().game = this;
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
                aiBoard[i, j] = gameBoard[i, j].GetComponent<PieceLogic>().player;
            }
        }

        return aiBoard;
    }

    public Task ResetBoard()
    {
        foreach (GameObject piece in gameBoard)
        {
            Destroy(piece);
        }

        gameBoard = new GameObject[5, 6];

        currentPlayer = p1;
        buttonHandler.changeArrowsBack();
        winScreen.enabled = false;
        loseScreen.enabled = false;
        drawButton.gameObject.SetActive(true);
        buttonsCanvas.enabled = true;

        gameOver = false;

        Destroy(vikingWeapon);

        if (swordInstance != null)
        {
            Destroy(swordInstance);
        }
        if (axeInstance != null)
        {
            Destroy(axeInstance);
        }
        if (spearInstance != null)
        {
            Destroy(spearInstance);
        }

        PauseButton pauseButton = FindObjectOfType<PauseButton>();
        pauseButton.HideAllDrawMenus();
        pauseButton.pauseButton.gameObject.SetActive(true);

        ResetCameraRotation();

        Time.timeScale = 1;

        gamePaused = false;

        return Task.CompletedTask;
    }

    public void ResetCameraRotation()
    {
        // Use main Camera vaiable (CameraPosition) to reset Camera Position
        CameraPosition.transform.rotation = Quaternion.Euler(59.205f, 270f, 0f);
    }
}