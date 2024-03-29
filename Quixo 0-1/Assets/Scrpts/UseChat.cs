using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class UseChat : MonoBehaviour
{
    public Button openChat;
    public Button closeChat;
    public InputField chat;
    public Image chatCanvas;

    // Event for when a new chat message is sent
    public delegate void ChatUpdated(string message);
    public static event ChatUpdated OnChatUpdated;

    void Start()
    {
        closeChat.gameObject.SetActive(false);
        chatCanvas.enabled = false;
        chat.gameObject.SetActive(false);

        chat.onEndEdit.AddListener(delegate { HandleSubmit(chat.text); });
        NetworkChat.OnNetworkChatUpdated += UpdateChat;
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

    public void HandleSubmit(string text)
    {
        // Any other input logic should go here. I.E checking for pushing enter etc.
        // Also need to clear the input field after submitting.
        if (text != "")
        {
            OnChatUpdated?.Invoke(text);
        }
    }

    public void UpdateChat(string message, PlayerRef sendingPlayerRef, PlayerRef localPlayerRef)
    {
        // @R1tz02: This function will be called on both the client and server when a new chat message is sent.
        // NetworkingManager.ChatLog contains a list of chatMessage which is an object with a string and the player that sent it.
        if (sendingPlayerRef == localPlayerRef)
        {
            // This is the local player's message
            Debug.Log("I sent this message: " + message);
        }
        else
        {
            // This is the remote player's message
            Debug.Log("They sent this message: " + message);
        }
    }
}
