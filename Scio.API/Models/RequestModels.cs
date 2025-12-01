using System.ComponentModel.DataAnnotations;

namespace Scio.API.Models
{
    /// <summary>
    /// Request model for adding a new book
    /// </summary>
    public class AddBookRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(500, MinimumLength = 1,
            ErrorMessage = "Title must be between 1 and 500 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-\.\,'&:;!?()]*$",
            ErrorMessage = "Title contains invalid characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author is required")]
        [StringLength(200, MinimumLength = 1,
            ErrorMessage = "Author must be between 1 and 200 characters")]
        [RegularExpression(@"^[a-zA-Z\s\-\.']*$",
            ErrorMessage = "Author name contains invalid characters")]
        public string Author { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Year must be 50 characters or less")]
        [RegularExpression(@"^\d{0,4}$",
            ErrorMessage = "Year must be a valid number")]
        public string? YearOfPublication { get; set; }

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(17, MinimumLength = 10,
            ErrorMessage = "ISBN must be between 10 and 17 characters")]
        [RegularExpression(@"^[0-9\-]*$",
            ErrorMessage = "ISBN can only contain numbers and hyphens")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Total copies is required")]
        [Range(1, 10000, ErrorMessage = "Total copies must be between 1 and 10000")]
        public int TotalCopies { get; set; }
    }

    /// <summary>
    /// Request model for borrowing a book
    /// </summary>
    public class BorrowRequest
    {
        [Required(ErrorMessage = "User name is required")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "User name must be between 2 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s\-\.]*$",
            ErrorMessage = "User name can only contain letters, spaces, hyphens, and periods")]
        public string UserName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for searching books
    /// </summary>
    public class SearchRequest
    {
        [StringLength(500, ErrorMessage = "Search term must be 500 characters or less")]
        public string? SearchTerm { get; set; }
    }

    /// <summary>
    /// Request model for returning a borrow record
    /// </summary>
    public class ReturnRequest
    {
        [Required(ErrorMessage = "Borrow record ID is required")]
        public Guid BorrowRecordId { get; set; }
    }
}
