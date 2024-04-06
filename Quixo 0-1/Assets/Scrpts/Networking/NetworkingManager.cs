using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;

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

public class ChatMessage
{
    public string message;
    public PlayerRef playerRef;

    public ChatMessage(string message, PlayerRef playerRef)
    {
        this.message = message;
        this.playerRef = playerRef;
    }
}

public class NetworkingManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public List<KeyValuePair<PlayerRef, NetworkedPlayer>> _players = new();

    public static bool GameSetUp = false;
    private bool IsRunnerDestoryed = false;

    // Only used in the case of disconnects and reconnects
    public int currentTurn = 0;
    public string lobbyName = null;

    public bool drawInProgress = false;

    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private NetworkPrefabRef _networkChatPrefab;

    public void Start()
    {
        HideChat();

        HideButtons();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        _players.Add(new KeyValuePair<PlayerRef, NetworkedPlayer>(player, null));

        if (runner.IsServer && _players.Count == 1)
        {
            game = GameObject.Find("GameMaster").GetComponent<GameCore>();

            game.showError("Waiting for client to join...");

            Time.timeScale = 1;
        }

        if (_players.Count != 2) return;

        runner.SessionInfo.IsOpen = false;

        SetupGame();

        if (runner.IsServer)
        {
            game.closeError();
        }

        ShowButtons();
        ShowChat();

        ButtonHandler.OnMoveMade += SendMove;
        GameCore.OnChosenPiece += SetChosenPiece;
        UseChat.OnChatUpdated += SendChat;
        NetworkChat.OnNetworkChatUpdated += UpdateLocalChatLog;
        PauseButton.OnNetworkingGameRestart += Rematch;
    }

    public async void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        int playerIndex = _players.FindIndex(p => p.Key == player);

        if (runner.IsServer && player != runner.LocalPlayer)
        {
            if (game.gameOver)
            {
                await DisconnectFromPhoton();

                OnNetworkError?.Invoke(ShutdownReason.OperationCanceled);

                return;
            }

            if (drawInProgress)
            {
                await DisconnectFromPhoton();

                OnNetworkError?.Invoke(ShutdownReason.IncompatibleConfiguration);

                return;
            }

            currentTurn = game.currentPlayer.piece == 'O' ? 2 : 1;

            game.showError("Client has disconnected. Waiting until they rejoin...");

            runner.Despawn(chat.GetComponent<NetworkObject>());
            Destroy(chat.gameObject);
            chat = null;

            HideButtons();

            HideChat();
        }

        if (playerIndex != -1)
        {
            NetworkedPlayer networkedPlayer = _players[playerIndex].Value;
            runner.Despawn(networkedPlayer.GetComponent<NetworkObject>());
            _players.RemoveAt(playerIndex);
            GameObject.Destroy(networkedPlayer.gameObject);
        }

        ButtonHandler.OnMoveMade -= SendMove;
        GameCore.OnChosenPiece -= SetChosenPiece;
        UseChat.OnChatUpdated -= SendChat;
        NetworkChat.OnNetworkChatUpdated -= UpdateLocalChatLog;
        PauseButton.OnNetworkingGameRestart -= Rematch;

        runner.SessionInfo.IsOpen = true;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public async void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (IsRunnerDestoryed) return;

        if (game != null)
        {
            game.gamePaused = true;

            Time.timeScale = 0;
        }

        OnNetworkError?.Invoke(shutdownReason);

        IsRunnerDestoryed = true;

        await DisconnectFromPhoton();
    }

#pragma warning disable UNT0006 // Incorrect message signature. Signature is correct, not sure why it is saying that it isn't

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }
    public async void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        game.gamePaused = true;

        Time.timeScale = 0;

        OnNetworkError?.Invoke(ShutdownReason.ConnectionTimeout);

        await DisconnectFromPhoton();
    }

#pragma warning restore UNT0006 // Incorrect message signature. Signature is correct, not sure why it is saying that it isn't

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
    public NetworkChat chat;

    public List<ChatMessage> chatLog = new();

    // Event used for error handler when the scene needs to change
    public delegate void NetworkError(ShutdownReason shutdownReason);
    public static event NetworkError OnNetworkError;

    public async Task StartGame(GameMode mode, string sessionName)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();

        _runner.ProvideInput = true;

        bool enableClientSessionCreation = false;
        bool isOpen = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        lobbyName = sessionName;

        if (mode == GameMode.Host)
        {
            isOpen = false;

            System.Random res = new System.Random();

            String str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int size = 6;

            String ran = "";

            for (int i = 0; i < size; i++)
            {
                ran += str[res.Next(26)];
            }

            lobbyName = ran;

            // TODO: Display this room name on the host's screen. Stored in class's lobbyName variable so it can always be accessed
        }

        if (mode == GameMode.AutoHostOrClient)
        {
            enableClientSessionCreation = true;
            lobbyName = null;
        }

        var result = await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = lobbyName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2,
            EnableClientSessionCreation = enableClientSessionCreation,
            IsOpen = isOpen,
        });

        if (!result.Ok)
        {
            IsRunnerDestoryed = true;

            if (game != null)
            {
                game.gamePaused = true;

                Time.timeScale = 0;
            }

            OnNetworkError?.Invoke(ShutdownReason.GameNotFound);

            await DisconnectFromPhoton();
        }
    }

    public void SetupGame(bool rematch = false)
    {
        NetworkedPlayer.TotalPlayers = 0;

        game = GameObject.Find("GameMaster").GetComponent<GameCore>();

        gameState = GameState.Create();

        game.buttonHandler = GameObject.FindObjectOfType<ButtonHandler>();

        if (!GameObject.Find("GamePiece(Clone)") || rematch)
        {
            GameSetUp = false;

            game.populateBoard();
        }

        if (_runner.IsServer && rematch != true)
        {
            // TODO @R1tz02 #26: Hide message about waiting for client to rejoin

            // AssignPlayers might need to check if the game is set up
            // If so, it will just skip creating new players on host
            AssignPlayers(() =>
            {
                GetNetworkedPlayer(_runner.LocalPlayer).RpcAssignPlayers(_players[0].Key, _players[1].Key, currentTurn);

                currentTurn = 0;

                SyncBoard();
            });
        }

        game.drawButton.gameObject.SetActive(true);
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
                game.gameBoard[i, j].GetComponent<PieceLogic>().player = piece.player;

                // Sync colors based on player piece
                switch (piece.player)
                {
                    case 'X':
                        game.gameBoard[piece.row, piece.col].GetComponent<Renderer>().material = game.playerOneSpace;
                        break;
                    case 'O':
                        game.gameBoard[piece.row, piece.col].GetComponent<Renderer>().material = game.playerTwoSpace;
                        break;
                }
            }
        }
    }

    public NetworkedPlayer GetNetworkedPlayer(PlayerRef playerRef)
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

        if (GetNetworkedPlayer(_runner.LocalPlayer) != null)
        {
            NetworkedPlayer networkedPlayer = GetNetworkedPlayer(_runner.LocalPlayer);
            _runner.Despawn(networkedPlayer.GetComponent<NetworkObject>());
            GameObject.Destroy(networkedPlayer.gameObject);
        }

        for (int playerIndex = 0; playerIndex < 2; playerIndex++)
        {
            var player = _players[playerIndex];

            char playerSymbol = playerIndex == 0 ? 'X' : 'O';

            NetworkedPlayer networkedPlayer = SpawnNetworkedPlayer(player.Key, playerSymbol);

            _players[playerIndex] = new KeyValuePair<PlayerRef, NetworkedPlayer>(player.Key, networkedPlayer);

            Debug.Log("Spawned player: " + player.Key);
        }

        // Spawn NetworkChat Object
        if (chat == null)
        {
            _runner.Spawn(_networkChatPrefab);
        }

        // Use this callback to make sure that the players are created on the client before continuing
        StartCoroutine(NetworkedPlayer.WaitForClientConfirmation(OnComplete));

        ShowChat();
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
        byte move = (byte)direction;

        GetNetworkedPlayer(_runner.LocalPlayer).RpcSendMove(move, GetNetworkedPlayer(_runner.LocalPlayer).PlayerRef);
    }

    public void OnDestroy()
    {
        ButtonHandler.OnMoveMade -= SendMove;
        GameCore.OnChosenPiece -= SetChosenPiece;
        UseChat.OnChatUpdated -= SendChat;
        NetworkChat.OnNetworkChatUpdated -= UpdateLocalChatLog;
        PauseButton.OnNetworkingGameRestart -= Rematch;
    }

    public void SetChosenPiece(int row, int col)
    {
        GetNetworkedPlayer(_runner.LocalPlayer).RpcSetChosenPiece(row, col);
    }
    public async Task DisconnectFromPhoton()
    {
        IsRunnerDestoryed = true;

        GameSetUp = false;

        await _runner.Shutdown();
    }

    public void SendChat(string message)
    {
        chat.RpcSendChatMessage(message, GetNetworkedPlayer(_runner.LocalPlayer).PlayerRef);
    }

    public void UpdateLocalChatLog(string message, PlayerRef sendingPlayerRef, PlayerRef localPlayerRef, PlayerRef hostsPlayerRef)
    {
        chatLog.Add(new ChatMessage(message, sendingPlayerRef));
    }

    public void Rematch()
    {
        // Sets the local player as wanting a rematch
        // If both players do, then ResetGame() will be called
        GetNetworkedPlayer(_runner.LocalPlayer).Rematch();
    }

    public async void ResetGame()
    {
        await game.ResetBoard();

        _runner.SessionInfo.IsOpen = true;

        GameSetUp = false;
        currentTurn = 0;
        SetupGame(true);

        if (Data.CURRENT_LANGUAGE == "English")
        {
            GameObject.Find("playAgainTxt").gameObject.GetComponent<TMP_Text>().text = "Restart";
            GameObject.Find("tiePlayAgainTxt").gameObject.GetComponent<TMP_Text>().text = "Restart";
        }
        else if (Data.CURRENT_LANGUAGE == "Espa�ol")
        {
            GameObject.Find("tiePlayAgainTxt").gameObject.GetComponent<TMP_Text>().text = "Reiniciar";
            GameObject.Find("playAgainTxt").gameObject.GetComponent<TMP_Text>().text = "Reiniciar";
        }
    }

    private void HideButtons()
    {
        if (game == null)
        {
            game = GameObject.Find("GameMaster").GetComponent<GameCore>();
        }
        if (game.currentGameMode == GameType.Online)
        {
            game.drawButton.gameObject.SetActive(false);
            GameObject.Find("Menu Manager").GetComponent<PauseButton>().pauseButton.gameObject.SetActive(false);
            game.buttonsCanvas.enabled = false;
        }
    }

    private void ShowButtons()
    {
        if (game == null)
        {
            game = GameObject.Find("GameMaster").GetComponent<GameCore>();
        }

        game.drawButton.gameObject.SetActive(true);
        GameObject.Find("Menu Manager").GetComponent<PauseButton>().pauseButton.gameObject.SetActive(true);
        game.buttonsCanvas.enabled = true;
    }

    private void ShowChat()
    {
        Canvas chat = GameObject.Find("Chat").GetComponent<Canvas>();
        chat.enabled = true;
    }

    private void HideChat()
    {
        Canvas chat = GameObject.Find("Chat").GetComponent<Canvas>();
        chat.enabled = false;
    }
}
