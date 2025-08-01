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
        Search
    }

    public InputMode CurrentMode { get; private set; } = InputMode.Normal;
    public bool IsShowingSearchResults { get; private set; }

    private string _promptText = "";
    private string _inputText = "";
    private string? _statusMessage;

    private List<FileSystemItem> _fullSearchResults = [];
    private CancellationTokenSource _debounceCts = new();

    private readonly FilePreviewService _filePreviewService = new();
    private readonly FileManagerRenderer _renderer;
    private readonly DoubleBufferedRenderer _doubleBuffer = new();
    private readonly InputHandler _inputHandler;

    private string _currentPath = Directory.GetCurrentDirectory();
    private int _selectedIndex;
    private List<FileSystemItem> _currentItems = [];
    private IRenderable _currentPreview = new Text("");
    private int _viewOffset;
    private int _previewVerticalOffset;
    private int _previewHorizontalOffset;

    public FileManager(bool useIcons)
    {
        var iconProvider = new IconProvider(useIcons);
        _renderer = new FileManagerRenderer(iconProvider);
        _inputHandler = new InputHandler(this);
    }

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
        return CurrentMode switch
        {
            InputMode.Add or InputMode.Rename or InputMode.Search =>
                $"{_promptText.EscapeMarkup()}[yellow]{_inputText.EscapeMarkup()}[/][grey]█[/]",
            InputMode.DeleteConfirm => _promptText,
            _ => null
        };
    }

    #region Public Methods for InputHandler

    public string GetInputText(int? sliceEnd = null) => sliceEnd.HasValue ? _inputText[..^1] : _inputText;
    public void ClearStatusMessage() => _statusMessage = null;
    public void AppendInputText(char c) => _inputText += c;

    public void HandleBackspace()
    {
        if (_inputText.Length > 0) _inputText = _inputText[..^1];
    }

    public void ResetToNormalMode()
    {
        CurrentMode = InputMode.Normal;
        _inputText = "";
        _promptText = "";
    }

    public void BeginAdd()
    {
        CurrentMode = InputMode.Add;
        _promptText = $"Create in [{GetDisplayFolderName(GetSelectedItem())}]: ";
        _inputText = "";
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

    public void BeginSearch()
    {
        CurrentMode = InputMode.Search;
        _promptText = "Filter: ";
        _inputText = "";
        _fullSearchResults.Clear();
        LoadCurrentDirectory();
    }

    public void ExitSearch()
    {
        IsShowingSearchResults = false;
        _statusMessage = "Exited search.";
        RefreshDirectory(setInitialSelection: true);
        ResetToNormalMode();
    }

    public void FinalizeSearch()
    {
        IsShowingSearchResults = true;
        _statusMessage = $"Showing {_currentItems.Count} results. Press Esc to exit.";
        ResetToNormalMode();
    }

    public void UpdateSearchQuery(string newQuery)
    {
        _inputText = newQuery;
        _debounceCts.Cancel();
        _debounceCts = new CancellationTokenSource();

        Task.Run(async () =>
        {
            await Task.Delay(200, _debounceCts.Token);
            if (_debounceCts.IsCancellationRequested) return;

            if (_fullSearchResults.Count == 0 && !string.IsNullOrEmpty(_inputText))
            {
                var response = ActionService.Search(_currentPath, _inputText);
                if (response is { Success: true, Payload: List<FileSystemItem> results })
                {
                    _fullSearchResults = results;
                }
            }

            if (string.IsNullOrEmpty(_inputText))
            {
                _currentItems = _fullSearchResults;
            }
            else
            {
                _currentItems = _fullSearchResults
                    .Where(item => item.Name.Contains(_inputText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            _selectedIndex = _currentItems.Count != 0 ? 0 : -1;
            AdjustViewPort();
            UpdatePreview();
        }, _debounceCts.Token);
    }

    public void CommitStandardTextInput()
    {
        var response = CurrentMode == InputMode.Add
            ? ActionService.Create(_currentPath, _inputText)
            : ActionService.Rename(_currentPath, GetSelectedItem().Name, _inputText);

        _statusMessage = response.Message;
        if (response.Success) RefreshDirectory(findAndSelect: (string?)response.Payload);
        ResetToNormalMode();
    }

    public void CommitDelete()
    {
        var response = ActionService.Delete(GetSelectedItem());
        _statusMessage = response.Message;
        if (response.Success) RefreshDirectory(preserveSelection: true);
        ResetToNormalMode();
    }

    #endregion

    #region UI & State Updates

    private FileSystemItem GetSelectedItem() => _currentItems[_selectedIndex];

    private void RefreshDirectory(string? findAndSelect = null, bool preserveSelection = false,
        bool setInitialSelection = false)
    {
        var oldSelectedIndex = _selectedIndex;
        LoadCurrentDirectory();

        if (findAndSelect != null)
        {
            _selectedIndex =
                _currentItems.FindIndex(item => item.Name.Equals(findAndSelect, StringComparison.OrdinalIgnoreCase));
        }
        else if (preserveSelection)
        {
            _selectedIndex = Math.Clamp(oldSelectedIndex, 0, _currentItems.Count - 1);
        }
        else if (setInitialSelection)
        {
            _selectedIndex = _currentItems.Count != 0 ? 0 : -1;
        }

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
        UpdatePreview(resetScroll: false);
    }

    private void UpdatePreview(bool resetScroll = true)
    {
        if (resetScroll)
        {
            _previewVerticalOffset = 0;
            _previewHorizontalOffset = 0;
        }

        var selectedItem = (_selectedIndex >= 0 && _selectedIndex < _currentItems.Count) ? GetSelectedItem() : null;
        _currentPreview = (selectedItem == null || selectedItem.IsDirectory)
            ? _filePreviewService.GetPreview(null, 0, 0)
            : _filePreviewService.GetPreview(selectedItem.Path, _previewVerticalOffset, _previewHorizontalOffset);
    }

    public void OpenSelectedItem()
    {
        if (_currentItems.Count == 0) return;
        var selectedItem = GetSelectedItem();
        if (selectedItem.IsDirectory)
        {
            NavigateToDirectory(selectedItem.Path);
        }
        else
        {
            FileSystemService.OpenFile(selectedItem.Path);
        }
    }

    private void NavigateToDirectory(string path)
    {
        IsShowingSearchResults = false;
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
        if (IsShowingSearchResults) return;

        var parent = Directory.GetParent(_currentPath);
        if (parent != null) NavigateToDirectory(parent.FullName);
    }

    private void LoadCurrentDirectory()
    {
        try
        {
            if (!IsShowingSearchResults)
            {
                _currentItems = FileSystemService.GetDirectoryContents(_currentPath);
            }
        }
        catch (Exception ex)
        {
            _statusMessage = $"[red]Error loading directory: {ex.Message.EscapeMarkup()}[/]";
            _currentItems = [];
            _selectedIndex = -1;
        }
    }

    private string GetDisplayFolderName(FileSystemItem item)
    {
        return item.IsDirectory
            ? item.Name
            : Path.GetFileName(Path.GetDirectoryName(item.Path) ?? _currentPath);
    }

    #endregion
}