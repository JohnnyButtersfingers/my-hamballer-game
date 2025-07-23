#!/bin/bash

# Deploy script for Dodge BLTZ smart contracts
#
# REQUIREMENTS:
# - Linux environment (Ubuntu 20.04+ recommended)
# - EOSIO CDT (WAX edition) installed
# - cleos command available in PATH
# - WAX testnet accounts: dbptoken.acc, gameplay.acc
# - Testnet accounts funded with WAX tokens
#
# PREREQUISITES:
# 1. Run build_contracts.sh first to compile contracts
# 2. Create testnet accounts at https://waxsweden.org/testnet/
# 3. Fund accounts with testnet WAX tokens
#
# NOTE: This script will not work on macOS due to platform compatibility issues.

echo "Deploying Dodge BLTZ Smart Contracts to WAX Testnet..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
WAX_TESTNET_URL="${WAX_TESTNET_URL:-https://testnet.wax.pink.gg}"
TOKEN_ACCOUNT="${TOKEN_ACCOUNT:-dbptoken.acc}"
GAMEPLAY_ACCOUNT="${GAMEPLAY_ACCOUNT:-gameplay.acc}"

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
CONTRACTS_DIR="$PROJECT_ROOT/contracts"

echo -e "${BLUE}Configuration:${NC}"
echo "WAX Testnet URL: $WAX_TESTNET_URL"
echo "Token Account: $TOKEN_ACCOUNT"
echo "Gameplay Account: $GAMEPLAY_ACCOUNT"
echo ""

# Check if cleos is installed
if ! command -v cleos &> /dev/null; then
    echo -e "${RED}Error: cleos not found. Please install EOSIO.${NC}"
    exit 1
fi

# Function to check if account exists
check_account() {
    local account=$1
    cleos -u $WAX_TESTNET_URL get account $account &> /dev/null
    return $?
}

# Check if accounts exist
echo -e "${YELLOW}Checking accounts...${NC}"
if ! check_account $TOKEN_ACCOUNT; then
    echo -e "${RED}✗ Token account $TOKEN_ACCOUNT does not exist${NC}"
    echo "Please create the account on WAX testnet first"
    exit 1
fi

if ! check_account $GAMEPLAY_ACCOUNT; then
    echo -e "${RED}✗ Gameplay account $GAMEPLAY_ACCOUNT does not exist${NC}"
    echo "Please create the account on WAX testnet first"
    exit 1
fi

echo -e "${GREEN}✓ Accounts verified${NC}"

# Deploy DBP Token Contract
echo -e "\n${YELLOW}Deploying DBP Token Contract...${NC}"
cd "$CONTRACTS_DIR/dbp_token"

if [ -f "dbp_token.wasm" ] && [ -f "dbp_token.abi" ]; then
    cleos -u $WAX_TESTNET_URL set contract $TOKEN_ACCOUNT . \
        dbp_token.wasm dbp_token.abi -p $TOKEN_ACCOUNT@active
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ DBP Token contract deployed successfully${NC}"
    else
        echo -e "${RED}✗ Failed to deploy DBP Token contract${NC}"
        exit 1
    fi
else
    echo -e "${RED}✗ Contract files not found. Run build_contracts.sh first${NC}"
    exit 1
fi

# Deploy Gameplay Contract
echo -e "\n${YELLOW}Deploying Gameplay Contract...${NC}"
cd "$CONTRACTS_DIR/gameplay"

if [ -f "gameplay.wasm" ] && [ -f "gameplay.abi" ]; then
    cleos -u $WAX_TESTNET_URL set contract $GAMEPLAY_ACCOUNT . \
        gameplay.wasm gameplay.abi -p $GAMEPLAY_ACCOUNT@active
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ Gameplay contract deployed successfully${NC}"
    else
        echo -e "${RED}✗ Failed to deploy Gameplay contract${NC}"
        exit 1
    fi
else
    echo -e "${RED}✗ Contract files not found. Run build_contracts.sh first${NC}"
    exit 1
fi

# Set permissions
echo -e "\n${YELLOW}Setting permissions...${NC}"

# Get public key (you'll need to modify this for your setup)
echo -e "${BLUE}Please enter the public key for the accounts:${NC}"
read -p "Public key: " PUBLIC_KEY

# Set eosio.code permission for token contract
echo "Setting eosio.code permission for $TOKEN_ACCOUNT..."
cleos -u $WAX_TESTNET_URL set account permission $TOKEN_ACCOUNT active \
    "{\"threshold\":1,\"keys\":[{\"key\":\"$PUBLIC_KEY\",\"weight\":1}],\"accounts\":[{\"permission\":{\"actor\":\"$TOKEN_ACCOUNT\",\"permission\":\"eosio.code\"},\"weight\":1}]}" \
    owner -p $TOKEN_ACCOUNT@owner

# Set eosio.code permission for gameplay contract
echo "Setting eosio.code permission for $GAMEPLAY_ACCOUNT..."
cleos -u $WAX_TESTNET_URL set account permission $GAMEPLAY_ACCOUNT active \
    "{\"threshold\":1,\"keys\":[{\"key\":\"$PUBLIC_KEY\",\"weight\":1}],\"accounts\":[{\"permission\":{\"actor\":\"$GAMEPLAY_ACCOUNT\",\"permission\":\"eosio.code\"},\"weight\":1}]}" \
    owner -p $GAMEPLAY_ACCOUNT@owner

# Initialize contracts
echo -e "\n${YELLOW}Initializing contracts...${NC}"

# Create DBP token
echo "Creating DBP token..."
cleos -u $WAX_TESTNET_URL push action $TOKEN_ACCOUNT create \
    "[\"$TOKEN_ACCOUNT\", \"1000000.0000 DBP\"]" \
    -p $TOKEN_ACCOUNT@active

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓ DBP token created${NC}"
else
    echo -e "${YELLOW}Token might already exist${NC}"
fi

# Configure gameplay contract
echo "Configuring gameplay contract..."
cleos -u $WAX_TESTNET_URL push action $GAMEPLAY_ACCOUNT settoken \
    "[\"$TOKEN_ACCOUNT\"]" \
    -p $GAMEPLAY_ACCOUNT@active

cleos -u $WAX_TESTNET_URL push action $GAMEPLAY_ACCOUNT setrng \
    "[\"orng.wax\"]" \
    -p $GAMEPLAY_ACCOUNT@active

echo -e "\n${GREEN}Deployment complete!${NC}"
echo -e "${BLUE}Next steps:${NC}"
echo "1. Test the contracts with test transactions"
echo "2. Configure the Unity client with these account names"
echo "3. Run integration tests"