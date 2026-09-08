using DevHub.Domain.Models;
using DevHub.Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace DevHub.EFCore.EntitiesConfigurations;
public class PermissionsConfigurations : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasData(
           new() {
            Id = 1,
            PermissionName = PermissionsList.CREATE_POST
        }, new() { 
            Id = 2,
            PermissionName = PermissionsList.DELETE_POST
        }, new() { 
            Id = 3,
            PermissionName = PermissionsList.EDIT_POST
        }, new() { 
            Id = 4,
            PermissionName = PermissionsList.VIEW_POST
        });
    }
}