using Library.Core;
using Library.Core.Model;
using Library.Core.Model.Front.Author;
using Library.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Library
{
    public class AuthorService : IAuthorService
    {
        private readonly LibraryDbContext _context;

        public AuthorService(LibraryDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<IEnumerable<AuthorResponse>> GetAuthors(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
        {
            try
            {
                var authors = await _context.Authors
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(author => author.Books)
                    .ToListAsync(ct);

                var authorResponses = authors.Select(a => new AuthorResponse(a));

                return authorResponses;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while getting authors. Please try again.", ex);
            }
        }

        public async Task<AuthorResponse> GetAuthorById(long id, CancellationToken ct = default)
        {
            try
            {
                var author = await _context.Authors
                    .Include(author => author.Books)
                    .FirstOrDefaultAsync(author => author.Id == id, ct);
                if (author is null) return null;

                return new AuthorResponse(author);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while getting the author. Please try again.", ex);
            }
        }

        public async Task<AuthorResponse> CreateAuthor(AuthorDetails authorDetails, CancellationToken ct = default)
        {
            try
            {
                var author = new Author
                {
                    FirstName = authorDetails.FirstName,
                    MiddleName = authorDetails.MiddleName,
                    LastName = authorDetails.LastName
                };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync(ct);
                return new AuthorResponse(author);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while creating the author. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while creating the author. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating the author. Please try again.", ex);
            }
        }

        public async Task<AuthorResponse> UpdateAuthor(long id, AuthorDetails authorDetails, CancellationToken ct = default)
        {
            var existingAuthor = await _context.Authors.FindAsync(id, ct);
            if (existingAuthor is null)
                return null;

            if (authorDetails.FirstName is not null)
                existingAuthor.FirstName = authorDetails.FirstName;
            if (authorDetails.LastName is not null)
                existingAuthor.LastName = authorDetails.LastName;

            try
            {
                _context.Authors.Update(existingAuthor);

                await _context.SaveChangesAsync(ct);
                return new AuthorResponse(existingAuthor);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while updating the author. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while updating the author. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating the author. Please try again.", ex);
            }
        }

        public async Task<bool> DeleteAuthor(long id, CancellationToken ct = default)
        {
            var author = await _context.Authors.FindAsync(id, ct);
            if (author is null)
                return false;

            try
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while deleting the author. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while deleting the author. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting the author. Please try again.", ex);
            }
        }
        #endregion

        public async Task<AuthorSearchResponse> SearchAuthors(AuthorDetails author, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Authors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(author.FirstName))
                query = query.Where(a => EF.Functions.Like(a.FirstName, $"%{author.FirstName}%"));
            if (!string.IsNullOrWhiteSpace(author.MiddleName))
                query = query.Where(a => EF.Functions.Like(a.MiddleName, $"%{author.MiddleName}%"));
            if (!string.IsNullOrWhiteSpace(author.LastName))
                query = query.Where(a => EF.Functions.Like(a.LastName, $"%{author.LastName}%"));

            try
            {
                var totalCount = await query.CountAsync(ct);

                var authors = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(author => author.Books)
                    .ToListAsync(ct);

                var authorResponses = authors.Select(a => new AuthorResponse(a));

                return new AuthorSearchResponse(authorResponses, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while searching authors. Please try again.", ex);
            }
        }
    }
}
