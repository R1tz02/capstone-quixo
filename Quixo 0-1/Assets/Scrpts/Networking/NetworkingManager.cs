using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;

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
    public List<KeyValuePair<PlayerRef, NetworkedPlayer>> _players = new();

    public static bool GameSetUp = false;

    [SerializeField] private NetworkPrefabRef _playerPrefab;

    // TODO: add field like 'gameboardPopulated' to check if the gameboard has been populated
    // This will be needed to make sure the host doesn't populate the gameboard again if a client disconnects and reconnects

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        _players.Add(new KeyValuePair<PlayerRef, NetworkedPlayer>(player, null));

        if (_players.Count != 2) return;

        game = GameObject.Find("GameMaster").GetComponent<GameCore>();

        gameState = GameState.Create();

        game.buttonHandler = GameObject.FindObjectOfType<ButtonHandler>();

        game.populateBoard();

        if (runner.IsServer)
        {
            AssignPlayers(() =>
            {
                GetNetworkedPlayer(runner.LocalPlayer).RpcAssignPlayers(_players[0].Key, _players[1].Key);
                SyncBoard();
            });

        }

        ButtonHandler.OnMoveMade += SendMove;
        GameCore.OnChosenPiece += SetChosenPiece;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        int playerIndex = _players.FindIndex(p => p.Key == player);
        if (playerIndex != -1)
        {
            NetworkedPlayer networkedPlayer = _players[playerIndex].Value;
            runner.Despawn(networkedPlayer.GetComponent<NetworkObject>());
            _players.RemoveAt(playerIndex);
        }

        // TODO: If the same player joins back, they join as player 3
        // They need to join back as player 2
        // Find way to clear them out from Photon and reassign them as player 2
    }
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

    public NetworkRunner _runner;
    public GameCore game;
    public GameState gameState;
    private int PlayerTurn = 1;

    public async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();

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
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            // TODO: Might be possible to specify how many players are allowed in the room here
        });

        Debug.Log("Game started");
    }

    private void SyncBoard()
    {
        UpdateSerializableObject();

        GetNetworkedPlayer(_runner.LocalPlayer).RpcUpdateGameState(gameState.Serialize());
    }

    public void UpdateGameState()
    {
        for (int i = 0; i < GameState.Rows; i++)
        {
            for (int j = 0; j < GameState.Cols - 1; j++)
            {
                int index = i * (GameState.Cols - 1) + j;
                PieceData piece = gameState.piecesData[index];
                game.gameBoard[piece.row, piece.col].GetComponent<PieceLogic>().player = piece.player;
            }
        }
    }

    private NetworkedPlayer GetNetworkedPlayer(PlayerRef playerRef)
    {
        int playerIndex = _players.FindIndex(p => p.Key == playerRef);
        if (playerIndex != -1)
        {
            return _players[playerIndex].Value;
        }

        throw new Exception("Player not found");
    }

    private void UpdateSerializableObject()
    {
        for (int i = 0; i < GameState.Rows; i++)
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

    private void AssignPlayers(Action OnComplete)
    {
        if (_runner == null)
        {
            Debug.Log("Runner is not initialized");
            return;
        }

        for (int playerIndex = 0; playerIndex < 2; playerIndex++)
        {
            var player = _players[playerIndex];

            char playerSymbol = playerIndex == 0 ? 'X' : 'O';

            NetworkedPlayer networkedPlayer = SpawnNetworkedPlayer(player.Key, playerSymbol);

            _players[playerIndex] = new KeyValuePair<PlayerRef, NetworkedPlayer>(player.Key, networkedPlayer);

            Debug.Log("Spawned player: " + player.Key);
        }

        // Use this callback to make sure that the players are created on the client before continuing
        StartCoroutine(NetworkedPlayer.WaitForClientConfirmation(OnComplete));
    }

    public NetworkedPlayer SpawnNetworkedPlayer(PlayerRef playerRef, char playerSymbol)
    {
        NetworkObject networkedObject = _runner.Spawn(_playerPrefab, Vector3.zero, Quaternion.identity, playerRef);

        NetworkedPlayer networkedPlayer = networkedObject.GetComponent<NetworkedPlayer>();

        networkedPlayer.Initialize(playerSymbol, playerRef);

        return networkedPlayer;
    }

    public void SendMove(char direction)
    {
        if (GetNetworkedPlayer(_runner.LocalPlayer).PlayerNumber != PlayerTurn) return;

        byte move = (byte)direction;

        GetNetworkedPlayer(_runner.LocalPlayer).RpcSendMove(move);

        PlayerTurn = PlayerTurn == 1 ? 2 : 1;
    }

    public void OnDestroy()
    {
        ButtonHandler.OnMoveMade -= SendMove;
        GameCore.OnChosenPiece -= SetChosenPiece;
    }

    public void SetChosenPiece(int row, int col)
    {
        GetNetworkedPlayer(_runner.LocalPlayer).RpcSetChosenPiece(row, col);
    }
}