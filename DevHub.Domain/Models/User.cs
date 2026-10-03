using DevHub.Domain.Base;

namespace DevHub.Domain.Models;
public class User : ISoftDeletable
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHashed { get; set; }
    public string JobTitle { get; set; }
    public string ProfileImageUrl { get; set; }
    public string Bio { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? Wallet { get; set; }
    public Wallet? WalletId { get; set; }
    public int RoleId { get; set; }
    public Role Role{ get; set; }
    public ICollection<RefreshToken>? RefreshTokens { set; get; }
    public ICollection<Post>? MyPosts { get; set; }
    public ICollection<Permission>? Permissions { get; set; }
    public ICollection<UserSubscription>? UserSubscriptions { get; set; }
    public ICollection<BookmarkedPost>? BookmarkedPosts { get; set; }
    public bool IsDeleted { get; set; } = false;
}
