using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Unity.VisualScripting;
using System.Threading.Tasks;

[Serializable]
public struct PieceData
{
    public int row;
    public int col;
    public char player;
}


[Serializable]
public struct GameState
{
    public const int Rows = 5;
    public const int Cols = 6;

    public PieceData[] piecesData;

    public static GameState Create()
    {
        return new GameState
        {
            piecesData = new PieceData[Rows * Cols]
        };
    }

    public string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    public static GameState Deserialize(string data)
    {
        return JsonUtility.FromJson<GameState>(data);
    }
}

public class NetworkingManager : MonoBehaviour, INetworkRunnerCallbacks
{
    private List<PlayerRef> _players = new List<PlayerRef>();

    [SerializeField] public NetworkPrefabRef _playerPrefab;

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player joined: " + player);
        _players.Add(player);

        if (_players.Count != 2)
        {
            Debug.Log("Need to wait for more players to join before starting the game.");
            return;
        }

        Debug.Log("All players have joined. Starting the game...");

        game = GameObject.Find("GameMaster").GetComponent<GameCore>();

        gameState = GameState.Create();

        if (_runner.IsServer)
        {
            Debug.Log("Starting game as server");

            AssignPlayers();

            game.populateBoard();

            SyncBoard();
        }

        game.currentPlayer = game.p1;

        game.buttonHandler = GameObject.FindObjectOfType<ButtonHandler>();
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
        //ButtonHandler.OnMoveMade += RpcSendMove;
    }

    public async void StartGame(GameMode mode)
    {
        Debug.Log("Starting game...");
        GameObject NetworkManager = GameObject.Find("NetworkManager");

        if (!NetworkManager.TryGetComponent<NetworkRunner>(out _runner))
        {
            Debug.LogError("Failed to get NetworkRunner component to NetworkManager game object");
            return;
        }

        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = NetworkManager.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private async void SyncBoard()
    {
        Debug.Log("Syncing board...");
        UpdateSerializableObject();
        Debug.Log("Serialized game state: " + gameState.Serialize());

        await Task.Delay(5000);
        RpcUpdateGameState(gameState.Serialize());
    }

    private void UpdateGameState()
    {
        for (int i = 0; i < GameState.Rows - 1; i++)
        {
            for (int j = 0; j < GameState.Cols - 1; j++)
            {
                int index = i * (GameState.Cols - 1) + j;
                PieceData piece = gameState.piecesData[index];
                game.gameBoard[piece.row, piece.col].GetComponent<PieceLogic>().player = piece.player;
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcUpdateGameState(string serializedState)
    {
        Debug.Log("Updating game state...");
        gameState = GameState.Deserialize(serializedState);
        Debug.Log("Deserialized game state" + gameState.piecesData.Length + " pieces.");
        UpdateGameState();
    }

    private void UpdateSerializableObject()
    {
        for (int i = 0; i < GameState.Rows - 1; i++)
        {
            for (int j = 0; j < GameState.Cols - 1; j++)
            {
                int index = i * (GameState.Cols - 1) + j;
                gameState.piecesData[index] = new PieceData()
                {
                    row = i,
                    col = j,
                    player = game.gameBoard[i, j].GetComponent<PieceLogic>().player
                };
            }
        }
    }
    /* 
        [Rpc(RpcSources.All, RpcTargets.All)]
        private void RpcSendMove(char direction) // Change char to byte
        {
            Debug.Log("Sending move: " + (char)direction); // Convert byte back to char
            game.makeMove(direction); // Convert byte back to char
        } */

    private void AssignPlayers()
    {
        Debug.Log("Assigning players...");
        if (_runner == null)
        {
            Debug.Log("Runner is not initialized");
            return;
        }
        game.p1 = SpawnNetworkedPlayer(_players[0], 'X');
        game.p2 = SpawnNetworkedPlayer(_players[1], 'O');
    }

    public NetworkedPlayer SpawnNetworkedPlayer(PlayerRef playerRef, char playerSymbol)
    {
        NetworkObject networkedObject = _runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, playerRef);

        NetworkedPlayer networkedPlayer = networkedObject.AddComponent<NetworkedPlayer>();

        networkedPlayer.Initialize(playerRef, playerSymbol);

        return networkedPlayer;
    }
}