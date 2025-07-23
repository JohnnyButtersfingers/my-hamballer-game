# Unity WebGL Build Configuration
*Dodge BLTZ v1.0 Beta Production Build Settings*

## 🎯 **Build Overview**

**Target**: Production WebGL Build  
**Version**: 1.0.0-beta  
**Output**: `builds/webgl-v1.0-beta/`  
**Template**: ProfessionalWAXTemplate  

---

## 🔧 **Step-by-Step Build Process**

### **Step 1: Project Setup**
1. Open Unity Hub
2. Open the Dodge BLTZ project
3. Ensure project is on the correct branch: `cursor/p0-final-ui-wallet`
4. Verify all scripts compile without errors

### **Step 2: Platform Switch**
1. Go to `File → Build Settings`
2. Select `WebGL` platform
3. Click `Switch Platform` (if not already selected)
4. Wait for platform switch to complete

### **Step 3: Build Settings Configuration**

#### **Scenes in Build**
- [ ] Ensure main game scene is included
- [ ] Verify scene order is correct
- [ ] Remove any test/debug scenes

#### **WebGL Settings**
```
Compression Format: Gzip ✅
Development Build: ❌ UNCHECKED (Production)
Autoconnect Profiler: ❌ UNCHECKED
Script Debugging: ❌ UNCHECKED
Scripts Only Build: ❌ UNCHECKED
```

### **Step 4: Player Settings**

#### **Company & Product**
```
Company Name: HamBaller
Product Name: Dodge BLTZ Beta
Version: 1.0.0-beta
```

#### **Icon & Cursor Settings**
```
Default Icon: [Game Icon]
Default Cursor: Default
Cursor Hotspot: (0, 0)
```

#### **Resolution & Presentation**
```
Default Canvas Width: 1920
Default Canvas Height: 1080
Run In Background: ✅ CHECKED
```

#### **WebGL Template**
```
Template: ProfessionalWAXTemplate ✅
(Custom template with WAX integration)
```

### **Step 5: Publishing Settings**

#### **Memory**
```
Memory Size: 512 MB
Enable Exceptions: Explicitly Thrown Exceptions Only
Compression Format: Gzip
Name Files As Hashes: ✅ CHECKED
Data Caching: ✅ CHECKED
```

#### **Code Optimization**
```
Code Optimization: Master
Managed Stripping Level: Medium
Vertex Compression: Mix
Optimize Mesh Data: ✅ CHECKED
```

#### **Graphics**
```
Color Space: Linear
Auto Graphics API: ✅ CHECKED
Graphics API: WebGL 2.0
```

### **Step 6: XR Settings**
```
Virtual Reality Supported: ❌ UNCHECKED
```

### **Step 7: Audio Settings**
```
Disable Unity Audio: ❌ UNCHECKED
Sample Rate Setting: DSP Buffer Size
DSP Buffer Size: Best Performance
```

---

## 🏗️ **Build Execution**

### **Pre-Build Checklist**
- [ ] All scripts compile successfully
- [ ] No console errors or warnings
- [ ] WAX integration scripts are included
- [ ] Custom WebGL template is selected
- [ ] Build settings configured correctly

### **Build Process**
1. Click `Build` button in Build Settings
2. Create new folder: `builds/webgl-v1.0-beta/`
3. Select the folder and start build
4. Wait for build completion (5-15 minutes typical)

### **Build Verification**
```bash
# Check build output structure
ls -la builds/webgl-v1.0-beta/
# Should contain:
# - index.html
# - Build/ directory
# - TemplateData/ directory
# - StreamingAssets/ (if any)
```

---

## 📊 **Expected Build Output**

### **File Structure**
```
builds/webgl-v1.0-beta/
├── index.html                 (Custom WAX template)
├── Build/
│   ├── webgl-v1.0-beta.loader.js
│   ├── webgl-v1.0-beta.framework.js.gz
│   ├── webgl-v1.0-beta.data.gz
│   ├── webgl-v1.0-beta.wasm.gz
│   └── webgl-v1.0-beta.symbols.json.gz
├── StreamingAssets/           (if any)
└── TemplateData/
    ├── favicon.ico
    ├── fullscreen-button.png
    ├── progress-bar-empty-dark.png
    ├── progress-bar-full-dark.png
    ├── unity-logo-dark.png
    └── webgl-logo.png
```

### **File Size Targets**
| File | Target Size | Description |
|------|-------------|-------------|
| .wasm.gz | < 30MB | WebAssembly binary |
| .data.gz | < 20MB | Game assets |
| .framework.js.gz | < 5MB | Unity framework |
| .loader.js | < 100KB | Unity loader |
| index.html | < 50KB | Custom template |

---

## 🔍 **Build Validation**

### **Local Testing**
1. **Serve Locally with HTTPS**:
   ```bash
   # Option A: Using Python
   cd builds/webgl-v1.0-beta/
   python3 -m http.server 8000 --bind localhost
   
   # Option B: Using Node.js http-server with SSL
   npm install -g http-server
   http-server -p 8000 -c-1
   ```

2. **Open in Browser**:
   - Navigate to `https://localhost:8000`
   - Check developer console for errors
   - Verify WAX SDK loads correctly

### **WAX Integration Validation**
```javascript
// Open browser console and check:
// 1. WAX SDK loaded
console.log(window.waxWallet);

// 2. No HTTPS errors
// 3. Unity instance created
console.log(window.unityInstance);

// 4. No CORS errors
// 5. All assets load successfully
```

### **Performance Check**
- [ ] Initial load time < 30 seconds
- [ ] Memory usage < 512MB
- [ ] No console errors
- [ ] Smooth frame rate
- [ ] Mobile responsive design

---

## 🚨 **Common Build Issues**

### **Build Fails**
| Issue | Cause | Solution |
|-------|-------|----------|
| Compilation errors | Script errors | Fix all compiler errors first |
| Out of memory | Large assets | Optimize textures and audio |
| Platform not switched | Wrong platform | Switch to WebGL platform |

### **Runtime Issues**
| Issue | Cause | Solution |
|-------|-------|----------|
| White screen | Template issue | Verify template selection |
| WAX not loading | HTTPS required | Serve over HTTPS |
| Assets not loading | MIME types | Configure server MIME types |

### **Performance Issues**
| Issue | Cause | Solution |
|-------|-------|----------|
| Slow loading | Large files | Enable compression |
| High memory | Asset quality | Reduce texture resolution |
| Low frame rate | Graphics settings | Optimize rendering settings |

---

## 📋 **Post-Build Checklist**

### **Technical Validation**
- [ ] Build completes without errors
- [ ] All required files present
- [ ] File sizes within targets
- [ ] WAX template correctly applied
- [ ] No missing dependencies

### **Functional Validation**
- [ ] Game loads in browser
- [ ] WAX SDK initializes
- [ ] No console errors
- [ ] Responsive design works
- [ ] HTTPS compatibility confirmed

### **Ready for Deployment**
- [ ] Build validation passed
- [ ] Local testing completed
- [ ] Performance targets met
- [ ] WAX integration verified
- [ ] Files ready for upload

---

## 🔄 **Build Optimization Tips**

### **Asset Optimization**
```
Textures:
- Use compressed formats (DXT/ETC)
- Power-of-2 dimensions
- Appropriate resolution for usage

Audio:
- Use compressed formats (Vorbis)
- Optimize sample rates
- Remove unused audio

Models:
- Optimize mesh complexity
- Use appropriate LOD levels
- Remove unused vertices
```

### **Code Optimization**
```
Scripts:
- Remove debug logging
- Optimize update loops
- Use object pooling
- Minimize allocations
```

---

## 📞 **Build Support**

### **Common Commands**
```bash
# Check Unity version
/Applications/Unity/Hub/Editor/[VERSION]/Unity.app/Contents/MacOS/Unity -version

# Build from command line (advanced)
/Applications/Unity/Hub/Editor/[VERSION]/Unity.app/Contents/MacOS/Unity \
  -projectPath $(pwd) \
  -buildTarget WebGL \
  -quit \
  -batchmode
```

### **Troubleshooting Resources**
- Unity Console logs
- Browser developer tools
- Unity WebGL documentation
- WAX SDK documentation

---

**Build Date**: [When build was created]  
**Build By**: [Developer name]  
**Unity Version**: [Unity version used]  
**Build Duration**: [Time taken to build]

*Next Step: Run deployment script `./scripts/deploy-webgl.sh`*