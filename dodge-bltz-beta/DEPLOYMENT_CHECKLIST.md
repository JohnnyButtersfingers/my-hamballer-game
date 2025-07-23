# 🚀 WAX Testnet Deployment Checklist

**Date**: January 22, 2025  
**Status**: ⏳ **READY FOR EXECUTION**  
**Target**: Complete WAX Testnet Deployment & QA Validation

## 📋 **Phase 1: Environment Setup & Contract Deployment**

### **1.1 Provision Ubuntu 20.04+ Machine**
- [ ] **System Requirements**: Ubuntu 20.04+ with package management rights
- [ ] **Package Manager**: `sudo apt update && sudo apt upgrade -y`
- [ ] **Build Tools**: `sudo apt install build-essential cmake git`
- [ ] **Note**: macOS not supported for Antelope CDT

### **1.2 Install Antelope CDT v4.1.0**
- [ ] **Clone Repository**: `git clone https://github.com/AntelopeIO/cdt`
- [ ] **Build from Source**: `cd cdt && cmake . && make install`
- [ ] **Verify Installation**: `eosio-cpp --version` (should show v4.1.0+)
- [ ] **Note**: v1.7.0-wax02 and 1.6.1-1 are outdated legacy versions

### **1.3 Install EOSIO CLI (cleos)**
- [ ] **Install via apt**: `sudo apt install eosio.cdt`
- [ ] **Verify Installation**: `cleos --version`
- [ ] **Alternative**: Install via Homebrew if available

### **1.4 Clone Repository & Verify Documentation**
- [ ] **Clone Repo**: `git clone <repository-url>`
- [ ] **Switch to Branch**: `cd dodge-bltz-beta`
- [ ] **Verify Docs**: Confirm all documentation is up-to-date
- [ ] **Check Files**: Verify `INSTALLATION_NOTES.md` reflects latest CDT requirements

### **1.5 Compile Contracts**
- [ ] **Navigate to Scripts**: `cd dodge-bltz-beta/scripts`
- [ ] **Make Executable**: `chmod +x *.sh`
- [ ] **Run Build**: `./build_contracts.sh`
- [ ] **Expected Output**: 
  - ✅ DBP Token contract compiled successfully
  - ✅ Gameplay contract compiled successfully
  - ✅ Generated `.wasm` and `.abi` files

### **1.6 Create & Fund Testnet Accounts**
- [ ] **Create Accounts**: Use WAX testnet faucet (https://faucet.waxsweden.org/)
- [ ] **Required Accounts**: 
  - `dbptoken.acc` (for DBP token contract)
  - `gameplay.acc` (for gameplay contract)
- [ ] **Fund Accounts**: Ensure both accounts have testnet WAX tokens
- [ ] **Verify Accounts**: `cleos -u https://testnet.wax.pink.gg get account dbptoken.acc`

### **1.7 Deploy Contracts**
- [ ] **Run Deployment**: `./deploy_contracts.sh`
- [ ] **Manual Alternative**: Follow `DEPLOYMENT.md` for manual cleos commands
- [ ] **Set Permissions**: Configure eosio.code permissions
- [ ] **Configure RNG**: Set orng.wax for RNG callbacks
- [ ] **Expected Output**:
  - ✅ DBP Token contract deployed to `dbptoken.acc`
  - ✅ Gameplay contract deployed to `gameplay.acc`
  - ✅ Permissions configured correctly

## 🔧 **Phase 2: Contract Initialization & Unity Setup**

### **2.1 Initialize DBP Token**
- [ ] **Create Token**: `cleos -u https://testnet.wax.pink.gg push action dbptoken.acc create '["dbptoken.acc", "1000000.0000 DBP"]' -p dbptoken.acc@active`
- [ ] **Optional**: Issue initial supply for testing
- [ ] **Verify Token**: Check token statistics and supply

### **2.2 Configure Gameplay Contract**
- [ ] **Set Token Contract**: `cleos -u https://testnet.wax.pink.gg push action gameplay.acc settoken '["dbptoken.acc"]' -p gameplay.acc@active`
- [ ] **Set RNG Oracle**: `cleos -u https://testnet.wax.pink.gg push action gameplay.acc setrng '["orng.wax"]' -p gameplay.acc@active`
- [ ] **Verify Configuration**: Check contract configuration

### **2.3 Unity Client Setup**
- [ ] **Open Unity**: Unity 2022 LTS or newer
- [ ] **Load Project**: Open `unity-client/` project
- [ ] **Update Configuration**: Modify `GameConfig.cs` with deployed contract addresses:
  ```csharp
  public const string TOKEN_CONTRACT = "dbptoken.acc";
  public const string GAMEPLAY_CONTRACT = "gameplay.acc";
  ```
- [ ] **Import SDK**: Ensure WAX Cloud Wallet SDK v2+ is configured for testnet
- [ ] **Test Connection**: Verify Unity can connect to testnet

## 🧪 **Phase 3: QA Testing & Validation**

### **3.1 Follow QA_HANDOFF.md Test Cases**
- [ ] **Test Case 1**: Contract Deployment (cases 1-3)
- [ ] **Test Case 2**: Token Initialization
- [ ] **Test Case 3**: Gameplay Configuration
- [ ] **Test Case 4**: Unity Client Connection
- [ ] **Test Case 5**: BLTZ Gameplay (cases 5-6)
- [ ] **Test Case 6**: Multiple Plays Validation

### **3.2 Wallet Connection Testing**
- [ ] **Connect Wallet**: WAX Cloud Wallet integration
- [ ] **Display Account**: Show account information
- [ ] **Show Balance**: Display DBP balance (should be 0 initially)
- [ ] **Error Handling**: Test connection failures gracefully

### **3.3 Gameplay Testing**
- [ ] **Submit BLTZ Action**: Verify transaction submission
- [ ] **Nonce Generation**: Confirm unique nonce creation
- [ ] **RNG Integration**: Monitor RNG Oracle callback
- [ ] **Success Rate**: Verify 35% success rate over 100+ plays (±5% tolerance)
- [ ] **Token Minting**: Confirm 1 DBP token minted on successful plays

### **3.4 UI Flow Testing**
- [ ] **Start Screen**: Displays correctly with wallet status
- [ ] **Resolving Screen**: Shows transaction progress
- [ ] **Result Screen**: Displays outcome and updated balance
- [ ] **Error States**: Handle and display errors appropriately

### **3.5 Stress Testing**
- [ ] **Multiple Plays**: Run 1,000 automated plays
- [ ] **Performance**: Check response times and reliability
- [ ] **Edge Cases**: Test boundary conditions and error scenarios
- [ ] **Nonce Protection**: Verify no replay attacks possible

## 📊 **Phase 4: Documentation & Reporting**

### **4.1 Record QA Results**
- [ ] **Create QA_RESULTS.md**: Use template from QA_HANDOFF.md
- [ ] **Environment Details**: Document OS, CDT version, Unity version
- [ ] **Test Outcomes**: Record pass/fail for each test case
- [ ] **Issues Found**: Document any problems encountered
- [ ] **Performance Metrics**: Record response times and success rates

### **4.2 Update Documentation**
- [ ] **Update DEPLOYMENT_MILESTONE.md**: Append deployment logs
- [ ] **Update TEST_REPORT.md**: Add real-world testing results
- [ ] **Create POST_BETA_ROADMAP.md**: Plan for FlexBLTZ/CP/leaderboard/NFTs
- [ ] **WAX 2025 Features**: Document Antelope 3.2+ integration plans

### **4.3 CI/CD Setup**
- [ ] **GitHub Actions**: Set up for contract linting
- [ ] **Unity Builds**: Automate Unity client builds
- [ ] **Test Automation**: Implement automated testing pipeline
- [ ] **Deployment Pipeline**: Automate testnet deployments

## 🎯 **Phase 5: Post-Validation & Cleanup**

### **5.1 Update Repository**
- [ ] **Update README.md**: Add "WAX Testnet Deployed & E2E QA Validated"
- [ ] **Update HANDOFF_REPORT.md**: Document successful deployment
- [ ] **Reference QA_RESULTS.md**: Link to detailed results
- [ ] **Status Update**: "All tests passed; post-Beta planning underway"

### **5.2 Archive Old Directories**
- [ ] **Move Pre-merge Folders**: Move to `archive/` directory
- [ ] **Keep Primary**: Maintain `dodge-bltz-beta/` as primary
- [ ] **Clean Repository**: Remove redundant files and directories
- [ ] **Update References**: Ensure all documentation points to correct locations

### **5.3 Create Branch & PR**
- [ ] **Create Branch**: `testnet-qa-complete`
- [ ] **Push Changes**: All updates and documentation
- [ ] **Submit PR**: "WAX Testnet Deployment, E2E QA Validation, & Post-Beta Prep"
- [ ] **Attach Files**: Include simulation CSV and supply_curve.png if available

## 📈 **Success Criteria**

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
- ✅ Post-Beta roadmap created
- ✅ Repository properly organized

## 🚨 **Risk Mitigation**

### **Technical Risks**
- **CDT Compatibility**: Use latest Antelope CDT v4.1.0+
- **Testnet Stability**: Have fallback testnet endpoints
- **RNG Oracle**: Monitor rate limits and performance

### **Process Risks**
- **Documentation**: Keep detailed logs of all steps
- **Testing**: Comprehensive test coverage
- **Rollback**: Plan for deployment rollback if needed

## 🎉 **Completion Checklist**

- [ ] **Environment Setup**: Ubuntu 20.04+ with Antelope CDT v4.1.0+
- [ ] **Contract Deployment**: Both contracts deployed successfully
- [ ] **Contract Initialization**: Token and gameplay configured
- [ ] **Unity Testing**: Client connects and functions correctly
- [ ] **QA Validation**: All test cases pass
- [ ] **Documentation**: Complete results and logs recorded
- [ ] **Repository**: Updated and organized
- [ ] **PR Submitted**: Ready for review and merge

**Status**: ⏳ **READY FOR EXECUTION**

**Next Goal**: 🚀 **Complete WAX Testnet Deployment & QA Validation** 