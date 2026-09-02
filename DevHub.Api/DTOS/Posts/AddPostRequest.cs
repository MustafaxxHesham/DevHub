using DevHub.Domain.Enums;
using DevHub.Domain.Models;
namespace DevHub.DTOS.Posts;
public record class AddPostRequest
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string Summary { get; set; }
    public string CategoryName { get; set; }
    public int AuthorId { get; set; }
    public IFormFile MainImageUrl { get; set; }
    public PostStatus Status { get; set; }
    public DateTime PublishedAt { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public ICollection<IFormFile> PostImages { get; set; }
} 