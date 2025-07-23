# 📝 Commit Messages for UI Bug Fixes

## Critical Issues (P0)

### Issue #001: Color Palette Implementation
```bash
git commit -m "feat(ui): add centralized color palette system

- Add UIColorPalette.cs for centralized color management
- Include accessibility contrast validation utilities
- Prepare foundation for approved color theme implementation
- Add semantic color enumeration for consistent usage

Addresses: Issue #001 (Critical)
TODO: Update colors once design team approves final palette"
```

### Issue #002: WAX Integration Testing
```bash
git commit -m "fix(integration): enhance WAX wallet connection handling

- Add comprehensive error handling for wallet connection failures
- Improve transaction timeout handling and user feedback
- Add testnet configuration validation
- Enhance mobile browser compatibility checks

Addresses: Issue #002 (Critical)
Testing: Requires WAX testnet deployment validation"
```

### Issue #003: Brand Messaging Update
```bash
git commit -m "content(ui): update result screen messaging for brand consistency

- Change win message to 'BLTZ SUCCESSFUL!' for brand alignment
- Update loss message to 'BLTZ FAILED' with encouragement
- Use centralized color system for text colors
- Improve DBP reward display format with precision

Addresses: Issue #003 (Critical)
Pending: Final brand messaging approval from marketing"
```

## Major Issues (P1)

### Issue #004: Visual Assets Implementation
```bash
git commit -m "assets(ui): implement missing visual icons and elements

- Add DBP token icon with consistent branding
- Implement RNG dice icon for resolving screen
- Add wallet connection status indicators
- Create loading spinner with smooth animations
- Add success/failure state icons

Addresses: Issue #004 (Major)
Assets: SVG format for scalability, multiple resolutions"
```

### Issue #005: Mobile Responsiveness
```bash
git commit -m "responsive(ui): enhance mobile device compatibility

- Implement 44px minimum touch targets for accessibility
- Add responsive layout adjustments for mobile screens
- Improve viewport configuration and safe area handling
- Optimize WAX wallet popup behavior on mobile browsers
- Add touch feedback for better mobile UX

Addresses: Issue #005 (Major)
Testing: Validated on iOS Safari and Chrome Android"
```

## Minor Issues (P2)

### Issue #006: Typography Standardization
```bash
git commit -m "design(ui): standardize typography system

- Define consistent font sizing approach (responsive vs fixed)
- Update UI_Brief.md with approved typography specifications
- Implement typography presets for Unity UI components
- Ensure readability across all screen sizes

Addresses: Issue #006 (Minor)
Documentation: Updated style guide included"
```

### Issue #007: Animation Polish
```bash
git commit -m "animation(ui): implement smooth transitions and micro-interactions

- Add 300ms ease-out transitions between screens
- Implement button press feedback animations (100ms)
- Add loading spinner with smooth rotation
- Ensure 60fps performance on target devices
- Add accessibility considerations for reduced motion

Addresses: Issue #007 (Minor)
Performance: Validated on mobile and desktop platforms"
```

## Combined Commits (if working on multiple issues)

### Color Palette + Brand Messaging
```bash
git commit -m "feat(ui): implement unified color system and brand messaging

- Add centralized UIColorPalette with accessibility validation
- Update result screen with approved brand messaging
- Implement semantic color usage throughout UI
- Prepare foundation for final design approval

Addresses: Issues #001, #003 (Critical)
Status: Awaiting design team final approval"
```

### Mobile + Responsive Improvements
```bash
git commit -m "mobile(ui): comprehensive mobile experience improvements

- Implement proper touch targets and responsive layouts
- Add mobile-specific optimizations for WAX wallet integration
- Enhance typography scaling for mobile readability
- Optimize animations for mobile device performance

Addresses: Issues #005, #006 (Major/Minor)
Testing: Cross-browser mobile validation required"
```

## Naming Conventions

### Commit Type Prefixes
- `feat`: New feature or enhancement
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks
- `content`: Content or copy changes
- `assets`: Asset-related changes
- `mobile`: Mobile-specific improvements
- `responsive`: Responsive design changes
- `animation`: Animation and transition work

### Issue Reference Format
- Always include "Addresses: Issue #XXX (Priority)"
- Add status notes for dependencies or blockers
- Include testing requirements if applicable

### Example Full Commit
```bash
git add .
git commit -m "feat(ui): implement centralized color palette system

- Add UIColorPalette.cs for consistent color management
- Include WCAG 2.1 AA contrast validation utilities  
- Update GameplayManager to use semantic colors
- Prepare foundation for approved Retro/Arcade theme

Addresses: Issue #001 (Critical - Launch Blocker)
TODO: Update color values once design approval received
Testing: Accessibility validation required"
```

## Pre-Commit Checklist

Before committing UI changes:
- [ ] Test on multiple screen sizes (mobile, tablet, desktop)
- [ ] Verify color contrast meets accessibility standards
- [ ] Ensure consistent messaging and branding
- [ ] Test interactive elements (buttons, transitions)
- [ ] Update documentation if needed
- [ ] Link to appropriate GitHub issue number
- [ ] Include any testing requirements or dependencies