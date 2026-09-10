namespace DevHub.Services.RecommendationService;

public sealed class RecommendationService(HttpClient _httpClient)
{
    public async Task<PostScore[]> GetRecommendedPostsIdsForPost(int postId)
    {
        var content = await _httpClient.GetFromJsonAsync<List<PostScore>>($"posts/{postId}/recommendations");

        return content!.ToArray();
    }

    public async Task<PostScore[]> GetRecommendedForUserAsync(int userId)
    {
        var content = await _httpClient.GetFromJsonAsync<List<PostScore>>($"users/{userId}/recommendations");

        return content!.ToArray();
    }

    public async Task<int[]> PostsFeedGeneral(int userId)
    {
        return (await _httpClient.GetFromJsonAsync<int[]>($"feed"))!;
    }

    public async Task BulkUpdateForPostViews()
    {
        
    }
}

public record class PostsViews(int ViewsCount, int PostId);

/*
    Tables in MongoDB
    Post_Views_By_User
    General_Events 
    (userId, postId, eventType(postviewed), createdAt, tagsListAsString[,,], isAuthorFollowed?
 */