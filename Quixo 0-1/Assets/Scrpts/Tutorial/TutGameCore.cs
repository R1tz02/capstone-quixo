using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class Player
{
    public char piece;
    public bool won;
}

public class TutGameCore : MonoBehaviour
{
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
    public int usrCounter = 0;


    void Start()
    {
        p1.piece = 'X';
        p2.piece = 'O';

        currentPlayer = p1;
        winScreen.enabled = false;
        helpMenu.enabled = false;
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
            baseSymbol = gameBoard[0, col].GetComponent<TutPieceLogic>().player; ;
            for (int row = 0; row < 5; row++)
            {
                pieceToCheck = gameBoard[row, col].GetComponent<TutPieceLogic>().player;
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
        baseSymbol = gameBoard[0, 0].GetComponent<TutPieceLogic>().player; ;
        for (int i = 1; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, i].GetComponent<TutPieceLogic>().player;
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
        char baseSymbol = gameBoard[0, 4].GetComponent<TutPieceLogic>().player;
        bool success = true;
        for (int i = 0; i < 5; i++)
        {
            pieceToCheck = gameBoard[i, 4 - i].GetComponent<TutPieceLogic>().player;
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
        switch (tutLvl)
        {
            case 0: if (leftDiagonalWin()) {  return true; } break;
            case 1: if (rightDiagonalWin()) {  return true; } break;
            case 2: if (horizontalWin()) {  return true; } break;
            case 3: if (verticalWin()) {  return true; } break;
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
                TutPieceLogic currentPieceObject = gameBoard[i - 1, chosenPiece.col].GetComponent<TutPieceLogic>();
                currentPieceObject.GetComponent<TutPieceLogic>().row = i;
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
                TutPieceLogic currentPieceObject = gameBoard[i + 1, chosenPiece.col].GetComponent<TutPieceLogic>();
                currentPieceObject.GetComponent<TutPieceLogic>().row = i;
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
                TutPieceLogic currentPieceObject = gameBoard[chosenPiece.row, i + 1].GetComponent<TutPieceLogic>();
                currentPieceObject.GetComponent<TutPieceLogic>().col = i;
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
                TutPieceLogic currentPieceObject = gameBoard[chosenPiece.row, i - 1].GetComponent<TutPieceLogic>();
                currentPieceObject.GetComponent<TutPieceLogic>().col = i;
                Vector3 newPosition = currentPieceObject.transform.position + new Vector3(0, 0, 20);
                StartCoroutine(MovePieceSmoothly(currentPieceObject, newPosition));
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
                GameObject.Find("Menu Manager").gameObject.GetComponent<TutPauseButton>().pauseButton.gameObject.SetActive(false);
                WaitFor(3000);
                winScreen.enabled = true;
                Time.timeScale = 0;
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

            switch (tutLvl)
            {
                case 0: LDiagFakeMove(); break;
                case 1: RDiagFakeMove(); break;
                case 2: horFakeMove(); break;
                case 3: verFakeMove(); break;
            }

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
        validPiece(fakeMove.Item1.row, fakeMove.Item1.col);
        shiftBoard(fakeMove.Item2, currentPlayer.piece);

        if (currentPlayer.piece == 'X')
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
        }
    }

    async void verFakeMove()
    {
        await WaitFor();
        Piece fakeAI = new Piece();
        fakeAI.row = 0;
        fakeAI.col = 4;
        (Piece, char) fakeMove = new(fakeAI, 'D');
        validPiece(fakeMove.Item1.row, fakeMove.Item1.col);
        shiftBoard(fakeMove.Item2, currentPlayer.piece);

        if (currentPlayer.piece == 'X')
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
        }
    }

    async void LDiagFakeMove()
    {
        await WaitFor();
        Piece fakeAI = new Piece();
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

        validPiece(fakeMove.Item1.row, fakeMove.Item1.col);
        shiftBoard(fakeMove.Item2, currentPlayer.piece);

        if (currentPlayer.piece == 'X')
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
        }
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

        validPiece(fakeMove.Item1.row, fakeMove.Item1.col);
        shiftBoard(fakeMove.Item2, currentPlayer.piece);

        if (currentPlayer.piece == 'X')
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
        }
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
        if (gamePaused)
        {
            return false;
        }
        TutPieceLogic piece = gameBoard[row, col].GetComponent<TutPieceLogic>();
        if (currentPlayer.piece == p1.piece)
        {
            List<(Piece, char)> tutMoves = allowedPieces();
            if(piece.row == tutMoves[usrCounter].Item1.row && piece.col == tutMoves[usrCounter].Item1.col)
            {
                chosenPiece = piece;
                OnChosenPiece?.Invoke(row, col);
                return true;
            }
            tutButtonHandler.changeArrowsBack();
            return false;
        }
        else
        {
            if ((row == 0 || row == 4) || (col == 0 || col == 4))
            {
                if (piece.player == '-' || currentPlayer.piece == piece.player)
                {
                    chosenPiece = piece;

                    OnChosenPiece?.Invoke(row, col);

                    return true;
                }
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
