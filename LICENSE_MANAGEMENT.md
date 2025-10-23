# License Management Guide

## 🔑 Understanding the Key System

The WhiteBeard Pawn Plugin uses **RSA digital signatures** for license verification:

- **Private Key** (PrivateKey.xml): You keep this SECRET. Used to SIGN licenses.
- **Public Key** (PublicKey.xml): Embedded in installer. Used to VERIFY licenses.
- **License Files** (.lic): Sent to customers. Contains signed data.

### Why This Approach?

✅ **Secure**: Even if customers have the public key (they do, it's in the installer), they can't create fake licenses  
✅ **Offline**: License verification works without internet connection  
✅ **Tamper-proof**: Any modification to license data invalidates the signature  
✅ **Flexible**: Include expiry dates, installation limits, etc.

## 🚀 Complete Workflow

### Phase 1: Initial Setup (One Time Only)

**On your Mac:**

1. **Generate RSA Key Pair**
   ```bash
   cd LicenseGenerator
   dotnet run
   # Select option 1: Generate RSA Key Pair
   ```

   This creates:
   - `LicenseGenerator/PrivateKey.xml` ← **KEEP SECRET!**
   - `PublicKey.xml` ← Include in installer ✓

2. **Backup Private Key Securely**
   ```bash
   # Copy to secure location (encrypted USB drive, password manager, etc.)
   cp LicenseGenerator/PrivateKey.xml ~/Secure/WhiteBeard_PrivateKey_BACKUP.xml
   
   # Set restrictive permissions
   chmod 600 LicenseGenerator/PrivateKey.xml
   ```

3. **Verify Setup**
   ```bash
   # Check that public key exists
   ls -la PublicKey.xml
   
   # Check that private key is protected
   ls -la LicenseGenerator/PrivateKey.xml
   # Should show: -rw------- (only owner can read/write)
   ```

### Phase 2: For Each Customer

**On your Mac:**

1. **Generate Customer License**
   ```bash
   cd LicenseGenerator
   dotnet run
   # Select option 2: Create License File
   ```

   Enter customer details:
   ```
   Company Name: Acme Trading Corp
   Company Email: admin@acmetrading.com
   License Valid Until: 2026-12-31  (or press Enter for unlimited)
   Max Installations: 5              (or press Enter for unlimited)
   ```

   Output: `Files/Acme_Trading_Corp_pawn_plugin.lic`

2. **Verify the License** (Optional but recommended)
   ```bash
   dotnet run
   # Select option 3: Verify License File
   # Enter path: ../Files/Acme_Trading_Corp_pawn_plugin.lic
   ```

   Should show: ✓ License signature is VALID

3. **Send to Customer**
   - Email the `.lic` file to customer
   - Include instructions (see "Customer Instructions" below)

### Phase 3: Build Installer (On Windows)

**On Windows VPS:**

1. **Ensure PublicKey.xml is present**
   ```cmd
   dir PublicKey.xml
   ```

2. **Build Installer**
   ```cmd
   build.bat
   ```

3. **Distribute Installer**
   - Upload `WhiteBeardPawnPlugin.msi` to your website
   - Share download link with customers

### Phase 4: Customer Installation

**Customer receives:**
- ✉️ License file: `Acme_Trading_Corp_pawn_plugin.lic`
- 📥 Installer: `WhiteBeardPawnPlugin.msi`

**Customer steps:**
1. Place license file in: `C:\ProgramData\WhiteBeard\`
2. Run installer
3. Installer automatically finds and verifies license
4. Displays company name and email for confirmation
5. Installs plugin to MetaTrader 5

## 📋 Customer Instructions Template

Copy this and send to customers:

---

**WhiteBeard Pawn Plugin Installation Instructions**

Thank you for purchasing the WhiteBeard Pawn Plugin!

**Before Installing:**

1. Locate your license file: `[CompanyName]_pawn_plugin.lic`
2. Create folder: `C:\ProgramData\WhiteBeard\` (if it doesn't exist)
3. Copy your `.lic` file into that folder

**Installation:**

1. Run `WhiteBeardPawnPlugin.msi`
2. The installer will verify your license
3. Confirm your company details are displayed correctly
4. Accept the license agreement
5. Select your MetaTrader 5 installation path
6. Complete installation

**Troubleshooting:**

- If license not found: Use "Browse" button to locate your `.lic` file
- If MT5 not detected: Use "Browse" button to select your MT5 directory
- For support: support@whitebeard.ai

---

## 🔒 Security Best Practices

### DO ✅

- Keep `PrivateKey.xml` in a secure, encrypted location
- Backup `PrivateKey.xml` to multiple secure locations
- Set file permissions to 600 (owner read/write only)
- Use `.gitignore` to prevent committing private key
- Generate unique licenses per customer
- Set expiry dates for trial/subscription licenses
- Keep a record of issued licenses (spreadsheet/database)

### DON'T ❌

- Commit `PrivateKey.xml` to version control
- Email `PrivateKey.xml` to anyone
- Store `PrivateKey.xml` on cloud services without encryption
- Share `PrivateKey.xml` with team members (use secure sharing if needed)
- Reuse the same key for different products
- Store private key on production servers

### What If Private Key Is Compromised?

1. **Generate new key pair**
2. **Rebuild installer with new public key**
3. **Regenerate all customer licenses with new private key**
4. **Distribute updated installer and licenses**

## 📊 License Tracking

Consider maintaining a spreadsheet:

| Customer | Email | License ID | Issue Date | Expiry | Max Installs | Status |
|----------|-------|------------|------------|--------|--------------|--------|
| Acme Trading | admin@acme.com | abc-123... | 2025-10-23 | 2026-12-31 | 5 | Active |

Or use a database for automated license management.

## 🧪 Testing

### Test License Verification

```bash
cd LicenseGenerator
dotnet run
# Option 3: Verify License File
```

### Test Installer (Windows)

```cmd
# Install with logging
msiexec /i WhiteBeardPawnPlugin.msi /l*v install.log

# Check log for license verification
notepad install.log
# Search for: "License verified successfully"
```

## 🔄 License Updates

### Extend License Expiry

1. Generate new license with updated expiry date
2. Send new `.lic` file to customer
3. Customer replaces old license file
4. No reinstallation needed (unless you want to validate on startup)

### Add More Installations

1. Generate new license with higher max installations
2. Send to customer
3. Customer replaces license file

## 🛠️ Advanced: Automated License Generation

For high-volume license generation, create a script:

```bash
#!/bin/bash
# generate_licenses.sh

while IFS=',' read -r company email expiry installs; do
  echo "Generating license for: $company"
  # TODO: Automate license generation from CSV
done < customers.csv
```

## 📞 Support Scenarios

**Customer: "License verification failed"**
- Check they placed `.lic` file in correct location
- Verify file wasn't corrupted during email
- Regenerate and resend license if needed

**Customer: "License expired"**
- Check expiry date: `dotnet run` → option 3
- Generate new license with extended date
- Send updated `.lic` file

**Customer: "Lost license file"**
- Regenerate license with same details
- Send new `.lic` file
- (Or keep backups of all generated licenses)

## 📝 Notes

- License files are human-readable (includes plaintext company info for reference)
- Signature prevents tampering with any data
- Public key in installer can't be used to create licenses
- Each license is uniquely identifiable via License ID (GUID)
- Expiry checking happens during installation (add runtime checks if needed)

## 🎯 Quick Reference Commands

```bash
# Generate keys (first time)
cd LicenseGenerator && dotnet run
# → Option 1

# Create license
cd LicenseGenerator && dotnet run
# → Option 2

# Verify license
cd LicenseGenerator && dotnet run
# → Option 3

# Build installer (Windows)
build.bat

# Test install (Windows)
msiexec /i WhiteBeardPawnPlugin.msi /l*v install.log
```

## 🚨 Emergency: Lost Private Key

If you lose your private key:
1. **You cannot generate licenses for existing installers**
2. **Action**: Generate new key pair → Rebuild installer → Regenerate all licenses
3. **Prevention**: Backup private key to multiple secure locations NOW

---

**Questions?** See `LicenseGenerator/README.md` for more details.
