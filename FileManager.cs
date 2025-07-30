using Spectre.Console;
using Spectre.Console.Rendering;
using termix.models;
using termix.Services;
using termix.UI;

namespace termix;

public class FileManager
{
    private readonly FilePreviewService _filePreviewService = new();
    private readonly FileManagerRenderer _renderer;
    private readonly DoubleBufferedRenderer _doubleBuffer = new();

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
    }

    public void Run()
    {
        AnsiConsole.Clear();
        LoadCurrentDirectory();
        UpdatePreview();
        while (true)
        {
            var layout = _renderer.GetLayout(_currentPath, _currentItems, _selectedIndex, _currentPreview, _viewOffset);
            _doubleBuffer.Render(layout);

            var keyInfo = Console.ReadKey(true);
            if (!HandleKeyPress(keyInfo)) break;
        }
    }

    private bool HandleKeyPress(ConsoleKeyInfo keyInfo)
    {
        var selectionChanged = false;
        var previewNeedsUpdate = false;

        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow when keyInfo.Modifiers == ConsoleModifiers.Alt:
            case ConsoleKey.K when keyInfo.Modifiers == ConsoleModifiers.Alt:
                _previewVerticalOffset = Math.Max(0, _previewVerticalOffset - 1);
                previewNeedsUpdate = true;
                break;
            case ConsoleKey.DownArrow when keyInfo.Modifiers == ConsoleModifiers.Alt:
            case ConsoleKey.J when keyInfo.Modifiers == ConsoleModifiers.Alt:
                _previewVerticalOffset += 1;
                previewNeedsUpdate = true;
                break;
            case ConsoleKey.LeftArrow when keyInfo.Modifiers == ConsoleModifiers.Alt:
            case ConsoleKey.H when keyInfo.Modifiers == ConsoleModifiers.Alt:
                _previewHorizontalOffset = Math.Max(0, _previewHorizontalOffset - 5);
                previewNeedsUpdate = true;
                break;
            case ConsoleKey.RightArrow when keyInfo.Modifiers == ConsoleModifiers.Alt:
            case ConsoleKey.L when keyInfo.Modifiers == ConsoleModifiers.Alt:
                _previewHorizontalOffset += 5;
                previewNeedsUpdate = true;
                break;
            case ConsoleKey.DownArrow or ConsoleKey.J:
                selectionChanged = MoveSelection(1);
                break;
            case ConsoleKey.UpArrow or ConsoleKey.K:
                selectionChanged = MoveSelection(-1);
                break;
            case ConsoleKey.Enter:
                OpenSelectedItem();
                break;
            case ConsoleKey.Backspace:
                NavigateUp();
                break;
            case ConsoleKey.Q or ConsoleKey.Escape:
                return false;
            case ConsoleKey.Home:
                _selectedIndex = 0;
                selectionChanged = true;
                break;
            case ConsoleKey.End:
                _selectedIndex = Math.Max(0, _currentItems.Count - 1);
                selectionChanged = true;
                break;
        }

        if (selectionChanged)
        {
            AdjustViewPort();
            UpdatePreview();
        }
        else if (previewNeedsUpdate)
        {
            UpdatePreview(resetScroll: false);
        }

        return true;
    }

    private void AdjustViewPort()
    {
        int pageSize = Console.WindowHeight - 12;
        pageSize = Math.Max(5, pageSize);
        if (_selectedIndex < _viewOffset) _viewOffset = _selectedIndex;
        else if (_selectedIndex >= _viewOffset + pageSize) _viewOffset = _selectedIndex - pageSize + 1;
        if (_currentItems.Count > pageSize) _viewOffset = Math.Clamp(_viewOffset, 0, _currentItems.Count - pageSize);
    }

    private bool MoveSelection(int direction)
    {
        if (_currentItems.Count == 0) return false;
        var newIndex = Math.Clamp(_selectedIndex + direction, 0, _currentItems.Count - 1);
        if (newIndex == _selectedIndex) return false;
        _selectedIndex = newIndex;
        return true;
    }

    private void UpdatePreview(bool resetScroll = true)
    {
        if (resetScroll)
        {
            _previewVerticalOffset = 0;
            _previewHorizontalOffset = 0;
        }
        var selectedItem = (_selectedIndex >= 0 && _selectedIndex < _currentItems.Count) ? _currentItems[_selectedIndex] : null;
        _currentPreview = (selectedItem == null || selectedItem.IsDirectory)
            ? _filePreviewService.GetPreview(null, 0, 0)
            : _filePreviewService.GetPreview(selectedItem.Path, _previewVerticalOffset, _previewHorizontalOffset);
    }

    private void OpenSelectedItem()
    {
        if (_currentItems.Count == 0) return;
        var selectedItem = _currentItems[_selectedIndex];
        if (selectedItem.IsDirectory) NavigateToDirectory(selectedItem.Path);
        else FileSystemService.OpenFile(selectedItem.Path);
    }

    private void NavigateToDirectory(string path)
    {
        try
        {
            var fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(fullPath)) return;
            _currentPath = fullPath;
            _viewOffset = 0;
            _selectedIndex = 0;
            LoadCurrentDirectory();
            AdjustViewPort();
            UpdatePreview();
        }
        catch (Exception ex) { FileManagerRenderer.ShowError(ex.Message); }
    }

    private void NavigateUp()
    {
        var parent = Directory.GetParent(_currentPath);
        if (parent != null) NavigateToDirectory(parent.FullName);
    }

    private void LoadCurrentDirectory()
    {
        try
        {
            _currentItems = FileSystemService.GetDirectoryContents(_currentPath);
            _selectedIndex = _currentItems.Count > 0 ? 0 : -1;
        }
        catch (Exception ex)
        {
            FileManagerRenderer.ShowError(ex.Message);
            _currentItems = [];
            _selectedIndex = -1;
        }
    }
}
