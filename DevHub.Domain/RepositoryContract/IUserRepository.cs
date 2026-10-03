using DevHub.Domain.Helpers;
using DevHub.Domain.Models;
namespace DevHub.Domain.LogicContract.RepositoryContract;
public interface IUserRepository : IBaseRepository<User, int>
{
    Task<bool> IsEmailExistAsync(string email);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<IPagedList<User>> SearchUsersAsync(string searchKey,int pageSize, int pageNumber);
    Task<IEnumerable<User>> GetActiveUsersAsync(int pageSize, int pageNumber);
    IQueryable<User> GetUsersListAsync(int pageSize, int pageNumber);
    IQueryable<User> GetUsersListRoleBasedAsync(int pageSize, int pageNumber, int roleId);
    Task<double[]> GetIncreasedUsersRatioMonthly();
    Task<List<Permission>> GetUserPermissionsAsync(int userId);
    Task<Tuple<User?, List<UserFollower?>, int>> GetUserProfileAsync(int id);
    Task<IPagedList<User>> GetMyFollowings(int userId, int pageSize, int pageNumber);
    Task<IPagedList<User>> GetMyFollowers(int userId, int pageSize, int pageNumber);
    Task<IPagedList<ExternalLogin>> GetUsersGoogleAuth(int pageSize, int pageNumber);
    Task<IPagedList<ExternalLogin>> GetUsersGitHubAuth(int pageSize, int pageNumber);
    Task FollowUserAsync(UserFollower userFollower);
    Task<Dictionary<string, double>> GetAuthRatio();
}