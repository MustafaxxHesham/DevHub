namespace DevHub.Domain.Models;
public class PostImage
{
    public int PostId { get; set; }
    public Post Post { get; set; }
    public string ImageUrl { get; set; }
    public string ImageKey { get; set; }
}
