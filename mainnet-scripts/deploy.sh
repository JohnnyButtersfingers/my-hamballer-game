#!/bin/bash

# Deployment script for Dodge BLTZ smart contracts
# Deploys to WAX testnet

set -e

# Configuration
TOKEN_ACCOUNT="dbptoken.acc"
GAMEPLAY_ACCOUNT="gameplay.acc"
WAX_MAINNET_URL="https://wax.greymass.com"

echo "🚀 Deploying Dodge BLTZ to WAX Mainnet..."

# Check if contracts are built
if [ ! -f "../contracts/build/dbp_token.wasm" ] || [ ! -f "../contracts/build/gameplay.wasm" ]; then
    echo "❌ Contract files not found. Please run build.sh first."
    exit 1
fi

echo "🔧 Setting up cleos for WAX mainnet..."
alias cleos="cleos -u $WAX_MAINNET_URL"

echo "📦 Deploying DBP Token Contract..."
cleos set contract $TOKEN_ACCOUNT ../contracts/build dbp_token.wasm dbp_token.abi -p $TOKEN_ACCOUNT@active

echo "🎮 Deploying Gameplay Contract..."
cleos set contract $GAMEPLAY_ACCOUNT ../contracts/build gameplay.wasm gameplay.abi -p $GAMEPLAY_ACCOUNT@active

echo "🔐 Setting up permissions..."
# Allow gameplay contract to call token contract
cleos set account permission $TOKEN_ACCOUNT active --add-code

# Allow RNG Oracle to call gameplay contract
cleos set account permission $GAMEPLAY_ACCOUNT active '{"threshold":1,"keys":[{"key":"YOUR_ACTIVE_KEY","weight":1}],"accounts":[{"permission":{"actor":"orng.wax","permission":"eosio.code"},"weight":1}]}' owner -p $GAMEPLAY_ACCOUNT@owner

echo "✅ Deployment completed successfully!"
echo ""
echo "Contract Addresses:"
echo "  - DBP Token: $TOKEN_ACCOUNT"
echo "  - Gameplay: $GAMEPLAY_ACCOUNT"
echo ""
echo "Next steps:"
echo "  1. Run init.sh to initialize the contracts"
echo "  2. Update Unity client with contract addresses"
echo "  3. Test the game flow"