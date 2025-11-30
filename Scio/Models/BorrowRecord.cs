namespace Scio.Models
{
    public class BorrowRecord
    {
        public Guid Id { get; set; }
        public string User { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}