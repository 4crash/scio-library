namespace Scio.API.Models
{
    public class BorrowRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string User { get; set; } = string.Empty; // In real app, this would be UserId
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned => ReturnDate.HasValue;
    }
}