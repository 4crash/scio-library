namespace Scio.Models
{
    /// <summary>
    /// Request model for adding a new book
    /// </summary>
    public class AddBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? YearOfPublication { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
    }

    /// <summary>
    /// Request model for borrowing a book
    /// </summary>
    public class BorrowRequest
    {
        public string UserName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for searching books
    /// </summary>
    public class SearchRequest
    {
        public string? SearchTerm { get; set; }
    }

    /// <summary>
    /// Request model for returning a borrow record
    /// </summary>
    public class ReturnRequest
    {
        public Guid BorrowRecordId { get; set; }
    }
}
