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
- **Bottom**: A minimal status bar with hints. For a full list of commands, press `?` at any time.

## Basic Navigation

### Moving Around

| Key                 | Action                     |
| :------------------ | :------------------------- |
| `↑` / `k`           | Move selection up          |
| `↓` / `j`           | Move selection down        |
| `gg`                | Jump to the first item     |
| `G`                 | Jump to the last item      |
| `Ctrl+u` / `Ctrl+d` | Scroll up/down half a page |

### Opening Files and Directories

| Key                | Action                                |
| :----------------- | :------------------------------------ |
| `l` or `Enter`     | Open selected file or enter directory |
| `h` or `Backspace` | Go to parent directory                |

Try navigating through your file system using these keys. Notice how the right pane updates to show a preview of the selected file!

## Essential File Operations

### Yanking, Cutting, and Pasting

Termix uses Vim-style verbs for file operations. "Yank" means copy.

**Yank (Copy) a file:**

1.  Select the file.
2.  Press `y` to yank (copy) it to the internal clipboard.
3.  Navigate to the destination.
4.  Press `p` to paste.

**Move (Cut) a file:**

1.  Select the file.
2.  Press `x` to cut it.
3.  Navigate to the destination.
4.  Press `p` to paste.

**Yank a File's Full Path:**

Need to paste a file path into another terminal window?

1.  Select the file or directory.
2.  Press `Y` (Shift + y) to yank its absolute path to the **system clipboard**.

### Creating Files and Folders

Press `a` to add a new file or folder:

1.  Press `a`
2.  Type the name (add `/` at the end for folders)
3.  Press `Enter`

Examples:

- `README.md` - Creates a file
- `new-folder/` - Creates a directory
- `script` - Creates `script.txt` (auto-adds .txt extension)

### Renaming

1.  Select the file/folder you want to rename.
2.  Press `r`.
3.  Edit the name and press `Enter`.

### Deleting

1.  Select the file/folder.
2.  Press `d`.
3.  Confirm with `y` or cancel with `n`.

## Working with Multiple Items: Visual Mode

For batch operations, Termix includes a powerful **Visual Mode**, inspired by Vim.

1.  Press `v` to enter Visual Mode. The status bar will change to indicate you're in a new mode.
2.  Use `k` and `j` to move up and down.
3.  Press `Space` to toggle the selection for the highlighted item. Selected files will be marked.

While in Visual Mode, you have access to new shortcuts:

| Key          | Action                                              |
| :----------- | :-------------------------------------------------- |
| `a`          | Select **all** items in the current directory       |
| `i`          | **Invert** the current selection                    |
| `y`          | **Yank (copy)** all selected items to the clipboard |
| `x`          | **Cut** (mark for moving) all selected items        |
| `d`          | **Delete** all selected items (with confirmation)   |
| `v` or `Esc` | Exit Visual Mode                                    |

### Example Workflow: Moving Multiple Files

1.  Navigate to a directory with several files you want to move.
2.  Press `v` to enter Visual Mode.
3.  Move down with `j` and press `Space` on each file you want to select.
4.  Once all files are selected, press `x` to cut them. You will exit Visual Mode, and the clipboard will show you have multiple items.
5.  Navigate to the destination folder.
6.  Press `p` to paste all the files.

## Smart Pasting & Conflict Resolution

What happens if you try to paste a file into a directory that already has a file with the same name? Termix won't just fail—it will ask you what to do.

When a conflict is detected, you'll see a prompt with these options:

| Key   | Action                                                      |
| :---- | :---------------------------------------------------------- |
| `S`   | **Skip** pasting this one file and move to the next.        |
| `L`   | **Skip All** remaining files that have a name conflict.     |
| `R`   | **Replace** the existing file with the one you are pasting. |
| `A`   | **Replace All** existing files if any more conflicts occur. |
| `Esc` | **Cancel** the entire paste operation.                      |

This gives you full control over your file operations, preventing accidental data loss.

## Managing and Jumping to Favorite Directories: Bookmarks

Tired of navigating to the same project directories over and over? Bookmarks let you save and instantly jump to your favorite locations.

### Creating a Bookmark

1.  Navigate to the directory you want to save.
2.  Press `m`.
3.  Type a memorable name for your bookmark and press `Enter`.

### The Bookmark Menu

From anywhere in the file navigator, press `b` to open the Bookmark Menu. This full-screen menu lists all your saved bookmarks and provides a complete set of tools for managing them.

| Key       | Action                                              |
| :-------- | :-------------------------------------------------- |
| `↑` / `k` | Move selection up                                   |
| `↓` / `j` | Move selection down                                 |
| `Enter`   | Jump to the selected bookmark's directory           |
| `s`       | Enter live filter mode to search by name or path    |
| `r`       | Rename the selected bookmark                        |
| `d`       | Delete the selected bookmark (with confirmation)    |
| `v`       | Enter Visual Mode to select multiple bookmarks      |
| `Esc`     | Close the bookmark menu and return to the file view |

Just like with files, you can press `v` to enter a visual mode for selecting and deleting multiple bookmarks at once.

## The Help Screen

Termix keeps the main interface clean. To see a full list of all available keybindings at any time, simply press `?`. This will open a scrollable help screen. Press `?`, `q`, or `Esc` to close it.

## Organizing Your View with Sorting

Tired of scrolling to find the most recent file? Termix's interactive sort menu lets you reorganize the file list instantly.

1.  Press `t` to open the sort menu.
2.  Use `k` and `j` to highlight an option.
3.  Press `Enter` to apply the sort.

You can sort by Name, Date, or Size, and choose whether to keep folders grouped at the top.

## Search and Filter

One of Termix's most powerful features is real-time, recursive search:

1.  Press `s` to enter search mode.
2.  Start typing to filter files instantly across all subdirectories.
3.  Press `Esc` to apply the filter and navigate the results.
4.  Press `Esc` again to clear the filter.

### Search Tips

- Search is **case-insensitive**.
- Results update in real-time as you type.
- After opening a directory from a search result, press `b` to return to your search list.

## Preview Pane

The right pane shows previews of your files:

- **Text files**: Content with syntax highlighting
- **Images**: Terminal-friendly image preview
- **Directories**: A tree view of the directory's contents
- **Archives**: A browsable list of the archive's contents

### Scrolling Previews

For large files, you can scroll the preview:

| Key         | Action              |
| :---------- | :------------------ |
| `Alt + ↑/↓` | Scroll vertically   |
| `Alt + ←/→` | Scroll horizontally |

## Tips for Success

::: tip The Help Key is Your Friend
The footer is intentionally minimal. Whenever you're unsure of a key, just press `?` to see everything.
:::

::: tip Smart Ignoring
Termix automatically respects `.gitignore` files and ignores common build directories like `node_modules`, `bin`, and `obj`. This keeps your search results clean and relevant.
:::

::: tip Vim Users
If you're comfortable with Vim, you'll feel right at home with `v` for Visual Mode, `j`/`k`/`h`/`l` for movement, and `y`/`p` for yank/paste.
:::
