using DevHub.Domain.Helpers;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Commons;
using DevHub.DTOS.Posts;
using DevHub.Utilities;
namespace DevHub.Services.PostsService;
public interface IPostService
{
    Task<PagedResponse<Post>> Check();
    Task<SimpleResult<int>> CreatePostAsync(AddPostRequest request);
    Task<SimpleResult<bool>> EditPostAsync(EditPostRequest request);
    Task<SimpleResult<bool>> DeletePostAsync(string postId);
    Task<Result<PostDetailsResponse>> GetPostInDetailAsync(string id);
    Task<Result<IEnumerable<TagPostsCountResponse>>> GetPostsCountByTagAsync();
    Task<Result<IEnumerable<CategoryPostsCountResponse>>> GetPostsCountByCategoryAsync();
    Task<Result<IEnumerable<MinimalPost>>> GetForYouPosts(int userId);
    Task<Result<IEnumerable<MinimalPost>>> GetBookmarkedPosts(KeyPagedRequest<string> request);
    Task<Result<IEnumerable<MinimalPost>>> GetFeed(int userId);
    Task<Result<IEnumerable<MinimalPost>>> SearchPostsAsMinimalAsync(KeyPagedRequest<string> request);
    Task<Result<IEnumerable<MinimalPost>>> GetPostsByCategoryAsync(KeyPagedRequest<string> request);
    Task<Result<IEnumerable<MinimalPost>>> GetPostsByTagAsync(KeyPagedRequest<string> request);
    Task<Result<IEnumerable<MinimalPost>>> GetPostsOrderedByViews(PagedRequest request);
    Task<Result<IEnumerable<MinimalPost>>> GetRecommendedPostsByPostAsync(string postId);
    //    Task<bool> ValidatePost(AddPostRequest req);
}