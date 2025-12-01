using Scio.Models;
using System.Net.Http.Json;
using System.Diagnostics;

namespace Scio.Services
{
    public interface IBookApiService
    {
        Task<ApiResult<List<Book>>> GetAllBooksAsync();
        Task<ApiResult<Book>> GetBookByIdAsync(Guid id);
        Task<ApiResult<List<Book>>> SearchBooksAsync(string searchTerm);
        Task<ApiResult> AddBookAsync(Book book);
        Task<ApiResult> BorrowBookAsync(Guid id, string userName);
        Task<ApiResult> ReturnBookAsync(Guid id);
        Task<ApiResult> ReturnBorrowRecordAsync(Guid borrowRecordId);
        Task<ApiResult<List<BorrowedBookInfo>>> GetBorrowedBooksAsync();
    }

    public class BookApiService : IBookApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "http://localhost:5250/api/book";

        public BookApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResult<List<Book>>> GetAllBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiBaseUrl);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var books = await response.Content.ReadFromJsonAsync<List<Book>>() ?? [];
                    return ApiResult<List<Book>>.SuccessResult(books);
                }

                return ApiResult<List<Book>>.FailureResult(
                    $"API returned {response.StatusCode}: {response.ReasonPhrase}",
                    ApiErrorType.HttpError);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"Network error in GetAllBooksAsync: {ex.Message}");
                return ApiResult<List<Book>>.FailureResult(
                    "Network error: Unable to reach the API service",
                    ApiErrorType.NetworkError);
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine($"Timeout in GetAllBooksAsync: {ex.Message}");
                return ApiResult<List<Book>>.FailureResult(
                    "Request timeout: API service is not responding",
                    ApiErrorType.Timeout);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error in GetAllBooksAsync: {ex.Message}");
                return ApiResult<List<Book>>.FailureResult(
                    $"Unexpected error: {ex.Message}",
                    ApiErrorType.ServerError);
            }
        }

        public async Task<ApiResult<Book>> GetBookByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var book = await response.Content.ReadFromJsonAsync<Book>();
                    return book != null
                        ? ApiResult<Book>.SuccessResult(book)
                        : ApiResult<Book>.FailureResult("Book not found", ApiErrorType.HttpError);
                }

                return ApiResult<Book>.FailureResult(
                    $"API returned {response.StatusCode}",
                    ApiErrorType.HttpError);
            }
            catch (HttpRequestException)
            {
                return ApiResult<Book>.FailureResult(
                    "Network error: Unable to reach the API service",
                    ApiErrorType.NetworkError);
            }
            catch (TaskCanceledException)
            {
                return ApiResult<Book>.FailureResult(
                    "Request timeout: API service is not responding",
                    ApiErrorType.Timeout);
            }
            catch (Exception)
            {
                return ApiResult<Book>.FailureResult(
                    "Unexpected error: Failed to fetch book",
                    ApiErrorType.ServerError);
            }
        }

        public async Task<ApiResult<List<Book>>> SearchBooksAsync(string searchTerm)
        {
            try
            {
                // Trim and encode search term for security
                var cleanSearchTerm = (searchTerm ?? string.Empty).Trim();
                var encodedTerm = Uri.EscapeDataString(cleanSearchTerm);
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/search?searchTerm={encodedTerm}");

                if (response.IsSuccessStatusCode)
                {
                    var books = await response.Content.ReadFromJsonAsync<List<Book>>() ?? [];
                    return ApiResult<List<Book>>.SuccessResult(books);
                }

                return ApiResult<List<Book>>.FailureResult(
                    $"API returned {response.StatusCode}",
                    ApiErrorType.HttpError);
            }
            catch (HttpRequestException)
            {
                return ApiResult<List<Book>>.FailureResult(
                    "Network error: Unable to reach the API service",
                    ApiErrorType.NetworkError);
            }
            catch (TaskCanceledException)
            {
                return ApiResult<List<Book>>.FailureResult(
                    "Request timeout: API service is not responding",
                    ApiErrorType.Timeout);
            }
            catch (Exception)
            {
                return ApiResult<List<Book>>.FailureResult(
                    "Unexpected error: Failed to fetch books",
                    ApiErrorType.ServerError);
            }
        }

        public async Task<ApiResult> AddBookAsync(Book book)
        {
            try
            {
                // Convert Book to AddBookRequest for validation on server
                var request = new AddBookRequest
                {
                    Title = book.Title?.Trim() ?? string.Empty,
                    Author = book.Author?.Trim() ?? string.Empty,
                    YearOfPublication = book.YearOfPublication > 0 ? book.YearOfPublication.ToString() : null,
                    ISBN = book.ISBN?.Trim() ?? string.Empty,
                    TotalCopies = book.TotalCopies
                };

                var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, request);
                if (response.IsSuccessStatusCode)
                {
                    return ApiResult.SuccessResult();
                }

                return ApiResult.FailureResult(
                    $"API returned {response.StatusCode}: {response.ReasonPhrase}",
                    ApiErrorType.HttpError);
            }
            catch (HttpRequestException)
            {
                return ApiResult.FailureResult(
                    "Network error: Unable to reach the API service",
                    ApiErrorType.NetworkError);
            }
            catch (TaskCanceledException)
            {
                return ApiResult.FailureResult(
                    "Request timeout: API service is not responding",
                    ApiErrorType.Timeout);
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult(
                    $"Unexpected error: {ex.Message}",
                    ApiErrorType.ServerError);
            }
        }

        public async Task<ApiResult> BorrowBookAsync(Guid id, string userName)
        {
            try
            {
                var request = new BorrowRequest { UserName = userName?.Trim() ?? string.Empty };
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/{id}/borrow", request);
                if (response.IsSuccessStatusCode)
                {
                    return ApiResult.SuccessResult();
                }

                return ApiResult.FailureResult(
                    $"API returned {response.StatusCode}: {response.ReasonPhrase}",
                    ApiErrorType.HttpError);
            }
            catch (HttpRequestException)
            {
                return ApiResult.FailureResult(
                    "Network error: Unable to reach the API service",
                    ApiErrorType.NetworkError);
            }
            catch (TaskCanceledException)
            {
                return ApiResult.FailureResult(
                    "Request timeout: API service is not responding",
                    ApiErrorType.Timeout);
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult(
                    $"Unexpected error: {ex.Message}",
                    ApiErrorType.ServerError);
            }
        }

        public async Task<ApiResult> ReturnBookAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/{id}/return", null);
                if (response.IsSuccessStatusCode)
                {
                    return ApiResult.SuccessResult();
                }

                return ApiResult.FailureResult(
                    $"API returned {response.StatusCode}: {response.ReasonPhrase}",
                    ApiErrorType.HttpError);
            }
            catch (HttpRequestException)
            {
                return ApiResult.FailureResult(
                    "Network error: Unable to reach the API service",
                    ApiErrorType.NetworkError);
            }
            catch (TaskCanceledException)
            {
                return ApiResult.FailureResult(
                    "Request timeout: API service is not responding",
                    ApiErrorType.Timeout);
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult(
                    $"Unexpected error: {ex.Message}",
                    ApiErrorType.ServerError);
            }
        }

        public async Task<ApiResult> ReturnBorrowRecordAsync(Guid borrowRecordId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/return-record/{borrowRecordId}", null);
                if (response.IsSuccessStatusCode)
                {
                    return ApiResult.SuccessResult();
                }

                return ApiResult.FailureResult(
                    $"API returned {response.StatusCode}: {response.ReasonPhrase}",
                    ApiErrorType.HttpError);
            }
            catch (HttpRequestException)
            {
                return ApiResult.FailureResult(
                    "Network error: Unable to reach the API service",
                    ApiErrorType.NetworkError);
            }
            catch (TaskCanceledException)
            {
                return ApiResult.FailureResult(
                    "Request timeout: API service is not responding",
                    ApiErrorType.Timeout);
            }
            catch (Exception ex)
            {
                return ApiResult.FailureResult(
                    $"Unexpected error: {ex.Message}",
                    ApiErrorType.ServerError);
            }
        }

        public async Task<ApiResult<List<BorrowedBookInfo>>> GetBorrowedBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/borrowed");
                if (response.IsSuccessStatusCode)
                {
                    var books = await response.Content.ReadFromJsonAsync<List<BorrowedBookInfo>>() ?? [];
                    return ApiResult<List<BorrowedBookInfo>>.SuccessResult(books);
                }

                return ApiResult<List<BorrowedBookInfo>>.FailureResult(
                    $"API returned {response.StatusCode}",
                    ApiErrorType.HttpError);
            }
            catch (HttpRequestException)
            {
                return ApiResult<List<BorrowedBookInfo>>.FailureResult(
                    "Network error: Unable to reach the API service",
                    ApiErrorType.NetworkError);
            }
            catch (TaskCanceledException)
            {
                return ApiResult<List<BorrowedBookInfo>>.FailureResult(
                    "Request timeout: API service is not responding",
                    ApiErrorType.Timeout);
            }
            catch (Exception ex)
            {
                return ApiResult<List<BorrowedBookInfo>>.FailureResult(
                    $"Unexpected error: {ex.Message}",
                    ApiErrorType.ServerError);
            }
        }
    }

}
