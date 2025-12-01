using Microsoft.AspNetCore.Mvc;
using Scio.API.Models;
using Scio.API.Services;

namespace Scio.API.Controllers
{
    public class BorrowedBookInfo
    {
        public Guid BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public Guid BorrowRecordId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime BorrowDate { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        // GET /api/book - Get all books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        // GET /api/book/{id} - Get book by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }

        // GET /api/book/search?term={searchTerm} - Search books
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] SearchRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var searchTerm = string.IsNullOrWhiteSpace(request?.SearchTerm)
                ? string.Empty
                : request.SearchTerm.Trim();

            var books = await _bookService.SearchBooksAsync(searchTerm);
            return Ok(books);
        }

        // POST /api/book - Add new book
        [HttpPost]
        public async Task<ActionResult<Book>> AddBook([FromBody] AddBookRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request == null)
                return BadRequest("Book data is required");

            // Trim string inputs for security
            var yearOfPublication = 0;
            if (!string.IsNullOrWhiteSpace(request.YearOfPublication))
            {
                int.TryParse(request.YearOfPublication.Trim(), out yearOfPublication);
            }

            var book = new Book
            {
                Title = request.Title.Trim(),
                Author = request.Author.Trim(),
                YearOfPublication = yearOfPublication,
                ISBN = request.ISBN.Trim(),
                TotalCopies = request.TotalCopies
            };

            var addedBook = await _bookService.AddBookAsync(book);
            return CreatedAtAction(nameof(GetBookById), new { id = addedBook.Id }, addedBook);
        }

        // POST /api/book/{id}/borrow - Borrow a book
        [HttpPost("{id}/borrow")]
        public async Task<IActionResult> BorrowBook(Guid id, [FromBody] BorrowRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request == null)
                return BadRequest("Request data is required");

            var success = await _bookService.BorrowBookAsync(id, request.UserName.Trim());
            if (!success)
                return BadRequest("Book not available or not found");

            return Ok("Book borrowed successfully");
        }

        // POST /api/book/{id}/return - Return a book
        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(Guid id)
        {
            var success = await _bookService.ReturnBookAsync(id);
            if (!success)
                return BadRequest("Book not found");

            return Ok("Book returned successfully");
        }

        // POST /api/book/return-record/{borrowRecordId} - Return a specific borrow record
        [HttpPost("return-record/{borrowRecordId}")]
        public async Task<IActionResult> ReturnBorrowRecord(Guid borrowRecordId)
        {
            var success = await _bookService.ReturnBorrowRecordAsync(borrowRecordId);
            if (!success)
                return BadRequest("Borrow record not found or already returned");

            return Ok("Book returned successfully");
        }

        // GET /api/book/borrowed - Get all currently borrowed books
        [HttpGet("borrowed")]
        public async Task<ActionResult<IEnumerable<BorrowedBookInfo>>> GetBorrowedBooks()
        {
            var borrowedBooks = await _bookService.GetBorrowedBooksAsync();
            return Ok(borrowedBooks);
        }
    }
}
