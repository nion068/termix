# Termix

![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg) ![NuGet](https://img.shields.io/nuget/v/Termix)

A modern, high-performance file navigator for your terminal. Built with .NET 9 and Spectre.Console, Termix delivers a fluid, visually rich, flicker-free interface for navigating, searching, and managing files—all from your terminal.

---

## Demo

https://github.com/user-attachments/assets/53192fdb-a882-47c0-8138-c9b4d2cd84c8

Watch a 1‑minute walkthrough → [Termix on Vimeo](https://vimeo.com/1105824424)

## Features

- **Instant Recursive Filtering**: Start typing to filter files and directories in real-time.
- **Intuitive File Operations**: Create, rename, move, copy, and delete files and directories directly within the UI with progress bar for long-running tasks.
- **Smart Ignoring**: Respects `.gitignore` and automatically filters out `bin/`, `obj/`, and `node_modules/` to keep your view clean.
- **Flicker-Free UI**: A double-buffered interface ensures smooth rendering in a two-pane layout.
- **Live Syntax Highlighting**: Preview a variety of file types with syntax highlighting, including `.cs`, `.js`, `.ts`, and `.py`.
- **Vim-Style Navigation**: Use `J`/`K` alongside arrow keys for efficient movement.
- **Nerd Font Support**: Enhance your experience with Nerd Font icons, with a fallback to ASCII for maximum compatibility.
- **Cross-Platform**: Works seamlessly on Windows, macOS, and Linux.

---

## Installation

### Prerequisites

You must have the **.NET 9 SDK** (or a later version) installed on your system.

### Install as a Global Tool

```bash
dotnet tool install --global termix
```

Launch the application by simply running:

```bash
termix
```

### Install via Chocolatey (Windows)

Termix is now available on **Chocolatey**, the Windows package manager ⁠just run:

```powershell
choco install termix
```

This installs the latest stable release.

### Updating Termix

- **With .NET Global Tool:**

  ```bash
  dotnet tool update --global termix
  ```

- **With Chocolatey (Windows):**

  ```powershell
  choco upgrade termix
  ```

### Uninstalling Termix

- **With .NET Global Tool:**

  ```bash
  dotnet tool uninstall --global termix
  ```

- **With Chocolatey (Windows):**

  ```powershell
  choco uninstall termix
  ```

---

## Keyboard Shortcuts

### Navigation

| Keys              | Action                       |
| :---------------- | :--------------------------- |
| ↑ / ↓             | Move selection up/down       |
| `J` / `K`         | Vim-style movement           |
| `Enter` / `L`     | Open file or enter directory |
| `Backspace` / `H` | Go to parent directory       |
| `Home` / `End`    | Jump to first or last entry  |
| `Q`               | Quit Termix                  |

### File Operations

| Keys                  | Action                                              |
| :-------------------- | :-------------------------------------------------- |
| `S`                   | Enter search mode (filters recursively as you type) |
| `Esc` (during search) | Apply the filter and navigate the results           |
| `Esc` (after search)  | Clear the filter and show all items                 |
| `B`                   | Go back to the search results when navigating       |
| `A`                   | Create a new file or folder                         |
| `R`                   | Rename the selected file or folder                  |
| `D`                   | Delete the selected item (with confirmation)        |
| `X`                   | Move the selected file or folder                    |
| `C`                   | Copy the selected file or folder                    |
| `P`                   | Paste the file or folder                            |

### Preview Pane

| Keys          | Action              |
| :------------ | :------------------ |
| `Alt + ↑ / ↓` | Scroll vertically   |
| `Alt + ← / →` | Scroll horizontally |

---

## Contributing

We welcome contributions, bug reports, and feature requests! Please visit the [Issues page](https://github.com/amrohan/termix/issues) to get started.

### Developer Roadmap

If you'd like to contribute to the development of Termix, fork and clone the repository, then open it in your preferred editor such as Visual Studio or VS Code. Key components of the project include:

- `Services/ActionService.cs`: Logic for creating, renaming, deleting, and searching.
- `Services/IgnoreService.cs`: Handles `.gitignore` and default ignore rules.
- `Services/IconProvider.cs`: Manages file type icon mappings.
- `Services/CustomSyntaxHighlighter.cs`: Add new language themes here.
- `UI/FileManagerRenderer.cs`: Contains the logic for styling and rendering panes.

Please branch from `main`, use descriptive commit messages.

---

## Acknowledgements

- This project is made possible by the fantastic [Spectre.Console](https://spectreconsole.net/).
- Glob-style ignore matching is handled by [DotNet.Glob](https://github.com/dazinator/DotNet.Glob).
- Icons are powered by the [Nerd Fonts](https://www.nerdfonts.com/) project.
- Inspired by the simplicity and elegance of .NET Global Tools.

---

## License

Termix is licensed under the **MIT License**.
