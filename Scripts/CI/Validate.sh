#!/bin/bash
set -euo pipefail

# Configure Package vars.
source Scripts/Package.sh

# Strip the leading "v" from the tag (e.g. v1.2.3 → 1.2.3).
TAG_VERSION="${GITHUB_REF_NAME#v}"

if [[ -z "$TAG_VERSION" ]]; then
  echo "❌ GITHUB_REF_NAME is not set or is not a tag." >&2
  exit 1
fi

if [[ "$TAG_VERSION" != "$PACKAGE_VERSION" ]]; then
  echo "❌ Tag version ($TAG_VERSION) does not match package.json version ($PACKAGE_VERSION)." >&2
  exit 1
fi

echo "✅ Tag version matches package.json version ($PACKAGE_VERSION)."