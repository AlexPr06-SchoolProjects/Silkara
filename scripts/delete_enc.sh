#!/bin/bash

echo "=== DELETING ALL .enc FILES ==="

# Delete .env*.enc files in the root
find . -maxdepth 1 -type f -name ".env*.enc" -delete
echo "Deleted .env*.enc files."

# Delete .enc files in secrets/
if [ -d "secrets" ]; then
    find secrets -type f -name "*.enc" -delete
    echo "Deleted secrets/*.enc files."
fi

# Delete .enc files in certs/
if [ -d "certs" ]; then
    find certs -type f -name "*.enc" -delete
    echo "Deleted certs/*.enc files."
fi

echo "=== ALL .enc FILES DELETED! ==="
