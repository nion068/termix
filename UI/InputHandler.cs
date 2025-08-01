namespace termix.UI;

public class InputHandler(FileManager fileManager)
{
    public bool ProcessKey(ConsoleKeyInfo keyInfo)
    {
        if (fileManager.CurrentMode == FileManager.InputMode.Normal)
        {
            fileManager.ClearStatusMessage();
        }

        return fileManager.CurrentMode switch
        {
            FileManager.InputMode.Normal => HandleNormalKeyPress(keyInfo),
            _ => HandleInputModeKeyPress(keyInfo)
        };
    }

    private bool HandleNormalKeyPress(ConsoleKeyInfo keyInfo)
    {
        var key = keyInfo.Key;
        var modifier = keyInfo.Modifiers;

        if (fileManager.IsShowingSearchResults && (key is ConsoleKey.Escape or ConsoleKey.Backspace))
        {
            fileManager.ExitSearch();
            return true;
        }
        
        if (key is ConsoleKey.Q or ConsoleKey.Escape) return false;

        if (HandleSelectionMovement(key)) return true;
        if (HandlePreviewScrolling(key, modifier)) return true;
        
        switch (key)
        {
            case ConsoleKey.Enter: fileManager.OpenSelectedItem(); break;
            case ConsoleKey.Backspace: fileManager.NavigateUp(); break;
            case ConsoleKey.A: fileManager.BeginAdd(); break;
            case ConsoleKey.R: fileManager.BeginRename(); break;
            case ConsoleKey.D: fileManager.BeginDelete(); break;
            case ConsoleKey.S: fileManager.BeginSearch(); break;
        }

        return true;
    }

    private bool HandleInputModeKeyPress(ConsoleKeyInfo keyInfo)
    {
        switch (fileManager.CurrentMode)
        {
            case FileManager.InputMode.Search:
                HandleSearchInput(keyInfo);
                break;
            case FileManager.InputMode.Add or FileManager.InputMode.Rename:
                HandleStandardTextInput(keyInfo);
                break;
            case FileManager.InputMode.DeleteConfirm:
                HandleDeleteConfirmation(keyInfo.Key);
                break;
        }
        return true;
    }

    private void HandleSearchInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
                fileManager.FinalizeSearch();
                break;
            case ConsoleKey.Escape:
                fileManager.ExitSearch();
                break;
            case ConsoleKey.Backspace:
                fileManager.UpdateSearchQuery(fileManager.GetInputText(sliceEnd: -1));
                break;
            default:
            {
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    fileManager.UpdateSearchQuery(fileManager.GetInputText() + keyInfo.KeyChar);
                }

                break;
            }
        }
    }

    private void HandleStandardTextInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter: fileManager.CommitStandardTextInput(); break;
            case ConsoleKey.Escape: fileManager.ResetToNormalMode(); break;
            case ConsoleKey.Backspace: fileManager.HandleBackspace(); break;
            default:
                if (!char.IsControl(keyInfo.KeyChar)) fileManager.AppendInputText(keyInfo.KeyChar);
                break;
        }
    }

    private void HandleDeleteConfirmation(ConsoleKey key)
    {
        if (key == ConsoleKey.Y) fileManager.CommitDelete();
        else if (key is ConsoleKey.N or ConsoleKey.Escape) fileManager.ResetToNormalMode();
    }
    
    private bool HandleSelectionMovement(ConsoleKey key)
    {
        var direction = key switch
        {
            ConsoleKey.DownArrow or ConsoleKey.J => 1,
            ConsoleKey.UpArrow or ConsoleKey.K => -1,
            _ => 0
        };

        if (direction != 0)
        {
            fileManager.MoveSelection(direction);
            return true;
        }

        if (key is not (ConsoleKey.Home or ConsoleKey.End)) return false;
        fileManager.MoveSelectionToEdge(key == ConsoleKey.Home);
        return true;
    }

    private bool HandlePreviewScrolling(ConsoleKey key, ConsoleModifiers modifier)
    {
        if (modifier != ConsoleModifiers.Alt) return false;
        
        (int v, int h) offset = key switch
        {
            ConsoleKey.UpArrow or ConsoleKey.K => (-1, 0),
            ConsoleKey.DownArrow or ConsoleKey.J => (1, 0),
            ConsoleKey.LeftArrow or ConsoleKey.H => (0, -5),
            ConsoleKey.RightArrow or ConsoleKey.L => (0, 5),
            _ => (0, 0)
        };

        if (offset == (0, 0)) return false;
        fileManager.ScrollPreview(offset.v, offset.h);
        return true;
    }
}
