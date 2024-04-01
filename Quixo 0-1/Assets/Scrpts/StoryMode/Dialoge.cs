using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialoge : MonoBehaviour
{
    public Canvas dialogeBox;
    public Canvas exampleWin;


    private void Start()
    {
        dialogeBox.enabled = true;
        exampleWin.enabled = false;
    }

    public void closeTextBox()
    { 
        dialogeBox.enabled = false;
        exampleWin.enabled = true;
    }

    public void closeExample() 
    { 
        exampleWin.enabled = false;
        Time.timeScale = 1;
        GameObject.Find("GameMaster").GetComponent<StoryGameCore>().gamePaused = false;
        GameObject.Find("Menu Manager").GetComponent<StoryPauseButton>().pauseButton.gameObject.SetActive(true);
    }
}
