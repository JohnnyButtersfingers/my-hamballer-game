# Dodge BLTZ Beta - Quick Start Guide

Get up and running with Dodge BLTZ in 15 minutes!

## Prerequisites Checklist

- [ ] Ubuntu/Linux system (or WSL on Windows)
- [ ] Git installed
- [ ] EOSIO CDT (WAX edition) installed
- [ ] Unity 2022.3 LTS or newer
- [ ] Node.js 16+ (optional, for additional tooling)

## Step 1: Install EOSIO CDT (2 minutes)

```bash
# Download WAX CDT
wget https://github.com/worldwide-asset-exchange/wax-cdt/releases/download/v1.7.0-wax02/cdt_1.7.0-wax02_amd64.deb

# Install
sudo apt install ./cdt_1.7.0-wax02_amd64.deb

# Verify installation
eosio-cpp --version
```

## Step 2: Get WAX Testnet Accounts (5 minutes)

1. Go to https://waxsweden.org/testnet/
2. Create two accounts:
   - `dbptoken.acc` (or similar, must be 12 characters)
   - `gameplay.acc` (or similar, must be 12 characters)
3. Save your private keys securely!

## Step 3: Clone and Build (3 minutes)

```bash
# Clone the repo (if not already done)
cd /path/to/your/workspace
git clone <repository-url> dodge-bltz-beta
cd dodge-bltz-beta

# Build contracts
./scripts/build_contracts.sh
```

## Step 4: Deploy to Testnet (5 minutes)

```bash
# Set your account names (if different from defaults)
export TOKEN_ACCOUNT="your_token_acc"
export GAMEPLAY_ACCOUNT="your_game_acc"

# Deploy contracts
./scripts/deploy_contracts.sh
```

You'll be prompted for your public key during deployment.

## Step 5: Test the Contracts

```bash
# Set up cleos alias for convenience
alias waxtest='cleos -u https://testnet.wax.pink.gg'

# Test playing the game
NONCE=$(date +%s)
waxtest push action $GAMEPLAY_ACCOUNT play '["testuser1234", "'$NONCE'"]' -p testuser1234@active

# Check player stats
waxtest get table $GAMEPLAY_ACCOUNT $GAMEPLAY_ACCOUNT players
```

## Step 6: Unity Client Setup

1. Open Unity Hub
2. Add the `unity-client` folder as a project
3. Install required packages:
   - TextMeshPro
   - Newtonsoft JSON
   - WAX Cloud Wallet SDK (manual import)
4. Open the main scene
5. Configure the GameManager with your contract names

## Common Issues & Solutions

### "Account does not exist"
→ Make sure you created the accounts on WAX testnet first

### "Contract not found"
→ Run the build script before deploying

### "Permission denied"
→ Make scripts executable: `chmod +x scripts/*.sh`

### "eosio-cpp not found"
→ Install EOSIO CDT or add it to your PATH

## What's Next?

1. **Read the docs**: Check out the [Developer Handoff Report](docs/DEVELOPER_HANDOFF.md)
2. **Run tests**: `cd tests && ./run_tests.sh`
3. **Customize**: Modify the win rate, rewards, or add features
4. **Deploy to mainnet**: Follow the deployment guide with mainnet accounts

## Need Help?

- WAX Discord: https://discord.gg/wax
- WAX Docs: https://developers.wax.io/
- Check the handoff report for detailed information

---

Happy building! 🎮🚀