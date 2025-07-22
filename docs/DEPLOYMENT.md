# Dodge BLTZ Deployment Guide

**Version**: Beta v1.0  
**Date**: December 28, 2024  
**Target Networks**: WAX Testnet & Mainnet  

## Overview

This document provides comprehensive deployment instructions for the Dodge BLTZ smart contracts and Unity client on both WAX testnet and mainnet environments.

## Prerequisites

### Development Environment
- **EOSIO CDT**: Contract Development Toolkit for compiling C++ contracts
- **cleos**: EOSIO CLI tool for blockchain interaction
- **Unity 2022 LTS**: For Unity client modifications and building
- **Git**: Version control and repository management
- **Bash Shell**: Linux/macOS environment for running deployment scripts

### WAX Accounts Required
- **Token Account**: `dbptoken.acc` (or your chosen token contract account)
- **Gameplay Account**: `gameplay.acc` (or your chosen gameplay contract account)
- **Deployment Account**: Account with sufficient resources for deployment

### Access Requirements
- Private keys for deployment accounts
- WAX wallet access (Anchor, WAX Cloud Wallet, etc.)
- Network access to WAX RPC endpoints

---

## Testnet Deployment

### Network Configuration
- **RPC Endpoint**: https://testnet.waxsweden.org
- **Chain ID**: WAX Testnet
- **Explorer**: https://wax-test.bloks.io/
- **Faucet**: Available for test tokens

### Deployment Steps

#### 1. Build Contracts
```bash
cd /workspace/dodge-bltz/scripts
./build.sh
```

**Expected Output**: Compiled WASM and ABI files in `contracts/build/`

#### 2. Deploy Contracts
```bash
./deploy.sh
```

**Actions Performed**:
- Deploy DBP token contract to `dbptoken.acc`
- Deploy gameplay contract to `gameplay.acc`
- Set up cross-contract permissions
- Configure RNG Oracle permissions

#### 3. Initialize Contracts
```bash
./init.sh
```

**Actions Performed**:
- Create DBP token with 1M max supply
- Initialize gameplay contract
- Set token issuance permissions
- Verify configuration

### Testnet Resource Usage
- **RAM**: ~2KB per contract deployment
- **CPU**: ~1 WAX staked per account for deployment
- **NET**: ~0.5 WAX staked per account for network operations

---

## Mainnet Deployment

### Network Configuration
- **RPC Endpoint**: https://wax.greymass.com
- **Chain ID**: WAX Mainnet
- **Explorer**: https://wax.bloks.io/
- **Wallet**: WAX Cloud Wallet, Anchor, or hardware wallet

### Pre-Deployment Security Checklist
- [ ] Smart contract security audit completed
- [ ] Private key security validated
- [ ] Backup recovery procedures tested
- [ ] Multi-signature setup (if applicable)
- [ ] Resource allocation planned and funded

### Mainnet Resource Budgeting

#### RAM Requirements
```yaml
Contract Deployment:
  - DBP Token Contract: ~3KB RAM
  - Gameplay Contract: ~4KB RAM
  - Contract Tables (initial): ~1KB RAM
  - Buffer for growth: ~2KB RAM
  
Total RAM Estimate: ~10KB (~$15-20 USD at current rates)

Account Operations:
  - Per Player Account: ~5KB RAM
  - Expected Beta Users: 1,000 accounts
  - Total User RAM: ~5MB (~$7,500-10,000 USD)
```

#### CPU Staking Requirements
```yaml
Contract Operations:
  - gameplay.acc: ~10 WAX staked
  - dbptoken.acc: ~5 WAX staked
  - Buffer for peak usage: ~15 WAX staked
  
Total CPU Stake: ~30 WAX (~$60-90 USD)

Player Operations:
  - Average per BLTZ action: ~0.1 WAX worth of CPU
  - Daily actions (1,000 players): ~100 WAX worth of CPU
  - Recommended staking: ~50 WAX for smooth operations
```

#### NET Staking Requirements
```yaml
Contract Operations:
  - gameplay.acc: ~5 WAX staked
  - dbptoken.acc: ~3 WAX staked
  - Buffer for transactions: ~7 WAX staked
  
Total NET Stake: ~15 WAX (~$30-45 USD)

Network Bandwidth:
  - Average transaction size: ~300 bytes
  - Daily transactions: ~3,500 (1,000 players × 3.5 actions)
  - Bandwidth requirement: ~1MB/day (minimal)
```

### Mainnet Deployment Steps

#### 1. Resource Allocation
```bash
# Stake resources for gameplay account
cleos system delegatebw deployment.acc gameplay.acc "10.00000000 WAX" "5.00000000 WAX"

# Stake resources for token account  
cleos system delegatebw deployment.acc dbptoken.acc "5.00000000 WAX" "3.00000000 WAX"

# Buy RAM for contracts
cleos system buyram deployment.acc gameplay.acc "0.1000 WAX"
cleos system buyram deployment.acc dbptoken.acc "0.0800 WAX"
```

#### 2. Deploy to Mainnet
```bash
cd /workspace/mainnet-scripts
./build.sh      # Build contracts
./deploy.sh     # Deploy to mainnet
./init.sh       # Initialize contracts
```

#### 3. Verification
```bash
# Verify token contract
cleos get table dbptoken.acc DBP stat

# Verify gameplay contract
cleos get table gameplay.acc gameplay.acc config

# Test transaction
cleos push action gameplay.acc play '["testplayer", 12345]' -p testplayer@active
```

---

## Unity Client Deployment

### WebGL Build Configuration
```yaml
Build Settings:
  - Platform: WebGL
  - Compression: Gzip
  - Memory Size: 256MB
  - Development Build: No (for production)
  - Script Debugging: No (for production)

Player Settings:
  - Company Name: HamBaller.xyz
  - Product Name: Dodge BLTZ
  - Version: 1.0
  - WebGL Template: Default (or custom)
```

### Network Configuration
Update Unity scripts with production endpoints:

```csharp
// WalletConnection.cs
private string waxRpcEndpoint = "https://wax.greymass.com";

// GameplayManager.cs  
private string gameplayContract = "gameplay.acc";
private string tokenContract = "dbptoken.acc";
```

### Deployment Process
1. **Build Unity Project**: Export WebGL build
2. **Host Files**: Upload to web server (CDN recommended)
3. **SSL Certificate**: Ensure HTTPS for wallet security
4. **Domain Setup**: Configure custom domain if desired
5. **Testing**: Verify wallet connection and gameplay flow

---

## Monitoring & Maintenance

### Performance Monitoring
```yaml
Key Metrics:
  - Transaction success rate: >95%
  - Average transaction time: <5 seconds
  - Wallet connection rate: >90%
  - Game session completion: >80%
  - Resource utilization: <70% of staked amounts
```

### Operational Procedures
- **Daily**: Monitor transaction volume and success rates
- **Weekly**: Review resource usage and adjust staking if needed
- **Monthly**: Analyze player engagement and technical performance
- **Quarterly**: Security review and contract audit consideration

### Emergency Procedures
- **High Resource Usage**: Increase CPU/NET staking
- **Contract Issues**: Prepare for redeployment (requires new accounts)
- **Oracle Downtime**: Monitor WAX RNG Oracle status
- **Network Issues**: Switch to backup RPC endpoints

---

## Cost Summary

### Initial Deployment (Mainnet)
```yaml
One-time Costs:
  - Contract RAM: ~$20 USD
  - Initial CPU/NET staking: ~$150 USD
  - Domain and hosting: ~$100 USD/year
  - Security audit (recommended): ~$5,000-15,000 USD

Total Initial Investment: ~$270 + audit costs
```

### Operational Costs (Monthly)
```yaml
Ongoing Costs:
  - Additional RAM (user growth): ~$500-1,000 USD/month
  - CPU/NET adjustments: ~$50-100 USD/month
  - Hosting and CDN: ~$50-200 USD/month
  - Monitoring tools: ~$100-300 USD/month

Total Monthly Operational: ~$700-1,600 USD
```

### Revenue Projections (Beta)
```yaml
Token Economics:
  - DBP max supply: 1,000,000 tokens
  - Beta emission rate: ~1,225 DBP/day (35% × 3,500 actions)
  - Beta duration: 30 days
  - Total Beta emissions: ~36,750 DBP (3.7% of supply)

Market Value (Speculative):
  - Conservative: $0.01/DBP = $10,000 total value
  - Moderate: $0.10/DBP = $100,000 total value
  - Optimistic: $1.00/DBP = $1,000,000 total value
```

---

## Troubleshooting

### Common Deployment Issues

#### Contract Compilation Errors
```bash
# Issue: eosio-cpp not found
# Solution: Install EOSIO CDT
wget https://github.com/EOSIO/eosio.cdt/releases/download/v3.0.1/eosio.cdt_3.0.1-1-ubuntu-20.04_amd64.deb
sudo apt install ./eosio.cdt_3.0.1-1-ubuntu-20.04_amd64.deb

# Issue: Include path errors
# Solution: Verify contract file structure and CMakeLists.txt
```

#### Deployment Permission Errors
```bash
# Issue: Account permission denied
# Solution: Verify active key and account ownership
cleos get account your.account

# Issue: Insufficient resources
# Solution: Stake more CPU/NET or buy more RAM
cleos system delegatebw payer account "1.0000 WAX" "1.0000 WAX"
```

#### Unity Build Issues
```bash
# Issue: WebGL build fails
# Solution: Check Unity version compatibility and build settings

# Issue: Wallet connection fails
# Solution: Verify HTTPS deployment and CORS configuration
```

### Support Resources
- **WAX Developer Docs**: https://developer.wax.io/
- **EOSIO Documentation**: https://developers.eos.io/
- **Unity WebGL Guide**: https://docs.unity3d.com/Manual/webgl.html
- **Community Support**: WAX Telegram, Discord channels

---

## Security Considerations

### Pre-Launch Security
- [ ] Smart contract audit by reputable firm
- [ ] Penetration testing of Unity client
- [ ] Key management security review
- [ ] Incident response plan prepared
- [ ] Insurance considerations evaluated

### Operational Security
- [ ] Multi-signature deployment accounts
- [ ] Regular security monitoring
- [ ] Automated anomaly detection
- [ ] Backup and recovery procedures
- [ ] Team security training

### Player Security
- [ ] HTTPS deployment mandatory
- [ ] Wallet integration security validated
- [ ] Transaction signing verification
- [ ] User education materials prepared
- [ ] Phishing protection measures

---

**Deployment Status**: ✅ Ready for mainnet deployment with proper resource planning and security measures in place.