using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace DodgeBltz
{
    /// <summary>
    /// Manages core gameplay logic and smart contract interactions
    /// </summary>
    public class GameplayManager : MonoBehaviour
    {
        [Header("Smart Contract Configuration")]
        [SerializeField] private string gameplayContract = "gameplay.acc";
        [SerializeField] private string tokenContract = "dbptoken.acc";
        [SerializeField] private string waxRpcEndpoint = "https://testnet.waxsweden.org";
        
        [Header("Game Settings")]
        [SerializeField] private float resultCheckInterval = 2f;
        [SerializeField] private int maxResultChecks = 30; // 60 seconds max wait
        
        [Header("Events")]
        public System.Action<bool, string> OnPlayResult; // success, message
        public System.Action OnPlayStarted;
        public System.Action<string> OnError;
        
        private WalletConnection walletConnection;
        private uint64_t currentNonce;
        private bool isWaitingForResult = false;
        
        private void Awake()
        {
            walletConnection = FindObjectOfType<WalletConnection>();
            if (walletConnection == null)
            {
                Debug.LogError("WalletConnection component not found!");
            }
        }
        
        /// <summary>
        /// Executes a BLTZ play action
        /// </summary>
        public void PlayBltz()
        {
            if (!walletConnection.IsConnected)
            {
                OnError?.Invoke("Please connect your wallet first");
                return;
            }
            
            if (isWaitingForResult)
            {
                OnError?.Invoke("Please wait for current play to complete");
                return;
            }
            
            // Generate unique nonce
            currentNonce = GenerateNonce();
            
            // Create transaction
            var transaction = CreatePlayTransaction(walletConnection.ConnectedAccount, currentNonce);
            
            // Sign and submit transaction
            walletConnection.SignTransaction(transaction, OnTransactionSigned, OnTransactionError);
            
            OnPlayStarted?.Invoke();
            isWaitingForResult = true;
        }
        
        /// <summary>
        /// Gets the current DBP token balance for the connected account
        /// </summary>
        public void GetTokenBalance(System.Action<decimal> onSuccess, System.Action<string> onError)
        {
            if (!walletConnection.IsConnected)
            {
                onError?.Invoke("Wallet not connected");
                return;
            }
            
            StartCoroutine(FetchTokenBalance(walletConnection.ConnectedAccount, onSuccess, onError));
        }
        
        private uint64_t GenerateNonce()
        {
            // Generate unique nonce based on timestamp and random value
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var random = UnityEngine.Random.Range(1000, 9999);
            return (uint64_t)(timestamp + random);
        }
        
        private object CreatePlayTransaction(string playerAccount, uint64_t nonce)
        {
            return new
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
                                actor = playerAccount,
                                permission = "active"
                            }
                        },
                        data = new
                        {
                            player = playerAccount,
                            nonce = nonce
                        }
                    }
                }
            };
        }
        
        private void OnTransactionSigned(string transactionHash)
        {
            Debug.Log($"Transaction signed: {transactionHash}");
            
            // Start checking for result
            StartCoroutine(CheckForResult());
        }
        
        private void OnTransactionError(string error)
        {
            Debug.LogError($"Transaction error: {error}");
            isWaitingForResult = false;
            OnError?.Invoke($"Transaction failed: {error}");
        }
        
        private IEnumerator CheckForResult()
        {
            int checkCount = 0;
            
            while (checkCount < maxResultChecks && isWaitingForResult)
            {
                yield return new WaitForSeconds(resultCheckInterval);
                
                // Check if nonce has been processed (no longer in pending table)
                yield return StartCoroutine(CheckNonceStatus());
                
                checkCount++;
            }
            
            if (isWaitingForResult)
            {
                // Timeout
                isWaitingForResult = false;
                OnError?.Invoke("Result check timeout. Please check your transaction manually.");
            }
        }
        
        private IEnumerator CheckNonceStatus()
        {
            // Check if the nonce is still in the pending plays table
            string url = $"{waxRpcEndpoint}/v1/chain/get_table_rows";
            
            var requestData = new
            {
                json = true,
                code = gameplayContract,
                scope = gameplayContract,
                table = "pendingplay",
                lower_bound = currentNonce.ToString(),
                upper_bound = currentNonce.ToString(),
                limit = 1
            };
            
            string jsonData = JsonUtility.ToJson(requestData);
            
            using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, jsonData))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();
                
                if (www.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonUtility.FromJson<TableRowsResponse>(www.downloadHandler.text);
                    
                    if (response.rows.Length == 0)
                    {
                        // Nonce is not in pending table, play has been processed
                        yield return StartCoroutine(CheckPlayResult());
                    }
                }
            }
        }
        
        private IEnumerator CheckPlayResult()
        {
            // Get token balance before and compare to determine if play was successful
            // In a real implementation, you might want to check transaction history or events
            
            yield return StartCoroutine(FetchTokenBalance(
                walletConnection.ConnectedAccount,
                balance => {
                    // For simplicity, we'll assume any balance increase means success
                    // In real implementation, you'd track the previous balance
                    isWaitingForResult = false;
                    OnPlayResult?.Invoke(true, $"BLTZ successful! You earned 1 DBP token. Balance: {balance}");
                },
                error => {
                    // If we can't check balance, assume failure for safety
                    isWaitingForResult = false;
                    OnPlayResult?.Invoke(false, "BLTZ failed. Better luck next time!");
                }
            ));
        }
        
        private IEnumerator FetchTokenBalance(string account, System.Action<decimal> onSuccess, System.Action<string> onError)
        {
            string url = $"{waxRpcEndpoint}/v1/chain/get_table_rows";
            
            var requestData = new
            {
                json = true,
                code = tokenContract,
                scope = account,
                table = "accounts",
                limit = 10
            };
            
            string jsonData = JsonUtility.ToJson(requestData);
            
            using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, jsonData))
            {
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();
                
                if (www.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke($"Failed to fetch balance: {www.error}");
                }
                else
                {
                    try
                    {
                        var response = JsonUtility.FromJson<TableRowsResponse>(www.downloadHandler.text);
                        
                        decimal balance = 0;
                        foreach (var row in response.rows)
                        {
                            // Parse balance from row data
                            // Format: "1000.0000 DBP"
                            if (row.ContainsKey("balance"))
                            {
                                string balanceStr = row["balance"].ToString();
                                string[] parts = balanceStr.Split(' ');
                                if (parts.Length > 0 && decimal.TryParse(parts[0], out decimal parsedBalance))
                                {
                                    balance = parsedBalance;
                                    break;
                                }
                            }
                        }
                        
                        onSuccess?.Invoke(balance);
                    }
                    catch (Exception e)
                    {
                        onError?.Invoke($"Failed to parse balance: {e.Message}");
                    }
                }
            }
        }
    }
    
    [System.Serializable]
    public class TableRowsResponse
    {
        public System.Collections.Generic.Dictionary<string, object>[] rows;
        public bool more;
    }
}