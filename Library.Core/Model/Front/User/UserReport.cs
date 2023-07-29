namespace Library.Core.Model.Front.User
{
    public class UserReport
    {
        public UserReport(Model.User user, int books, int days)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            TotalBooks = books;
            TotalDays = days;
        }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int TotalBooks { get; set; }
        public int TotalDays { get; set; }
    }
}
