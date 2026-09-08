using DevHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Layer.EFCore.EntitiesConfigurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasMany(u => u.BookmarkedPosts)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .IsRequired(false);

        builder.HasMany(u => u.MyPosts)
            .WithOne(p => p.Author)
            .HasForeignKey(p => p.AuthorId)
            .IsRequired(false);

        builder.HasMany(u => u.Permissions)
            .WithMany(p => p.Users)
            .UsingEntity<UserPermission>();
    }
}

/*
 
 Your target project 'DevHub.Api' doesn't match your migrations assembly 'DevHub.EFCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'. Either change your target project or change your migrations assembly.
Change your migrations assembly by using DbContextOptionsBuilder. E.g. options.UseSqlServer(connection, b => b.MigrationsAssembly("DevHub.Api")). By default, the migrations assembly is the assembly containing the DbContext.
Change your target project to the migrations project by using the Package Manager Console's Default project drop-down list, or by executing "dotnet ef" from the directory containing the migrations project.*/