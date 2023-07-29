using Library.Core;
using Library.Core.Model.Front.Author;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    /// <summary>
    /// The Author Controller provides CRUD operations and a search endpoint for authors.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        /// <summary>
        /// Gets a list of authors with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
        /// <param name="pageSize">The number of authors to retrieve per page. Default is 10.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of authors.</returns>
        [HttpGet("author")]
        public async Task<ActionResult<IEnumerable<AuthorResponse>>> GetAuthors(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination parameters. PageNumber and PageSize must both be greater than 0.");

            var authors = await _authorService.GetAuthors(pageNumber, pageSize, cancellationToken);

            if (authors is null)
                return NotFound();

            return Ok(authors);
        }

        /// <summary>
        /// Gets a specific author by their ID.
        /// </summary>
        /// <param name="id">The ID of the author to retrieve.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The requested author if found.</returns>
        [HttpGet("author/{id}")]
        public async Task<ActionResult<AuthorResponse>> GetAuthor(long id, CancellationToken cancellationToken = default)
        {
            var author = await _authorService.GetAuthorById(id, cancellationToken);

            if (author is null)
                return NotFound();

            return Ok(author);
        }

        /// <summary>
        /// Creates a new author.
        /// </summary>
        /// <param name="author">Details of the author to create.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The created author.</returns>
        [HttpPost("author/")]
        public async Task<ActionResult<AuthorResponse>> CreateAuthor(AuthorDetails author, CancellationToken cancellationToken = default)
        {
            var createdAuthor = await _authorService.CreateAuthor(author, cancellationToken);
            return CreatedAtAction(
                nameof(GetAuthor),
                new { id = createdAuthor.Id },
                createdAuthor);
        }

        /// <summary>
        /// Updates a specific author by their ID.
        /// </summary>
        /// <param name="id">The ID of the author to update.</param>
        /// <param name="author">Details of the author to update.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The updated author if found.</returns>
        [HttpPut("author/{id}")]
        public async Task<ActionResult<AuthorResponse>> UpdateAuthor(long id, AuthorDetails author, CancellationToken cancellationToken = default)
        {
            var updatedAuthor = await _authorService.UpdateAuthor(id, author, cancellationToken);

            if (updatedAuthor is null)
                return NotFound();

            return Ok(updatedAuthor);
        }

        /// <summary>
        /// Deletes a specific author by their ID.
        /// </summary>
        /// <param name="id">The ID of the author to delete.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>No content if deletion was successful.</returns>
        [HttpDelete("author/{id}")]
        public async Task<IActionResult> DeleteAuthor(long id, CancellationToken cancellationToken = default)
        {
            var result = await _authorService.DeleteAuthor(id, cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Searches for authors based on specified criteria.
        /// </summary>
        /// <param name="author">Details of the author to search for.</param>
        /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
        /// <param name="pageSize">The number of authors to retrieve per page. Default is 10.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of authors that match the criteria with total count of available results.</returns>
        [HttpPost("author/search")]
        public async Task<ActionResult<AuthorSearchResponse>> SearchBooks([FromBody] AuthorDetails author, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination parameters. PageNumber and PageSize must both be greater than 0.");

            var result = await _authorService.SearchAuthors(author, pageNumber, pageSize, cancellationToken);

            if (result?.Authors is null || !result.Authors.Any())
                return NotFound();

            return Ok(result);
        }
    }
}
