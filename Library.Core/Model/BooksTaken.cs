using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Core.Model
{
    [Table("BooksTaken")]
    public class BooksTaken
    {
        public long BookId { get; set; }
        public long UserId { get; set; }
        public DateTime DateTaken { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Book Book { get; set; }
    }
}
