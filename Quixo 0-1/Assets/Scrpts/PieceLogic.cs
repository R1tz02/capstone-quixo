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
    private static PieceLogic selectedPiece; // C: Track the currently selected piece

    private void OnMouseDown()
    {
        if (selectedPiece != this && selectedPiece != null && !game.gamePaused)
        {
            // C: If another piece is already selected, deselect it and enable gravity
            selectedPiece.isSelected = false;
            StartCoroutine(game.MovePieceSmoothly(selectedPiece, new Vector3(selectedPiece.transform.position.x, 96f, selectedPiece.transform.position.z)));
        }

        if (game.validPiece(row, col) && !game.gamePaused)
        {
            // C: Temporarily disable gravity to lift the piece
            StartCoroutine(game.MovePieceSmoothly(this, new Vector3(transform.position.x, 114f, transform.position.z)));
            isSelected = true;
            SoundFXManage.Instance.PlaySoundFXClip(game.pieceClickSound, transform, 1f);
            moveList = game.moveOptions(row, col); // C: Creates a list of directional moves based on the piece selected
            selectedPiece = this; // C: Set the current piece as the selected one
        }
        else
        {
            // C: Re-enable gravity if the piece is not valid and already selected
            if (isSelected && !game.gamePaused)
            {
                game.MovePieceSmoothly(selectedPiece, new Vector3(selectedPiece.transform.position.x, 96f, selectedPiece.transform.position.z));
            }
        }
    }

    public void SimulateOnMouseDown()
    {
        OnMouseDown();
    }
}