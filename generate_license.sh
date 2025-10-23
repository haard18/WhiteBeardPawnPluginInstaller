#!/bin/bash
# Quick License Generator Helper Script
# Usage: ./generate_license.sh

cd "$(dirname "$0")/LicenseGenerator"

if [ ! -f "PrivateKey.xml" ]; then
    echo "⚠️  Private key not found!"
    echo "Running first-time setup..."
    echo ""
    dotnet run -- generate-keys
    echo ""
    echo "✓ Setup complete!"
    echo ""
fi

echo "Starting License Generator..."
dotnet run
