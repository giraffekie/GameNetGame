using UnityEngine;

namespace Resources.Gameplay.Scripts
{
    public class HitCircleBehavior : MonoBehaviour
    {
        private Player _assignedPlayer;
        [SerializeField] private float lifetime = 2f;
        private float _timer;

        public void AssignPlayer(Player player)
        {
            _assignedPlayer = player;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= lifetime)
            {
                Debug.Log($"Circle for {_assignedPlayer?.Name ?? "Unknown"} expired!");
                Destroy(gameObject);
            }
        }

        private void OnMouseDown()
        {
            if (_assignedPlayer == null)
            {
                Debug.LogWarning("Hit circle has no assigned player!");
                return;
            }

            // Only allow local player to hit if this circle belongs to them
            if (_assignedPlayer != LocalPlayerManager.LocalPlayer)
            {
                Debug.Log("Di para sayo to :3");
                return;
            }

            _assignedPlayer.RegisterHit();
            Destroy(gameObject);
        }
    }
}