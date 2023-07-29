using Library.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BooksTaken> BooksTaken { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BooksTaken>()
                .HasKey(bt => new { bt.BookId, bt.UserId });

            modelBuilder.Entity<BooksTaken>()
                .HasOne(bt => bt.User)
                .WithMany(u => u.BooksTaken)
                .HasForeignKey(bt => bt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BooksTaken>()
                .HasOne(bt => bt.Book)
                .WithMany(b => b.BooksTaken)
                .HasForeignKey(bt => bt.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);
        }
    }
}
