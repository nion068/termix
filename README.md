# Termix
![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)
&nbsp;
![Nuget](https://img.shields.io/nuget/v/Termix)

A modern, high-performance file navigator for your terminal. Built with .NET and Spectre.Console, Termix provides a fluid, visually-rich, and flicker-free experience for browsing, searching, and managing files directly from the command line.

---
# Demo


Watch Termix in action: [Video](https://vimeo.com/1105824424)

https://github.com/user-attachments/assets/53192fdb-a882-47c0-8138-c9b4d2cd84c8




---

## Why You’ll Love Termix

-   **🚀 Instant Search & Filtering**: Filter files and folders recursively as you type with a debounced, high-performance search.
-   **✍️ Interactive File Management**: Create, rename, and delete files and directories on the fly without leaving the UI.
-   **🧠 Smart Ignore**: Automatically respects `.gitignore` rules and ignores common development directories (`bin`, `obj`, `node_modules`) for a cleaner, faster search.
-   **✨ Flicker-Free Rendering**: A smooth double-buffered UI means no redraw artifacts.
-   **🖥️ Modern Two-Pane Layout**: An intuitive file list and live preview side-by-side.
-   **🎨 Live Syntax Highlighting**: Instant preview for `.cs`, `.js`, `.ts`, `.py` and more.
-   **📁 Nerd Font Icons (Recommended)**: Beautiful file and folder glyphs for quick recognition, with a fallback mode for all terminals.
-   **Cross-Platform**: Runs natively on Windows, macOS, and Linux.
-   **⌨️ Vim-Inspired Controls**: Use `J/K` alongside arrow keys for lightning-fast navigation.

---

## 🚀 Installation

Termix is available as a .NET Global Tool, which is the recommended way to install it.

### Prerequisites

You need the **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** or newer installed.

### Install Command

Open your terminal and run the following command:

```bash
dotnet tool install --global Termix
```

Then, you can run the application by simply typing:
```bash
termix```

#### Updating to the Latest Version
```bash
dotnet tool update --global Termix
```

#### Uninstalling
```bash
dotnet tool uninstall --global Termix
```

<details>
<summary><b>Alternative: Build and Run from Source</b></summary>

If you prefer to build the project yourself:

```bash
git clone https://github.com/amrohan/termix.git
cd termix
dotnet run --configuration Release
```
</details>

---

## Icon Support & Fallback Mode

Termix uses **[Nerd Fonts](https://www.nerdfonts.com/)** by default for pretty icons. For the best experience, it is recommended to install a Nerd Font and configure your terminal to use it.

### Running Without Icons

If you don't have a Nerd Font installed, icons may appear as `?` or `□`. To solve this, simply run Termix with the `--no-icons` flag for a clean, universal experience:

```bash
termix --no-icons
```

This will replace glyphs with simple, text-based indicators (`[DIR]/`, `..`) that work in any terminal.

---

## ⌨️ How to Use

### File Navigation

| Key(s)         | Action                       |
|:---------------|:-----------------------------|
| ↑ / ↓          | Move selection up or down    |
| `J` / `K`      | Vim-style selection movement |
| `Enter`        | Open file or directory       |
| `Backspace`    | Go to parent directory       |
| `Home` / `End` | Jump to start/end of list    |
| `Q` / `Escape` | Quit Termix                  |

### File Management & Search

| Key(s)         | Action                                                                   |
|:---------------|:-------------------------------------------------------------------------|
| `S`            | **Search/Filter** the current directory and subdirectories as you type.    |
| `A`            | **Add** a new file or directory (e.g., `new.txt` or `folder/`).            |
| `R`            | **Rename** the selected file or directory.                                 |
| `D`            | **Delete** the selected item (with confirmation).                          |
| `Esc`          | Exit search mode or cancel an action.                                    |

### Preview Pane Controls

| Key(s)        | Action                      |
|:--------------|:----------------------------|
| `Alt` + ↑ / ↓ | Scroll content vertically   |
| `Alt` + ← / → | Scroll content horizontally |

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check the **[issues page](https://github.com/amrohan/termix/issues)**.

### Development Guide

Want to contribute to the code? Here are the key areas:
-   **Core File Logic**: See `Services/ActionService.cs` for Create, Rename, Delete, and Search operations.
-   **Ignore Rules**: Modify default ignores or `.gitignore` parsing in `Services/IgnoreService.cs`.
-   **Adding File Icons**: Modify the `_extensionIcons` dictionary in `Services/IconProvider.cs`.
-   **Adding Syntax Highlighting**: Add a new `LanguageTheme` in `Services/CustomSyntaxHighlighter.cs`.
-   **Styling & Colors**: Tweak `Style` objects in `UI/FileManagerRenderer.cs` and `Services/CustomSyntaxHighlighter.cs`.

---

## 🙏 Acknowledgements

-   This project would not be possible without the incredible **[Spectre.Console](https://spectreconsole.net/)** library.
-   Glob pattern matching for the ignore service is powered by **[DotNet.Glob](https://github.com/dazinator/DotNet.Glob)**.
-   Icons are provided by the **[Nerd Fonts](https://www.nerdfonts.com/)** project.
-   Inspired by the power and simplicity of .NET global tools.

---

## 📝 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
