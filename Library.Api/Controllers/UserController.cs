using Library.Core;
using Library.Core.Model.Front.User;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    /// <summary>
    /// Provides CRUD operations for users, as well as a user report generation and a user search endpoint.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves a list of users with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
        /// <param name="pageSize">The number of users to retrieve per page. Default is 10.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of users.</returns>
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetUsers(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination parameters. PageNumber and PageSize must both be greater than 0.");

            var users = await _userService.GetUsers(pageNumber, pageSize, cancellationToken);

            if (users is null)
                return NotFound();

            return Ok(users);
        }

        /// <summary>
        /// Retrieves a specific user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The requested user if found.</returns>
        [HttpGet("user/{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(long id, CancellationToken cancellationToken = default)
        {
            var user = await _userService.GetUserById(id, cancellationToken);

            if (user is null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">Details of the user to create.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The created user.</returns>
        [HttpPost("user/")]
        public async Task<ActionResult<UserResponse>> CreateUser(UserDetails user, CancellationToken cancellationToken = default)
        {
            var newUser = await _userService.CreateUser(user, cancellationToken);
            return CreatedAtAction(
                nameof(GetUser),
                new { id = newUser.Id },
                newUser);
        }

        /// <summary>
        /// Updates a specific user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to update.</param>
        /// <param name="user">Details of the user to update.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The updated user if found.</returns>
        [HttpPut("user/{id}")]
        public async Task<ActionResult<UserResponse>> UpdateUser(long id, UserDetails user, CancellationToken cancellationToken = default)
        {
            var updatedUser = await _userService.UpdateUser(id, user, cancellationToken);

            if (updatedUser is null)
                return NotFound();

            return Ok(updatedUser);
        }

        /// <summary>
        /// Deletes a specific user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>No content if deletion was successful.</returns>
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(long id, CancellationToken cancellationToken = default)
        {
            var result = await _userService.DeleteUser(id, cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Generates a report of users with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
        /// <param name="pageSize">The number of users to retrieve per page. Default is 10.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A users report with total count of available reports.</returns>
        [HttpGet("user/report")]
        public async Task<ActionResult<UserReportResponse>> GenerateReport(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination parameters. PageNumber and PageSize must both be greater than 0.");

            var result = await _userService.GenerateReport(pageNumber, pageSize, cancellationToken);

            if (result.Reports is null || !result.Reports.Any())
                return NotFound("No report data available for the given page.");

            return Ok(result);
        }

        /// <summary>
        /// Searches for users based on specified criteria.
        /// </summary>
        /// <param name="user">Details of the user to search for.</param>
        /// <param name="pageNumber">The page number to retrieve. Default is 1.</param>
        /// <param name="pageSize">The number of users to retrieve per page. Default is 10.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of users that match the criteri with total count of available results.</returns>
        [HttpPost("user/search")]
        public async Task<ActionResult<UserSearchResponse>> SearchUsers([FromBody] UserDetails user, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pagination parameters. PageNumber and PageSize must both be greater than 0.");

            var result = await _userService.SearchUsers(user, pageNumber, pageSize, cancellationToken);

            if (result?.Users is null || !result.Users.Any())
                return NotFound();

            return Ok(result);
        }
    }
}
