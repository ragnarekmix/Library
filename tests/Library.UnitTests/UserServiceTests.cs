using Library.Core.Model;
using Library.Core.Model.Front.User;
using Library.DataAccess;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Library.UnitTests
{

    public class UserServiceTests
    {
        private UserService _service;
        private LibraryDbContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(databaseName: "LibraryDbTest")
                .Options;

            _context = new LibraryDbContext(options);
            _service = new UserService(_context);
        }

        [TearDown]
        public async Task TearDown()
        {
            await _context.Database.EnsureDeletedAsync();
        }

        [Test]
        public async Task GetUsers_ReturnsUsers()
        {
            var user1 = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com" };
            var user2 = new User { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@test.com" };

            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();

            var result = await _service.GetUsers(1, 10);

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetUserById_ReturnsCorrectUser()
        {
            var user = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.GetUserById(user.Id);

            Assert.AreEqual(user.Id, result.Id);
        }

        [Test]
        public async Task CreateUser_CreatesUser()
        {
            var userDetails = new UserDetails { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com" };

            var result = await _service.CreateUser(userDetails);

            Assert.AreEqual(userDetails.FirstName, result.FirstName);
            Assert.AreEqual(userDetails.LastName, result.LastName);
            Assert.AreEqual(userDetails.Email, result.Email);
        }

        [Test]
        public async Task UpdateUser_UpdatesUser()
        {
            var user = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDetails = new UserDetails { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@test.com" };

            var result = await _service.UpdateUser(user.Id, userDetails);

            Assert.AreEqual(userDetails.FirstName, result.FirstName);
            Assert.AreEqual(userDetails.LastName, result.LastName);
            Assert.AreEqual(userDetails.Email, result.Email);
        }

        [Test]
        public async Task DeleteUser_DeletesUser()
        {
            var user = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteUser(user.Id);

            Assert.True(result);
        }

        [Test]
        public async Task SearchUsers_ReturnsMatchingUsers()
        {
            var user1 = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com" };
            var user2 = new User { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@test.com" };
            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();

            var userDetails = new UserDetails { FirstName = "John" };

            var result = await _service.SearchUsers(userDetails, 1, 10);

            Assert.AreEqual(1, result.Users.Count());
        }

        [Test]
        public async Task GenerateReportAsync_ReturnsCorrectReport()
        {
            var author1 = new Author { FirstName = "Author1", LastName = "Surname1" };
            var author2 = new Author { FirstName = "Author2", LastName = "Surname2" };
            await _context.Authors.AddRangeAsync(author1, author2);

            var book1 = new Book { Title = "Title1", Description = "Description1", AuthorId = 1 };
            var book2 = new Book { Title = "Title2", Description = "Description2", AuthorId = 2 };
            await _context.Books.AddRangeAsync(book1, book2);

            var user = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@test.com" };
            _context.Users.Add(user);

            var book1Taken = new BooksTaken { BookId = book1.Id, UserId = user.Id, DateTaken = DateTime.UtcNow.AddDays(-3) };
            var book2Taken = new BooksTaken { BookId = book2.Id, UserId = user.Id, DateTaken = DateTime.UtcNow.AddDays(-4) };
            _context.BooksTaken.AddRange(book1Taken, book2Taken);
            await _context.SaveChangesAsync();

            var result = await _service.GenerateReport(1, 10);

            var userReport = result.Reports.First();
            Assert.AreEqual(user.FirstName, userReport.FirstName);
            Assert.AreEqual(user.LastName, userReport.LastName);
            Assert.AreEqual(user.Email, userReport.Email);
            Assert.AreEqual(2, userReport.TotalBooks);
            Assert.AreEqual(7, userReport.TotalDays);
        }
    }
}
