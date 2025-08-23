using termix.models;

namespace termix.UI;

public class InputHandler(FileManager fileManager)
{
    private readonly FileManagerState _state = fileManager.State;
    private string _keyBuffer = "";
    private DateTime _lastKeyTime = DateTime.MinValue;
    private const int KEY_TIMEOUT_MS = 1000; // 1 second timeout for multi-key sequences

    public void ProcessKey(ConsoleKeyInfo keyInfo)
    {
        _state.StatusMessage = null;

        // Handle key buffer timeout
        if ((DateTime.Now - _lastKeyTime).TotalMilliseconds > KEY_TIMEOUT_MS)
        {
            _keyBuffer = "";
        }
        _lastKeyTime = DateTime.Now;

        if (_state.CurrentMode == InputMode.Normal && keyInfo.Key == ConsoleKey.Escape && _state.Clipboard != null)
        {
            fileManager.ActionHandler.ClearClipboard();
            return;
        }
        
        if (_state.CurrentMode != InputMode.Normal || !string.IsNullOrEmpty(_state.InputText) ||
            keyInfo.Key != ConsoleKey.Escape)
            switch (_state.CurrentMode)
            {
                case InputMode.Normal or InputMode.FilteredNavigation:
                    HandleNormalKeyPress(keyInfo);
                    break;
                case InputMode.SortMenu:
                    HandleSortMenuInput(keyInfo);
                    break;
                default:
                    HandleInputModeKeyPress(keyInfo);
                    break;
            }
        else
            fileManager.FilterHandler.ClearFilter();
    }

    private void HandleNormalKeyPress(ConsoleKeyInfo keyInfo)
    {
        var key = keyInfo.Key;

        if (HandleMultiKeySequence(keyInfo)) return;

        switch (key)
        {
            case ConsoleKey.Escape when !string.IsNullOrEmpty(_state.InputText):
                fileManager.FilterHandler.ClearFilter();
                return;
            case ConsoleKey.Q:
            case ConsoleKey.Escape:
                fileManager.ActionHandler.RequestQuit();
                return;
            case ConsoleKey.B when _state.CurrentMode == InputMode.FilteredNavigation:
                fileManager.FilterHandler.ReturnToFilter();
                return;
        }

        if (HandleSelectionMovement(key, keyInfo.Modifiers)) return;

        switch (key)
        {
            case ConsoleKey.Enter:
            case ConsoleKey.L:
            case ConsoleKey.O: fileManager.NavigationHandler.OpenSelectedItem(); break;
            case ConsoleKey.Backspace:
            case ConsoleKey.H: fileManager.NavigationHandler.NavigateUp(); break;
            case ConsoleKey.A: fileManager.ActionHandler.BeginAdd(); break;
            case ConsoleKey.R: fileManager.ActionHandler.BeginRename(); break;
            case ConsoleKey.D: fileManager.ActionHandler.BeginDelete(); break;
            case ConsoleKey.S: fileManager.FilterHandler.BeginFilter(); break;
            case ConsoleKey.T: fileManager.ActionHandler.BeginSortMenu(); break;
            case ConsoleKey.C: fileManager.ActionHandler.BeginCopy(); break;
            case ConsoleKey.X: fileManager.ActionHandler.BeginMove(); break;
            case ConsoleKey.P: fileManager.ActionHandler.BeginPaste(); break;
        }
    }

    private bool HandleMultiKeySequence(ConsoleKeyInfo keyInfo)
    {
        var keyChar = keyInfo.KeyChar;
        
        if (keyChar == 'G')
        {
            fileManager.NavigationHandler.MoveSelectionToEdge(false);
            _keyBuffer = "";
            return true;
        }
        
        _keyBuffer += keyChar.ToString().ToLower();
        
        if (_keyBuffer == "gg")
        {
            fileManager.NavigationHandler.MoveSelectionToEdge(true);
            _keyBuffer = "";
            return true;
        }
        
        if (_keyBuffer.Length > 2)
        {
            _keyBuffer = keyChar.ToString().ToLower();
        }
        
        if (keyChar != 'g')
        {
            _keyBuffer = "";
        }
        
        return false;
    }

    private void HandleInputModeKeyPress(ConsoleKeyInfo keyInfo)
    {
        switch (_state.CurrentMode)
        {
            case InputMode.Filter:
                HandleFilterInput(keyInfo);
                break;
            case InputMode.Add or InputMode.Rename:
                HandleStandardTextInput(keyInfo);
                break;
            case InputMode.DeleteConfirm:
                HandleDeleteConfirmation(keyInfo.Key);
                break;
            case InputMode.QuitConfirm:
                HandleQuitConfirmation(keyInfo.Key);
                break;
        }
    }
    
    private void HandleSortMenuInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Escape:
            case ConsoleKey.Q:
                fileManager.ResetToNormalMode();
                break;
            case ConsoleKey.Enter:
                fileManager.ActionHandler.ApplySort();
                break;
            case ConsoleKey.DownArrow:
            case ConsoleKey.J:
                fileManager.NavigationHandler.MoveSortMenuSelection(1);
                break;
            case ConsoleKey.UpArrow:
            case ConsoleKey.K:
                fileManager.NavigationHandler.MoveSortMenuSelection(-1);
                break;
        }
    }

    private void HandleQuitConfirmation(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Y: fileManager.Quit(true); break;
            case ConsoleKey.N or ConsoleKey.Escape: fileManager.ResetToNormalMode(); break;
        }
    }

    private void HandleFilterInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
            case ConsoleKey.UpArrow:
            case ConsoleKey.DownArrow:
                fileManager.FilterHandler.AcceptFilter();
                HandleNormalKeyPress(keyInfo);
                break;
            case ConsoleKey.Escape:
                fileManager.FilterHandler.AcceptFilter();
                break;
            case ConsoleKey.Backspace when _state.InputText.Length > 0:
                fileManager.FilterHandler.UpdateFilter(_state.InputText[..^1]);
                break;
            default:
                if (!char.IsControl(keyInfo.KeyChar))
                    fileManager.FilterHandler.UpdateFilter(_state.InputText + keyInfo.KeyChar);
                break;
        }
    }

    private void HandleStandardTextInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter: fileManager.ActionHandler.CommitStandardTextInput(); break;
            case ConsoleKey.Escape: fileManager.ResetToNormalMode(); break;
            case ConsoleKey.Backspace when _state.InputText.Length > 0: 
                _state.InputText = _state.InputText[..^1]; 
                fileManager.SetNeedsRedraw(); 
                break;
            default:
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    _state.InputText += keyInfo.KeyChar;
                    fileManager.SetNeedsRedraw();
                }
                break;
        }
    }

    private void HandleDeleteConfirmation(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Y: fileManager.ActionHandler.CommitDelete(); break;
            case ConsoleKey.N or ConsoleKey.Escape: fileManager.ResetToNormalMode(); break;
        }
    }

    private bool HandleSelectionMovement(ConsoleKey key, ConsoleModifiers modifier)
    {
        if (modifier == ConsoleModifiers.Control)
        {
            switch (key)
            {
                case ConsoleKey.D:
                    fileManager.NavigationHandler.ScrollSelection(1); // Scroll down (positive direction)
                    return true;
                case ConsoleKey.U:
                    fileManager.NavigationHandler.ScrollSelection(-1); // Scroll up (negative direction)
                    return true;
            }
        }

        if (modifier == ConsoleModifiers.Alt)
        {
            (int v, int h) offset = key switch
            {
                ConsoleKey.UpArrow or ConsoleKey.K => (-1, 0),
                ConsoleKey.DownArrow or ConsoleKey.J => (1, 0),
                ConsoleKey.LeftArrow or ConsoleKey.H => (0, -5),
                ConsoleKey.RightArrow or ConsoleKey.L => (0, 5),
                _ => (0, 0)
            };
            if (offset == (0, 0)) return false;
            fileManager.NavigationHandler.ScrollPreview(offset.v, offset.h);
            return true;
        }

        var direction = key switch
        {
            ConsoleKey.DownArrow or ConsoleKey.J => 1,
            ConsoleKey.UpArrow or ConsoleKey.K => -1,
            _ => 0
        };
        if (direction != 0)
        {
            fileManager.NavigationHandler.MoveSelection(direction);
            return true;
        }

        if (key is not (ConsoleKey.Home or ConsoleKey.End)) return false;
        fileManager.NavigationHandler.MoveSelectionToEdge(key == ConsoleKey.Home);
        return true;
    }
}
