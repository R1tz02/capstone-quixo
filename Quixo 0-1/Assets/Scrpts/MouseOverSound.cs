using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseOverSound : MonoBehaviour
{
    public bool mouseOverBtn = false;
    public GameCore game;

    public void Start()
    {
        game = GameObject.FindObjectOfType<GameCore>();
    }

    public void OnMouseOver()
    {
        Debug.Log("hovered over");
        if (!mouseOverBtn)
        {
            SoundFXManage.Instance.PlaySoundFXClip(game.pieceClickSound, transform, 1f);
            mouseOverBtn = true;
        }
    }

    public void OnMouseExit()
    {
        mouseOverBtn = false;
    }
}
