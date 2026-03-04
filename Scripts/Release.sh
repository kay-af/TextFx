#!/bin/bash
set -euo pipefail

# Configure Package vars.
source Scripts/Package.sh

# Ensure we are on the main branch.
CURRENT_BRANCH=$(git symbolic-ref --short HEAD)
if [ "$CURRENT_BRANCH" != main ]; then
  echo "❌ Releases can only be made from the main branch (currently on '$CURRENT_BRANCH')."
  exit 1
fi

# Ensure the working tree is clean.
if ! git diff --quiet || ! git diff --cached --quiet; then
  echo "❌ Working tree is not clean. Commit or stash changes before releasing."
  exit 1
fi

# Configure documentation.
bash Scripts/ConfigureDocumentation.sh

# Configure samples.
bash Scripts/ConfigureSamples.sh

git add -A
git commit -m "Pre-release v$PACKAGE_VERSION" || echo "Nothing to commit for pre-release."
git push origin main

# Release Tag.
bash Scripts/ReleaseTag.sh

# Release Git branch.
bash Scripts/ReleaseGit.sh

# Build & Release Documentation.
bash Scripts/ReleaseDocumentation.sh