#!/bin/bash
set -euo pipefail

# Configure Package vars.
source Scripts/Package.sh

# Tag the release commit and push the tag.
git tag v$PACKAGE_VERSION
git push origin v$PACKAGE_VERSION

echo "✅ Tag v$PACKAGE_VERSION created for $PACKAGE_ID@$PACKAGE_VERSION."