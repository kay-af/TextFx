#!/bin/bash
set -euo pipefail

# Configure Package vars.
source Scripts/Package.sh

# Copy the CHANGELOG.md and LICENSE.md to the Documentation folder.
cp CHANGELOG.md Documentation
cp LICENSE.md Documentation

# Update toc.yml to include the CHANGELOG.md and LICENSE.md files (idempotent).
if ! grep -q "href: CHANGELOG.md" Documentation/toc.yml; then
  echo "- name: Changelog" >> Documentation/toc.yml
  echo "  href: CHANGELOG.md" >> Documentation/toc.yml
fi

if ! grep -q "href: LICENSE.md" Documentation/toc.yml; then
  echo "- name: License" >> Documentation/toc.yml
  echo "  href: LICENSE.md" >> Documentation/toc.yml
fi

# Build the documentation metadata.
docfx metadata Documentation/docfx.json

# Build the documentation.
docfx build Documentation/docfx.json

echo "✅ Documentation configured for $PACKAGE_ID@$PACKAGE_VERSION."