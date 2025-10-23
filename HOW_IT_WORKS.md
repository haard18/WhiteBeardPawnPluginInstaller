# License Management System - Complete Answer

## ✅ Direct Answer to Your Question

**Yes, you generate licenses on your Mac and send:**

1. **License file (.lic)** → Send to customer ✉️
2. **Public key (PublicKey.xml)** → Include in installer (embedded) 📦
3. **Private key (PrivateKey.xml)** → NEVER send to anyone! Keep secret! 🔒

## 🔐 How the Security Works

### The RSA Signature System

```
┌─────────────────────────────────────────────────────────────┐
│                    YOUR MAC (License Generation)             │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  1. Generate Key Pair (ONE TIME)                            │
│     ├── PrivateKey.xml  ───► Keep SECRET! Sign licenses     │
│     └── PublicKey.xml   ───► Include in installer           │
│                                                               │
│  2. Create License (PER CUSTOMER)                           │
│     ├── Customer data (name, email, expiry)                 │
│     ├── Sign with PrivateKey ───► Creates signature         │
│     └── Save as customer_pawn_plugin.lic                    │
│                                                               │
└─────────────────────────────────────────────────────────────┘
                           │
                           │ Send .lic file via email
                           ↓
┌─────────────────────────────────────────────────────────────┐
│                    CUSTOMER'S WINDOWS                        │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  1. Receives: customer_pawn_plugin.lic                      │
│  2. Places in: C:\ProgramData\WhiteBeard\                   │
│  3. Runs: WhiteBeardPawnPlugin.msi                          │
│                                                               │
└─────────────────────────────────────────────────────────────┘
                           │
                           ↓
┌─────────────────────────────────────────────────────────────┐
│                    INSTALLER (During Installation)           │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  1. Read customer_pawn_plugin.lic                           │
│     ├── Extract: Customer data (base64 JSON)                │
│     └── Extract: Signature (base64 RSA signature)           │
│                                                               │
│  2. Verify with PublicKey.xml (embedded in installer)       │
│     ├── Check: Signature matches data?                      │
│     ├── ✓ Valid   → Extract company name, email, etc.      │
│     └── ✗ Invalid → Installation fails                      │
│                                                               │
│  3. Check Expiry Date (if set)                             │
│     ├── Not expired → Continue                              │
│     └── Expired     → Installation fails                    │
│                                                               │
│  4. Display company info & proceed with installation        │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

### Why This is Secure

**Q: Can't customers create fake licenses since they have PublicKey.xml?**  
**A: NO!** The public key can only VERIFY signatures, not CREATE them. Only your private key can create valid signatures.

**Q: Can't they modify the license file?**  
**A: They can try, but it will FAIL verification.** Any change to the data invalidates the signature.

**Q: What if they share their license?**  
**A: You can add installation limits or unique machine IDs (future enhancement).**

## 📋 Complete Step-by-Step Workflow

### Phase 1: Initial Setup (One Time)

**What:** Generate RSA key pair  
**Where:** Your Mac  
**Command:**
```bash
cd LicenseGenerator
dotnet run
# Select: 1. Generate RSA Key Pair
```

**Result:**
- ✅ `LicenseGenerator/PrivateKey.xml` created (KEEP SECRET!)
- ✅ `PublicKey.xml` created (will be embedded in installer)

**Action Required:**
```bash
# Backup private key securely
cp LicenseGenerator/PrivateKey.xml ~/Secure_Backup/

# Set restrictive permissions
chmod 600 LicenseGenerator/PrivateKey.xml

# Verify it's in .gitignore (it is!)
cat .gitignore | grep PrivateKey
```

---

### Phase 2: For Each Customer

**What:** Generate signed license file  
**Where:** Your Mac  
**Command:**
```bash
cd LicenseGenerator
dotnet run
# Select: 2. Create License File
```

**Input:**
```
Company Name: Acme Trading Corp
Company Email: admin@acmetrading.com
License Valid Until: 2026-12-31  (or Enter for unlimited)
Max Installations: 5              (or Enter for unlimited)
```

**Result:**
- ✅ `Files/Acme_Trading_Corp_pawn_plugin.lic` created

**What's Inside the .lic File:**
```
-----BEGIN WHITEBEARD LICENSE-----
eyJDb21wYW55TmFtZSI6IkFjbWUgVHJhZGluZyBDb3JwIiwi... (base64 JSON)
-----BEGIN SIGNATURE-----
kMH8F3n2... (base64 RSA signature)
-----END WHITEBEARD LICENSE-----

# License Information (for reference)
Company: Acme Trading Corp
Email: admin@acmetrading.com
License ID: 12345678-abcd-...
Issue Date: 2025-10-23
Expires: 2026-12-31
Max Installations: 5
```

**Action Required:**
```
📧 Email this .lic file to customer
📝 Include installation instructions (see below)
```

---

### Phase 3: Build Installer

**What:** Compile the MSI installer  
**Where:** Windows VPS  
**Prerequisites:**
- WiX Toolset v4 installed
- .NET SDK 4.8+
- `PublicKey.xml` present in project root

**Command:**
```cmd
build.bat
```

**Result:**
- ✅ `WhiteBeardPawnPlugin.msi` created

**What Gets Embedded in Installer:**
- ✅ `PublicKey.xml` (copied during CustomActions build)
- ✅ PawnPlugin64.dll
- ✅ LicenseAgreement.pdf
- ✅ PawnPlugin_UserGuide.pdf
- ✅ Custom action DLLs (license verification logic)

**Action Required:**
```
📤 Upload WhiteBeardPawnPlugin.msi to your website
🔗 Share download link with customers
```

---

### Phase 4: Customer Installation

**What Customer Receives:**
1. Email with `Acme_Trading_Corp_pawn_plugin.lic`
2. Download link for `WhiteBeardPawnPlugin.msi`

**What Customer Does:**
1. Create folder: `C:\ProgramData\WhiteBeard\`
2. Place `.lic` file in that folder
3. Download and run `WhiteBeardPawnPlugin.msi`
4. Follow installation wizard

**What Installer Does:**
1. Search `C:\ProgramData\WhiteBeard\` for `*_pawn_plugin.lic`
2. If found, verify signature using embedded `PublicKey.xml`
3. If valid, extract and display company name & email
4. Check expiry date (if set)
5. Proceed with installation

---

## 📧 Customer Email Template

```
Subject: Your WhiteBeard Pawn Plugin License

Hello [Customer Name],

Thank you for purchasing the WhiteBeard Pawn Plugin!

Attached to this email is your license file:
📎 [CompanyName]_pawn_plugin.lic

INSTALLATION STEPS:

1. Download the installer:
   https://whitebeard.ai/downloads/WhiteBeardPawnPlugin.msi

2. Create this folder if it doesn't exist:
   C:\ProgramData\WhiteBeard\

3. Place your license file (.lic) in that folder

4. Run the installer (WhiteBeardPawnPlugin.msi)

5. The installer will automatically verify your license

6. Follow the wizard to complete installation

Your license details:
- Company: [Company Name]
- License ID: [License ID]
- Valid Until: [Expiry Date or "Unlimited"]
- Max Installations: [Number or "Unlimited"]

TROUBLESHOOTING:

- If license not found: Use "Browse" button in installer
- If MT5 not detected: Browse to your MT5 installation folder
- For support: support@whitebeard.ai

Best regards,
WhiteBeard.ai Team
```

---

## 🔒 Security Checklist

Before distributing:

- [ ] Private key backed up to secure location
- [ ] Private key permissions set to 600 (Mac/Linux)
- [ ] Private key NOT in version control (check .gitignore)
- [ ] Public key exists in project root
- [ ] Test license verification (option 3 in generator)
- [ ] Test full installation on clean Windows VM
- [ ] Installer built with correct public key

---

## 🧪 Testing the System

### Test 1: Verify License Generation

```bash
cd LicenseGenerator
dotnet run
# Select: 3. Verify License File
# Enter: ../Files/[customer]_pawn_plugin.lic
```

Expected output:
```
✓ License signature is VALID

License Details:
  Company: Acme Trading Corp
  Email: admin@acmetrading.com
  License ID: 12345678-...
  Issue Date: 2025-10-23
  Expires: 2026-12-31
  Max Installations: 5
```

### Test 2: Test Tamper Detection

```bash
# Manually edit a .lic file (change company name in plaintext)
nano Files/test_pawn_plugin.lic

# Try to verify
cd LicenseGenerator && dotnet run
# Select: 3
```

Expected output:
```
✗ License signature is INVALID!
  This license may have been tampered with.
```

### Test 3: Test Installer

```cmd
REM On Windows
msiexec /i WhiteBeardPawnPlugin.msi /l*v install.log

REM Check log
findstr "License verified successfully" install.log
```

---

## 🎯 Summary

| Item | Purpose | Location | Share? |
|------|---------|----------|--------|
| **PrivateKey.xml** | Sign licenses | Your Mac (LicenseGenerator/) | ❌ NEVER |
| **PublicKey.xml** | Verify licenses | Project root → Embedded in installer | ✅ Yes (in installer) |
| **customer.lic** | Customer's signed license | Email to customer | ✅ Yes (to that customer) |
| **WhiteBeardPawnPlugin.msi** | Installer | Website/downloads | ✅ Yes (public) |

---

## 🚀 You're Ready!

1. **Now:** Generate your keys (if you haven't)
   ```bash
   cd LicenseGenerator && dotnet run
   # Option 1
   ```

2. **For each sale:** Generate license
   ```bash
   dotnet run
   # Option 2
   ```

3. **Once:** Build installer on Windows
   ```cmd
   build.bat
   ```

4. **Distribute:** Email `.lic` to customer, host `.msi` on website

---

**Questions?** Check these docs:
- Quick reference: `QUICKSTART.md`
- Detailed workflow: `LICENSE_MANAGEMENT.md`
- Technical details: `LicenseGenerator/README.md`
- Build instructions: `BUILD.md`
