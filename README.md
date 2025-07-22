# my-hamballer-game
HamBaller.xyz Plush Paper v1.1
1. Project Overview
HamBaller.xyz is a Web3-native, Abstract Blockchain-based dodgeball survival game called 'Dodge
& HODL'.
Players enter fast-paced matches with avatars, dodge boosts, and HODL their position to earn
rewards.
The game emphasizes skill, timing, and risk-reward dynamics using tokenized items and RNG
elements.
Target audience: 18-35 crypto-savvy players comfortable with wallets, NFTs, and GameFi
mechanics.
2. Core Gameplay
Loop: Entry (CP/DBP/NFT) -> Slipnode (Dodge/Jump/Dive) -> Bonus Throw -> HODL/Climb
Slipnodes are narrow floating platforms that visually enhance tension (no input impact).
Actions and outcomes depend on RNG and avatar stats (DAFs - Dodge Avatar Fragments).
Players earn CP (progression) and DBP (scarce ERC-20) based on match outcomes.
3. Tokenomics
CP: Off-chain progression currency earned every round.
DBP: On-chain scarce ERC-20 token minted through successful bonuses and HODL outcomes.
CP can convert to DBP at 100:1. All DBP transactions incur a 10% fee for burn/buyback.
No infinite mints. Tokens are earned, not airdropped, with deflationary design.
4. NFTs and Utility
Boost NFTs: Burnable power-ups like Reflex Sticker or Jump Sticker.
Dodgeproof NFTs: Proof-of-human item to block bots and spoofers.
Mascots (DAFs): Character NFTs with narrative traits and gameplay perks.
NFTs are limited and integrated into progression and DAO voting.
5. Lore and Narrative
Set in the Abstract Arena after the Minting Wars, players embody HamBallersplush fighters evolved
from meme codes.
Seasonal arcs explore virtue development, glitches, mascot corruption, and lore-driven quests.
DAO votes unlock story branches, forks (e.g., Echo vs. Codeline), and community-curated
expansions.
Narrative themes reflect risk-taking, redemption, and ownership.
6. Tech Stack
Frontend: React 18, Vite, Tailwind, RainbowKit (wallets)
Backend: Node.js, Supabase, Express, WebSockets (real-time)
Smart Contracts: DBPToken.sol (ERC-20), BoostNFT.sol (ERC-1155), HODLManager.sol
Thirdweb for minting and Abstract Testnet for deployment.
7. Anti-Bot Features
Signed player moves + time-limited reactions to verify live play.
Dodgeproof NFTs and CAPTCHA-backed access to prevent automation.
DAO treasury funds audits and random game integrity checks.
Replay system with bot flagging and community review tools.
8. Governance
Phase-based DAO: Preseason votes, Phase 2 treasury and features voting.
DBP staking unlocks badge voting rights and DAO proposals.
Community decisions affect roadmap, expansions, and lore direction.
Anti-toxicity measures include code of conduct and staking penalties.
9. Roadmap (2025+)
Preseason: MVP loop, rewards, NFT packs (10K wallets goal).
Phase 1: Dodgeproof launch, mascot reveals, replay bots.
Phase 2: Vaults, ladders, DAO funding, Roblox/WAX expansions.
Cross-chain plans, mobile PWA, and community meme integrations.
10. Disclaimers
This whitepaper is not financial advice or a token offering document.
Gameplay, mechanics, and token design are experimental and may evolve.
All in-game assets are subject to regulatory compliance and platform terms.
HamBaller.xyz operates transparently but assumes no responsibility for player risks.

---

## 📣 Development Status Update

✅ **Beta MVP PR merged. All tests passed, including Phase 10.2A UI polish.**  
Ready for QA and mainnet readiness steps.  
Scope confirmed via `/docs/MVP_CHECKLIST.md`.  
Tracks with dev reports v2.2, whitepaper v3.1, and Year 1 supply simulation (~169k DBP @ capped DAU).