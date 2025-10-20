using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

namespace Resources.Fusion.Scripts
{
    public class PlayerInfoSender : NetworkBehaviour
    {
        public static PlayerInfoSender Instance;
        private Dictionary<PlayerRef, string> _joinedPlayers = new Dictionary<PlayerRef, string>();

        [SerializeField] private TMP_InputField usernameInput;

        /// <summary>
        /// Event fired when a player joins
        /// </summary>
        public static event Action<PlayerRef, string, GameObject> OnPlayerJoined;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public override void Spawned()
        {
            base.Spawned();
            if (!Runner.IsServer && !Runner.IsClient)
                return;

            // Send username automatically after Spawned
            SendUsername();
        }

        /// <summary>
        /// Called by UI button when player clicks "Submit Username"
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

            string username = usernameInput.text;
            if (string.IsNullOrEmpty(username))
                username = $"Player_{Runner.LocalPlayer.PlayerId}";

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

            // Fire the C# event for anyone listening
            OnPlayerJoined?.Invoke(player, username, null); // null for PlayerObject for now
        }
    }
}
