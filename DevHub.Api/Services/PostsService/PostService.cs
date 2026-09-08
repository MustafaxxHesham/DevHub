using DevHub.Domain.DataStoreContract;
using DevHub.Domain.Enums;
using DevHub.Domain.Helpers;
using DevHub.Domain.Models;
using DevHub.Domain.Result;
using DevHub.DTOS.Posts;
using DevHub.EFCore.ErrorTypes;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace DevHub.Services.PostsService;
public class PostService(IDataStore _dataStore, 
                         IWebHostEnvironment _env, 
                         IDataProtectionProvider provider, 
                         ILogger<PostService> _logger) : IPostService
{

    private readonly IDataProtector _protector = provider.CreateProtector("PostProtectId");
    public async Task<Result<PostDetailsResponse>> GetPostInDetailAsync(int id)
    {
        //  Query <Mapster in IQueryable.>
        PostDetailsResponse response = (await _dataStore.Posts.GetPostDetailsAsync()
                .Where(p => p.Id == id)
                .Select(post => new PostDetailsResponse
                {
                    PostId = _protector.Protect(post.Id.ToString()),
                    Title = post.Title,
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
            return Result<PostDetailsResponse>.Failure("Not Found");

        return Result<PostDetailsResponse>.Success(response);
    }
    public async Task<SimpleResult<int>> CreatePostAsync(AddPostRequest request)
    {

        var post = new Post
        {
            Slug = request.Slug,
            Status = request.Status,
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
            postImage[i].ImageKey = request.ImagesKey[i];
            postImage[i].ImageUrl = await UploadImageFileToServerAsync(request.PostImages.ElementAt(i), false);
        }

        post.PostImages = postImage;

        if (post.Status == PostStatus.Published)
            post.PublishedAt = DateTime.Now.ToLocalTime();

        await _dataStore.Posts.AddAsync(post);

        await _dataStore.CompleteAsync();

        return SimpleResult<int>.Success(post.Id);

    }
    public async Task<SimpleResult<bool>> DeletePostAsync(int postId)
    {
        var post = await _dataStore.Posts.GetByIdAsync(postId);

        if (post is null)
        {
            return SimpleResult<bool>.Failure(DbErrors.NotFoundError.ToString());
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
                post.PostId = _protector.Protect(post.Id.ToString());
            }
            return Result<IEnumerable<MinimalPost>>.Success(postsList);
        }

        return Result<IEnumerable<MinimalPost>>.Failure(DbErrors.NotFoundError.ToString());
    }    
    public async Task<Result<IEnumerable<MinimalPost>>> Feed(int userId)
    {
        var followersList = await _dataStore.Users.GetByIdAsync(userId);
        throw new NotImplementedException();
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetForYouPosts(int userId)
    {
        throw new NotImplementedException();
    }

    // Critical Revision
    public async Task<Result<IEnumerable<MinimalPost>>> GetPostsByCategoryAsync(string categoryId, int pageNumber, int pageSize)
    {
        int cId = int.Parse(_protector.Unprotect(categoryId));

        var isCategoryExist = await _dataStore.Categories.IsExistAsync(cId);
        
        if (!isCategoryExist)
            return Result<IEnumerable<MinimalPost>>.Failure(DbErrors.NotFoundError.ToString() + nameof(Category));

        var result = await _dataStore.MinimalPosts.GetMinimalPostsByCategoryAsync(cId, pageNumber, pageSize);

        if (!result.Any())
            return Result<IEnumerable<MinimalPost>>.Failure(DbErrors.NotFoundError.ToString());

        return Result<IEnumerable<MinimalPost>>.Success(result);
    }

    public async Task<Result<IEnumerable<MinimalPost>>> GetPostsByTagAsync(string tagId, int pageNumber, int pageSize)
    {
        int tId = int.Parse(_protector.Unprotect(tagId));

        var isCategoryExist = await _dataStore.Categories.IsExistAsync(tId);
        
        if (!isCategoryExist)
            return Result<IEnumerable<MinimalPost>>.Failure(DbErrors.NotFoundError.ToString() + nameof(Tag));

        var result = await _dataStore.MinimalPosts.GetMinimalPostsByTagAsync(tId, pageNumber, pageSize);

        if (!result.Any())
            return Result<IEnumerable<MinimalPost>>.Failure(DbErrors.NotFoundError.ToString());

        return Result<IEnumerable<MinimalPost>>.Success(result);
    }
    public async Task<Result<IEnumerable<MinimalPost>>> GetBookmarkedPosts(int userId)
    {
        if (!await _dataStore.Users.IsExistAsync(userId))
        {
            return Result<IEnumerable<MinimalPost>>.Failure(DbErrors.NotFoundError.ToString() + " User");
        }

        var bookmarkedPosts = await _dataStore.BookmarkedPosts.GetByCriteriaAsync(bp => bp.UserId == userId, bp => bp.UserId);

        if (!bookmarkedPosts.ValueList.Any())
        {
            return Result<IEnumerable<MinimalPost>>.Failure(DbErrors.NotFoundError.ToString() + " Bookmarked Posts");
        }

        var posts = await _dataStore.MinimalPosts.GetMinimalPostsBookmarkedAsync(userId, 1, 6);

        return Result<IEnumerable<MinimalPost>>.Success(posts);
    }
    public async Task<Result<IPagedList<MinimalPost>>> GetPostsByCategoryAsync(string categoryId)
    {
        int pageSize = 0, pageNumber = 0;
        var result = await _dataStore.Posts.GetPaginatedByCriteriaAsync(pageSize, pageNumber, x => x.CategoryId == 1, x => x.ViewsCount); ;
        
        throw new NotImplementedException();
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
    public async Task<Result<IEnumerable<MinimalPost>>> GetRecommendedPostsByPostAsync(string postId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<IEnumerable<MinimalPost>>> GetPostsOrderedByViews(int pageSize, int pageNumber)
    {
        var result = await _dataStore.MinimalPosts.GetPostsOrderedByViews(pageSize, pageNumber);
        return Result<IEnumerable<MinimalPost>>.Success(result);
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

    private async Task<string> injectImageUrlsInContent(string content, IFormFile file, string imageKey)
    {
        content = content.Replace(imageKey, await UploadImageFileToServerAsync(file, false));

        return content;
    }


}
