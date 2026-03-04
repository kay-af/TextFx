#!/bin/bash
set -euo pipefail

# Configure Package vars.
source Scripts/Package.sh

DOCUMENTATION_DOMAIN=text-fx.freedee.me
DOCUMENTATION_BRANCH=documentation
WORKTREE_DIR=$(mktemp -d)

cleanup() {
  git worktree remove "$WORKTREE_DIR" >/dev/null 2>&1 || true
}
trap cleanup EXIT

git worktree prune
rmdir "$WORKTREE_DIR"
git fetch origin $DOCUMENTATION_BRANCH:$DOCUMENTATION_BRANCH 2>/dev/null || true
git worktree add "$WORKTREE_DIR" $DOCUMENTATION_BRANCH

git -C "$WORKTREE_DIR" rm -rf . >/dev/null 2>&1 || true
cp -R Documentation/_site/. "$WORKTREE_DIR"
echo $DOCUMENTATION_DOMAIN > "$WORKTREE_DIR"/CNAME

git -C "$WORKTREE_DIR" add .
git -C "$WORKTREE_DIR" commit -m "docs: update documentation for $PACKAGE_ID@$PACKAGE_VERSION"
git -C "$WORKTREE_DIR" push origin $DOCUMENTATION_BRANCH

echo "✅ Documentation published for $PACKAGE_ID@$PACKAGE_VERSION."