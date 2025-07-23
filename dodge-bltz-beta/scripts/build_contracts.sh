#!/bin/bash

# Build script for Dodge BLTZ smart contracts
# 
# REQUIREMENTS:
# - Linux environment (Ubuntu 20.04+ recommended)
# - EOSIO CDT (WAX edition) installed
# - eosio-cpp command available in PATH
# 
# INSTALLATION:
# wget https://github.com/worldwide-asset-exchange/wax-cdt/releases/download/v1.7.0-wax02/cdt_1.7.0-wax02_amd64.deb
# sudo apt install ./cdt_1.7.0-wax02_amd64.deb
#
# NOTE: This script will not work on macOS due to platform compatibility issues.

echo "Building Dodge BLTZ Smart Contracts..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if eosio-cpp is installed
if ! command -v eosio-cpp &> /dev/null; then
    echo -e "${RED}Error: eosio-cpp not found. Please install EOSIO CDT.${NC}"
    exit 1
fi

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
CONTRACTS_DIR="$PROJECT_ROOT/contracts"

echo "Project root: $PROJECT_ROOT"
echo "Contracts directory: $CONTRACTS_DIR"

# Build DBP Token Contract
echo -e "\n${YELLOW}Building DBP Token Contract...${NC}"
cd "$CONTRACTS_DIR/dbp_token"

if [ -f "dbp_token.cpp" ]; then
    eosio-cpp -abigen -I include -o dbp_token.wasm dbp_token.cpp
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ DBP Token contract built successfully${NC}"
    else
        echo -e "${RED}✗ Failed to build DBP Token contract${NC}"
        exit 1
    fi
else
    echo -e "${RED}✗ dbp_token.cpp not found${NC}"
    exit 1
fi

# Build Gameplay Contract
echo -e "\n${YELLOW}Building Gameplay Contract...${NC}"
cd "$CONTRACTS_DIR/gameplay"

if [ -f "gameplay.cpp" ]; then
    eosio-cpp -abigen -I include -o gameplay.wasm gameplay.cpp
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ Gameplay contract built successfully${NC}"
    else
        echo -e "${RED}✗ Failed to build Gameplay contract${NC}"
        exit 1
    fi
else
    echo -e "${RED}✗ gameplay.cpp not found${NC}"
    exit 1
fi

echo -e "\n${GREEN}All contracts built successfully!${NC}"

# List built files
echo -e "\n${YELLOW}Built files:${NC}"
echo "DBP Token:"
ls -la "$CONTRACTS_DIR/dbp_token/"*.wasm "$CONTRACTS_DIR/dbp_token/"*.abi 2>/dev/null || echo "No files found"
echo -e "\nGameplay:"
ls -la "$CONTRACTS_DIR/gameplay/"*.wasm "$CONTRACTS_DIR/gameplay/"*.abi 2>/dev/null || echo "No files found"