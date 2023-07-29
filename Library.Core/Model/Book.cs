using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Core.Model
{
    [Table("Book")]
    public class Book
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public long AuthorId { get; set; }

        // Navigation properties
        public Author Author { get; set; }
        public ICollection<BooksTaken> BooksTaken { get; set; }
    }
}
