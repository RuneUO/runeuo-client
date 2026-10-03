#!/bin/bash

set -e

# Resolve paths from the script location so it can be run from any folder
script_dir="$(cd -P -- "$(dirname -- "$0")" && pwd -P)"
bootstrap_project="$script_dir/../src/ClassicUO.Bootstrap/src/ClassicUO.Bootstrap.csproj"
client_project="$script_dir/../src/ClassicUO.Client"
output_directory="$script_dir/../bin/dist"

# The bootstrap hosts (net472 on Windows, mono kickstart on Linux/macOS) are x64 only,
# so the client library must be x64 too, even on arm64 machines.
arch="x64"

case $(uname -s) in
  Linux) target="linux-$arch" ;;
  Darwin) target="osx-$arch" ;;
  MINGW* | MSYS* | CYGWIN*) target="win-$arch" ;;
  *)
    echo "Unsupported platform: $(uname -s)"
    exit 1
    ;;
esac

echo "Building RuneUO for $target"

dotnet publish "$bootstrap_project" -c Release -o "$output_directory"
dotnet publish "$client_project" -c Release -p:NativeLib=Shared -p:OutputType=Library -r "$target" -o "$output_directory"
