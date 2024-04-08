using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] public AudioClip menuHoverSound;
    [SerializeField] public AudioClip menuClickSound;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered restart button");
        SoundFXManage.Instance.PlaySoundFXClip(menuHoverSound, transform, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse clicked restart button");
        SoundFXManage.Instance.PlaySoundFXClip(menuClickSound, transform, 1f);
    }
}
