#!/bin/bash
set -euo pipefail

# Configure Package vars.
source Scripts/Package.sh

GIT_BRANCH=v$PACKAGE_VERSION-git
WORKTREE_DIR=$(mktemp -d)

cleanup() {
  git worktree remove "$WORKTREE_DIR" >/dev/null 2>&1 || true
}
trap cleanup EXIT

git worktree prune
rmdir "$WORKTREE_DIR"
git worktree add --orphan -b $GIT_BRANCH "$WORKTREE_DIR"

cp -R Packages/$PACKAGE_ID/. "$WORKTREE_DIR"

git -C "$WORKTREE_DIR" add .
git -C "$WORKTREE_DIR" commit -m "Release v$PACKAGE_VERSION-git"
git -C "$WORKTREE_DIR" push origin $GIT_BRANCH

echo "✅ Git branch $GIT_BRANCH published for $PACKAGE_ID@$PACKAGE_VERSION."