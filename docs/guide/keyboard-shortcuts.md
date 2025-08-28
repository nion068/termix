# Keyboard Shortcuts

Master all of Termix's keyboard shortcuts for maximum productivity. This comprehensive reference covers every key binding organized by function and context.

## Quick Reference Card

### Essential Shortcuts

| Key               | Action         | Key           | Action          |
| ----------------- | -------------- | ------------- | --------------- |
| `↑↓` / `JK`       | Move selection | `Enter` / `L` | Open/Enter      |
| `Backspace` / `H` | Go up          | `S`           | Search          |
| `A`               | Add            | `R`           | Rename          |
| `C`               | Copy           | `X`           | Move/Cut        |
| `P`               | Paste          | `D`           | Delete          |
| `T`               | Sort           | `V`           | **Visual Mode** |
| `Q`               | Quit           | `Esc`         | Cancel/Clear    |

## Navigation Shortcuts

### Basic Movement

| Key           | Action          | Description               |
| ------------- | --------------- | ------------------------- |
| `↑`           | Move up         | Select previous item      |
| `↓`           | Move down       | Select next item          |
| `J`           | Move down (Vim) | Vim-style down movement   |
| `K`           | Move up (Vim)   | Vim-style up movement     |
| `Home` / `gg` | Jump to top     | Select first item in list |
| `End` / `G`   | Jump to bottom  | Select last item in list  |
| `Ctrl+d`      | Scroll down     | Scroll down half a page   |
| `Ctrl+u`      | Scroll up       | Scroll up half a page     |

### Directory Navigation

| Key         | Action      | Description                  |
| ----------- | ----------- | ---------------------------- |
| `Enter`     | Open/Enter  | Open file or enter directory |
| `L`         | Open/Enter  | Alternative key for opening  |
| `Backspace` | Go up       | Move to parent directory     |
| `H`         | Go up (Vim) | Vim-style parent navigation  |

### Preview Pane Control

| Key       | Action       | Description                  |
| --------- | ------------ | ---------------------------- |
| `Alt + ↑` | Scroll up    | Scroll preview content up    |
| `Alt + ↓` | Scroll down  | Scroll preview content down  |
| `Alt + ←` | Scroll left  | Scroll preview content left  |
| `Alt + →` | Scroll right | Scroll preview content right |

## File Operations

### Creation and Modification

| Key | Action | Description                               |
| --- | ------ | ----------------------------------------- |
| `A` | Add    | Create new file or directory              |
| `R` | Rename | Rename selected item                      |
| `D` | Delete | Delete selected item(s) with confirmation |

### Clipboard and Batch Operations

| Key | Action          | Description                                  |
| --- | --------------- | -------------------------------------------- |
| `V` | **Visual Mode** | Enter/Exit mode for selecting multiple items |
| `C` | Copy            | Copy selected item(s) to clipboard           |
| `X` | Move/Cut        | Cut selected item(s) to clipboard            |
| `P` | Paste           | Paste from clipboard                         |

## Search and Filter

### Search Control

| Key   | Mode     | Action          | Description                     |
| ----- | -------- | --------------- | ------------------------------- |
| `S`   | Normal   | Start search    | Enter search mode               |
| `Esc` | Search   | Apply filter    | Finish search, navigate results |
| `Esc` | Filtered | Clear filter    | Remove filter, show all files   |
| `B`   | Any      | Back to results | Return to search results        |

### Search Input

| Key            | Mode   | Action                     |
| -------------- | ------ | -------------------------- |
| Character keys | Search | Add to query               |
| `Backspace`    | Search | Remove from query          |
| `Enter`        | Search | (Same as Esc) Apply filter |

## Application Control

### Session Management

| Key   | Action | Description              |
| ----- | ------ | ------------------------ |
| `Q`   | Quit   | Exit Termix              |
| `Esc` | Cancel | Cancel current operation |

### Special Contexts

| Key | Context        | Action                            |
| --- | -------------- | --------------------------------- |
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
| -------------- | ------------------------------------------ | ---------------------- |
| **Movement**   | `↑↓JK`, `Home/End`, `G/gg`                 | Navigate through files |
| **Navigation** | `Enter/L`, `Backspace/H`                   | Open/close directories |
| **Operations** | `A R C X D P`                              | File operations        |
| **Modes**      | `S` (Search), `T` (Sort), `V` (**Visual**) | Switch to other modes  |
| **Exit**       | `Q`                                        | Quit application       |

### Visual Mode

_Active when selecting multiple files for batch operations_

| Key         | Action           | Notes                                           |
| ----------- | ---------------- | ----------------------------------------------- |
| `Space`     | Toggle Selection | Mark or unmark the highlighted item             |
| `A`         | Select All       | Select every item in the current view           |
| `I`         | Invert Selection | Deselect all selected and select all unselected |
| `C` / `X`   | Copy / Cut       | Copy/Cut all selected items to the clipboard    |
| `D`         | Delete           | Delete all selected items                       |
| `V` / `Esc` | Exit Mode        | Return to Normal Mode                           |

### Search Mode

_Active when typing search queries_

| Key             | Action           | Notes                         |
| --------------- | ---------------- | ----------------------------- |
| Character keys  | Add to search    | Real-time filtering           |
| `Backspace`     | Remove character | Update results                |
| `Esc` / `Enter` | Apply filter     | Switch to filtered navigation |

### Filtered Navigation Mode

_Active after applying a search filter_

| Key             | Action               | Notes                                    |
| --------------- | -------------------- | ---------------------------------------- |
| Navigation keys | Move through results | Only filtered items are shown            |
| `B`             | Back to results      | Return to the original search results    |
| `Esc`           | Clear filter         | Show all files and return to Normal Mode |
| `S`             | New search           | Start a fresh search                     |

### Paste Conflict Mode

_Active when a paste operation encounters a file with a duplicate name_

| Key   | Action       |
| ----- | ------------ |
| `S`   | Skip         |
| `L`   | Skip All     |
| `R`   | Replace      |
| `A`   | Replace All  |
| `Esc` | Cancel Paste |

## Advanced Key Combinations

### Vim-Style Navigation

For Vim users, these combinations feel natural:

```
h j k l    →    H K J L
← ↑ ↓ →    →    Parent Up Down Enter
gg         →    Go to top of list
G          →    Go to bottom of list
Ctrl+d     →    Scroll down half a page
Ctrl+u     →    Scroll up half a page
```

### Power User Workflows

**Search and Batch Delete:**

```
S → .tmp → Esc → V → A → D → y
```

(Search for `.tmp` files, enter Visual Mode, Select All, Delete, and Confirm)

**Batch Move with Visual Mode:**

V → (Space, Space, ...) → X → navigate → P```
(Enter Visual Mode, select items with Space, cut them, navigate, and paste)

## Context-Sensitive Behavior

### Esc Key Behavior

The `Esc` key behaves differently based on context:

| Context                    | Action                            |
| -------------------------- | --------------------------------- |
| Normal mode with filter    | Clear active filter               |
| Normal mode with clipboard | Clear clipboard                   |
| Visual Mode                | Exit Visual Mode                  |
| Search mode                | Apply filter and enter navigation |
| Add/Rename mode            | Cancel operation                  |
| Delete/Quit confirm        | Cancel action                     |
| Paste Conflict             | Cancel paste operation            |

## Memory Aids

### Mnemonic Devices

| Key | Mnemonic          | Action               |
| --- | ----------------- | -------------------- |
| `A` | **A**dd           | Create new item      |
| `R` | **R**ename        | Rename item          |
| `C` | **C**opy          | Copy to clipboard    |
| `X` | Cut (e**X**tract) | Move to clipboard    |
| `P` | **P**aste         | Paste from clipboard |
| `D` | **D**elete        | Delete item          |
| `S` | **S**earch        | Start search         |
| `T` | Sor**t**          | Open sort menu       |
| `V` | **V**isual        | Enter Visual Mode    |
| `Q` | **Q**uit          | Exit application     |

## Troubleshooting Key Issues

### Keys Not Working

If shortcuts aren't responding:

1.  **Check terminal focus**: Ensure the terminal window has focus.
2.  **Verify key support**: Some terminals may not support all key combinations (e.g., `Alt`).
3.  **Check for conflicts**: Other applications might intercept keys.

All essential functions have alternative key bindings that work in any standard terminal.
