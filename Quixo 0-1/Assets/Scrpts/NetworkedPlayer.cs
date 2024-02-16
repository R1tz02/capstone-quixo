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
    private static int TotalPlayers = 0;

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

        if (networkingManager._runner.IsClient && TotalPlayers == 2)
        {
            Debug.Log("Client is good");
            RpcSendClientConfirmation();
        }
    }

    public void Initialize(char playerSymbol)
    {
        throw new System.NotImplementedException("This method is not possible for a NetworkedPlayer. Use other Initialize method instead.");
    }

    public void Initialize(char playerSymbol, PlayerRef playerRef)
    {
        piece = playerSymbol;
        this.PlayerRef = playerRef;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateGameState(string serializedState)
    {
        Debug.Log("Updating game state...");

        networkingManager.gameState = GameState.Deserialize(serializedState);

        Debug.Log("Deserialized game state" + networkingManager.gameState.piecesData.Length + " pieces.");

        networkingManager.UpdateGameState();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Sending message...");
            RPC_SendMessage("Hey Mate!");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage(string message, RpcInfo rpcInfo = default)
    {
        Debug.Log("Relaying message: " + message);
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcAssignPlayers(PlayerRef p1Ref, PlayerRef p2Ref)
    {
        // Get all NetworkedPlayer instances in the scene
        NetworkedPlayer[] allPlayers = GameObject.FindObjectsOfType<NetworkedPlayer>();

        int playerIndex = 0;

        foreach (NetworkedPlayer player in allPlayers)
        {
            Debug.Log("Player is valid: " + player.PlayerRef);
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
        Debug.Log("Assigned Current Player: " + networkingManager.game.currentPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendMove(byte direction)
    {
        networkingManager.game.makeMove((char)direction);
    }

    public static IEnumerator WaitForClientConfirmation(Action OnComplete)
    {
        while (!NetworkingManager.GameSetUp)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("Client says GTG");
        OnComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendClientConfirmation()
    {
        NetworkingManager.GameSetUp = true;
    }
}