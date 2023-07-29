using Library.Core;
using Library.Core.Model;
using Library.Core.Model.Front.Author;
using Library.DataAccess;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Library.UnitTests
{

    [TestFixture]
    public class AuthorServiceTests
    {
        private IAuthorService _service;
        private LibraryDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for in-memory database
                .Options;

            _context = new LibraryDbContext(options);
            _service = new AuthorService(_context);
        }

        [TearDown]
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAuthors_ShouldReturnAllAuthors()
        {
            var author1 = new Author { FirstName = "First", MiddleName = "Middle", LastName = "Last" };
            var author2 = new Author { FirstName = "First2", MiddleName = "Middle2", LastName = "Last2" };
            _context.Authors.AddRange(author1, author2);
            await _context.SaveChangesAsync();

            var result = await _service.GetAuthors();

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetAuthorById_ShouldReturnCorrectAuthor()
        {
            var author = new Author { FirstName = "First", MiddleName = "Middle", LastName = "Last" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var result = await _service.GetAuthorById(author.Id);

            Assert.NotNull(result);
            Assert.AreEqual(author.FirstName, result.FirstName);
            Assert.AreEqual(author.LastName, result.LastName);
        }

        [Test]
        public async Task CreateAuthor_ShouldCreateNewAuthor()
        {
            var authorDetails = new AuthorDetails { FirstName = "First", MiddleName = "Middle", LastName = "Last" };

            var result = await _service.CreateAuthor(authorDetails);

            Assert.NotNull(result);
            Assert.AreEqual(authorDetails.FirstName, result.FirstName);
            Assert.AreEqual(authorDetails.LastName, result.LastName);
        }

        [Test]
        public async Task UpdateAuthor_ShouldUpdateExistingAuthor()
        {
            var author = new Author { FirstName = "First", MiddleName = "Middle", LastName = "Last" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var updatedDetails = new AuthorDetails { FirstName = "UpdatedFirst", LastName = "UpdatedLast" };
            var result = await _service.UpdateAuthor(author.Id, updatedDetails);

            Assert.NotNull(result);
            Assert.AreEqual(updatedDetails.FirstName, result.FirstName);
            Assert.AreEqual(updatedDetails.LastName, result.LastName);
        }

        [Test]
        public async Task DeleteAuthor_ShouldDeleteExistingAuthor()
        {
            var author = new Author { FirstName = "First", MiddleName = "Middle", LastName = "Last" };
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAuthor(author.Id);

            Assert.True(result);
            Assert.IsEmpty(_context.Authors);
        }

        [Test]
        public async Task SearchAuthors_ShouldReturnMatchingAuthors()
        {
            var author1 = new Author { FirstName = "First", MiddleName = "Middle", LastName = "Last" };
            var author2 = new Author { FirstName = "First2", MiddleName = "Middle2", LastName = "Last2" };
            _context.Authors.AddRange(author1, author2);
            await _context.SaveChangesAsync();

            var searchDetails = new AuthorDetails { FirstName = "First" };
            var result = await _service.SearchAuthors(searchDetails, 1, 10);

            Assert.AreEqual(2, result.Authors.Count());
        }
    }
}
