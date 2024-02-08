using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceLogic : MonoBehaviour
{
    //F: Gives each spawned piece characteristics that we can use low-level
    public int row;
    public int col;
    public char player;
    public GameCore game;
    private ButtonHandler buttonHandler;

    private bool isSelected = false;
    private Rigidbody rb;
    private List<char> moveList;
  
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnMouseDown()
    {
        if (game.validPiece(row, col))
        {
            GetComponent<Rigidbody>().useGravity = false; //F: lifts the piece for a second
            transform.position = new Vector3(transform.position.x, 15f, transform.position.z); // raises the selected piece
            isSelected = true;

            moveList = game.moveOptions(row, col); // Creates a list of directional moves based on the piece selected
            GetComponent<Rigidbody>().useGravity = true; //F: drops the piece back down, giving it the falling effect
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
