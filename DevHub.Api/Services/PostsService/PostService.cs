using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Commons;
using DevHub.DTOS.Posts;
using DevHub.Responses;
using DevHub.Utilities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.PostsService;
public class PostService(IDataStore _dataStore, IWebHostEnvironment _env, 
                         ProtectionHandler _protectionHandler,
                         RecommendationService.RecommendationService _recommendationService,
                         IDataProtectionProvider provider, ILogger<PostService> _logger) : IPostService
{

    public async Task<Result<PostDetailsResponse>> GetPostInDetailAsync(string id)
    {
        //var realPostId = _protectionHandler.GetRealPostId(id);

        //if (realPostId == -1 || !await _dataStore.Posts.IsExistAsync(realPostId))
        //{
        //    return Result<PostDetailsResponse>.Failure(ResponseMessages.POST_NOT_FOUND);
        //}

        var post = await _dataStore.Posts.GetAsync();

        if (post is null)
        {
            return Result<PostDetailsResponse>.Failure(ResponseMessages.POST_NOT_FOUND);
        }

        throw new NotImplementedException();
        //var response = GetPostDetailsResponse(post);
        
        //return Result<PostDetailsResponse>.Success(response);
    }
    public async Task<SimpleResult<int>> CreatePostAsync(AddPostRequest request)
    {
        var realUserId = _protectionHandler.GetRealUserId(request.AuthorId);

        if (realUserId == -1)
        {
            return SimpleResult<int>.Failure("Error About Getting Id.");//Error to be revisioned
        }

        var post = new Post
        {
            AuthorId = realUserId,
            Slug = request.Slug,
            Status = request.Status,
            Tags = request.Tags,
            CategoryId = 0,
            Summary = request.Summary,
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

//        BackgroundJob.Enqueue<PyServer>(() => ());

        return SimpleResult<int>.Success(post.Id);

    }
    public async Task<SimpleResult<bool>> DeletePostAsync(string postId)
    {
        var realPostId = _protectionHandler.GetRealPostId(postId);

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
    public async Task<Result<IEnumerable<MinimalPost>>> SearchPostsAsMinimalAsync(KeyPagedRequest<string> request)
    {
        var postsList = await _dataStore.MinimalPosts.GetMinimalPostsWithQueryAsync(request.Key, request.PageNumber, request.PageSize);

        if (postsList.Any())
        {
            foreach (var post in postsList)
            {
                post.PostId = _protectionHandler.GetProtectedPostId(post.Id);
            }
            return Result<IEnumerable<MinimalPost>>.Success(postsList);
        }

        return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.POST_NOT_FOUND);
    }    
    public async Task<Result<IEnumerable<MinimalPost>>> GetFeed(int userId)
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
    public async Task<Result<IEnumerable<MinimalPost>>> GetPostsByCategoryAsync(KeyPagedRequest<string> request)
    {
        int cId = _protectionHandler.GetRealCategoryId(request.Key);

        var isCategoryExist = await _dataStore.Categories.IsExistAsync(cId);

        if (!isCategoryExist)
        {
            return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.CATEGORY_NOT_FOUND);
        }
        var result = await _dataStore.MinimalPosts.GetMinimalPostsByCategoryAsync(cId, request.PageNumber, request.PageSize);

        return Result<IEnumerable<MinimalPost>>.Success(result);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetPostsByTagAsync(KeyPagedRequest<string> request)
    {
        int tId = _protectionHandler.GetRealTagId(request.Key);

        var isTagExist = await _dataStore.Tags.IsExistAsync(tId);

        if (!isTagExist)
        {
            return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.TAG_NOT_FOUND);
        }

        var result = await _dataStore.MinimalPosts.GetMinimalPostsByTagAsync(tId, request.PageNumber, request.PageSize);

        return Result<IEnumerable<MinimalPost>>.Success(result);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetBookmarkedPosts(KeyPagedRequest<string> request)
    {
        int realUserId = _protectionHandler.GetRealUserId(request.Key);

        if (realUserId == -1 || !await _dataStore.Users.IsExistAsync(realUserId))
        {
            return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.USER_NOT_FOUND);
        }

        var bookmarkedPosts = await _dataStore.BookmarkedPosts.GetByCriteriaAsync(bp => bp.UserId == realUserId, bp => bp.UserId);

        var posts = await _dataStore.MinimalPosts.GetMinimalPostsBookmarkedAsync(realUserId, 1, 6);

        return Result<IEnumerable<MinimalPost>>.Success(posts);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetRecommendedPostsByPostAsync(string postId)
    {
        var realPostId = _protectionHandler.GetRealPostId(postId);

        if (realPostId == -1)
        {
            return Result<IEnumerable<MinimalPost>>.Failure(ResponseMessages.POST_NOT_FOUND);
        }

        var postsScores = await _recommendationService.GetRecommendedPostsIdsForPost(realPostId);

        var postsIds = postsScores.Select(x => x.postId).ToArray();

        var data = await _dataStore.MinimalPosts.GetRecommendedPosts(postsIds);

        return Result<IEnumerable<MinimalPost>>.Success(data);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetPostsOrderedByViews(PagedRequest request)
    {
        var result = await _dataStore.MinimalPosts.GetPostsOrderedByViews(request.PageSize, request.PageNumber);
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
    public async Task<PagedResponse<Post>> Check()
    {
        var result = await _dataStore.Posts.GetAsync();
        var response = PagedResponse<Post>.Create(result.ValueList, result.PageSize, result.CurrentPage, result.TotalPages);
        return response;
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

    private PostDetailsResponse GetPostDetailsResponse(Post post)
    {
        var response = new PostDetailsResponse
        {
            PostId = _protectionHandler.GetProtectedPostId(post.Id),
            ViewsCount = post.ViewsCount,
            Reacts = post.Reactions!.Count(),
            Title = post.Title,
            Slug = post.Slug,
            Content = post.Content,
            AuthorId = _protectionHandler.GetProtectedUserId(post.AuthorId),
            Summary = post.Summary,
            MainImageUrl = post.MainImageUrl,
            AuthorName = post.Author.FirstName + " " + post.Author.LastName,
            Tags = post.Tags.Select(t => t.Name).ToArray(),
            CategoryId = post.CategoryId,
            CategoryName = post.Category.Name,
            AuthorImageUrl = post.Author.ProfileImageUrl,
            AuthorJobTitle = post.Author.JobTitle,
            PublishedAt = post.PublishedAt
        };
        return response;
    }
    public async Task<SimpleResult<bool>> EditPostAsync(EditPostRequest request)
    {
        var realPostId = _protectionHandler.GetRealPostId(request.PostId);

        if (realPostId == -1)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.POST_NOT_FOUND);
        }

        var post = await _dataStore.Posts.GetByIdAsync(realPostId);

        if (post == null)
        {
            return SimpleResult<bool>.Failure(ResponseMessages.POST_NOT_FOUND);
        }

        if (!string.IsNullOrEmpty(request.Title))
        {
            post.Title = request.Title;
        }

        //if (!string.IsNullOrEmpty(request.MainImage))
        //{
        //    post.Title = request.Title;
        //}

        _dataStore.Posts.UpdateItem(post);

        await _dataStore.CompleteAsync();

        return SimpleResult<bool>.Success(true);
    }
}