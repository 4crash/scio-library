using Scio.Models;

namespace Scio.Services
{
    /// <summary>
    /// Service for managing toast notifications throughout the application
    /// </summary>
    public class ToastService
    {
        /// <summary>
        /// Event fired when a new toast message should be displayed
        /// </summary>
        public event Action<ToastMessage>? OnToastAdded;

        /// <summary>
        /// Event fired when a toast should be removed
        /// </summary>
        public event Action<string>? OnToastRemoved;

        /// <summary>
        /// Show a success toast
        /// </summary>
        public void ShowSuccess(string message, int durationMs = 5000)
        {
            var toast = new ToastMessage
            {
                Type = ToastType.Success,
                Message = message,
                DurationMs = durationMs
            };
            OnToastAdded?.Invoke(toast);
        }

        /// <summary>
        /// Show an error toast
        /// </summary>
        public void ShowError(string message, int durationMs = 7000)
        {
            var toast = new ToastMessage
            {
                Type = ToastType.Error,
                Message = message,
                DurationMs = durationMs
            };
            OnToastAdded?.Invoke(toast);
        }

        /// <summary>
        /// Show a warning toast
        /// </summary>
        public void ShowWarning(string message, int durationMs = 6000)
        {
            var toast = new ToastMessage
            {
                Type = ToastType.Warning,
                Message = message,
                DurationMs = durationMs
            };
            OnToastAdded?.Invoke(toast);
        }

        /// <summary>
        /// Show an info toast
        /// </summary>
        public void ShowInfo(string message, int durationMs = 5000)
        {
            var toast = new ToastMessage
            {
                Type = ToastType.Info,
                Message = message,
                DurationMs = durationMs
            };
            OnToastAdded?.Invoke(toast);
        }

        /// <summary>
        /// Show toast based on ApiResult status
        /// </summary>
        public void ShowFromResult(ApiResult result, string? successMessage = null)
        {
            if (result.Success)
            {
                ShowSuccess(successMessage ?? "Operation completed successfully");
            }
            else
            {
                HandleError(result.ErrorType, result.ErrorMessage);
            }
        }

        /// <summary>
        /// Show toast based on generic ApiResult<T> status
        /// </summary>
        public void ShowFromResult<T>(ApiResult<T> result, string? successMessage = null)
        {
            if (result.Success)
            {
                ShowSuccess(successMessage ?? "Operation completed successfully");
            }
            else
            {
                HandleError(result.ErrorType, result.ErrorMessage);
            }
        }

        /// <summary>
        /// Remove a toast by its ID
        /// </summary>
        public void RemoveToast(string toastId)
        {
            OnToastRemoved?.Invoke(toastId);
        }

        /// <summary>
        /// Handle different error types with appropriate messages
        /// </summary>
        private void HandleError(ApiErrorType errorType, string? message)
        {
            var displayMessage = errorType switch
            {
                ApiErrorType.NetworkError => "Network Error: Unable to connect to the server. Please check your internet connection.",
                ApiErrorType.Timeout => "Request Timeout: The server is not responding. Please try again.",
                ApiErrorType.HttpError => message ?? "Operation failed. The server returned an error. Please try again.",
                ApiErrorType.ValidationError => message ?? "Validation Error: Please check your input and try again.",
                ApiErrorType.ServerError => message ?? "Server Error: An unexpected error occurred. Please try again later.",
                _ => message ?? "An unexpected error occurred."
            };

            ShowError(displayMessage, durationMs: 8000); // Errors stay longer
        }
    }
}
