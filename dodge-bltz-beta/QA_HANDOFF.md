# 🧪 QA Handoff Guide - Dodge BLTZ Beta MVP

**Date**: January 22, 2025  
**Status**: ✅ **READY FOR QA TESTING**  
**Target**: WAX Testnet Deployment & End-to-End Validation

## 🎯 **QA Mission Overview**

The Dodge BLTZ Beta MVP is ready for comprehensive QA testing on WAX testnet. This document provides the QA team with all necessary information to validate the complete game flow from contract deployment to end-to-end gameplay.

## ✅ **Pre-QA Verification Complete**

### **Documentation Status**
- ✅ **13/13 tests passed** (simulated scaffold)
- ✅ **All files verified and synced**
- ✅ **Build scripts validated** (error handling confirmed)
- ✅ **Unity client complete** (all scripts present)
- ⚠️ **CDT install requires Linux** (macOS unsupported)

### **Repository Status**
- **Primary Directory**: `dodge-bltz-beta/` is the unified Beta MVP source
- **Documentation**: Complete verification reports available
- **Scripts**: All build and deployment scripts ready
- **Contracts**: All source files present with headers

## 🛠 **QA Environment Setup**

### **Required Environment**
- **Operating System**: Ubuntu 20.04+ (Linux required)
- **EOSIO CDT**: WAX edition (v1.7.0-wax02)
- **Unity**: 2022 LTS or newer
- **WAX Testnet**: Active testnet accounts

### **Installation Steps**

#### **1. Linux Environment Setup**
```bash
# Ubuntu 20.04+ recommended
sudo apt update
sudo apt upgrade -y
```

#### **2. Install WAX CDT**
```bash
# Download WAX CDT
wget https://github.com/worldwide-asset-exchange/wax-cdt/releases/download/v1.7.0-wax02/cdt_1.7.0-wax02_amd64.deb

# Install CDT
sudo apt install ./cdt_1.7.0-wax02_amd64.deb

# Verify installation
eosio-cpp --version
```

#### **3. Install EOSIO CLI (cleos)**
```bash
# Install cleos for blockchain interaction
sudo apt install eosio.cdt
```

#### **4. Clone Repository**
```bash
git clone <repository-url>
cd dodge-bltz-beta
```

## 🚀 **QA Testing Checklist**

### **Phase 1: Contract Compilation & Deployment**

#### **1.1 Contract Compilation**
```bash
cd scripts
./build_contracts.sh
```

**Expected Results**:
- ✅ DBP Token contract compiled successfully
- ✅ Gameplay contract compiled successfully
- ✅ Generated `.wasm` and `.abi` files

#### **1.2 WAX Testnet Account Setup**
```bash
# Create testnet accounts at: https://waxsweden.org/testnet/
# Required accounts:
# - dbptoken.acc (for DBP token contract)
# - gameplay.acc (for gameplay contract)
```

#### **1.3 Contract Deployment**
```bash
./deploy_contracts.sh
```

**Expected Results**:
- ✅ DBP Token contract deployed to `dbptoken.acc`
- ✅ Gameplay contract deployed to `gameplay.acc`
- ✅ Permissions configured correctly

### **Phase 2: Contract Initialization**

#### **2.1 Initialize DBP Token**
```bash
# Create token with maximum supply
cleos -u https://testnet.wax.pink.gg push action dbptoken.acc create \
  '["dbptoken.acc", "1000000.0000 DBP"]' \
  -p dbptoken.acc@active
```

#### **2.2 Configure Gameplay Contract**
```bash
# Set token contract
cleos -u https://testnet.wax.pink.gg push action gameplay.acc settoken \
  '["dbptoken.acc"]' \
  -p gameplay.acc@active

# Configure RNG oracle
cleos -u https://testnet.wax.pink.gg push action gameplay.acc setrng \
  '["orng.wax"]' \
  -p gameplay.acc@active
```

### **Phase 3: Unity Client Testing**

#### **3.1 Unity Project Setup**
1. **Open Unity 2022 LTS**
2. **Load Project**: `unity-client/`
3. **Update Configuration**: Modify `GameConfig.cs` with deployed contract addresses
4. **Import WAX Cloud Wallet SDK**

#### **3.2 Unity Configuration**
```csharp
// Update GameConfig.cs with deployed contract addresses
public const string TOKEN_CONTRACT = "dbptoken.acc";
public const string GAMEPLAY_CONTRACT = "gameplay.acc";
```

### **Phase 4: End-to-End Game Flow Testing**

#### **4.1 Wallet Connection Test**
- ✅ Connect WAX Cloud Wallet
- ✅ Display account information
- ✅ Show DBP balance (should be 0 initially)

#### **4.2 BLTZ Action Test**
- ✅ Submit BLTZ play action
- ✅ Verify nonce generation
- ✅ Confirm transaction submission

#### **4.3 RNG Integration Test**
- ✅ Monitor RNG Oracle callback
- ✅ Verify 35% success rate over multiple plays
- ✅ Confirm token minting on successful plays

#### **4.4 UI Flow Test**
- ✅ Start screen displays correctly
- ✅ Resolving screen shows transaction progress
- ✅ Result screen displays outcome and updated balance

## 📊 **QA Test Cases**

### **Test Case 1: Contract Deployment**
- **Objective**: Verify contracts deploy successfully
- **Steps**: Run deployment scripts
- **Expected**: Both contracts deployed without errors
- **Status**: ⏳ Pending QA execution

### **Test Case 2: Token Initialization**
- **Objective**: Verify DBP token is properly initialized
- **Steps**: Create token with maximum supply
- **Expected**: Token created with 1,000,000 DBP max supply
- **Status**: ⏳ Pending QA execution

### **Test Case 3: Gameplay Configuration**
- **Objective**: Verify gameplay contract is properly configured
- **Steps**: Set token contract and RNG oracle
- **Expected**: Contract accepts configuration without errors
- **Status**: ⏳ Pending QA execution

### **Test Case 4: Unity Client Connection**
- **Objective**: Verify Unity client connects to deployed contracts
- **Steps**: Load Unity project and connect wallet
- **Expected**: Wallet connects and displays account info
- **Status**: ⏳ Pending QA execution

### **Test Case 5: BLTZ Gameplay**
- **Objective**: Verify complete game flow
- **Steps**: Submit BLTZ action and monitor results
- **Expected**: 35% success rate, token minting on wins
- **Status**: ⏳ Pending QA execution

### **Test Case 6: Multiple Plays**
- **Objective**: Verify system handles multiple plays correctly
- **Steps**: Submit 10+ BLTZ actions
- **Expected**: Nonce protection works, no replay attacks
- **Status**: ⏳ Pending QA execution

## 🎯 **Success Criteria**

### **Contract Deployment**
- ✅ Both contracts deploy without errors
- ✅ Permissions configured correctly
- ✅ Token initialized with correct parameters

### **Game Flow**
- ✅ Wallet connection works reliably
- ✅ BLTZ actions submit successfully
- ✅ RNG Oracle integration functions
- ✅ 35% success rate maintained
- ✅ Token minting occurs on wins

### **UI/UX**
- ✅ Three-screen flow works smoothly
- ✅ Transaction status updates correctly
- ✅ Error handling displays appropriate messages
- ✅ Balance updates reflect game outcomes

## 🚨 **Known Issues & Limitations**

### **Platform Limitations**
- **EOSIO CDT**: Requires Linux environment (macOS unsupported)
- **WAX CDT**: Only available as `.deb` package for Ubuntu/Debian

### **Testnet Considerations**
- **Account Creation**: Requires manual setup on WAX testnet
- **Funding**: Testnet accounts need WAX tokens for transactions
- **RNG Oracle**: May have rate limits on testnet

## 📋 **QA Reporting**

### **Required Reports**
1. **Deployment Report**: Contract deployment results
2. **Game Flow Report**: End-to-end testing results
3. **Performance Report**: Response times and reliability
4. **Bug Report**: Any issues encountered

### **Report Template**
```markdown
## QA Test Report - [Date]

### Environment
- OS: [Ubuntu version]
- CDT Version: [WAX CDT version]
- Unity Version: [Unity version]

### Test Results
- Contract Deployment: [Pass/Fail]
- Game Flow: [Pass/Fail]
- Performance: [Acceptable/Issues]

### Issues Found
- [List any issues]

### Recommendations
- [Next steps]
```

## 🎉 **QA Handoff Complete**

The Beta MVP is ready for comprehensive QA testing. All documentation is complete, scripts are validated, and the development team has clear guidance for deployment and testing.

**Status**: ✅ **READY FOR QA EXECUTION**

**Next Phase**: 🚀 **WAX Testnet Deployment & End-to-End Validation** 