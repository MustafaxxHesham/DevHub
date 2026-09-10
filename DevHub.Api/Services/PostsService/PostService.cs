using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Posts;
using DevHub.EFCore.ErrorTypes;
using DevHub.Responses;
using DevHub.Services.RecommendationService;
using DevHub.Utilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DevHub.Services.PostsService;
public class PostService(IDataStore _dataStore, 
                         IWebHostEnvironment _env, 
                         RecommendationService.RecommendationService _recommendationService,
                         IDataProtectionProvider provider, 
                         ILogger<PostService> _logger) : IPostService
{

    private readonly Dictionary<string, IDataProtector> _protectors = new()
    {
        ["userId"] = provider.CreateProtector(ProtectionPurposes.USER_ID_PURPOSE),
        ["postId"] = provider.CreateProtector(ProtectionPurposes.POST_ID_PURPOSE),
        ["categoryId"] = provider.CreateProtector(ProtectionPurposes.CATEGORY_ID_PURPOSE),
        ["tagId"] = provider.CreateProtector(ProtectionPurposes.TAG_ID_PURPOSE),
    };

    public async Task<Result<PostDetailsResponse>> GetPostInDetailAsync(string id)
    {
        var realPostId = handlePostId(id);

        if (realPostId == -1)
        {
            return Result<PostDetailsResponse>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        PostDetailsResponse response = (await _dataStore.Posts.GetPostDetailsAsync()
                .Where(p => p.Id == realPostId)
                .Select(post => new PostDetailsResponse
                {
                    PostId = _protectors["postId"].Protect(post.Id.ToString()),
                    Title = post.Title,
                    AuthorJobTitle = post.Author.JobTitle,
                    AuthorId = _protectors["userId"].Protect(post.AuthorId.ToString()),
                    Slug = post.Slug,
                    Content = post.Content,
                    Summary = post.Summary,
                    PublishedAt = post.PublishedAt,
                    AuthorName = post.Author.FirstName + " " + post.Author.LastName,
                    ViewsCount = post.ViewsCount,
                    AuthorImageUrl = post.Author.ProfileImageUrl,
                    Reacts = post.Reactions.Count,
                    CategoryName = post.Category.Name,
                    CategoryId = post.Category.Id,
                    Tags = post.Tags.Select(t => t.Name).ToArray(),
                    MainImageUrl = post.MainImageUrl,
                }).FirstOrDefaultAsync())!;

        if (response is null)
            return Result<PostDetailsResponse>.Failure(ResponseMessages.POST_NOT_FOUND);

        return Result<PostDetailsResponse>.Success(response);
    }
    public async Task<SimpleResult<int>> CreatePostAsync(AddPostRequest request)
    {

        var post = new Post
        {
            //Adding Author Id
            Slug = request.Slug,
            Status = request.Status,
            CategoryId = 0,
            Summary = request.Summary,
            AuthorId = 1,
            Content = request.Content,
            MainImageUrl = await UploadImageFileToServerAsync(request.MainImageUrl, true),
            ViewsCount = 0,
            CreatedAt = DateTime.UtcNow,
        };

        List<PostImage> postImage = new List<PostImage>();

        for (int i = 0; i < request.PostImages.Count; i++)
        {
            postImage[i].ImageUrl = await UploadImageFileToServerAsync(request.PostImages.ElementAt(i), false);
            injectImageUrlsInContent(post.Content, postImage[i].ImageUrl, request.ImagesKey[i]);
        }

        post.PostImages = postImage;

        if (post.Status == PostStatus.Published)
        {
            post.PublishedAt = DateTime.UtcNow;
        }

        await _dataStore.Posts.AddAsync(post);

        await _dataStore.CompleteAsync();

        return SimpleResult<int>.Success(post.Id);

    }
    public async Task<SimpleResult<bool>> DeletePostAsync(string postId)
    {
        var realPostId = handlePostId(postId);

        if (realPostId < 0)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        var post = await _dataStore.Posts.GetByIdAsync(realPostId);

        if (post is null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.POST_NOT_FOUND);
        }

        await _dataStore.Posts.DeletePostAsync(post);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> SearchPostsAsMinimalAsync(SearchPostsRequest request)
    {
        var postsList = await _dataStore.MinimalPosts.GetMinimalPostsWithQueryAsync(request.Query, request.PageNumber, request.PageSize);

        if (postsList.Any())
        {
            foreach (var post in postsList)
            {
                post.PostId = _protectors["postId"].Protect(post.Id.ToString());
            }
            return Result<IEnumerable<MinimalPost>>.Success(postsList);
        }

        return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.POST_NOT_FOUND);
    }    
    public async Task<Result<IEnumerable<MinimalPost>>> Feed(int userId)
    {
        if(!await _dataStore.Users.IsExistAsync(userId))
        {
            return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var postsIds = await _recommendationService.PostsFeedGeneral(userId);

        var posts = await _dataStore.MinimalPosts.GetRecommendedPosts(postsIds);

        return Result<IEnumerable<MinimalPost>>.Success(posts);


    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetForYouPosts(int userId)
    {
        await Task.Delay(1000);
        PostScore[] result = [
            new PostScore(1, 4),
            new PostScore(2, 2),
            new PostScore(3, 1.9),
            new PostScore(4, 1.8)
        ];


        var data = await _dataStore.MinimalPosts.GetRecommendedPosts(result.Select(x => x.postId).ToArray());

        return Result<IEnumerable<MinimalPost>>.Success(data);

    }
    // Critical Revision
    public async Task<Result<IEnumerable<MinimalPost>>> GetPostsByCategoryAsync(string categoryId, int pageNumber, int pageSize)
    {
        int cId = int.Parse(_protectors["categoryId"].Unprotect(categoryId));

        var isCategoryExist = await _dataStore.Categories.IsExistAsync(cId);

        if (!isCategoryExist)
        {
            return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.CATEGORY_NOT_FOUND);
        }
        var result = await _dataStore.MinimalPosts.GetMinimalPostsByCategoryAsync(cId, pageNumber, pageSize);

        return Result<IEnumerable<MinimalPost>>.Success(result);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetPostsByTagAsync(string tagId, int pageNumber, int pageSize)
    {
        int tId = int.Parse(_protectors["tagId"].Unprotect(tagId));

        var isTagExist = await _dataStore.Tags.IsExistAsync(tId);

        if (!isTagExist)
        {
            return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.TAG_NOT_FOUND);
        }
        var result = await _dataStore.MinimalPosts.GetMinimalPostsByTagAsync(tId, pageNumber, pageSize);

        return Result<IEnumerable<MinimalPost>>.Success(result);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetBookmarkedPosts(int userId)
    {
        if (!await _dataStore.Users.IsExistAsync(userId))
        {
            return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var bookmarkedPosts = await _dataStore.BookmarkedPosts.GetByCriteriaAsync(bp => bp.UserId == userId, bp => bp.UserId);

        if (!bookmarkedPosts.ValueList.Any())
        {
            return Result<IEnumerable<MinimalPost>>.Failure(DbErrors.NotFoundError.ToString() + " Bookmarked Posts");
        }

        var posts = await _dataStore.MinimalPosts.GetMinimalPostsBookmarkedAsync(userId, 1, 6);

        return Result<IEnumerable<MinimalPost>>.Success(posts);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetRecommendedPostsByPostAsync(string postId)
    {
        var realPostId = handlePostId(postId);

        if (realPostId == -1)
        {
            return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.DATA_SENT_MANIPULATED);
        }

        var postsScores = await _recommendationService.GetRecommendedPostsIdsForPost(realPostId);

        var postsIds = postsScores.Select(x => x.postId).ToArray();

        var data = await _dataStore.MinimalPosts.GetRecommendedPosts(postsIds);

        return Result<IEnumerable<MinimalPost>>.Success(data);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetPostsOrderedByViews(int pageSize, int pageNumber)
    {
        var result = await _dataStore.MinimalPosts.GetPostsOrderedByViews(pageSize, pageNumber);
        return Result<IEnumerable<MinimalPost>>.Success(result);
    }
    public async Task<Result<IEnumerable<CategoryPostsCountResponse>>> GetPostsCountByCategoryAsync()
    {
        var dict = await _dataStore.Posts.GetPostsCountByCategoryAsync();
        
        var values = dict.Select(item => new CategoryPostsCountResponse(item.Key, item.Value)).ToList();

        return Result<IEnumerable<CategoryPostsCountResponse>>.Success(values);
    }
    public async Task<Result<IEnumerable<TagPostsCountResponse>>> GetPostsCountByTagAsync()
    {
        var dict = await _dataStore.Posts.GetPostsCountByTagAsync();
        
        var values = dict.Select(item => new TagPostsCountResponse(item.Key, item.Value)).ToList();

        return Result<IEnumerable<TagPostsCountResponse>>.Success(values);
    }

    private async Task<string> UploadImageFileToServerAsync(IFormFile image, bool isMainImage)
    {
        string imageName = Guid.NewGuid().ToString() + Path.GetFileName(image.FileName);
        string imagePath = string.Empty;
        
        if (isMainImage)
            imagePath = Path.Combine(_env.WebRootPath, "Images", "PostsImages", imageName);
        else
            imagePath = Path.Combine(_env.WebRootPath, "Images", "PostsMainImage", imageName);


        using (var fs = new FileStream(imagePath, FileMode.Create))
            await image.CopyToAsync(fs);

        return imagePath;
    }

    private string injectImageUrlsInContent(string content, string newPath, string imageKey)
    {
        content = content.Replace(imageKey, newPath);

        return content;
    }
    private int handlePostId(string id)
    {
        try
        {
            return int.Parse(_protectors["postId"].Unprotect(id));
        }
        catch (Exception ex)
        {
            if (ex is CryptographicException)
            {
                return -2;
            }
            else if (ex is FormatException)
            {
                return -3;
            }
            else if (ex is OverflowException)
            {
                return -4;
            }
            _logger.LogError(ex.Message);
            return -1;
        }
    }

}
