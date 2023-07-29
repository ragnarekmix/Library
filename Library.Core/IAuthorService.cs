using Library.Core.Model.Front.Author;

namespace Library.Core
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorResponse>> GetAuthors(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
        Task<AuthorResponse> GetAuthorById(long id, CancellationToken ct = default);
        Task<AuthorResponse> CreateAuthor(AuthorDetails author, CancellationToken ct = default);
        Task<AuthorResponse> UpdateAuthor(long id, AuthorDetails author, CancellationToken ct = default);
        Task<bool> DeleteAuthor(long id, CancellationToken ct = default);
        Task<AuthorSearchResponse> SearchAuthors(AuthorDetails author, int pageNumber, int pageSize, CancellationToken ct = default);
    }
}