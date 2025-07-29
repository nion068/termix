# Termix

![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)

A modern, high-performance file navigator for your terminal. Built with .NET and Spectre.Console, Termix provides a fluid, visually-rich, and flicker-free experience for browsing and managing files directly from the command line.

---

## 🚀 Demo

Watch Termix in action:

https://github.com/user-attachments/assets/c5d1dcb3-6428-441f-a807-6367a464adee

---

## Why You’ll Love Termix

- **Flicker-Free Rendering**: Smooth double-buffered UI with no redraw artifacts.
- **Modern Two-Pane Layout**: File list and live preview side-by-side.
- **Advanced Scrolling**: Dynamic scrollbars and smooth vertical/horizontal performance.
- **Live File Preview**: Instant preview with syntax highlighting (`.cs`, `.js`, `.py`, etc.).
- **Nerd Font Icons**: Beautiful file/folder glyphs for quick recognition.
- **Cross-Platform**: Runs on Windows, macOS, and Linux.
- **Vim-Inspired Controls**: Use `J/K/H/L` alongside arrow keys for lightning-fast navigation.

---

## Getting Started

Follow these instructions to get Termix up and running on your local machine.

### Prerequisites

1. **.NET 8 SDK (or newer):** Required to build and run Termix.
2. **A Nerd Font:** Essential for icons to render correctly:
   - Download from [Nerd Fonts](https://www.nerdfonts.com/font-downloads)
   - Install on your system
   - Set your terminal to use the newly installed Nerd Font

---

### Option 1: Install via .NET Global Tool (Recommended)

Install Termix with a single command:

```
dotnet tool install --global termix
```

Run Termix:

```
termix
```

Update to the latest version:

```
dotnet tool update --global termix
```

To uninstall:

```
dotnet tool uninstall --global termix
```

---

### Option 2: Build and Run from Source

If you prefer working directly with the source:

```
git clone https://github.com/amrohan/termix.git
cd termix
dotnet build --configuration Release
dotnet run
```

This allows for full access to the source code, customizable changes, and local testing.

---

## Publishing & Packaging

Termix is configured as a .NET global tool and automatically published to [nuget.org](https://nuget.org). Relevant metadata ensures your README, license, and authorship appear on the package listing.

### ✅ Important `.csproj` Settings

```

  true
  termix
  1.0.0
  Termix
  YourName
  A modern, high‑performance terminal file navigator.
  termix, file‑navigator, terminal
  https://github.com/amrohan/termix
  MIT
  README.md




```

This enables:

- Packing Termix as a global CLI tool
- Embedding README and license in the NuGet listing
- Improved discoverability via tags and metadata ([learn.microsoft.com][1], [matthewregis.dev][2], [dev.to][3])

---

## GitHub Actions CI: Automatic Publishing

Place the following in `.github/workflows/publish.yml` to auto-publish on tagged releases (`vX.Y.Z`):

```
name: Publish Termix to NuGet
on:
  push:
    tags:
      - 'v*.*.*'
jobs:
  build-and-publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      - run: dotnet restore
      - run: dotnet build --configuration Release --no-restore
      - run: dotnet pack Termix.csproj --configuration Release --no-build --output nupkg
      - run: dotnet nuget push nupkg/*.nupkg \
          --source https://api.nuget.org/v3/index.json \
          --api-key ${{ secrets.NUGET_API_KEY }} \
          --skip-duplicate
```

**Setup Steps:**

1. Push a version tag, e.g. `git tag v1.0.0 && git push origin v1.0.0`
2. Add repository secret `NUGET_API_KEY` with your NuGet token
3. GitHub Actions will pack and publish Termix on each tag push ([weekenddive.com][4], [meziantou.net][5])

---

## How to Use

### File Navigation

| Key(s)         | Action                       |
| -------------- | ---------------------------- |
| ↑ / ↓ or W / S | Move selection up or down    |
| J / K          | Vim-style selection movement |
| Enter          | Open file or directory       |
| Backspace      | Go to parent directory       |
| Home / End     | Jump to start/end of list    |
| Q / Escape     | Quit Termix                  |

### Preview Pane Controls

| Key(s)                     | Action                      |
| -------------------------- | --------------------------- |
| Alt + ↑ / ↓ or Alt + K / J | Scroll content vertically   |
| Alt + ← / → or Alt + H / L | Scroll content horizontally |

---

## Customization & Extension

- **Adding Icons**: Modify `_extensionIcons` in `Services/IconProvider.cs`.
- **Adding Syntax Highlighting**: Extend `CustomSyntaxHighlighter.cs` and update `FilePreviewService.cs`.
- **Styling & Colors**: Tweak color and style settings in `UI/FileManagerRenderer.cs` and `Program.cs`.

---

## Project Structure

```
/
├── Models/
│   └── FileSystemItem.cs
├── Services/
│   ├── FileSystemService.cs
│   ├── IconProvider.cs
│   ├── FilePreviewService.cs
│   └── CustomSyntaxHighlighter.cs
├── UI/
│   ├── FileManagerRenderer.cs
│   └── DoubleBufferedRenderer.cs
├── FileManager.cs
└── Program.cs
```

---

## License

This project is licensed under the MIT License—see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgements

- Built using **[Spectre.Console](https://spectreconsole.net/)** for rich terminal UI
- Thanks to **Nerd Fonts** for enabling beautiful icons
- Inspired by the power of .NET global tools and the open NuGet ecosystem ([meziantou.net][5])

---

## Summary

Termix combines a fluid, modern terminal UI with easy .NET global tool distribution.  
Install with `dotnet tool install --global termix`, use seamlessly, and get automatic updates via tags—with full automation via GitHub Actions.

**Happy navigating!** 🚀

[1]: https://learn.microsoft.com/en-us/nuget/nuget-org/package-readme-on-nuget-org?utm_source=chatgpt.com "Package readme on NuGet.org - Learn Microsoft"
[2]: https://matthewregis.dev/posts/5-steps-for-publishing-a-dotnet-tool-to-nuget-org?utm_source=chatgpt.com "5 steps for publishing a dotnet tool to nuget.org - Matthew Regis"
[3]: https://dev.to/kasuken/readme-generator-a-global-dotnet-tool-for-your-next-project-57bg?utm_source=chatgpt.com "readme-generator: a global dotnet tool for your next project"
[4]: https://www.weekenddive.com/dotnet/publishing-nuget-packages-using-github-actions?utm_source=chatgpt.com "Publishing NuGet packages using GitHub Actions - WeekendDive"
[5]: https://www.meziantou.net/publishing-a-nuget-package-following-best-practices-using-github.htm?utm_source=chatgpt.com "Publishing a NuGet package using GitHub and GitHub Actions"
