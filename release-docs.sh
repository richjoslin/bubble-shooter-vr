#!/bin/sh
set -e

# export VSINSTALLDIR="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community"
# export VisualStudioVersion="15.0"

docfx ./docs/docfx.json

SOURCE_DIR=$PWD
cd ..
TEMP_REPO_DIR=$PWD/bubble-shooter-vr-docfx-temp

echo "remove existing temp dir $TEMP_REPO_DIR"
rm -rf "$TEMP_REPO_DIR"
echo "create temp dir $TEMP_REPO_DIR"
mkdir "$TEMP_REPO_DIR"

echo "clone repo, gh-pages branch"
git clone git@github.com:richjoslin/bubble-shooter-vr.git --branch gh-pages "$TEMP_REPO_DIR"

echo "clear repo dir"
cd "$TEMP_REPO_DIR"
git rm -r *

echo "copy docs into repo"
cp -r "$SOURCE_DIR"/docs/_site/* .

echo "push new docs to remote branch"
git add . -A
git commit -m "auto-update generated documentation"
git push origin gh-pages
