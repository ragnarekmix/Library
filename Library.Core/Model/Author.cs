using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Core.Model
{
    [Table("Author")]
    public class Author
    {
        [Key]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }

        // Navigation property
        public ICollection<Book> Books { get; set; }
    }
}
