namespace Scio.API.Models
{
    public class BorrowedBookInfo
    {
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public Guid BorrowRecordId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
