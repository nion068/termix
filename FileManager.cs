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
        FilteredNavigation
    }

    private readonly DoubleBufferedRenderer _doubleBuffer = new();

    private readonly FilePreviewService _filePreviewService = new();
    private readonly InputHandler _inputHandler;
    private readonly FileManagerRenderer _renderer;

    private string _addBasePath = "";
    private List<FileSystemItem> _currentItems = [];

    private string _currentPath = Directory.GetCurrentDirectory();
    private IRenderable _currentPreview = new Text("");
    private CancellationTokenSource _debounceCts = new();
    private string _inputText = "";
    private bool _isDeepSearchRunning;
    private int _previewHorizontalOffset;
    private int _previewVerticalOffset;

    private string _promptText = "";
    private List<FileSystemItem>? _recursiveSearchCache;

    private (string Path, string Filter, List<FileSystemItem> Items, int SelectedIndex)? _savedFilterState;
    private int _selectedIndex;
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

        while (true)
        {
            var layout = _renderer.GetLayout(_currentPath, _currentItems, _selectedIndex, _currentPreview, _viewOffset,
                GetFooterContent());
            _doubleBuffer.Render(layout);

            if (!_inputHandler.ProcessKey(Console.ReadKey(true))) break;
        }
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
                var input = _inputText ?? string.Empty;
                return
                    $"[grey]Results for '[yellow]{input.EscapeMarkup()}[/]'. Press [cyan]Esc[/] to clear, or [cyan]S[/] for new search.[/]";
            }
        }

        var safeInput = _inputText ?? string.Empty;
        var searchIndicator = _isDeepSearchRunning ? "[grey](Searching...)[/]" : "";
        return CurrentMode switch
        {
            InputMode.Filter =>
                $"{_promptText.EscapeMarkup()}{searchIndicator} [yellow]{safeInput.EscapeMarkup()}[/][grey]█[/] | [grey]Press[/] [cyan]Esc[/] [grey]to navigate results[/]",
            InputMode.Add or InputMode.Rename =>
                $"{_promptText.EscapeMarkup()}[yellow]{safeInput.EscapeMarkup()}[/][grey]█[/]",
            InputMode.DeleteConfirm => _promptText,
            _ => null
        };
    }

    #region Public Methods for InputHandler

    public string GetInputText(int? sliceEnd = null)
    {
        return sliceEnd.HasValue ? _inputText[..^1] : _inputText;
    }

    public void ClearStatusMessage()
    {
        _statusMessage = null;
    }

    public void AppendInputText(char c)
    {
        _inputText += c;
    }

    public void HandleBackspace()
    {
        if (_inputText.Length > 0) _inputText = _inputText[..^1];
    }

    public void ResetToNormalMode()
    {
        _debounceCts.Cancel();
        _isDeepSearchRunning = false;
        CurrentMode = InputMode.Normal;
        _inputText = "";
        _promptText = "";
    }

    public void AcceptFilter()
    {
        CurrentMode = InputMode.Normal;
        _promptText = "";
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
    }

    public void BeginRename()
    {
        if (_currentItems.Count == 0 || GetSelectedItem().IsParentDirectory) return;
        CurrentMode = InputMode.Rename;
        _promptText = "Rename: ";
        _inputText = GetSelectedItem().Name;
    }

    public void BeginDelete()
    {
        if (_currentItems.Count == 0 || GetSelectedItem().IsParentDirectory) return;
        CurrentMode = InputMode.DeleteConfirm;
        _promptText = $"Delete '{GetSelectedItem().Name.EscapeMarkup()}'? [bold green]y[/]/[bold red]n[/]";
    }

    public void BeginFilter()
    {
        CurrentMode = InputMode.Filter;
        _promptText = "Search: ";
        _inputText = "";
        _recursiveSearchCache = null;
    }

    public void UpdateFilter(string newFilterText)
    {
        _inputText = newFilterText;
        _debounceCts.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        if (_recursiveSearchCache == null && !_isDeepSearchRunning && !string.IsNullOrEmpty(_inputText))
        {
            _isDeepSearchRunning = true;
            ActionService.GetDeepDirectoryContentsAsync(_currentPath, token).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    _recursiveSearchCache = task.Result;
                    if (!token.IsCancellationRequested && _inputText.Length > 0) ApplyFilter();
                }

                _isDeepSearchRunning = false;
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
    }

    public void OpenSelectedItem()
    {
        if (_currentItems.Count == 0) return;
        var selectedItem = GetSelectedItem();

        if (selectedItem.IsDirectory)
        {
            if (IsViewFiltered && CurrentMode != InputMode.FilteredNavigation)
            {
                _savedFilterState = (_currentPath, _inputText, new List<FileSystemItem>(_currentItems), _selectedIndex);
                CurrentMode = InputMode.FilteredNavigation;
            }

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


    private void NavigateToDirectory(string path)
    {
        if (CurrentMode != InputMode.FilteredNavigation) ResetToNormalMode();

        try
        {
            _currentPath = Path.GetFullPath(path);
            RefreshDirectory(setInitialSelection: true);
        }
        catch (Exception ex)
        {
            _statusMessage = $"[red]Navigation failed: {ex.Message.EscapeMarkup()}[/]";
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
        if (parent != null) NavigateToDirectory(parent.FullName);
    }

    private void LoadCurrentDirectory()
    {
        try
        {
            _unfilteredItems = FileSystemService.GetDirectoryContents(_currentPath);
            if (CurrentMode != InputMode.Filter) _currentItems = new List<FileSystemItem>(_unfilteredItems);
        }
        catch (Exception ex)
        {
            _statusMessage = $"[red]Error loading directory: {ex.Message.EscapeMarkup()}[/]";
            _currentItems = [];
            _unfilteredItems = [];
            _selectedIndex = -1;
        }
    }

    #endregion
}