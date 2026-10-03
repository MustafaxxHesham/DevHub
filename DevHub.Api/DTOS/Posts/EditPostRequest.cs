using DevHub.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace DevHub.DTOS.Posts;
public record class EditPostRequest
{
    public string PostId { get; set; }
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? Content { get; set; }
    public string? Summary { get; set; }
    public string? CategoryName { get; set; }
    public string AuthorId { get; set; }
    [FileExtensions(Extensions = "jpg,png,jpeg")]
    public IFormFile? MainImage { get; set; }
    public ICollection<Tag> Tags { get; set; }
    public List<string> ImagesKey { get; set; }
    [FileExtensions(Extensions = "jpg,png,jpeg")]
    public ICollection<IFormFile> PostImages { get; set; }
}