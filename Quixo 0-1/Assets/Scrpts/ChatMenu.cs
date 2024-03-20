using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.Contracts;
using UnityEditor.UI;

public class ChatMenu : MonoBehaviour
{
    public Button openButton;
    public Button closeButton;
    public Image chatBackground;
    public InputField chatText;

    private void Start()
    {
        closeButton.gameObject.SetActive(false);
        chatBackground.enabled = false;
        chatText.gameObject.SetActive(false);
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
}
