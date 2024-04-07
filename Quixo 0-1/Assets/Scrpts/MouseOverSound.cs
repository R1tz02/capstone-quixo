using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class MouseOverSound : MonoBehaviour, IPointerEnterHandler
{

    public GameCore game;
    [SerializeField] public AudioClip menuHoverSound;
    [SerializeField] public AudioClip menuClickSound;

    public void Start()
    {
        game = FindObjectOfType<GameCore>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered restart  button");
        SoundFXManage.Instance.PlaySoundFXClip(menuHoverSound, transform, 1f);

        // Add your custom actions here
    }
    /*public bool mouseOverBtn = false;
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
    }*/
}
