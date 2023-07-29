using Library.Core;
using Library.Core.Model;
using Library.Core.Model.Front.Book;
using Library.DataAccess;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Library.UnitTests
{

    [TestFixture]
    public class BookServiceTests
    {
        private IBookService _service;
        private LibraryDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for in-memory database
                .Options;

            _context = new LibraryDbContext(options);
            _service = new BookService(_context);
        }

        [TearDown]
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetBooks_ShouldReturnAllBooks()
        {
            var book1 = new Book { Title = "Title1", Description = "Description1", AuthorId = 1 };
            var book2 = new Book { Title = "Title2", Description = "Description2", AuthorId = 2 };
            _context.Books.AddRange(book1, book2);
            await _context.SaveChangesAsync();

            var result = await _service.GetBooks();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetBookById_ShouldReturnCorrectBook()
        {
            var book = new Book { Title = "Title", Description = "Description", AuthorId = 1 };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var result = await _service.GetBookById(book.Id);

            Assert.NotNull(result);
            Assert.AreEqual(book.Title, result.Title);
            Assert.AreEqual(book.Description, result.Description);
        }

        [Test]
        public async Task CreateBook_ShouldCreateNewBook()
        {
            var author = new Author { FirstName = "First", MiddleName = "Middle", LastName = "Last" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var bookDetails = new BookDetails { Title = "Title", Description = "Description", AuthorId = author.Id };
            var result = await _service.CreateBook(bookDetails);

            Assert.NotNull(result);
            Assert.AreEqual(bookDetails.Title, result.Title);
            Assert.AreEqual(bookDetails.Description, result.Description);
        }

        [Test]
        public async Task UpdateBook_ShouldUpdateExistingBook()
        {
            var author = new Author { FirstName = "First", MiddleName = "Middle", LastName = "Last" };
            _context.Authors.Add(author);
            var book = new Book { Title = "Title", Description = "Description", AuthorId = author.Id };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var updatedDetails = new BookDetails { Title = "UpdatedTitle", Description = "UpdatedDescription" };
            var result = await _service.UpdateBook(book.Id, updatedDetails);

            Assert.NotNull(result);
            Assert.AreEqual(updatedDetails.Title, result.Title);
            Assert.AreEqual(updatedDetails.Description, result.Description);
        }

        [Test]
        public async Task DeleteBook_ShouldDeleteExistingBook()
        {
            var book = new Book { Title = "Title", Description = "Description", AuthorId = 1 };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteBook(book.Id);

            Assert.True(result);
            Assert.IsEmpty(_context.Books);
        }

        [Test]
        public async Task LendBook_ShouldShouldCreateBooksTakenEntry()
        {
            var book = new Book { Title = "Title", Description = "Description", AuthorId = 1 };
            _context.Books.Add(book);
            var user = new User { FirstName = "Name", LastName = "Surname", Email = "test@gmail.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _service.LendBook(book.Id, user.Id);
            Assert.IsNotNull(_context.BooksTaken.FirstOrDefault(bt => bt.BookId == book.Id && bt.UserId == user.Id));
        }

        [Test]
        public async Task ReturnBook_ShouldShouldRemoveBooksTakenEntry()
        {
            var book = new Book { Title = "Title", Description = "Description", AuthorId = 1 };
            _context.Books.Add(book);
            var user = new User { FirstName = "Name", LastName = "Surname", Email = "test@gmail.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _service.LendBook(book.Id, user.Id);

            await _service.ReturnBook(book.Id, user.Id);

            Assert.IsNull(_context.BooksTaken.FirstOrDefault(bt => bt.BookId == book.Id && bt.UserId == user.Id));
        }

        [Test]
        public async Task SearchBooks_ShouldReturnMatchingBooks()
        {
            var author1 = new Author { FirstName = "Author1", LastName = "Surname1" };
            var author2 = new Author { FirstName = "Author2", LastName = "Surname2" };
            await _context.Authors.AddRangeAsync(author1, author2);

            var book1 = new Book { Title = "Title1", Description = "Description1", AuthorId = 1 };
            var book2 = new Book { Title = "Title2", Description = "Description2", AuthorId = 2 };
            await _context.Books.AddRangeAsync(book1, book2);
            await _context.SaveChangesAsync();

            var searchDetails = new BookSearchCriteria { Text = "Title", Condition = SearchCondition.And };
            var result = await _service.SearchBooks(searchDetails, 1, 10);

            Assert.AreEqual(2, result.Books.Count());
        }

        [Test]
        public async Task InvertWordsInTitle_ShouldInvertWordsInTitle()
        {
            var book = new Book { Title = "The Great Gatsby", Description = "Description", AuthorId = 1 };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var result = await _service.InvertWordsInTitle(book.Id);

            Assert.NotNull(result);
            Assert.AreEqual("ehT taerG ybstaG", result.Title);
        }
    }
}
