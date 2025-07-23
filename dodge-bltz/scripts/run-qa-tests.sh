#!/bin/bash

# Dodge BLTZ v1.0 Beta QA Test Execution Helper
# Automated QA workflow assistant

set -e

# Configuration
VERSION="1.0.0-beta"
DEPLOY_URL="https://beta.dodgebltz.xyz"
QA_CHECKLIST="tests/QARegressionChecklist.md"
RESULTS_FILE="docs/CROSS_PLATFORM_RESULTS.md"
BUG_REPORTS="docs/BUG_REPORTS.md"

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Logging
log() {
    echo -e "${BLUE}[QA]${NC} $1"
}

success() {
    echo -e "${GREEN}[PASS]${NC} $1"
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

error() {
    echo -e "${RED}[FAIL]${NC} $1"
}

# Check prerequisites
check_prerequisites() {
    log "Checking QA prerequisites..."
    
    # Check files exist
    if [ ! -f "$QA_CHECKLIST" ]; then
        error "QA checklist not found: $QA_CHECKLIST"
        exit 1
    fi
    
    if [ ! -f "$RESULTS_FILE" ]; then
        error "Results file not found: $RESULTS_FILE"
        exit 1
    fi
    
    # Check deployment is accessible
    if ! curl -s --head "$DEPLOY_URL" | head -n 1 | grep -q "200 OK"; then
        error "Deployment not accessible at $DEPLOY_URL"
        echo "Please deploy the application first using ./scripts/deploy-webgl.sh"
        exit 1
    fi
    
    success "Prerequisites check passed"
}

# Browser detection
detect_browsers() {
    log "Detecting available browsers..."
    
    BROWSERS=()
    
    # Check for common browsers
    if command -v google-chrome &> /dev/null || command -v chromium-browser &> /dev/null; then
        BROWSERS+=("Chrome")
        echo "✅ Chrome detected"
    fi
    
    if command -v firefox &> /dev/null; then
        BROWSERS+=("Firefox")
        echo "✅ Firefox detected"
    fi
    
    if command -v safari &> /dev/null; then
        BROWSERS+=("Safari")
        echo "✅ Safari detected"
    fi
    
    if [ ${#BROWSERS[@]} -eq 0 ]; then
        warning "No browsers detected automatically. Manual testing required."
    else
        log "Found ${#BROWSERS[@]} browser(s) for automated opening"
    fi
}

# Open test URLs
open_test_urls() {
    local browser="$1"
    
    case $browser in
        "Chrome"|"chrome")
            if command -v google-chrome &> /dev/null; then
                google-chrome "$DEPLOY_URL" &
            elif command -v chromium-browser &> /dev/null; then
                chromium-browser "$DEPLOY_URL" &
            fi
            ;;
        "Firefox"|"firefox")
            firefox "$DEPLOY_URL" &
            ;;
        "Safari"|"safari")
            open -a Safari "$DEPLOY_URL" &
            ;;
        *)
            warning "Unknown browser: $browser"
            ;;
    esac
}

# Test category execution
run_functional_tests() {
    log "Running Functional Tests (TC-001 to TC-022)..."
    echo ""
    echo "🎯 FUNCTIONAL TEST GUIDANCE"
    echo "=========================="
    echo ""
    echo "1. WAX Wallet Connection Flow (TC-001 to TC-008):"
    echo "   - Test wallet connect/disconnect"
    echo "   - Verify session persistence"
    echo "   - Check error handling"
    echo ""
    echo "2. Transaction Submission (TC-009 to TC-015):"
    echo "   - Test transaction signing"
    echo "   - Verify nonce generation"
    echo "   - Check replay protection"
    echo ""
    echo "3. Game Result Processing (TC-016 to TC-022):"
    echo "   - Test win/loss scenarios"
    echo "   - Verify blockchain integration"
    echo "   - Check RNG oracle"
    echo ""
    echo "📝 Mark results in: $QA_CHECKLIST"
    echo "🐛 Log any issues in: $BUG_REPORTS"
    
    read -p "Press Enter when functional tests are complete..."
}

run_ui_tests() {
    log "Running UI/UX Tests (TC-023 to TC-036)..."
    echo ""
    echo "🎨 UI/UX TEST GUIDANCE"
    echo "====================="
    echo ""
    echo "1. SVG Icon Rendering (TC-023 to TC-026):"
    echo "   - Check icon clarity across resolutions"
    echo "   - Test button states (hover, pressed)"
    echo "   - Verify loading animations"
    echo ""
    echo "2. Mobile Touch Targets (TC-030 to TC-036):"
    echo "   - Verify 44px minimum touch targets"
    echo "   - Test responsive design"
    echo "   - Check iOS Safari popup handling"
    echo ""
    echo "💡 Test on multiple screen sizes and orientations"
    
    read -p "Press Enter when UI tests are complete..."
}

run_performance_tests() {
    log "Running Performance Tests (TC-037 to TC-042)..."
    echo ""
    echo "⚡ PERFORMANCE TEST GUIDANCE"
    echo "============================"
    echo ""
    echo "1. Load Time Testing:"
    echo "   - Use browser dev tools (Network tab)"
    echo "   - Test on different connection speeds"
    echo "   - Target: < 30 seconds on standard connection"
    echo ""
    echo "2. Memory Testing:"
    echo "   - Use browser dev tools (Memory tab)"
    echo "   - Monitor during gameplay"
    echo "   - Target: < 512MB RAM usage"
    echo ""
    echo "3. Network Resilience:"
    echo "   - Test with slow connections (3G simulation)"
    echo "   - Test network interruptions"
    echo "   - Verify graceful degradation"
    
    read -p "Press Enter when performance tests are complete..."
}

run_security_tests() {
    log "Running Security & Error Handling Tests (TC-043 to TC-048)..."
    echo ""
    echo "🛡️ SECURITY TEST GUIDANCE"
    echo "========================="
    echo ""
    echo "1. WAX Integration Security:"
    echo "   - Test HTTPS enforcement"
    echo "   - Verify nonce replay protection"
    echo "   - Check popup handling"
    echo ""
    echo "2. Error Scenarios:"
    echo "   - Test with insufficient RAM accounts"
    echo "   - Test network timeouts"
    echo "   - Test invalid responses"
    echo ""
    echo "3. Browser Compatibility:"
    echo "   - Test popup blockers"
    echo "   - Test different security settings"
    echo "   - Verify CORS handling"
    
    read -p "Press Enter when security tests are complete..."
}

# Platform-specific testing
test_platform() {
    local platform="$1"
    
    log "Testing platform: $platform"
    echo ""
    echo "🌐 PLATFORM: $platform"
    echo "========================"
    echo ""
    echo "Test URL: $DEPLOY_URL"
    echo ""
    echo "Key items to verify:"
    echo "1. ✅ Site loads correctly"
    echo "2. ✅ WAX wallet connection works"
    echo "3. ✅ Transaction signing functional"
    echo "4. ✅ Game UI renders properly"
    echo "5. ✅ No console errors"
    echo "6. ✅ Performance meets targets"
    echo ""
    
    # Open browser if possible
    case $platform in
        "Chrome Desktop")
            open_test_urls "Chrome"
            ;;
        "Firefox Desktop")
            open_test_urls "Firefox"
            ;;
        "Safari Desktop")
            open_test_urls "Safari"
            ;;
        *)
            echo "Manual browser opening required for: $platform"
            ;;
    esac
    
    echo "📝 Update results in: $RESULTS_FILE"
    echo "🐛 Log any issues in: $BUG_REPORTS"
    echo ""
    
    read -p "Press Enter when $platform testing is complete..."
}

# Generate test report
generate_report() {
    log "Generating QA test report..."
    
    local report_file="qa-test-report-$(date +%Y%m%d-%H%M%S).md"
    
    cat > "$report_file" << EOF
# QA Test Report - Dodge BLTZ v1.0 Beta
*Generated: $(date)*

## Test Summary
- **Version**: $VERSION
- **URL**: $DEPLOY_URL
- **Date**: $(date +%Y-%m-%d)
- **Tester**: [Name]

## Test Execution Status
- [ ] Functional Tests (TC-001 to TC-022)
- [ ] UI/UX Tests (TC-023 to TC-036)
- [ ] Performance Tests (TC-037 to TC-042)
- [ ] Security Tests (TC-043 to TC-048)

## Platform Testing Status
- [ ] Chrome Desktop
- [ ] Firefox Desktop
- [ ] Safari Desktop
- [ ] Chrome Mobile
- [ ] Safari iOS

## Issues Found
[Link to: $BUG_REPORTS]

## Performance Results
- Load Time: [X] seconds
- Memory Usage: [X] MB
- Console Errors: [X] errors

## Release Recommendation
- [ ] ✅ PASS - Ready for release
- [ ] ⚠️ CONDITIONAL - Minor issues, can release with notes
- [ ] ❌ FAIL - Critical issues, cannot release

## Comments
[Additional notes and observations]

---
*For detailed test results, see: $QA_CHECKLIST*
*For cross-platform results, see: $RESULTS_FILE*
EOF

    success "Test report generated: $report_file"
}

# Main menu
show_menu() {
    echo ""
    echo "🧪 Dodge BLTZ v1.0 Beta QA Test Suite"
    echo "====================================="
    echo ""
    echo "Available options:"
    echo "1. Run Full QA Test Suite (Guided)"
    echo "2. Run Functional Tests Only"
    echo "3. Run UI/UX Tests Only"
    echo "4. Run Performance Tests Only"
    echo "5. Run Security Tests Only"
    echo "6. Test Specific Platform"
    echo "7. Open Test URL in Browsers"
    echo "8. Generate Test Report"
    echo "9. View Test Progress"
    echo "10. Exit"
    echo ""
}

# Platform selection menu
select_platform() {
    echo ""
    echo "Select platform to test:"
    echo "1. Chrome Desktop"
    echo "2. Firefox Desktop"
    echo "3. Safari Desktop"
    echo "4. Chrome Mobile (manual)"
    echo "5. Safari iOS (manual)"
    echo "6. All Desktop Platforms"
    echo ""
    read -p "Choose platform [1-6]: " platform_choice
    
    case $platform_choice in
        1) test_platform "Chrome Desktop" ;;
        2) test_platform "Firefox Desktop" ;;
        3) test_platform "Safari Desktop" ;;
        4) test_platform "Chrome Mobile" ;;
        5) test_platform "Safari iOS" ;;
        6) 
            test_platform "Chrome Desktop"
            test_platform "Firefox Desktop"
            test_platform "Safari Desktop"
            ;;
        *) error "Invalid selection" ;;
    esac
}

# View progress
view_progress() {
    log "QA Test Progress Summary"
    echo ""
    echo "📊 Test Files Status:"
    echo "====================="
    
    if [ -f "$QA_CHECKLIST" ]; then
        local completed=$(grep -c "✅" "$QA_CHECKLIST" 2>/dev/null || echo "0")
        local pending=$(grep -c "⏳" "$QA_CHECKLIST" 2>/dev/null || echo "48")
        echo "QA Checklist: $completed completed, $pending pending"
    fi
    
    if [ -f "$BUG_REPORTS" ]; then
        local bugs=$(grep -c "## Bug #" "$BUG_REPORTS" 2>/dev/null || echo "0")
        echo "Bug Reports: $bugs issues logged"
    fi
    
    echo ""
    echo "📁 Files to review:"
    echo "- QA Checklist: $QA_CHECKLIST"
    echo "- Cross-Platform Results: $RESULTS_FILE"
    echo "- Bug Reports: $BUG_REPORTS"
}

# Main execution
main() {
    check_prerequisites
    detect_browsers
    
    while true; do
        show_menu
        read -p "Choose option [1-10]: " choice
        
        case $choice in
            1)
                run_functional_tests
                run_ui_tests
                run_performance_tests
                run_security_tests
                log "Full QA test suite completed!"
                ;;
            2) run_functional_tests ;;
            3) run_ui_tests ;;
            4) run_performance_tests ;;
            5) run_security_tests ;;
            6) select_platform ;;
            7)
                log "Opening test URL in available browsers..."
                for browser in "${BROWSERS[@]}"; do
                    open_test_urls "$browser"
                done
                ;;
            8) generate_report ;;
            9) view_progress ;;
            10)
                log "QA testing session ended"
                exit 0
                ;;
            *)
                error "Invalid option: $choice"
                ;;
        esac
    done
}

# Run main function
main "$@"