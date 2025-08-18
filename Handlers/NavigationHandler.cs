using Spectre.Console;
using termix.models;

namespace termix.Handlers;

public class NavigationHandler(FileManager fileManager)
{
    private readonly FileManagerState _state = fileManager.State;

    public void MoveSelection(int direction)
    {
        if (_state.CurrentItems.Count == 0) return;
        var newIndex = Math.Clamp(_state.SelectedIndex + direction, 0, _state.CurrentItems.Count - 1);
        if (newIndex == _state.SelectedIndex) return;
        _state.SelectedIndex = newIndex;
        fileManager.AdjustViewPort();
        fileManager.UpdatePreview();
    }

    public void MoveSelectionToEdge(bool toStart)
    {
        _state.SelectedIndex = toStart ? 0 : Math.Max(0, _state.CurrentItems.Count - 1);
        fileManager.AdjustViewPort();
        fileManager.UpdatePreview();
    }

    public void OpenSelectedItem()
    {
        if (_state.CurrentItems.Count == 0) return;
        var selectedItem = _state.CurrentItems[_state.SelectedIndex];

        if (selectedItem.IsDirectory)
        {
            if (!string.IsNullOrEmpty(_state.InputText) &&
                _state.CurrentMode != InputMode.FilteredNavigation)
            {
                _state.SavedFilterState = (_state.CurrentPath, _state.InputText, [.._state.CurrentItems],
                    _state.SelectedIndex);
                _state.CurrentMode = InputMode.FilteredNavigation;
                fileManager.SetNeedsRedraw();
            }

            _state.NavigationStack.Push(selectedItem.Name);
            NavigateToDirectory(selectedItem.Path);
        }
        else
        {
            Services.FileSystemService.OpenFile(selectedItem.Path);
        }
    }

    public void NavigateUp()
    {
        if (_state.CurrentMode is not (InputMode.Normal or InputMode.FilteredNavigation))
            return;

        if (!string.IsNullOrEmpty(_state.InputText))
        {
            fileManager.FilterHandler.ClearFilter();
            return;
        }

        var parent = Directory.GetParent(_state.CurrentPath);
        if (parent != null)
            NavigateToDirectory(parent.FullName, _state.NavigationStack.TryPop(out var result) ? result : null);
    }

    private void NavigateToDirectory(string path, string? findAndSelect = null)
    {
        if (_state.CurrentMode != InputMode.FilteredNavigation) fileManager.ResetToNormalMode();
        try
        {
            _state.CurrentPath = Path.GetFullPath(path);
            fileManager.RefreshDirectory(findAndSelect, setInitialSelection: true);
        }
        catch (Exception ex)
        {
            _state.StatusMessage = $"[red]Navigation failed: {ex.Message.EscapeMarkup()}[/]";
            fileManager.SetNeedsRedraw();
        }
    }

    public void MoveSortMenuSelection(int direction)
    {
        if (_state.CurrentMode != InputMode.SortMenu) return;
        var newIndex = _state.SortMenuSelectedIndex + direction;
        _state.SortMenuSelectedIndex = Math.Clamp(newIndex, 0, fileManager.SortOptions.Count - 1);
        fileManager.SetNeedsRedraw();
    }

    public void ScrollPreview(int vertical, int horizontal)
    {
        _state.PreviewVerticalOffset = Math.Max(0, _state.PreviewVerticalOffset + vertical);
        _state.PreviewHorizontalOffset = Math.Max(0, _state.PreviewHorizontalOffset + horizontal);
        fileManager.UpdatePreview(false);
    }
}