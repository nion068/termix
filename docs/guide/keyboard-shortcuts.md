# Keyboard Shortcuts

Master all of Termix's keyboard shortcuts for maximum productivity. This comprehensive reference covers every key
binding organized by function and context. To view this guide inside the app, press `?` at any time.

## Quick Reference Card

### Essential Shortcuts

| Key               | Action         | Key           | Action          |
|:------------------|:---------------|:--------------|:----------------|
| `↑↓` / `kj`       | Move selection | `Enter` / `l` | Open/Enter      |
| `Backspace` / `h` | Go up          | `s`           | Search          |
| `a`               | Add            | `r`           | Rename          |
| `y`               | Yank (Copy)    | `x`           | Move/Cut        |
| `p`               | Paste          | `d`           | Delete          |
| `t`               | Sort           | `v`           | **Visual Mode** |
| `Y`               | Yank Path      | `?`           | **Help Screen** |
| `q`               | Quit           | `Esc`         | Cancel/Clear    |

## Navigation Shortcuts

### Basic Movement

| Key      | Action          | Description               |
|:---------|:----------------|:--------------------------|
| `↑`      | Move up         | Select previous item      |
| `↓`      | Move down       | Select next item          |
| `k`      | Move up (Vim)   | Vim-style up movement     |
| `j`      | Move down (Vim) | Vim-style down movement   |
| `gg`     | Jump to top     | Select first item in list |
| `G`      | Jump to bottom  | Select last item in list  |
| `Ctrl+d` | Scroll down     | Scroll down half a page   |
| `Ctrl+u` | Scroll up       | Scroll up half a page     |

### Directory Navigation

| Key         | Action           | Description                  |
|:------------|:-----------------|:-----------------------------|
| `Enter`     | Open/Enter       | Open file or enter directory |
| `l`         | Open/Enter (Vim) | Alternative key for opening  |
| `Backspace` | Go up            | Move to parent directory     |
| `h`         | Go up (Vim)      | Vim-style parent navigation  |

### Preview Pane Control

| Key       | Action       | Description                  |
|:----------|:-------------|:-----------------------------|
| `Alt + ↑` | Scroll up    | Scroll preview content up    |
| `Alt + ↓` | Scroll down  | Scroll preview content down  |
| `Alt + ←` | Scroll left  | Scroll preview content left  |
| `Alt + →` | Scroll right | Scroll preview content right |

## File Operations

### Creation and Modification

| Key | Action | Description                               |
|:----|:-------|:------------------------------------------|
| `a` | Add    | Create new file or directory              |
| `r` | Rename | Rename selected item                      |
| `d` | Delete | Delete selected item(s) with confirmation |

### Clipboard and Batch Operations

| Key | Action          | Description                                   |
|:----|:----------------|:----------------------------------------------|
| `v` | **Visual Mode** | Enter/Exit mode for selecting multiple items  |
| `y` | Yank (Copy)     | Yank selected item(s) to internal clipboard   |
| `x` | Cut (Move)      | Cut selected item(s) to internal clipboard    |
| `p` | Paste           | Paste from internal clipboard                 |
| `Y` | Yank Path       | Yank item's full path to **system clipboard** |

## Search and Filter

### Search Control

| Key   | Mode         | Action          | Description                                         |
|:------|:-------------|:----------------|:----------------------------------------------------|
| `s`   | Normal       | Start search    | Enter search mode                                   |
| `Esc` | Search       | Apply filter    | Finish search, navigate results                     |
| `Esc` | Filtered     | Clear filter    | Remove filter, show all files                       |
| `b`   | Filtered Nav | Back to results | Return to search results after entering a directory |

### Search Input

| Key            | Mode   | Action                     |
|:---------------|:-------|:---------------------------|
| Character keys | Search | Add to query               |
| `Backspace`    | Search | Remove from query          |
| `Enter`        | Search | (Same as Esc) Apply filter |

## Application Control

### Session Management

| Key   | Action | Description               |
|:------|:-------|:--------------------------|
| `q`   | Quit   | Exit Termix               |
| `?`   | Help   | Show/hide the help screen |
| `Esc` | Cancel | Cancel current operation  |

### Special Contexts

| Key | Context        | Action                            |
|:----|:---------------|:----------------------------------|
| `y` | Delete confirm | Confirm deletion                  |
| `n` | Delete confirm | Cancel deletion                   |
| `y` | Quit confirm   | Force quit with operations        |
| `n` | Quit confirm   | Cancel quit                       |
| `S` | Paste Conflict | **Skip** current file             |
| `L` | Paste Conflict | **Skip All** conflicting files    |
| `R` | Paste Conflict | **Replace** current file          |
| `A` | Paste Conflict | **Replace All** conflicting files |

## Mode-Specific Shortcuts

### Normal Mode

_Default mode for navigation and file operations_

| Category       | Keys                                       | Actions                |
|:---------------|:-------------------------------------------|:-----------------------|
| **Movement**   | `↑↓kj`, `gg`, `G`                          | Navigate through files |
| **Navigation** | `Enter/l`, `Backspace/h`                   | Open/close directories |
| **Operations** | `a`, `r`, `y`, `Y`, `x`, `p`, `d`          | File operations        |
| **Modes**      | `s` (Search), `t` (Sort), `v` (**Visual**) | Switch to other modes  |
| **App**        | `q` (Quit), `?` (Help)                     | Application control    |

### Visual Mode

_Active when selecting multiple files for batch operations_

| Key         | Action           | Notes                                           |
|:------------|:-----------------|:------------------------------------------------|
| `Space`     | Toggle Selection | Mark or unmark the highlighted item             |
| `a`         | Select All       | Select every item in the current view           |
| `i`         | Invert Selection | Deselect all selected and select all unselected |
| `y` / `x`   | Yank / Cut       | Yank/Cut all selected items to the clipboard    |
| `d`         | Delete           | Delete all selected items                       |
| `v` / `Esc` | Exit Mode        | Return to Normal Mode                           |

### Search Mode

_Active when typing search queries_

| Key             | Action           | Notes                         |
|:----------------|:-----------------|:------------------------------|
| Character keys  | Add to search    | Real-time filtering           |
| `Backspace`     | Remove character | Update results                |
| `Esc` / `Enter` | Apply filter     | Switch to filtered navigation |

### Filtered Navigation Mode

_Active after applying a search filter_

| Key             | Action               | Notes                                      |
|:----------------|:---------------------|:-------------------------------------------|
| Navigation keys | Move through results | Only filtered items are shown              |
| `b`             | Back to results      | Return to the original search results list |
| `Esc`           | Clear filter         | Show all files and return to Normal Mode   |
| `s`             | New search           | Start a fresh search                       |

### Paste Conflict Mode

_Active when a paste operation encounters a file with a duplicate name_

| Key   | Action       |
|:------|:-------------|
| `S`   | Skip         |
| `L`   | Skip All     |
| `R`   | Replace      |
| `A`   | Replace All  |
| `Esc` | Cancel Paste |

## Advanced Key Combinations

### Vim-Style Navigation

For Vim users, these combinations feel natural:

```
h j k l    →    ← ↑ ↓ →
h j k l    →    Parent Up Down Enter
gg         →    Go to top of list
G          →    Go to bottom of list
Ctrl+d     →    Scroll down half a page
Ctrl+u     →    Scroll up half a page
```

### Power User Workflows

**Search and Batch Delete:**

```
s → .tmp → Esc → v → a → d → y
```

(Search for `.tmp` files, enter Visual Mode, Select All, Delete, and Confirm)

**Batch Move with Visual Mode:**

`v → (Space, Space, ...) → x → navigate → p`
(Enter Visual Mode, select items with Space, cut them, navigate, and paste)

## Context-Sensitive Behavior

### Esc Key Behavior

The `Esc` key behaves differently based on context:

| Context                    | Action                            |
|:---------------------------|:----------------------------------|
| Normal mode with filter    | Clear active filter               |
| Normal mode with clipboard | Clear clipboard                   |
| Visual Mode                | Exit Visual Mode                  |
| Search mode                | Apply filter and enter navigation |
| Add/Rename mode            | Cancel operation                  |
| Delete/Quit confirm        | Cancel action                     |
| Paste Conflict             | Cancel paste operation            |

## Memory Aids

### Mnemonic Devices

| Key | Mnemonic          | Action                   |
|:----|:------------------|:-------------------------|
| `a` | **A**dd           | Create new item          |
| `r` | **R**ename        | Rename item              |
| `y` | **Y**ank          | Yank to clipboard (copy) |
| `x` | Cut (e**X**tract) | Move to clipboard        |
| `p` | **P**aste         | Paste from clipboard     |
| `d` | **D**elete        | Delete item              |
| `s` | **S**earch        | Start search             |
| `t` | Sor**t**          | Open sort menu           |
| `v` | **V**isual        | Enter Visual Mode        |
| `q` | **Q**uit          | Exit application         |

## Troubleshooting Key Issues

### Keys Not Working

If shortcuts aren't responding:

1. **Check terminal focus**: Ensure the terminal window has focus.
2. **Verify key support**: Some terminals may not support all key combinations (e.g., `Alt`).
3. **Check for conflicts**: Other applications might intercept keys.

All essential functions have alternative key bindings that work in any standard terminal.
