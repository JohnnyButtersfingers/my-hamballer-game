using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace DodgeBLTZ
{
    public class WalletConnection : MonoBehaviour
    {
        // Events
        public static event Action<string> OnAccountLoggedIn;
        public static event Action OnAccountLoggedOut;
        public static event Action<string> OnLoginError;

        // WAX Cloud Wallet endpoints
        private const string WAX_CLOUD_WALLET_URL = "https://www.mycloudwallet.com";
        private const string WAX_TESTNET_ENDPOINT = "https://testnet.wax.pink.gg";
        
        // Current user state
        private string currentAccount;
        private bool isLoggedIn;

        // Singleton instance
        private static WalletConnection instance;
        public static WalletConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<WalletConnection>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("WalletConnection");
                        instance = go.AddComponent<WalletConnection>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initiates WAX Cloud Wallet login
        /// </summary>
        public void Login()
        {
            if (isLoggedIn)
            {
                Debug.LogWarning("Already logged in");
                return;
            }

            // In a real implementation, this would open the WAX Cloud Wallet
            // For now, we'll simulate the login process
            StartCoroutine(SimulateLogin());
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        public void Logout()
        {
            if (!isLoggedIn)
            {
                Debug.LogWarning("Not logged in");
                return;
            }

            currentAccount = null;
            isLoggedIn = false;
            
            OnAccountLoggedOut?.Invoke();
            Debug.Log("Logged out successfully");
        }

        /// <summary>
        /// Gets the current logged-in account name
        /// </summary>
        public string GetAccount()
        {
            return currentAccount;
        }

        /// <summary>
        /// Checks if a user is currently logged in
        /// </summary>
        public bool IsLoggedIn()
        {
            return isLoggedIn;
        }

        /// <summary>
        /// Signs a transaction with the user's account
        /// </summary>
        public IEnumerator SignTransaction(TransactionData transaction, Action<string> onSuccess, Action<string> onError)
        {
            if (!isLoggedIn)
            {
                onError?.Invoke("Not logged in");
                yield break;
            }

            // Simulate transaction signing
            yield return new WaitForSeconds(1f);

            // In a real implementation, this would interact with WAX Cloud Wallet
            string signedTransaction = JsonConvert.SerializeObject(new
            {
                transaction = transaction,
                signature = "simulated_signature_" + UnityEngine.Random.Range(1000, 9999),
                account = currentAccount
            });

            onSuccess?.Invoke(signedTransaction);
        }

        /// <summary>
        /// Submits a signed transaction to the blockchain
        /// </summary>
        public IEnumerator SubmitTransaction(string signedTransaction, Action<TransactionResult> onSuccess, Action<string> onError)
        {
            var request = new UnityWebRequest(WAX_TESTNET_ENDPOINT + "/v1/chain/push_transaction", "POST");
            
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(signedTransaction);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(request.error);
            }
            else
            {
                try
                {
                    var result = JsonConvert.DeserializeObject<TransactionResult>(request.downloadHandler.text);
                    onSuccess?.Invoke(result);
                }
                catch (Exception e)
                {
                    onError?.Invoke("Failed to parse transaction result: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Simulates the login process (replace with actual WAX Cloud Wallet integration)
        /// </summary>
        private IEnumerator SimulateLogin()
        {
            Debug.Log("Initiating WAX Cloud Wallet login...");
            
            // Simulate network delay
            yield return new WaitForSeconds(2f);

            // Simulate successful login
            currentAccount = "testuser" + UnityEngine.Random.Range(1000, 9999);
            isLoggedIn = true;

            OnAccountLoggedIn?.Invoke(currentAccount);
            Debug.Log($"Logged in as: {currentAccount}");
        }
    }

    /// <summary>
    /// Transaction data structure
    /// </summary>
    [Serializable]
    public class TransactionData
    {
        public string account;
        public string name;
        public object data;
        public string[] authorization;
    }

    /// <summary>
    /// Transaction result structure
    /// </summary>
    [Serializable]
    public class TransactionResult
    {
        public string transaction_id;
        public bool processed;
        public string block_num;
        public string block_time;
    }
}