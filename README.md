# Termix

![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)  ![NuGet](https://img.shields.io/nuget/v/Termix)

_A modern, high‑performance file navigator for your terminal._  
Built with .NET 9 and Spectre.Console, Termix delivers a fluid, visually rich, flicker‑free interface for navigating,
searching, and managing files—all from your terminal.

---

## Demo

https://github.com/user-attachments/assets/53192fdb-a882-47c0-8138-c9b4d2cd84c8

Watch a 1‑minute walkthrough → [Termix on Vimeo](https://vimeo.com/1105824424)

---

## 🚀 Why You’ll Love Termix

* Instant recursive filtering as you type (`S` → search mode). Press `Esc` to apply the filter, navigate matches, then
  `Esc` again to clear.
* Create, rename, or delete files and directories right within the UI (`A`, `R`, `D`).
* Respects `.gitignore` and filters out `bin/`, `obj/`, `node_modules/` by default for speed and clarity.
* Double‑buffered UI ensures flicker‑free rendering and a smooth two‑pane layout.
* Live syntax highlighting for `.cs`, `.js`, `.ts`, `.py`, and more supported at preview time.
* Vim‑style shortcuts: use `J/K` with arrows for fast navigation.
* Nerd‑Font glyphs (configurable fallback to ASCII mode).
* Cross‑platform: compatible with Windows, macOS, and Linux. :contentReference[oaicite:1]{index=1}

---

## 📦 Installation

### Prerequisites

You must have **.NET 9 SDK** or later installed. :contentReference[oaicite:2]{index=2}

### Install as a Global Tool

```bash
dotnet tool install --global Termix
````

Launch it simply with:

```bash
termix
```

### Update

```bash
dotnet tool update --global Termix
```

### Uninstall

```bash
dotnet tool uninstall --global Termix
```

### From Source (Optional)

```bash
git clone https://github.com/amrohan/termix.git
cd termix
dotnet run --configuration Release
```

---

## Icon Support & Fallback Mode

Termix comes with beautiful Nerd Font icons by default. For terminals without Nerd Font support, add `--no-icons` to
fall back to text‑only glyphs like `[DIR]/` and `…` for ultimate compatibility. :contentReference[oaicite:5]{index=5}

---

## ⌨️ Keyboard Shortcuts

### Navigation

| Keys              | Action                       |
|-------------------|------------------------------|
| ↑ / ↓             | Move selection up/down       |
| `J` / `K`         | Vim-style movement           |
| `Enter` / `L`     | Open file or enter directory |
| `Backspace` / `H` | Go to parent directory       |
| `Home` / `End`    | Jump to first or last entry  |
| `Q` or `Esc`      | Quit Termix                  |

### Search, Filter & File Ops

| Keys                      | Action                                          |
|---------------------------|-------------------------------------------------|
| `S`                       | Enter search mode (ﬁlters recursively as typed) |
| `Esc` (during typing)     | Apply filter and navigate results               |
| `Esc` (during navigation) | Clear filter and show all items                 |
| `B`                       | Move back to search results when navigating     |
| `A`                       | Create a new file or folder                     |
| `R`                       | Rename selected file or folder                  |
| `D`                       | Delete selected item (with confirmation)        |

### Preview Pane Scrolling

- `Alt + ↑ / ↓`: Scroll vertically
- `Alt + ← / →`: Scroll horizontally

---

## 🛠️ Contributing

Contributions, bug reports, and feature requests are very welcome!
See [the Issues page](https://github.com/amrohan/termix/issues) to get started.

### Developer Roadmap

Fork & clone the repo, then open Visual Studio, VS Code, or your editor of choice. Key components:

- `Services/ActionService.cs`: Create / Rename / Delete / Search logic
- `Services/IgnoreService.cs`: `.gitignore` and default ignore rules
- `Services/IconProvider.cs`: File type icon mappings (_extensionIcons_)
- `Services/CustomSyntaxHighlighter.cs`: Add new language themes
- `UI/FileManagerRenderer.cs`: Styling and pane rendering logic

Branch from `main`, commit using descriptive messages, and send pull requests with tests included.


---

## 🧠 Acknowledgements

- Made possible by the fantastic Spectre.Console
- Glob-style ignore matching via DotNet.Glob
- Icons powered by the NerdFonts project
- Inspired by the elegance and simplicity of .NET Global Tools

---

## 📄 License

Termix is licensed under the **MIT License**. 
