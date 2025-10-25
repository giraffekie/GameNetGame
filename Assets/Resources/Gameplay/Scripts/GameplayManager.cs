using UnityEngine;

namespace Resources.Gameplay.Scripts
{
    public class GameplayManager : MonoBehaviour
    {
        public static GameplayManager Instance { get; private set; }

        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            Player1 = new Player("Player 1");
            Player2 = new Player("Player 2");
        }
    }
}
