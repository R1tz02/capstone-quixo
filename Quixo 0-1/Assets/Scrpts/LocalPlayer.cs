using UnityEngine;

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
}
