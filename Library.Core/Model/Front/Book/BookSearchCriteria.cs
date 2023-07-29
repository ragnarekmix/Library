namespace Library.Core.Model.Front.Book
{
    public enum SearchCondition
    {
        And = 0,
        Or = 1
    }

    public class BookSearchCriteria
    {
        public long? AuthorId { get; set; }
        /// <summary>
        /// Specifies the text to look for in book Title and Description
        /// </summary>
        public string? Text { get; set; }
        public long? UserId { get; set; }
        /// <summary>
        /// Specifies the search condition.
        /// 0 - All conditions must be met (AND logic).
        /// 1 - Any of the conditions must be met (OR logic).
        /// </summary>
        public SearchCondition Condition { get; set; }
    }
}
