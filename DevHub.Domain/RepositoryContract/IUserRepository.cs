using DevHub.Domain.Models;
namespace DevHub.Domain.LogicContract.RepositoryContract;
public interface IUserRepository : IBaseRepository<User, int>
{
    Task<bool> IsEmailExistAsync(string email);
    Task<User> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<IEnumerable<User>> GetActiveUsersAsync(int pageSize, int pageNumber);
    IQueryable<User> GetUsersListAsync(int pageSize, int pageNumber);
    IQueryable<User> GetUsersListRoleBasedAsync(int pageSize, int pageNumber, int roleId);
}