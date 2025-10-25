using UnityEngine;
using TMPro;
using Resources.Gameplay.Scripts;

namespace Resources.Gameplay.Scripts.UI
{
    public class HitFeedbackUI : MonoBehaviour
    {
        public static HitFeedbackUI Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI feedbackText;

        [Header("Display Settings")]
        [SerializeField] private float displayDuration = 1f;

        [Header("Feedback Colors")]
        [SerializeField] private Color perfectColor = Color.yellow;
        [SerializeField] private Color greatColor = Color.green;
        [SerializeField] private Color goodColor = Color.cyan;
        [SerializeField] private Color missColor = Color.red;
        [SerializeField] private Color defaultColor = Color.white;

        private float _timer;
        private bool _isShowing;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            feedbackText.text = "";
        }

        private void Update()
        {
            if (_isShowing)
            {
                _timer += Time.deltaTime;
                if (_timer >= displayDuration)
                {
                    feedbackText.text = "";
                    _isShowing = false;
                }
            }
        }

        // Only show feedback for the local player
        public void ShowFeedback(Player player, string result)
        {
            if (player != LocalPlayerManager.LocalPlayer)
                return; // ignore if not local player

            feedbackText.text = result;
            feedbackText.color = GetColorForResult(result);

            _timer = 0f;
            _isShowing = true;
        }

        private Color GetColorForResult(string result)
        {
            switch (result.ToUpper())
            {
                case "PERFECT!": return perfectColor;
                case "GREAT!":   return greatColor;
                case "GOOD":     return goodColor;
                case "MISS":     return missColor;
                default:         return defaultColor;
            }
        }
    }
}
