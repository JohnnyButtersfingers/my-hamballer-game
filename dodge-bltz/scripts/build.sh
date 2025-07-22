#!/bin/bash

# Build script for Dodge BLTZ smart contracts
# Compiles EOSIO C++ contracts using eosio-cpp

set -e

echo "🔨 Building Dodge BLTZ Smart Contracts..."

# Navigate to contracts directory
cd "$(dirname "$0")/../contracts"

# Create build directory if it doesn't exist
mkdir -p build

echo "📦 Compiling DBP Token Contract..."
eosio-cpp -I include -o build/dbp_token.wasm dbp_token.cpp --abigen

echo "🎮 Compiling Gameplay Contract..."
eosio-cpp -I include -o build/gameplay.wasm gameplay.cpp --abigen

echo "✅ Build completed successfully!"
echo ""
echo "Generated files:"
echo "  - build/dbp_token.wasm"
echo "  - build/dbp_token.abi"
echo "  - build/gameplay.wasm"
echo "  - build/gameplay.abi"
echo ""
echo "Ready for deployment to WAX testnet!"