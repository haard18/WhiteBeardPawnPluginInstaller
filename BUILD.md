# Build Instructions

## Prerequisites

1. **Windows Environment** (Windows 10/11 or Windows Server)
2. **WiX Toolset v4** - https://wixtoolset.org/
3. **.NET SDK 4.8+** - https://dotnet.microsoft.com/download

## Quick Start

1. Replace placeholder files in `Files/` directory with actual files
2. Add your RSA public key to `PublicKey.xml`
3. Add your icon to `icon.ico`
4. Run `build.bat`

## Detailed Steps

### 1. Prepare Files

Replace these placeholder files:
- `Files/PawnPlugin64.dll` - Your compiled plugin DLL
- `Files/LicenseAgreement.pdf` - Your End User License Agreement
- `Files/PawnPlugin_UserGuide.pdf` - User documentation
- `icon.ico` - Your installer icon (256x256 recommended)
- `PublicKey.xml` - Your RSA public key for license verification

### 2. Configure Product Settings

Edit `Product.wxs`:
- Line 12: Change `UpgradeCode` to a new GUID (generate once, keep forever)
- Line 10: Update `Version` number
- Line 17-20: Update URLs and contact info

### 3. Build Custom Actions

```cmd
cd CustomActions
dotnet restore
dotnet build -c Release
cd ..
```

### 4. Compile Installer

Option A - Using build script:
```cmd
build.bat
```

Option B - Manual compilation:
```cmd
wix build Product.wxs UI\VerifyLicenseDialog.wxs UI\TermsDialog.wxs UI\MT5PathDialog.wxs ^
    -ext WixToolset.Util.wixext ^
    -ext WixToolset.UI.wixext ^
    -out WhiteBeardPawnPlugin.msi
```

### 5. Test Installation

```cmd
msiexec /i WhiteBeardPawnPlugin.msi /l*v install.log
```

View log file to check for any issues:
```cmd
notepad install.log
```

## Common Issues

**Issue**: "WiX not found"
**Solution**: Install WiX Toolset v4 and restart your command prompt

**Issue**: "CustomActions.dll not found"
**Solution**: Build the CustomActions project first

**Issue**: ".NET Framework not found"
**Solution**: Install .NET Framework 4.8 Developer Pack

**Issue**: "Files not found during build"
**Solution**: Ensure all placeholder files are replaced with actual files

## Customization

### Add More Custom Actions

1. Create new .cs file in `CustomActions/`
2. Add `[CustomAction]` attribute to your method
3. Reference it in `Product.wxs` with `<CustomAction>` element
4. Add to execution sequence

### Modify UI Flow

1. Edit dialog files in `UI/` directory
2. Update navigation in `Product.wxs` `<Publish>` elements
3. Add new dialogs as needed

### Change Installation Directory

Edit `Product.wxs`:
- Modify `<StandardDirectory>` elements
- Update `INSTALLFOLDER` references

## Generating GUIDs

For `UpgradeCode` and component GUIDs, use:

PowerShell:
```powershell
[guid]::NewGuid().ToString().ToUpper()
```

Or use online GUID generator: https://www.guidgenerator.com/

**Important**: UpgradeCode should remain the same across all versions of your product!

## Version Updates

When releasing a new version:
1. Update version number in `Product.wxs` (line 10)
2. Keep the same `UpgradeCode`
3. Rebuild the installer
4. Test upgrade from previous version

## Distribution

After successful build:
1. Test the MSI on a clean VM
2. Sign the MSI with a code signing certificate (recommended)
3. Distribute via your website or installation portal

Code signing (optional but recommended):
```cmd
signtool sign /f YourCertificate.pfx /p password /t http://timestamp.digicert.com WhiteBeardPawnPlugin.msi
```

## Support

For issues or questions:
- Email: support@whitebeard.ai
- Documentation: README.md
