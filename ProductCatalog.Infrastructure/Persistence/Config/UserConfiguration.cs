using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Config;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
    
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired();
        
        builder.Property(u => u.PasswordHash)
            .IsRequired();
        
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(u => u.IsBlocked)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.HasData(
            new User
            {
                Id = new Guid("7ac34c8c-eb1d-48c0-9cf5-92d7bacbaffc"),
                Email = "admin@gmail.com",
                Name = "Administrator",
                Role = "Admin",
                PasswordHash = "AQAAAAIAAYagAAAAEIeQe+1xlicOfpcjYkptypRASSGGm0wwYMtJns4bunawj2LcRGPwVLUcw3wuLCZc7w==",
                IsBlocked = false
            },
            new User
            {
                Id = new Guid("6d259ae6-6f45-4dc7-a780-c9bb81950ee1"),
                Email = "prouser@gmail.com",
                Name = "Pro User",
                Role = "ProUser",
                PasswordHash = "AQAAAAIAAYagAAAAENbW4YvWmFhhwjSMmEsY8jagvNaJ+EIZUmkoFrgA8w04E7rqWUipj5pfNYrDxhJFMQ==",
                IsBlocked = false
            },
            new User
            {
                Id = new Guid("f0ca277a-aac2-4e54-b246-3f7435aa9449"),
                Email = "user@gmail.com",
                Name = "User",
                Role = "User",
                PasswordHash = "AQAAAAIAAYagAAAAEFj/E2JOZ1wIU25CtVZ9a1PgyswG8gYXxNfKKO28/mO8RWxzTokrbRbgWLc7tGtMlw==",
                IsBlocked = false
            }
        );
    }
}