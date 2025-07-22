namespace DodgeBLTZ
{
    public static class GameConfig
    {
        // Contract Accounts
        public const string TOKEN_CONTRACT = "dbptoken.acc";
        public const string GAMEPLAY_CONTRACT = "gameplay.acc";
        
        // WAX Network Configuration
        public const string WAX_TESTNET_ENDPOINT = "https://testnet.wax.pink.gg";
        public const string WAX_MAINNET_ENDPOINT = "https://wax.greymass.com";
        public const string WAX_CLOUD_WALLET_URL = "https://www.mycloudwallet.com";
        
        // Game Settings
        public const int WIN_CHANCE_PERCENT = 35;
        public const int DBP_REWARD_AMOUNT = 1;
        public const string DBP_SYMBOL = "DBP";
        public const int DBP_PRECISION = 4;
        
        // UI Settings
        public const float TRANSACTION_TIMEOUT = 30f; // seconds
        public const float RESULT_DISPLAY_TIME = 5f; // seconds
        
        // Debug Settings
        public const bool USE_TESTNET = true;
        public const bool ENABLE_DEBUG_LOGS = true;
        
        // Get current endpoint based on settings
        public static string GetEndpoint()
        {
            return USE_TESTNET ? WAX_TESTNET_ENDPOINT : WAX_MAINNET_ENDPOINT;
        }
    }
}