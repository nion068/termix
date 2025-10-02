# Introduction

Welcome to Termix, a modern, high-performance file navigator designed to make command-line file management fast, intuitive, and visually enjoyable.

<VideoPlayer src="/videos/termix.mp4" />

## What is Termix?

Termix is a terminal-based file manager built with .NET 9 and [Spectre.Console](https://spectreconsole.net/). It provides a rich, interactive interface for navigating, searching, and managing files and your favorite directories directly from your terminal.

## Why Choose Termix?

Traditional command-line file operations can be cumbersome and error-prone. Termix bridges the gap between the power of the command line and the convenience of modern file managers by offering:

### **Performance First**

- **Instant Search**: Real-time recursive filtering across entire directory trees.
- **Memory Efficient**: Handles large directories without performance degradation.
- **Responsive UI**: Double-buffered rendering ensures smooth, flicker-free updates.

### **A Beautiful and Intuitive Interface**

- **Two-Pane Layout**: Clear separation between file list and preview.
- **Syntax Highlighting**: Live preview of code files with proper syntax coloring.
- **Icon Support**: Beautiful file type icons with Nerd Fonts integration.
- **Progress Tracking**: Visual indicators for long-running operations.

### **Powerful Functionality**

- **File Management**: Create, rename, move, copy, and delete files and directories.
- **Smart Clipboard**: Copy/cut files and paste them anywhere in the file system.
- **Advanced Bookmarking**: Save frequently accessed directories as named bookmarks. Instantly jump to any saved location through a powerful, live-filterable menu.
- **Bulk Operations**: Handle multiple files and bookmarks efficiently with visual selection.

### **Completely Keyboard-Driven**

- **Vim-Style Navigation**: Use `J`/`K` alongside arrow keys for movement.
- **Intuitive Shortcuts**: Memorable keyboard shortcuts for all operations.
- **Modal Interface**: Different modes for navigation, search, bookmarks, and file operations.

## Key Features

- **Advanced Bookmark Management**: Save, name, rename, and delete bookmarks for your favorite directories. Instantly jump to any location via a live-searchable menu (`b`).
- **Instant Recursive Filtering**: Start typing to filter files and directories in real-time.
- **Intuitive File Operations**: Create, rename, move, copy, and delete with visual feedback.
- **Smart Ignoring**: Respects `.gitignore` and filters out build directories automatically.
- **Live Syntax Highlighting**: Preview various file types (`.cs`, `.js`, `.ts`, `.py`, and more).
- **Cross-Platform**: Works seamlessly on Windows, macOS, and Linux.
- **Nerd Font Support**: Enhanced experience with icons, with an ASCII fallback for compatibility.

## Built With a Modern Tech Stack

Termix is built on a foundation of powerful and reliable technologies to ensure a high-quality experience:

- **.NET 9**: The latest version of the .NET platform, providing cutting-edge performance, security, and cross-platform capabilities.
- **Spectre.Console**: A best-in-class .NET library for creating beautiful and interactive terminal applications.
- **SixLabors.ImageSharp**: A powerful and flexible image processing library, enabling rich graphical previews directly in the terminal.

This combination ensures Termix is fast, reliable, and feature-rich on any platform.

## Getting Started

Ready to try Termix? Head over to the [Installation Guide](./installation.md) to get started, or jump straight to the [Quick Start](./quick-start.md) to see Termix in action.

::: tip Pro Tip
Termix works best with a terminal that supports [Nerd Fonts](https://www.nerdfonts.com/) for the full icon experience, but it gracefully falls back to ASCII characters if icons aren't available.
:::

## Community & Support

- **GitHub**: [amrohan/termix](https://github.com/amrohan/termix)
- **Issues**: Report bugs and request features on [GitHub Issues](https://github.com/amrohan/termix/issues)
- **Discussions**: Join the conversation in [GitHub Discussions](https://github.com/amrohan/termix/discussions)

## License

Termix is open-source software licensed under the [MIT License](https://github.com/amrohan/termix/blob/main/LICENSE.txt).
