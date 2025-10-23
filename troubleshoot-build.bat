@echo off
REM Troubleshooting Script for CustomActions Build Issues
echo ========================================
echo CustomActions Build Troubleshooter
echo ========================================
echo.

echo Step 1: Checking .NET installation...
dotnet --version
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET SDK not found!
    echo Please install .NET SDK 6.0 or later
    pause
    exit /b 1
)
echo.

echo Step 2: Checking WiX installation...
wix --version
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: WiX not found!
    echo Please install WiX Toolset v4
    pause
    exit /b 1
)
echo.

echo Step 3: Clearing NuGet cache...
dotnet nuget locals all --clear
echo NuGet cache cleared.
echo.

echo Step 4: Removing old build artifacts...
cd CustomActions
if exist bin rmdir /s /q bin
if exist obj rmdir /s /q obj
echo Old artifacts removed.
echo.

echo Step 5: Restoring NuGet packages...
dotnet restore -v detailed
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Package restore failed!
    echo.
    echo Trying alternative package source...
    dotnet restore --source https://api.nuget.org/v3/index.json -v detailed
)
echo.

echo Step 6: Building CustomActions...
dotnet build -c Release -v detailed
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Build failed!
    echo.
    echo Please check the error messages above.
    cd ..
    pause
    exit /b 1
)
cd ..
echo.

echo ========================================
echo SUCCESS! CustomActions built.
echo ========================================
echo.
echo CustomActions.dll location:
dir CustomActions\bin\Release\net48\CustomActions.dll
echo.
pause
