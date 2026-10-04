# RuneUO Client

RuneUO is the Ultima Online client distributed through the **Rune UO** portal. The Rune UO launcher downloads it and keeps it up to date, so any shard listed in the portal can offer it to its players.

RuneUO is a fork of [ClassicUO](https://github.com/ClassicUO/ClassicUO), an open source implementation of the Ultima Online Classic Client. This project would not exist without the work of the ClassicUO team and its contributors. Please consider supporting them:

* [ClassicUO on GitHub](https://github.com/ClassicUO/ClassicUO)
* [ClassicUO Patreon](http://www.patreon.com/classicuo)
* [ClassicUO Discord](https://discord.gg/VdyCpjQ)

> [!NOTE]
> Please report RuneUO issues in this repository, not in ClassicUO's. Bugs that also reproduce on upstream ClassicUO are best reported there.

# Introduction
RuneUO keeps ClassicUO's goals: emulate the standard client versions, run natively on every major platform and stay compatible with free shard emulators (ServUO, ModernUO, RunUO, POL, Sphere...). On top of that it adds features aimed at shard owners, so they can customise their content without shipping full replacement archives.

The client is built on [FNA](https://fna-xna.github.io/) and C#, and supports:
* Windows [DirectX 11, OpenGL, Vulkan]
* Linux   [OpenGL, Vulkan]
* macOS   [Metal, OpenGL, MoltenVK]

# Download & Play
Install the **Rune UO launcher** from the Rune UO portal. It downloads RuneUO, keeps it updated and lets you pick any shard that supports it.

# Shard customisation
RuneUO can load shard-specific assets as loose files placed in the Ultima Online data folder. These are read before the original archives, so they can both add new entries and replace existing ones.

| Asset | Path (inside the UO folder) | Format |
| --- | --- | --- |
| Sounds | `Sounds/<id>.wav` | WAV, 22050 Hz, mono, 16-bit |
| Strings (cliloc) | `Clilocs.txt` | Plain text, one `<number> <text>` per line, `#` for comments. Numbers above 3,100,000 are free |
| Gumps | `Gumps/<id>.gump` | Width, height and run-length rows, same as the gump archive entries |
| Item art | `Art/Statics/<itemId>.art` | Same run-length data as the art archive |
| Land art | `Art/Land/<landId>.art` | 1012 raw pixels of a land tile |

Folder and file names are matched ignoring case on every platform. Invalid files are logged and the original asset is used instead. Custom assets can be reloaded in-game from the Options menu without restarting the client.

Other changes over ClassicUO include animations stored only in `verdata.mul`, additional mount mappings and fixes to tiled buttons and map patches. See the commit history for details.

# Build
```
git clone --recursive https://github.com/RuneUO/runeuo-client.git
bash runeuo-client/scripts/build-naot.sh
```
Binaries are placed in the `bin/dist` folder. The executable is `RuneUO.exe` (`RuneUO` launcher script on Linux and macOS).
> [!WARNING]
> To execute .sh scripts on Windows, use Git Bash, which is installed with Git: https://git-scm.com/download/win

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/download). Builds are x64 only, because the launcher hosts are x64.

# Publish to the Rune UO portal
RuneUO is the portal's official client. The launcher installs it once and starts it with the game files of each server set to use it. `scripts/deploy-portal.sh` publishes a release, one platform at a time. Each publish replaces that platform's whole client on the portal.
```
export RUNE_PORTAL_URL=https://play.example.com RUNE_PORTAL_USER=admin
scripts/deploy-portal.sh --release                       # the zips of the RuneUO-main-release GitHub release (all three platforms)
scripts/deploy-portal.sh --zip RuneUO-win-x64-release.zip --platform win-x64 --version 1.1.0.42
scripts/deploy-portal.sh --build --version 1.1.0.42      # build this machine's platform and publish it
```
The script signs in as a portal admin. It reads the password from `RUNE_PORTAL_PASSWORD` or asks for it. `--dry-run` prepares the zips without uploading them, and `--help` lists every option. An admin can also upload the same zip from the portal, in Admin → Launcher → RuneUO client.

# Contribute
Contributions are welcome. Open an issue or a pull request in this repository.

Projects, namespaces and binaries use the `RuneUO` name: `RuneUO.sln`, `src/RuneUO.*` and `tests/RuneUO.UnitTests`. The launcher starts `RuneUO.exe`, which loads the client library `RuneUO.Client`.

Plugin compatibility keeps a few `ClassicUO` names on purpose:
* The plugin host assembly is still named `ClassicUO`, although its file is `RuneUO.exe`, because assistants such as ClassicAssist look it up by that name.
* The types in `src/RuneUO.Bootstrap/src/ClassicUOCompat.cs` keep their `ClassicUO` namespace for the same reason.
* The plugin API (`cuoapi`) is unchanged.

Upstream ClassicUO changes can still be merged, but files touched on both sides need their `ClassicUO` namespaces switched to `RuneUO`.

# Credits
* [ClassicUO](https://github.com/ClassicUO/ClassicUO) by andreakarasho and contributors, the base of this client.

ClassicUO itself was written using these projects as reference:
* [OrionUO](https://github.com/hotride/orionuo)
* [Razor](https://github.com/msturgill/razor)
* [UltimaXNA](https://github.com/ZaneDubya/UltimaXNA)
* [ServUO](https://github.com/servuo/servuo)

Backend:
* [FNA](https://github.com/FNA-XNA/FNA)

# Legal
This work is released under the BSD 2-Clause license, see [LICENSE.md](LICENSE.md). The original ClassicUO copyright notice is retained as required by the license.

This project does not distribute any copyrighted game assets. In order to run this client you need to legally obtain a copy of the Ultima Online Classic Client.
Using a custom client to connect to official UO servers is strictly forbidden. We do not assume any responsibility for the usage of this client.

Ultima Online(R) © 2024 Electronic Arts Inc. All Rights Reserved.

This program will not transfer any information to other networked systems unless specifically requested by the user or the person installing or operating it.
