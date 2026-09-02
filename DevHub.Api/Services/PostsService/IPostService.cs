using DevHub.Domain.Helpers;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Posts;
namespace DevHub.Services.PostsService;

public interface IPostService
{
    Task<SimpleResult<int>> CreatePostAsync(AddPostRequest request);
    Task<SimpleResult<bool>> DeletePostAsync(int postId);
    Task<Result<PostDetailsResponse>> GetPostInDetailAsync(int id);
    Task<Result<IEnumerable<MinimalPost>>> GetForYouPosts(int userId);
    Task<Result<IEnumerable<MinimalPost>>> GetBookmarkedPosts(int userId);
    Task<Result<IEnumerable<MinimalPost>>> Feed(int userId);
    Task<Result<IEnumerable<MinimalPost>>> SearchPostsAsMinimalAsync(SearchPostsRequest request);
    Task<Result<IEnumerable<MinimalPost>>> GetPostsByCategoryAsync(string categoryId, int pageNumber, int pageSize);
    Task<Result<IEnumerable<MinimalPost>>> GetPostsByTagAsync(string tagId, int pageNumber, int pageSize);
    Task<Result<IPagedList<MinimalPost>>> GetPostsByCategoryAsync(string categoryId);
    //    Task<bool> ValidatePost(AddPostRequest req);
}
