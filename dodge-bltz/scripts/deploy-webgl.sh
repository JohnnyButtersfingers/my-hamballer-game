#!/bin/bash

# Dodge BLTZ v1.0 Beta Deployment Script
# WebGL Build and Deploy Automation

set -e  # Exit on any error

# Configuration
VERSION="1.0.0-beta"
BUILD_DIR="builds/webgl-v1.0-beta"
DEPLOY_URL="https://beta.dodgebltz.xyz"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging function
log() {
    echo -e "${BLUE}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
    exit 1
}

success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Check prerequisites
check_prerequisites() {
    log "Checking prerequisites..."
    
    # Check if we're in the right directory
    if [ ! -f "DEPLOY_NOTES.md" ]; then
        error "Please run this script from the dodge-bltz root directory"
    fi
    
    # Check if Unity build exists
    if [ ! -d "$BUILD_DIR" ]; then
        error "WebGL build not found at $BUILD_DIR. Please build in Unity first."
    fi
    
    # Check for required files
    if [ ! -f "$BUILD_DIR/index.html" ]; then
        error "index.html not found in build directory"
    fi
    
    success "Prerequisites check passed"
}

# Validate WebGL build
validate_build() {
    log "Validating WebGL build..."
    
    cd "$BUILD_DIR"
    
    # Check for required files
    local required_files=(
        "index.html"
        "Build"
        "TemplateData"
    )
    
    for file in "${required_files[@]}"; do
        if [ ! -e "$file" ]; then
            error "Required file/directory missing: $file"
        fi
    done
    
    # Check build directory contents
    if [ ! -f "Build/"*.loader.js ]; then
        error "Unity loader.js not found"
    fi
    
    if [ ! -f "Build/"*.wasm* ]; then
        error "WebAssembly files not found"
    fi
    
    # Check for WAX template
    if ! grep -q "waxjs" index.html; then
        warning "WAX SDK integration not found in index.html"
    fi
    
    cd - > /dev/null
    success "Build validation passed"
}

# Check hosting options
check_hosting() {
    log "Checking available hosting options..."
    
    # Check for Vercel CLI
    if command -v vercel &> /dev/null; then
        echo "✅ Vercel CLI available"
        VERCEL_AVAILABLE=true
    else
        echo "❌ Vercel CLI not found"
        VERCEL_AVAILABLE=false
    fi
    
    # Check for Netlify CLI
    if command -v netlify &> /dev/null; then
        echo "✅ Netlify CLI available"
        NETLIFY_AVAILABLE=true
    else
        echo "❌ Netlify CLI not found"
        NETLIFY_AVAILABLE=false
    fi
    
    if [ "$VERCEL_AVAILABLE" = false ] && [ "$NETLIFY_AVAILABLE" = false ]; then
        warning "No hosting CLI tools found. Manual deployment required."
        echo "Install with:"
        echo "  npm install -g vercel"
        echo "  npm install -g netlify-cli"
    fi
}

# Deploy to Vercel
deploy_vercel() {
    log "Deploying to Vercel..."
    
    cd "$BUILD_DIR"
    
    # Create vercel.json configuration
    cat > vercel.json << EOF
{
  "version": 2,
  "name": "dodge-bltz-beta",
  "builds": [
    {
      "src": "**/*",
      "use": "@vercel/static"
    }
  ],
  "routes": [
    {
      "src": "/(.*)",
      "dest": "/\$1"
    }
  ],
  "headers": [
    {
      "source": "/(.*)",
      "headers": [
        {
          "key": "Cross-Origin-Embedder-Policy",
          "value": "require-corp"
        },
        {
          "key": "Cross-Origin-Opener-Policy",
          "value": "same-origin"
        }
      ]
    },
    {
      "source": "/Build/(.*\\.wasm.*)",
      "headers": [
        {
          "key": "Content-Type",
          "value": "application/wasm"
        }
      ]
    },
    {
      "source": "/Build/(.*\\.data.*)",
      "headers": [
        {
          "key": "Content-Type",
          "value": "application/octet-stream"
        }
      ]
    }
  ]
}
EOF
    
    # Deploy
    vercel --prod --yes
    
    cd - > /dev/null
    success "Vercel deployment completed"
}

# Deploy to Netlify
deploy_netlify() {
    log "Deploying to Netlify..."
    
    cd "$BUILD_DIR"
    
    # Create _headers file for Netlify
    cat > _headers << EOF
/*
  Cross-Origin-Embedder-Policy: require-corp
  Cross-Origin-Opener-Policy: same-origin

/Build/*.wasm*
  Content-Type: application/wasm

/Build/*.data*
  Content-Type: application/octet-stream

/Build/*.js
  Content-Type: application/javascript
EOF
    
    # Deploy
    netlify deploy --prod --dir .
    
    cd - > /dev/null
    success "Netlify deployment completed"
}

# Verify deployment
verify_deployment() {
    log "Verifying deployment at $DEPLOY_URL..."
    
    # Check if URL is accessible
    if curl -s --head "$DEPLOY_URL" | head -n 1 | grep -q "200 OK"; then
        success "Site is accessible"
    else
        error "Site is not accessible at $DEPLOY_URL"
    fi
    
    # Check for HTTPS
    if [[ $DEPLOY_URL == https://* ]]; then
        success "HTTPS enabled"
    else
        error "HTTPS not enabled"
    fi
    
    # Check WebGL files
    local base_url="${DEPLOY_URL%/}"
    
    # Check for key WebGL files
    if curl -s --head "$base_url/Build/" | head -n 1 | grep -q "200\|301\|302"; then
        success "Build directory accessible"
    else
        warning "Build directory may not be accessible"
    fi
    
    log "Manual verification required:"
    echo "1. Open $DEPLOY_URL in browser"
    echo "2. Check developer console for errors"
    echo "3. Test WAX wallet connection"
    echo "4. Verify game loads and runs"
}

# Update deployment notes
update_deployment_notes() {
    log "Updating DEPLOY_NOTES.md..."
    
    local current_date=$(date '+%Y-%m-%d %H:%M:%S')
    
    # Update deployment status
    sed -i.bak "s/Build Date: \[TBD\]/Build Date: $current_date/" DEPLOY_NOTES.md
    sed -i.bak "s/Deployment Status\*\*: ⏳ \*\*PENDING\*\*/Deployment Status**: ✅ **DEPLOYED**/" DEPLOY_NOTES.md
    
    # Remove backup file
    rm -f DEPLOY_NOTES.md.bak
    
    success "Deployment notes updated"
}

# Main deployment function
main() {
    echo "🚀 Dodge BLTZ v1.0 Beta Deployment"
    echo "=================================="
    echo ""
    
    check_prerequisites
    validate_build
    check_hosting
    
    echo ""
    echo "Available deployment options:"
    if [ "$VERCEL_AVAILABLE" = true ]; then
        echo "1. Deploy to Vercel"
    fi
    if [ "$NETLIFY_AVAILABLE" = true ]; then
        echo "2. Deploy to Netlify"
    fi
    echo "3. Manual deployment instructions"
    echo "4. Verify existing deployment"
    echo "5. Exit"
    
    echo ""
    read -p "Choose deployment option [1-5]: " choice
    
    case $choice in
        1)
            if [ "$VERCEL_AVAILABLE" = true ]; then
                deploy_vercel
                verify_deployment
                update_deployment_notes
            else
                error "Vercel CLI not available"
            fi
            ;;
        2)
            if [ "$NETLIFY_AVAILABLE" = true ]; then
                deploy_netlify
                verify_deployment
                update_deployment_notes
            else
                error "Netlify CLI not available"
            fi
            ;;
        3)
            log "Manual deployment instructions:"
            echo ""
            echo "1. Upload contents of $BUILD_DIR to your hosting provider"
            echo "2. Ensure SSL is enabled"
            echo "3. Configure MIME types:"
            echo "   .wasm → application/wasm"
            echo "   .data → application/octet-stream"
            echo "4. Set CORS headers for WAX integration"
            echo "5. Test the deployment"
            ;;
        4)
            verify_deployment
            ;;
        5)
            log "Deployment cancelled"
            exit 0
            ;;
        *)
            error "Invalid option"
            ;;
    esac
    
    echo ""
    success "Deployment process completed!"
    echo ""
    echo "Next steps:"
    echo "1. Run QA tests using tests/QARegressionChecklist.md"
    echo "2. Fill out docs/CROSS_PLATFORM_RESULTS.md"
    echo "3. Tag release: git tag v1.0.0-beta"
    echo "4. Merge to main branch after QA approval"
}

# Run main function
main "$@"