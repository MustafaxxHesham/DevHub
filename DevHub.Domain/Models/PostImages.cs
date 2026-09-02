namespace DevHub.Domain.Models;
public class PostImages
{
    public int PostId { get; set; }
    public IEnumerable<string> ImagesUrls { get; set; }
}
