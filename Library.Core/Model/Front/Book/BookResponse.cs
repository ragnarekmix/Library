namespace Library.Core.Model.Front.Book
{
    public class BookResponse
    {
        public BookResponse(Model.Book book)
        {
            Id = book.Id;
            Title = book.Title;
            Description = book.Description;
            AuthorId = book.AuthorId;
            TakenByUsers = book.BooksTaken?.Select(b => b.UserId);
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long AuthorId { get; set; }
        public IEnumerable<long>? TakenByUsers { get; set; }
    }
}
