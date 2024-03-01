using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PieceLogic : MonoBehaviour
{
    public int row;
    public int col;
    public char player;
    public GameCore game;
    private ButtonHandler buttonHandler;

    private bool isSelected = false;
    // private Rigidbody rb;
    private List<char> moveList;

    // Start is called before the first frame update
    void Start()
    {

    }

    /* private void OnMouseDown()
     {
         if (game.validPiece(row, col))
         {
             GetComponent<Rigidbody>().useGravity = false; //F: lifts the piece for a second
             transform.position = new Vector3(transform.position.x, 114f, transform.position.z); // raises the selected piece
             isSelected = true;
             moveList = game.moveOptions(row, col); // Creates a list of directional moves based on the piece selected
             GetComponent<Rigidbody>().useGravity = true; //F: drops the piece back down, giving it the falling effect
         }
     }*/
    private static PieceLogic selectedPiece; // C: Track the currently selected piece

    private void OnMouseDown()
    {
        if (selectedPiece != null && selectedPiece != this)
        {
            // C: If another piece is already selected, deselect it and enable gravity
            selectedPiece.isSelected = false;
            //selectedPiece.GetComponent<Rigidbody>().useGravity = true;
            //game.MovePieceSmoothly(selectedPiece, new Vector3(selectedPiece.transform.position.x, 96f, selectedPiece.transform.position.z));
            selectedPiece.transform.position = Vector3.Lerp(selectedPiece.transform.position, new Vector3(selectedPiece.transform.position.x, 96f, selectedPiece.transform.position.z), 1);
        }

        if (game.validPiece(row, col))
        {
            // C: Temporarily disable gravity to lift the piece
            //GetComponent<Rigidbody>().useGravity = false;
            transform.position = new Vector3(transform.position.x, 114f, transform.position.z); // C: raises the selected piece
            isSelected = true;
            moveList = game.moveOptions(row, col); // C: Creates a list of directional moves based on the piece selected
            selectedPiece = this; // C: Set the current piece as the selected one
        }
        else
        {
            // C: Re-enable gravity if the piece is not valid and already selected
            if (isSelected)
            {
                selectedPiece.transform.position = Vector3.Lerp(selectedPiece.transform.position, new Vector3(selectedPiece.transform.position.x, 96f, selectedPiece.transform.position.z), 1);
                //GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }




    // Update is called once per frame
    void Update()
    {

    }
}