using Library.Core.Model.Front.User;

namespace Library.Core
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetUsers(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<UserResponse> GetUserById(long id, CancellationToken ct = default);
        Task<UserResponse> CreateUser(UserDetails user, CancellationToken ct = default);
        Task<UserResponse> UpdateUser(long id, UserDetails user, CancellationToken ct = default);
        Task<bool> DeleteUser(long id, CancellationToken ct = default);
        Task<UserReportResponse> GenerateReport(int pageNumber, int pageSize, CancellationToken ct = default);
        Task<UserSearchResponse> SearchUsers(UserDetails user, int pageNumber, int pageSize, CancellationToken ct = default);
    }
}
