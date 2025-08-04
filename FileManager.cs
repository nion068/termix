using Spectre.Console;
using Spectre.Console.Rendering;
using termix.models;
using termix.Services;
using termix.UI;

namespace termix;

public class FileManager
{
    public enum InputMode
    {
        Normal,
        Add,
        Rename,
        DeleteConfirm,
        Filter,
        FilteredNavigation,
        QuitConfirm
    }

    private readonly DoubleBufferedRenderer _doubleBuffer = new();
    private readonly FilePreviewService _filePreviewService = new();
    private readonly InputHandler _inputHandler;
    private readonly Stack<string> _navigationStack = new();
    private readonly FileManagerRenderer _renderer;

    private string _addBasePath = "";
    private ClipboardItem? _clipboard;
    private List<FileSystemItem> _currentItems = [];

    private string _currentPath = Directory.GetCurrentDirectory();
    private IRenderable _currentPreview = new Text("");
    private CancellationTokenSource _debounceCts = new();
    private string _inputText = "";
    private bool _isDeepSearchRunning;
    private bool _isOperationInProgress;
    private bool _needsRedraw = true;
    private CancellationTokenSource? _operationCts;
    private int _previewHorizontalOffset;
    private int _previewVerticalOffset;

    private string _promptText = "";
    private List<FileSystemItem>? _recursiveSearchCache;

    private (string Path, string Filter, List<FileSystemItem> Items, int SelectedIndex)? _savedFilterState;
    private int _selectedIndex;
    private bool _shouldQuit;
    private string? _statusMessage;
    private List<FileSystemItem> _unfilteredItems = [];
    private int _viewOffset;

    public FileManager(bool useIcons)
    {
        var iconProvider = new IconProvider(useIcons);
        _renderer = new FileManagerRenderer(iconProvider);
        _inputHandler = new InputHandler(this);
    }

    public InputMode CurrentMode { get; private set; } = InputMode.Normal;
    public bool IsViewFiltered => !string.IsNullOrEmpty(_inputText);

    public void Run()
    {
        AnsiConsole.Clear();
        RefreshDirectory(setInitialSelection: true);

        while (!_shouldQuit)
        {
            if (_needsRedraw)
            {
                _needsRedraw = false;
                if (!_isOperationInProgress)
                {
                    var layout = _renderer.GetLayout(_currentPath, _currentItems, _selectedIndex, _currentPreview,
                        _viewOffset, GetFooterContent(), _clipboard);
                    _doubleBuffer.Render(layout);
                }
            }

            while (!Console.KeyAvailable && !_needsRedraw && !_shouldQuit) Thread.Sleep(50);

            if (_shouldQuit) break;

            if (Console.KeyAvailable) _inputHandler.ProcessKey(Console.ReadKey(true));
        }
    }

    private void SetNeedsRedraw()
    {
        _needsRedraw = true;
    }

    private string? GetFooterContent()
    {
        if (_statusMessage != null) return _statusMessage;

        switch (CurrentMode)
        {
            case InputMode.FilteredNavigation:
                return
                    "[grey]Use[/] [cyan]B[/] [grey]to return to search results[/] | [grey]Currently browsing from a search result.[/]";
            case InputMode.Normal when IsViewFiltered:
            {
                var input = _inputText;
                return
                    $"[grey]Results for '[yellow]{input.EscapeMarkup()}[/]'. Press [cyan]Esc[/] to clear, or [cyan]S[/] for new search.[/]";
            }
        }

        var safeInput = _inputText;
        var searchIndicator = _isDeepSearchRunning ? "[grey](Searching...)[/]" : "";
        return CurrentMode switch
        {
            InputMode.Filter =>
                $"{_promptText.EscapeMarkup()}{searchIndicator} [yellow]{safeInput.EscapeMarkup()}[/][grey]█[/] | [grey]Press[/] [cyan]Esc[/] [grey]to navigate results[/]",
            InputMode.Add or InputMode.Rename =>
                $"{_promptText.EscapeMarkup()}[yellow]{safeInput.EscapeMarkup()}[/][grey]█[/]",
            InputMode.DeleteConfirm or InputMode.QuitConfirm => _promptText,
            _ => null
        };
    }

    #region Public Methods for InputHandler

    public bool HasClipboardItem()
    {
        return _clipboard != null;
    }

    public void ClearClipboard()
    {
        _clipboard = null;
        _statusMessage = "[grey]Clipboard cleared.[/]";
        SetNeedsRedraw();
    }

    public string GetInputText(int? sliceEnd = null)
    {
        return sliceEnd.HasValue ? _inputText[..^1] : _inputText;
    }

    public void ClearStatusMessage()
    {
        if (_statusMessage != null)
        {
            _statusMessage = null;
            SetNeedsRedraw();
        }
    }

    public void AppendInputText(char c)
    {
        _inputText += c;
        SetNeedsRedraw();
    }

    public void HandleBackspace()
    {
        if (_inputText.Length > 0)
        {
            _inputText = _inputText[..^1];
            SetNeedsRedraw();
        }
    }

    public void ResetToNormalMode()
    {
        _debounceCts.Cancel();
        _isDeepSearchRunning = false;
        CurrentMode = InputMode.Normal;
        _inputText = "";
        _promptText = "";
        SetNeedsRedraw();
    }

    public void AcceptFilter()
    {
        CurrentMode = InputMode.Normal;
        _promptText = "";
        SetNeedsRedraw();
    }

    public void BeginAdd()
    {
        CurrentMode = InputMode.Add;
        _inputText = "";
        var selectedItem = _selectedIndex >= 0 && _selectedIndex < _currentItems.Count
            ? GetSelectedItem()
            : null;

        if (selectedItem is { IsDirectory: true, IsParentDirectory: false })
        {
            _addBasePath = selectedItem.Path;
            _promptText = $"Create in [{selectedItem.Name.EscapeMarkup()}]: ";
        }
        else
        {
            _addBasePath = _currentPath;
            var currentFolderName = Path.GetFileName(Path.GetFullPath(_currentPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.IsNullOrEmpty(currentFolderName)) currentFolderName = _currentPath;
            _promptText = $"Create in [{currentFolderName.EscapeMarkup()}]: ";
        }

        SetNeedsRedraw();
    }

    public void BeginRename()
    {
        if (_currentItems.Count == 0 || GetSelectedItem().IsParentDirectory) return;
        CurrentMode = InputMode.Rename;
        _promptText = "Rename: ";
        _inputText = GetSelectedItem().Name;
        SetNeedsRedraw();
    }

    public void BeginDelete()
    {
        if (_currentItems.Count == 0 || GetSelectedItem().IsParentDirectory) return;
        CurrentMode = InputMode.DeleteConfirm;
        _promptText = $"Delete '{GetSelectedItem().Name.EscapeMarkup()}'? [bold green]y[/]/[bold red]n[/]";
        SetNeedsRedraw();
    }

    public void BeginCopy()
    {
        if (_currentItems.Count == 0 || GetSelectedItem().IsParentDirectory) return;
        var item = GetSelectedItem();
        _clipboard = new ClipboardItem(item, ClipboardMode.Copy);
        _statusMessage = $"[yellow]{item.Name.EscapeMarkup()}[/] copied to clipboard.";
        SetNeedsRedraw();
    }

    public void BeginMove()
    {
        if (_currentItems.Count == 0 || GetSelectedItem().IsParentDirectory) return;
        var item = GetSelectedItem();
        _clipboard = new ClipboardItem(item, ClipboardMode.Move);
        _statusMessage = $"[yellow]{item.Name.EscapeMarkup()}[/] marked for move.";
        SetNeedsRedraw();
    }

    public void BeginPaste()
    {
        if (_clipboard == null)
        {
            _statusMessage = "[red]Clipboard is empty.[/]";
            SetNeedsRedraw();
            return;
        }

        var destinationBasePath = _currentPath;
        var selectedItem = _currentItems.Count > 0 && _selectedIndex >= 0 ? GetSelectedItem() : null;

        if (selectedItem is { IsDirectory: true, IsParentDirectory: false }) destinationBasePath = selectedItem.Path;

        var sourcePath = _clipboard.Item.Path;
        var destPath = Path.Combine(destinationBasePath, _clipboard.Item.Name);

        if (sourcePath.Equals(destPath, StringComparison.OrdinalIgnoreCase) ||
            (Directory.GetParent(sourcePath)?.FullName
                 .Equals(destinationBasePath, StringComparison.OrdinalIgnoreCase) == true &&
             _clipboard.Mode == ClipboardMode.Move))
        {
            _statusMessage = "[yellow]Source and destination are the same.[/]";
            if (_clipboard.Mode == ClipboardMode.Move) ClearClipboard();
            SetNeedsRedraw();
            return;
        }

        if (File.Exists(destPath) || Directory.Exists(destPath))
        {
            _statusMessage = $"[red]An item named '{_clipboard.Item.Name.EscapeMarkup()}' already exists here.[/]";
            SetNeedsRedraw();
            return;
        }

        var clipboardItem = _clipboard;
        ClearClipboard();

        _isOperationInProgress = true;
        _operationCts = new CancellationTokenSource();
        var token = _operationCts.Token;

        AnsiConsole.Progress()
            .AutoClear(true)
            .Columns(new TaskDescriptionColumn(), new ProgressBarColumn(), new PercentageColumn(), new SpinnerColumn())
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask($"Preparing to {clipboardItem.Mode.ToString().ToLower()}...");
                ActionResponse response;

                try
                {
                    if (clipboardItem.Mode == ClipboardMode.Copy)
                        response = await ActionService.CopyAsync(sourcePath, destPath, task, token);
                    else
                        response = await ActionService.MoveAsync(sourcePath, destPath, task, token);
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

                _statusMessage = response.Message;
            })
            .ContinueWith(_ =>
            {
                _isOperationInProgress = false;
                _operationCts?.Dispose();
                _operationCts = null;
                RefreshDirectory();
            }, token);
    }

    public void RequestQuit()
    {
        if (_isOperationInProgress)
        {
            CurrentMode = InputMode.QuitConfirm;
            _promptText = "[bold yellow]A file operation is in progress. Quit and cancel? (y/n)[/]";
            SetNeedsRedraw();
        }
        else
        {
            _shouldQuit = true;
        }
    }

    public void Quit(bool force = false)
    {
        if (force && _isOperationInProgress) _operationCts?.Cancel();
        _shouldQuit = true;
    }

    public void BeginFilter()
    {
        CurrentMode = InputMode.Filter;
        _promptText = "Search: ";
        _inputText = "";
        _recursiveSearchCache = null;
        SetNeedsRedraw();
    }

    public void UpdateFilter(string newFilterText)
    {
        _inputText = newFilterText;
        SetNeedsRedraw();
        _debounceCts.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        if (_recursiveSearchCache == null && !_isDeepSearchRunning && !string.IsNullOrEmpty(_inputText))
        {
            _isDeepSearchRunning = true;
            SetNeedsRedraw();
            ActionService.GetDeepDirectoryContentsAsync(_currentPath, token).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    _recursiveSearchCache = task.Result;
                    if (!token.IsCancellationRequested && _inputText.Length > 0) ApplyFilter();
                }

                _isDeepSearchRunning = false;
                SetNeedsRedraw();
            }, token);
        }

        ApplyFilter();
    }

    public void ClearFilter()
    {
        _inputText = "";
        _recursiveSearchCache = null;
        _currentItems = new List<FileSystemItem>(_unfilteredItems);
        ResetToNormalMode();
        RefreshViewAfterFilter();
    }

    public void CommitStandardTextInput()
    {
        var response = CurrentMode == InputMode.Add
            ? ActionService.Create(_addBasePath, _inputText)
            : ActionService.Rename(_currentPath, GetSelectedItem().Name, _inputText);

        _statusMessage = response.Message;
        ResetToNormalMode();
        if (response.Success) RefreshDirectory((string?)response.Payload);
    }

    public void CommitDelete()
    {
        var response = ActionService.Delete(GetSelectedItem());
        _statusMessage = response.Message;
        ResetToNormalMode();
        if (response.Success) RefreshDirectory(preserveSelection: true);
    }

    #endregion

    #region UI & State Updates

    private void ApplyFilter()
    {
        var sourceList = _recursiveSearchCache ?? _unfilteredItems;

        _currentItems = string.IsNullOrEmpty(_inputText)
            ? [.._unfilteredItems]
            : sourceList
                .Where(item => item.Name.Contains(_inputText, StringComparison.OrdinalIgnoreCase))
                .ToList();
        RefreshViewAfterFilter();
    }

    private void RefreshViewAfterFilter()
    {
        _selectedIndex = _currentItems.Count != 0 ? 0 : -1;
        AdjustViewPort();
        UpdatePreview();
    }

    private FileSystemItem GetSelectedItem()
    {
        return _currentItems[_selectedIndex];
    }

    private void RefreshDirectory(string? findAndSelect = null, bool preserveSelection = false,
        bool setInitialSelection = false)
    {
        var oldSelectedIndex = _selectedIndex;
        LoadCurrentDirectory();

        if (findAndSelect != null)
            _selectedIndex =
                _currentItems.FindIndex(item => item.Name.Equals(findAndSelect, StringComparison.OrdinalIgnoreCase));
        else if (preserveSelection)
            _selectedIndex = Math.Clamp(oldSelectedIndex, 0, _currentItems.Count - 1);
        else if (setInitialSelection) _selectedIndex = _currentItems.Count != 0 ? 0 : -1;

        if (_selectedIndex == -1 && _currentItems.Count != 0) _selectedIndex = 0;

        AdjustViewPort();
        UpdatePreview();
    }

    private void AdjustViewPort()
    {
        var pageSize = Console.WindowHeight - 12;
        pageSize = Math.Max(5, pageSize);
        _viewOffset = _selectedIndex < _viewOffset ? _selectedIndex :
            _selectedIndex >= _viewOffset + pageSize ? _selectedIndex - pageSize + 1 : _viewOffset;
        _viewOffset = Math.Clamp(_viewOffset, 0, Math.Max(0, _currentItems.Count - pageSize));
        SetNeedsRedraw();
    }

    public void MoveSelection(int direction)
    {
        if (_currentItems.Count == 0) return;
        var newIndex = Math.Clamp(_selectedIndex + direction, 0, _currentItems.Count - 1);
        if (newIndex == _selectedIndex) return;
        _selectedIndex = newIndex;
        AdjustViewPort();
        UpdatePreview();
    }

    public void MoveSelectionToEdge(bool toStart)
    {
        _selectedIndex = toStart ? 0 : Math.Max(0, _currentItems.Count - 1);
        AdjustViewPort();
        UpdatePreview();
    }

    public void ScrollPreview(int vertical, int horizontal)
    {
        _previewVerticalOffset = Math.Max(0, _previewVerticalOffset + vertical);
        _previewHorizontalOffset = Math.Max(0, _previewHorizontalOffset + horizontal);
        UpdatePreview(false);
    }

    private void UpdatePreview(bool resetScroll = true)
    {
        if (resetScroll)
        {
            _previewVerticalOffset = 0;
            _previewHorizontalOffset = 0;
        }

        var selectedItem = _selectedIndex >= 0 && _selectedIndex < _currentItems.Count ? GetSelectedItem() : null;
        _currentPreview = selectedItem == null || selectedItem.IsDirectory
            ? _filePreviewService.GetPreview(null, 0, 0)
            : _filePreviewService.GetPreview(selectedItem.Path, _previewVerticalOffset, _previewHorizontalOffset);
        SetNeedsRedraw();
    }

    public void OpenSelectedItem()
    {
        if (_currentItems.Count == 0) return;
        var selectedItem = GetSelectedItem();

        if (selectedItem.IsDirectory)
        {
            if (IsViewFiltered && CurrentMode != InputMode.FilteredNavigation)
            {
                _savedFilterState = (_currentPath, _inputText, [.._currentItems], _selectedIndex);
                CurrentMode = InputMode.FilteredNavigation;
                SetNeedsRedraw();
            }

            _navigationStack.Push(selectedItem.Name);
            NavigateToDirectory(selectedItem.Path);
        }
        else
        {
            FileSystemService.OpenFile(selectedItem.Path);
        }
    }

    public void ReturnToFilter()
    {
        if (!_savedFilterState.HasValue) return;

        var state = _savedFilterState.Value;
        _currentPath = state.Path;
        _inputText = state.Filter;
        _currentItems = state.Items;
        _selectedIndex = state.SelectedIndex;
        _savedFilterState = null;
        CurrentMode = InputMode.Filter;

        AdjustViewPort();
        UpdatePreview();
    }

    private void NavigateToDirectory(string path, string? findAndSelect = null)
    {
        if (CurrentMode != InputMode.FilteredNavigation) ResetToNormalMode();

        try
        {
            _currentPath = Path.GetFullPath(path);
            RefreshDirectory(findAndSelect, setInitialSelection: true);
        }
        catch (Exception ex)
        {
            _statusMessage = $"[red]Navigation failed: {ex.Message.EscapeMarkup()}[/]";
            SetNeedsRedraw();
        }
    }

    public void NavigateUp()
    {
        if (CurrentMode is not (InputMode.Normal or InputMode.FilteredNavigation)) return;

        if (IsViewFiltered)
        {
            ClearFilter();
            return;
        }

        var parent = Directory.GetParent(_currentPath);

        if (parent != null)
            NavigateToDirectory(parent.FullName, _navigationStack.TryPop(out var result) ? result : null);
    }

    private void LoadCurrentDirectory()
    {
        try
        {
            _unfilteredItems = FileSystemService.GetDirectoryContents(_currentPath);
            if (CurrentMode != InputMode.Filter) _currentItems = [.._unfilteredItems];
        }
        catch (Exception ex)
        {
            _statusMessage = $"[red]Error loading directory: {ex.Message.EscapeMarkup()}[/]";
            _currentItems = [];
            _unfilteredItems = [];
            _selectedIndex = -1;
        }

        SetNeedsRedraw();
    }

    #endregion
}