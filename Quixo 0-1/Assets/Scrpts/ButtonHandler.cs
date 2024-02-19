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


    void Start()
    {
        game = GameObject.FindObjectOfType<GameCore>();
<<<<<<< Updated upstream
        up.onClick.AddListener(delegate { doOnClick('u'); });   //F: Give each arrow an on click event listener that calls doOnClick and we pass it a char
        down.onClick.AddListener(delegate { doOnClick('d'); }); // representing what arrow was clicked
        left.onClick.AddListener(delegate { doOnClick('l'); }); 
        right.onClick.AddListener(delegate { doOnClick('r'); });
=======
        up.onClick.AddListener(delegate { doOnClick('U'); });   //F: Give each arrow an on click event listener that calls doOnClick and we pass it a char
        down.onClick.AddListener(delegate { doOnClick('D'); }); // representing what arrow was clicked
        left.onClick.AddListener(delegate { doOnClick('L'); });
        right.onClick.AddListener(delegate { doOnClick('R'); });
>>>>>>> Stashed changes
    }


    private void doOnClick(char dir)
    {
        game.makeMove(dir); //F: make a move
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
            case 'U': up.GetComponent<Image>().color = Color.red; 
                break;
            case 'D':
                down.GetComponent<Image>().color = Color.red;
                break;
            case 'L':
                left.GetComponent<Image>().color = Color.red;
                break;
            case 'R':
                right.GetComponent<Image>().color = Color.red;
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
