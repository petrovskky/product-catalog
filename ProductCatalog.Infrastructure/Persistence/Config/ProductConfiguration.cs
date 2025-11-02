using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Infrastructure.Persistence.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .HasMaxLength(100);
        
        builder.HasIndex(p => p.Name);
        
        builder.Property(p => p.CategoryId)
            .IsRequired();
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(p => p.Description)
            .HasMaxLength(300);
        
        builder.Property(p => p.Price)
            .IsRequired();
        
        builder.Property(p => p.Note)
            .HasMaxLength(200);
        
        builder.Property(p => p.SpecialNote)
            .HasMaxLength(200);

        var foodId = new Guid("bb059a77-e12c-4182-b798-872b77c28ea5");
        var sweetsId = new Guid("7739170d-02d3-463c-9c61-22e66eed1323");
        var waterId = new Guid("39cc7d9d-656f-44cd-b2af-a6b65bd694ea");
        var fruitsVeggiesId = new Guid("f04987ea-6f6e-4d30-9628-eb5be4cee7e9");
        var spicesId = new Guid("3d0634fe-52ac-488d-a8ba-1257376c0965");
        
        builder.HasData(
            new Product
            {
                Id = new Guid("5f0ab2ae-2167-4069-96c1-97c98af3cfae"),
                Name = "Селедка",
                Description = "Селедка соленая",
                Price = 10.0m,
                CategoryId = foodId,
                Note = "Акция",
                SpecialNote = "Пересоленая"
            },
            new Product
            {
                Id = new Guid("ef9bcabc-9bc5-4405-a675-31c29d249e1a"),
                Name = "Тушенка",
                Description = "Тушенка говяжья",
                Price = 20.0m,
                CategoryId = foodId,
                Note = "Вкусная",
                SpecialNote = "Жилы"
            },
            new Product
            {
                Id = new Guid("446a3a36-4ac8-475f-9694-983a4c67e55b"),
                Name = "Хлеб",
                Description = "Ржаной хлеб",
                Price = 2.5m,
                CategoryId = foodId,
                Note = "Свежий",
                SpecialNote = "Без консервантов"
            },
            new Product
            {
                Id = new Guid("58b7c28a-4fba-4d6e-bba6-8982adf3b429"),
                Name = "Сгущенка",
                Description = "В банках",
                Price = 30.0m,
                CategoryId = sweetsId,
                Note = "С ключом",
                SpecialNote = "Вкусная"
            },
            new Product
            {
                Id = new Guid("7c655000-e4a7-4bdd-8f7c-83c8c0d79711"),
                Name = "Печенье",
                Description = "Овсяное печенье",
                Price = 4.5m,
                CategoryId = sweetsId,
                Note = "С изюмом",
                SpecialNote = "Большое"
            },
            new Product
            {
                Id = new Guid("2a6a5866-1dc3-4153-aa71-fd7ed58dd8e0"),
                Name = "Мармелад",
                Description = "Фруктовый мармелад",
                Price = 2.8m,
                CategoryId = sweetsId,
                Note = "Много вкусов",
                SpecialNote = "Без красителей"
            },
            new Product
            {
                Id = new Guid("b3420a01-18ac-4cf4-ab3f-4bcf2629e94d"),
                Name = "Квас",
                Description = "В бутылках",
                Price = 2.2m,
                CategoryId = waterId,
                Note = "Вятский",
                SpecialNote = "Теплый"
            },
            new Product
            {
                Id = new Guid("daab57fb-c097-4dad-93eb-de69dadf87ff"),
                Name = "Вода",
                Description = "В бутылках",
                Price = 2.0m,
                CategoryId = waterId,
                Note = "Минеральная"
            },
            new Product
            {
                Id = new Guid("b3686849-c3b7-4f08-905e-f6bdff1d05fd"),
                Name = "Сок",
                Description = "В пакетах",
                Price = 3.5m,
                CategoryId = waterId,
                Note = "Виноградный",
                SpecialNote = "Кислый"
            },
            new Product
            {
                Id = new Guid("6017d5be-fc8e-45b4-b561-f4a7ee4a5383"),
                Name = "Яблоки",
                Description = "Голден",
                Price = 3.8m,
                CategoryId = fruitsVeggiesId,
                Note = "Большие",
                SpecialNote = "Новая партия"
            },
            new Product
            {
                Id = new Guid("17159cd6-355a-4e42-952a-59145d1a1fdd"),
                Name = "Помидоры",
                Description = "В коробках",
                Price = 5.2m,
                CategoryId = fruitsVeggiesId,
                Note = "Черри",
                SpecialNote = "На ветке"
            },
            new Product
            {
                Id = new Guid("a4ee39d4-c9bc-4f8a-883f-6a8b6ecdff70"),
                Name = "Бананы",
                Description = "Из Бразилии",
                Price = 2.9m,
                CategoryId = fruitsVeggiesId,
                Note = "Свежие",
                SpecialNote = "Зеленые",
            },
            new Product
            {
                Id = new Guid("97942486-51e8-4935-80ca-2adb6d2d2c55"),
                Name = "Соль",
                Description = "Морская соль",
                Price = 1.5m,
                CategoryId = spicesId,
                Note = "Среднего помола",
                SpecialNote = "Богата минералами",
            },
            new Product
            {
                Id = new Guid("c4ec5301-01b4-4249-b21e-65559689a59c"),
                Name = "Перец",
                Description = "Молотый черный перец",
                Price = 2.8m,
                CategoryId = spicesId,
                Note = "Острый",
            },
            new Product
            {
                Id = new Guid("67f257d8-c76d-4631-a618-5569fac0b3c5"),
                Name = "Паприка",
                Description = "Молотая паприка",
                Price = 3.2m,
                CategoryId = spicesId,
                Note = "Венгерская",
                SpecialNote = "Сладкая"
            }
        );
    }
}