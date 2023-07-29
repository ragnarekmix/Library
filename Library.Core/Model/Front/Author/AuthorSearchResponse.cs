namespace Library.Core.Model.Front.Author
{
    public class AuthorSearchResponse
    {
        public AuthorSearchResponse(IEnumerable<AuthorResponse> authors, int totalCount)
        {
            Authors = authors;
            TotalCount = totalCount;
        }

        public IEnumerable<AuthorResponse> Authors { get; set; }
        public int TotalCount { get; set; }
    }
}
