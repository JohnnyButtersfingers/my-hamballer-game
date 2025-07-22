# Dodge BLTZ Deployment Guide

This guide provides step-by-step instructions for deploying the Dodge BLTZ Beta to WAX testnet.

## Prerequisites

Before deploying, ensure you have the following:

### Development Environment
- **EOSIO CDT (WAX edition)**: Contract Development Toolkit
- **cleos**: Command line interface for EOSIO
- **Unity 2022 LTS**: For the client application
- **WAX Cloud Wallet account**: For testing

### WAX Testnet Accounts
You'll need two WAX testnet accounts:
- `dbptoken.acc`: For the DBP token contract
- `gameplay.acc`: For the gameplay contract

### Account Setup
1. **Create testnet accounts** at https://wax-testnet.bloks.io/
2. **Fund accounts** with testnet WAX tokens from the faucet
3. **Import accounts** into your local wallet/cleos setup

## Step 1: Environment Setup

### Install EOSIO CDT (WAX Edition)
```bash
# Download and install EOSIO CDT for WAX
# Follow WAX documentation for your operating system
wget https://github.com/EOSIO/eosio.cdt/releases/download/v1.8.1/eosio.cdt_1.8.1-1-ubuntu-18.04_amd64.deb
sudo apt install ./eosio.cdt_1.8.1-1-ubuntu-18.04_amd64.deb
```

### Configure cleos for WAX Testnet
```bash
# Set up cleos alias for WAX testnet
echo 'alias cleos="cleos -u https://testnet.waxsweden.org"' >> ~/.bashrc
source ~/.bashrc
```

### Import Your Accounts
```bash
# Import your account keys (replace with your actual private keys)
cleos wallet create --to-console
cleos wallet import --private-key YOUR_DBPTOKEN_PRIVATE_KEY
cleos wallet import --private-key YOUR_GAMEPLAY_PRIVATE_KEY
```

## Step 2: Compile Smart Contracts

Navigate to the project root and compile the contracts:

```bash
cd dodge-bltz
chmod +x scripts/*.sh
./scripts/build.sh
```

**Expected Output:**
```
🔨 Building Dodge BLTZ Smart Contracts...
📦 Compiling DBP Token Contract...
🎮 Compiling Gameplay Contract...
✅ Build completed successfully!

Generated files:
  - build/dbp_token.wasm
  - build/dbp_token.abi
  - build/gameplay.wasm
  - build/gameplay.abi
```

## Step 3: Deploy Contracts to WAX Testnet

Deploy both contracts to your testnet accounts:

```bash
./scripts/deploy.sh
```

**Manual Deployment (if script fails):**
```bash
# Deploy DBP Token Contract
cleos set contract dbptoken.acc ./contracts/build dbp_token.wasm dbp_token.abi -p dbptoken.acc@active

# Deploy Gameplay Contract
cleos set contract gameplay.acc ./contracts/build gameplay.wasm gameplay.abi -p gameplay.acc@active
```

## Step 4: Set Permissions

Configure the necessary permissions for contract interactions:

### Allow Gameplay Contract to Issue Tokens
```bash
# Add eosio.code permission to token contract
cleos set account permission dbptoken.acc active --add-code

# Allow gameplay contract to call token contract
cleos set account permission dbptoken.acc active \
'{"threshold":1,"keys":[{"key":"YOUR_DBPTOKEN_PUBLIC_KEY","weight":1}],"accounts":[{"permission":{"actor":"gameplay.acc","permission":"eosio.code"},"weight":1}]}' \
owner -p dbptoken.acc@owner
```

### Allow RNG Oracle to Call Gameplay Contract
```bash
# Allow WAX RNG Oracle to call gameplay contract
cleos set account permission gameplay.acc active \
'{"threshold":1,"keys":[{"key":"YOUR_GAMEPLAY_PUBLIC_KEY","weight":1}],"accounts":[{"permission":{"actor":"orng.wax","permission":"eosio.code"},"weight":1}]}' \
owner -p gameplay.acc@owner
```

## Step 5: Initialize Contracts

Initialize the contracts with their configuration:

```bash
./scripts/init.sh
```

**Manual Initialization:**
```bash
# Create DBP Token
cleos push action dbptoken.acc create '["dbptoken.acc", "1000000.0000 DBP"]' -p dbptoken.acc@active

# Initialize Gameplay Contract
cleos push action gameplay.acc init '["dbptoken.acc"]' -p gameplay.acc@active
```

## Step 6: Verify Deployment

Test that the contracts are working correctly:

### Check Token Creation
```bash
cleos get table dbptoken.acc DBP stat
```
**Expected Output:**
```json
{
  "rows": [{
    "supply": "0.0000 DBP",
    "max_supply": "1000000.0000 DBP",
    "issuer": "dbptoken.acc"
  }]
}
```

### Check Gameplay Configuration
```bash
cleos get table gameplay.acc gameplay.acc config
```
**Expected Output:**
```json
{
  "rows": [{
    "token_contract": "dbptoken.acc",
    "success_rate_percent": 35,
    "reward_amount": "1.0000 DBP"
  }]
}
```

## Step 7: Unity Client Setup

### Import WAX Cloud Wallet SDK
1. Download the WAX Cloud Wallet SDK for Unity
2. Import the package into your Unity project
3. Configure the SDK with your settings

### Update Contract Addresses
Update the contract addresses in the Unity scripts:

```csharp
// In GameplayManager.cs
[SerializeField] private string gameplayContract = "gameplay.acc";  // Your actual account
[SerializeField] private string tokenContract = "dbptoken.acc";     // Your actual account
```

### Configure WAX Endpoints
```csharp
// In WalletConnection.cs and GameplayManager.cs
[SerializeField] private string waxRpcEndpoint = "https://testnet.waxsweden.org";
```

## Step 8: Test the Game Flow

### Test Play Action
```bash
# Test a play action (replace with actual player account)
cleos push action gameplay.acc play '["testplayer.wam", 123456789]' -p testplayer.wam@active
```

### Monitor RNG Oracle Response
```bash
# Check pending plays
cleos get table gameplay.acc gameplay.acc pendingplay

# Check if tokens were issued (after oracle response)
cleos get table dbptoken.acc testplayer.wam accounts
```

## Step 9: Build and Test Unity Client

1. **Open Unity Project**: Load the project in Unity 2022 LTS
2. **Build for WebGL**: Recommended for web-based WAX wallet integration
3. **Test Wallet Connection**: Verify WAX Cloud Wallet integration works
4. **Test Gameplay Flow**: Complete end-to-end play testing

## Troubleshooting

### Common Issues

#### Contract Compilation Errors
```bash
# Check EOSIO CDT installation
eosio-cpp --version

# Verify include paths
export EOSIO_CDT_ROOT="/usr/opt/eosio.cdt"
```

#### Permission Errors
```bash
# Verify account permissions
cleos get account dbptoken.acc
cleos get account gameplay.acc

# Check if accounts have sufficient resources
cleos get account dbptoken.acc | grep "ram\|cpu\|net"
```

#### RNG Oracle Issues
- Verify the oracle account name is correct: `orng.wax`
- Check that the oracle is active on testnet
- Ensure proper permissions are set for oracle callbacks

#### Unity Integration Issues
- Verify WAX Cloud Wallet SDK is properly imported
- Check console for JavaScript errors in WebGL builds
- Ensure CORS settings allow WAX domain requests

### Useful Commands

#### Monitor Contract Activity
```bash
# Watch for new transactions
cleos get actions gameplay.acc -1 -50

# Check contract logs
cleos get table gameplay.acc gameplay.acc usednonces
```

#### Reset Contract State (if needed)
```bash
# Clear contract tables (development only)
cleos push action gameplay.acc clear '[]' -p gameplay.acc@active
```

## Security Considerations

### Testnet vs Mainnet
- **Never use testnet keys on mainnet**
- **Always test thoroughly on testnet before mainnet deployment**
- **Review all permissions and contract logic**

### Key Management
- Store private keys securely
- Use different keys for different environments
- Consider multi-signature setups for production

### Contract Security
- Audit contract code before deployment
- Test all edge cases and failure scenarios
- Monitor contract activity for unusual patterns

## Next Steps

1. **Complete Testing**: Validate all game flows work correctly
2. **Performance Testing**: Test with multiple concurrent players
3. **Security Audit**: Review contract code for vulnerabilities
4. **Mainnet Preparation**: Prepare for production deployment
5. **Documentation**: Update user guides and API documentation

## Support Resources

- **WAX Developer Documentation**: https://developer.wax.io/
- **EOSIO Developer Portal**: https://developers.eos.io/
- **WAX Testnet Explorer**: https://wax-testnet.bloks.io/
- **WAX RNG Oracle Documentation**: Check WAX developer resources

For issues during deployment, refer to the contract error messages and WAX community forums for assistance.