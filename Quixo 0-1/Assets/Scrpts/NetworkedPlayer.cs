using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;

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

        pauseButton = GameObject.Find("Menu Manager").GetComponent<PauseButton>();

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

        if (playerTurnLocal == 2)
        {
            networkingManager.game.currentPlayer = networkingManager.game.p2;
            if (Data.CURRENT_LANGUAGE == "English")
            {
                networkingManager.playerIndicator.text = "Player 2's turn";
            }
            else if (Data.CURRENT_LANGUAGE == "Español")
            {
                networkingManager.playerIndicator.text = "Turno del jugador 2";
            }
        }
        else
        {
            networkingManager.game.currentPlayer = networkingManager.game.p1;
            networkingManager.playerIndicator.text = "Player 1's turn";
        }
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendMove(byte direction, PlayerRef sendingPlayerRef)
    {
        NetworkedPlayer localPlayer = networkingManager.GetNetworkedPlayer(networkingManager._runner.LocalPlayer);

        if (networkingManager.playerIndicator.text == "Player 1's turn" || networkingManager.playerIndicator.text == "Turno del jugador 1")
        {
           if (Data.CURRENT_LANGUAGE == "English")
            {
                networkingManager.playerIndicator.text = "Player 2's turn";
            }
            else if (Data.CURRENT_LANGUAGE == "Español")
            {
                networkingManager.playerIndicator.text = "Turno del jugador 2";
            }
        }
        else
        {
            if (Data.CURRENT_LANGUAGE == "English")
            {
                networkingManager.playerIndicator.text = "Player 1's turn";
            }
            else if (Data.CURRENT_LANGUAGE == "Español")
            {
                networkingManager.playerIndicator.text = "Turno del jugador 1";
            }
        }

        if (localPlayer.PlayerRef == sendingPlayerRef) return;

        networkingManager.game.makeMove((char)direction, true);
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

        if (Data.CURRENT_LANGUAGE == "English")
        {
            GameObject.Find("playAgainTxt").gameObject.GetComponent<TMP_Text>().text = "Restart (1/2)";
            GameObject.Find("tiePlayAgainTxt").gameObject.GetComponent<TMP_Text>().text = "Restart (1/2)";
        }
        else if (Data.CURRENT_LANGUAGE == "Espa�ol")
        {
            GameObject.Find("tiePlayAgainTxt").gameObject.GetComponent<TMP_Text>().text = "Reiniciar (1/2)";
            GameObject.Find("playAgainTxt").gameObject.GetComponent<TMP_Text>().text = "Reiniciar (1/2)";
        }

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

        // Set var in networkingManager to test if the player is waiting on a draw - if so and the other player leaves, disconnect
        networkingManager.drawInProgress = true;

        if (callingPlayer != localPlayerRef)
        {
            pauseButton.requestDraw(true);
        }
        else
        {
            // Dialog box to tell user that they are waiting on the other player to accept or deny the draw

            networkingManager.game.showError("Waiting on other player to accept or deny draw...");
            
            HideButtons();
        }
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcAcceptDraw()
    {
        pauseButton.acceptDraw(true);

        networkingManager.PlayerIndicatorCanvas.gameObject.SetActive(false);

        networkingManager.drawInProgress = false;

        networkingManager.game.closeError();

        if (networkingManager._runner.IsServer)
        {
            networkingManager._runner.SessionInfo.IsOpen = false;
        }
    }

    // Called by both client and server
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcDenyDraw()
    {
        networkingManager.drawInProgress = false;

        pauseButton.denyDraw(true);
        
        networkingManager.game.closeError();

        networkingManager.pauseButton.gameObject.SetActive(false);
    }

    private void HideButtons()
    {
        networkingManager.pauseButton.gameObject.SetActive(false);
        networkingManager.game.buttonsCanvas.enabled = false;
        networkingManager.game.drawButton.gameObject.SetActive(false);
    }
}