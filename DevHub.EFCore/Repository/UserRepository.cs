using DevHub.Domain.LogicContract.RepositoryContract;
using DevHub.Domain.Models;
using DevHub.EFCore.EFCorePaginationHelper;
using Microsoft.EntityFrameworkCore;

namespace Data.Layer.EFCore.Repository
{
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
            return _context.Users.Where(u => u.RoleId == roleId).Skip((pageNumber - 1) * pageSize)
                                                         .Take(pageSize)
                                                         .AsQueryable();
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.Equals(email));
        }
    }
}
