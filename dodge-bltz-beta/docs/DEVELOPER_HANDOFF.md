# Developer Handoff Report - Dodge BLTZ Beta

## Executive Summary

This document consolidates all artifacts, code, and documentation for the Dodge BLTZ Beta MVP. The project implements a provably-fair dodgeball mini-game on the WAX blockchain with a 35% success rate, rewarding successful plays with $DBP tokens.

## Project Status

### Completed Components

1. **Smart Contracts** ✓
   - DBP Token Contract - ERC20-style token management
   - Gameplay Contract - Core game logic with RNG integration

2. **Unity Client** ✓
   - WAX Cloud Wallet integration
   - Game UI (Start, Resolving, Result screens)
   - Transaction management

3. **Documentation** ✓
   - README with project overview
   - Deployment guide with step-by-step instructions
   - This handoff report

4. **Testing Framework** ✓
   - Unit test scaffolds for contracts
   - Integration test scripts

## Architecture Overview

```
┌─────────────────┐     ┌──────────────┐     ┌───────────────┐
│  Unity Client   │────▶│ WAX Testnet  │────▶│  WAX RNG      │
│  (WebGL/PC)     │     │              │     │  Oracle       │
└─────────────────┘     └──────────────┘     └───────────────┘
                               │
                               ▼
                    ┌──────────────────┐
                    │  Smart Contracts │
                    ├──────────────────┤
                    │ • dbp_token.cpp  │
                    │ • gameplay.cpp   │
                    └──────────────────┘
```

## Smart Contract Details

### DBP Token Contract (dbp_token.cpp)

**Purpose**: Manages the $DBP token issuance and transfers

**Key Actions**:
- `create(issuer, max_supply)` - Initialize token
- `issue(to, quantity, memo)` - Mint new tokens
- `transfer(from, to, quantity, memo)` - Transfer tokens
- `burn(owner, quantity)` - Burn tokens

**Tables**:
- `accounts` - Token balances per account
- `stat` - Token statistics (supply, max supply, issuer)

### Gameplay Contract (gameplay.cpp)

**Purpose**: Handles game logic and RNG integration

**Key Actions**:
- `play(player, nonce)` - Initiate a game round
- `receiverand(caller_id, random_value)` - RNG callback
- `settoken(token_contract)` - Configure token contract
- `setrng(rng_contract)` - Configure RNG oracle

**Tables**:
- `players` - Player statistics (plays, wins, last_nonce)
- `pending` - Pending RNG requests
- `config` - Contract configuration

**Game Flow**:
1. Player calls `play` with unique nonce
2. Contract validates nonce (replay protection)
3. Contract requests RNG from oracle
4. Oracle calls back with random value
5. Contract calculates win (35% chance)
6. If win, contract mints 1 DBP token to player

## Unity Client Implementation

### Core Scripts

**WalletConnection.cs**
```csharp
public class WalletConnection : MonoBehaviour {
    // Handles WAX Cloud Wallet authentication
    // Methods: Login(), Logout(), GetAccount()
}
```

**GameplayManager.cs**
```csharp
public class GameplayManager : MonoBehaviour {
    // Manages game state and blockchain interactions
    // Methods: PlayGame(), GenerateNonce(), SubmitTransaction()
}
```

### UI Screens

1. **Start Screen**
   - Login button (WAX Cloud Wallet)
   - Play button (disabled until logged in)
   - Account info display

2. **Resolving Screen**
   - Animated dodgeball
   - "Waiting for blockchain..." message
   - Transaction status

3. **Result Screen**
   - Win/Loss display
   - Token reward animation (if won)
   - Play again button

## Deployment Requirements

### WAX Testnet Accounts
- `dbptoken.acc` - Token contract account
- `gameplay.acc` - Gameplay contract account

### Dependencies
- EOSIO CDT v1.7.0 (WAX edition)
- Unity 2022.3 LTS or newer
- WAX Cloud Wallet SDK
- Node.js 16+ (for scripts)

### Environment Variables
```bash
export WAX_TESTNET_URL="https://testnet.wax.pink.gg"
export TOKEN_ACCOUNT="dbptoken.acc"
export GAMEPLAY_ACCOUNT="gameplay.acc"
```

## Testing Strategy

### Unit Tests
Located in `tests/` directory:
- `test_dbp_token.cpp` - Token contract tests
- `test_gameplay.cpp` - Gameplay contract tests
- `test_rng_integration.cpp` - RNG oracle integration

### Integration Tests
- End-to-end gameplay flow
- Wallet connection tests
- Transaction submission and confirmation

### Test Scenarios
1. Successful play with win
2. Successful play with loss
3. Duplicate nonce rejection
4. Invalid player rejection
5. RNG timeout handling

## Security Considerations

1. **Nonce-based Replay Protection**
   - Each play requires unique nonce
   - Nonces stored and validated on-chain

2. **RNG Fairness**
   - Uses official WAX RNG Oracle
   - 35% win rate enforced on-chain
   - No client-side RNG

3. **Token Security**
   - Limited minting (1 DBP per win)
   - Only gameplay contract can mint
   - Standard transfer protections

## Known Issues & Limitations

1. **Current Limitations**
   - Single game mode only (BLTZ)
   - Fixed 35% win rate
   - No multiplayer features
   - No NFT integration

2. **Technical Debt**
   - Unity UI needs polish
   - Error handling could be improved
   - More comprehensive test coverage needed

## Future Enhancements (Post-Beta)

1. **Phase 2 Features**
   - FlexBLTZ game mode
   - CP (Credit Points) currency
   - NFT boost items
   - Leaderboards

2. **Phase 3 Features**
   - Governance system
   - Tournament mode
   - Social features
   - Mobile support

## Support Resources

- WAX Documentation: https://developers.wax.io/
- WAX RNG Oracle: https://github.com/worldwide-asset-exchange/wax-orng
- Unity WAX SDK: https://github.com/worldwide-asset-exchange/wax-unity-sdk
- EOSIO Developer Portal: https://developers.eos.io/

## Handoff Checklist

- [ ] Clone repository
- [ ] Install EOSIO CDT
- [ ] Create testnet accounts
- [ ] Deploy contracts
- [ ] Configure Unity project
- [ ] Run tests
- [ ] Verify end-to-end flow

## Contact & Support

For questions or issues during development:
1. Check this handoff report first
2. Review the deployment guide
3. Consult WAX documentation
4. Flag specific blockers to the team

---

*Document Version: 1.0*  
*Last Updated: [Current Date]*  
*Status: Ready for Development*