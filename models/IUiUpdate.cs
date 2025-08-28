namespace termix.models
{
    public interface IUiUpdate { }

    public record ProgressUpdate(string Description, double Value) : IUiUpdate;

    public record OperationComplete(Services.ActionResponse Response, bool IsPasteOperation) : IUiUpdate;
}
