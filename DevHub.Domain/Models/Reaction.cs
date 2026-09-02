using DevHub.Domain.Enums;

namespace DevHub.Domain.Models;
public class Reaction
{
    public int Id { get; set; }
    public ReactionType ReactionType { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}
