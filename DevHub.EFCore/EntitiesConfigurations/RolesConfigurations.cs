using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Layer.EFCore.EntitiesConfigurations
{
    public class RolesConfigurations : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(new Role[]
            {
                new Role { Id = 1, RoleName = Roles.Reader.ToString()},
                new Role { Id = 2, RoleName = Roles.Author.ToString()},
                new Role { Id = 3, RoleName = Roles.Admin.ToString()}
            });
        }
    }
}
