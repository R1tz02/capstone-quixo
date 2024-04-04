using UnityEngine;
using UnityEngine.UI;
using Fusion;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System.Collections.Generic;
using TMPro;

public class UseChat : MonoBehaviour
{
    public UnityEngine.UI.Button openChat;
    public UnityEngine.UI.Button closeChat;
    public InputField chat;
    public UnityEngine.UI.Image chatCanvas;
    public GameObject chatPanel, textObjectOne, textObjectTwo;
    public GameObject chatBox;

    List<Message> messageList = new List<Message>();

    // Event for when a new chat message is sent
    public delegate void ChatUpdated(string message);
    public static event ChatUpdated OnChatUpdated;

    void Start()
    {
        closeChat.gameObject.SetActive(false);
        chatCanvas.enabled = false;
        chat.gameObject.SetActive(false);
        chatPanel.SetActive(false);
        chatBox.SetActive(false);

        chat.onEndEdit.AddListener(delegate { HandleSubmit(chat.text); });
        NetworkChat.OnNetworkChatUpdated += UpdateChat;
    }

    public void OpenChat()
    {
        openChat.gameObject.SetActive(false);
        closeChat.gameObject.SetActive(true);
        chat.gameObject.SetActive(true);
        chatPanel.SetActive(true);
        chatCanvas.enabled = true;
        chatBox.SetActive(true);
    }

    public void CloseChat()
    {
        openChat.gameObject.SetActive(true);
        closeChat.gameObject.SetActive(false);
        chat.gameObject.SetActive(false);
        chatPanel.SetActive(false);
        chatCanvas.enabled = false;
        chatBox.SetActive(false);
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

            Message newMessage = new Message();

            newMessage.text = message;

            GameObject newText = Instantiate(textObjectOne, chatPanel.transform);

            newMessage.textObject = newText.GetComponent<TMP_Text>();

            newMessage.textObject.text = newMessage.text;

            messageList.Add(newMessage);
            
            // This is the local player's message
            Debug.Log("I sent this message: " + message);
        }
        else
        {
            Message newMessage = new Message();

            newMessage.text = message;

            GameObject newText = Instantiate(textObjectTwo, chatPanel.transform);

            newMessage.textObject = newText.GetComponent<TMP_Text>();

            newMessage.textObject.text = newMessage.text;

            messageList.Add(newMessage);
            // This is the remote player's message
            Debug.Log("They sent this message: " + message);
        }
    }
}

public class Message
{
    public string text;
    public TMP_Text textObject;
}
