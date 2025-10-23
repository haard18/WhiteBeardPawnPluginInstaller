# WhiteBeard Pawn Plugin Installer

Enterprise-grade Windows Installer (.msi) for the WhiteBeard Pawn Plugin for MetaTrader 5, built with WiX Toolset v4.

## 🚀 Quick Start

**New to this project?** → See **[QUICKSTART.md](QUICKSTART.md)**

**Need to generate licenses?** → See **[LICENSE_MANAGEMENT.md](LICENSE_MANAGEMENT.md)**

**Building on Windows?** → See **[WINDOWS_BUILD_GUIDE.md](WINDOWS_BUILD_GUIDE.md)** or **[MAC_TO_WINDOWS.txt](MAC_TO_WINDOWS.txt)**

**Ready to build?** → See **[BUILD.md](BUILD.md)**

## Overview

This installer handles:
- License file verification with RSA digital signatures
- MetaTrader 5 detection and validation
- Plugin deployment to MT5 Plugins directory
- Registry configuration
- Secure installation with administrator privileges

## Project Structure

```
WhiteBeardPawnPluginInstaller/
├── Files/                          # Files to be included in installer
│   ├── PawnPlugin64.dll           # Main plugin DLL (64-bit)
│   ├── LicenseAgreement.pdf       # EULA for installation
│   ├── PawnPlugin_UserGuide.pdf   # User documentation
│   └── example_pawn_plugin.lic    # Example license file
│
├── LicenseGenerator/               # License generation tool (Mac/Windows)
│   ├── Program.cs                 # License generator
│   ├── LicenseGenerator.csproj    # .NET project
│   ├── PrivateKey.xml             # RSA private key (NEVER COMMIT!)
│   └── README.md                  # License tool docs
│
├── CustomActions/                  # C# Custom Action DLLs
│   ├── VerifyLicense.cs           # License verification logic
│   ├── DetectMT5.cs               # MetaTrader 5 detection
│   ├── CopyFiles.cs               # File deployment and registry
│   └── CustomActions.csproj       # .NET project file
│
├── UI/                            # WiX UI Dialog definitions
│   ├── VerifyLicenseDialog.wxs    # License verification screen
│   ├── TermsDialog.wxs            # EULA acceptance screen
│   └── MT5PathDialog.wxs          # MT5 path selection screen
│
├── Product.wxs                    # Main WiX product definition
├── PluginFiles.wxs                # Auto-generated file components
├── build.bat                      # Windows build script
├── generate_license.sh            # Quick license generator (Mac)
├── PublicKey.xml                  # RSA public key for license verification
├── icon.ico                       # Installer icon
├── WhiteBeardPawnPlugin.sln       # Visual Studio solution
├── QUICKSTART.md                  # Quick start guide
├── LICENSE_MANAGEMENT.md          # License workflow guide
├── BUILD.md                       # Build instructions
└── README.md                      # This file
```

## Prerequisites

### On Windows (Build Environment):
1. **WiX Toolset v4** - Download from [https://wixtoolset.org/](https://wixtoolset.org/)
2. **.NET SDK 4.8 or later** - For building Custom Actions
3. **Visual Studio 2019/2022** (Optional) - For IDE support

### On macOS (Development):
- No build tools required
- Use this environment for source code development only
- Transfer to Windows VPS for building

## Installation Flow

1. **Welcome Screen** - Introduction and overview
2. **License Verification** - Detect or browse for `*_pawn_plugin.lic` file
   - Searches `C:\ProgramData\WhiteBeard\` by default
   - Decrypts and validates license using RSA public key
   - Displays Company Name and Email from license
3. **License Agreement** - User must accept EULA
4. **MT5 Path Selection** - Detect or browse for MetaTrader 5 installation
   - Default: `C:\Program Files\MetaTrader 5 Platform\TradeMain`
   - Validates by checking for `terminal64.exe`
5. **Installation Progress** - Copies files and configures system
   - `PawnPlugin64.dll` → `[MT5]\Plugins\`
   - License file → `C:\ProgramData\WhiteBeard\`
   - Registry: `HKLM\Software\WhiteBeard\PawnPlugin`
6. **Finish Screen** - Installation complete with support info

## Building the Installer

### On Windows:

1. **Clone/Copy this project** to your Windows VPS

2. **Replace placeholder files** in `Files/` directory:
   - `PawnPlugin64.dll` - Your compiled plugin
   - `LicenseAgreement.pdf` - Your EULA document
   - `PawnPlugin_UserGuide.pdf` - User guide
   - `example_pawn_plugin.lic` - Example license

3. **Add your RSA public key**:
   - Create `PublicKey.xml` in the project root
   - Format: RSA XML key format

4. **Build the Custom Actions**:
   ```cmd
   cd CustomActions
   dotnet build -c Release
   cd ..
   ```

5. **Run the build script**:
   ```cmd
   build.bat
   ```

6. **Output**: `WhiteBeardPawnPlugin.msi` will be created in the root directory

### Alternative: Manual Build

```cmd
REM Build Custom Actions
cd CustomActions
dotnet build -c Release
cd ..

REM Compile WiX source
wix build Product.wxs UI\VerifyLicenseDialog.wxs UI\TermsDialog.wxs UI\MT5PathDialog.wxs ^
    -ext WixToolset.Util.wixext ^
    -ext WixToolset.UI.wixext ^
    -out WhiteBeardPawnPlugin.msi
```

## Testing the Installer

### Install with verbose logging:
```cmd
msiexec /i WhiteBeardPawnPlugin.msi /l*v install.log
```

### Uninstall:
```cmd
msiexec /x WhiteBeardPawnPlugin.msi /l*v uninstall.log
```

### Silent install (for enterprise deployment):
```cmd
msiexec /i WhiteBeardPawnPlugin.msi /qn /l*v install.log
```

## Configuration

### Update Product Information

Edit `Product.wxs`:
- **UpgradeCode**: Generate a new GUID (keep same for all versions)
- **Version**: Update version number (format: X.Y.Z)
- **Manufacturer**: Company name
- **ARPHELPLINK**: Support URL
- **ARPCONTACT**: Support email

### Custom Actions

Located in `CustomActions/` directory:
- **VerifyLicense.cs**: Modify license decryption logic
- **DetectMT5.cs**: Add alternative MT5 installation paths
- **CopyFiles.cs**: Customize file deployment locations

### UI Customization

- Edit WiX dialog files in `UI/` directory
- Add `icon.ico` for installer icon
- Customize banner images (370x44 px recommended)

## License Verification

The installer expects encrypted license files with this structure:
```
-----BEGIN ENCRYPTED LICENSE-----
[Base64 Encoded Encrypted Data]
Company: [Company Name]
Email: [Company Email]
-----END ENCRYPTED LICENSE-----
```

**Note**: Update `VerifyLicense.cs` with your actual encryption/decryption logic.

## Registry Keys Created

```
HKLM\Software\WhiteBeard\PawnPlugin
├── InstallPath (String) - MT5 installation path
├── LicensePath (String) - License file location
├── Version (String) - Plugin version
├── InstallDate (String) - Installation timestamp
├── CompanyName (String) - Licensed company name
└── CompanyEmail (String) - Licensed company email
```

## Files Deployed

| Source | Destination |
|--------|-------------|
| `PawnPlugin64.dll` | `[MT5]\Plugins\PawnPlugin64.dll` |
| `*_pawn_plugin.lic` | `C:\ProgramData\WhiteBeard\*.lic` |
| `LicenseAgreement.pdf` | `C:\Program Files\WhiteBeard\PawnPlugin\` |
| `PawnPlugin_UserGuide.pdf` | `C:\Program Files\WhiteBeard\PawnPlugin\` |

## Troubleshooting

### Build Errors

**"WiX not found"**
- Install WiX Toolset v4 from official website
- Restart command prompt after installation

**"CustomActions.dll not found"**
- Build CustomActions project first: `cd CustomActions && dotnet build -c Release`

**".NET Framework 4.8 not found"**
- Install .NET Framework 4.8 Developer Pack

### Installation Errors

Check the installation log for details:
```cmd
msiexec /i WhiteBeardPawnPlugin.msi /l*v install.log
notepad install.log
```

Common issues:
- **License not found**: Place license file in `C:\ProgramData\WhiteBeard\`
- **MT5 not detected**: Manually browse to MT5 installation directory
- **Access denied**: Run installer as Administrator

## Support

- **Website**: https://whitebeard.ai
- **Email**: support@whitebeard.ai
- **Documentation**: See `PawnPlugin_UserGuide.pdf`

## Version History

### Version 1.0.0
- Initial release
- License verification and validation
- Automatic MT5 detection
- Custom installation dialogs
- Registry integration

## License

Copyright © 2025 WhiteBeard.ai. All rights reserved.

---

**Note**: This installer is designed to be built on Windows. The source code can be developed on macOS, but compilation requires a Windows environment with WiX Toolset v4 installed.
