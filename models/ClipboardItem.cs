namespace termix.models;

public enum ClipboardMode
{
    Copy,
    Move
}

public record ClipboardItem(List<FileSystemItem> Items, ClipboardMode Mode);