using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Collections;
using System;

public class NetworkedPlayer : NetworkBehaviour, IPlayer
{
    [Networked]
    public string _piece { get; set; }
    [Networked]
    public bool won { get; set; } = false;
    [Networked]
    public PlayerRef PlayerRef { get; set; }
    public static int TotalPlayers = 0;

    private NetworkingManager networkingManager;

    public char piece
    {
        get { return _piece[0]; }
        set { _piece = value.ToString(); }
    }

    public void Start()
    {
        networkingManager = GameObject.Find("NetworkManager").GetComponent<NetworkingManager>();

        TotalPlayers++;

        // Potentially a race condition where the client doesn't have the networked players created yet
        // Server will wait until the client's networked players are both created
        // Client calls Start when the networked player is created
        if (networkingManager._runner.IsClient && TotalPlayers == 2)
        {
            Debug.Log("Client is good");
            RpcSendClientConfirmation();
        }
    }

    public void Initialize(char playerSymbol, PlayerRef playerRef)
    {
        piece = playerSymbol;
        this.PlayerRef = playerRef;
    }

    public void Initialize(char playerSymbol)
    {
        throw new NotImplementedException("This method should not be called for a Networked Player");
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateGameState(string serializedState)
    {
        networkingManager.gameState = GameState.Deserialize(serializedState);

        networkingManager.UpdateGameState();

        Debug.Log("Updated game state");
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcAssignPlayers(PlayerRef p1Ref, PlayerRef p2Ref)
    {
        // Get all NetworkedPlayer instances in the scene
        NetworkedPlayer[] allPlayers = GameObject.FindObjectsOfType<NetworkedPlayer>();

        int playerIndex = 0;

        foreach (NetworkedPlayer player in allPlayers)
        {
            if (player.PlayerRef == p1Ref)
            {
                networkingManager.game.p1 = player;
            }
            else if (player.PlayerRef == p2Ref)
            {
                networkingManager.game.p2 = player;
            }
            else
            {
                Debug.Log("PlayerRef: " + player.PlayerRef + " does not match either p1Ref: " + p1Ref + " or p2Ref: " + p2Ref);
                throw new System.Exception("PlayerRef does not match either p1Ref or p2Ref");
            }

            networkingManager._players[playerIndex] = new KeyValuePair<PlayerRef, NetworkedPlayer>(player.PlayerRef, player);
            playerIndex++;
        }

        networkingManager.game.currentPlayer = networkingManager.game.p1;
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendMove(byte direction)
    {
        networkingManager.game.makeMove((char)direction);
    }

    // Called by server
    public static IEnumerator WaitForClientConfirmation(Action OnComplete)
    {
        while (!NetworkingManager.GameSetUp)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Client says game is set up!");
        OnComplete();
    }

    // Called by client
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendClientConfirmation()
    {
        NetworkingManager.GameSetUp = true;
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetChosenPiece(int row, int col)
    {
        networkingManager.game.chosenPiece = networkingManager.game.gameBoard[row, col].GetComponent<PieceLogic>();
    }
}