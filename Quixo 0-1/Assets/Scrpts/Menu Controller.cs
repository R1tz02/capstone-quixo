using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void NewEasyGame() 
    {
        SceneManager.LoadScene(1);
    }
}
