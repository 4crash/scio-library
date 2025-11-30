namespace Scio.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int YearOfPublication { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public List<BorrowRecord> BorrowHistory { get; set; } = new();
    }
}
