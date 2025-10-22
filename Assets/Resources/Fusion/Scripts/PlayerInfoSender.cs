using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Resources.MainMenu.Scripts;
using UnityEngine;

namespace Resources.Fusion.Scripts
{
    public class PlayerInfoSender : NetworkBehaviour
    {
        public static PlayerInfoSender Instance;
        private Dictionary<PlayerRef, string> _joinedPlayers = new Dictionary<PlayerRef, string>();

        /// <summary>
        /// Event fired when a player joins
        /// </summary>
        public static event Action<PlayerRef, string, GameObject> OnPlayerJoined;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public override void Spawned()
        {
            base.Spawned();
            Debug.Log($"[PlayerInfoSender] Spawned for player {Runner.LocalPlayer}, Object: {gameObject.name}");
            
            // Send username automatically after Spawned
            SendUsername();
        }

        /// <summary>
        /// Called to send username (now automatically called in Spawned)
        /// </summary>
        public void SendUsername()
        {
            StartCoroutine(SendUsernameWhenReady());
        }

        private IEnumerator SendUsernameWhenReady()
        {
            // Wait until Runner and LocalPlayer are ready
            while (Runner == null || !Runner.LocalPlayer.IsRealPlayer)
            {
                yield return null;
            }

            // Get username from AuthManager
            string username = AuthManager.GetCurrentUser();
            
            // If no username found, use a default based on player ID
            if (string.IsNullOrEmpty(username))
            {
                username = $"Player_{Runner.LocalPlayer.PlayerId}";
                Debug.Log($"[PlayerInfoSender] No stored username found, using default: {username}");
            }
            else
            {
                Debug.Log($"[PlayerInfoSender] Using stored username: {username}");
            }

            Debug.Log($"[PlayerInfoSender] Sending username: {username} for player {Runner.LocalPlayer}");
            RPC_SendUserInfo(username);
        }

        /// <summary>
        /// Sends the username to server/state authority
        /// </summary>
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_SendUserInfo(string username, RpcInfo info = default)
        {
            PlayerRef player = info.Source;
            if (!player.IsRealPlayer)
                player = Runner.LocalPlayer;

            Debug.Log($"[PlayerInfoSender] Player {player.PlayerId} set username: {username}");

            // Store player info
            if (!_joinedPlayers.ContainsKey(player))
            {
                _joinedPlayers.Add(player, username);
            }
            else
            {
                _joinedPlayers[player] = username;
            }

            // Broadcast join to all clients
            RPC_BroadcastPlayerJoined(player, username);
        }

        /// <summary>
        /// Broadcast to all clients that this player joined
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_BroadcastPlayerJoined(PlayerRef player, string username)
        {
            Debug.Log($"[PlayerInfoSender] Broadcasted player joined: {username}, ID: ({player.PlayerId})");

            // Store locally as well
            if (!_joinedPlayers.ContainsKey(player))
            {
                _joinedPlayers.Add(player, username);
            }

            // Fire the C# event for anyone listening
            OnPlayerJoined?.Invoke(player, username, null);
        }

        /// <summary>
        /// Get username for a specific player
        /// </summary>
        public string GetPlayerUsername(PlayerRef player)
        {
            return _joinedPlayers.ContainsKey(player) ? _joinedPlayers[player] : $"Player_{player.PlayerId}";
        }

        /// <summary>
        /// Get all joined players
        /// </summary>
        public Dictionary<PlayerRef, string> GetAllPlayers()
        {
            return new Dictionary<PlayerRef, string>(_joinedPlayers);
        }
    }
}