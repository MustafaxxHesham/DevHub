using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DevHub.Domain.Models;
public class MinimalPost
{
    [JsonIgnore]
    public int Id { get; set; }
    [NotMapped]
    public string PostId { get; set; }
    public string Title { get; set; }
    public string FeatureImageUrl { get; set; }
    public string SecondaryText { get; set; }
    public string AuthorName { get; set; }
    public string AuthorImageProfileUrl { get; set; }
    public string Category { get; set; }
    [JsonIgnore]
    public int FinalCount { get; set; } = -1;
}
