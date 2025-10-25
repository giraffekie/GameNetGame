using Resources.Gameplay.Scripts.UI;
using UnityEngine;

namespace Resources.Gameplay.Scripts
{
    public class HitCircleBehavior : MonoBehaviour
    {
        private Player _assignedPlayer;

        [Header("Timing & Visuals")]
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private Transform shrinkingRing;
        [SerializeField] private float startScale = 2f;
        [SerializeField] private float targetScale = 1f;
        [SerializeField] private float endScale = 0.8f;

        [Header("Accuracy Thresholds (scale difference)")]
        [SerializeField] private float perfectThreshold = 0.05f;
        [SerializeField] private float greatThreshold = 0.15f;
        [SerializeField] private float goodThreshold = 0.3f;

        private float _timer;
        private bool _clicked = false;

        public void AssignPlayer(Player player)
        {
            _assignedPlayer = player;
        }

        private void Start()
        {
            if (shrinkingRing != null)
                shrinkingRing.localScale = Vector2.one * startScale;
        }

        private void Update()
        {
            _timer += Time.deltaTime;

            // Shrink ring smoothly
            if (shrinkingRing != null)
            {
                float t = _timer / lifetime;
                float currentScale = Mathf.Lerp(startScale, endScale, t);
                shrinkingRing.localScale = Vector2.one * currentScale;
            }

            // Auto-miss if not clicked in time
            if (_timer >= lifetime && !_clicked && _assignedPlayer != null)
            {
                Debug.Log($"{_assignedPlayer?.Name ?? "Unknown"} missed!");
                HitFeedbackUI.Instance.ShowFeedback(_assignedPlayer, "MISS");
                Destroy(gameObject);
            }
        }

        private void OnMouseDown()
        {
            if (_clicked) return;

            if (_assignedPlayer == null)
            {
                Debug.LogWarning("Hit circle has no assigned player!");
                return;
            }

            if (_assignedPlayer != LocalPlayerManager.LocalPlayer)
            {
                Debug.Log("This circle is not for you!");
                return;
            }

            _clicked = true;

            float currentScale = shrinkingRing.localScale.x;
            float diff = currentScale - targetScale; // can be positive (too early) or negative (too late)
            float absDiff = Mathf.Abs(diff);

            string result;

            // Evaluate accuracy
            if (absDiff <= perfectThreshold)
                result = "PERFECT!";
            else if (absDiff <= greatThreshold)
                result = "GREAT!";
            else if (absDiff <= goodThreshold)
                result = "GOOD";
            else
                result = "MISS";

            Debug.Log($"{_assignedPlayer.Name} hit → scale {currentScale:F2} | diff {diff:F2} → {result}");
            _assignedPlayer.RegisterHit();

            HitFeedbackUI.Instance.ShowFeedback(_assignedPlayer, result);
            Destroy(gameObject);
        }
    }
}
