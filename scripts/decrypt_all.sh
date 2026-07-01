#!/bin/bash

echo "=== STARTING SECRET DECRYPTION ==="

# 1. Restore .env files from .enc versions
find . -type f -name ".env*.enc" -not -path "*/node_modules/*" | while read -r file; do
    original="${file%.enc}"
    echo "Decrypting .env: $file -> $original"
    sops -d "$file" > "$original"
done

# 2. Restore files in the secrets/ directory
find . -type f -path "./secrets/*.enc" | while read -r file; do
    original="${file%.enc}"
    echo "Decrypting secrets: $file -> $original"
    sops -d "$file" > "$original"
done

# 3. Restore files in the certs/ directory
find . -type f -path "./certs/*.enc" | while read -r file; do
    original="${file%.enc}"
    echo "Decrypting certs: $file -> $original"
    sops -d "$file" > "$original"
done

echo "=== ALL SECRETS HAVE BEEN RESTORED AND ARE READY TO USE! ==="
