#!/usr/bin/env bash
#
# Publishes RuneUO to a Rune Portal as its official client. The launcher then
# installs it for every server set to the RuneUO client.
#
# A release is the build's zip, the one the Deploy workflow attaches to a
# GitHub release as RuneUO-<platform>-release.zip. Publishing replaces that
# platform's whole release on the portal: files the new build dropped stop
# reaching players too.
#
# Sources (pick one; --release is the default):
#   --release [TAG]   download the zips from the GitHub release TAG
#                     (default RuneUO-main-release); the version comes from
#                     the RuneUO-<platform>-manifest.xml beside each zip
#   --zip FILE        a release zip already on disk (one --platform, --version)
#   --build           build this machine's platform with build-naot.sh and
#                     zip bin/dist (--version, or git describe)
#
# Options:
#   --portal URL      the portal (or RUNE_PORTAL_URL), e.g. https://play.example.com
#   --user NAME       an admin's username (or RUNE_PORTAL_USER)
#                     the password is RUNE_PORTAL_PASSWORD, or asked for
#   --platform P      win-x64, linux-x64 or osx-x64; repeat for several
#                     (default: all three for --release, this machine's for --build)
#   --version V       the version to publish under (1.1.0.42)
#   --executable F    the file Play starts, if not the platform's default
#                     (RuneUO.exe on Windows, RuneUO elsewhere)
#   --dry-run         get the zips ready and stop before uploading
#
# Example:
#   RUNE_PORTAL_URL=https://play.example.com RUNE_PORTAL_USER=admin \
#     scripts/deploy-portal.sh --release

set -euo pipefail

script_dir="$(cd -P -- "$(dirname -- "$0")" && pwd -P)"
repo_dir="$(cd -- "$script_dir/.." && pwd -P)"

source_kind="release"
tag="RuneUO-main-release"
zip_file=""
portal="${RUNE_PORTAL_URL:-}"
user="${RUNE_PORTAL_USER:-}"
platforms=()
version=""
executable=""
dry_run=0

die() { echo "deploy-portal: $*" >&2; exit 1; }
usage() { sed -n '2,36p' "$0" | sed 's/^# \{0,1\}//'; exit "${1:-0}"; }

while [[ $# -gt 0 ]]; do
  case "$1" in
    --release)
      source_kind="release"
      if [[ $# -gt 1 && "$2" != --* ]]; then tag="$2"; shift; fi ;;
    --zip) source_kind="zip"; zip_file="${2:?--zip needs a file}"; shift ;;
    --build) source_kind="build" ;;
    --portal) portal="${2:?--portal needs a URL}"; shift ;;
    --user) user="${2:?--user needs a name}"; shift ;;
    --platform) platforms+=("${2:?--platform needs a value}"); shift ;;
    --version) version="${2:?--version needs a value}"; shift ;;
    --executable) executable="${2:?--executable needs a path}"; shift ;;
    --dry-run) dry_run=1 ;;
    -h|--help) usage 0 ;;
    *) echo "deploy-portal: unknown option $1" >&2; usage 1 ;;
  esac
  shift
done

host_platform() {
  case "$(uname -s)" in
    Linux) echo linux-x64 ;;
    Darwin) echo osx-x64 ;;
    MINGW* | MSYS* | CYGWIN*) echo win-x64 ;;
    *) die "unsupported platform: $(uname -s)" ;;
  esac
}

for p in "${platforms[@]}"; do
  case "$p" in win-x64|linux-x64|osx-x64) ;; *) die "unknown platform $p (win-x64, linux-x64 or osx-x64)" ;; esac
done

work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

# Each entry: platform|zip path|version
releases=()

case "$source_kind" in
  release)
    command -v gh >/dev/null || die "--release needs the GitHub CLI (gh)"
    [[ ${#platforms[@]} -gt 0 ]] || platforms=(win-x64 linux-x64 osx-x64)
    for p in "${platforms[@]}"; do
      echo "Downloading $p from $tag…"
      gh release download "$tag" --repo RuneUO/runeuo-client --dir "$work/$p" \
        --pattern "RuneUO-$p-release.zip" --pattern "RuneUO-$p-manifest.xml" \
        || die "the $tag release has no $p build"
      v="$version"
      if [[ -z "$v" ]]; then
        # ManifestCreator writes the build's version on the release element.
        v="$(grep -o '<release [^>]*' "$work/$p/RuneUO-$p-manifest.xml" | grep -o 'version="[^"]*"' | head -n1 | cut -d'"' -f2 || true)"
        [[ -n "$v" ]] || die "no version in RuneUO-$p-manifest.xml: pass --version"
      fi
      releases+=("$p|$work/$p/RuneUO-$p-release.zip|$v")
    done
    ;;
  zip)
    [[ -f "$zip_file" ]] || die "no such file: $zip_file"
    [[ ${#platforms[@]} -eq 1 ]] || die "--zip needs exactly one --platform"
    [[ -n "$version" ]] || die "--zip needs --version"
    releases+=("${platforms[0]}|$zip_file|$version")
    ;;
  build)
    p="$(host_platform)"
    if [[ ${#platforms[@]} -gt 0 && ( ${#platforms[@]} -ne 1 || "${platforms[0]}" != "$p" ) ]]; then
      die "--build makes this machine's platform only ($p)"
    fi
    command -v zip >/dev/null || die "--build needs zip"
    v="${version:-$(git -C "$repo_dir" describe --tags --always 2>/dev/null | sed 's/^RuneUO-//; s/[^0-9A-Za-z.+-]/-/g')}"
    [[ -n "$v" ]] || die "no version: pass --version"
    rm -rf "$repo_dir/bin/dist"
    "$script_dir/build-naot.sh"
    (cd "$repo_dir/bin/dist" && zip -qr "$work/RuneUO-$p-release.zip" .)
    releases+=("$p|$work/RuneUO-$p-release.zip|${v:0:32}")
    ;;
esac

for r in "${releases[@]}"; do
  IFS='|' read -r p z v <<<"$r"
  echo "  $p  $v  $(du -h "$z" | cut -f1)  $z"
done
if [[ $dry_run -eq 1 ]]; then
  echo "Dry run: nothing uploaded."
  exit 0
fi

[[ -n "$portal" ]] || die "no portal: pass --portal or set RUNE_PORTAL_URL"
[[ -n "$user" ]] || die "no admin: pass --user or set RUNE_PORTAL_USER"
portal="${portal%/}"
password="${RUNE_PORTAL_PASSWORD:-}"
if [[ -z "$password" ]]; then
  read -r -s -p "Password for $user on $portal: " password
  echo
fi

# The portal's CSRF check is a double submit: the same value as a cookie and
# a header. Any value works, so one is made here.
csrf="$(od -An -N32 -tx1 /dev/urandom | tr -d ' \n')"
jar="$work/cookies"

echo "Signing in to $portal as $user…"
login_body="$(printf '{"username":%s,"password":%s}' \
  "$(printf '%s' "$user" | python3 -c 'import json,sys; print(json.dumps(sys.stdin.read()))')" \
  "$(printf '%s' "$password" | python3 -c 'import json,sys; print(json.dumps(sys.stdin.read()))')")"
status="$(curl -sS -o "$work/login.json" -w '%{http_code}' -c "$jar" \
  -H "Content-Type: application/json" -H "X-CSRF-Token: $csrf" -b "csrf=$csrf" \
  --data-binary @- "$portal/api/v1/auth/login" <<<"$login_body" || true)"
[[ "$status" != 000 ]] || die "cannot reach $portal"
[[ "$status" == 200 ]] || die "sign-in failed (HTTP $status): $(cat "$work/login.json")"
session="$(awk '$6 == "rp_session" { print $7 }' "$jar" | tail -n1)"
[[ -n "$session" ]] || die "sign-in answered without a session"

for r in "${releases[@]}"; do
  IFS='|' read -r p z v <<<"$r"
  echo "Publishing $p $v…"
  form=(-F "platform=$p" -F "version=$v" -F "file=@$z;type=application/zip")
  [[ -n "$executable" ]] && form+=(-F "executable=$executable")
  status="$(curl -sS -o "$work/publish.json" -w '%{http_code}' \
    -H "X-CSRF-Token: $csrf" -b "rp_session=$session; csrf=$csrf" \
    "${form[@]}" "$portal/admin/runeuo-client/publish" || true)"
  [[ "$status" == 200 ]] || die "$p: publish failed (HTTP $status): $(cat "$work/publish.json")"
  echo "  $(cat "$work/publish.json")"
done

curl -sS -o /dev/null -X POST -H "X-CSRF-Token: $csrf" -b "rp_session=$session; csrf=$csrf" \
  "$portal/api/v1/auth/logout" || true
echo "Done. Launchers pick the new client up on their next check."
