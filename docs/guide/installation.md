# Installation

This guide explains how to install **Termix** on your system.  
We offer multiple installation methods so you can choose the one that best fits your setup.

## Prerequisites

Before installing Termix, ensure you have:

- A terminal application (e.g., Windows Terminal, iTerm2, Warp, Kitty, GNOME Terminal)
- **(Optional, but highly recommended)** A [Nerd Font](https://www.nerdfonts.com/) for enhanced icon support

## Install via Script (All Platforms) <Badge type="tip" text="Recommended"/>

Termix provides script-based installers for **macOS**, **Linux**, and **Windows**.

### macOS / Linux

Requirements:

* `curl`
* `unzip` or `tar`
* `jq`

```bash
# Install or update to the latest version
curl -fsSL https://raw.githubusercontent.com/amrohan/termix/main/install.sh | bash

# Install a specific version
curl -fsSL https://raw.githubusercontent.com/amrohan/termix/main/install.sh | bash -s v1.5.0

# Uninstall
curl -fsSL https://raw.githubusercontent.com/amrohan/termix/main/install.sh | bash -s uninstall
```

### Windows (PowerShell)

Requirements:

* PowerShell 5.1 or later (PowerShell Core 7+ recommended)

```powershell
# Install or update to latest
iex (iwr "https://raw.githubusercontent.com/amrohan/termix/main/install.ps1")

# Install specific version
iex (iwr "https://raw.githubusercontent.com/amrohan/termix/main/install.ps1") -Tag v1.5.0

# Uninstall
iex (iwr "https://raw.githubusercontent.com/amrohan/termix/main/install.ps1") -Uninstall
```

## Install as a .NET Global Tool

Requires:

* **.NET 9 SDK** or later

```bash
dotnet tool install --global termix
```

Run Termix:

```bash
termix
```

Update:

```bash
dotnet tool update --global termix
```

Uninstall:

```bash
dotnet tool uninstall --global termix
```

## Install from Source

```bash
# Clone repository
git clone https://github.com/amrohan/termix.git
cd termix

# Build and install locally
dotnet pack
dotnet tool install --global --add-source ./nupkg termix
```

---

## Font Setup (Recommended)

For icons, install a [Nerd Font](https://www.nerdfonts.com/):

**Recommended:**

* FiraCode Nerd Font
* JetBrains Mono Nerd Font
* Cascadia Code Nerd Font

1. Download from [nerdfonts.com](https://www.nerdfonts.com/)
2. Install on your system
3. Set it as your terminal font

::: tip Icon Fallback
If Nerd Fonts aren’t installed, Termix falls back to ASCII characters.
:::


## Command-Line Options

```bash
# Launch normally with icons
termix

# Launch without icons
termix --no-icons
```

## Troubleshooting

### Command Not Found

* Ensure .NET tools are in `PATH`
* Restart your terminal
* Verify with:

```bash
dotnet tool list --global
```

### .NET SDK Not Found

* Install .NET 9 from [dotnet.microsoft.com](https://dotnet.microsoft.com/)
* Check version with `dotnet --version`

### Permission Errors

```bash
# Linux/macOS
sudo dotnet tool install --global termix

# Windows (Run as Administrator)
dotnet tool install --global termix
```

::: info Version Info
**Current stable version:** 1.5.1

**Minimum .NET:** 9.0

**Platforms:** Windows, macOS, Linux
:::
