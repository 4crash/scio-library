namespace Scio.Models
{
    /// <summary>
    /// Types of toast notifications
    /// </summary>
    public enum ToastType
    {
        Success,
        Error,
        Warning,
        Info
    }

    /// <summary>
    /// Represents a single toast message
    /// </summary>
    public class ToastMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ToastType Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int DurationMs { get; set; } = 5000; // Auto-dismiss after 5 seconds

        public string CssClass => Type switch
        {
            ToastType.Success => "toast-success",
            ToastType.Error => "toast-error",
            ToastType.Warning => "toast-warning",
            ToastType.Info => "toast-info",
            _ => "toast-info"
        };
    }
}
