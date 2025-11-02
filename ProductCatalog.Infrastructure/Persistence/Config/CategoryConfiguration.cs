using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Config;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasData(
            new Category { Id = new Guid("bb059a77-e12c-4182-b798-872b77c28ea5"), Name = "Еда" },
            new Category { Id = new Guid("7739170d-02d3-463c-9c61-22e66eed1323"), Name = "Вкусности" },
            new Category { Id = new Guid("39cc7d9d-656f-44cd-b2af-a6b65bd694ea"), Name = "Вода" },
            new Category { Id = new Guid("f04987ea-6f6e-4d30-9628-eb5be4cee7e9"), Name = "Фрукты/овощи" },
            new Category { Id = new Guid("3d0634fe-52ac-488d-a8ba-1257376c0965"), Name = "Специи" }
        );
    }
}