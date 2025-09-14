# File Operations

Termix provides powerful file and directory operations with visual feedback, multi-selection capabilities, and
intelligent conflict handling. This guide covers all file management features, from basic creation to advanced batch
operations.

## Overview

All file operations in Termix are designed to be:

- **Safe**: Confirmation prompts for destructive operations and smart conflict resolution.
- **Informative**: Clear feedback and progress indicators for single or multiple files.
- **Efficient**: Optimized for both small files and large, multi-file operations.
- **Flexible**: Manage single items or select many with Visual Mode.

## Creating Files and Directories

Press `a` to create new files or directories:

1. Press `a` to enter creation mode.
2. Type the desired name.
3. Press `Enter` to create.

### Creation Rules

| Input             | Result       | Description                          |
|:------------------|:-------------|:-------------------------------------|
| `filename.txt`    | File         | Creates a text file                  |
| `script`          | `script.txt` | Auto-adds .txt extension             |
| `folder/`         | Directory    | Trailing slash creates a directory   |
| `path/to/file.md` | Nested file  | Creates parent directories as needed |

Termix intelligently determines where to create new items:

- If a directory is selected, the new item is created inside it.
- Otherwise, it's created in the current directory.

## Renaming Files and Directories

Press `r` to rename the selected item:

1. Select the file or directory.
2. Press `r` to enter rename mode.
3. The current name is pre-filled for easy editing.
4. Press `Enter` to confirm or `Esc` to cancel.

## Yank, Cut, and Move Operations

Termix uses a clipboard-based system that works seamlessly with both single items and multiple selections from Visual
Mode.

### Yanking (Copying) Items (`y`)

1. Select a single item or multiple items using **Visual Mode** (`v`).
2. Press `y` to yank (copy) all selected items to the internal clipboard.
3. Navigate to the destination.
4. Press `p` to paste.

### Moving (Cutting) Items (`x`)

1. Select a single item or multiple items using **Visual Mode** (`v`).
2. Press `x` to cut (move) all selected items to the clipboard.
3. Navigate to the destination.
4. Press `p` to paste.

### Yanking a File's Full Path (`Y`)

To copy the absolute path of an item to your system clipboard for use in other applications:

1. Select the file or directory.
2. Press `Y` (Shift + y). The path is now in your system clipboard.

### Progress Tracking

For large files or batch operations, Termix shows detailed progress:

![Progress Image](/progress.png)

- **Progress bar**: Visual indication of the total completion.
- **Current file**: Shows which file is being processed out of the total batch.
- **Cancellation**: Press `q` during operations to safely cancel.

## Advanced Operations

### Batch Operations with Visual Mode

Visual Mode is the primary way to perform operations on multiple files at once.

1. Press `v` to enter Visual Mode.
2. Select multiple files using the `Space` key.
3. Press `y` (Yank), `x` (Cut), or `d` (Delete) to perform the action on **all selected items**.
4. If yanking or cutting, navigate to the destination and press `p` to paste the entire batch.

### Smart Pasting: Conflict Resolution

When pasting, Termix protects you from accidentally overwriting files. If a file with the same name already exists at
the destination, the operation pauses and asks you what to do.

| Key   | Action          | Description                                                                       |
|:------|:----------------|:----------------------------------------------------------------------------------|
| `S`   | **Skip**        | Skips this one conflicting file and continues with the rest of the batch.         |
| `L`   | **Skip All**    | Automatically skips any other files in this batch that would cause a conflict.    |
| `R`   | **Replace**     | Deletes the existing file and pastes the new one in its place.                    |
| `A`   | **Replace All** | Automatically replaces any other files that conflict during this batch operation. |
| `Esc` | **Cancel**      | Immediately stops the entire paste operation, leaving remaining files untouched.  |

### Cross-Drive Operations

Termix intelligently handles operations across different drives (e.g., from `C:\` to `D:\`):

- **Same drive moves**: Uses a native filesystem move, which is instantaneous.
- **Cross-drive moves**: Automatically performs a copy to the destination, verifies it, and then deletes the original,
  all with a single progress bar.

## Delete Operations

Press `d` to delete the selected item or items.

1. Select a single file or multiple files using **Visual Mode**.
2. Press `d` to start the delete operation.
3. A prompt will ask you to confirm, showing how many items will be deleted.
4. Confirm with `y` or cancel with `n`.

### Delete Features

- **Confirmation required**: All deletes require explicit confirmation.
- **Batch Deletion**: Works with multiple selections from Visual Mode.
- **Recursive Deletion**: Directories are deleted with all their contents.

## Error Handling

### Common Error Scenarios

| Error                 | Cause                                | Resolution                                           |
|:----------------------|:-------------------------------------|:-----------------------------------------------------|
| **Permission denied** | Insufficient file system permissions | Check file/directory permissions                     |
| **File in use**       | Another process has the file locked  | Close other applications using the file              |
| **Disk full**         | Not enough space for the operation   | Free up disk space or choose a different location    |
| **Invalid name**      | Name contains illegal characters     | Use valid filename characters                        |
| **Path too long**     | Exceeds filesystem limits            | Use shorter names or a shallower directory structure |

When errors occur, Termix provides clear messages explaining the problem, allowing you to correct it and retry the
operation.

## Status and Feedback

The status bar provides real-time information:

- **Operation mode**: Shows `VISUAL` when in visual mode, or the current operation.
- **Clipboard contents**: Displays the number of items yanked or cut.
- **Progress information**: Shows completion status for batch operations.
- **Conflict prompts**: Clearly displays options when a paste conflict occurs.

## Keyboard Shortcuts Reference

| Key   | Operation   | Description                                 |
|:------|:------------|:--------------------------------------------|
| `a`   | Add         | Create new file or directory                |
| `r`   | Rename      | Rename selected item                        |
| `y`   | Yank (Copy) | Yank selected item(s) to internal clipboard |
| `Y`   | Yank Path   | Yank full path to **system clipboard**      |
| `x`   | Cut (Move)  | Cut selected item(s) to internal clipboard  |
| `p`   | Paste       | Paste from internal clipboard               |
| `d`   | Delete      | Delete selected item(s) with confirmation   |
| `v`   | Visual Mode | Enter/Exit mode for multi-select            |
| `Esc` | Cancel      | Cancel current operation or clear clipboard |
