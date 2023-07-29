namespace Library.Core.Model.Front.User
{
    public class UserSearchResponse
    {
        public UserSearchResponse(IEnumerable<UserResponse> users, int totalCount)
        {
            Users = users;
            TotalCount = totalCount;
        }

        public IEnumerable<UserResponse> Users { get; set; }
        public int TotalCount { get; set; }
    }
}
