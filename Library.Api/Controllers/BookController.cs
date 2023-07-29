using Library.Core;
using Library.Core.Model.Front.Book;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    /// <summary>
    /// Provides CRUD operations for books, including lending and returning books, searching for books, and inverting words in a book title.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Retrieves a list of books with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
        /// <param name="pageSize">The number of books to retrieve per page. Default is 10.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of books.</returns>
        [HttpGet("book")]
        public async Task<ActionResult<IEnumerable<BookResponse>>> GetBooks(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination parameters. PageNumber and PageSize must both be greater than 0.");

            var books = await _bookService.GetBooks(pageNumber, pageSize, cancellationToken);

            if (books is null)
                return NotFound();

            return Ok(books);
        }

        /// <summary>
        /// Retrieves a specific book by their ID.
        /// </summary>
        /// <param name="id">The ID of the book to retrieve.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The requested book if found.</returns>
        [HttpGet("book/{id}")]
        public async Task<ActionResult<BookResponse>> GetBook(long id, CancellationToken cancellationToken = default)
        {
            var book = await _bookService.GetBookById(id, cancellationToken);

            if (book is null)
                return NotFound();

            return Ok(book);
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="book">Details of the book to create.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The created book.</returns>
        [HttpPost("book/")]
        public async Task<ActionResult<BookResponse>> CreateBook(BookDetails book, CancellationToken cancellationToken = default)
        {
            var createdBook = await _bookService.CreateBook(book, cancellationToken);
            return CreatedAtAction(
                nameof(GetBook),
                new { id = createdBook.Id },
                createdBook);
        }

        /// <summary>
        /// Updates a specific book by their ID.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="book">Details of the book to update.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The updated book if found.</returns>
        [HttpPut("book/{id}")]
        public async Task<ActionResult<BookResponse>> UpdateBook(long id, BookDetails book, CancellationToken cancellationToken = default)
        {
            var updatedBook = await _bookService.UpdateBook(id, book, cancellationToken);

            if (updatedBook is null)
                return NotFound();

            return Ok(updatedBook);
        }

        /// <summary>
        /// Deletes a specific book by their ID.
        /// </summary>
        /// <param name="id">The ID of the book to delete.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>No content if deletion was successful.</returns>
        [HttpDelete("book/{id}")]
        public async Task<IActionResult> DeleteBook(long id, CancellationToken cancellationToken = default)
        {
            var result = await _bookService.DeleteBook(id, cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Lends a specific book to a user.
        /// </summary>
        /// <param name="bookId">The ID of the book to lend.</param>
        /// <param name="userId">The ID of the user who is borrowing the book.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>OK status if the book was successfully lent.</returns>
        [HttpPost("book/lend/{bookId}")]
        public async Task<ActionResult> LendBook(long bookId, long userId, CancellationToken cancellationToken = default)
        {
            await _bookService.LendBook(bookId, userId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Returns a specific book from a user.
        /// </summary>
        /// <param name="bookId">The ID of the book to return.</param>
        /// <param name="userId">The ID of the user who is returning the book.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>OK status if the book was successfully returned.</returns>
        [HttpPost("book/return/{bookId}")]
        public async Task<ActionResult> ReturnBook(long bookId, long userId, CancellationToken cancellationToken = default)
        {
            await _bookService.ReturnBook(bookId, userId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Searches for books based on provided search criteria.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <param name="pageNumber">The page number for pagination. Default is 1.</param>
        /// <param name="pageSize">The number of search results per page. Default is 10.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The search results if any matches were found with total count of available results.</returns>
        [HttpPost("search")]
        public async Task<ActionResult<BookSearchResponse>> SearchBooks([FromBody] BookSearchCriteria criteria, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination parameters. PageNumber and PageSize must both be greater than 0.");

            var result = await _bookService.SearchBooks(criteria, pageNumber, pageSize, cancellationToken);

            if (result?.Books is null || !result.Books.Any())
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Inverts the words in a specific book's title.
        /// </summary>
        /// <param name="id">The ID of the book to modify.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The modified book if found.</returns>
        [HttpPut("invertwords/{id}")]
        public async Task<ActionResult<BookResponse>> InvertWordsInTitle(long id, CancellationToken cancellationToken = default)
        {
            var book = await _bookService.InvertWordsInTitle(id, cancellationToken);
            if (book is null)
                return NotFound();

            return Ok(book);
        }
    }
}
