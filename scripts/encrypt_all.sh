#!/bin/bash

echo "=== STARTING SECRET ENCRYPTION ==="

# 1. Encrypt .env files in root and subdirectories
# Skips already encrypted files, hidden files except .env*, and node_modules
find . -type f -name ".env*" ! -name "*.enc" -not -path "*/node_modules/*" | while read -r file; do
    echo "Encrypting .env: $file -> $file.enc"
    sops -e "$file" > "$file.enc"
done

# 2. Encrypt files in the secrets/ directory
if [ -d "secrets" ]; then
    find secrets -type f ! -name "*.enc" | while read -r file; do
        echo "Encrypting secrets: $file -> $file.enc"
        sops -e "$file" > "$file.enc"
    done
fi

# 3. Encrypt files in the certs/ directory
if [ -d "certs" ]; then
    find certs -type f ! -name "*.enc" | while read -r file; do
        echo "Encrypting certs: $file -> $file.enc"
        sops -e "$file" > "$file.enc"
    done
fi

echo "=== ALL SECRETS ARE SUCCESSFULLY ENCRYPTED! ==="
