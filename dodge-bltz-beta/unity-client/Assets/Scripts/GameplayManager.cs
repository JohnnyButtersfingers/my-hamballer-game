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

        // Contract configuration
        private const string GAMEPLAY_CONTRACT = "gameplay.acc";
        private const string TOKEN_CONTRACT = "dbptoken.acc";

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
        /// </summary>
        private IEnumerator GetGameResult()
        {
            // In production, this would query the blockchain for the transaction result
            // For now, we'll simulate the result

            yield return new WaitForSeconds(2f);

            // Simulate 35% win chance
            bool won = UnityEngine.Random.Range(0, 100) < 35;
            int tokensWon = won ? 1 : 0;

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
            
            if (playButton) playButton.interactable = isLoggedIn && !isProcessing;
            if (loginButton) loginButton.gameObject.SetActive(!isLoggedIn);
            
            if (accountText)
            {
                if (isLoggedIn)
                {
                    accountText.text = $"Account: {WalletConnection.Instance.GetAccount()}";
                }
                else
                {
                    accountText.text = "Not logged in";
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
                // TODO: Update messaging based on final brand approval (Issue #003)
                // Current options: "You dodged successfully!" vs "BLTZ SUCCESSFUL!"
                resultText.text = won ? "BLTZ SUCCESSFUL!" : "BLTZ FAILED";
                // Use centralized color palette for consistency (Issue #001)
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