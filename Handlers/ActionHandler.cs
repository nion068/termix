using Spectre.Console;
using termix.models;
using termix.Services;

namespace termix.Handlers;

public class ActionHandler(FileManager fileManager)
{
    private readonly FileManagerState _state = fileManager.State;

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
        if (_state.CurrentItems.Count == 0 || _state.CurrentItems[_state.SelectedIndex].IsParentDirectory) return;
        _state.CurrentMode = InputMode.DeleteConfirm;
        _state.PromptText =
            $"Delete '{_state.CurrentItems[_state.SelectedIndex].Name.EscapeMarkup()}'? [bold green]y[/]/[bold red]n[/]";
        fileManager.SetNeedsRedraw();
    }

    public void BeginCopy()
    {
        if (_state.CurrentItems.Count == 0 || _state.CurrentItems[_state.SelectedIndex].IsParentDirectory) return;
        var item = _state.CurrentItems[_state.SelectedIndex];
        _state.Clipboard = new ClipboardItem(item, ClipboardMode.Copy);
        _state.StatusMessage = $"[yellow]{item.Name.EscapeMarkup()}[/] copied to clipboard.";
        fileManager.SetNeedsRedraw();
    }

    public void BeginMove()
    {
        if (_state.CurrentItems.Count == 0 || _state.CurrentItems[_state.SelectedIndex].IsParentDirectory) return;
        var item = _state.CurrentItems[_state.SelectedIndex];
        _state.Clipboard = new ClipboardItem(item, ClipboardMode.Move);
        _state.StatusMessage = $"[yellow]{item.Name.EscapeMarkup()}[/] marked for move.";
        fileManager.SetNeedsRedraw();
    }

    public void ClearClipboard()
    {
        _state.Clipboard = null;
        _state.StatusMessage = "[grey]Clipboard cleared.[/]";
        fileManager.SetNeedsRedraw();
    }

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

        var sourcePath = _state.Clipboard.Item.Path;
        var destPath = Path.Combine(destinationBasePath, _state.Clipboard.Item.Name);

        if (sourcePath.Equals(destPath, StringComparison.OrdinalIgnoreCase) ||
            (Directory.GetParent(sourcePath)?.FullName
                 .Equals(destinationBasePath, StringComparison.OrdinalIgnoreCase) == true &&
             _state.Clipboard.Mode == ClipboardMode.Move))
        {
            _state.StatusMessage = "[yellow]Source and destination are the same.[/]";
            if (_state.Clipboard.Mode == ClipboardMode.Move) ClearClipboard();
            fileManager.SetNeedsRedraw();
            return;
        }

        if (File.Exists(destPath) || Directory.Exists(destPath))
        {
            _state.StatusMessage =
                $"[red]An item named '{_state.Clipboard.Item.Name.EscapeMarkup()}' already exists here.[/]";
            fileManager.SetNeedsRedraw();
            return;
        }

        var clipboardItem = _state.Clipboard;
        ClearClipboard();

        _state.IsOperationInProgress = true;
        _state.OperationCts = new CancellationTokenSource();
        var token = _state.OperationCts.Token;

        var progress = new Progress<(long totalBytes, long completedBytes, string currentFile)>(value =>
        {
            if (!string.IsNullOrEmpty(value.currentFile))
            {
                _state.ProgressTaskDescription = value.currentFile;
            }

            _state.ProgressValue = value.totalBytes > 0 ? (double)value.completedBytes / value.totalBytes * 100 : 0;
            fileManager.SetNeedsRedraw();
        });

        Task.Run(async () =>
        {
            ActionResponse response;
            try
            {
                response = clipboardItem.Mode == ClipboardMode.Copy
                    ? await ActionService.CopyAsync(sourcePath, destPath, progress, token)
                    : await ActionService.MoveAsync(sourcePath, destPath, progress, token);
            }
            catch (OperationCanceledException)
            {
                response = new ActionResponse(false, "[yellow]Operation was cancelled by user.[/]");
            }
            catch (Exception ex)
            {
                response = new ActionResponse(false,
                    $"[red]An unexpected error occurred: {ex.Message.EscapeMarkup()}[/]");
            }

            _state.StatusMessage = response.Message;
        }, token).ContinueWith(t =>
        {
            _state.IsOperationInProgress = false;
            _state.OperationCts?.Dispose();
            _state.OperationCts = null;
            _state.ProgressTaskDescription = null;
            _state.ProgressValue = 0;
            fileManager.RefreshDirectory();
        }, CancellationToken.None);
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

    public void CommitDelete()
    {
        var response = ActionService.Delete(_state.CurrentItems[_state.SelectedIndex]);
        _state.StatusMessage = response.Message;
        fileManager.ResetToNormalMode();
        if (response.Success) fileManager.RefreshDirectory(preserveSelection: true);
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
}