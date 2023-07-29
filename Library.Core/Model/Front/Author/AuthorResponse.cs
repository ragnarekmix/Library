namespace Library.Core.Model.Front.Author
{
    public class AuthorResponse
    {

        public AuthorResponse(Model.Author author)
        {
            Id = author.Id;
            FirstName = author.FirstName;
            MiddleName = author.MiddleName;
            LastName = author.LastName;
            Books = author.Books?.Select(b => b.Id);
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<long>? Books { get; set; }
    }
}
