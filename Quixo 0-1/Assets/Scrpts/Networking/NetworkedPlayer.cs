using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkedPlayer : NetworkBehaviour
{
    public NetworkProperty<char> Piece = new NetworkProperty<char>();
    public NetworkProperty<bool> Won = new NetworkProperty<bool>();

    // Create a new local player and assign the player symbol based on playerChar
    // @param playerChar[Char] - the player symbol to assign to the player
    public NetworkedPlayer(char playerSymbol)
    {
        if (playerSymbol == 'X')
        {
            this.Piece = 'X';
        }
        else
        {
            this.Piece = 'O';
        }
    }
}