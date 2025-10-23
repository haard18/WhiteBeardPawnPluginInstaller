@echo off
REM WhiteBeard Pawn Plugin Installer Build Script
REM Requires WiX Toolset v4 to be installed
REM Run this script on Windows to build the MSI installer

echo ========================================
echo WhiteBeard Pawn Plugin Installer Build
echo ========================================
echo.

REM Check if WiX is installed
where wix >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: WiX Toolset v4 not found!
    echo Please install WiX Toolset v4 from https://wixtoolset.org/
    echo.
    pause
    exit /b 1
)

echo [1/5] Restoring NuGet packages...
cd CustomActions
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to restore NuGet packages
    cd ..
    pause
    exit /b 1
)
echo NuGet packages restored successfully.
echo.

echo [2/5] Building Custom Actions...
dotnet build -c Release
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to build CustomActions
    cd ..
    pause
    exit /b 1
)
cd ..
echo Custom Actions built successfully.
echo.

echo [3/5] Generating file component definitions with heat...
REM Uncomment the following line if you want to auto-generate PluginFiles.wxs
REM wix heat dir "Files" -cg PluginFiles -dr PLUGINDIR -srd -var var.SourceDir -out PluginFiles.wxs
echo Skipping heat generation (using manual component definitions in Product.wxs)
echo.

echo [4/5] Compiling WiX source files...
wix build Product.wxs UI\VerifyLicenseDialog.wxs UI\TermsDialog.wxs UI\MT5PathDialog.wxs ^
    -ext WixToolset.Util.wixext ^
    -ext WixToolset.UI.wixext ^
    -out WhiteBeardPawnPlugin.msi
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: WiX compilation failed
    pause
    exit /b 1
)
echo WiX source files compiled successfully.
echo.

echo [5/5] Validating MSI package...
if exist WhiteBeardPawnPlugin.msi (
    echo MSI package created successfully!
    dir WhiteBeardPawnPlugin.msi
) else (
    echo ERROR: MSI package not created
    pause
    exit /b 1
)
echo.

echo [6/6] Build complete!
echo ========================================
echo Output: WhiteBeardPawnPlugin.msi
echo ========================================
echo.
echo You can now test the installer by running:
echo     msiexec /i WhiteBeardPawnPlugin.msi /l*v install.log
echo.
pause
