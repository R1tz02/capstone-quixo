using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//F: This script controls the arrows that display the options for the moves

public class ButtonHandler : MonoBehaviour
{
    public Button left;
    public GameObject what;
    public Button right;
    public Button up;
    public Button down;
    public GameCore game;

    // Create Event so that we can add a listener to any other class that wants to know when a move was made
    public delegate void MoveMade(char direction);
    public static event MoveMade OnMoveMade;

    void Start()
    {
        game = GameObject.FindObjectOfType<GameCore>();
        up.onClick.AddListener(delegate { doOnClick('u'); }); //F: Give each arrow an on click event listener that calls doOnClick and we pass it a char
        down.onClick.AddListener(delegate { doOnClick('d'); }); // representing what arrow was clicked
        left.onClick.AddListener(delegate { doOnClick('l'); });
        right.onClick.AddListener(delegate { doOnClick('r'); });
    }


    private void doOnClick(char c)
    {
        game.makeMove(c); //F: make a move
        OnMoveMade?.Invoke(c); // Call the event to let other classes know that a move was made
    }

    public void changeArrowsBack() //F: we change the arrows back to white
    {
        left.GetComponent<Image>().color = Color.white;
        right.GetComponent<Image>().color = Color.white;
        up.GetComponent<Image>().color = Color.white;
        down.GetComponent<Image>().color = Color.white;
    }

    public void changeLeftArrowColor() //F: Change corresponding arrows depending on the available move options
    {
        Debug.Log("changed color of left");
        left.GetComponent<Image>().color = Color.red;
    }

    public void changeRightArrowColor()
    {
        Debug.Log("changed color of right");
        right.GetComponent<Image>().color = Color.red;
    }

    public void changeUpArrowColor()
    {
        Debug.Log("changed color of up");
        up.GetComponent<Image>().color = Color.red;
    }

    public void changeDownArrowColor()
    {
        Debug.Log("changed color of down");
        down.GetComponent<Image>().color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
