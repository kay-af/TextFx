#!/bin/bash

# Setup PackageID.
PACKAGE_ID=me.freedee.text-fx

# Setup the version from the package.json file.
PACKAGE_VERSION=$(jq -r '.version' "Packages/$PACKAGE_ID/package.json")