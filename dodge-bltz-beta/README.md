# DODGE BLTZ BETA

**A provably-fair, RNG-based dodgeball mini-game on the WAX blockchain**

> 🔗 *This is the unified integration of work from multiple development sub-agents (Opus, Sonnet, and O3) for the Dodge-BLTZ Beta MVP.*

## Overview

DODGE BLTZ BETA is a blockchain-based game where players attempt to dodge incoming balls with a 35% success chance. Each successful play mints one $DBP token. The game uses WAX Cloud Wallet for authentication and the official WAX RNG Oracle to guarantee fairness.

This Beta MVP focuses on a single "BLTZ" action with three-screen UI (Start, Resolving, Result). Features like FlexBLTZ, CP currency, NFTs, leaderboards, and governance are planned for future phases.

## Integration Summary

This unified project combines:
- **Core EOSIO/WAX contracts** with RNG Oracle integration
- **Unity client** with WAX Cloud Wallet support  
- **Comprehensive documentation** and deployment guides
- **Testing infrastructure** for contract validation
- **Build and deployment scripts** for WAX testnet

## Project Structure

```
dodge-bltz-beta/
├── contracts/          # EOSIO C++ smart contracts
│   ├── dbp_token/     # $DBP token management
│   └── gameplay/      # Game logic and RNG integration
├── tests/             # Unit tests for contracts
├── scripts/           # Build and deployment scripts
│   ├── build_contracts.sh  # Contract compilation
│   └── deploy_contracts.sh # WAX testnet deployment
├── docs/              # Documentation
│   ├── DEPLOYMENT.md  # Step-by-step deployment guide
│   └── DEVELOPER_HANDOFF.md # Comprehensive project docs
├── unity-client/      # Unity game client
│   └── Assets/Scripts/
│       ├── WalletConnection.cs   # WAX Cloud Wallet integration
│       ├── GameplayManager.cs    # Game logic and transactions
│       └── GameConfig.cs         # Configuration management
├── QUICKSTART.md      # Quick setup guide
├── DOCUMENTATION_STATUS.md # Verification report
├── TEST_REPORT.md     # Testing results and validation
├── INSTALLATION_NOTES.md # EOSIO CDT setup guide
└── README.md          # This file
```

## Key Components

### Smart Contracts

1. **dbp_token.cpp** - Manages $DBP token issuance and transfers
2. **gameplay.cpp** - Handles play action, nonce-based replay protection, and RNG integration

### Unity Client

- **WalletConnection.cs** - WAX Cloud Wallet integration with session management
- **GameplayManager.cs** - Game logic, transaction submission, and result handling
- **GameConfig.cs** - Centralized configuration for contract accounts and settings
- Three-screen UI flow: Start → Resolving → Result

### Beta MVP Scope

- ✅ Single BLTZ action with 35% success rate
- ✅ $DBP token rewards (1 token per successful play)
- ✅ WAX Cloud Wallet authentication
- ✅ WAX RNG Oracle for provably fair randomness
- ✅ Nonce-based replay protection
- ✅ Three-screen Unity UI

## Prerequisites

- EOSIO CDT (WAX edition)
- Unity 2022 LTS or newer
- WAX testnet accounts for `dbptoken.acc` and `gameplay.acc`
- Node.js and npm
- cleos (EOSIO CLI)

## Quick Start

1. **Clone and Setup**
   ```bash
   git clone <repository-url>
   cd dodge-bltz-beta
   ```

2. **Compile Contracts**
   ```bash
   cd scripts
   ./build_contracts.sh
   ```

3. **Deploy to WAX Testnet**
   ```bash
   ./deploy_contracts.sh
   ```

4. **Configure Unity Client**
   - Open Unity 2022 LTS
   - Load the unity-client project
   - Update contract account names in GameConfig.cs
   - Build and test

## Documentation

### **Core Documentation**
- [Quick Start Guide](QUICKSTART.md) - Fast setup for developers
- [Deployment Guide](docs/DEPLOYMENT.md) - Step-by-step deployment instructions
- [Developer Handoff Report](docs/DEVELOPER_HANDOFF.md) - Comprehensive project documentation

### **Verification & Testing**
- [Documentation Status](DOCUMENTATION_STATUS.md) - Complete verification report
- [Test Report](TEST_REPORT.md) - Testing results and validation
- [Installation Notes](INSTALLATION_NOTES.md) - EOSIO CDT setup guide

### **QA & Deployment**
- [Final PR Summary](../FINAL_PR_SUMMARY.md) - Complete PR overview
- [Installation Requirements](INSTALLATION_NOTES.md#deployment-readiness) - Linux environment setup

## Testing

Run unit tests:
```bash
cd tests
./run_tests.sh
```

## Sub-Agent Integration Notes

This project successfully unifies contributions from:
- **Opus**: Core contract architecture and RNG integration design
- **Sonnet**: Unity client implementation and WAX wallet integration
- **O3**: Testing infrastructure and deployment automation

All redundancies have been removed and naming conventions unified for consistency.

## License

[License details to be added]