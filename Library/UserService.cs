using Library.Core;
using Library.Core.Model;
using Library.Core.Model.Front.User;
using Library.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Library
{
    public class UserService : IUserService
    {
        private readonly LibraryDbContext _context;

        public UserService(LibraryDbContext context)
        {
            _context = context;
        }
        #region CRUD
        public async Task<IEnumerable<UserResponse>> GetUsers(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
        {
            try
            {
                var users = await _context.Users
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(user => user.BooksTaken)
                    .ToListAsync(ct);

                var userResponses = users.Select(u => new UserResponse(u));

                return userResponses;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while getting the users. Please try again.", ex);
            }
        }

        public async Task<UserResponse> GetUserById(long id, CancellationToken ct = default)
        {
            try
            {
                var user = await _context.Users
                    .Include(user => user.BooksTaken)
                    .FirstOrDefaultAsync(user => user.Id == id, ct);
                if (user is null) return null;

                return new UserResponse(user);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while getting the user. Please try again.", ex);
            }
        }

        public async Task<UserResponse> CreateUser(UserDetails userDetails, CancellationToken ct = default)
        {
            try
            {
                var user = new User
                {
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Email = userDetails.Email
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync(ct);
                return new UserResponse(user);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while creating the user. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while creating the user. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating the user. Please try again.", ex);
            }
        }

        public async Task<UserResponse> UpdateUser(long id, UserDetails userDetails, CancellationToken ct = default)
        {
            var existingUser = await _context.Users.FindAsync(id, ct);
            if (existingUser is null)
                return null;

            if (userDetails.FirstName is not null)
                existingUser.FirstName = userDetails.FirstName;
            if (userDetails.LastName is not null)
                existingUser.LastName = userDetails.LastName;
            if (userDetails.Email is not null)
                existingUser.Email = userDetails.Email;

            try
            {
                _context.Users.Update(existingUser);

                await _context.SaveChangesAsync(ct);
                return new UserResponse(existingUser);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while updating the user. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while updating the user. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating the user. Please try again.", ex);
            }
        }

        public async Task<bool> DeleteUser(long id, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(id, ct);
            if (user is null)
                return false;

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while deleting the user. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while deleting the user. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting the user. Please try again.", ex);
            }
        }
        #endregion
        #region Report
        public async Task<UserReportResponse> GenerateReport(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                var totalCount = await query.CountAsync(ct);

                var reports = await query
                    .OrderBy(u => u.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(user => new
                    {
                        User = user,
                        TotalBooks = user.BooksTaken.Count(),
                        BookTakenDates = user.BooksTaken.Select(bt => bt.DateTaken).ToList()
                    })
                    .ToListAsync(ct);

                var userReports = reports.Select(r =>
                new UserReport(
                    r.User,
                    r.TotalBooks,
                    r.BookTakenDates.Sum(dateTaken => (DateTime.UtcNow.Date - dateTaken.Date).Days)));

                return new UserReportResponse(userReports, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while generating user report. Please try again.", ex);
            }
        }
        #endregion

        public async Task<UserSearchResponse> SearchUsers(UserDetails user, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(user.FirstName))
                query = query.Where(u => EF.Functions.Like(u.FirstName, $"%{user.FirstName}%"));
            if (!string.IsNullOrWhiteSpace(user.LastName))
                query = query.Where(u => EF.Functions.Like(u.LastName, $"%{user.LastName}%"));
            if (!string.IsNullOrWhiteSpace(user.Email))
                query = query.Where(u => EF.Functions.Like(u.Email, $"%{user.Email}%"));

            try
            {
                var totalCount = await query.CountAsync(ct);

                var users = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(user => user.BooksTaken)
                    .ToListAsync(ct);

                var userResponses = users.Select(u => new UserResponse(u));

                return new UserSearchResponse(userResponses, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while searching users. Please try again.", ex);
            }
        }
    }
}
