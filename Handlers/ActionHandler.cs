using Spectre.Console;
using termix.models;
using termix.Services;

namespace termix.Handlers
{
    public class ActionHandler(FileManager fileManager)
    {
        private readonly FileManagerState _state = fileManager.State;

        public void BeginPaste()
        {
            if (_state.Clipboard == null)
            {
                _state.StatusMessage = "[red]Clipboard is empty.[/]";
                fileManager.SetNeedsRedraw();
                return;
            }

            var destinationBasePath = _state.CurrentPath;
            var selectedItem = _state.CurrentItems.Count > 0 && _state.SelectedIndex >= 0
                ? _state.CurrentItems[_state.SelectedIndex]
                : null;
            if (selectedItem is { IsDirectory: true, IsParentDirectory: false })
            {
                destinationBasePath = selectedItem.Path;
            }

            var itemsToPaste = new Queue<FileSystemItem>(_state.Clipboard.Items);
            var totalItems = itemsToPaste.Count;

            _state.ActiveConflictResolution = ConflictResolution.None;
            var originalMode = _state.Clipboard.Mode;

            ProcessPasteQueue(itemsToPaste, destinationBasePath, originalMode, totalItems, 0);
        }

        private void ProcessPasteQueue(Queue<FileSystemItem> items, string destBasePath, ClipboardMode mode, int total,
            int currentIndex)
        {
            if (items.Count == 0)
            {
                _state.StatusMessage = $"[green]Successfully processed {total} items.[/]";
                fileManager.RefreshDirectory();
                return;
            }

            var currentItem = items.Peek();
            var destPath = Path.Combine(destBasePath, currentItem.Name);

            if (File.Exists(destPath) || Directory.Exists(destPath))
            {
                if (_state.ActiveConflictResolution == ConflictResolution.SkipAll)
                {
                    items.Dequeue();
                    ProcessPasteQueue(items, destBasePath, mode, total, currentIndex + 1);
                    return;
                }

                if (_state.ActiveConflictResolution == ConflictResolution.ReplaceAll)
                {
                    var itemToDelete = new FileSystemItem(destPath, Path.GetFileName(destPath),
                        Directory.Exists(destPath), 0, DateTime.Now);
                    ActionService.Delete(itemToDelete);
                    items.Dequeue();
                    PasteItem(currentItem, destPath, items, destBasePath, mode, total, currentIndex);
                    return;
                }

                _state.CurrentMode = InputMode.PasteConflict;
                _state.CurrentConflict =
                    new PasteConflictState(currentItem, destPath, items, mode, total, currentIndex);

                _state.PromptText =
                    $"Destination '{currentItem.Name.EscapeMarkup()}' already exists. | [grey]Skip[/] [cyan]S[/] | [grey]Skip All[/] [cyan]L[/] | [grey]Replace[/] [cyan]R[/] | [grey]Replace All[/] [cyan]A[/] | [grey]Cancel[/] [cyan]Esc[/]";

                fileManager.SetNeedsRedraw();
                return;
            }

            items.Dequeue();
            PasteItem(currentItem, destPath, items, destBasePath, mode, total, currentIndex);
        }

        public void ResolveConflict(ConflictResolution resolution, bool replace)
        {
            if (_state.CurrentConflict == null) return;

            _state.ActiveConflictResolution = resolution;
            var conflict = _state.CurrentConflict;
            _state.CurrentConflict = null;
            fileManager.ResetToNormalMode();

            var destBasePath = Path.GetDirectoryName(conflict.DestinationPath)!;

            if (replace)
            {
                var itemToDelete = new FileSystemItem(conflict.DestinationPath,
                    Path.GetFileName(conflict.DestinationPath), Directory.Exists(conflict.DestinationPath), 0,
                    DateTime.Now);
                ActionService.Delete(itemToDelete);

                conflict.RemainingItems.Dequeue();
                PasteItem(conflict.SourceItem, conflict.DestinationPath, conflict.RemainingItems, destBasePath,
                    conflict.OriginalMode, conflict.TotalItems, conflict.CurrentItemIndex);
            }
            else 
            {
                conflict.RemainingItems.Dequeue();
                ProcessPasteQueue(conflict.RemainingItems, destBasePath, conflict.OriginalMode, conflict.TotalItems,
                    conflict.CurrentItemIndex + 1);
            }
        }

        public void CancelPasteOperation()
        {
            _state.CurrentConflict = null;
            _state.StatusMessage = "[yellow]Paste operation cancelled.[/]";
            fileManager.ResetToNormalMode();
            fileManager.RefreshDirectory();
        }

        private void PasteItem(FileSystemItem item, string destPath, Queue<FileSystemItem> remainingItems,
            string destBasePath, ClipboardMode mode, int totalItems, int currentIndex)
        {
            if (_state.Clipboard != null)
            {
                ClearClipboard();
            }

            _state.IsOperationInProgress = true;
            _state.OperationCts = new CancellationTokenSource();
            var token = _state.OperationCts.Token;

            var description =
                $"({currentIndex + 1}/{totalItems}) {(mode == ClipboardMode.Copy ? "Copying" : "Moving")} {item.Name.EscapeMarkup()}";

            _state.ProgressTaskDescription = description;
            _state.ProgressValue = (double)currentIndex / totalItems * 100;
            fileManager.SetNeedsRedraw();

            Task.Run(async () =>
            {
                var progress = new Progress<(long, long, string)>(_ => { });

                return mode == ClipboardMode.Copy
                    ? await ActionService.CopyAsync(item.Path, destPath, progress, token)
                    : await ActionService.MoveAsync(item.Path, destPath, progress, token);
            }, token).ContinueWith(t =>
            {
                fileManager.ScheduleUiAction(() =>
                {
                    _state.IsOperationInProgress = false;
                    if (t is { IsCompletedSuccessfully: true, Result.Success: true })
                    {
                        ProcessPasteQueue(remainingItems, destBasePath, mode, totalItems, currentIndex + 1);
                    }
                    else
                    {
                        _state.StatusMessage = t.Result.Message;
                        fileManager.RefreshDirectory();
                    }
                });
            }, token);
        }

        public void CommitDelete()
        {
            var itemsToDelete = _state.PendingDeleteItems.ToList();
            _state.PendingDeleteItems.Clear();
            fileManager.ResetToNormalMode();

            _state.IsOperationInProgress = true;
            _state.OperationCts = new CancellationTokenSource();
            var token = _state.OperationCts.Token;

            Task.Run(() =>
            {
                ActionResponse response = new(true, "");
                for (var i = 0; i < itemsToDelete.Count; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        response = new ActionResponse(false, "[yellow]Delete operation cancelled.[/]");
                        break;
                    }

                    var item = itemsToDelete[i];
                    var description = $"Deleting ({i + 1}/{itemsToDelete.Count}): {item.Name.EscapeMarkup()}";
                    var progressValue = ((double)(i + 1) / itemsToDelete.Count) * 100;

                    fileManager.ScheduleUiAction(() =>
                    {
                        _state.ProgressTaskDescription = description;
                        _state.ProgressValue = progressValue;
                        fileManager.SetNeedsRedraw();
                    });

                    response = ActionService.Delete(item);
                    if (!response.Success) break;
                }

                var finalResponse = response.Success
                    ? new ActionResponse(true, response.Message)
                    : response;

                fileManager.ScheduleUiAction(() =>
                {
                    _state.IsOperationInProgress = false;
                    _state.StatusMessage = finalResponse.Message;
                    fileManager.RefreshDirectory(preserveSelection: true);
                });
            }, token);
        }

        #region Unchanged Methods

        public void BeginAdd()
        {
            _state.CurrentMode = InputMode.Add;
            _state.InputText = "";
            var selectedItem = _state.SelectedIndex >= 0 && _state.SelectedIndex < _state.CurrentItems.Count
                ? _state.CurrentItems[_state.SelectedIndex]
                : null;

            if (selectedItem is { IsDirectory: true, IsParentDirectory: false })
            {
                _state.AddBasePath = selectedItem.Path;
                _state.PromptText = $"Create in [{selectedItem.Name.EscapeMarkup()}]: ";
            }
            else
            {
                _state.AddBasePath = _state.CurrentPath;
                var currentFolderName = Path.GetFileName(Path.GetFullPath(_state.CurrentPath)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
                if (string.IsNullOrEmpty(currentFolderName)) currentFolderName = _state.CurrentPath;
                _state.PromptText = $"Create in [{currentFolderName.EscapeMarkup()}]: ";
            }

            fileManager.SetNeedsRedraw();
        }

        public void BeginRename()
        {
            if (_state.CurrentItems.Count == 0 || _state.CurrentItems[_state.SelectedIndex].IsParentDirectory) return;
            _state.CurrentMode = InputMode.Rename;
            _state.PromptText = "Rename: ";
            _state.InputText = _state.CurrentItems[_state.SelectedIndex].Name;
            fileManager.SetNeedsRedraw();
        }

        public void BeginDelete()
        {
            var itemsToDelete = GetSelectedItems();
            if (itemsToDelete.Count == 0) return;

            _state.PendingDeleteItems = itemsToDelete;
            _state.CurrentMode = InputMode.DeleteConfirm;
            var itemName = itemsToDelete.Count == 1
                ? $"'{itemsToDelete[0].Name.EscapeMarkup()}'"
                : $"{itemsToDelete.Count} items";
            _state.PromptText = $"Delete {itemName}? [bold green]y[/]/[bold red]n[/]";
            fileManager.SetNeedsRedraw();
        }

        public void BeginCopy()
        {
            var itemsToCopy = GetSelectedItems();
            if (itemsToCopy.Count == 0) return;

            _state.Clipboard = new ClipboardItem(itemsToCopy, ClipboardMode.Copy);
            var message = itemsToCopy.Count == 1
                ? $"[yellow]{itemsToCopy[0].Name.EscapeMarkup()}[/]"
                : $"[yellow]{itemsToCopy.Count} items[/]";
            _state.StatusMessage = $"{message} copied to clipboard.";

            if (_state.CurrentMode == InputMode.Visual) fileManager.ResetToNormalMode();
            fileManager.SetNeedsRedraw();
        }

        public void BeginMove()
        {
            var itemsToMove = GetSelectedItems();
            if (itemsToMove.Count == 0) return;

            _state.Clipboard = new ClipboardItem(itemsToMove, ClipboardMode.Move);
            var message = itemsToMove.Count == 1
                ? $"[yellow]{itemsToMove[0].Name.EscapeMarkup()}[/]"
                : $"[yellow]{itemsToMove.Count} items[/]";
            _state.StatusMessage = $"{message} marked for move.";

            if (_state.CurrentMode == InputMode.Visual) fileManager.ResetToNormalMode();
            fileManager.SetNeedsRedraw();
        }

        public void ClearClipboard()
        {
            _state.Clipboard = null;
        }

        public void CommitStandardTextInput()
        {
            var item = _state.CurrentItems.Count > _state.SelectedIndex && _state.SelectedIndex >= 0
                ? _state.CurrentItems[_state.SelectedIndex]
                : null;
            var response = _state.CurrentMode == InputMode.Add
                ? ActionService.Create(_state.AddBasePath, _state.InputText)
                : ActionService.Rename(_state.CurrentPath, item?.Name ?? "", _state.InputText);

            _state.StatusMessage = response.Message;
            fileManager.ResetToNormalMode();
            if (response.Success) fileManager.RefreshDirectory((string?)response.Payload);
        }

        public void RequestQuit()
        {
            if (_state.IsOperationInProgress)
            {
                _state.CurrentMode = InputMode.QuitConfirm;
                _state.PromptText = "[bold yellow]A file operation is in progress. Quit and cancel? (y/n)[/]";
                fileManager.SetNeedsRedraw();
            }
            else
            {
                fileManager.Quit();
            }
        }

        public void BeginSortMenu()
        {
            _state.CurrentMode = InputMode.SortMenu;
            _state.SortMenuSelectedIndex = 0;
            fileManager.SetNeedsRedraw();
        }

        public void ApplySort()
        {
            if (_state.CurrentMode != InputMode.SortMenu) return;

            var selected = fileManager.SortOptions[_state.SortMenuSelectedIndex];
            _state.SortBy = selected.By;
            _state.SortDirection = selected.Dir;
            _state.GroupDirectories = selected.Group;

            fileManager.ResetToNormalMode();
            fileManager.RefreshDirectory(setInitialSelection: true);
        }

        private List<FileSystemItem> GetSelectedItems()
        {
            var selectedItems = new List<FileSystemItem>();
            if (_state.CurrentMode == InputMode.Visual && _state.VisuallySelectedItems.Any())
            {
                var selectedPaths = _state.VisuallySelectedItems.ToHashSet();
                selectedItems.AddRange(_state.CurrentItems.Where(i => selectedPaths.Contains(i.Path)));
            }
            else if (_state.SelectedIndex >= 0 && _state.SelectedIndex < _state.CurrentItems.Count)
            {
                var item = _state.CurrentItems[_state.SelectedIndex];
                if (!item.IsParentDirectory)
                {
                    selectedItems.Add(item);
                }
            }

            return selectedItems;
        }

        #endregion
    }
}