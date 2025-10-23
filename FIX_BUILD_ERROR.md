# 🔧 Build Error Fix: Microsoft.Deployment Namespace Not Found

## Error You're Seeing

```
error CS0234: The type or namespace name 'Deployment' does not exist in the namespace 'Microsoft'
error CS0246: The type or namespace name 'CustomActionAttribute' could not be found
```

## Root Cause

The WiX Toolset DTF (Deployment Tools Foundation) NuGet package version needs to match your installed WiX version.

## ✅ SOLUTION

I've already updated the `CustomActions.csproj` file to use the correct package version.

### What Changed:

**Before:**
```xml
<PackageReference Include="WixToolset.Dtf.WindowsInstaller" Version="4.0.0" />
```

**After:**
```xml
<PackageReference Include="WixToolset.Dtf.WindowsInstaller" Version="5.0.0" />
```

## 📋 Steps to Fix on Windows

### Method 1: Pull Updated Files (Recommended)

If using Git:
```cmd
cd C:\Users\Haard\Desktop\WhiteBeardPawnPluginInstaller
git pull
```

Then rebuild:
```cmd
build.bat
```

### Method 2: Manual Update

1. Open `CustomActions\CustomActions.csproj` in Notepad
2. Find the line:
   ```xml
   <PackageReference Include="WixToolset.Dtf.WindowsInstaller" Version="4.0.0" />
   ```
3. Change to:
   ```xml
   <PackageReference Include="WixToolset.Dtf.WindowsInstaller" Version="5.0.0" />
   ```
4. Save and run `build.bat` again

### Method 3: Let NuGet Find Latest Version

```cmd
cd CustomActions
dotnet remove package WixToolset.Dtf.WindowsInstaller
dotnet add package WixToolset.Dtf.WindowsInstaller
cd ..
build.bat
```

## 🔍 Alternative: Check Your WiX Version

If the above doesn't work, check which WiX version you have:

```cmd
wix --version
```

Then use the corresponding DTF package version:
- WiX 4.x → DTF 4.0.x
- WiX 5.x → DTF 5.0.x

## 🧪 Verify the Fix

After updating, run:

```cmd
cd CustomActions
dotnet restore
dotnet build -c Release
```

You should see:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Then run the full build:
```cmd
cd ..
build.bat
```

## 💡 Why This Happened

The original code was created with WiX v4 package references, but you might have WiX v5 installed, or the package naming/versioning has changed since the project was created.

## 🆘 Still Not Working?

### Try These Steps:

1. **Clear NuGet cache:**
   ```cmd
   dotnet nuget locals all --clear
   ```

2. **Delete bin and obj folders:**
   ```cmd
   cd CustomActions
   rmdir /s /q bin
   rmdir /s /q obj
   cd ..
   ```

3. **Restore and rebuild:**
   ```cmd
   build.bat
   ```

### Check Package Exists:

Search for available versions:
```cmd
dotnet add CustomActions package WixToolset.Dtf.WindowsInstaller --version
```

Or visit: https://www.nuget.org/packages/WixToolset.Dtf.WindowsInstaller/

## 📋 Complete Working Configuration

Your `CustomActions.csproj` should look like:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net48</TargetFramework>
    <RootNamespace>WhiteBeardPawnPlugin.CustomActions</RootNamespace>
    <AssemblyName>CustomActions</AssemblyName>
    <OutputType>Library</OutputType>
    <PlatformTarget>x64</PlatformTarget>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="WixToolset.Dtf.WindowsInstaller" Version="5.0.0" />
  </ItemGroup>

  <ItemGroup>
    <None Include="..\PublicKey.xml">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

</Project>
```

## ✅ Expected Output After Fix

```cmd
[1/6] Restoring NuGet packages...
  Determining projects to restore...
  Restored CustomActions.csproj (in XXX ms).

[2/6] Building Custom Actions...
  CustomActions -> CustomActions\bin\Release\net48\CustomActions.dll
  Build succeeded.

[3/6] Generating file component definitions...
[4/6] Compiling WiX source files...
[5/6] Validating MSI package...
[6/6] Build complete!
```

---

**The fix has been applied to the repository. Pull the latest changes and run build.bat again!**
