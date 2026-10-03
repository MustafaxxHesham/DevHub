using DevHub.Domain.Base;
using DevHub.Domain.Enums;
namespace DevHub.Domain.Models;
public class Post : ISoftDeletable
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string Summary { get; set; }
    public string MainImageUrl { get; set; }
    public int ViewsCount { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public int AuthorId { get; set; }
    public User Author { get; set; }
    public PostStatus Status { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Reaction>? Reactions { get; set; }
    public ICollection<PostImage>? PostImages { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public ICollection<Comment>? Comments { get; set; }
    public bool IsDeleted { get; set; } = false;
}
