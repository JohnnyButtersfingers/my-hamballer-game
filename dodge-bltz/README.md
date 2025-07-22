# Dodge BLTZ Beta

**Provably-fair, RNG-based dodgeball mini-game on the WAX blockchain**

## Project Overview

Dodge BLTZ is a provably-fair, RNG-based dodgeball mini-game built on the WAX blockchain. The Beta MVP focuses on a single "BLTZ" action with a 35% success chance. A successful play mints one $DBP token; nothing is rewarded on failure. The game integrates WAX Cloud Wallet for authentication and uses the official WAX RNG Oracle to guarantee fairness.

### Beta MVP Scope

- **Single Action**: BLTZ action with 35% success rate
- **Simple Rewards**: 1 $DBP token on success, nothing on failure
- **Provably Fair**: Uses WAX RNG Oracle for true randomness
- **WAX Integration**: WAX Cloud Wallet authentication
- **Nonce Protection**: Prevents replay attacks

### Excluded from Beta

Features deliberately excluded until later phases:
- FlexBLTZ mechanics
- CP currency system
- NFT integration
- Leaderboards
- Governance features

## Architecture

```
dodge-bltz/
├── contracts/              # EOSIO C++ Smart Contracts
│   ├── dbp_token.cpp      # $DBP token management
│   ├── gameplay.cpp       # Core game logic with RNG
│   ├── dbp_token.hpp      # Token contract header
│   ├── gameplay.hpp       # Gameplay contract header
│   └── CMakeLists.txt     # Build configuration
├── scripts/               # Deployment and build scripts
│   ├── build.sh          # Contract compilation
│   ├── deploy.sh         # Testnet deployment
│   └── init.sh           # Contract initialization
├── tests/                 # Unit test scaffolds
│   ├── token_tests.cpp   # $DBP token tests
│   └── gameplay_tests.cpp # Gameplay logic tests
├── unity/                 # Unity client
│   ├── Scripts/          # C# gameplay scripts
│   │   ├── WalletConnection.cs    # WAX Cloud Wallet integration
│   │   ├── GameplayManager.cs     # Game logic and transactions
│   │   └── UIManager.cs          # UI state management
│   └── UI_Brief.md       # UI design specifications
├── docs/                  # Documentation
│   ├── README.md         # This file
│   ├── DEPLOYMENT.md     # Deployment guide
│   └── HANDOFF_REPORT.md # Developer handoff report
└── .gitignore            # Git ignore rules
```

## Technology Stack

- **Smart Contracts**: EOSIO C++ (WAX blockchain)
- **RNG**: WAX RNG Oracle for provably fair outcomes
- **Client**: Unity 2022 LTS or newer
- **Wallet**: WAX Cloud Wallet SDK
- **Build Tools**: EOSIO CDT (WAX edition)

## Quick Start

### Prerequisites

- EOSIO CDT (WAX edition)
- Unity 2022 LTS or newer
- WAX testnet accounts for `dbptoken.acc` and `gameplay.acc`
- cleos CLI tool configured for WAX testnet

### Development Setup

1. **Clone Repository**
   ```bash
   git clone <repository-url>
   cd dodge-bltz
   ```

2. **Compile Contracts**
   ```bash
   cd contracts
   ./build.sh
   ```

3. **Deploy to Testnet**
   ```bash
   ./deploy.sh
   ```

4. **Initialize Token**
   ```bash
   ./init.sh
   ```

5. **Setup Unity Client**
   - Open Unity 2022 LTS
   - Import WAX Cloud Wallet SDK
   - Update contract account names in scripts
   - Build and test

## Game Flow

1. **Connect Wallet**: Player authenticates with WAX Cloud Wallet
2. **Generate Nonce**: Client creates unique nonce for replay protection
3. **Submit BLTZ**: Player submits play transaction with nonce
4. **RNG Request**: Contract requests random number from WAX RNG Oracle
5. **Receive Result**: Oracle callback determines success (35% chance)
6. **Reward**: On success, 1 $DBP token is minted to player

## Smart Contracts

### DBP Token Contract (`dbp_token.cpp`)
- Manages $DBP token issuance and transfers
- Standard EOSIO token contract with restricted minting
- Only allows gameplay contract to mint tokens

### Gameplay Contract (`gameplay.cpp`)
- Handles BLTZ play actions
- Enforces nonce-based replay protection
- Issues deferred RNG requests to WAX Oracle
- Processes RNG callbacks and awards tokens on success

## Testing

Unit test scaffolds are provided in the `tests/` directory:
- `token_tests.cpp`: Validates token issuance and transfer logic
- `gameplay_tests.cpp`: Tests nonce protection and RNG reward logic

Run tests after compilation:
```bash
cd tests
# Test execution commands will be added during development
```

## Deployment

Detailed deployment instructions are available in `docs/DEPLOYMENT.md`. The process includes:

1. Compiling contracts with `eosio-cpp`
2. Deploying to WAX testnet using `cleos`
3. Setting necessary permissions
4. Initializing the $DBP token
5. Configuring Unity client with contract addresses

## Security Features

- **Nonce Protection**: Prevents replay attacks using unique nonces
- **Provably Fair**: WAX RNG Oracle ensures true randomness
- **Permission Management**: Restricted minting access to gameplay contract only
- **Input Validation**: All contract actions validate input parameters

## Development Status

This project implements the Beta MVP as specified in the project handoff:
- ✅ Repository structure created
- ✅ Smart contract specifications defined
- ✅ Unity client architecture planned
- ⏳ Contract implementation in progress
- ⏳ Unity client development pending
- ⏳ Testing and deployment pending

## Next Steps

1. Implement smart contracts (`dbp_token.cpp` and `gameplay.cpp`)
2. Create deployment and build scripts
3. Develop Unity client with WAX Cloud Wallet integration
4. Write comprehensive unit tests
5. Deploy to WAX testnet for validation
6. Test end-to-end game flow

## License

MIT License - See LICENSE file for details

## Support

For questions or issues during development, refer to:
- `docs/HANDOFF_REPORT.md` for detailed implementation guidance
- `docs/DEPLOYMENT.md` for deployment instructions
- WAX Developer Documentation for blockchain integration