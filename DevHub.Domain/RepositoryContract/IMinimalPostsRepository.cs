using DevHub.Domain.Models;
namespace DevHub.Domain.LogicContract.RepositoryContract;
public interface IMinimalPostsRepository
{
    Task<IEnumerable<MinimalPost>> GetMinimalPostsWithQueryAsync(string searchText, int pageNumber, int pageSize);
    Task<IEnumerable<MinimalPost>> GetMinimalPostsByCategoryAsync(int categoryId, int pageNumber, int pageSize);
    Task<IEnumerable<MinimalPost>> GetMinimalPostsByTagAsync(int tagId, int pageNumber, int pageSize);
    Task<IEnumerable<MinimalPost>> GetMinimalPostsBookmarkedAsync(int userId, int pageNumber, int pageSize);
    Task<IEnumerable<MinimalPost>> GetPostsOrderedByViews(int pageSize, int pageNumber);
    Task<IEnumerable<MinimalPost>> GetRecommendedPosts(int[] postsIds);

}