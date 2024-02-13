using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public interface IPlayer
{
    char piece { get; set; }
    bool won { get; set; }
    void Initialize(char playerSymbol);
    void Initialize(PlayerRef playerRef, char playerSymbol);
}


public class LocalPlayer : MonoBehaviour, IPlayer
{
    public char piece { get; set; }
    public bool won { get; set; } = false;

    // Create a new local player and assign the player symbol based on playerChar
    // @param playerChar[Char] - the player symbol to assign to the player
    public void Initialize(char playerSymbol)
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

    public void Initialize(PlayerRef playerRef, char playerSymbol)
    {
        throw new System.NotImplementedException("LocalPlayer does not support network initialization.");
    }
}

public class NetworkedPlayer : NetworkBehaviour, IPlayer
{
    public string _piece { get; set; }
    [Networked]
    public bool won { get; set; } = false;

    //[Networked]
    public string playerId { get; set; }
    public PlayerRef playerRef { get; set; }

    public char piece
    {
        get { return _piece[0]; }
        set { _piece = value.ToString(); }
    }

    // Create a new networked player and assign the player symbol based on playerChar
    // @param playerChar[Char] - the player symbol to assign to the player
    public void Initialize(PlayerRef playerRef, char playerSymbol)
    {
        this.playerRef = playerRef;
        playerId = playerRef.PlayerId.ToString();
        piece = playerSymbol;
    }

    public void Initialize(char playerSymbol)
    {
        throw new System.NotImplementedException("NetworkedPlayer does not support local initialization.");
    }
}
