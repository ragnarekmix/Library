namespace Library.Core.Model.Front.Book
{
    public class BookSearchResponse
    {
        public BookSearchResponse(IEnumerable<BookResponse> books, int totalCount)
        {
            Books = books;
            TotalCount = totalCount;
        }

        public IEnumerable<BookResponse> Books { get; set; }
        public int TotalCount { get; set; }
    }
}
