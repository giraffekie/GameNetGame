using UnityEngine;

namespace Resources.Gameplay.Scripts
{
    public class HitCircleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject hitCirclePrefab;
        [SerializeField] private float spawnInterval = 1f;
        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= spawnInterval)
            {
                SpawnCircle();
                _timer = 0f;
            }
        }

        private void SpawnCircle()
        {
            var circle = Instantiate(hitCirclePrefab, RandomPosition(), Quaternion.identity);
            var behavior = circle.GetComponent<HitCircleBehavior>();

            // Randomly assign circle to player 1 or 2
            Player assigned = Random.value > 0.5f
                ? GameplayManager.Instance.Player1
                : GameplayManager.Instance.Player2;

            // Color the circle to match the assigned player
            var sprite = behavior.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.color = assigned == GameplayManager.Instance.Player1
                    ? Color.blue
                    : Color.red;
            }

            behavior.AssignPlayer(assigned);
        }

        private Vector3 RandomPosition()
        {
            return new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0f);
        }
    }
}