mergeInto(LibraryManager.library, {

    WaxCloudWallet_Init: function(rpcEndpoint) {
        var rpcUrl = UTF8ToString(rpcEndpoint);
        
        window.waxWallet = {
            wax: null,
            account: null,
            rpcEndpoint: rpcUrl,
            
            init: async function() {
                try {
                    // Initialize waxjs
                    const { WaxJS } = await import('https://unpkg.com/@waxio/waxjs@latest/dist/waxjs.js');
                    this.wax = new WaxJS(this.rpcEndpoint, null, null, false);
                    console.log('WAX SDK initialized');
                } catch (error) {
                    console.error('Failed to initialize WAX SDK:', error);
                }
            }
        };
        
        // Initialize WAX SDK
        window.waxWallet.init();
    },

    WaxCloudWallet_Login: function(gameObjectPtr, callbackMethodPtr) {
        var gameObjectName = UTF8ToString(gameObjectPtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);
        
        if (!window.waxWallet || !window.waxWallet.wax) {
            console.error('WAX SDK not initialized');
            SendMessage(gameObjectName, callbackMethod, 'ERROR:WAX SDK not initialized');
            return;
        }
        
        window.waxWallet.wax.login().then(function(userAccount) {
            window.waxWallet.account = userAccount;
            console.log('WAX login successful:', userAccount);
            SendMessage(gameObjectName, callbackMethod, userAccount);
        }).catch(function(error) {
            console.error('WAX login failed:', error);
            SendMessage(gameObjectName, callbackMethod, 'ERROR:' + error.message);
        });
    },

    WaxCloudWallet_SignTransaction: function(gameObjectPtr, callbackMethodPtr, transactionJsonPtr) {
        var gameObjectName = UTF8ToString(gameObjectPtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);
        var transactionJson = UTF8ToString(transactionJsonPtr);
        
        if (!window.waxWallet || !window.waxWallet.wax || !window.waxWallet.account) {
            console.error('WAX not connected');
            SendMessage(gameObjectName, callbackMethod, 'ERROR:WAX not connected');
            return;
        }
        
        try {
            var transaction = JSON.parse(transactionJson);
            
            // Create proper WAX transaction structure
            var waxTransaction = {
                actions: [{
                    account: 'gameplayacc2',
                    name: 'play',
                    authorization: [{
                        actor: window.waxWallet.account,
                        permission: 'active'
                    }],
                    data: {
                        player: transaction.player,
                        nonce: transaction.nonce
                    }
                }]
            };
            
            window.waxWallet.wax.api.transact(waxTransaction, {
                blocksBehind: 3,
                expireSeconds: 30
            }).then(function(result) {
                console.log('Transaction signed successfully:', result);
                SendMessage(gameObjectName, callbackMethod, JSON.stringify(result));
            }).catch(function(error) {
                console.error('Transaction signing failed:', error);
                SendMessage(gameObjectName, callbackMethod, 'ERROR:' + error.message);
            });
            
        } catch (parseError) {
            console.error('Failed to parse transaction:', parseError);
            SendMessage(gameObjectName, callbackMethod, 'ERROR:Invalid transaction format');
        }
    },

    WaxCloudWallet_GetAccount: function() {
        if (window.waxWallet && window.waxWallet.account) {
            var bufferSize = lengthBytesUTF8(window.waxWallet.account) + 1;
            var buffer = _malloc(bufferSize);
            stringToUTF8(window.waxWallet.account, buffer, bufferSize);
            return buffer;
        }
        return null;
    },

    WaxCloudWallet_Logout: function() {
        if (window.waxWallet) {
            window.waxWallet.account = null;
            console.log('WAX wallet logged out');
        }
    }

});