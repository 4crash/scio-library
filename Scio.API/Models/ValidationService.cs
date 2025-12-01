using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Scio.API.Models
{
    /// <summary>
    /// Input validation service for book-related requests
    /// </summary>
    public interface IValidationService
    {
        ValidationResult ValidateAddBookRequest(AddBookRequest request);
        ValidationResult ValidateBorrowRequest(BorrowRequest request);
        ValidationResult ValidateSearchRequest(SearchRequest request);
    }

    public class ValidationService : IValidationService
    {
        private const int MaxTitleLength = 256;
        private const int MaxAuthorLength = 256;
        private const int MaxISBNLength = 20;
        private const int MaxUserNameLength = 256;
        private const int MaxSearchTermLength = 100;
        private const int MinTitleLength = 1;
        private const int MinAuthorLength = 1;
        private const int MaxTotalCopies = 999;
        private const int MinTotalCopies = 1;

        /// <summary>
        /// Validates AddBookRequest input
        /// </summary>
        public ValidationResult ValidateAddBookRequest(AddBookRequest request)
        {
            if (request == null)
                return new ValidationResult("Book data is required", new[] { nameof(request) });

            // Title validation
            if (string.IsNullOrWhiteSpace(request.Title))
                return new ValidationResult("Title is required", new[] { nameof(request.Title) });

            if (request.Title.Length < MinTitleLength || request.Title.Length > MaxTitleLength)
                return new ValidationResult($"Title must be {MinTitleLength}-{MaxTitleLength} characters",
                    new[] { nameof(request.Title) });

            // Author validation
            if (string.IsNullOrWhiteSpace(request.Author))
                return new ValidationResult("Author is required", new[] { nameof(request.Author) });

            if (request.Author.Length < MinAuthorLength || request.Author.Length > MaxAuthorLength)
                return new ValidationResult($"Author must be {MinAuthorLength}-{MaxAuthorLength} characters",
                    new[] { nameof(request.Author) });

            // ISBN validation
            if (!string.IsNullOrWhiteSpace(request.ISBN))
            {
                if (request.ISBN.Length > MaxISBNLength)
                    return new ValidationResult($"ISBN must not exceed {MaxISBNLength} characters",
                        new[] { nameof(request.ISBN) });

                if (!IsValidISBN(request.ISBN))
                    return new ValidationResult("ISBN format is invalid. Use ISBN-10, ISBN-13, or similar format",
                        new[] { nameof(request.ISBN) });
            }

            // Year of Publication validation
            if (!string.IsNullOrWhiteSpace(request.YearOfPublication))
            {
                if (!int.TryParse(request.YearOfPublication.Trim(), out var year))
                    return new ValidationResult("Year of Publication must be a valid number",
                        new[] { nameof(request.YearOfPublication) });

                var currentYear = DateTime.UtcNow.Year;
                if (year < 1000 || year > currentYear)
                    return new ValidationResult($"Year of Publication must be between 1000 and {currentYear}",
                        new[] { nameof(request.YearOfPublication) });
            }

            // Total Copies validation
            if (request.TotalCopies < MinTotalCopies || request.TotalCopies > MaxTotalCopies)
                return new ValidationResult($"Total Copies must be between {MinTotalCopies} and {MaxTotalCopies}",
                    new[] { nameof(request.TotalCopies) });

            return new ValidationResult();
        }

        /// <summary>
        /// Validates BorrowRequest input
        /// </summary>
        public ValidationResult ValidateBorrowRequest(BorrowRequest request)
        {
            if (request == null)
                return new ValidationResult("Request data is required", new[] { nameof(request) });

            if (string.IsNullOrWhiteSpace(request.UserName))
                return new ValidationResult("User name is required", new[] { nameof(request.UserName) });

            if (request.UserName.Length < 1 || request.UserName.Length > MaxUserNameLength)
                return new ValidationResult($"User name must be 1-{MaxUserNameLength} characters",
                    new[] { nameof(request.UserName) });

            return new ValidationResult();
        }

        /// <summary>
        /// Validates SearchRequest input
        /// </summary>
        public ValidationResult ValidateSearchRequest(SearchRequest request)
        {
            if (request == null)
                return new ValidationResult();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                if (request.SearchTerm.Length > MaxSearchTermLength)
                    return new ValidationResult($"Search term must not exceed {MaxSearchTermLength} characters",
                        new[] { nameof(request.SearchTerm) });
            }

            return new ValidationResult();
        }

        /// <summary>
        /// Validates ISBN format (supports ISBN-10, ISBN-13, and variations with hyphens/spaces)
        /// </summary>
        private bool IsValidISBN(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
                return true; // Empty ISBN is allowed

            // Remove hyphens and spaces
            var cleanISBN = Regex.Replace(isbn.Trim(), @"[\s\-]", "");

            // ISBN-10: 10 digits
            if (cleanISBN.Length == 10)
            {
                return Regex.IsMatch(cleanISBN, @"^\d{10}$");
            }

            // ISBN-13: 13 digits starting with 978 or 979
            if (cleanISBN.Length == 13)
            {
                return Regex.IsMatch(cleanISBN, @"^(978|979)\d{10}$");
            }

            return false;
        }
    }

    /// <summary>
    /// Validation result representing success or failure of validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string? ErrorMessage { get; }
        public string[]? MemberNames { get; }

        public ValidationResult()
        {
            IsValid = true;
            ErrorMessage = null;
            MemberNames = null;
        }

        public ValidationResult(string errorMessage, string[]? memberNames = null)
        {
            IsValid = false;
            ErrorMessage = errorMessage;
            MemberNames = memberNames;
        }
    }
}
