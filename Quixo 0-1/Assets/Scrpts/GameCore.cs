using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameCore : MonoBehaviour
{ 
    public GameObject piecePrefab;
    public Material playerOneSpace;
    public Material playerTwoSpace;

    private PieceLogic pieceLogic;
    private GameObject[,] gameBoard = new GameObject[5, 5];
    private bool playerOneTurn = true;

    // Start is called before the first frame update
    void Start()
    {
        populateBoard();
    }

    //Returns a list of Directions a piece can move based on the locations of the passes piece
    public List<char> moveOptions(int row, int col)
    {
        List<char> moveList = new List<char>();
        if (row > 0)
        {
            moveList.Add('U');
        }
        if (row < 4)
        {
            moveList.Add('D');
        }
        if (col > 0)
        {
            moveList.Add('L');
        }
        if (col < 4)
        {
            moveList.Add('R');
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
}
