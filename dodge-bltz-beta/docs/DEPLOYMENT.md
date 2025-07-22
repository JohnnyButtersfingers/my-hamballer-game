# Dodge BLTZ Deployment Guide

This guide provides step-by-step instructions for deploying the Dodge BLTZ smart contracts to the WAX testnet.

## Prerequisites

1. **EOSIO CDT (WAX Edition)**
   ```bash
   wget https://github.com/worldwide-asset-exchange/wax-cdt/releases/download/v1.7.0-wax02/cdt_1.7.0-wax02_amd64.deb
   sudo apt install ./cdt_1.7.0-wax02_amd64.deb
   ```

2. **WAX Testnet Accounts**
   - Create accounts at: https://waxsweden.org/testnet/
   - Required accounts:
     - `dbptoken.acc` - For the DBP token contract
     - `gameplay.acc` - For the gameplay contract

3. **Cleos Configuration**
   ```bash
   # Set testnet endpoint
   export WAX_TESTNET_URL="https://testnet.wax.pink.gg"
   ```

## Contract Compilation

### 1. Compile DBP Token Contract

```bash
cd contracts/dbp_token
eosio-cpp -abigen -I include -o dbp_token.wasm dbp_token.cpp
```

### 2. Compile Gameplay Contract

```bash
cd ../gameplay
eosio-cpp -abigen -I include -o gameplay.wasm gameplay.cpp
```

## Contract Deployment

### 1. Deploy DBP Token Contract

```bash
# Deploy the contract
cleos -u $WAX_TESTNET_URL set contract dbptoken.acc ./contracts/dbp_token \
  dbp_token.wasm dbp_token.abi -p dbptoken.acc@active

# Set eosio.code permission
cleos -u $WAX_TESTNET_URL set account permission dbptoken.acc active \
  '{"threshold":1,"keys":[{"key":"YOUR_PUBLIC_KEY","weight":1}],"accounts":[{"permission":{"actor":"dbptoken.acc","permission":"eosio.code"},"weight":1}]}' \
  owner -p dbptoken.acc@owner
```

### 2. Deploy Gameplay Contract

```bash
# Deploy the contract
cleos -u $WAX_TESTNET_URL set contract gameplay.acc ./contracts/gameplay \
  gameplay.wasm gameplay.abi -p gameplay.acc@active

# Set eosio.code permission
cleos -u $WAX_TESTNET_URL set account permission gameplay.acc active \
  '{"threshold":1,"keys":[{"key":"YOUR_PUBLIC_KEY","weight":1}],"accounts":[{"permission":{"actor":"gameplay.acc","permission":"eosio.code"},"weight":1}]}' \
  owner -p gameplay.acc@owner
```

## Contract Initialization

### 1. Initialize DBP Token

```bash
# Create token with maximum supply
cleos -u $WAX_TESTNET_URL push action dbptoken.acc create \
  '["dbptoken.acc", "1000000.0000 DBP"]' \
  -p dbptoken.acc@active

# Issue initial supply (optional)
cleos -u $WAX_TESTNET_URL push action dbptoken.acc issue \
  '["dbptoken.acc", "10000.0000 DBP", "Initial supply"]' \
  -p dbptoken.acc@active
```

### 2. Configure Gameplay Contract

```bash
# Set token contract
cleos -u $WAX_TESTNET_URL push action gameplay.acc settoken \
  '["dbptoken.acc"]' \
  -p gameplay.acc@active

# Configure RNG oracle (WAX RNG contract)
cleos -u $WAX_TESTNET_URL push action gameplay.acc setrng \
  '["orng.wax"]' \
  -p gameplay.acc@active
```

## Unity Client Configuration

1. **Update Contract Names**
   - Open `unity-client/Assets/Scripts/GameConfig.cs`
   - Set contract account names:
     ```csharp
     public const string TOKEN_CONTRACT = "dbptoken.acc";
     public const string GAMEPLAY_CONTRACT = "gameplay.acc";
     ```

2. **Configure WAX Cloud Wallet**
   - Update endpoint URLs in `WalletConnection.cs`
   - Set testnet configuration

## Testing the Deployment

### 1. Test Token Transfer

```bash
cleos -u $WAX_TESTNET_URL push action dbptoken.acc transfer \
  '["dbptoken.acc", "testuser1234", "1.0000 DBP", "Test transfer"]' \
  -p dbptoken.acc@active
```

### 2. Test Gameplay

```bash
# Generate a unique nonce
NONCE=$(date +%s%N)

# Submit play action
cleos -u $WAX_TESTNET_URL push action gameplay.acc play \
  '["testuser1234", "'$NONCE'"]' \
  -p testuser1234@active
```

## Monitoring

### Check Token Balance
```bash
cleos -u $WAX_TESTNET_URL get currency balance dbptoken.acc testuser1234 DBP
```

### View Contract Tables
```bash
# View token stats
cleos -u $WAX_TESTNET_URL get table dbptoken.acc DBP stat

# View gameplay stats
cleos -u $WAX_TESTNET_URL get table gameplay.acc gameplay.acc players
```

## Troubleshooting

### Common Issues

1. **Permission Errors**
   - Ensure eosio.code permission is set correctly
   - Verify account has sufficient CPU/NET/RAM

2. **RNG Oracle Issues**
   - Confirm orng.wax is accessible on testnet
   - Check that gameplay contract has permission to call RNG

3. **Token Minting Fails**
   - Verify gameplay contract has permission to issue tokens
   - Check token maximum supply hasn't been exceeded

## Next Steps

1. Run comprehensive tests using the test suite
2. Monitor contract performance on testnet
3. Gather feedback from beta testers
4. Prepare for mainnet deployment