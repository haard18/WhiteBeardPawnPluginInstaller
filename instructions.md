# Project: WhiteBeard Pawn Plugin Installer (WiX Setup Project)

I’m building an enterprise-grade Windows Installer (.msi) for the WhiteBeard Pawn Plugin used with MetaTrader 5.

Create a full folder structure and initial source files for a WiX v4-based installer project that will later be built on Windows.

## Goals
- Output: WhiteBeardPawnPlugin.msi
- Purpose: Install the WhiteBeard Pawn Plugin for MetaTrader 5 with license verification and admin privilege checks.

## Requirements
1. **Product Info**
   - Product Name: WhiteBeard Pawn Plugin
   - Manufacturer: WhiteBeard.ai
   - Version: 1.0.0
   - UpgradeCode: `{YOUR-GUID-HERE}`
   - InstallScope: perMachine (requires admin)
   - Output file: `WhiteBeardPawnPlugin.msi`

2. **Installation Flow**
   - Step 1: Detect license file in `C:\ProgramData\WhiteBeard\*_pawn_plugin.lic`.
   - Step 2: If not found, prompt the user to locate their license file manually.
   - Step 3: Decrypt and verify authenticity of license using internal public key.
     - Extract and display Company Name and Email from decrypted data.
   - Step 4: Require user to agree to the License Agreement (PDF included).
   - Step 5: Detect if MetaTrader 5 is installed in the default path:
       `C:\Program Files\MetaTrader 5 Platform\TradeMain`
     - If not found, prompt user to browse to MT5 directory.
   - Step 6: Copy:
       - `PawnPlugin64.dll` → `<MT5 Root>\Plugins\`
       - License file → `C:\ProgramData\WhiteBeard\`
   - Step 7: Create registry key under:
       `HKLM\Software\WhiteBeard\PawnPlugin`
   - Step 8: Finish screen shows success and support contact info.

3. **Files to include in the installer**
   - `PawnPlugin64.dll` (plugin binary)
   - `LicenseAgreement.pdf` (for Terms & Conditions)
   - `PawnPlugin_UserGuide.pdf` (optional help)
   - `example_pawn_plugin.lic` (optional placeholder license)
   - `CustomActions.dll` (will contain license verification, MT5 detection, and file copy logic)

4. **Custom Actions**
   - `VerifyLicense.cs`
     - Looks for existing license file
     - Verifies signature/decryption
     - Returns CompanyName and CompanyEmail
   - `DetectMT5.cs`
     - Checks default MT5 directory, if not found prompts
   - `CopyFiles.cs`
     - Copies DLL + license file to correct folders
   - Each action should log to the Windows Installer session.

5. **UI**
   - Custom dialogs for:
     - License verification (path input + validation)
     - EULA agreement
     - MT5 path selection
   - Include simple logo or banner for WhiteBeard.ai

6. **Folder structure**
WhiteBeardPawnPluginInstaller/
├── Files/
│ ├── PawnPlugin64.dll
│ ├── LicenseAgreement.pdf
│ ├── PawnPlugin_UserGuide.pdf
│ ├── example_pawn_plugin.lic
├── CustomActions/
│ ├── VerifyLicense.cs
│ ├── DetectMT5.cs
│ ├── CopyFiles.cs
├── UI/
│ ├── VerifyLicenseDialog.wxs
│ ├── TermsDialog.wxs
│ ├── MT5PathDialog.wxs
├── Product.wxs
├── PluginFiles.wxs
├── build.bat
├── README.md
└── WhiteBeardPawnPlugin.sln

7. **Build Script**
- Include `build.bat` that runs:
  ```
  heat dir "Files" -cg PluginFiles -dr INSTALLFOLDER -srd -var var.SourceDir -out PluginFiles.wxs
  candle Product.wxs PluginFiles.wxs -ext WixUtilExtension
  light Product.wixobj PluginFiles.wixobj -ext WixUtilExtension -out WhiteBeardPawnPlugin.msi
  ```

8. **Notes**
- The Mac side should only generate source code and structure — no WiX build required here.
- Once copied to Windows VPS, install WiX Toolset v4 and run `build.bat` to generate the `.msi`.
- Prepare a `PublicKey.xml` or `.pem` file in the project root for license verification.
- Include placeholders in XML for Company, Version, Website, Email, etc.

Generate this entire folder structure with initial templates and comments explaining each step.

