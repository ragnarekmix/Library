using Library.Core.Model.Front.Book;

namespace Library.Core
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponse>> GetBooks(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default);
        Task<BookResponse> GetBookById(long id, CancellationToken ct = default);
        Task<BookResponse> CreateBook(BookDetails book, CancellationToken ct = default);
        Task<BookResponse> UpdateBook(long id, BookDetails book, CancellationToken ct = default);
        Task<bool> DeleteBook(long id, CancellationToken ct = default);
        Task<BookResponse> InvertWordsInTitle(long bookId, CancellationToken ct = default);
        Task<BookSearchResponse> SearchBooks(BookSearchCriteria criteria, int pageNumber, int pageSize, CancellationToken ct = default);
        Task LendBook(long bookId, long userId, CancellationToken ct = default);
        Task ReturnBook(long bookId, long userId, CancellationToken ct = default);
    }
}
