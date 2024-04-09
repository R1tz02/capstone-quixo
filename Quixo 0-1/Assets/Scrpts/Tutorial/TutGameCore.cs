using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Player
{
    public char piece;
    public bool won;
}

public class TutGameCore : MonoBehaviour
{
    public WinType winType;
    public Material playerOneSpace;
    public Material playerTwoSpace;
    public GameObject[,] gameBoard = new GameObject[5, 6];
    public GameObject piecePrefab;
    public TutPieceLogic chosenPiece;
    public Canvas winScreen;
    public Canvas helpMenu;
    public delegate void ChosenPieceEvent(int row, int col);
    public static event ChosenPieceEvent OnChosenPiece;
    public Player currentPlayer;
    public bool gamePaused;
    public TutButtonHandler tutButtonHandler;
    public Player p1 = new Player();
    public Player p2 = new Player();
    public int tutLvl = -1;
    public int AIcounter = 0;
    public bool aiMoving = false;
    public int usrCounter = 0;
    public List<(int, int)> winnerPieces = new List<(int, int)>();
    public bool gameOver = false;

    [SerializeField] public AudioClip pieceClickSound;

    [SerializeField] private AudioClip hotPieceMoveSound;
    [SerializeField] private AudioClip coldPieceMoveSound;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip defeat;
    [SerializeField] private AudioClip growl;
    [SerializeField] private AudioClip swordWin;
    [SerializeField] private AudioClip spearWin;
    [SerializeField] private AudioClip axeWin;
    [SerializeField] private AudioClip hammerHit;

    public Canvas buttonCanvas;
    public bool aiTurn;
    public GameObject swordPrefab;
    public GameObject axePrefab;
    public GameObject spearPrefab;

    void Start()
    {
        p1.piece = 'X';
        p2.piece = 'O';

        currentPlayer = p1;
        winScreen.enabled = false;
        helpMenu.enabled = false;
        buttonCanvas.enabled = false;
        populateBoard();
    }

    public void StartTutorial(bool tryOtherMode = false)
    {
        if (tryOtherMode)
        {
            helpMenu.enabled = false;
        }
        else { helpMenu.enabled = true; }
    }

    private System.Collections.IEnumerator winAnimation()
    {
        if (tutLvl == 0) dehighlightPeice(3, 4);
        if (tutLvl == 1) dehighlightPeice(4, 2);

        yield return new WaitForSeconds(2f);
        List<int> verPos = new List<int> { -2866, -2876, -2856, -2846, -2836 };
        List<int> horPos = new List<int> { -10, -20, 0, 10, 20 };
        List<(int, int)> leftDiagPos = new List<(int, int)> { (-2866, -10), (-2876, -20), (-2856, 0), (-2846, 10), (-2836, 20) };
        List<(int, int)> rightDiagPos = new List<(int, int)> { (-2866, 10), (-2876, 20), (-2856, 0), (-2846, -10), (-2836, -20) };
        List<TutPieceLogic> listOfPieces = new List<TutPieceLogic>();
        for (int i = 0; i < 5; i++)
        {
            TutPieceLogic curPiece = gameBoard[winnerPieces[i].Item1, winnerPieces[i].Item2].GetComponent<TutPieceLogic>();
            listOfPieces.Add(curPiece);
            if (winType == WinType.vertical)
            {
                SoundFXManage.Instance.PlaySoundFXClip(hotPieceMoveSound, transform, 1f);
                yield return StartCoroutine(MovePieceSmoothly(curPiece, new Vector3(verPos[i], 140, 0)));
            }
            else if (winType == WinType.horizontal)
            {
                SoundFXManage.Instance.PlaySoundFXClip(hotPieceMoveSound, transform, 1f);
                yield return StartCoroutine(MovePieceSmoothly(curPiece, new Vector3(-2856, 140, horPos[i])));
            }
            else
            {
                if (winnerPieces.Contains((0, 0))) //means it is left diagonal
                {
                    SoundFXManage.Instance.PlaySoundFXClip(hotPieceMoveSound, transform, 1f);
                    yield return StartCoroutine(MovePieceSmoothly(curPiece, new Vector3(leftDiagPos[i].Item1, 140, leftDiagPos[i].Item2)));
                }
                else //right diagonal
                {
                    SoundFXManage.Instance.PlaySoundFXClip(hotPieceMoveSound, transform, 1f);
                    yield return StartCoroutine(MovePieceSmoothly(curPiece, new Vector3(rightDiagPos[i].Item1, 140, rightDiagPos[i].Item2)));
                }
            }
            SoundFXManage.Instance.PlaySoundFXClip(hammerHit, transform, 1f);
        }
        foreach (TutPieceLogic piece in listOfPieces)
        {
            piece.gameObject.SetActive(false);
        }
        if (winType == WinType.vertical)
        {
            SoundFXManage.Instance.PlaySoundFXClip(swordWin, transform, 1f);
            GameObject sword = Instantiate(swordPrefab, new Vector3(-2800, 140, 0), Quaternion.identity);
            Vector3 scale = sword.transform.localScale;
            scale.y = 100f;
            scale.x = 100f;
            scale.z = 100f;
            sword.transform.localScale = scale;
            sword.transform.Rotate(90.0f, 0f, 90.0f, Space.Self);
        }
        if (winType == WinType.Leftdiagonal)
        {
            SoundFXManage.Instance.PlaySoundFXClip(axeWin, transform, 1f);
            GameObject axe = Instantiate(axePrefab, new Vector3(-2800, 140, 45), Quaternion.identity);
            Vector3 scale = axe.transform.localScale;
            scale.y = 80;
            scale.x = 80;
            scale.z = 80;
            axe.transform.localScale = scale;
            axe.transform.Rotate(90.0f, 0, 135.0f, Space.Self);
        }
        if (winType == WinType.horizontal)
        {
            SoundFXManage.Instance.PlaySoundFXClip(spearWin, transform, 1f);
            GameObject spear = Instantiate(spearPrefab, new Vector3(-2850, 140, 45), Quaternion.identity);
            Vector3 scale = spear.transform.localScale;
            scale.y = 50f;
            scale.x = 50f;
            scale.z = 50f;
            spear.transform.localScale = scale;
            spear.transform.Rotate(0f, 0, 0, Space.Self);
        }
        if (winType == WinType.Rightdiagonal)
        {
            SoundFXManage.Instance.PlaySoundFXClip(axeWin, transform, 1f);
            GameObject axe = Instantiate(axePrefab, new Vector3(-2800, 140, -45), Quaternion.identity);
            Vector3 scale = axe.transform.localScale;
            scale.y = 80;
            scale.x = 80;
            scale.z = 80;
            axe.transform.localScale = scale;
            axe.transform.Rotate(-90.0f, 0, 135.0f, Space.Self);
        }
        gameOver = true;
    }
    private void highlightPieces()
    {
        for (int i = 0; i < 5; i++)
        {
            if(gameBoard[winnerPieces[i].Item1, winnerPieces[i].Item2].GetComponent<Outline>())
            {
                gameBoard[winnerPieces[i].Item1, winnerPieces[i].Item2].GetComponent<Outline>().enabled = true;
            }
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
            baseSymbol = gameBoard[row, 0].GetComponent<TutPieceLogic>().player; //F: first value of every row is base
            for (int col = 0; col < 5; col++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<TutPieceLogic>().player; //F: assigned to a variable instead of callind GetComponent twice in the if
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
            baseSymbol = gameBoard[0, col].GetComponent<TutPieceLogic>().player; ;
            for (int row = 0; row < 5; row++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<TutPieceLogic>().player;
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
        baseSymbol = gameBoard[0, 0].GetComponent<TutPieceLogic>().player;
        winnerPieces.Add((0, 0));
        for (int i = 1; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, i].GetComponent<TutPieceLogic>().player;
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
        char baseSymbol = gameBoard[0, 4].GetComponent<TutPieceLogic>().player;
        bool success = true;
        for (int i = 0; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, 4 - i].GetComponent<TutPieceLogic>().player;
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
        switch (tutLvl)
        {
            case 0: if (leftDiagonalWin()) {  winType = WinType.Leftdiagonal; return true;  } break;
            case 1: if (rightDiagonalWin()) {  winType = WinType.Rightdiagonal; return true;  } break;
            case 2: if (horizontalWin()) {  winType = WinType.horizontal; return true;  } break;
            case 3: if (verticalWin()) { winType = WinType.vertical; return true;  } break;
        }
        return false;
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
                TutPieceLogic currentPieceObject = gameBoard[i - 1, chosenPiece.col].GetComponent<TutPieceLogic>();
                currentPieceObject.GetComponent<TutPieceLogic>().row = i;
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
                TutPieceLogic currentPieceObject = gameBoard[i + 1, chosenPiece.col].GetComponent<TutPieceLogic>();
                currentPieceObject.GetComponent<TutPieceLogic>().row = i;
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
                TutPieceLogic currentPieceObject = gameBoard[chosenPiece.row, i + 1].GetComponent<TutPieceLogic>();
                currentPieceObject.GetComponent<TutPieceLogic>().col = i;
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
                TutPieceLogic currentPieceObject = gameBoard[chosenPiece.row, i - 1].GetComponent<TutPieceLogic>();
                currentPieceObject.GetComponent<TutPieceLogic>().col = i;
                Vector3 newPosition = currentPieceObject.transform.position + new Vector3(0, 0, 20);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
                currentPlayerSFX();
                gameBoard[chosenPiece.row, i] = gameBoard[chosenPiece.row, i - 1];
            }
            StartCoroutine(moveChosenPiece(chosenPiece.row, 0, pieceColor, currentPiece, gameBoard[chosenPiece.row, 1].transform.position.x, 100f, -40));
        }
    }

    public System.Collections.IEnumerator MovePieceSmoothly(TutPieceLogic piece, Vector3 targetPosition)
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
        gameBoard[row, col].GetComponent<TutPieceLogic>().player = currentPiece; //F: changing the moved piece's symbol to the current
        gameBoard[row, col].GetComponent<Renderer>().material = pieceColor; //F: changing the moved piece's material (color) 
        Vector3 target = new Vector3(x, y + 15, z);
        yield return StartCoroutine(MovePieceSmoothly(gameBoard[row, col].GetComponent<TutPieceLogic>(), target));
        gameBoard[row, col].GetComponent<TutPieceLogic>().row = row; //F: changing the moved piece's row
        gameBoard[row, col].GetComponent<TutPieceLogic>().col = col; //F: changing the moved piece's col
        yield return StartCoroutine(MovePieceSmoothly(gameBoard[row, col].GetComponent<TutPieceLogic>(), new Vector3(target.x, 96f, target.z)));
        if(aiTurn && GameObject.Find("Menu Manager").GetComponent<TutPauseButton>().pauseMenu.enabled == false)
            gamePaused = false;
        chosenPiece.row = row;
        chosenPiece.col = col;
    }

    public System.Collections.IEnumerator Delay()
    {
        yield return new WaitForSeconds(6f);
        SoundFXManage.Instance.PlaySoundFXClip(victory, transform, 1f);
        winScreen.enabled = true;
    }

    public bool makeMove(char c)
    {
        if (gamePaused || (aiMoving && aiTurn))
        {
            return false;
        }
        if (validPiece(chosenPiece.row, chosenPiece.col) && moveOptions(chosenPiece.row, chosenPiece.col).Contains(c))
        {
            GameObject menu = GameObject.Find("Menu Manager");
            if (menu.GetComponent<TutPauseButton>().stepTwo.enabled == true)
            {
                menu.GetComponent<TutPauseButton>().stepTwo.enabled = false;
            }
            shiftBoard(c, currentPlayer.piece);
            tutButtonHandler.changeArrowsBack(); //F: change arrows back for every new piece selected
            usrCounter++;
            if (won())
            {
                StartCoroutine(winAnimation());
                highlightPieces();
                GameObject.Find("Menu Manager").gameObject.GetComponent<TutPauseButton>().pauseButton.gameObject.SetActive(false);
                buttonCanvas.enabled = false;
                StartCoroutine(Delay());
                
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

            aiMoving = true;
            aiTurn = true;
            switch (tutLvl)
            {
                case 0: LDiagFakeMove(); break;
                case 1: RDiagFakeMove(); break;
                case 2: horFakeMove(); break;
                case 3: verFakeMove(); break;
            }
            aiMoving = false;
            aiTurn = false;
            gamePaused = false;
            if (usrCounter > 0)
            {
                for (int row = 0; row < 5; row++)
                {
                    for (int col = 0; col < 5; col++)
                    {
                        dehighlightPeice(row, col);
                    }
                }
            }

            return true;
        }
        return false;
    }

    async void horFakeMove()
    {
        await WaitFor();
        Piece fakeAI = new Piece();
        fakeAI.row = 4;
        fakeAI.col = 0;
        (Piece, char) fakeMove = (fakeAI, 'R');
        TutPieceLogic piece = gameBoard[fakeAI.row, fakeAI.col].GetComponent<TutPieceLogic>();
        chosenPiece = piece;
        OnChosenPiece?.Invoke(fakeAI.row, fakeAI.col);
        shiftBoard(fakeMove.Item2, currentPlayer.piece);
        if (currentPlayer.piece == 'X')
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
        }
        gamePaused = false;
    }

    async void verFakeMove()
    {
        await WaitFor();
        Piece fakeAI = new Piece();
        fakeAI.row = 0;
        fakeAI.col = 4;
        (Piece, char) fakeMove = new(fakeAI, 'D');

        TutPieceLogic piece = gameBoard[fakeAI.row, fakeAI.col].GetComponent<TutPieceLogic>();
        chosenPiece = piece;
        OnChosenPiece?.Invoke(fakeAI.row, fakeAI.col);
        shiftBoard(fakeMove.Item2, currentPlayer.piece);
        if (currentPlayer.piece == 'X')
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
        }
        gamePaused = false;
    }

    async void LDiagFakeMove()
    {

        await WaitFor();
        (Piece, char) fakeMove = new();
        (Piece, char) fakeMove1 = new(new Piece(0, 4), 'D');
        (Piece, char) fakeMove2 = new(new Piece(4, 1), 'U');
        (Piece, char) fakeMove3 = new(new Piece(0, 3), 'R');
        (Piece, char) fakeMove4 = new(new Piece(0, 3), 'D');
        (Piece, char) fakeMove5 = new(new Piece(0, 2), 'D');

        switch (AIcounter)
        {
            case 0: fakeMove = fakeMove1; break;
            case 1: fakeMove = fakeMove2; break;
            case 2: fakeMove = fakeMove3; break;
            case 3: fakeMove = fakeMove4; break;
            case 4: fakeMove = fakeMove5; break;
        }
        TutPieceLogic piece = gameBoard[fakeMove.Item1.row, fakeMove.Item1.col].GetComponent<TutPieceLogic>();
        chosenPiece = piece;
        OnChosenPiece?.Invoke(fakeMove.Item1.row, fakeMove.Item1.col);
        shiftBoard(fakeMove.Item2, 'O');
        if (currentPlayer.piece == 'X')
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
        }
        gamePaused = false;
        AIcounter++;
    }

    async void RDiagFakeMove()
    {
        await WaitFor();
        Piece fakeAI = new Piece();
        (Piece, char) fakeMove = new();
        (Piece, char) fakeMove1 = new(new Piece(4, 4), 'U');
        (Piece, char) fakeMove2 = new(new Piece(1, 0), 'R');
        (Piece, char) fakeMove3 = new(new Piece(1, 0), 'R');
        (Piece, char) fakeMove4 = new(new Piece(3, 4), 'L');
        (Piece, char) fakeMove5 = new(new Piece(0, 2), 'D');

        switch (AIcounter)
        {
            case 0: fakeMove = fakeMove1; break;
            case 1: fakeMove = fakeMove2; break;
            case 2: fakeMove = fakeMove3; break;
            case 3: fakeMove = fakeMove4; break;
            case 4: fakeMove = fakeMove5; break;
        }
        TutPieceLogic piece = gameBoard[fakeMove.Item1.row, fakeMove.Item1.col].GetComponent<TutPieceLogic>();
        chosenPiece = piece;
        OnChosenPiece?.Invoke(fakeMove.Item1.row, fakeMove.Item1.col);
        shiftBoard(fakeMove.Item2, 'O');
        if (currentPlayer.piece == 'X')
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
        }
        gamePaused = false;
        AIcounter++;
    }


    private async Task WaitFor(int delay = 2000)
    {
        await Task.Delay(delay);
    }

    

    public List<char> moveOptions(int row, int col)
    {
        tutButtonHandler = GameObject.FindObjectOfType<TutButtonHandler>();
        tutButtonHandler.changeArrowsBack();
        List<char> moveList = new List<char>();
        if (currentPlayer.piece == 'X') 
        {
            List<(Piece, char)> tutMoves = allowedPieces();
            moveList.Add(tutMoves[usrCounter].Item2);
            tutButtonHandler.changeArrowColor(tutMoves[usrCounter].Item2);
        }
        else
        {
            if (row > 0)
            {
                moveList.Add('U');
                tutButtonHandler.changeArrowColor('U');
            }
            if (row < 4)
            {
                moveList.Add('D');
                tutButtonHandler.changeArrowColor('D');
            }
            if (col > 0)
            {
                moveList.Add('L');
                tutButtonHandler.changeArrowColor('L');
            }
            if (col < 4)
            {
                moveList.Add('R');
                tutButtonHandler.changeArrowColor('R');
            }
        }
        
        return moveList;
    }

    public List<(Piece, char)> allowedPieces()
    {
        List<(Piece, char)>pieces = new List<(Piece, char)>();
        if (tutLvl == 0)
        {
            pieces = new List<(Piece, char)>
            {
                (new Piece(4, 0), 'U'),
                (new Piece(4, 1), 'U'),
                (new Piece(0, 4), 'D'),
                (new Piece(0, 3), 'D'),
                (new Piece(3, 0), 'R'),
                (new Piece(3, 0), 'R'),
            };
        }
        else if (tutLvl == 1)
        {
            pieces = new List<(Piece, char)>
            {
                (new Piece(0, 0), 'D'),
                (new Piece(0, 0), 'R'),
                (new Piece(4, 4), 'U'),
                (new Piece(3, 4), 'L'),
                (new Piece(0, 2), 'D'),
                (new Piece(0, 2), 'D'),
            };
        }
        else if(tutLvl == 2)
        {
            pieces = new List<(Piece, char)>
            {
                (new Piece(0, 0), 'R'),
                (new Piece(0, 0), 'R'),
                (new Piece(0, 0), 'R'),
                (new Piece(0, 0), 'R'),
                (new Piece(0, 0), 'R'),
            };
        }
        else if(tutLvl==3)
        {
            pieces = new List<(Piece, char)>
            {
                (new Piece(4, 0), 'U'),
                (new Piece(4, 0), 'U'),
                (new Piece(4, 0), 'U'),
                (new Piece(4, 0), 'U'),
                (new Piece(4, 0), 'U'),
            };
        }
        return pieces;
    }
    //checks to see if the passed piece is a selectable piece for the player to choose
    public bool validPiece(int row, int col)
    {
        if ( gameOver)
        {
            return false;
        }
        TutPieceLogic piece = gameBoard[row, col].GetComponent<TutPieceLogic>();
        if (currentPlayer.piece == p1.piece)
        {
                List<(Piece, char)> tutMoves = allowedPieces();
                if (piece.row == tutMoves[usrCounter].Item1.row && piece.col == tutMoves[usrCounter].Item1.col)
                {
                    chosenPiece = piece;
                    OnChosenPiece?.Invoke(row, col);
                    return true;
                }
                tutButtonHandler.changeArrowsBack();
                return false;
            
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
                gameBoard[i, j].GetComponent<TutPieceLogic>().row = i;
                gameBoard[i, j].GetComponent<TutPieceLogic>().col = j;
                gameBoard[i, j].GetComponent<TutPieceLogic>().player = '-';
                gameBoard[i, j].GetComponent<TutPieceLogic>().game = this;
                z += 20;
            }
            x += 20;
        }
    }
    private void highlightPeice(int row, int col)
    {
        TutPieceLogic peice = gameBoard[row, col].GetComponent<TutPieceLogic>();
        if (peice.GetComponent<Outline>() == null)
        {
            Outline outline = peice.AddComponent<Outline>();
            outline.enabled = true;
            peice.GetComponent<Outline>().OutlineColor = Color.white;
            peice.GetComponent<Outline>().OutlineWidth = 10.0f;
        }

    }
    private void dehighlightPeice(int row, int col)
    {
        TutPieceLogic peice = gameBoard[row, col].GetComponent<TutPieceLogic>();
        if (peice.GetComponent<Outline>() != null)
        {
            peice.GetComponent<Outline>().enabled = false;
        }
    }    

    private void Update()
    {
        if (tutLvl != -1)
        {
            List<(Piece, char)> tutMoves = allowedPieces();
            if (currentPlayer.piece == 'X' && won() == false)
            {
                highlightPeice(tutMoves[usrCounter].Item1.row, tutMoves[usrCounter].Item1.col);
            }
        }
    }
}
