namespace Scio.API.Models
{
    public class Book
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int YearOfPublication { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public int TotalCopies { get; set; } // Total physical copies
        public int AvailableCopies { get; set; } // Calculated: TotalCopies - ActiveBorrows
        public List<BorrowRecord> BorrowHistory { get; set; } = new();
    }
}
