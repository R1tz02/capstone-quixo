using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public char piece;
    public bool won = false;

    // Create a new local player and assign the player symbol based on playerChar
    // @param playerChar[Char] - the player symbol to assign to the player
    public Player (char playerSymbol)
    {
        if (playerSymbol == 'X')
        {
            this.piece = 'X';
        }
        else
        {
            this.piece = 'O';
        }
    }
}
