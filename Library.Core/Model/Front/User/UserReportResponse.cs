namespace Library.Core.Model.Front.User
{
    public class UserReportResponse
    {
        public UserReportResponse(IEnumerable<UserReport> reports, int totalCount)
        {
            Reports = reports;
            TotalCount = totalCount;
        }

        public IEnumerable<UserReport> Reports { get; set; }
        public int TotalCount { get; set; }
    }
}
