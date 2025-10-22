using System.Collections.Generic;
using UnityEngine;

namespace Resources.MainMenu.Scripts
{
    public static class AuthManager
    {
        private const string ACCOUNTS_KEY = "SavedAccounts";
        private const string CURRENT_USER_KEY = "CurrentUser";
        
        [System.Serializable]
        private class AccountList
        {
            public List<Account> accounts = new List<Account>();
        }

        [System.Serializable]
        private class Account
        {
            public string username;
            public string password;
        }
        
        /// <summary>
        /// Gets a ParrelSync-aware key that includes the clone name if applicable
        /// </summary>
        private static string GetCloneAwareKey(string baseKey)
        {
            #if UNITY_EDITOR
            // Check if ParrelSync is available and we're running in a clone
            if (ParrelSync.ClonesManager.IsClone())
            {
                string cloneName = ParrelSync.ClonesManager.GetCurrentProjectPath();
                // Extract just the folder name from the full path
                string cloneFolderName = System.IO.Path.GetFileName(cloneName);
                if (!string.IsNullOrEmpty(cloneFolderName))
                {
                    return $"{baseKey}_{cloneFolderName}";
                }
            }
            #endif
            return baseKey;
        }
        
        /// <summary>
        /// Registers a new account if username doesn't already exist.
        /// Returns true if successful.
        /// </summary>
        public static bool Register(string username, string password)
        {
            AccountList list = LoadAccounts();

            if (list.accounts.Exists(a => a.username == username))
                return false;

            list.accounts.Add(new Account { username = username, password = password });
            SaveAccounts(list);
            return true;
        }
        
        /// <summary>
        /// Checks if credentials match an existing account.
        /// </summary>
        public static bool Login(string username, string password)
        {
            AccountList list = LoadAccounts();
            bool success = list.accounts.Exists(a => a.username == username && a.password == password);
            
            if (success)
            {
                // Store the current logged-in user with clone-aware key
                string currentUserKey = GetCloneAwareKey(CURRENT_USER_KEY);
                PlayerPrefs.SetString(currentUserKey, username);
                PlayerPrefs.Save();
                
                Debug.Log($"[AuthManager] Logged in as {username} with key: {currentUserKey}");
            }
            
            return success;
        }
        
        /// <summary>
        /// Gets the currently logged-in username
        /// </summary>
        public static string GetCurrentUser()
        {
            string currentUserKey = GetCloneAwareKey(CURRENT_USER_KEY);
            string username = PlayerPrefs.GetString(currentUserKey, "");
            
            Debug.Log($"[AuthManager] Retrieved user: {username} from key: {currentUserKey}");
            return username;
        }
        
        /// <summary>
        /// Logs out the current user
        /// </summary>
        public static void Logout()
        {
            string currentUserKey = GetCloneAwareKey(CURRENT_USER_KEY);
            PlayerPrefs.DeleteKey(currentUserKey);
            PlayerPrefs.Save();
        }
        
        private static void SaveAccounts(AccountList list)
        {
            string json = JsonUtility.ToJson(list);
            PlayerPrefs.SetString(ACCOUNTS_KEY, json);
            PlayerPrefs.Save();
        }

        private static AccountList LoadAccounts()
        {
            string json = PlayerPrefs.GetString(ACCOUNTS_KEY, "{}");
            AccountList list = JsonUtility.FromJson<AccountList>(json);
            return list ?? new AccountList();
        }
    }
}