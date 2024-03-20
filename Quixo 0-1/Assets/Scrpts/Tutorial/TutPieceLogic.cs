using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutPieceLogic : MonoBehaviour
{
    public int row;
    public int col;
    public char player;
    public TutGameCore game;
    private bool isSelected = false;
    // private Rigidbody rb;
    private List<char> moveList;

    // Start is called before the first frame update
    void Start()
    {

    }

    private static TutPieceLogic selectedPiece; // C: Track the currently selected piece

    private void OnMouseDown()
    {
        if (selectedPiece != this && selectedPiece != null)
        {
            // C: If another piece is already selected, deselect it and enable gravity
            selectedPiece.isSelected = false;
            StartCoroutine(game.MovePieceSmoothly(selectedPiece, new Vector3(selectedPiece.transform.position.x, 96f, selectedPiece.transform.position.z)));
        }

        if (game.validPiece(row, col))
        {
            // C: Temporarily disable gravity to lift the piece
            StartCoroutine(game.MovePieceSmoothly(this, new Vector3(transform.position.x, 114f, transform.position.z)));
            isSelected = true;
            moveList = game.moveOptions(row, col); // C: Creates a list of directional moves based on the piece selected
            selectedPiece = this; // C: Set the current piece as the selected one
        }
        else
        {
            // C: Re-enable gravity if the piece is not valid and already selected
            if (isSelected)
            {
                game.MovePieceSmoothly(selectedPiece, new Vector3(selectedPiece.transform.position.x, 96f, selectedPiece.transform.position.z));
              }
        }
    }

    public void SimulateOnMouseDown(){
        OnMouseDown();
    }




    // Update is called once per frame
    void Update()
    {

    }
}