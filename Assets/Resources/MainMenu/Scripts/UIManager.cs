using System.Reflection;
using Resources.General.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.MainMenu.Scripts
{
    public class UIManager : MonoBehaviour
    {
        [Header("Login UI Elements")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private TMP_InputField loginUsernameInput;
        [SerializeField] private TMP_InputField loginPasswordInput;
        [SerializeField] private Button[] loginButtons;
        [SerializeField] private TextMeshProUGUI loginErrorMessage;
        [SerializeField] private Button switchSignupButton;
        
        [Header("Signup UI Elements")]
        [SerializeField] private GameObject signupPanel;
        [SerializeField] private TMP_InputField signupUsernameInput;
        [SerializeField] private TMP_InputField signupPasswordInput;
        [SerializeField] private TMP_InputField signupConfirmPasswordInput;
        [SerializeField] private Button signupButton;
        [SerializeField] private TextMeshProUGUI signupErrorMessage;
        [SerializeField] private Button switchLoginButton;
        
        private void Awake()
        {
            CheckSerializedFields();

            // Hook up navigation buttons
            if (switchSignupButton != null)
                switchSignupButton.onClick.AddListener(SwitchToSignup);

            if (switchLoginButton != null)
                switchLoginButton.onClick.AddListener(SwitchToLogin);

            foreach (var btn in loginButtons)
            {
                if (btn != null)
                {
                    var capturedButton = btn;
                    capturedButton.onClick.AddListener(() => OnLoginClicked(capturedButton));
                }
            }

            if (signupButton != null)
                signupButton.onClick.AddListener(OnSignupClicked);
        }
        
        /// <summary>
        /// Loops through all serialized fields and logs any missing references.
        /// </summary>
        private void CheckSerializedFields()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                if (field.IsDefined(typeof(SerializeField), false))
                {
                    object value = field.GetValue(this);
                    if (value == null)
                    {
                        Debug.LogWarning($"[UIManager] Missing reference for: {field.Name}", this);
                    }
                }
            }
        }
        
        // -------------------------------
        // UI Functions
        // -------------------------------
        
        /// <summary>
        /// Switches from Login UI to Signup UI.
        /// </summary>
        public void SwitchToSignup()
        {
            loginPanel.SetActive(false);
            signupPanel.SetActive(true);
            ClearLoginFields();
            ClearLoginError();
        }

        /// <summary>
        /// Switches from Signup UI to Login UI.
        /// </summary>
        public void SwitchToLogin()
        {
            signupPanel.SetActive(false);
            loginPanel.SetActive(true);
            ClearSignupFields();
            ClearSignupError();
        }
        
        /// <summary>
        /// Triggered when user clicks any login button.
        /// </summary>
        private void OnLoginClicked(Button clickedButton)
        {
            string username = loginUsernameInput.text.Trim();
            string password = loginPasswordInput.text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowLoginError("Please enter both username and password.");
                return;
            }

            if (AuthManager.Login(username, password))
            {
                Debug.Log($"Login success for {username}");
                ClearLoginError();

                // Store username immediately
                string loggedInUser = AuthManager.GetCurrentUser();
                Debug.Log($"[UIManager] Stored user: {loggedInUser}");

                // Determine which login button was clicked
                if (clickedButton == loginButtons[0])
                {
                    GameManager.Instance.StartAsHost();
                }
                else if (clickedButton == loginButtons[1])
                {
                    GameManager.Instance.StartAsClient();
                }
        
                // Hide UI panels
                loginPanel.SetActive(false);
                signupPanel.SetActive(false);
            }
            else
            {
                ShowLoginError("Invalid username or password.");
            }
        }

        
        /// <summary>
        /// Triggered when user clicks signup button.
        /// </summary>
        private void OnSignupClicked()
        {
            string username = signupUsernameInput.text.Trim();
            string password = signupPasswordInput.text.Trim();
            string confirm = signupConfirmPasswordInput.text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowSignupError("All fields are required.");
                return;
            }

            if (password != confirm)
            {
                ShowSignupError("Passwords do not match.");
                return;
            }

            if (AuthManager.Register(username, password))
            {
                Debug.Log($"Account created for {username}");
                ClearSignupError();
                SwitchToLogin();
            }
            else
            {
                ShowSignupError("Username already exists.");
            }
        }
        
        // -------------------------------
        // Helper Methods
        // -------------------------------
        
        private void ClearLoginFields()
        {
            loginUsernameInput.text = "";
            loginPasswordInput.text = "";
        }

        private void ClearSignupFields()
        {
            signupUsernameInput.text = "";
            signupPasswordInput.text = "";
            signupConfirmPasswordInput.text = "";
        }

        private void ShowLoginError(string message)
        {
            if (loginErrorMessage != null)
            {
                loginErrorMessage.gameObject.SetActive(true);
                loginErrorMessage.text = message;
            }
        }

        private void ShowSignupError(string message)
        {
            if (signupErrorMessage != null)
            {
                signupErrorMessage.gameObject.SetActive(true);
                signupErrorMessage.text = message;
            }
        }

        private void ClearLoginError()
        {
            if (loginErrorMessage != null)
                loginErrorMessage.text = "";
        }

        private void ClearSignupError()
        {
            if (signupErrorMessage != null)
                signupErrorMessage.text = "";
        }
    }
}
