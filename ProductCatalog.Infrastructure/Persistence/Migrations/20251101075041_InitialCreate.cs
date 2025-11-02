using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductCatalog.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Note = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    SpecialNote = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("39cc7d9d-656f-44cd-b2af-a6b65bd694ea"), "Вода" },
                    { new Guid("3d0634fe-52ac-488d-a8ba-1257376c0965"), "Специи" },
                    { new Guid("7739170d-02d3-463c-9c61-22e66eed1323"), "Вкусности" },
                    { new Guid("bb059a77-e12c-4182-b798-872b77c28ea5"), "Еда" },
                    { new Guid("f04987ea-6f6e-4d30-9628-eb5be4cee7e9"), "Фрукты/овощи" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { new Guid("6d259ae6-6f45-4dc7-a780-c9bb81950ee1"), "prouser@gmail.com", "Pro User", "AQAAAAIAAYagAAAAENbW4YvWmFhhwjSMmEsY8jagvNaJ+EIZUmkoFrgA8w04E7rqWUipj5pfNYrDxhJFMQ==", "ProUser" },
                    { new Guid("7ac34c8c-eb1d-48c0-9cf5-92d7bacbaffc"), "admin@gmail.com", "Administrator", "AQAAAAIAAYagAAAAEIeQe+1xlicOfpcjYkptypRASSGGm0wwYMtJns4bunawj2LcRGPwVLUcw3wuLCZc7w==", "Admin" },
                    { new Guid("f0ca277a-aac2-4e54-b246-3f7435aa9449"), "user@gmail.com", "User", "AQAAAAIAAYagAAAAEFj/E2JOZ1wIU25CtVZ9a1PgyswG8gYXxNfKKO28/mO8RWxzTokrbRbgWLc7tGtMlw==", "User" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "Name", "Note", "Price", "SpecialNote" },
                values: new object[,]
                {
                    { new Guid("17159cd6-355a-4e42-952a-59145d1a1fdd"), new Guid("f04987ea-6f6e-4d30-9628-eb5be4cee7e9"), "В коробках", "Помидоры", "Черри", 5.2m, "На ветке" },
                    { new Guid("2a6a5866-1dc3-4153-aa71-fd7ed58dd8e0"), new Guid("7739170d-02d3-463c-9c61-22e66eed1323"), "Фруктовый мармелад", "Мармелад", "Много вкусов", 2.8m, "Без красителей" },
                    { new Guid("446a3a36-4ac8-475f-9694-983a4c67e55b"), new Guid("bb059a77-e12c-4182-b798-872b77c28ea5"), "Ржаной хлеб", "Хлеб", "Свежий", 2.5m, "Без консервантов" },
                    { new Guid("58b7c28a-4fba-4d6e-bba6-8982adf3b429"), new Guid("7739170d-02d3-463c-9c61-22e66eed1323"), "В банках", "Сгущенка", "С ключом", 30.0m, "Вкусная" },
                    { new Guid("5f0ab2ae-2167-4069-96c1-97c98af3cfae"), new Guid("bb059a77-e12c-4182-b798-872b77c28ea5"), "Селедка соленая", "Селедка", "Акция", 10.0m, "Пересоленая" },
                    { new Guid("6017d5be-fc8e-45b4-b561-f4a7ee4a5383"), new Guid("f04987ea-6f6e-4d30-9628-eb5be4cee7e9"), "Голден", "Яблоки", "Большие", 3.8m, "Новая партия" },
                    { new Guid("67f257d8-c76d-4631-a618-5569fac0b3c5"), new Guid("3d0634fe-52ac-488d-a8ba-1257376c0965"), "Молотая паприка", "Паприка", "Венгерская", 3.2m, "Сладкая" },
                    { new Guid("7c655000-e4a7-4bdd-8f7c-83c8c0d79711"), new Guid("7739170d-02d3-463c-9c61-22e66eed1323"), "Овсяное печенье", "Печенье", "С изюмом", 4.5m, "Большое" },
                    { new Guid("97942486-51e8-4935-80ca-2adb6d2d2c55"), new Guid("3d0634fe-52ac-488d-a8ba-1257376c0965"), "Морская соль", "Соль", "Среднего помола", 1.5m, "Богата минералами" },
                    { new Guid("a4ee39d4-c9bc-4f8a-883f-6a8b6ecdff70"), new Guid("f04987ea-6f6e-4d30-9628-eb5be4cee7e9"), "Из Бразилии", "Бананы", "Свежие", 2.9m, "Зеленые" },
                    { new Guid("b3420a01-18ac-4cf4-ab3f-4bcf2629e94d"), new Guid("39cc7d9d-656f-44cd-b2af-a6b65bd694ea"), "В бутылках", "Квас", "Вятский", 2.2m, "Теплый" },
                    { new Guid("b3686849-c3b7-4f08-905e-f6bdff1d05fd"), new Guid("39cc7d9d-656f-44cd-b2af-a6b65bd694ea"), "В пакетах", "Сок", "Виноградный", 3.5m, "Кислый" },
                    { new Guid("c4ec5301-01b4-4249-b21e-65559689a59c"), new Guid("3d0634fe-52ac-488d-a8ba-1257376c0965"), "Молотый черный перец", "Перец", "Острый", 2.8m, null },
                    { new Guid("daab57fb-c097-4dad-93eb-de69dadf87ff"), new Guid("39cc7d9d-656f-44cd-b2af-a6b65bd694ea"), "В бутылках", "Вода", "Минеральная", 2.0m, null },
                    { new Guid("ef9bcabc-9bc5-4405-a675-31c29d249e1a"), new Guid("bb059a77-e12c-4182-b798-872b77c28ea5"), "Тушенка говяжья", "Тушенка", "Вкусная", 20.0m, "Жилы" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
