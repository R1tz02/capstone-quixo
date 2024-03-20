using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.Contracts;

public class ChatMenu : MonoBehaviour
{
    public Button openButton;
    public Button closeButton;
    public Image chatBackground;
    public InputField chatText;

    // Event for when a new chat message is sent
    public delegate void ChatUpdated(string message);
    public static event ChatUpdated OnChatUpdated;

    private void Start()
    {
        closeButton.gameObject.SetActive(false);
        chatBackground.enabled = false;
        chatText.gameObject.SetActive(false);
        
        chatText.onEndEdit.AddListener(delegate { HandleSubmit(chatText.text); });
        NetworkChat.OnNetworkChatUpdated += UpdateChat;
    }

    public void OpenChat()
    {
        closeButton.gameObject.SetActive(true);
        chatBackground.enabled = true;
        chatText.gameObject.SetActive(true);
        openButton.gameObject.SetActive(false);
    }

    public void ClosedChat()
    {
        closeButton.gameObject.SetActive(false);
        chatBackground.enabled = false;
        chatText.gameObject.SetActive(false);  
        openButton.gameObject.SetActive(true);
    }

    public void HandleSubmit(string text)
    {
        // Any other input logic should go here. I.E checking for pushing enter etc.
        if (text != "")
        {
            OnChatUpdated.Invoke(text);
        }
    }

    public void UpdateChat(NetworkChat.ChatMessage chatMessage)
    {
        // This is where we would update the chat log
        // ChatMessage is an object with a string and the player that sent it.
        Debug.Log(chatMessage.message);
        Debug.Log(chatMessage.playerRef);
    }
}
