# 🎯 QUICK START: License Management

## Step-by-Step Guide

### 1️⃣ First Time Setup (ONE TIME)

```bash
cd LicenseGenerator
dotnet run
```

Select: **Option 1: Generate RSA Key Pair**

✅ Creates:
- `PrivateKey.xml` (in LicenseGenerator/) - **KEEP SECRET!**
- `PublicKey.xml` (in project root) - Include in installer

⚠️ **IMPORTANT:** Backup `PrivateKey.xml` to a secure location NOW!

---

### 2️⃣ Generate Customer License (FOR EACH CUSTOMER)

```bash
cd LicenseGenerator
dotnet run
```

Select: **Option 2: Create License File**

Enter customer details:
- Company Name: `Acme Trading Corp`
- Email: `admin@acmetrading.com`
- Expiry: `2026-12-31` (or press Enter for unlimited)
- Max Installs: `5` (or press Enter for unlimited)

✅ Creates: `Files/Acme_Trading_Corp_pawn_plugin.lic`

📧 **Email this .lic file to your customer**

---

### 3️⃣ Build Installer (ON WINDOWS)

```cmd
build.bat
```

✅ Creates: `WhiteBeardPawnPlugin.msi`

📤 **Distribute this installer to customers**

---

## 🔑 Key Concepts

| File | Purpose | Who Has It? | Secret? |
|------|---------|-------------|---------|
| **PrivateKey.xml** | Sign licenses | YOU ONLY | ✅ YES - Never share! |
| **PublicKey.xml** | Verify licenses | In installer | ❌ No - Public |
| **customer.lic** | License data + signature | Customer | ❌ No - Signed data |

**The Magic:** Even though customers have the public key (in the installer), they **cannot** create fake licenses because they don't have your private key!

---

## 📋 Customer Instructions

Send this to customers:

**Installation Steps:**
1. Create folder: `C:\ProgramData\WhiteBeard\`
2. Place your `.lic` file in that folder
3. Run `WhiteBeardPawnPlugin.msi`
4. Installer will automatically verify your license
5. Follow the installation wizard

---

## 🔒 Security Checklist

- [ ] Private key backed up to secure location
- [ ] Private key has restricted permissions (chmod 600)
- [ ] Private key NOT committed to git (.gitignore is set)
- [ ] Public key is in project root for installer
- [ ] Test license verification before distributing

---

## 🧪 Testing

**Verify a license:**
```bash
cd LicenseGenerator
dotnet run
# Select option 3
# Enter path to .lic file
```

**Test installer (Windows):**
```cmd
msiexec /i WhiteBeardPawnPlugin.msi /l*v install.log
notepad install.log
# Search for "License verified successfully"
```

---

## 📞 Common Issues

**"PrivateKey.xml not found"**
→ Run option 1 first to generate keys

**"License verification failed"**
→ Check customer placed .lic file in correct location
→ Verify file wasn't corrupted
→ Regenerate license if needed

**"License expired"**
→ Generate new license with updated expiry date

---

## 🚀 Quick Commands

```bash
# Easy way - use the helper script
./generate_license.sh

# Or manually
cd LicenseGenerator && dotnet run
```

---

## 📚 More Details

- Full workflow: `LICENSE_MANAGEMENT.md`
- Technical details: `LicenseGenerator/README.md`
- Build instructions: `BUILD.md`
- Project overview: `README.md`

---

## ⚡ TL;DR

1. **Once:** Generate keys (`dotnet run` → option 1)
2. **Per customer:** Create license (`dotnet run` → option 2)
3. **Once:** Build installer on Windows (`build.bat`)
4. **Distribute:** Send `.lic` to customer, `.msi` on website
5. **Customer:** Place `.lic` in `C:\ProgramData\WhiteBeard\`, run installer

✅ Done!
