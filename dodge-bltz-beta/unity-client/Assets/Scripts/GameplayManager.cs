using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace DodgeBLTZ
{
    public class GameplayManager : MonoBehaviour
    {
        // Game state enum
        public enum GameState
        {
            Start,
            Playing,
            Resolving,
            Result
        }

        // Events
        public static event Action<GameState> OnGameStateChanged;
        public static event Action<bool, int> OnGameResult; // won, DBP earned

        // Contract configuration - using GameConfig for consistency
        private const string GAMEPLAY_CONTRACT = GameConfig.GAMEPLAY_CONTRACT;
        private const string TOKEN_CONTRACT = GameConfig.TOKEN_CONTRACT;

        // UI References (to be assigned in Unity Inspector)
        [Header("UI Elements")]
        [SerializeField] private GameObject startScreen;
        [SerializeField] private GameObject resolvingScreen;
        [SerializeField] private GameObject resultScreen;
        [SerializeField] private Button playButton;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Text accountText;
        [SerializeField] private Text resultText;
        [SerializeField] private Text tokenRewardText;
        [SerializeField] private Text statusText; // For showing connection/loading status
        [SerializeField] private Text gameInfoText; // For showing game rules/info

        // Game state
        private GameState currentState;
        private string lastTransactionId;
        private bool isProcessing;

        // Singleton
        private static GameplayManager instance;
        public static GameplayManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameplayManager>();
                }
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            // Subscribe to wallet events
            WalletConnection.OnAccountLoggedIn += OnWalletLoggedIn;
            WalletConnection.OnAccountLoggedOut += OnWalletLoggedOut;

            // Setup UI buttons
            if (loginButton) loginButton.onClick.AddListener(OnLoginClicked);
            if (playButton) playButton.onClick.AddListener(OnPlayClicked);
            if (playAgainButton) playAgainButton.onClick.AddListener(OnPlayAgainClicked);

            // Initialize game state
            SetGameState(GameState.Start);
            UpdateUI();
        }

        void OnDestroy()
        {
            // Unsubscribe from events
            WalletConnection.OnAccountLoggedIn -= OnWalletLoggedIn;
            WalletConnection.OnAccountLoggedOut -= OnWalletLoggedOut;
        }

        /// <summary>
        /// Generates a unique nonce for replay protection
        /// </summary>
        private string GenerateNonce()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int random = UnityEngine.Random.Range(100000, 999999);
            return $"{timestamp}_{random}";
        }

        /// <summary>
        /// Initiates a game play
        /// </summary>
        public void PlayGame()
        {
            if (isProcessing || !WalletConnection.Instance.IsLoggedIn())
            {
                Debug.LogError("Cannot play: either processing or not logged in");
                return;
            }

            StartCoroutine(PlayGameCoroutine());
        }

        private IEnumerator PlayGameCoroutine()
        {
            isProcessing = true;
            SetGameState(GameState.Playing);

            string account = WalletConnection.Instance.GetAccount();
            string nonce = GenerateNonce();

            Debug.Log($"Playing game with nonce: {nonce}");

            // Create transaction data
            var transactionData = new TransactionData
            {
                account = GAMEPLAY_CONTRACT,
                name = "play",
                data = new
                {
                    player = account,
                    nonce = nonce
                },
                authorization = new string[] { $"{account}@active" }
            };

            // Sign transaction
            bool transactionSigned = false;
            string signedTransaction = null;
            string signError = null;

            yield return WalletConnection.Instance.SignTransaction(
                transactionData,
                (signed) => { signedTransaction = signed; transactionSigned = true; },
                (error) => { signError = error; }
            );

            if (!transactionSigned)
            {
                Debug.LogError($"Failed to sign transaction: {signError}");
                SetGameState(GameState.Start);
                isProcessing = false;
                yield break;
            }

            // Submit transaction
            SetGameState(GameState.Resolving);

            bool transactionSubmitted = false;
            TransactionResult txResult = null;
            string submitError = null;

            yield return WalletConnection.Instance.SubmitTransaction(
                signedTransaction,
                (result) => { txResult = result; transactionSubmitted = true; },
                (error) => { submitError = error; }
            );

            if (!transactionSubmitted)
            {
                Debug.LogError($"Failed to submit transaction: {submitError}");
                SetGameState(GameState.Start);
                isProcessing = false;
                yield break;
            }

            lastTransactionId = txResult.transaction_id;
            Debug.Log($"Transaction submitted: {lastTransactionId}");

            // Wait for blockchain confirmation and RNG result
            yield return new WaitForSeconds(3f);

            // Simulate getting the result (in production, poll blockchain for result)
            yield return GetGameResult();

            isProcessing = false;
        }

        /// <summary>
        /// Gets the game result from blockchain
        /// TODO: Replace with actual blockchain polling for RNG Oracle result
        /// </summary>
        private IEnumerator GetGameResult()
        {
            // TODO: In production, query WAX blockchain for RNG Oracle result
            // Should poll /v1/chain/get_table_rows for pending games table
            // Wait for Oracle to call back with random value
            
            Debug.Log("Waiting for RNG Oracle result...");
            yield return new WaitForSeconds(2f);

            // Temporary simulation - REMOVE for production
            // Real implementation should get result from blockchain state
            bool won = UnityEngine.Random.Range(0, 100) < GameConfig.WIN_CHANCE_PERCENT;
            int tokensWon = won ? GameConfig.DBP_REWARD_AMOUNT : 0;
            
            Debug.Log($"Game result: {(won ? "WIN" : "LOSS")}, DBP earned: {tokensWon}");
            OnGameResult?.Invoke(won, tokensWon);
            
            SetGameState(GameState.Result);
            UpdateResultUI(won, tokensWon);
        }

        /// <summary>
        /// Updates the game state
        /// </summary>
        private void SetGameState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged?.Invoke(newState);
            UpdateUI();
        }

        /// <summary>
        /// Updates UI based on current state
        /// </summary>
        private void UpdateUI()
        {
            // Hide all screens
            if (startScreen) startScreen.SetActive(false);
            if (resolvingScreen) resolvingScreen.SetActive(false);
            if (resultScreen) resultScreen.SetActive(false);

            // Show current screen
            switch (currentState)
            {
                case GameState.Start:
                    if (startScreen) startScreen.SetActive(true);
                    UpdateStartScreenUI();
                    break;
                    
                case GameState.Playing:
                case GameState.Resolving:
                    if (resolvingScreen) resolvingScreen.SetActive(true);
                    break;
                    
                case GameState.Result:
                    if (resultScreen) resultScreen.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// Updates the start screen UI
        /// </summary>
        private void UpdateStartScreenUI()
        {
            bool isLoggedIn = WalletConnection.Instance.IsLoggedIn();
            
            if (playButton) 
            {
                playButton.interactable = isLoggedIn && !isProcessing;
                // TODO: Apply UIColorPalette colors to button components in Unity Inspector
            }
            
            if (loginButton) 
            {
                loginButton.gameObject.SetActive(!isLoggedIn);
                // TODO: Apply UIColorPalette.ButtonPrimary color in Unity Inspector
            }
            
            if (accountText)
            {
                if (isLoggedIn)
                {
                    accountText.text = $"👤 {WalletConnection.Instance.GetAccount()}";
                    accountText.color = UIColorPalette.Connected;
                }
                else
                {
                    accountText.text = "Connect WAX Wallet to play";
                    accountText.color = UIColorPalette.TextSecondary;
                }
            }
            
            // Update game info display
            if (gameInfoText)
            {
                gameInfoText.text = $"35% chance to win {GameConfig.DBP_REWARD_AMOUNT} DBP";
                gameInfoText.color = UIColorPalette.TextSecondary;
            }
            
            // Update status text
            if (statusText)
            {
                if (isProcessing)
                {
                    statusText.text = "Processing...";
                    statusText.color = UIColorPalette.Processing;
                }
                else if (isLoggedIn)
                {
                    statusText.text = "Ready to play!";
                    statusText.color = UIColorPalette.Connected;
                }
                else
                {
                    statusText.text = "Wallet connection required";
                    statusText.color = UIColorPalette.Disconnected;
                }
            }
        }

        /// <summary>
        /// Updates the result screen UI
        /// </summary>
        private void UpdateResultUI(bool won, int tokensWon)
        {
            if (resultText)
            {
                // Final branding: DODGE BLTZ BETA
                resultText.text = won ? "BLTZ SUCCESSFUL!" : "BLTZ FAILED";
                resultText.color = won ? UIColorPalette.Success : UIColorPalette.Error;
            }

            if (tokenRewardText)
            {
                tokenRewardText.text = won ? $"You earned {tokensWon}.0000 DBP!" : "Better luck next time!";
                tokenRewardText.gameObject.SetActive(true); // Show encouragement even on loss
                tokenRewardText.color = won ? UIColorPalette.Success : UIColorPalette.TextSecondary;
            }
        }

        // UI Event Handlers
        private void OnLoginClicked()
        {
            WalletConnection.Instance.Login();
        }

        private void OnPlayClicked()
        {
            PlayGame();
        }

        private void OnPlayAgainClicked()
        {
            SetGameState(GameState.Start);
        }

        // Wallet Event Handlers
        private void OnWalletLoggedIn(string account)
        {
            Debug.Log($"Wallet logged in: {account}");
            UpdateUI();
        }

        private void OnWalletLoggedOut()
        {
            Debug.Log("Wallet logged out");
            UpdateUI();
        }
    }
}