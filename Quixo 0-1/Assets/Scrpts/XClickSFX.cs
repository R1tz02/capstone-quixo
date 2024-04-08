using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class XClickSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] public AudioClip xHover;
    [SerializeField] public AudioClip xClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundFXManage.Instance.PlaySoundFXClip(xHover, transform, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundFXManage.Instance.PlaySoundFXClip(xClick, transform, 1f);
    }
}
