using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DodgeBltz
{
    /// <summary>
    /// Manages the UI states and transitions for Dodge BLTZ
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private GameObject resolvingPanel;
        [SerializeField] private GameObject resultPanel;
        
        [Header("Start Panel Components")]
        [SerializeField] private Button connectWalletButton;
        [SerializeField] private Button playBltzButton;
        [SerializeField] private TextMeshProUGUI accountText;
        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private TextMeshProUGUI statusText;
        
        [Header("Resolving Panel Components")]
        [SerializeField] private TextMeshProUGUI resolvingText;
        [SerializeField] private Image loadingSpinner;
        [SerializeField] private Button cancelButton;
        
        [Header("Result Panel Components")]
        [SerializeField] private TextMeshProUGUI resultTitleText;
        [SerializeField] private TextMeshProUGUI resultMessageText;
        [SerializeField] private TextMeshProUGUI newBalanceText;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button disconnectButton;
        [SerializeField] private Image resultIcon;
        [SerializeField] private Sprite successIcon;
        [SerializeField] private Sprite failureIcon;
        
        [Header("Animation Settings")]
        [SerializeField] private float spinnerSpeed = 360f;
        
        private WalletConnection walletConnection;
        private GameplayManager gameplayManager;
        private GameState currentState = GameState.Start;
        
        private enum GameState
        {
            Start,
            Resolving,
            Result
        }
        
        private void Awake()
        {
            walletConnection = FindObjectOfType<WalletConnection>();
            gameplayManager = FindObjectOfType<GameplayManager>();
            
            SetupEventListeners();
            SetGameState(GameState.Start);
        }
        
        private void Start()
        {
            UpdateUI();
        }
        
        private void Update()
        {
            // Animate loading spinner
            if (currentState == GameState.Resolving && loadingSpinner != null)
            {
                loadingSpinner.transform.Rotate(0, 0, -spinnerSpeed * Time.deltaTime);
            }
        }
        
        private void SetupEventListeners()
        {
            // Wallet events
            if (walletConnection != null)
            {
                walletConnection.OnWalletConnected += OnWalletConnected;
                walletConnection.OnWalletDisconnected += OnWalletDisconnected;
                walletConnection.OnConnectionError += OnConnectionError;
            }
            
            // Gameplay events
            if (gameplayManager != null)
            {
                gameplayManager.OnPlayStarted += OnPlayStarted;
                gameplayManager.OnPlayResult += OnPlayResult;
                gameplayManager.OnError += OnGameplayError;
            }
            
            // Button events
            if (connectWalletButton != null)
                connectWalletButton.onClick.AddListener(ConnectWallet);
            
            if (playBltzButton != null)
                playBltzButton.onClick.AddListener(PlayBltz);
            
            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(PlayAgain);
            
            if (disconnectButton != null)
                disconnectButton.onClick.AddListener(DisconnectWallet);
            
            if (cancelButton != null)
                cancelButton.onClick.AddListener(CancelPlay);
        }
        
        private void SetGameState(GameState newState)
        {
            currentState = newState;
            
            // Hide all panels
            if (startPanel != null) startPanel.SetActive(false);
            if (resolvingPanel != null) resolvingPanel.SetActive(false);
            if (resultPanel != null) resultPanel.SetActive(false);
            
            // Show appropriate panel
            switch (newState)
            {
                case GameState.Start:
                    if (startPanel != null) startPanel.SetActive(true);
                    break;
                case GameState.Resolving:
                    if (resolvingPanel != null) resolvingPanel.SetActive(true);
                    break;
                case GameState.Result:
                    if (resultPanel != null) resultPanel.SetActive(true);
                    break;
            }
            
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            UpdateStartPanel();
            UpdateResolvingPanel();
        }
        
        private void UpdateStartPanel()
        {
            if (currentState != GameState.Start) return;
            
            bool isConnected = walletConnection != null && walletConnection.IsConnected;
            
            // Update button states
            if (connectWalletButton != null)
            {
                connectWalletButton.gameObject.SetActive(!isConnected);
            }
            
            if (playBltzButton != null)
            {
                playBltzButton.gameObject.SetActive(isConnected);
                playBltzButton.interactable = isConnected;
            }
            
            // Update account info
            if (accountText != null)
            {
                if (isConnected)
                {
                    accountText.text = $"Connected: {walletConnection.ConnectedAccount}";
                }
                else
                {
                    accountText.text = "Not Connected";
                }
            }
            
            // Update balance
            if (isConnected && gameplayManager != null)
            {
                gameplayManager.GetTokenBalance(
                    balance => {
                        if (balanceText != null)
                            balanceText.text = $"DBP Balance: {balance:F4}";
                    },
                    error => {
                        if (balanceText != null)
                            balanceText.text = "Balance: Error loading";
                    }
                );
            }
            else if (balanceText != null)
            {
                balanceText.text = "DBP Balance: --";
            }
            
            // Update status
            if (statusText != null)
            {
                if (isConnected)
                {
                    statusText.text = "Ready to play BLTZ! 35% chance to win 1 DBP";
                }
                else
                {
                    statusText.text = "Connect your WAX wallet to start playing";
                }
            }
        }
        
        private void UpdateResolvingPanel()
        {
            if (currentState != GameState.Resolving) return;
            
            if (resolvingText != null)
            {
                resolvingText.text = "Processing your BLTZ...\nWaiting for WAX RNG Oracle result";
            }
        }
        
        private void ShowResult(bool success, string message, decimal newBalance)
        {
            SetGameState(GameState.Result);
            
            if (resultTitleText != null)
            {
                resultTitleText.text = success ? "BLTZ SUCCESS!" : "BLTZ FAILED";
                resultTitleText.color = success ? Color.green : Color.red;
            }
            
            if (resultMessageText != null)
            {
                resultMessageText.text = message;
            }
            
            if (newBalanceText != null)
            {
                newBalanceText.text = $"DBP Balance: {newBalance:F4}";
            }
            
            if (resultIcon != null)
            {
                resultIcon.sprite = success ? successIcon : failureIcon;
            }
        }
        
        // Event handlers
        private void OnWalletConnected(string account)
        {
            Debug.Log($"UI: Wallet connected - {account}");
            UpdateUI();
        }
        
        private void OnWalletDisconnected()
        {
            Debug.Log("UI: Wallet disconnected");
            SetGameState(GameState.Start);
        }
        
        private void OnConnectionError(string error)
        {
            Debug.LogError($"UI: Connection error - {error}");
            if (statusText != null)
            {
                statusText.text = $"Connection Error: {error}";
            }
        }
        
        private void OnPlayStarted()
        {
            Debug.Log("UI: Play started");
            SetGameState(GameState.Resolving);
        }
        
        private void OnPlayResult(bool success, string message)
        {
            Debug.Log($"UI: Play result - Success: {success}, Message: {message}");
            
            // Get updated balance and show result
            if (gameplayManager != null)
            {
                gameplayManager.GetTokenBalance(
                    balance => ShowResult(success, message, balance),
                    error => ShowResult(success, message, 0)
                );
            }
            else
            {
                ShowResult(success, message, 0);
            }
        }
        
        private void OnGameplayError(string error)
        {
            Debug.LogError($"UI: Gameplay error - {error}");
            SetGameState(GameState.Start);
            if (statusText != null)
            {
                statusText.text = $"Error: {error}";
            }
        }
        
        // Button handlers
        private void ConnectWallet()
        {
            if (walletConnection != null)
            {
                walletConnection.ConnectWallet();
            }
        }
        
        private void PlayBltz()
        {
            if (gameplayManager != null)
            {
                gameplayManager.PlayBltz();
            }
        }
        
        private void PlayAgain()
        {
            SetGameState(GameState.Start);
        }
        
        private void DisconnectWallet()
        {
            if (walletConnection != null)
            {
                walletConnection.DisconnectWallet();
            }
        }
        
        private void CancelPlay()
        {
            // Note: In real implementation, you might not be able to cancel
            // once the transaction is submitted to the blockchain
            SetGameState(GameState.Start);
        }
        
        private void OnDestroy()
        {
            // Clean up event listeners
            if (walletConnection != null)
            {
                walletConnection.OnWalletConnected -= OnWalletConnected;
                walletConnection.OnWalletDisconnected -= OnWalletDisconnected;
                walletConnection.OnConnectionError -= OnConnectionError;
            }
            
            if (gameplayManager != null)
            {
                gameplayManager.OnPlayStarted -= OnPlayStarted;
                gameplayManager.OnPlayResult -= OnPlayResult;
                gameplayManager.OnError -= OnGameplayError;
            }
        }
    }
}