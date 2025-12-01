using Xunit;
using Moq;
using Scio.API.Models;
using Scio.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scio.API.Tests
{
    public class BookServiceTests
    {
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _bookService = new BookService();
        }

        #region GetAllBooks Tests

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnAllBooks()
        {
            // Arrange
            // Act
            var books = await _bookService.GetAllBooksAsync();

            // Assert
            Assert.NotNull(books);
            Assert.NotEmpty(books);
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnListOfBooks()
        {
            // Arrange
            // Act
            var books = await _bookService.GetAllBooksAsync();

            // Assert
            Assert.IsType<List<Book>>(books);
            Assert.All(books, book => Assert.NotNull(book));
        }

        #endregion

        #region GetBookById Tests

        [Fact]
        public async Task GetBookByIdAsync_WithValidId_ShouldReturnBook()
        {
            // Arrange
            var books = await _bookService.GetAllBooksAsync();
            var firstBook = books.First();
            var bookId = firstBook.Id;

            // Act
            var result = await _bookService.GetBookByIdAsync(bookId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookId, result.Id);
            Assert.Equal(firstBook.Title, result.Title);
        }

        [Fact]
        public async Task GetBookByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var result = await _bookService.GetBookByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region SearchBooks Tests

        [Fact]
        public async Task SearchBooksAsync_WithValidSearchTerm_ShouldReturnMatchingBooks()
        {
            // Arrange
            var searchTerm = "1984";

            // Act
            var results = await _bookService.SearchBooksAsync(searchTerm);

            // Assert
            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.All(results, book =>
                Assert.True(
                    book.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                )
            );
        }

        [Fact]
        public async Task SearchBooksAsync_WithEmptySearchTerm_ShouldReturnAllBooks()
        {
            // Arrange
            var searchTerm = string.Empty;

            // Act
            var results = await _bookService.SearchBooksAsync(searchTerm);

            // Assert
            Assert.NotNull(results);
            Assert.NotEmpty(results);
        }

        [Fact]
        public async Task SearchBooksAsync_WithNonMatchingTerm_ShouldReturnEmptyList()
        {
            // Arrange
            var searchTerm = "NonExistentBookTitle123456";

            // Act
            var results = await _bookService.SearchBooksAsync(searchTerm);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        #endregion

        #region AddBook Tests

        [Fact]
        public async Task AddBookAsync_WithValidBook_ShouldAddBook()
        {
            // Arrange
            var newBook = new Book
            {
                Title = "Test Book",
                Author = "Test Author",
                YearOfPublication = 2024,
                ISBN = "978-1234567890",
                TotalCopies = 3,
                AvailableCopies = 3
            };

            // Act
            var addedBook = await _bookService.AddBookAsync(newBook);

            // Assert
            Assert.NotNull(addedBook);
            Assert.NotEqual(Guid.Empty, addedBook.Id);
            Assert.Equal(newBook.Title, addedBook.Title);
            Assert.Equal(newBook.Author, addedBook.Author);
        }

        [Fact]
        public async Task AddBookAsync_ShouldGenerateUniqueId()
        {
            // Arrange
            var book1 = new Book
            {
                Title = "Book 1",
                Author = "Author 1",
                TotalCopies = 1
            };
            var book2 = new Book
            {
                Title = "Book 2",
                Author = "Author 2",
                TotalCopies = 1
            };

            // Act
            var added1 = await _bookService.AddBookAsync(book1);
            var added2 = await _bookService.AddBookAsync(book2);

            // Assert
            Assert.NotEqual(added1.Id, added2.Id);
        }

        #endregion

        #region BorrowBook Tests

        [Fact]
        public async Task BorrowBookAsync_WithAvailableBook_ShouldSucceed()
        {
            // Arrange
            var books = await _bookService.GetAllBooksAsync();
            var availableBook = books.First(b => b.AvailableCopies > 0);
            var initialAvailable = availableBook.AvailableCopies;

            // Act
            var result = await _bookService.BorrowBookAsync(availableBook.Id, "John Doe");

            // Assert
            Assert.True(result);
            var updatedBook = await _bookService.GetBookByIdAsync(availableBook.Id);
            Assert.Equal(initialAvailable - 1, updatedBook?.AvailableCopies);
        }

        [Fact]
        public async Task BorrowBookAsync_WithInvalidBookId_ShouldFail()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var result = await _bookService.BorrowBookAsync(invalidId, "John Doe");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task BorrowBookAsync_ShouldCreateBorrowRecord()
        {
            // Arrange
            var books = await _bookService.GetAllBooksAsync();
            var book = books.First(b => b.AvailableCopies > 0);
            var userName = "Test User";

            // Act
            await _bookService.BorrowBookAsync(book.Id, userName);

            // Assert
            var updatedBook = await _bookService.GetBookByIdAsync(book.Id);
            Assert.NotNull(updatedBook);
            Assert.Contains(updatedBook.BorrowHistory, record =>
                record.User == userName && record.ReturnDate == null
            );
        }

        #endregion

        #region ReturnBook Tests

        [Fact]
        public async Task ReturnBookAsync_WithBorrowedBook_ShouldSucceed()
        {
            // Arrange
            var books = await _bookService.GetAllBooksAsync();
            var book = books.First(b => b.AvailableCopies > 0);
            await _bookService.BorrowBookAsync(book.Id, "Test User");
            var initialAvailable = (await _bookService.GetBookByIdAsync(book.Id))?.AvailableCopies ?? 0;

            // Act
            var result = await _bookService.ReturnBookAsync(book.Id);

            // Assert
            Assert.True(result);
            var updatedBook = await _bookService.GetBookByIdAsync(book.Id);
            Assert.Equal(initialAvailable + 1, updatedBook?.AvailableCopies);
        }

        [Fact]
        public async Task ReturnBookAsync_WithInvalidBookId_ShouldFail()
        {
            // Arrange
            var invalidId = Guid.NewGuid();

            // Act
            var result = await _bookService.ReturnBookAsync(invalidId);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region GetBorrowedBooks Tests

        [Fact]
        public async Task GetBorrowedBooksAsync_ShouldReturnList()
        {
            // Arrange
            // Act
            var borrowedBooks = await _bookService.GetBorrowedBooksAsync();

            // Assert
            Assert.NotNull(borrowedBooks);
            Assert.IsType<List<BorrowedBookInfo>>(borrowedBooks);
        }

        [Fact]
        public async Task GetBorrowedBooksAsync_AfterBorrow_ShouldIncludeBook()
        {
            // Arrange
            var books = await _bookService.GetAllBooksAsync();
            var book = books.First(b => b.AvailableCopies > 0);
            var userName = "Borrower";

            // Act
            await _bookService.BorrowBookAsync(book.Id, userName);
            var borrowedBooks = await _bookService.GetBorrowedBooksAsync();

            // Assert
            Assert.Contains(borrowedBooks, b =>
                b.BookId == book.Id && b.UserName == userName
            );
        }

        #endregion
    }
}
