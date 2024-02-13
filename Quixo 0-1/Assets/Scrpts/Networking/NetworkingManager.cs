using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;


[Serializable]
public class PieceData
{
    public int row;
    public int col;
    public char player;
}

[Serializable]
public class GameState
{
    public PieceData[,] pieces;
}

public class NetworkingManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player joined: " + player);
        if (runner.IsServer)
        {
            SyncBoard();
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    private NetworkRunner _runner;
    public GameCore game;
    public GameState gameState;

    public void Start()
    {
        ButtonHandler.OnMoveMade += RPCSendMove;
    }

    public async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = GameObject.Find("NetworkManager").AddComponent<NetworkRunner>();

        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,

            // TODO: This is where lobby settings would go

            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        game = gameObject.AddComponent<GameCore>();

        game.populateBoard();

        gameState = new GameState();

        AssignPlayers();

        SyncBoard();
    }

    // Sync the board with the other player
    // This only needs to be called on connect and other cases where the board may not be synced
    private void SyncBoard()
    {
        UpdateSerializableObject();
        UpdateGameState(gameState);
    }

    // Update the game state with the current serializable object
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void UpdateGameState(GameState state)
    {
        gameState = state;
        for (int i = 0; i < gameState.pieces.GetLength(0); i++)
        {
            for (int j = 0; j < gameState.pieces.GetLength(1); j++)
            {
                game.gameBoard[i, j].GetComponent<PieceLogic>().player = gameState.pieces[i, j].player;
            }
        }
    }

    // Update the serializable object with the current game state
    private void UpdateSerializableObject()
    {
        for (int i = 0; i < game.gameBoard.GetLength(0); i++)
        {
            for (int j = 0; j < game.gameBoard.GetLength(1); j++)
            {
                gameState.pieces[i, j] = new PieceData()
                {
                    row = i,
                    col = j,
                    player = game.gameBoard[i, j].GetComponent<PieceLogic>().player
                };
            }
        }
    }

    // Send the move to the other player
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPCSendMove(char direction)
    {
        Debug.Log("Sending move: " + direction);
        game.makeMove(direction);
    }

    private void AssignPlayers()
    {
        // Assign the players to the game
        game.p1 = new NetworkedPlayer('X');
        game.p2 = new NetworkedPlayer('O');
    }
}
