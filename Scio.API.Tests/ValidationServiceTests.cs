using Xunit;
using Scio.API.Models;

namespace Scio.API.Tests
{
    public class ValidationServiceTests
    {
        private readonly ValidationService _validationService;

        public ValidationServiceTests()
        {
            _validationService = new ValidationService();
        }

        #region AddBookRequest Validation Tests

        [Fact]
        public void ValidateAddBookRequest_WithValidRequest_ShouldSucceed()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Valid Book",
                Author = "Valid Author",
                ISBN = "978-0451524935",
                YearOfPublication = "2024",
                TotalCopies = 5
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateAddBookRequest_WithNullRequest_ShouldFail()
        {
            // Act
            var result = _validationService.ValidateAddBookRequest(null);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("required", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ValidateAddBookRequest_WithEmptyTitle_ShouldFail()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "",
                Author = "Valid Author",
                TotalCopies = 5
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Title", result.ErrorMessage);
        }

        [Fact]
        public void ValidateAddBookRequest_WithTitleExceedingMaxLength_ShouldFail()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = new string('a', 257),
                Author = "Valid Author",
                TotalCopies = 5
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("256", result.ErrorMessage);
        }

        [Fact]
        public void ValidateAddBookRequest_WithInvalidISBN_ShouldFail()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Valid Book",
                Author = "Valid Author",
                ISBN = "INVALID-ISBN-FORMAT",
                TotalCopies = 5
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("ISBN", result.ErrorMessage);
        }

        [Fact]
        public void ValidateAddBookRequest_WithValidISBN10_ShouldSucceed()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Valid Book",
                Author = "Valid Author",
                ISBN = "0451524934",
                TotalCopies = 5
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateAddBookRequest_WithValidISBN13_ShouldSucceed()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Valid Book",
                Author = "Valid Author",
                ISBN = "978-0-451-52493-5",
                TotalCopies = 5
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateAddBookRequest_WithEmptyISBN_ShouldSucceed()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Valid Book",
                Author = "Valid Author",
                ISBN = "",
                TotalCopies = 5
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateAddBookRequest_WithInvalidYear_ShouldFail()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Valid Book",
                Author = "Valid Author",
                YearOfPublication = "not-a-year",
                TotalCopies = 5
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("valid number", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ValidateAddBookRequest_WithYearTooHigh_ShouldFail()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Valid Book",
                Author = "Valid Author",
                YearOfPublication = "2999",
                TotalCopies = 5
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidateAddBookRequest_WithNegativeTotalCopies_ShouldFail()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Valid Book",
                Author = "Valid Author",
                TotalCopies = 0
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("1", result.ErrorMessage);
        }

        [Fact]
        public void ValidateAddBookRequest_WithExcessiveTotalCopies_ShouldFail()
        {
            // Arrange
            var request = new AddBookRequest
            {
                Title = "Valid Book",
                Author = "Valid Author",
                TotalCopies = 1000
            };

            // Act
            var result = _validationService.ValidateAddBookRequest(request);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("999", result.ErrorMessage);
        }

        #endregion

        #region BorrowRequest Validation Tests

        [Fact]
        public void ValidateBorrowRequest_WithValidRequest_ShouldSucceed()
        {
            // Arrange
            var request = new BorrowRequest { UserName = "John Doe" };

            // Act
            var result = _validationService.ValidateBorrowRequest(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateBorrowRequest_WithNullRequest_ShouldFail()
        {
            // Act
            var result = _validationService.ValidateBorrowRequest(null);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidateBorrowRequest_WithEmptyUserName_ShouldFail()
        {
            // Arrange
            var request = new BorrowRequest { UserName = "" };

            // Act
            var result = _validationService.ValidateBorrowRequest(request);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public void ValidateBorrowRequest_WithUserNameExceedingMaxLength_ShouldFail()
        {
            // Arrange
            var request = new BorrowRequest { UserName = new string('a', 257) };

            // Act
            var result = _validationService.ValidateBorrowRequest(request);

            // Assert
            Assert.False(result.IsValid);
        }

        #endregion

        #region SearchRequest Validation Tests

        [Fact]
        public void ValidateSearchRequest_WithValidTerm_ShouldSucceed()
        {
            // Arrange
            var request = new SearchRequest { SearchTerm = "Test" };

            // Act
            var result = _validationService.ValidateSearchRequest(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateSearchRequest_WithNullRequest_ShouldSucceed()
        {
            // Act
            var result = _validationService.ValidateSearchRequest(null);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateSearchRequest_WithEmptyRequest_ShouldSucceed()
        {
            // Arrange
            var request = new SearchRequest { SearchTerm = "" };

            // Act
            var result = _validationService.ValidateSearchRequest(request);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidateSearchRequest_WithSearchTermExceedingMaxLength_ShouldFail()
        {
            // Arrange
            var request = new SearchRequest { SearchTerm = new string('a', 101) };

            // Act
            var result = _validationService.ValidateSearchRequest(request);

            // Assert
            Assert.False(result.IsValid);
        }

        #endregion
    }
}
