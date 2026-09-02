namespace DevHub.DTOS.Posts;
public class PostDetailsResponse
{
    public string PostId { get; set; }
    public int ViewsCount { get; set; }
    public int Reacts { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string Summary { get; set; }
    public string MainImageUrl { get; set; }
    public string AuthorName { get; set; }
    public string[] Tags { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string AuthorImageUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
}
