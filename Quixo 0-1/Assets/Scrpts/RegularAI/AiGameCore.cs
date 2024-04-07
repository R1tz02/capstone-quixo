using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;

public class AiGameCore : MonoBehaviour
{
    public GameObject piecePrefab;
    public GameObject gameCoreGameObject;
    public WinType winType;
    public Material playerOneSpace;
    public Material playerTwoSpace;
    public AiButtonHandler aiButtonHandler;
    public GameObject AI;
    private AiPieceLogic aiPieceLogic;
    public AiPieceLogic chosenPiece;
    public GameObject[,] gameBoard = new GameObject[5, 6];
    private Renderer rd;
    public IPlayer currentPlayer;
    public IPlayer p1;
    public IPlayer p2;
    public int counter = 0;
    public bool gamePaused;
    public bool aiMoving = false;
    public bool aiFirst = false;
    public AIType aiType; 

    Image vikingWeapon;

    public GameType currentGameMode;

    public Camera CameraPosition;
    public Canvas loseScreen;
    public Canvas winScreen;
    public Canvas buttonsCanvas;
    private EasyAI easyAI;
    private HardAI hardAI;
    private MediumAI mediumAI;
    public List<(int, int)> winnerPieces = new List<(int, int)>();
    public bool requestDraw = false;




    //Event for sending chosen piece to the NetworkingManager
    public delegate void ChosenPieceEvent(int row, int col);
    public static event ChosenPieceEvent OnChosenPiece;
    
    void Start()
    {
        GameObject curPlayerVisual;
        winScreen.enabled = false;
        loseScreen.enabled = false;
        CameraPosition = Camera.main;

        vikingWeapon = winScreen.transform.Find("Background/vikingWeapon").GetComponent<Image>();
    }

    public enum AIType
    {
        EasyAI, 
        MediumAI,
        HardAI
    };

    void SetSprite(string spriteName, Image image)
    {
        
        // Load the sprite from the Resources folder
        Sprite sprite = Resources.Load<Sprite>(spriteName);

        // Assign the sprite to the Image component
        image.sprite = sprite;
    }

    public void StartAIGame()
    {
        if (aiType == AIType.HardAI)
        {
            currentGameMode = GameType.AIHard;
        }
        if(aiType == AIType.MediumAI)
        {
            currentGameMode = GameType.AIMedium;
        }
        else
        {
            currentGameMode = GameType.AIEasy;
        }
        GameObject player1Object = new GameObject("Player1");
        p1 = player1Object.AddComponent<LocalPlayer>();
        p1.Initialize('X');

        GameObject player2Object = new GameObject("Player2");
        p2 = player2Object.AddComponent<LocalPlayer>();
        p2.Initialize('O');

        currentPlayer = p1; //F: make X the first player/move
        aiButtonHandler = GameObject.FindObjectOfType<AiButtonHandler>();
        easyAI = AI.AddComponent(typeof(EasyAI)) as EasyAI;
        hardAI = AI.AddComponent(typeof(HardAI)) as HardAI;
        mediumAI = AI.AddComponent(typeof(MediumAI)) as MediumAI;
        populateBoard(); //Initialize board

        if (aiFirst)
        {
            Material temp = playerOneSpace;
            playerOneSpace = playerTwoSpace;
            playerTwoSpace = temp;

            if (aiType == AIType.HardAI)
            {
                HardAIMove(hardAI);
            }
            if (aiType == AIType.MediumAI)
            {
                MediumAIMove(mediumAI);
            }
            else if(aiType == AIType.EasyAI)
            {
                EasyAIMove(easyAI);
            }

        }
    }



    IEnumerator RotateCamera()
    {
        float timeelapsed = 0;

        Quaternion currentRotation = CameraPosition.transform.rotation;

        // Define the target rotation
        Quaternion targetRotation = Quaternion.Euler(-25f, 270f, 0f);

        if (currentPlayer == p2 || (aiFirst && currentPlayer == p1))
        {

            // One second delay before rotation starts
            yield return new WaitForSeconds(2.5f);

            while (timeelapsed < 1)
            {
                // Smoothly rotate the camera towards the target rotation
                CameraPosition.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, timeelapsed / 1);
                timeelapsed += Time.deltaTime;
                yield return null;
            }

            CameraPosition.transform.rotation = targetRotation;

            // One second delay after rotation ends
            yield return new WaitForSeconds(1.75f);

            loseScreen.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(2.25f);
            winScreen.enabled = true;
        }
    }

    private System.Collections.IEnumerator winAnimation()
    {
        List<int> verPos = new List<int> { -2866, -2876, -2856, -2846, -2836 };
        List<int> horPos = new List<int> { -10, -20, 0, 10, 20 };
        List<(int, int)> leftDiagPos = new List<(int, int)> { (-2866, -10), (-2876, -20), (-2856, 0), (-2846, 10), (-2836, 20) };
        List<(int, int)> rightDiagPos = new List<(int, int)> { (-2866, 10), (-2876, 20), (-2856, 0), (-2846, -10), (-2836, -20) };
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitUntil(() => gamePaused == false);
            AiPieceLogic curPiece = gameBoard[winnerPieces[i].Item1, winnerPieces[i].Item2].GetComponent<AiPieceLogic>();
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
    }

    private void highlightPieces()
    {
        for (int i = 0; i < 5; i++)
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
            baseSymbol = gameBoard[row, 0].GetComponent<AiPieceLogic>().player; //F: first value of every row is base
            for (int col = 0; col < 5; col++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<AiPieceLogic>().player; //F: assigned to a variable instead of callind GetComponent twice in the if
                winnerPieces.Add((row, col));
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
            baseSymbol = gameBoard[0, col].GetComponent<AiPieceLogic>().player; ;
            for (int row = 0; row < 5; row++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<AiPieceLogic>().player;
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
        baseSymbol = gameBoard[0, 0].GetComponent<AiPieceLogic>().player;
        winnerPieces.Add((0, 0));
        for (int i = 1; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, i].GetComponent<AiPieceLogic>().player;
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
        char baseSymbol = gameBoard[0, 4].GetComponent<AiPieceLogic>().player;
        bool success = true;
        for (int i = 0; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, 4 - i].GetComponent<AiPieceLogic>().player;
            winnerPieces.Add((i, 4 - i));
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
            winType = WinType.horizontal;
            SetSprite("spearWin", vikingWeapon);
            return true;
        };
        if (verticalWin())      {
            winType = WinType.vertical;
            SetSprite("swordWin", vikingWeapon);
            return true;
        };
        if (leftDiagonalWin())  {
            winType = WinType.diagonal;
            SetSprite("axeWin", vikingWeapon);
            return true;
        }; //separated checkDiagonalWin into two separate functions
        if (rightDiagonalWin()) {
            winType = WinType.diagonal;
            SetSprite("axeWin", vikingWeapon);
            return true;
        };
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
                AiPieceLogic currentPieceObject = gameBoard[i - 1, chosenPiece.col].GetComponent<AiPieceLogic>();
                currentPieceObject.GetComponent<AiPieceLogic>().row = i;
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
                AiPieceLogic currentPieceObject = gameBoard[i + 1, chosenPiece.col].GetComponent<AiPieceLogic>();
                currentPieceObject.GetComponent<AiPieceLogic>().row = i;
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
                AiPieceLogic currentPieceObject = gameBoard[chosenPiece.row, i + 1].GetComponent<AiPieceLogic>();
                currentPieceObject.GetComponent<AiPieceLogic>().col = i;
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
                AiPieceLogic currentPieceObject = gameBoard[chosenPiece.row, i - 1].GetComponent<AiPieceLogic>();
                currentPieceObject.GetComponent<AiPieceLogic>().col = i;
                Vector3 newPosition = currentPieceObject.transform.position + new Vector3(0, 0, 20);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i - 1];
            }
             StartCoroutine(moveChosenPiece(chosenPiece.row, 0, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, -40));
        }

    }

    public System.Collections.IEnumerator MovePieceSmoothly(AiPieceLogic piece, Vector3 targetPosition)
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
        gameBoard[row, col].GetComponent<AiPieceLogic>().player = currentPiece; //F: changing the moved piece's symbol to the current
        gameBoard[row, col].GetComponent<Renderer>().material = pieceColor; //F: changing the moved piece's material (color) 
        Vector3 target = new Vector3(x, y + 15, z);
        yield return StartCoroutine(MovePieceSmoothly(gameBoard[row, col].GetComponent<AiPieceLogic>(), target));
        gameBoard[row, col].GetComponent<AiPieceLogic>().row = row; //F: changing the moved piece's row
        gameBoard[row, col].GetComponent<AiPieceLogic>().col = col; //F: changing the moved piece's col
        yield return StartCoroutine(MovePieceSmoothly(gameBoard[row, col].GetComponent<AiPieceLogic>(), new Vector3(target.x, 96f, target.z)));
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
            aiButtonHandler.changeArrowsBack(); //F: change arrows back for every new piece selected
            if (won()) 
            {
                StartCoroutine(winAnimation());
                highlightPieces();
                buttonsCanvas.enabled = false; 
                GameObject.Find("Menu Manager").GetComponent<AiPauseButton>().pauseButton.gameObject.SetActive(false);
                //StartCoroutine(RotateCamera());

                gamePaused = true;
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
            gamePaused = false;
            aiMoving = true; 

            if (aiType == AIType.HardAI)
            {
                HardAIMove(hardAI);
            }
            if (aiType == AIType.MediumAI)
            {
                MediumAIMove(mediumAI);
            }
            else if(aiType == AIType.EasyAI)
            {
                EasyAIMove(easyAI);
            }
            return true;
        }
        return false;
    }

    public bool drawAccepted()
    {
        int score;
        char[,] board = translateBoard();
        if (aiType == AIType.HardAI)
        {
            score = hardAI.Evaluate(board);

        }
        else
        {
            score = easyAI.Evaluate(board);
        }
        if (score < 1800)
        {
            return true;
        }
        return false;
    }

    async void HardAIMove(HardAI hardAI)
    {
        aiMoving = true;
        char[,] board = translateBoard();
        Debug.Log("Fernando's mother");
        TimeSpan timeLimit = TimeSpan.FromSeconds(4);

        (Piece, char) move = await Task.Run(() => hardAI.IterativeDeepening(board, timeLimit, aiFirst));

        if(validPiece(move.Item1.row, move.Item1.col, true))
        {
            shiftBoard(move.Item2, currentPlayer.piece);
            Debug.Log("Row: " + move.Item1.row + "Col: " + move.Item1.col + ":" + move.Item2);
            if (won())
            {
                StartCoroutine(winAnimation());
                highlightPieces();
                buttonsCanvas.enabled = false;
                GameObject.Find("Menu Manager").GetComponent<AiPauseButton>().pauseButton.gameObject.SetActive(false);
                gamePaused = true;

                //StartCoroutine(RotateCamera());

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
            gamePaused = false;
            aiMoving = false;
            Debug.Log("Board State Score: " + hardAI.Evaluate(translateBoard()));

        }
    }
    async void MediumAIMove(MediumAI mediumAI)
    {
        Debug.Log("Fernando's mother");
        char[,] board = translateBoard();

        await Task.Delay(2000);
        (Piece, char) move = await Task.Run(() => mediumAI.FindBestMove(board, 2, aiFirst));

        //await WaitFor();
        validPiece(move.Item1.row, move.Item1.col, true);
        

            shiftBoard(move.Item2, currentPlayer.piece);
            Debug.Log("Row: " + move.Item1.row + "Col: " + move.Item1.col + ":" + move.Item2);
            if (won())
            {
                buttonsCanvas.enabled = false;
                GameObject.Find("Menu Manager").GetComponent<AiPauseButton>().pauseButton.gameObject.SetActive(false);
                highlightPieces();

                StartCoroutine(RotateCamera());

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
            gamePaused = false;
            aiMoving = false;
        
    }
    async void EasyAIMove(EasyAI easyAI)
    {
        Debug.Log("Fernando's mother");
        char[,] board = translateBoard();

        await Task.Delay(1500);
        (Piece, char) move = await Task.Run(() => easyAI.FindBestMove(board, 0));

        //await WaitFor();
        validPiece(move.Item1.row, move.Item1.col, true);
        shiftBoard(move.Item2, currentPlayer.piece);
        Debug.Log("Row: " + move.Item1.row + "Col: " + move.Item1.col + ":" + move.Item2);
        if (won())
        {
            buttonsCanvas.enabled = false;
            GameObject.Find("Menu Manager").GetComponent<AiPauseButton>().pauseButton.gameObject.SetActive(false);
            StartCoroutine(winAnimation());
            highlightPieces();
            //StartCoroutine(RotateCamera());

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
        gamePaused = false;
        aiMoving = false;
    }



    public IEnumerator waitAI(EasyAI easyAI)
    {
        new WaitForSeconds(2);
        EasyAIMove(easyAI);
        yield return null;
        
    }
    private void WaitFor(EasyAI easyAI)
    {
        waitAI(easyAI);
    }

    public List<char> moveOptions(int row, int col)
    {
        aiButtonHandler.changeArrowsBack();
        List<char> moveList = new List<char>();
        if (row > 0)
        {
            moveList.Add('U');
            aiButtonHandler.changeArrowColor('U');
        }
        if (row < 4)
        {
            moveList.Add('D');
            aiButtonHandler.changeArrowColor('D');
        }
        if (col > 0)
        {
            moveList.Add('L');
            aiButtonHandler.changeArrowColor('L');
        }
        if (col < 4)
        {
            moveList.Add('R');
            aiButtonHandler.changeArrowColor('R');
        }
        return moveList;
    }

    //checks to see if the passed piece is a selectable piece for the player to choose
    public bool validPiece(int row, int col, bool aiTurn = false)
    {
        if (gamePaused || (aiMoving && !aiTurn))
        {
            return false;
        }
        AiPieceLogic piece = gameBoard[row, col].GetComponent<AiPieceLogic>();
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
                gameBoard[i, j].GetComponent<AiPieceLogic>().row = i;
                gameBoard[i, j].GetComponent<AiPieceLogic>().col = j;
                gameBoard[i, j].GetComponent<AiPieceLogic>().player = '-';
                gameBoard[i, j].GetComponent<AiPieceLogic>().game = this;
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
                aiBoard[i, j] = gameBoard[i, j].GetComponent<AiPieceLogic>().player;
            }
        }

        return aiBoard;
    }
}