using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

namespace DodgeBltz
{
    /// <summary>
    /// Handles WAX Cloud Wallet authentication and connection using waxjs SDK
    /// </summary>
    public class WalletConnection : MonoBehaviour
    {
        [Header("WAX Configuration")]
        [SerializeField] private string waxCloudWalletUrl = "https://all-access.wax.io";
        [SerializeField] private string waxRpcEndpoint = "https://testnet.waxsweden.org";
        [SerializeField] private string gameplayContract = "gameplayacc2"; // WAX testnet contract
        
        [Header("Events")]
        public System.Action<string> OnWalletConnected;
        public System.Action OnWalletDisconnected;
        public System.Action<string> OnConnectionError;
        
        // Current wallet state
        public bool IsConnected { get; private set; }
        public string ConnectedAccount { get; private set; }
        
        // JavaScript bridge for WebGL
        [DllImport("__Internal")]
        private static extern void WaxCloudWallet_Init(string rpcEndpoint);
        
        [DllImport("__Internal")]
        private static extern void WaxCloudWallet_Login(string gameObject, string callbackMethod);
        
        [DllImport("__Internal")]
        private static extern void WaxCloudWallet_SignTransaction(string gameObject, string callbackMethod, string transactionJson);
        
        [DllImport("__Internal")]
        private static extern string WaxCloudWallet_GetAccount();
        
        [DllImport("__Internal")]
        private static extern void WaxCloudWallet_Logout();

        private void Start()
        {
            // Initialize WAX SDK in WebGL builds
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                WaxCloudWallet_Init(waxRpcEndpoint);
            }
            
            // Check for existing session
            CheckExistingSession();
        }
        
        /// <summary>
        /// Initiates WAX Cloud Wallet login process
        /// </summary>
        public void ConnectWallet()
        {
            if (IsConnected)
            {
                Debug.LogWarning("Wallet already connected");
                return;
            }
            
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // Use JavaScript bridge for WebGL
                WaxCloudWallet_Login(gameObject.name, "OnWaxLoginCallback");
            }
            else
            {
                // Fallback for editor/standalone testing
                StartCoroutine(InitiateWalletLogin());
            }
        }
        
        /// <summary>
        /// Disconnects from WAX Cloud Wallet
        /// </summary>
        public void DisconnectWallet()
        {
            if (!IsConnected)
            {
                Debug.LogWarning("Wallet not connected");
                return;
            }
            
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                WaxCloudWallet_Logout();
            }
            
            // Clear stored session data
            PlayerPrefs.DeleteKey("wax_account");
            PlayerPrefs.DeleteKey("wax_session");
            
            IsConnected = false;
            ConnectedAccount = string.Empty;
            
            OnWalletDisconnected?.Invoke();
            Debug.Log("Wallet disconnected");
        }
        
        /// <summary>
        /// Signs a transaction using WAX Cloud Wallet for gameplay contract
        /// </summary>
        public void SignTransaction(GameplayTransaction transaction, System.Action<string> onSuccess, System.Action<string> onError)
        {
            if (!IsConnected)
            {
                onError?.Invoke("Wallet not connected");
                return;
            }
            
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // Use JavaScript bridge for WebGL
                string transactionJson = JsonUtility.ToJson(transaction);
                WaxCloudWallet_SignTransaction(gameObject.name, "OnTransactionSignedCallback", transactionJson);
                
                // Store callbacks for later use
                _onTransactionSuccess = onSuccess;
                _onTransactionError = onError;
            }
            else
            {
                // Fallback for editor/standalone testing
                StartCoroutine(PerformTransactionSigning(transaction, onSuccess, onError));
            }
        }
        
        // Callback storage for WebGL
        private System.Action<string> _onTransactionSuccess;
        private System.Action<string> _onTransactionError;
        
        /// <summary>
        /// JavaScript callback for successful WAX login
        /// </summary>
        public void OnWaxLoginCallback(string accountName)
        {
            if (string.IsNullOrEmpty(accountName))
            {
                OnConnectionError?.Invoke("Login failed or cancelled");
                return;
            }
            
            // Store session information
            PlayerPrefs.SetString("wax_account", accountName);
            PlayerPrefs.SetString("wax_session", "wax_session_token");
            
            ConnectedAccount = accountName;
            IsConnected = true;
            
            OnWalletConnected?.Invoke(accountName);
            Debug.Log($"WAX Wallet connected: {accountName}");
        }
        
        /// <summary>
        /// JavaScript callback for transaction signing result
        /// </summary>
        public void OnTransactionSignedCallback(string result)
        {
            if (string.IsNullOrEmpty(result) || result.StartsWith("ERROR:"))
            {
                string error = result.StartsWith("ERROR:") ? result.Substring(6) : "Transaction signing failed";
                _onTransactionError?.Invoke(error);
                return;
            }
            
            _onTransactionSuccess?.Invoke(result);
            Debug.Log("Transaction signed successfully via WAX Cloud Wallet");
        }
        
        private IEnumerator InitiateWalletLogin()
        {
            Debug.Log("Initiating WAX Cloud Wallet login (fallback mode)...");
            
            // Open WAX Cloud Wallet in browser
            string loginUrl = $"{waxCloudWalletUrl}/cloud-wallet/login/";
            Application.OpenURL(loginUrl);
            
            // Wait for user to complete login
            yield return new WaitForSeconds(3f);
            
            // For testing in editor, simulate successful login
            #if UNITY_EDITOR
            SimulateSuccessfulLogin("testuser.wam");
            #else
            OnConnectionError?.Invoke("Login process requires WebGL build with WAX SDK");
            #endif
        }
        
        private void SimulateSuccessfulLogin(string accountName)
        {
            // Only for editor testing
            PlayerPrefs.SetString("wax_account", accountName);
            PlayerPrefs.SetString("wax_session", "editor_test_session");
            
            ConnectedAccount = accountName;
            IsConnected = true;
            
            OnWalletConnected?.Invoke(accountName);
            Debug.Log($"[EDITOR TEST] Wallet connected: {accountName}");
        }
        
        private void CheckExistingSession()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // Check with WAX SDK for existing session
                string savedAccount = WaxCloudWallet_GetAccount();
                if (!string.IsNullOrEmpty(savedAccount))
                {
                    ConnectedAccount = savedAccount;
                    IsConnected = true;
                    OnWalletConnected?.Invoke(savedAccount);
                    Debug.Log($"Restored WAX session: {savedAccount}");
                }
            }
            else
            {
                // Fallback session check for editor/standalone
                string savedAccount = PlayerPrefs.GetString("wax_account", "");
                string savedSession = PlayerPrefs.GetString("wax_session", "");
                
                if (!string.IsNullOrEmpty(savedAccount) && !string.IsNullOrEmpty(savedSession))
                {
                    StartCoroutine(ValidateSession(savedAccount, savedSession));
                }
            }
        }
        
        private IEnumerator ValidateSession(string account, string session)
        {
            // Validate session with WAX RPC
            yield return new WaitForSeconds(1f);
            
            // For demo, assume session is valid
            ConnectedAccount = account;
            IsConnected = true;
            OnWalletConnected?.Invoke(account);
            Debug.Log($"Restored wallet session: {account}");
        }
        
        private IEnumerator PerformTransactionSigning(GameplayTransaction transaction, System.Action<string> onSuccess, System.Action<string> onError)
        {
            Debug.Log("Signing gameplay transaction...");
            
            // Create proper WAX transaction structure for gameplay contract
            var waxTransaction = new
            {
                actions = new[]
                {
                    new
                    {
                        account = gameplayContract,
                        name = "play",
                        authorization = new[]
                        {
                            new
                            {
                                actor = ConnectedAccount,
                                permission = "active"
                            }
                        },
                        data = new
                        {
                            player = ConnectedAccount,
                            nonce = transaction.nonce
                        }
                    }
                }
            };
            
            yield return new WaitForSeconds(1.5f);
            
            // For editor testing, return success
            #if UNITY_EDITOR
            string signedTx = JsonUtility.ToJson(waxTransaction);
            onSuccess?.Invoke(signedTx);
            Debug.Log("[EDITOR TEST] Transaction signed successfully");
            #else
            onError?.Invoke("Transaction signing requires WebGL build with WAX SDK");
            #endif
        }
        
        /// <summary>
        /// Gets the current account information from WAX blockchain
        /// </summary>
        public void GetAccountInfo(System.Action<AccountInfo> onSuccess, System.Action<string> onError)
        {
            if (!IsConnected)
            {
                onError?.Invoke("Wallet not connected");
                return;
            }
            
            StartCoroutine(FetchAccountInfo(onSuccess, onError));
        }
        
        private IEnumerator FetchAccountInfo(System.Action<AccountInfo> onSuccess, System.Action<string> onError)
        {
            string url = $"{waxRpcEndpoint}/v1/chain/get_account";
            
            var requestData = new { account_name = ConnectedAccount };
            string jsonData = JsonUtility.ToJson(requestData);
            
            using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, jsonData))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                
                yield return www.SendWebRequest();
                
                if (www.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke($"Failed to fetch account info: {www.error}");
                }
                else
                {
                    try
                    {
                        AccountInfo accountInfo = JsonUtility.FromJson<AccountInfo>(www.downloadHandler.text);
                        onSuccess?.Invoke(accountInfo);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"Failed to parse account info: {e.Message}");
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Transaction structure for gameplay contract calls
    /// </summary>
    [System.Serializable]
    public class GameplayTransaction
    {
        public string player;
        public ulong nonce;
        
        public GameplayTransaction(string playerAccount, ulong gameNonce)
        {
            player = playerAccount;
            nonce = gameNonce;
        }
    }
    
    [System.Serializable]
    public class AccountInfo
    {
        public string account_name;
        public string created;
        public long ram_quota;
        public long net_weight;
        public long cpu_weight;
        // Add other fields as needed
    }
}