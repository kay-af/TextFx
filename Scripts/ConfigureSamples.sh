#!/bin/bash
set -euo pipefail

# Configure Package vars.
source Scripts/Package.sh

# Copy samples.
mkdir -p "Packages/$PACKAGE_ID"
rm -rf "Packages/$PACKAGE_ID/Samples~"
cp -R Assets/Samples "Packages/$PACKAGE_ID/Samples~"
cp Assets/Samples.meta "Packages/$PACKAGE_ID/Samples~.meta"

echo "✅ Samples configured for $PACKAGE_ID@$PACKAGE_VERSION."