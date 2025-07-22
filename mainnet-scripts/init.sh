#!/bin/bash

# Initialization script for Dodge BLTZ smart contracts
# Sets up initial token and gameplay configuration

set -e

# Configuration
TOKEN_ACCOUNT="dbptoken.acc"
GAMEPLAY_ACCOUNT="gameplay.acc"
WAX_MAINNET_URL="https://wax.greymass.com"

echo "⚙️ Initializing Dodge BLTZ Contracts on Mainnet..."

# Setup cleos alias
alias cleos="cleos -u $WAX_MAINNET_URL"

echo "🪙 Creating DBP Token..."
cleos push action $TOKEN_ACCOUNT create '["'$TOKEN_ACCOUNT'", "1000000.0000 DBP"]' -p $TOKEN_ACCOUNT@active

echo "🎮 Initializing Gameplay Contract..."
cleos push action $GAMEPLAY_ACCOUNT init '["'$TOKEN_ACCOUNT'"]' -p $GAMEPLAY_ACCOUNT@active

echo "🔐 Setting token permissions..."
# Allow gameplay contract to issue tokens
cleos set account permission $TOKEN_ACCOUNT active '{"threshold":1,"keys":[{"key":"YOUR_ACTIVE_KEY","weight":1}],"accounts":[{"permission":{"actor":"'$GAMEPLAY_ACCOUNT'","permission":"eosio.code"},"weight":1}]}' owner -p $TOKEN_ACCOUNT@owner

echo "✅ Initialization completed successfully!"
echo ""
echo "Token Configuration:"
echo "  - Symbol: DBP"
echo "  - Precision: 4 decimals"
echo "  - Max Supply: 1,000,000.0000 DBP"
echo "  - Issuer: $TOKEN_ACCOUNT"
echo ""
echo "Gameplay Configuration:"
echo "  - Success Rate: 35%"
echo "  - Reward: 1.0000 DBP per successful play"
echo "  - RNG Oracle: orng.wax"
echo ""
echo "🎯 Ready for testing!"
echo "Use the Unity client to connect and play BLTZ!"