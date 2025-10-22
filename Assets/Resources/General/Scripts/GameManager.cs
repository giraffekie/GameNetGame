using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Resources.General.Scripts
{
    public class GameManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static GameManager Instance;

        private NetworkRunner _runner;

        [Header("Game Settings")]
        [SerializeField] private GameMode currentGameMode;
        [SerializeField] private int gameSceneBuildIndex = 1;
        [SerializeField] private NetworkObject playerPrefab;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void StartAsHost()
        {
            CallStartGame(GameMode.Host);
        }

        public void StartAsClient()
        {
            CallStartGame(GameMode.Client);
        }

        public void CallStartGame(GameMode mode)
        {
            if (_runner == null) StartGame(mode);
            else Debug.LogWarning("Game already started!");
        }

        async void StartGame(GameMode mode)
        {
            currentGameMode = mode;

            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            // Start directly in the game scene
            await _runner.StartGame(new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "TestRoom",
                Scene = SceneRef.FromIndex(gameSceneBuildIndex),
                PlayerCount = 2,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });

            Debug.Log($"[GameManager] Started game in {mode} mode in scene: {gameSceneBuildIndex}");
        }

        // Implement OnPlayerJoined to spawn player prefab
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"[GameManager] Player {player} joined");
            
            if (runner.IsServer && playerPrefab != null)
            {
                // Spawn the player prefab for the joining player
                NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
                Debug.Log($"[GameManager] Spawned player object for {player}");
            }
        }

        #region INetworkRunnerCallbacks (empty)
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
        #endregion
    }
}