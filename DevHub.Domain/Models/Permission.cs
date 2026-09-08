namespace DevHub.Domain.Models;
public class Permission
{
    public int Id { get; set; }
    public string PermissionName { get; set; }
    public ICollection<User> Users { get; set; }
}

/*
    Permission<Create Blog, Edit Blog>

*/