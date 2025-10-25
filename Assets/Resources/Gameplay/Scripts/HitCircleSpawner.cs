using UnityEngine;

namespace Resources.Gameplay.Scripts
{
    public class HitCircleSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject hitCirclePrefab;
        [SerializeField] private float spawnInterval = 1f;

        [Header("Spawn Range Settings")]
        [SerializeField] private Vector2 spawnRangeX = new Vector2(-8f, 8f);
        [SerializeField] private Vector2 spawnRangeY = new Vector2(-4f, 4f);

        [Header("Player Colors")]
        [SerializeField] private Color player1Color = Color.cyan;
        [SerializeField] private Color player2Color = Color.magenta;
        [Range(0f, 1f)]
        [SerializeField] private float ringTransparency = 0.5f;

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

            // Find child sprites
            var ring = behavior.transform.Find("Ring")?.GetComponent<SpriteRenderer>();
            var sprite = behavior.transform.Find("Circle")?.GetComponent<SpriteRenderer>();

            if (ring != null && sprite != null)
            {
                var baseColor = assigned == GameplayManager.Instance.Player1
                    ? player1Color
                    : player2Color;

                // Apply colors
                sprite.color = baseColor;
                ring.color = new Color(baseColor.r, baseColor.g, baseColor.b, ringTransparency);

                // Ensure ring renders behind its circle but above background
                ring.sortingOrder = 0;
                sprite.sortingOrder = 1;
            }

            behavior.AssignPlayer(assigned);
        }

        private Vector3 RandomPosition()
        {
            float x = Random.Range(spawnRangeX.x, spawnRangeX.y);
            float y = Random.Range(spawnRangeY.x, spawnRangeY.y);
            return new Vector3(x, y, 0f);
        }
    }
}
