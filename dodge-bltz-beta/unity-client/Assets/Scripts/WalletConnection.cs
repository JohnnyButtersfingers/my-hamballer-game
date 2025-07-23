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

            // TODO: Replace with actual WAX Cloud Wallet SDK integration
            // For WebGL builds, this should open wallet popup window
            StartCoroutine(InitiateWAXLogin());
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
        /// Signs a transaction with the user's account via WAX Cloud Wallet
        /// </summary>
        public IEnumerator SignTransaction(TransactionData transaction, Action<string> onSuccess, Action<string> onError)
        {
            if (!isLoggedIn)
            {
                onError?.Invoke("Wallet not connected");
                yield break;
            }

            try
            {
                Debug.Log($"Signing transaction for account: {currentAccount}");
                
                // TODO: Replace with actual WAX Cloud Wallet transaction signing
                // This should open wallet popup for user to approve the transaction
                
                #if UNITY_WEBGL && !UNITY_EDITOR
                // WebGL-specific transaction signing
                var transactionJson = JsonConvert.SerializeObject(transaction);
                Application.ExternalEval($@"
                    console.log('Requesting transaction signature for:', {transactionJson});
                    // TODO: Integrate with WAX Cloud Wallet SDK
                ");
                #endif
                
                // Simulate signing delay
                yield return new WaitForSeconds(1.5f);
                
                // TODO: Replace with actual signed transaction from wallet
                var signedTx = new WAXSignedTransaction
                {
                    packed_trx = "simulated_packed_transaction_" + UnityEngine.Random.Range(1000, 9999),
                    signatures = new string[] { $"SIG_K1_simulated_signature_{UnityEngine.Random.Range(1000, 9999)}" },
                    packed_context_free_data = "",
                    compression = "none"
                };

                string signedTransactionJson = JsonConvert.SerializeObject(signedTx);
                onSuccess?.Invoke(signedTransactionJson);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Transaction signing failed: {e.Message}");
                onError?.Invoke($"Signing failed: {e.Message}");
            }
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
        /// Initiates WAX Cloud Wallet login process
        /// TODO: Integrate with actual WAX Cloud Wallet SDK for WebGL
        /// </summary>
        private IEnumerator InitiateWAXLogin()
        {
            Debug.Log("Initiating WAX Cloud Wallet login...");
            
            try 
            {
                // TODO: Replace with actual WAX Cloud Wallet integration
                // For WebGL builds, this should use JavaScript interop to open wallet popup
                
                #if UNITY_WEBGL && !UNITY_EDITOR
                // WebGL-specific wallet integration
                Application.ExternalEval($@"
                    window.open('{WAX_CLOUD_WALLET_URL}/login', 'wax_login', 
                    'width=400,height=600,scrollbars=yes,resizable=yes');
                ");
                #endif
                
                // Temporary simulation for development - REMOVE for production
                yield return new WaitForSeconds(2f);
                
                // TODO: Replace with actual wallet response handling
                string simulatedAccount = "testuser" + UnityEngine.Random.Range(1000, 9999);
                HandleLoginSuccess(simulatedAccount);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"WAX login failed: {e.Message}");
                OnLoginError?.Invoke(e.Message);
            }
        }
        
        /// <summary>
        /// Handles successful wallet login
        /// </summary>
        private void HandleLoginSuccess(string accountName)
        {
            currentAccount = accountName;
            isLoggedIn = true;
            
            OnAccountLoggedIn?.Invoke(currentAccount);
            Debug.Log($"WAX Wallet connected: {currentAccount}");
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
    /// WAX signed transaction structure
    /// </summary>
    [Serializable]
    public class WAXSignedTransaction
    {
        public string packed_trx;
        public string[] signatures;
        public string packed_context_free_data;
        public string compression;
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