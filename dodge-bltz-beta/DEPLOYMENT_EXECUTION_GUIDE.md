# 🚀 WAX Testnet Deployment Execution Guide

**Date**: January 22, 2025  
**Status**: ⏳ **READY FOR EXECUTION**  
**Target**: Complete WAX Testnet Deployment & QA Validation

## 🎯 **Execution Overview**

This guide provides step-by-step instructions for executing the WAX testnet deployment of the Dodge BLTZ Beta MVP. Follow each phase sequentially and document results as you proceed.

## 📋 **Phase 1: Environment & Toolchain Setup**

### **1.1 Ubuntu 20.04+ Environment**
```bash
# Verify Ubuntu version (20.04+ required)
lsb_release -a

# Update system packages
sudo apt update && sudo apt upgrade -y

# Install build essentials
sudo apt install build-essential cmake git curl wget
```

### **1.2 Install Antelope CDT v4.1.0+**
```bash
# Clone Antelope CDT repository
git clone https://github.com/AntelopeIO/cdt
cd cdt

# Build from source (recommended)
cmake . && make install

# Verify installation
eosio-cpp --version
# Should show Antelope CDT v4.1.0 or later

# Alternative: Check for pre-built packages
# Visit developers.wax.io for latest packages
```

**Important Notes**:
- **macOS not supported** - Use Ubuntu 20.04+ only
- **v4.1.0+ required** - Includes BLS host functions and Antelope Spring v1.0+ compatibility
- **Build from source** if pre-built packages unavailable

### **1.3 Install EOSIO CLI (cleos)**
```bash
# Install via apt
sudo apt install eosio.cdt

# Verify installation
cleos --version

# Alternative installation methods available in INSTALLATION_NOTES.md
```

### **1.4 Repository Setup**
```bash
# Clone repository
git clone https://github.com/JohnnyButtersfingers/my-hamballer-game.git
cd my-hamballer-game

# Verify dodge-bltz-beta/ is primary source
ls -la dodge-bltz-beta/

# Check all documentation is present
ls -la dodge-bltz-beta/*.md
ls -la dodge-bltz-beta/scripts/
```

## 🏗️ **Phase 2: Compile & Deploy Contracts**

### **2.1 Compile Contracts**
```bash
# Navigate to scripts directory
cd dodge-bltz-beta/scripts

# Make scripts executable
chmod +x *.sh

# Compile contracts
./build_contracts.sh
```

**Expected Output**:
```
Building Dodge BLTZ Smart Contracts...
✅ DBP Token contract compiled successfully
✅ Gameplay contract compiled successfully
✅ Generated .wasm and .abi files
```

**Troubleshooting**:
- If CDT installation fails, check `INSTALLATION_NOTES.md`
- Consider Docker or older WAX-specific CDT versions
- Verify `eosio-cpp` is in PATH

### **2.2 Create WAX Testnet Accounts**
```bash
# Visit WAX testnet faucet
# https://faucet.waxsweden.org/

# Create accounts:
# - dbptoken.acc (for DBP token contract)
# - gameplay.acc (for gameplay contract)

# Verify accounts exist
cleos -u https://testnet.wax.pink.gg get account dbptoken.acc
cleos -u https://testnet.wax.pink.gg get account gameplay.acc
```

### **2.3 Deploy Contracts**
```bash
# Deploy contracts to WAX testnet
./deploy_contracts.sh

# Expected output:
# ✅ DBP Token contract deployed to dbptoken.acc
# ✅ Gameplay contract deployed to gameplay.acc
# ✅ Permissions configured correctly
```

**Important**: Use WAX testnet endpoint: `https://waxtestnet.greymass.com`

### **2.4 Initial Verification**
```bash
# Check gameplay contract tables
cleos -u https://testnet.wax.pink.gg get table gameplay.acc gameplay.acc players

# Check DBP token contract tables
cleos -u https://testnet.wax.pink.gg get table dbptoken.acc dbptoken.acc accounts

# Verify eosio.code permissions
cleos -u https://testnet.wax.pink.gg get account gameplay.acc
```

**Document any issues** in the deployment checklist.

## ⚙️ **Phase 3: Initialize & Configure Contracts / Unity Client**

### **3.1 Initialize Token Contract**
```bash
# Create DBP token with maximum supply
cleos -u https://testnet.wax.pink.gg push action dbptoken.acc create \
  '["dbptoken.acc", "1000000000.0000 DBP"]' \
  -p dbptoken.acc@active

# Verify token creation
cleos -u https://testnet.wax.pink.gg get table dbptoken.acc dbptoken.acc stat
```

### **3.2 Link Contracts**
```bash
# Set token contract in gameplay contract
cleos -u https://testnet.wax.pink.gg push action gameplay.acc settoken \
  '["dbptoken.acc"]' \
  -p gameplay.acc@active

# Set RNG oracle (orng.wax)
cleos -u https://testnet.wax.pink.gg push action gameplay.acc setrng \
  '["orng.wax"]' \
  -p gameplay.acc@active

# Verify configuration
cleos -u https://testnet.wax.pink.gg get table gameplay.acc gameplay.acc config
```

### **3.3 Configure Unity Client**
```bash
# Open Unity 2022 LTS
# Load project: dodge-bltz-beta/unity-client/

# Update GameConfig.cs with deployed accounts:
public const string TOKEN_CONTRACT = "dbptoken.acc";
public const string GAMEPLAY_CONTRACT = "gameplay.acc";

# Ensure WAX Cloud Wallet SDK v2+ is set to testnet
# Import WAX Cloud Wallet SDK if not already present
```

## 🧪 **Phase 4: End-to-End QA Validation**

### **4.1 Follow QA_HANDOFF.md Test Cases**

Execute the six core test cases:

#### **Test Case 1: Contract Deployment**
- [ ] Verify contracts deployed successfully
- [ ] Check permissions configured correctly
- [ ] Confirm tables exist and are accessible

#### **Test Case 2: Token Initialization**
- [ ] Verify DBP token created with correct parameters
- [ ] Check token statistics and supply
- [ ] Confirm token contract is functional

#### **Test Case 3: Gameplay Configuration**
- [ ] Verify token contract linked to gameplay
- [ ] Confirm RNG oracle configured (orng.wax)
- [ ] Test contract configuration

#### **Test Case 4: Unity Client Connection**
- [ ] Test WAX Cloud Wallet connection
- [ ] Verify account information displays
- [ ] Check DBP balance shows correctly

#### **Test Case 5: BLTZ Gameplay**
- [ ] Submit BLTZ play action
- [ ] Verify nonce generation and submission
- [ ] Monitor RNG Oracle callback
- [ ] Confirm ~35% success rate (±5% tolerance)
- [ ] Verify token minting on successful plays

#### **Test Case 6: UI Flow & Multiple Plays**
- [ ] Test three-screen UI flow (Start → Resolving → Result)
- [ ] Execute 10+ BLTZ actions
- [ ] Verify nonce protection works
- [ ] Test error handling and edge cases

### **4.2 Record Results in QA_RESULTS_TEMPLATE.md**

Populate the template with:
- **Environment details**: OS, CDT version, Unity version
- **Expected vs actual results**: Pass/fail status for each test
- **Performance metrics**: Latency, success rate, response times
- **Issues found**: Any problems encountered and solutions

### **4.3 Stress Testing**
```bash
# Perform fuzz test with ~1,000 plays
# Monitor for edge cases and performance issues
# Record metrics:
# - Average response time
# - Success rate over large sample
# - Memory usage
# - CPU usage
# - Any errors or failures
```

### **4.4 Update TEST_REPORT.md**
- Add real-world testing results
- Include performance metrics
- Document any issues found
- Note solutions and workarounds

## 📊 **Phase 5: Post-Validation Updates**

### **5.1 Update DEPLOYMENT_MILESTONE.md**
```bash
# Append deployment logs and details
# Include:
# - Environment setup notes
# - Deployment commands executed
# - Any issues encountered
# - Solutions implemented
# - Performance metrics
```

### **5.2 Troubleshooting Resources**
If issues arise:
- **CDT Installation**: Check `INSTALLATION_NOTES.md`
- **WAX Documentation**: Visit developers.wax.io
- **Antelope Documentation**: Check docs.antelope.io
- **Community Support**: WAX Discord/Telegram

### **5.3 Finalize POST_BETA_ROADMAP.md**
- Ensure milestones align with timeline
- Update based on deployment learnings
- Confirm FlexBLTZ, CP currency, leaderboards, NFTs phases
- Review Q2 2025 - Q1 2026 timeline

## 🔄 **Phase 6: CI/CD Setup**

### **6.1 GitHub Actions Workflow**
Create `.github/workflows/ci.yml`:
```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  contract-lint:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Install Antelope CDT
        run: |
          git clone https://github.com/AntelopeIO/cdt
          cd cdt && cmake . && make install
      - name: Lint Contracts
        run: |
          cd dodge-bltz-beta/scripts
          ./build_contracts.sh

  unity-build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Unity Build
        run: |
          # Unity build commands
          # (Requires Unity license)
```

### **6.2 Documentation Updates**
Update key files:
- **README.md**: Add deployment status
- **HANDOFF_REPORT.md**: Document successful deployment
- **QA_RESULTS.md**: Link to detailed results

## 📝 **Phase 7: Commit & PR**

### **7.1 Repository Cleanup**
```bash
# Archive pre-merge folders
mkdir archive
mv dodge-bltz/ archive/
mv hamballer-game-starter/ archive/

# Keep dodge-bltz-beta/ as primary
# Verify all documentation is in place
```

### **7.2 Create Branch & Push**
```bash
# Create new branch
git checkout -b testnet-qa-complete

# Add all changes
git add .

# Commit with descriptive message
git commit -m "feat: complete WAX testnet deployment and QA validation

- Deploy contracts to WAX testnet
- Complete end-to-end QA testing
- Update documentation with results
- Prepare for post-beta development"

# Push to remote
git push origin testnet-qa-complete
```

### **7.3 Submit Pull Request**
**Title**: "WAX Testnet Deployment, E2E QA Validation, & Post-Beta Prep"

**Description**:
```
## 🚀 WAX Testnet Deployment Complete

### ✅ Completed Tasks
- Environment setup with Antelope CDT v4.1.0+
- Contract compilation and deployment
- Unity client configuration and testing
- End-to-end QA validation (6 test cases)
- Performance testing and stress testing
- Documentation updates

### 📊 Results
- All contracts deployed successfully
- Unity client connects and functions correctly
- 35% success rate maintained (±5% tolerance)
- Token minting works accurately
- UI flow operates smoothly

### 📋 Documentation
- QA_RESULTS.md: Complete test results
- DEPLOYMENT_MILESTONE.md: Deployment logs
- TEST_REPORT.md: Performance metrics
- POST_BETA_ROADMAP.md: Future development plan

### 🎯 Next Steps
- Review and merge this PR
- Begin post-beta development planning
- Execute FlexBLTZ implementation (Q2 2025)

**Status**: ✅ Ready for mainnet preparation
```

### **7.4 Attach Files**
If available, attach:
- Simulation CSV files
- Performance graphs (supply_curve.png)
- Deployment logs
- QA test results

## 🎯 **Success Criteria**

### **Technical Requirements**
- ✅ All contracts deploy without errors
- ✅ Unity client connects and functions correctly
- ✅ 35% success rate maintained (±5% tolerance)
- ✅ Token minting works accurately
- ✅ UI flow operates smoothly

### **Performance Requirements**
- ✅ BLTZ action submission < 5 seconds
- ✅ Wallet connection < 3 seconds
- ✅ UI transitions < 1 second
- ✅ No critical bugs or crashes

### **Documentation Requirements**
- ✅ Complete QA results documented
- ✅ Deployment logs recorded
- ✅ Post-Beta roadmap finalized
- ✅ Repository properly organized

## 🚨 **Important Reminders**

### **CDT Version Verification**
- **Antelope CDT v4.1.0+**: Latest release as of July 2025
- **Fallback options**: Docker or older WAX-specific CDT if needed
- **Documentation**: Check developers.wax.io for troubleshooting

### **Testnet Configuration**
- **Endpoint**: Use `https://waxtestnet.greymass.com`
- **RNG Oracle**: Ensure orng.wax is correctly configured
- **Account Funding**: Sufficient WAX tokens for deployment

### **Issue Logging**
- **Document all issues**: Installation, deployment, or testing problems
- **Note solutions**: Fixes found and implemented
- **Update documentation**: Keep guides current

## 🎉 **Completion Checklist**

- [ ] **Environment Setup**: Ubuntu 20.04+ with Antelope CDT v4.1.0+
- [ ] **Contract Deployment**: Both contracts deployed successfully
- [ ] **Contract Initialization**: Token and gameplay configured
- [ ] **Unity Testing**: Client connects and functions correctly
- [ ] **QA Validation**: All 6 test cases pass
- [ ] **Stress Testing**: 1,000+ plays executed successfully
- [ ] **Documentation**: Complete results and logs recorded
- [ ] **CI/CD Setup**: GitHub Actions workflow created
- [ ] **Repository**: Updated and organized
- [ ] **PR Submitted**: Ready for review and merge

**Status**: ⏳ **READY FOR EXECUTION**

**Next Goal**: 🚀 **Complete WAX Testnet Deployment & QA Validation** 