# Quick Start

Get up and running with Termix in minutes! This guide covers the essentials to help you become productive with Termix right away.

## First Launch

After [installing Termix](./installation.md), launch it from your terminal:

```bash
termix
```

You'll see Termix's two-pane interface:

- **Left pane**: File and directory listing
- **Right pane**: File preview (shows content of selected file)
- **Bottom**: Status bar with keyboard shortcuts and current mode

## Basic Navigation

### Moving Around

| Key            | Action                       |
| -------------- | ---------------------------- |
| `↑` / `↓`      | Move selection up/down       |
| `J` / `K`      | Vim-style movement (up/down) |
| `Home` / `End` | Jump to first/last item      |

### Opening Files and Directories

| Key                | Action                                |
| ------------------ | ------------------------------------- |
| `Enter` or `L`     | Open selected file or enter directory |
| `Backspace` or `H` | Go to parent directory                |

Try navigating through your file system using these keys. Notice how the right pane updates to show a preview of the selected file!

## Essential File Operations

### Creating Files and Folders

Press `A` to create a new file or folder:

1.  Press `A`
2.  Type the name (add `/` at the end for folders)
3.  Press `Enter`

Examples:

- `README.md` - Creates a file
- `new-folder/` - Creates a directory
- `script` - Creates `script.txt` (auto-adds .txt extension)

### Renaming

1.  Select the file/folder you want to rename
2.  Press `R`
3.  Edit the name
4.  Press `Enter`

### Copying and Moving (Single Item)

**Copy a file:**

1.  Select the file
2.  Press `C` (copy to clipboard)
3.  Navigate to destination
4.  Press `P` (paste)

**Move a file:**

1.  Select the file
2.  Press `X` (cut to clipboard)
3.  Navigate to destination
4.  Press `P` (paste)

### Deleting (Single Item)

1.  Select the file/folder
2.  Press `D`
3.  Confirm with `y` or cancel with `n`

## Working with Multiple Files/Dir: Visual Mode

For batch operations, Termix includes a powerful **Visual Mode**, inspired by Vim.

1.  Press `V` to enter Visual Mode. The status bar will change to indicate you're in a new mode.
2.  Use `↑` / `↓` or `j`/`k` to move up and down.
3.  Press `Space` to toggle the selection for the highlighted file. Selected files will be marked.

While in Visual Mode, you have access to new shortcuts:

| Key          | Action                                            |
| ------------ | ------------------------------------------------- |
| `A`          | Select **all** items in the current directory     |
| `I`          | **Invert** the current selection                  |
| `C`          | **Copy** all selected items to the clipboard      |
| `X`          | **Cut** (mark for moving) all selected items      |
| `D`          | **Delete** all selected items (with confirmation) |
| `V` or `Esc` | Exit Visual Mode                                  |

### Example Workflow: Moving Multiple Files

1.  Navigate to a directory with several files you want to move.
2.  Press `V` to enter Visual Mode.
3.  Move down and press `Space` on each file you want to select.
4.  Once all files are selected, press `X` to cut them. You will exit Visual Mode, and the clipboard will show you have multiple items.
5.  Navigate to the destination folder.
6.  Press `P` to paste all the files.

## Smart Pasting: Conflict Resolution

What happens if you try to paste a file into a directory that already has a file with the same name? Termix won't just fail—it will ask you what to do.

When a conflict is detected, the paste operation will pause, and you'll see a prompt with these options:

| Key   | Action                                                      |
| ----- | ----------------------------------------------------------- |
| `S`   | **Skip** pasting this one file and move to the next.        |
| `L`   | **Skip All** remaining files that have a name conflict.     |
| `R`   | **Replace** the existing file with the one you are pasting. |
| `A`   | **Replace All** existing files if any more conflicts occur. |
| `Esc` | **Cancel** the entire paste operation.                      |

This gives you full control over your file operations, preventing accidental data loss.

---

## Organizing Your View with Sorting

Tired of scrolling to find the most recent file? Termix's interactive sort menu lets you reorganize the file list instantly.

1.  Press `T` to open the sort menu.
2.  Use `↑` / `↓` or `j`/`k` to highlight an option.
3.  Press `Enter` to apply the sort.

You can sort by:

- **Name**: Alphabetically (A-Z) or reverse (Z-A).
- **Date**: See the newest or oldest files first.
- **Size**: Find the largest or smallest files.

You can also choose to keep folders grouped at the top ("Folders First") or mix them in with your files for a true chronological or size-based sort.

## Search and Filter

One of Termix's most powerful features is real-time search:

1.  Press `S` to enter search mode
2.  Start typing to filter files instantly
3.  Press `Esc` to apply the filter and navigate results
4.  Press `Esc` again to clear the filter

The search is **recursive**, meaning it searches through all subdirectories automatically!

### Search Tips

- Search is **case-insensitive**
- Matches partial file names
- Works across your entire directory tree
- Results update in real-time as you type

## Preview Pane

The right pane shows previews of your files:

- **Text files**: Content with syntax highlighting
- **Images**: Terminal-friendly image preview
- **Directories**: A tree view of the directory's contents

### Scrolling Previews

For large files, you can scroll the preview:

| Key         | Action              |
| ----------- | ------------------- |
| `Alt + ↑/↓` | Scroll vertically   |
| `Alt + ←/→` | Scroll horizontally |

---

## Quick Reference

Here are the essential shortcuts you'll use daily:

### Navigation & Modes

- `↑↓` or `JK` - Move selection
- `Enter` or `L` - Open/Enter
- `Backspace` or `H` - Go up
- `V` - **Enter/Exit Visual Mode**
- `Q` - Quit

### File Operations

- `A` - Add (create file/folder)
- `R` - Rename
- `D` - Delete
- `C` - Copy
- `X` - Move/Cut
- `P` - Paste

### View & Search

- `T` - **Open sort menu**
- `S` - Start search
- `Esc` - Apply filter / Clear filter
- `B` - Return to search results (when navigating from filtered results)

---

## Common Workflows

### Organizing Files

1.  **Find recently modified config files:**

    - Press `T`, select "Date: Newest First", press `Enter`
    - The most recently edited files will be at the top.

2.  **Move specific log files into an `archive` folder:**
    - Press `A`, type `archive/`, press `Enter`.
    - Press `V` to enter Visual Mode.
    - Select several log files with the `Space` key.
    - Press `X` to cut the selected files.
    - Navigate into the `archive` folder and press `P` to paste them.

### Finding Files

1.  **Search for a specific file:**

    - Press `S`, type part of the filename.
    - Press `Esc` to navigate results.
    - Use arrow keys to select, `Enter` to open.

2.  **Return to search results:**
    - After opening a file from a filtered search, press `B` to go back to your results list.

## Tips for Success

::: tip Smart Ignoring
Termix automatically respects `.gitignore` files and ignores common build directories like `node_modules`, `bin`, and `obj`. This keeps your search results clean and relevant.
:::

::: tip Vim Users
If you're comfortable with Vim, you'll feel right at home with `V` for Visual Mode, `J`/`K` for movement, and `H`/`L` for navigation.
:::

::: tip Large Directories
Termix handles large directories efficiently. The recursive search works even with thousands of files, updating results in real-time.
:::
