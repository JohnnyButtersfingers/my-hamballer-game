# Dodge BLTZ Beta

A provably-fair, RNG-based dodgeball mini-game on the WAX blockchain.

## Overview

Dodge BLTZ is a blockchain-based game where players attempt to dodge incoming balls with a 35% success chance. Each successful play mints one $DBP token. The game uses WAX Cloud Wallet for authentication and the official WAX RNG Oracle to guarantee fairness.

This Beta MVP focuses on a single "BLTZ" action. Features like FlexBLTZ, CP currency, NFTs, leaderboards, and governance are planned for future phases.

## Project Structure

```
dodge-bltz-beta/
├── contracts/          # EOSIO C++ smart contracts
│   ├── dbp_token/     # $DBP token management
│   └── gameplay/      # Game logic and RNG integration
├── tests/             # Unit tests for contracts
├── scripts/           # Build and deployment scripts
├── docs/              # Documentation
├── unity-client/      # Unity game client
└── README.md          # This file
```

## Key Components

### Smart Contracts

1. **dbp_token.cpp** - Manages $DBP token issuance and transfers
2. **gameplay.cpp** - Handles play action, nonce-based replay protection, and RNG integration

### Unity Client

- **WalletConnection.cs** - WAX Cloud Wallet integration
- **GameplayManager.cs** - Game logic and transaction submission
- Three screens: Start, Resolving, Result

## Prerequisites

- EOSIO CDT (WAX edition)
- Unity 2022 LTS or newer
- WAX testnet accounts
- Node.js and npm
- cleos (EOSIO CLI)

## Quick Start

1. Clone the repository
2. Set up WAX testnet accounts for `dbptoken.acc` and `gameplay.acc`
3. Follow the deployment guide in `docs/DEPLOYMENT.md`
4. Build and test the Unity client

## Documentation

- [Deployment Guide](docs/DEPLOYMENT.md) - Step-by-step deployment instructions
- [Developer Handoff Report](docs/DEVELOPER_HANDOFF.md) - Comprehensive project documentation
- [Contract Documentation](docs/CONTRACTS.md) - Smart contract details

## Testing

Run unit tests:
```bash
cd tests
./run_tests.sh
```

## License

[License details to be added]