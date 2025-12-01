using Scio.Models;

namespace Scio.Services
{
    public class BookManagementService
    {
        private readonly IBookApiService _bookApiService;

        public BookManagementService(IBookApiService bookApiService)
        {
            _bookApiService = bookApiService;
        }

        /// <summary>
        /// Get all books or search by term
        /// </summary>
        public async Task<ApiResult<List<Book>>> LoadBooksAsync(string? searchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _bookApiService.GetAllBooksAsync();
            }

            return await _bookApiService.SearchBooksAsync(searchTerm);
        }

        /// <summary>
        /// Add a new book to the library
        /// </summary>
        public async Task<ApiResult> AddBookAsync(Book newBook)
        {
            if (!ValidateBook(newBook))
            {
                return ApiResult.FailureResult(
                    "Validation failed: Please fill in all required fields (Title, Author, ISBN, Total Copies > 0)",
                    ApiErrorType.ValidationError);
            }

            newBook.AvailableCopies = newBook.TotalCopies;
            newBook.BorrowHistory = new List<BorrowRecord>();

            return await _bookApiService.AddBookAsync(newBook);
        }

        /// <summary>
        /// Borrow a book for a user
        /// </summary>
        public async Task<ApiResult> BorrowBookAsync(Guid bookId, string userName)
        {
            return await _bookApiService.BorrowBookAsync(bookId, userName);
        }

        /// <summary>
        /// Return a borrowed book
        /// </summary>
        public async Task<ApiResult> ReturnBookAsync(Guid borrowRecordId)
        {
            return await _bookApiService.ReturnBorrowRecordAsync(borrowRecordId);
        }

        /// <summary>
        /// Get all borrowed books (including returned ones)
        /// </summary>
        public async Task<ApiResult<List<BorrowedBookInfo>>> GetAllBorrowedBooksAsync()
        {
            return await _bookApiService.GetBorrowedBooksAsync();
        }

        /// <summary>
        /// Get only currently borrowed books (not returned)
        /// </summary>
        public async Task<ApiResult<List<BorrowedBookInfo>>> GetCurrentlyBorrowedBooksAsync()
        {
            var result = await _bookApiService.GetBorrowedBooksAsync();
            if (!result.Success)
            {
                return ApiResult<List<BorrowedBookInfo>>.FailureResult(
                    result.ErrorMessage ?? "Failed to fetch borrowed books",
                    result.ErrorType);
            }

            var filtered = result.Data?.Where(b => b.ReturnDate == null).ToList() ?? new();
            return ApiResult<List<BorrowedBookInfo>>.SuccessResult(filtered);
        }

        /// <summary>
        /// Get borrow history for a specific book
        /// </summary>
        public async Task<ApiResult<List<BorrowedBookInfo>>> GetBookHistoryAsync(Guid bookId)
        {
            var result = await _bookApiService.GetBorrowedBooksAsync();
            if (!result.Success)
            {
                return ApiResult<List<BorrowedBookInfo>>.FailureResult(
                    result.ErrorMessage ?? "Failed to fetch book history",
                    result.ErrorType);
            }

            var filtered = result.Data?.Where(b => b.BookId == bookId).ToList() ?? new();
            return ApiResult<List<BorrowedBookInfo>>.SuccessResult(filtered);
        }

        /// <summary>
        /// Validate book data before adding
        /// </summary>
        private bool ValidateBook(Book book)
        {
            return !string.IsNullOrWhiteSpace(book.Title) &&
                   !string.IsNullOrWhiteSpace(book.Author) &&
                   !string.IsNullOrWhiteSpace(book.ISBN) &&
                   book.TotalCopies > 0;
        }
    }
}
