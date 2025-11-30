using Scio.API.Models;
using System.Text.Json;

namespace Scio.API.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(Guid id);
        Task<List<Book>> SearchBooksAsync(string searchTerm);
        Task<Book> AddBookAsync(Book book);
        Task<bool> BorrowBookAsync(Guid id, string userName);
        Task<bool> ReturnBookAsync(Guid id);
        Task<bool> ReturnBorrowRecordAsync(Guid borrowRecordId);
        Task<List<BorrowedBookInfo>> GetBorrowedBooksAsync();
    }

    public class BookService : IBookService
    {
        private readonly string _filePathBooks = "books.json";
        private List<Book> _books = new();

        public BookService()
        {
            // Load from file on initialization
            _books = LoadBooksFromFile().Result;

            // Recalculate available copies based on borrow history
            foreach (var book in _books)
            {
                book.AvailableCopies = book.TotalCopies - book.BorrowHistory.Count(b => b.ReturnDate == null);
            }

            // If file doesn't exist or is empty, initialize with default data
            if (_books.Count == 0)
            {
                _books = new List<Book>
                {
                    new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = "1984",
                        Author = "George Orwell",
                        YearOfPublication = 1949,
                        ISBN = "978-0451524935",
                        TotalCopies = 5,
                        AvailableCopies = 5,
                        BorrowHistory = new List<BorrowRecord>()
                    },
                    new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = "To Kill a Mockingbird",
                        Author = "Harper Lee",
                        YearOfPublication = 1960,
                        ISBN = "978-0061120084",
                        TotalCopies = 3,
                        AvailableCopies = 3,
                        BorrowHistory = new List<BorrowRecord>()
                    },
                    new Book
                    {
                        Id = Guid.NewGuid(),
                        Title = "The Great Gatsby",
                        Author = "F. Scott Fitzgerald",
                        YearOfPublication = 1925,
                        ISBN = "978-0743273565",
                        TotalCopies = 7,
                        AvailableCopies = 7,
                        BorrowHistory = new List<BorrowRecord>()
                    }
                };
                SaveBooksToFile().Wait();
            }
        }

        public async Task<List<Book>> GetAllBooksAsync()
        {
            await Task.Delay(100); // Simulate network delay
            return _books;
        }

        public async Task<Book?> GetBookByIdAsync(Guid id)
        {
            await Task.Delay(50);
            return _books.FirstOrDefault(b => b.Id == id);
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm)
        {
            await Task.Delay(100);
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _books;

            var term = searchTerm.ToLowerInvariant();
            return _books.Where(b =>
                b.Title.ToLowerInvariant().Contains(term) ||
                b.Author.ToLowerInvariant().Contains(term) ||
                b.ISBN.Contains(term)
            ).ToList();
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            book.Id = Guid.NewGuid();
            _books.Add(book);
            await SaveBooksToFile();
            return book;
        }

        public async Task<bool> BorrowBookAsync(Guid id, string userName)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null || book.AvailableCopies <= 0)
                return false;

            // Add borrow record
            var borrowRecord = new BorrowRecord
            {
                Id = Guid.NewGuid(),
                User = userName,
                BorrowDate = DateTime.UtcNow,
                ReturnDate = null
            };
            book.BorrowHistory.Add(borrowRecord);

            // Update available copies
            book.AvailableCopies = book.TotalCopies - book.BorrowHistory.Count(b => b.ReturnDate == null);

            await SaveBooksToFile();
            return true;
        }

        public async Task<bool> ReturnBookAsync(Guid id)
        {
            var book = _books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return false;

            // Find the most recent unreturned borrow record
            var borrowRecord = book.BorrowHistory.LastOrDefault(b => b.ReturnDate == null);
            if (borrowRecord == null)
                return false; // No outstanding borrow

            borrowRecord.ReturnDate = DateTime.UtcNow;

            // Update available copies
            book.AvailableCopies = book.TotalCopies - book.BorrowHistory.Count(b => b.ReturnDate == null);

            await SaveBooksToFile();
            return true;
        }

        public async Task<bool> ReturnBorrowRecordAsync(Guid borrowRecordId)
        {
            // Find the borrow record across all books
            foreach (var book in _books)
            {
                var borrowRecord = book.BorrowHistory.FirstOrDefault(b => b.Id == borrowRecordId && b.ReturnDate == null);
                if (borrowRecord != null)
                {
                    borrowRecord.ReturnDate = DateTime.UtcNow;

                    // Update available copies
                    book.AvailableCopies = book.TotalCopies - book.BorrowHistory.Count(b => b.ReturnDate == null);

                    await SaveBooksToFile();
                    return true;
                }
            }
            return false; // Borrow record not found or already returned
        }

        public Task<List<BorrowedBookInfo>> GetBorrowedBooksAsync()
        {
            var borrowedBooks = new List<BorrowedBookInfo>();

            foreach (var book in _books)
            {
                foreach (var borrow in book.BorrowHistory)
                {
                    borrowedBooks.Add(new BorrowedBookInfo
                    {
                        BookId = book.Id,
                        BookTitle = book.Title,
                        BookAuthor = book.Author,
                        BorrowRecordId = borrow.Id,
                        UserName = borrow.User,
                        BorrowDate = borrow.BorrowDate,
                        ReturnDate = borrow.ReturnDate
                    });
                }
            }

            return Task.FromResult(borrowedBooks);
        }

        private async Task<List<Book>> LoadBooksFromFile()
        {
            try
            {
                if (!File.Exists(_filePathBooks))
                    return new List<Book>();

                var json = await File.ReadAllTextAsync(_filePathBooks);
                return JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }
            catch
            {
                return new List<Book>();
            }
        }

        private async Task SaveBooksToFile()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_books, options);
                await File.WriteAllTextAsync(_filePathBooks, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving books to file: {ex.Message}");
            }
        }
    }
}
