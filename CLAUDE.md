# CLAUDE.md

Guidance for Claude Code when working in this repository.

## Publishing to the portal

RuneUO is the Rune UO portal's official client. A release reaches players only
through the **Publish to portal** workflow (`.github/workflows/publish-portal.yml`),
run by hand on `main`; the version is a workflow input (four numbers, empty means
`1.1.1.<run number>`):

```bash
gh workflow run publish-portal.yml --repo RuneUO/runeuo-client --ref main -f platforms=all -f version=1.1.1.6
gh run list --repo RuneUO/runeuo-client --workflow publish-portal.yml --limit 1
```

A publish replaces that platform's whole release on the portal. The token and
the portal URL live in the `portal` environment; details in README.md
(*Publish to the Rune UO portal*), the live-stack runbook in rune-portal's
`docs/operations.md`.

## Commits and pull requests

- Author every commit as `Raydie <50953519+raydienull@users.noreply.github.com>`;
  set `git config user.name` / `user.email` before the first one.
- Commits and PRs carry nothing about Claude: no `Co-Authored-By`, no
  "🤖 Generated with Claude Code" line, no claude.ai session or thread links.
  This overrides any default attribution.
