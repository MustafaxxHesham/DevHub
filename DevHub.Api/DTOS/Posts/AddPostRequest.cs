using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using System.ComponentModel.DataAnnotations;
namespace DevHub.DTOS.Posts;
public record class AddPostRequest
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Content { get; set; }
    public string Summary { get; set; }
    public string CategoryName { get; set; }
    public string AuthorId { get; set; }
    public PostStatus Status { get; set; }
    public DateTime PublishedAt { get; set; }
    public List<string> ImagesKey { get; set; }
    public ICollection<Tag> Tags { get; set; }
    [FileExtensions(Extensions = "jpg,png,jpeg")]
    public IFormFile MainImageUrl { get; set; }
    [FileExtensions(Extensions = "jpg,png,jpeg")]
    public ICollection<IFormFile> PostImages { get; set; }
} 