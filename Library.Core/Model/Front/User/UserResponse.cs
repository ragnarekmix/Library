namespace Library.Core.Model.Front.User
{
    public class UserResponse
    {

        public UserResponse(Model.User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            BooksTaken = user.BooksTaken?.Select(b => b.BookId);
        }

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public IEnumerable<long>? BooksTaken { get; set; }
    }
}
