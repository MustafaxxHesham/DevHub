using DevHub.Domain.Helpers;
using DevHub.Domain.LogicContract.RepositoryContract;
using DevHub.Domain.Models;
using DevHub.EFCore.EFCorePaginationHelper;
using Microsoft.EntityFrameworkCore;

namespace Data.Layer.EFCore.Repository;

public class UserRepository(AppDbContext _context) : BaseRepository<User, int>(_context), IUserRepository
{
    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _context.Users.Where(x => x.IsActive)
            .ToListAsync();
    }
    public async Task<IEnumerable<User>> GetActiveUsersAsync(int pageSize, int pageNumber)
    {
        return await _context.Users.Where(x => x.IsActive)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    public async Task<User> GetByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email.Equals(email));
    }
    public async Task<IPagedList<User>> GetMyFollowers(int userId, int pageSize, int pageNumber)
    {
        var query = _context.Followers.Where(x => x.FollowedUserId == userId)
                                      .Select(x => x.FollowerUser);
        return await PagedResponse<User>.CreateAsync(query, pageSize, pageNumber);
    }
    public async Task<IPagedList<User>> GetMyFollowings(int userId, int pageSize, int pageNumber)
    {
        var query = _context.Followers.Where(x => x.FollowerUserId == userId)
                                      .Select(x => x.FollowedUser);
        return await PagedResponse<User>.CreateAsync(query, pageSize, pageNumber);
    }
    public async Task<double[]> GetIncreasedUsersRatioMonthly()
    {
        var result = new double[]
        {
            await _context.Users.CountAsync(),
            ((await _context.Users.CountAsync(u => u.CreatedAt.Month == DateTime.UtcNow.Month))
            /(await _context.Users.CountAsync(u => u.CreatedAt.Month == (DateTime.UtcNow.Month - 1)))) * 100,
        };
        return result;
    }
    public Task<List<Permission>> GetUserPermissionsAsync(int userId)
    {
        var userPermissions = _context.Users.Where(u => u.Id == userId)
            .Include(u => u.Permissions)
            .SelectMany(u => u.Permissions!)
            .ToListAsync();

        return userPermissions;
    }
    public async Task<Tuple<User, List<UserFollower>, int>> GetUserProfileAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return Tuple.Create<User, List<UserFollower>, int>(null, null, 0)!;

        var ufList = await _context.Followers
                                   .Where(uf => uf.FollowedUserId == id || uf.FollowerUserId == id).ToListAsync();

        var postsCount = await _context.Posts.Where(p => p.AuthorId == id)
                                             .Select(x => x.ViewsCount)
                                             .SumAsync();

        return Tuple.Create(user, ufList, postsCount);
    }
    public IQueryable<User> GetUsersListAsync(int pageSize, int pageNumber)
    {
        var query = _context.Users.Include(x => x.Role)
                                  .OrderBy(x => x.Id)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .AsQueryable();
        return query;
    }
    public IQueryable<User> GetUsersListRoleBasedAsync(int roleId, int pageSize = 6, int pageNumber = 1)
    {
        return _context.Users.Where(u => u.RoleId == roleId)
                             .Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .AsQueryable();
    }
    public async Task<bool> IsEmailExistAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email.Equals(email));
    }
    public async Task<IPagedList<User>> SearchUsersAsync(string searchKey, int pageSize, int pageNumber)
    {
        var query = _context.Users.Where(u => u.FirstName.Contains(searchKey) || u.LastName.Contains(searchKey))
                                   .OrderBy(u => u.Id);

        return await PagedResponse<User>.CreateAsync(query, pageSize, pageNumber);
    }
    public async Task FollowUserAsync(UserFollower userFollower)
    {
        await _context.Followers.AddAsync(userFollower);
    }
    public async Task<Dictionary<string, int>> GetAuthRatio()
    {
        int totalCount = await _context.Users.CountAsync();
        int gooleAuthCount = await _context.ExternalLogins.CountAsync(x => x.LoginKey.Equals("Google"));
        int githubAuthCount = await _context.ExternalLogins.CountAsync(x => x.LoginKey.Equals("GitHub"));
        Dictionary<string, int> resultRatio = new Dictionary<string, int>
        {
            ["Total Count"] = totalCount,
            ["Google Auth Count"] = gooleAuthCount,
            ["Github Auth Count"] = githubAuthCount
        };
        return resultRatio;
    }
    public async Task<IPagedList<ExternalLogin>> GetUsersGoogleAuth(int pageSize, int pageNumber)
    {
        var query = _context.ExternalLogins.Include(x => x.User)
                                            .Where(x => x.Provider.Equals("Google"))
                                            .OrderBy(x => x.UserId);

        return await PagedResponse<ExternalLogin>.CreateAsync(query, pageSize, pageNumber);
    }
    public async Task<IPagedList<ExternalLogin>> GetUsersGitHubAuth(int pageSize, int pageNumber)
    {
        var query = _context.ExternalLogins.Include(x => x.User)
                                            .Where(x => x.Provider.Equals("GitHub"))
                                            .OrderBy(x => x.UserId);

        return await PagedResponse<ExternalLogin>.CreateAsync(query, pageSize, pageNumber);
    }
}
