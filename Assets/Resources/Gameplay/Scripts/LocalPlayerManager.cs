using UnityEngine;

namespace Resources.Gameplay.Scripts
{
    public class LocalPlayerManager : MonoBehaviour
    {
        public static Player LocalPlayer { get; private set; }

        [SerializeField] private bool isPlayer1;

        private void Start()
        {
            if (isPlayer1)
                LocalPlayer = GameplayManager.Instance.Player1;
            else
                LocalPlayer = GameplayManager.Instance.Player2;

            Debug.Log($"This device is controlling: {LocalPlayer.Name}");
        }
    }
}