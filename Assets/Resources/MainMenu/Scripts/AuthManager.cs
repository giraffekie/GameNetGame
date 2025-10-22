using System.Collections.Generic;
using UnityEngine;

namespace Resources.MainMenu.Scripts
{
    public static class AuthManager
    {
        private const string ACCOUNTS_KEY = "SavedAccounts";
        
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
            return list.accounts.Exists(a => a.username == username && a.password == password);
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