using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace DodgeBltz
{
    /// <summary>
    /// Handles WAX Cloud Wallet authentication and connection
    /// </summary>
    public class WalletConnection : MonoBehaviour
    {
        [Header("WAX Configuration")]
        [SerializeField] private string waxCloudWalletUrl = "https://all-access.wax.io";
        [SerializeField] private string waxRpcEndpoint = "https://testnet.waxsweden.org";
        
        [Header("Events")]
        public System.Action<string> OnWalletConnected;
        public System.Action OnWalletDisconnected;
        public System.Action<string> OnConnectionError;
        
        // Current wallet state
        public bool IsConnected { get; private set; }
        public string ConnectedAccount { get; private set; }
        
        private void Start()
        {
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
            
            StartCoroutine(InitiateWalletLogin());
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
            
            // Clear stored session data
            PlayerPrefs.DeleteKey("wax_account");
            PlayerPrefs.DeleteKey("wax_session");
            
            IsConnected = false;
            ConnectedAccount = string.Empty;
            
            OnWalletDisconnected?.Invoke();
            Debug.Log("Wallet disconnected");
        }
        
        /// <summary>
        /// Signs a transaction using WAX Cloud Wallet
        /// </summary>
        public void SignTransaction(object transaction, System.Action<string> onSuccess, System.Action<string> onError)
        {
            if (!IsConnected)
            {
                onError?.Invoke("Wallet not connected");
                return;
            }
            
            StartCoroutine(PerformTransactionSigning(transaction, onSuccess, onError));
        }
        
        private IEnumerator InitiateWalletLogin()
        {
            Debug.Log("Initiating WAX Cloud Wallet login...");
            
            // In a real implementation, this would open the WAX Cloud Wallet login page
            // For this demo, we'll simulate the login process
            
            // Open WAX Cloud Wallet in browser or embedded view
            string loginUrl = $"{waxCloudWalletUrl}/cloud-wallet/login/";
            Application.OpenURL(loginUrl);
            
            // Wait for user to complete login (in real implementation, this would be handled by callback)
            yield return new WaitForSeconds(2f);
            
            // Simulate successful login for demo purposes
            // In real implementation, this would come from the wallet callback
            SimulateSuccessfulLogin("testuser.wam");
        }
        
        private void SimulateSuccessfulLogin(string accountName)
        {
            // Store session information
            PlayerPrefs.SetString("wax_account", accountName);
            PlayerPrefs.SetString("wax_session", "demo_session_token");
            
            ConnectedAccount = accountName;
            IsConnected = true;
            
            OnWalletConnected?.Invoke(accountName);
            Debug.Log($"Wallet connected: {accountName}");
        }
        
        private void CheckExistingSession()
        {
            string savedAccount = PlayerPrefs.GetString("wax_account", "");
            string savedSession = PlayerPrefs.GetString("wax_session", "");
            
            if (!string.IsNullOrEmpty(savedAccount) && !string.IsNullOrEmpty(savedSession))
            {
                // Validate session is still active
                StartCoroutine(ValidateSession(savedAccount, savedSession));
            }
        }
        
        private IEnumerator ValidateSession(string account, string session)
        {
            // In real implementation, validate session with WAX Cloud Wallet
            yield return new WaitForSeconds(1f);
            
            // For demo, assume session is valid
            ConnectedAccount = account;
            IsConnected = true;
            OnWalletConnected?.Invoke(account);
            Debug.Log($"Restored wallet session: {account}");
        }
        
        private IEnumerator PerformTransactionSigning(object transaction, System.Action<string> onSuccess, System.Action<string> onError)
        {
            Debug.Log("Signing transaction with WAX Cloud Wallet...");
            
            // In real implementation, this would communicate with WAX Cloud Wallet
            // to sign the transaction and return the signed transaction
            
            yield return new WaitForSeconds(1.5f);
            
            // Simulate successful signing
            string signedTransaction = "signed_transaction_hash_demo";
            onSuccess?.Invoke(signedTransaction);
            
            Debug.Log("Transaction signed successfully");
        }
        
        /// <summary>
        /// Gets the current account information
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
    
    [System.Serializable]
    public class AccountInfo
    {
        public string account_name;
        public string created;
        // Add other fields as needed
    }
}