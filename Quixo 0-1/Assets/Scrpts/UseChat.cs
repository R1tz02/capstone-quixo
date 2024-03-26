using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UseChat : MonoBehaviour
{
    public Button openChat;
    public Button closeChat;
    public UnityEngine.UI.InputField chat;
    public Image chatCanvas;
    // Start is called before the first frame update
    void Start()
    {
        closeChat.gameObject.SetActive(false);
        chatCanvas.enabled = false;
        chat.gameObject.SetActive(false);
    }

    public void OpenChat()
    {
        openChat.gameObject.SetActive(false);
        closeChat.gameObject.SetActive(true);
        chat.gameObject.SetActive(true);
        chatCanvas.enabled = true;
    }

    public void CloseChat()
    {
        openChat.gameObject.SetActive(true);
        closeChat.gameObject.SetActive(false);
        chat.gameObject.SetActive(false);
        chatCanvas.enabled = false;
    }
}
