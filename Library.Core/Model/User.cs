using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Core.Model
{
    [Table("User")]
    public class User
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // Navigation property
        public ICollection<BooksTaken> BooksTaken { get; set; }
    }
}
