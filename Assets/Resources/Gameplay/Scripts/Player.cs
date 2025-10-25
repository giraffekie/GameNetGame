using UnityEngine;

namespace Resources.Gameplay.Scripts
{
    [System.Serializable]
    public class Player
    {
        public string Name { get; private set; }
        public int Hits { get; private set; }

        public Player(string name)
        {
            Name = name;
            Hits = 0;
        }

        public void RegisterHit()
        {
            Hits++;
            Debug.Log($"{Name} total hits: {Hits}");
        }
    }
}
