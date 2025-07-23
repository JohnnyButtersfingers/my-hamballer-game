# QA Regression Checklist - Dodge BLTZ Beta
*Version: 0.9.0 → 1.0.0 Beta*  
*Branch: cursor/p0-final-ui-wallet*

## 🔍 **Pre-Test Setup Requirements**

### Environment Prerequisites
- [ ] Unity WebGL build hosted with SSL certificate
- [ ] WAX testnet wallet account with funds
- [ ] gameplayacc2 contract deployed on WAX testnet
- [ ] Browser extensions disabled (ad blockers, etc.)
- [ ] Clear browser cache and local storage

### Test Devices/Browsers
- [ ] Chrome Desktop (latest version)
- [ ] Firefox Desktop (latest version)  
- [ ] Safari Desktop (latest version)
- [ ] Chrome Mobile (Android)
- [ ] Safari Mobile (iOS)
- [ ] Edge Desktop (optional)

---

## 🎯 **Core Functionality Tests**

### **1. WAX Wallet Connection Flow**

#### Login Process
- [ ] **TC-001**: Click "Connect Wallet" button
  - Expected: WAX Cloud Wallet popup opens
  - Test URL: `https://all-access.wax.io/cloud-wallet/login/`
- [ ] **TC-002**: Complete login with valid WAX account
  - Expected: Account name displayed, wallet status shows "Connected"
- [ ] **TC-003**: Cancel login during popup
  - Expected: Error message displayed, wallet remains disconnected
- [ ] **TC-004**: Invalid credentials/account
  - Expected: Error handling, no connection established
- [ ] **TC-005**: Network timeout during login
  - Expected: Graceful error handling with retry option

#### Session Management
- [ ] **TC-006**: Refresh page while connected
  - Expected: Session restored automatically
- [ ] **TC-007**: Close and reopen browser
  - Expected: Session persistence works correctly
- [ ] **TC-008**: Manual disconnect via button
  - Expected: Account cleared, UI updated to disconnected state

### **2. Transaction Submission & Signing**

#### Gameplay Transaction Flow
- [ ] **TC-009**: Start game while wallet connected
  - Expected: Transaction popup appears for signing
- [ ] **TC-010**: Sign transaction successfully
  - Expected: Game proceeds, transaction broadcast to blockchain
- [ ] **TC-011**: Reject transaction signing
  - Expected: Game returns to menu, no blockchain submission
- [ ] **TC-012**: Transaction timeout/failure
  - Expected: Error message displayed, game state reset

#### Nonce & Replay Protection
- [ ] **TC-013**: Generate unique nonce for each play
  - Expected: Different nonce value on each attempt
- [ ] **TC-014**: Verify nonce replay protection
  - Expected: Reused nonce should be rejected by contract
- [ ] **TC-015**: Concurrent transactions
  - Expected: Multiple tabs/windows handle nonces correctly

### **3. Game Result Processing**

#### Win/Loss Display
- [ ] **TC-016**: Successful BLTZ (35% probability)
  - Expected: "Victory!" screen, DBP reward notification
- [ ] **TC-017**: Failed BLTZ (65% probability)
  - Expected: "Try Again" screen, no reward
- [ ] **TC-018**: Result screen UI elements
  - Expected: All buttons, text, and animations render correctly
- [ ] **TC-019**: Return to menu after result
  - Expected: Clean state reset for next game

#### Blockchain Integration
- [ ] **TC-020**: Verify transaction on blockchain explorer
  - Expected: Transaction appears in WAX testnet explorer
- [ ] **TC-021**: DBP token balance update (on win)
  - Expected: Player's DBP balance increases by reward amount
- [ ] **TC-022**: RNG oracle integration
  - Expected: Random values from WAX RNG Oracle used for results

---

## 🎨 **UI/UX Visual Tests**

### **4. SVG Icon & Asset Rendering**

#### Cross-Platform Icon Display
- [ ] **TC-023**: WAX wallet icon visibility (desktop)
  - Expected: Clear, crisp icon at all resolutions
- [ ] **TC-024**: Game UI icons (mobile)
  - Expected: Touch-friendly size, no pixelation
- [ ] **TC-025**: Button states (hover, pressed, disabled)
  - Expected: Visual feedback for all interactive states
- [ ] **TC-026**: Loading spinners/animations
  - Expected: Smooth animations during wallet/transaction operations

#### Responsive Design
- [ ] **TC-027**: Portrait mode (mobile)
  - Expected: UI elements properly arranged and readable
- [ ] **TC-028**: Landscape mode (mobile)
  - Expected: Game area optimized for landscape viewing
- [ ] **TC-029**: Different screen resolutions
  - Expected: Scalable UI from 320px to 4K displays

### **5. Mobile Touch Targets**

#### Touch Interaction
- [ ] **TC-030**: Connect Wallet button (minimum 44px touch target)
  - Expected: Easy finger tap without mis-clicks
- [ ] **TC-031**: Play/Start Game button
  - Expected: Responsive touch feedback
- [ ] **TC-032**: Menu navigation buttons
  - Expected: Clear visual feedback on tap
- [ ] **TC-033**: Scroll areas (if any)
  - Expected: Smooth scrolling without interference

#### Mobile Specific Issues
- [ ] **TC-034**: iOS Safari popup handling
  - Expected: WAX wallet popup opens correctly
- [ ] **TC-035**: Android Chrome touch events
  - Expected: No double-tap zoom issues
- [ ] **TC-036**: Mobile keyboard overlay
  - Expected: UI adjusts properly when keyboard appears

---

## ⚡ **Performance & Error Handling**

### **6. Network & Performance Tests**

#### Connection Scenarios
- [ ] **TC-037**: Slow internet connection (3G simulation)
  - Expected: Graceful loading with progress indicators
- [ ] **TC-038**: Network interruption during transaction
  - Expected: Proper error handling and recovery options
- [ ] **TC-039**: WAX testnet node downtime
  - Expected: Fallback RPC endpoints or error messages

#### Resource Loading
- [ ] **TC-040**: WebGL build loading time
  - Expected: Reasonable load time (< 30 seconds on standard connection)
- [ ] **TC-041**: Memory usage on mobile
  - Expected: No crashes or performance degradation
- [ ] **TC-042**: CPU usage during gameplay
  - Expected: Smooth frame rate maintained

### **7. Error Handling & Edge Cases**

#### Wallet Error Scenarios
- [ ] **TC-043**: WAX account with insufficient RAM
  - Expected: Clear error message with resolution steps
- [ ] **TC-044**: Wallet popup blocked by browser
  - Expected: Instructions to enable popups
- [ ] **TC-045**: Contract maintenance/downtime
  - Expected: Informative error message

#### Game State Recovery
- [ ] **TC-046**: Browser crash during transaction
  - Expected: State recovery on reload without duplicate charges
- [ ] **TC-047**: Network reconnection after disconnect
  - Expected: Seamless reconnection without data loss
- [ ] **TC-048**: Invalid transaction responses
  - Expected: Proper error parsing and user feedback

---

## 📊 **Test Execution Tracking**

### **Test Environment Setup**
| Environment | URL | Status | Tester | Date |
|-------------|-----|--------|--------|------|
| WAX Testnet | https://testnet.waxsweden.org | ⏳ | - | - |
| Contract | gameplayacc2 | ⏳ | - | - |
| WebGL Build | [SSL URL] | ⏳ | - | - |

### **Cross-Platform Results Matrix**
| Test Case | Chrome Desktop | Firefox Desktop | Safari Desktop | Chrome Mobile | Safari Mobile | Status |
|-----------|----------------|-----------------|----------------|---------------|---------------|--------|
| TC-001-005 | ⏳ | ⏳ | ⏳ | ⏳ | ⏳ | - |
| TC-006-008 | ⏳ | ⏳ | ⏳ | ⏳ | ⏳ | - |
| TC-009-015 | ⏳ | ⏳ | ⏳ | ⏳ | ⏳ | - |
| TC-016-022 | ⏳ | ⏳ | ⏳ | ⏳ | ⏳ | - |
| TC-023-029 | ⏳ | ⏳ | ⏳ | ⏳ | ⏳ | - |
| TC-030-036 | N/A | N/A | N/A | ⏳ | ⏳ | - |
| TC-037-042 | ⏳ | ⏳ | ⏳ | ⏳ | ⏳ | - |
| TC-043-048 | ⏳ | ⏳ | ⏳ | ⏳ | ⏳ | - |

**Legend**: ✅ Pass | ❌ Fail | ⏳ Pending | ⚠️ Warning | N/A Not Applicable

---

## 🚨 **Critical Issues Checklist**

### **Blocking Issues (Must Fix Before Release)**
- [ ] Wallet connection completely broken
- [ ] Unable to sign transactions
- [ ] Game crashes on mobile devices
- [ ] Duplicate nonce allowing replay attacks
- [ ] DBP tokens not distributed correctly

### **High Priority Issues**
- [ ] Inconsistent UI rendering across browsers
- [ ] Mobile touch targets too small
- [ ] Network errors without proper messaging
- [ ] Session not persisting correctly

### **Medium Priority Issues**
- [ ] Minor visual glitches
- [ ] Slow loading on 3G connections
- [ ] Non-critical error messages unclear

---

## 📝 **Test Sign-Off**

| Role | Name | Signature | Date |
|------|------|-----------|------|
| QA Lead | | | |
| Frontend Dev | | | |
| Blockchain Dev | | | |
| Product Owner | | | |

**Final Release Approval**: ⏳ Pending

*Last Updated: [Current Date]*  
*Next Review: [Review Date]*