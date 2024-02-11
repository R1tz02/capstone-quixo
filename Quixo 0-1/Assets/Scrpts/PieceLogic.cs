using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceLogic : MonoBehaviour
{
    public int row;
    public int col;
    public char player;
    public GameCore game;

    private bool isSelected = false;
    private List<char> moveList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        if (game.validPiece(row, col))
        {
            
            
            
            game.lowerPiece(); // Calls the function from the GameCore class to lower all raised pieces
>>>>>>> Stashed changes
>>>>>>> Stashed changes
            isSelected = true;

            moveList = game.moveOptions(row, col); // Creates a list of directional moves based on the piece selected
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
