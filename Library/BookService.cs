using Library.Core;
using Library.Core.Model;
using Library.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;
using Library.Core.Model.Front.Book;

namespace Library
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _context;

        public BookService(LibraryDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<IEnumerable<BookResponse>> GetBooks(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
        {
            try
            {
                var books = await _context.Books
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(book => book.BooksTaken)
                    .ToListAsync(ct);

                var bookResponses = books.Select(b => new BookResponse(b));

                return bookResponses;
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while getting books. Please try again.", ex);
            }
        }


        public async Task<BookResponse> GetBookById(long id, CancellationToken ct = default)
        {
            try
            {
                var book = await _context.Books
                    .Include(book => book.BooksTaken)
                    .FirstOrDefaultAsync(b => b.Id == id, ct);
                if (book is null) return null;

                return new BookResponse(book);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while getting the book. Please try again.", ex);
            }
        }

        public async Task<BookResponse> CreateBook(BookDetails bookDetails, CancellationToken ct = default)
        {
            try
            {
                var author = await _context.Authors
                    .FirstOrDefaultAsync(b => b.Id == bookDetails.AuthorId, ct);
                if (author is null)
                    throw new Exception($"Author with id {bookDetails.AuthorId} does not exist");
                var book = new Book
                {
                    Title = bookDetails.Title,
                    Description = bookDetails.Description,
                    AuthorId = bookDetails.AuthorId.Value
                };
                _context.Books.Add(book);
                await _context.SaveChangesAsync(ct);
                return new BookResponse(book);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while creating the book. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while creating the book. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while creating the book. Please try again.", ex);
            }
        }

        public async Task<BookResponse> UpdateBook(long id, BookDetails bookDetails, CancellationToken ct = default)
        {
            var existingBook = await _context.Books.FindAsync(id, ct);
            if (existingBook is null)
                return null;

            if (bookDetails.Title is not null)
                existingBook.Title = bookDetails.Title;
            if (bookDetails.Description is not null)
                existingBook.Description = bookDetails.Description;
            if (bookDetails.AuthorId is not null)
            {
                var author = await _context.Authors
                   .FirstOrDefaultAsync(b => b.Id == bookDetails.AuthorId, ct);
                if (author is null)
                    throw new Exception($"Author with id {bookDetails.AuthorId} does not exist");
                existingBook.AuthorId = bookDetails.AuthorId.Value;
            }

            try
            {
                _context.Books.Update(existingBook);

                await _context.SaveChangesAsync(ct);
                return new BookResponse(existingBook);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while updating the book. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while updating the book. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while updating the book. Please try again.", ex);
            }
        }

        public async Task<bool> DeleteBook(long id, CancellationToken ct = default)
        {
            var book = await _context.Books.FindAsync(id, ct);
            if (book is null)
                return false;

            try
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while deleting the book. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while deleting the book. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while deleting the book. Please try again.", ex);
            }
        }
        #endregion
        #region Lend/Return
        public async Task LendBook(long bookId, long userId, CancellationToken ct = default)
        {
            var book = await _context.Books
                            .Include(b => b.BooksTaken)
                            .FirstOrDefaultAsync(b => b.Id == bookId, ct);
            if (book is null)
                throw new Exception("No book with such id in library.");

            var user = await _context.Users.FindAsync(userId, ct);
            if (user is null)
                throw new Exception("No user with such id is registered in library.");

            try
            {
                if (book.BooksTaken.Any(bt => bt.UserId == userId && bt.BookId == bookId))
                    throw new Exception("This book is already taken by the user.");

                var bookTaken = new BooksTaken
                {
                    BookId = bookId,
                    UserId = userId,
                    DateTaken = DateTime.UtcNow,
                };

                _context.BooksTaken.Add(bookTaken);
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while lending the book. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while lending the book. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while lending the book. Please try again.", ex);
            }
        }

        public async Task ReturnBook(long bookId, long userId, CancellationToken ct = default)
        {
            var book = await _context.Books
                .Include(b => b.BooksTaken)
                .FirstOrDefaultAsync(b => b.Id == bookId, ct);
            if (book is null)
                throw new Exception("No book with such id in library.");

            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                throw new Exception("No user with such id is registered in library.");

            var bookTaken = await _context.BooksTaken
                .FirstOrDefaultAsync(bt => bt.BookId == bookId && bt.UserId == userId, ct);
            if (bookTaken is null)
                throw new Exception("The user did not take this book.");

            try
            {
                _context.BooksTaken.Remove(bookTaken);
                await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception("Concurrency conflict occurred while returning the book. Please try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error occurred while returning the book. Please check your data and try again.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while returning the book. Please try again.", ex);
            }
        }
        #endregion
        #region Search books
        public async Task<BookSearchResponse> SearchBooks(BookSearchCriteria criteria, int pageNumber, int pageSize, CancellationToken ct = default)
        {
            var books = _context.Books
                .Include(book => book.Author)
                .Include(book => book.BooksTaken)
                .AsQueryable();

            if (criteria.Condition == SearchCondition.And)
                books = ApplyAndCondition(books, criteria);
            else if (criteria.Condition == SearchCondition.Or)
                books = ApplyOrCondition(books, criteria);

            int totalCount = await books.CountAsync(ct);

            books = books
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var paginatedBooks = await books.Select(b => new BookResponse(b)).ToListAsync(ct);

            return new BookSearchResponse(paginatedBooks, totalCount);
        }

        private IQueryable<Book> ApplyAndCondition(IQueryable<Book> books, BookSearchCriteria criteria)
        {
            if (criteria.AuthorId.HasValue)
                books = ApplyAuthorCondition(books, criteria.AuthorId.Value);

            if (!string.IsNullOrEmpty(criteria.Text))
                books = ApplyTextCondition(books, criteria.Text);

            if (criteria.UserId.HasValue)
                books = ApplyUserCondition(books, criteria.UserId.Value);

            return books;
        }

        private IQueryable<Book> ApplyOrCondition(IQueryable<Book> books, BookSearchCriteria criteria)
        {
            IQueryable<Book> booksByAuthor = null;
            IQueryable<Book> booksByText = null;
            IQueryable<Book> booksByUserId = null;

            if (criteria.AuthorId.HasValue)
                booksByAuthor = ApplyAuthorCondition(_context.Books.AsQueryable(), criteria.AuthorId.Value);

            if (!string.IsNullOrEmpty(criteria.Text))
                booksByText = ApplyTextCondition(_context.Books.AsQueryable(), criteria.Text);

            if (criteria.UserId.HasValue)
                booksByUserId = ApplyUserCondition(_context.Books.AsQueryable(), criteria.UserId.Value);

            return (booksByAuthor ?? Enumerable.Empty<Book>().AsQueryable())
                .Union(booksByText ?? Enumerable.Empty<Book>().AsQueryable())
                .Union(booksByUserId ?? Enumerable.Empty<Book>().AsQueryable());
        }

        private IQueryable<Book> ApplyAuthorCondition(IQueryable<Book> books, long authorId)
        {
            books = books.Where(book => book.AuthorId == authorId);
            return books;
        }

        private IQueryable<Book> ApplyTextCondition(IQueryable<Book> books, string text)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                books = books.Where(book =>
                    EF.Functions.Like(book.Title, $"%{word}%") ||
                    EF.Functions.Like(book.Description, $"%{word}%"));
            }

            return books;
        }

        private IQueryable<Book> ApplyUserCondition(IQueryable<Book> books, long userId)
        {
            books = books.Where(book => book.BooksTaken.Any(bt => bt.UserId == userId));
            return books;
        }

        #endregion

        #region Invert words
        public async Task<BookResponse> InvertWordsInTitle(long bookId, CancellationToken ct = default)
        {
            var book = await _context.Books
                            .Include(b => b.BooksTaken)
                            .FirstOrDefaultAsync(b => b.Id == bookId, ct);
            if (book is null)
                return null;

            book.Title = InvertWordsInString(book.Title);

            _context.Books.Update(book);
            await _context.SaveChangesAsync(ct);

            return new BookResponse(book);
        }

        private static string InvertWordsInString(string str)
        {
            var matches = FindWordMatches(str);

            var sb = new StringBuilder();
            var currentPosition = 0;

            foreach (Match match in matches)
            {
                AppendSeparators(sb, str, match.Index, currentPosition);
                AppendReversedWord(sb, match.Value);

                currentPosition = match.Index + match.Length;
            }

            AppendRemainingSeparators(sb, str, currentPosition);

            return sb.ToString();
        }

        private static MatchCollection FindWordMatches(string str)
        {
            var rx = new Regex(@"([a-zA-Z0-9]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = rx.Matches(str);
            return matches;
        }

        private static void AppendSeparators(StringBuilder sb, string str, int matchIndex, int currentPosition)
        {
            if (matchIndex > currentPosition)
                sb.Append(str[currentPosition..matchIndex]);
        }

        private static void AppendReversedWord(StringBuilder sb, string word)
        {
            var reversedWord = new string(word.Reverse().ToArray());
            sb.Append(reversedWord);
        }

        private static void AppendRemainingSeparators(StringBuilder sb, string str, int currentPosition)
        {
            if (currentPosition < str.Length)
                sb.Append(str[currentPosition..]);
        }
        #endregion
    }
}
