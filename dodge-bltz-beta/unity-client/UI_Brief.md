# Dodge BLTZ UI Design Brief

This document outlines the user interface design for the Dodge BLTZ Beta MVP, focusing on three core screens that provide a clean, intuitive gaming experience.

## Design Principles

- **Simplicity First**: Clean, uncluttered interface focusing on core actions
- **Web3 Friendly**: Clear wallet connection status and transaction feedback
- **Mobile Responsive**: Scalable UI that works across devices
- **Accessibility**: High contrast, readable fonts, clear visual hierarchy
- **Gaming Aesthetic**: Energetic colors and smooth animations

## Color Palette

```
Primary: #FF6B35 (Vibrant Orange)
Secondary: #004E89 (Deep Blue)
Success: #2ECC71 (Green)
Error: #E74C3C (Red)
Background: #1A1A2E (Dark Purple)
Surface: #16213E (Navy)
Text Primary: #FFFFFF (White)
Text Secondary: #BDC3C7 (Light Gray)
```

## Typography

- **Headers**: Bold, sans-serif (Montserrat or similar)
- **Body Text**: Regular, clean sans-serif (Open Sans or similar)
- **UI Elements**: Medium weight for buttons and labels
- **Sizes**: Responsive scaling from 14px (mobile) to 18px (desktop) base

## Screen 1: Start Screen

### Layout Structure
```
┌─────────────────────────────────────┐
│              HEADER                 │
│         "DODGE BLTZ BETA"           │
│                                     │
│         WALLET STATUS CARD          │
│    ┌─────────────────────────────┐  │
│    │ 👤 Account: [Connected/Not] │  │
│    │ 💰 DBP Balance: X.XXXX     │  │
│    └─────────────────────────────┘  │
│                                     │
│         ACTION BUTTONS              │
│    ┌─────────────────────────────┐  │
│    │    [CONNECT WALLET]         │  │
│    │         OR                  │  │
│    │      [PLAY BLTZ]           │  │
│    └─────────────────────────────┘  │
│                                     │
│         GAME INFO                   │
│    "35% chance to win 1 DBP"       │
│                                     │
│              FOOTER                 │
│         Beta v1.0 • WAX            │
└─────────────────────────────────────┘
```

### Key Components

#### Header Section
- **Title**: "DODGE BLTZ" in large, bold typography
- **Subtitle**: "Beta" badge to indicate testing phase
- **Logo**: Simple dodgeball icon or abstract geometric shape

#### Wallet Status Card
- **Connection Status**: Clear visual indicator (green = connected, red = disconnected)
- **Account Name**: Display connected WAX account (e.g., "player.wam")
- **DBP Balance**: Current token balance with 4 decimal precision
- **Update Indicator**: Subtle loading animation when fetching balance

#### Action Area
- **Connect Wallet Button**: 
  - Primary orange color when wallet not connected
  - Full width, rounded corners
  - Icon: Wallet symbol
  - Text: "Connect WAX Wallet"
  
- **Play BLTZ Button**:
  - Prominent, animated glow effect
  - Disabled state when wallet not connected
  - Icon: Lightning bolt or play symbol
  - Text: "PLAY BLTZ"

#### Info Section
- **Game Rules**: Simple explanation of success rate and rewards
- **Status Messages**: Dynamic text showing current state or errors

### Interactive States

1. **Disconnected State**:
   - Show "Connect Wallet" button
   - Gray out play area
   - Display "Connect your wallet to play" message

2. **Connected State**:
   - Show account info and balance
   - Enable "Play BLTZ" button
   - Display "Ready to play!" message

3. **Loading State**:
   - Show spinner for balance loading
   - Disable buttons during operations

## Screen 2: Resolving Screen

### Layout Structure
```
┌─────────────────────────────────────┐
│              HEADER                 │
│         "PROCESSING BLTZ"           │
│                                     │
│         ANIMATION AREA              │
│    ┌─────────────────────────────┐  │
│    │          🎯                 │  │
│    │     [SPINNING LOADER]       │  │
│    │                             │  │
│    │   "Waiting for WAX RNG      │  │
│    │     Oracle result..."       │  │
│    └─────────────────────────────┘  │
│                                     │
│         PROGRESS INFO               │
│    • Transaction submitted         │
│    • Requesting random number      │
│    • Processing result...          │
│                                     │
│         ACTION                      │
│    ┌─────────────────────────────┐  │
│    │       [CANCEL] (optional)   │  │
│    └─────────────────────────────┘  │
└─────────────────────────────────────┘
```

### Key Components

#### Animation Section
- **Central Spinner**: Rotating dodgeball or lightning bolt
- **Pulsing Effect**: Subtle glow animation around the spinner
- **Progress Indicator**: Dots or bars showing oracle processing

#### Status Messages
- **Dynamic Text**: Updates based on processing stage
  - "Submitting transaction..."
  - "Waiting for RNG Oracle..."
  - "Processing result..."
- **Progress Steps**: Visual checklist of completed steps

#### Timing
- **Expected Duration**: 5-30 seconds typical
- **Timeout Handling**: Clear messaging if process takes too long
- **Cancel Option**: Allow users to return to start (with warning)

### Animation Details
- **Spinner Speed**: Smooth 1-2 second rotation cycle
- **Color Transitions**: Subtle color shifts during processing
- **Micro-animations**: Small bounces or pulses to maintain engagement

## Screen 3: Result Screen

### Layout Structure
```
┌─────────────────────────────────────┐
│              HEADER                 │
│          [SUCCESS/FAILED]           │
│                                     │
│         RESULT DISPLAY              │
│    ┌─────────────────────────────┐  │
│    │        🎉 or 💥           │  │
│    │                             │  │
│    │    "BLTZ SUCCESSFUL!"       │  │
│    │         OR                  │  │
│    │     "BLTZ FAILED"          │  │
│    │                             │  │
│    │  "You earned 1 DBP!"       │  │
│    │         OR                  │  │
│    │   "Better luck next time!"  │  │
│    └─────────────────────────────┘  │
│                                     │
│         UPDATED INFO                │
│    💰 New DBP Balance: X.XXXX      │
│                                     │
│         ACTION BUTTONS              │
│    ┌─────────────────────────────┐  │
│    │      [PLAY AGAIN]           │  │
│    │      [DISCONNECT]           │  │
│    └─────────────────────────────┘  │
└─────────────────────────────────────┘
```

### Success State
- **Color Scheme**: Green accents, celebratory palette
- **Icon**: Trophy, star burst, or checkmark
- **Animation**: Confetti or particle effects
- **Message**: "BLTZ SUCCESSFUL!" in large, bold text
- **Reward Info**: "You earned 1.0000 DBP!" with token icon

### Failure State
- **Color Scheme**: Red accents, neutral palette
- **Icon**: X mark or broken symbol
- **Animation**: Subtle shake or fade effect
- **Message**: "BLTZ FAILED" in clear but not harsh text
- **Encouragement**: "Better luck next time!" to maintain positivity

### Common Elements
- **Updated Balance**: Show new DBP token balance
- **Transaction Info**: Link to view transaction on block explorer (optional)
- **Action Buttons**: Clear paths for next actions

## Responsive Design

### Mobile (320px - 768px)
- Single column layout
- Larger touch targets (minimum 44px)
- Simplified typography hierarchy
- Reduced spacing and padding

### Tablet (768px - 1024px)
- Maintain single column with increased margins
- Slightly larger elements
- Enhanced button styling

### Desktop (1024px+)
- Center content with maximum width of 480px
- Add subtle background patterns or gradients
- Enhanced hover effects and animations

## Accessibility

### Visual
- **Contrast**: Minimum 4.5:1 ratio for all text
- **Focus States**: Clear visual indicators for keyboard navigation
- **Color Independence**: Information not conveyed by color alone

### Interaction
- **Touch Targets**: Minimum 44px for mobile interfaces
- **Keyboard Navigation**: Full accessibility via tab navigation
- **Screen Readers**: Proper ARIA labels and semantic markup

### Feedback
- **Loading States**: Clear progress indicators
- **Error Messages**: Descriptive, actionable error text
- **Success Confirmation**: Immediate, clear success feedback

## Animation Guidelines

### Entrance Animations
- **Slide Up**: 300ms ease-out for cards and panels
- **Fade In**: 200ms for text and icons
- **Scale In**: 250ms for buttons and interactive elements

### Loading Animations
- **Spinner**: Continuous rotation, 1.5s duration
- **Pulse**: 2s cycle for waiting states
- **Progress**: Linear progression for known durations

### Feedback Animations
- **Success**: Bounce or spring effect (400ms)
- **Error**: Shake effect (300ms)
- **Button Press**: Scale down/up (150ms)

## Technical Considerations

### Performance
- **Image Optimization**: Use SVG for icons, WebP for images
- **Animation Performance**: Use transform and opacity for smooth 60fps
- **Bundle Size**: Minimize CSS and JavaScript for fast loading

### Cross-Platform
- **WebGL Compatibility**: Ensure UI works in Unity WebGL builds
- **Touch Support**: Proper touch event handling
- **Orientation**: Support both portrait and landscape

### WAX Integration
- **Wallet Popups**: Handle wallet popup windows appropriately
- **Deep Linking**: Support return from wallet authentication
- **Error Handling**: Graceful handling of wallet connection issues

## Implementation Notes

### Unity-Specific
- **UI Framework**: Use Unity UI (uGUI) with Canvas components
- **Responsive Layout**: Utilize Content Size Fitters and Layout Groups
- **Animation**: Unity Animator or DOTween for smooth transitions

### Web Integration
- **WAX Cloud Wallet**: Handle iframe/popup integration seamlessly
- **CORS Handling**: Proper configuration for WAX API calls
- **Browser Compatibility**: Support major browsers (Chrome, Firefox, Safari, Edge)

This UI design provides a clean, professional interface that guides users through the simple but engaging BLTZ gameplay experience while maintaining the technical requirements for Web3 integration.