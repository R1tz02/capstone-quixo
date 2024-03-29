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

    public static Dictionary<PlayerRef, bool> RematchDict = new();

    private NetworkingManager networkingManager;

    private PauseButton pauseButton;

    public char piece
    {
        get { return _piece[0]; }
        set { _piece = value.ToString(); }
    }

    public void Start()
    {
        networkingManager = GameObject.Find("NetworkManager").GetComponent<NetworkingManager>();

        pauseButton = GameObject.FindObjectOfType<PauseButton>();

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
    // Pass in current player piece to set the current player turn if a game was already in progress (i.e the client disconnected and is back now)
    // Needs to be passed in as a byte because the RPC can't take a char
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcAssignPlayers(PlayerRef p1Ref, PlayerRef p2Ref, int playerTurnLocal)
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

        networkingManager.chat = GameObject.FindObjectOfType<NetworkChat>();

        networkingManager.game.currentPlayer = playerTurnLocal == 2 ? networkingManager.game.p2 : networkingManager.game.p1;
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendMove(byte direction)
    {
        NetworkedPlayer localPlayer = networkingManager.GetNetworkedPlayer(networkingManager._runner.LocalPlayer);

        if (localPlayer.piece != networkingManager.game.currentPlayer.piece)
        {
            networkingManager.game.makeMove((char)direction);
        }
    }

    // Called by server
    public static IEnumerator WaitForClientConfirmation(Action OnComplete)
    {
        NetworkingManager.GameSetUp = false;

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

    // Called by both client and server
    public void Rematch()
    {
        PlayerRef localPlayerRef = networkingManager.GetNetworkedPlayer(networkingManager._runner.LocalPlayer).PlayerRef;

        // Prevent one player from spamming the play again button
        if (RematchDict.ContainsKey(localPlayerRef) && RematchDict[localPlayerRef]) return;

        RpcUpdatePlayAgainCount(localPlayerRef);
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdatePlayAgainCount(PlayerRef wantsToPlayAgainRef)
    {
        RematchDict[wantsToPlayAgainRef] = true;

        // TODO #35: Change the GUI text to reflect the number of players who want to play again

        if (RematchDict.Count == 2 && networkingManager._runner.IsServer)
        {
            RpcResetGame();
        }
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcResetGame()
    {
        RematchDict.Clear();
        networkingManager.ResetGame();
    }

    // This is injected into the normal draw flow so that the result is a networked draw function
    // As this is an RPC, it will be called on both machines. The local player will stop execution and the other player will continue the draw flow
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcOfferDraw(PlayerRef callingPlayer)
    {
        PlayerRef localPlayerRef = networkingManager.GetNetworkedPlayer(networkingManager._runner.LocalPlayer).PlayerRef;

        if (callingPlayer != localPlayerRef)
        {
            pauseButton.requestDraw(true);
        }
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcAcceptDraw()
    {
        pauseButton.acceptDraw(true);
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcDenyDraw()
    {
        pauseButton.denyDraw(true);
    }
}