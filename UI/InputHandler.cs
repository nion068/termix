namespace termix.UI;

public class InputHandler(FileManager fileManager)
{
    public bool ProcessKey(ConsoleKeyInfo keyInfo)
    {
        fileManager.ClearStatusMessage();

        if (fileManager.CurrentMode != FileManager.InputMode.Normal || !fileManager.IsViewFiltered ||
            keyInfo.Key != ConsoleKey.Escape)
            return fileManager.CurrentMode switch
            {
                FileManager.InputMode.Normal or FileManager.InputMode.FilteredNavigation => HandleNormalKeyPress(
                    keyInfo),
                _ => HandleInputModeKeyPress(keyInfo)
            };
        fileManager.ClearFilter();
        return true;
    }

    private bool HandleNormalKeyPress(ConsoleKeyInfo keyInfo)
    {
        var key = keyInfo.Key;

        switch (key)
        {
            case ConsoleKey.Q or ConsoleKey.Escape when !fileManager.IsViewFiltered:
                return false;
            case ConsoleKey.B when fileManager.CurrentMode == FileManager.InputMode.FilteredNavigation:
                fileManager.ReturnToFilter();
                return true;
        }

        if (HandleSelectionMovement(key, keyInfo.Modifiers)) return true;

        switch (key)
        {
            case ConsoleKey.Enter:
            case ConsoleKey.L:
            case ConsoleKey.O: fileManager.OpenSelectedItem(); break;
            case ConsoleKey.Backspace:
            case ConsoleKey.H: fileManager.NavigateUp(); break;
            case ConsoleKey.A: fileManager.BeginAdd(); break;
            case ConsoleKey.R: fileManager.BeginRename(); break;
            case ConsoleKey.D: fileManager.BeginDelete(); break;
            case ConsoleKey.S: fileManager.BeginFilter(); break;
        }

        return true;
    }

    private bool HandleInputModeKeyPress(ConsoleKeyInfo keyInfo)
    {
        switch (fileManager.CurrentMode)
        {
            case FileManager.InputMode.Filter:
                HandleFilterInput(keyInfo);
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

    private void HandleFilterInput(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.Enter:
            case ConsoleKey.UpArrow:
            case ConsoleKey.DownArrow:
                fileManager.AcceptFilter();
                HandleNormalKeyPress(keyInfo);
                break;

            case ConsoleKey.Escape:
                fileManager.AcceptFilter();
                break;

            case ConsoleKey.Backspace:
                if (fileManager.GetInputText().Length > 0) fileManager.UpdateFilter(fileManager.GetInputText(-1));

                break;

            default:
                if (!char.IsControl(keyInfo.KeyChar))
                    fileManager.UpdateFilter(fileManager.GetInputText() + keyInfo.KeyChar);

                break;
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
        switch (key)
        {
            case ConsoleKey.Y:
                fileManager.CommitDelete();
                break;
            case ConsoleKey.N or ConsoleKey.Escape:
                fileManager.ResetToNormalMode();
                break;
        }
    }

    private bool HandleSelectionMovement(ConsoleKey key, ConsoleModifiers modifier)
    {
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
            fileManager.ScrollPreview(offset.v, offset.h);
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
            fileManager.MoveSelection(direction);
            return true;
        }

        if (key is not (ConsoleKey.Home or ConsoleKey.End)) return false;
        fileManager.MoveSelectionToEdge(key == ConsoleKey.Home);
        return true;
    }
}