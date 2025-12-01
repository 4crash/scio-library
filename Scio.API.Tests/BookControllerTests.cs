using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Scio.API.Controllers;
using Scio.API.Models;
using Scio.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scio.API.Tests
{
    public class BookControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly Mock<IValidationService> _mockValidationService;
        private readonly BookController _controller;

        public BookControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _mockValidationService = new Mock<IValidationService>();
            _controller = new BookController(_mockBookService.Object, _mockValidationService.Object);
        }

        #region GetAllBooks Tests

        [Fact]
        public async Task GetAllBooks_ShouldReturnOkWithBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Title = "Book 1", Author = "Author 1" },
                new Book { Id = Guid.NewGuid(), Title = "Book 2", Author = "Author 2" }
            };
            _mockBookService.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(books);

            // Act
            var result = await _controller.GetAllBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var returnedBooks = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count);
        }

        [Fact]
        public async Task GetAllBooks_ShouldCallServiceOnce()
        {
            // Arrange
            _mockBookService.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(new List<Book>());

            // Act
            await _controller.GetAllBooks();

            // Assert
            _mockBookService.Verify(s => s.GetAllBooksAsync(), Times.Once);
        }

        #endregion

        #region GetBookById Tests

        [Fact]
        public async Task GetBookById_WithValidId_ShouldReturnOkWithBook()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var book = new Book { Id = bookId, Title = "Test Book", Author = "Test Author" };
            _mockBookService.Setup(s => s.GetBookByIdAsync(bookId)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetBookById(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var returnedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal(bookId, returnedBook.Id);
        }

        [Fact]
        public async Task GetBookById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _mockBookService.Setup(s => s.GetBookByIdAsync(bookId)).ReturnsAsync((Book?)null);

            // Act
            var result = await _controller.GetBookById(bookId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion

        #region SearchBooks Tests

        [Fact]
        public async Task SearchBooks_WithValidSearchTerm_ShouldReturnOkWithBooks()
        {
            // Arrange
            var searchTerm = "Test";
            var request = new SearchRequest { SearchTerm = searchTerm };
            var books = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Title = "Test Book 1", Author = "Author 1" },
                new Book { Id = Guid.NewGuid(), Title = "Test Book 2", Author = "Author 2" }
            };

            _mockValidationService.Setup(s => s.ValidateSearchRequest(It.IsAny<SearchRequest>()))
                .Returns(new ValidationResult());

            _mockBookService.Setup(s => s.SearchBooksAsync(searchTerm)).ReturnsAsync(books);

            // Act
            var result = await _controller.SearchBooks(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var returnedBooks = Assert.IsType<List<Book>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count);
        }

        [Fact]
        public async Task SearchBooks_WithNullRequest_ShouldReturnOkWithAllBooks()
        {
            // Arrange
            var books = new List<Book>();

            _mockValidationService.Setup(s => s.ValidateSearchRequest(It.IsAny<SearchRequest>()))
                .Returns(new ValidationResult());

            _mockBookService.Setup(s => s.SearchBooksAsync(It.IsAny<string>())).ReturnsAsync(books);

            // Act
            var result = await _controller.SearchBooks(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        #endregion

        #region AddBook Tests

        [Fact]
        public async Task AddBook_WithValidRequest_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "New Book",
                Author = "New Author",
                ISBN = "978-1234567890",
                YearOfPublication = "2024",
                TotalCopies = 5
            };
            var addedBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Author = request.Author,
                ISBN = request.ISBN,
                YearOfPublication = 2024,
                TotalCopies = 5
            };

            // Mock valid validation result
            _mockValidationService.Setup(s => s.ValidateAddBookRequest(It.IsAny<AddBookRequest>()))
                .Returns(new ValidationResult());

            _mockBookService.Setup(s => s.AddBookAsync(It.IsAny<Book>())).ReturnsAsync(addedBook);

            // Act
            var result = await _controller.AddBook(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(BookController.GetBookById), createdResult.ActionName);
            Assert.Equal(addedBook.Id, ((Book)createdResult.Value).Id);
        }

        [Fact]
        public async Task AddBook_WithNullRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var validationResult = new ValidationResult("Book data is required");
            _mockValidationService.Setup(s => s.ValidateAddBookRequest(It.IsAny<AddBookRequest>()))
                .Returns(validationResult);

            // Act
            var result = await _controller.AddBook(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AddBook_ShouldCallServiceOnce()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "New Book",
                Author = "New Author",
                ISBN = "978-1234567890",
                YearOfPublication = "2024",
                TotalCopies = 5
            };
            var addedBook = new Book { Id = Guid.NewGuid(), Title = "New Book" };

            _mockValidationService.Setup(s => s.ValidateAddBookRequest(It.IsAny<AddBookRequest>()))
                .Returns(new ValidationResult());

            _mockBookService.Setup(s => s.AddBookAsync(It.IsAny<Book>())).ReturnsAsync(addedBook);

            // Act
            await _controller.AddBook(request);

            // Assert
            _mockBookService.Verify(s => s.AddBookAsync(It.IsAny<Book>()), Times.Once);
        }

        #endregion

        #region BorrowBook Tests

        [Fact]
        public async Task BorrowBook_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new BorrowRequest { UserName = "John Doe" };

            _mockValidationService.Setup(s => s.ValidateBorrowRequest(It.IsAny<BorrowRequest>()))
                .Returns(new ValidationResult());

            _mockBookService.Setup(s => s.BorrowBookAsync(bookId, request.UserName.Trim())).ReturnsAsync(true);

            // Act
            var result = await _controller.BorrowBook(bookId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task BorrowBook_WithUnavailableBook_ShouldReturnBadRequest()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new BorrowRequest { UserName = "John Doe" };

            _mockValidationService.Setup(s => s.ValidateBorrowRequest(It.IsAny<BorrowRequest>()))
                .Returns(new ValidationResult());

            _mockBookService.Setup(s => s.BorrowBookAsync(bookId, request.UserName.Trim())).ReturnsAsync(false);

            // Act
            var result = await _controller.BorrowBook(bookId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task BorrowBook_WithInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            var request = new BorrowRequest { UserName = "" };
            var validationResult = new ValidationResult("User name is required");

            _mockValidationService.Setup(s => s.ValidateBorrowRequest(It.IsAny<BorrowRequest>()))
                .Returns(validationResult);

            // Act
            var result = await _controller.BorrowBook(bookId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        #endregion

        #region ReturnBook Tests

        [Fact]
        public async Task ReturnBook_WithValidBookId_ShouldReturnOk()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _mockBookService.Setup(s => s.ReturnBookAsync(bookId)).ReturnsAsync(true);

            // Act
            var result = await _controller.ReturnBook(bookId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ReturnBook_WithInvalidBookId_ShouldReturnBadRequest()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _mockBookService.Setup(s => s.ReturnBookAsync(bookId)).ReturnsAsync(false);

            // Act
            var result = await _controller.ReturnBook(bookId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        #endregion

        #region ReturnBorrowRecord Tests

        [Fact]
        public async Task ReturnBorrowRecord_WithValidRecordId_ShouldReturnOk()
        {
            // Arrange
            var recordId = Guid.NewGuid();
            _mockBookService.Setup(s => s.ReturnBorrowRecordAsync(recordId)).ReturnsAsync(true);

            // Act
            var result = await _controller.ReturnBorrowRecord(recordId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ReturnBorrowRecord_WithInvalidRecordId_ShouldReturnBadRequest()
        {
            // Arrange
            var recordId = Guid.NewGuid();
            _mockBookService.Setup(s => s.ReturnBorrowRecordAsync(recordId)).ReturnsAsync(false);

            // Act
            var result = await _controller.ReturnBorrowRecord(recordId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        #endregion

        #region GetBorrowedBooks Tests

        [Fact]
        public async Task GetBorrowedBooks_ShouldReturnOkWithBorrowedBooks()
        {
            // Arrange
            var borrowedBooks = new List<Models.BorrowedBookInfo>
            {
                new Models.BorrowedBookInfo { BookId = Guid.NewGuid(), BookTitle = "Book 1", UserName = "User 1" },
                new Models.BorrowedBookInfo { BookId = Guid.NewGuid(), BookTitle = "Book 2", UserName = "User 2" }
            };
            _mockBookService.Setup(s => s.GetBorrowedBooksAsync()).ReturnsAsync(borrowedBooks);

            // Act
            var result = await _controller.GetBorrowedBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var returnedBooks = Assert.IsType<List<Models.BorrowedBookInfo>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count);
        }

        #endregion
    }
}
