using DevHub.Domain.Enums;
using DevHub.Domain.Models;

namespace DevHub.DTOS.Posts;

public record SavePostRequest(
 string Title,
 string Slug,
 string Content,
 string Summary,
 string MainImageUrl,
 int ViewsCount,
 int CategoryId,
 Category Category,
 string AuthorId,
 PostStatus Status,
 DateTime UpdatedAt,
 DateTime PublishedAt,
 DateTime CreatedAt);
