# my-hamballer-game
HamBaller.xyz Plush Paper v1.1

## 🎯 **Primary Project: Dodge BLTZ Beta MVP**

The unified Beta MVP is located in `dodge-bltz-beta/` - a provably-fair, RNG-based dodgeball mini-game on the WAX blockchain.

### 🚀 **Quick Start**
```bash
cd dodge-bltz-beta
# See QUICKSTART.md for detailed setup instructions
```

### 📚 **Documentation**
- **Project Overview**: `dodge-bltz-beta/README.md`
- **Quick Setup**: `dodge-bltz-beta/QUICKSTART.md`
- **Deployment Guide**: `dodge-bltz-beta/docs/DEPLOYMENT.md`
- **Developer Handoff**: `dodge-bltz-beta/docs/DEVELOPER_HANDOFF.md`
- **Integration Summary**: `dodge-bltz-beta/INTEGRATION_SUMMARY.md`

---

## 📁 **Project Structure**

### **dodge-bltz-beta/** (Unified Beta MVP - WAX Blockchain)
- **Technology**: EOSIO C++ contracts, Unity client, WAX Cloud Wallet
- **Scope**: Single BLTZ action with 35% success rate, $DBP token rewards
- **Status**: Ready for WAX testnet deployment

### **hamballer-game-starter/** (Production Version - Abstract Blockchain)
- **Technology**: Solidity contracts, React frontend, Express backend
- **Scope**: Full game hub with multiple features
- **Status**: Production-ready for Abstract testnet

### **dodge-bltz/** (Original WAX Version - Archived)
- **Technology**: EOSIO C++ contracts, Unity client
- **Scope**: Earlier iteration of WAX version
- **Status**: Superseded by dodge-bltz-beta

---

## 🎮 **Game Overview**

HamBaller.xyz is a Web3-native, blockchain-based dodgeball survival game called 'Dodge & HODL'.

### **Core Gameplay**
- Players enter fast-paced matches with avatars
- Dodge boosts and HODL position to earn rewards
- Emphasizes skill, timing, and risk-reward dynamics
- Uses tokenized items and RNG elements

### **Target Audience**
18-35 crypto-savvy players comfortable with wallets, NFTs, and GameFi mechanics.

### **Tokenomics**
- **CP**: Off-chain progression currency earned every round
- **DBP**: On-chain scarce ERC-20 token minted through successful bonuses and HODL outcomes
- **Conversion**: CP can convert to DBP at 100:1
- **Fees**: All DBP transactions incur a 10% fee for burn/buyback
- **Design**: No infinite mints, tokens are earned not airdropped, deflationary design

### **NFTs and Utility**
- **Boost NFTs**: Burnable power-ups like Reflex Sticker or Jump Sticker
- **Dodgeproof NFTs**: Proof-of-human item to block bots and spoofers
- **Mascots (DAFs)**: Character NFTs with narrative traits and gameplay perks
- **Integration**: Limited NFTs integrated into progression and DAO voting

### **Lore and Narrative**
Set in the Abstract Arena after the Minting Wars, players embody HamBallersplush fighters evolved from meme codes. Seasonal arcs explore virtue development, glitches, mascot corruption, and lore-driven quests.

### **Tech Stack**
- **Frontend**: React 18, Vite, Tailwind, RainbowKit (wallets)
- **Backend**: Node.js, Supabase, Express, WebSockets (real-time)
- **Smart Contracts**: DBPToken.sol (ERC-20), BoostNFT.sol (ERC-1155), HODLManager.sol
- **Deployment**: Thirdweb for minting and Abstract Testnet for deployment

### **Anti-Bot Features**
- Signed player moves + time-limited reactions to verify live play
- Dodgeproof NFTs and CAPTCHA-backed access to prevent automation
- DAO treasury funds audits and random game integrity checks
- Replay system with bot flagging and community review tools

### **Governance**
- Phase-based DAO: Preseason votes, Phase 2 treasury and features voting
- DBP staking unlocks badge voting rights and DAO proposals
- Community decisions affect roadmap, expansions, and lore direction
- Anti-toxicity measures include code of conduct and staking penalties

### **Roadmap (2025+)**
- **Preseason**: MVP loop, rewards, NFT packs (10K wallets goal)
- **Phase 1**: Dodgeproof launch, mascot reveals, replay bots
- **Phase 2**: Vaults, ladders, DAO funding, Roblox/WAX expansions
- **Future**: Cross-chain plans, mobile PWA, and community meme integrations

## QA + SDK Integration
- Ensure Unity WebGL build is hosted with SSL
- Test with real WAX testnet wallet  
- Wallet must support scatter-compatible signing

### **WAX Integration Requirements**
- **Unity WebGL**: SSL hosting required for WAX Cloud Wallet integration
- **Testing Environment**: WAX testnet with gameplayacc2 contract deployment
- **Browser Compatibility**: Chrome, Firefox, Safari (desktop + mobile)
- **Wallet Integration**: Real WAX accounts with scatter-compatible transaction signing

### **Quality Assurance**
- **Test Plan**: `dodge-bltz/tests/QARegressionChecklist.md`
- **Cross-Platform Testing**: Desktop and mobile browser validation
- **Performance Requirements**: <30s load time, smooth mobile experience
- **Security**: Nonce-based replay protection, secure RPC endpoints

### **Disclaimers**
This whitepaper is not financial advice or a token offering document. Gameplay, mechanics, and token design are experimental and may evolve. All in-game assets are subject to regulatory compliance and platform terms. HamBaller.xyz operates transparently but assumes no responsibility for player risks.