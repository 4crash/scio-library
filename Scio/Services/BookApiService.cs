using Scio.Models;
using System.Net.Http.Json;
using System.Diagnostics;

namespace Scio.Services
{
    public interface IBookApiService
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(Guid id);
        Task<List<Book>> SearchBooksAsync(string searchTerm);
        Task<Book?> AddBookAsync(Book book);
        Task<bool> BorrowBookAsync(Guid id, string userName);
        Task<bool> ReturnBookAsync(Guid id);
        Task<bool> ReturnBorrowRecordAsync(Guid borrowRecordId);
        Task<List<BorrowedBookInfo>> GetBorrowedBooksAsync();
    }

    public class BookApiService : IBookApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "http://localhost:5250/api/book";

        public BookApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            Debug.WriteLine("Fetching all books from API...");
            try
            {
                var response = await _httpClient.GetAsync(ApiBaseUrl);
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Book>>() ?? [];
                }
                return [];
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetAllBooksAsync: {ex.Message}");
                return [];
            }
        }

        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Book>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/search?term={Uri.EscapeDataString(searchTerm)}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Book>>() ?? [];
                }
                return [];
            }
            catch
            {
                return [];
            }
        }

        public async Task<Book?> AddBookAsync(Book book)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, book);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Book>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> BorrowBookAsync(Guid id, string userName)
        {
            try
            {
                var request = new { UserName = userName };
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/{id}/borrow", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ReturnBookAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/{id}/return", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ReturnBorrowRecordAsync(Guid borrowRecordId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{ApiBaseUrl}/return-record/{borrowRecordId}", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<BorrowedBookInfo>> GetBorrowedBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ApiBaseUrl}/borrowed");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<BorrowedBookInfo>>() ?? [];
                }
                return [];
            }
            catch
            {
                return [];
            }
        }
    }

}
