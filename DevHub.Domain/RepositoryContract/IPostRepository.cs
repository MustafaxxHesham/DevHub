using DevHub.Domain.Models;
namespace DevHub.Domain.LogicContract.RepositoryContract;

public interface IPostRepository : IBaseRepository<Post, int>
{
    IQueryable<Post> GetPostDetailsAsync();
    Task DeletePostAsync(Post post);
    Task UpdatePostAsync(Post post);
}
