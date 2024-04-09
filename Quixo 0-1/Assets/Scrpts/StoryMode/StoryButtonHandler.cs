using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//F: This script controls the arrows that display the options for the moves

public class StoryButtonHandler : MonoBehaviour
{
    public Button left;
    public Button right;
    public Button up;
    public Button down;
    public StoryGameCore game;
    public Color gold = new Color(1f, 0.843f, 0f);

    // Create Event so that we can add a listener to any other class that wants to know when a move was made
    public delegate void MoveMade(char direction);
    public static event MoveMade OnMoveMade;

    void Start()
    {
        game = GameObject.FindObjectOfType<StoryGameCore>();
        up.onClick.AddListener(delegate { doOnClick('U'); });   //F: Give each arrow an on click event listener that calls doOnClick and we pass it a char
        down.onClick.AddListener(delegate { doOnClick('D'); }); // representing what arrow was clicked
        left.onClick.AddListener(delegate { doOnClick('L'); });
        right.onClick.AddListener(delegate { doOnClick('R'); });
    }

    private void doOnClick(char dir)
    {
        bool success = game.makeMove(dir);

        if (success) {
            OnMoveMade?.Invoke(dir); // Call the event to let other classes know that a move was made
        }
    }

    public void changeArrowsBack() //F: we change the arrows back to white
    {
        left.GetComponent<Image>().color = Color.white;
        right.GetComponent<Image>().color = Color.white;
        up.GetComponent<Image>().color = Color.white;
        down.GetComponent<Image>().color = Color.white;
    }

    public void changeArrowColor(char dir)
    {
        switch (dir) //F: Change corresponding arrows depending on the available move options
        {
            case 'U':
                up.GetComponent<Image>().color = gold;
                break;
            case 'D':
                down.GetComponent<Image>().color = gold;
                break;
            case 'L':
                left.GetComponent<Image>().color = gold;
                break;
            case 'R':
                right.GetComponent<Image>().color = gold;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) { doOnClick('U'); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { doOnClick('D'); }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { doOnClick('L'); }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { doOnClick('R'); }
    }
}