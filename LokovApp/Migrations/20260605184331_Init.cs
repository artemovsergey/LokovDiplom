using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LokovApp.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Details = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brigades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ForemanName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    WorkersCount = table.Column<int>(type: "integer", nullable: false),
                    Specialization = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brigades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Patronymic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AdditionalPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    ActualCost = table.Column<decimal>(type: "numeric(15,2)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PlannedEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BrigadeId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Brigades_BrigadeId",
                        column: x => x.BrigadeId,
                        principalTable: "Brigades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientInteractions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Result = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    InteractionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientInteractions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientInteractions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientInteractions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectDocuments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectDocuments_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectExpenses_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    PlannedDays = table.Column<int>(type: "integer", nullable: false),
                    ActualDays = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletionPhoto = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStages_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AssignedToId = table.Column<Guid>(type: "uuid", nullable: true),
                    BrigadeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Brigades_BrigadeId",
                        column: x => x.BrigadeId,
                        principalTable: "Brigades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Users_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Brigades",
                columns: new[] { "Id", "CreatedAt", "ForemanName", "IsActive", "Name", "Phone", "Specialization", "WorkersCount" },
                values: new object[,]
                {
                    { new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Морозов И.П.", true, "Бригада №1 (Универсальная)", "+79011112233", 0, 5 },
                    { new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Волков Н.А.", true, "Бригада №2 (Кровельная)", "+79012223344", 1, 4 },
                    { new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Зайцев П.В.", true, "Бригада №3 (Фасадная)", "+79013334455", 2, 6 },
                    { new Guid("b4b4b4b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Новиков Р.Г.", true, "Бригада №4 (Отделочная)", "+79014445566", 3, 4 },
                    { new Guid("b5b5b5b5-b5b5-b5b5-b5b5-b5b5b5b5b5b5"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Федоров А.С.", true, "Бригада №5 (Универсальная)", "+79015556677", 0, 5 }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "AdditionalPhone", "Address", "ArchivedAt", "Category", "CreatedAt", "Email", "FirstName", "IsDeleted", "LastName", "Patronymic", "Phone", "Source", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c0a00000-0000-0000-0000-00000000000a"), null, "г. Москва, ул. Южная, д. 30, кв. 90", null, 0, new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "alekseev@email.ru", "Алексей", false, "Алексеев", "Михайлович", "+79011110010", 0, 3, null },
                    { new Guid("c0b00000-0000-0000-0000-00000000000b"), null, "г. Москва, ул. Северная, д. 8, кв. 44", null, 0, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), "titova@email.ru", "Татьяна", false, "Титова", "Николаевна", "+79011110011", 1, 0, null },
                    { new Guid("c0c00000-0000-0000-0000-00000000000c"), null, "г. Москва, ул. Бизнес, д. 10, офис 305", null, 1, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "info@stroyinvest.ru", "ООО", false, "СтройИнвест", null, "+74951112233", 4, 1, null },
                    { new Guid("c0d00000-0000-0000-0000-00000000000d"), null, "г. Москва, ул. Торговая, д. 15", null, 2, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sidorov@biznes.ru", "ИП", false, "Сидоров А.В.", null, "+79011110012", 0, 1, null },
                    { new Guid("c0e00000-0000-0000-0000-00000000000e"), null, "г. Москва, ул. Садовая, д. 40, кв. 12", null, 0, new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "vladimirov@email.ru", "Владимир", false, "Владимиров", "Романович", "+79011110013", 3, 0, null },
                    { new Guid("c0f00000-0000-0000-0000-00000000000f"), null, "г. Москва, ул. Цветочная, д. 3, кв. 56", null, 0, new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), "naumova@email.ru", "Наталья", false, "Наумова", "Владимировна", "+79011110014", 2, 2, null },
                    { new Guid("c1000000-0000-0000-0000-000000000001"), null, "г. Москва, ул. Ленина, д. 1, кв. 1", null, 0, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ivanov@email.ru", "Иван", false, "Иванов", "Иванович", "+79011110001", 0, 1, null },
                    { new Guid("c2000000-0000-0000-0000-000000000002"), null, "г. Москва, ул. Мира, д. 15, кв. 42", null, 0, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "petrov@email.ru", "Петр", false, "Петров", "Петрович", "+79011110002", 1, 1, null },
                    { new Guid("c3000000-0000-0000-0000-000000000003"), null, "г. Москва, ул. Пушкина, д. 10, кв. 5", null, 0, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "sergeev@email.ru", "Сергей", false, "Сергеев", "Сергеевич", "+79011110003", 2, 3, null },
                    { new Guid("c4000000-0000-0000-0000-000000000004"), null, "г. Москва, ул. Гагарина, д. 20, кв. 78", null, 0, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "antonova@email.ru", "Анна", false, "Антонова", "Викторовна", "+79011110004", 4, 0, null },
                    { new Guid("c5000000-0000-0000-0000-000000000005"), null, "г. Москва, ул. Строителей, д. 5, кв. 120", null, 0, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "markova@email.ru", "Мария", false, "Маркова", "Александровна", "+79011110005", 3, 1, null },
                    { new Guid("c6000000-0000-0000-0000-000000000006"), null, "г. Москва, пр. Вернадского, д. 78, кв. 15", null, 0, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "dmitriev@email.ru", "Дмитрий", false, "Дмитриев", "Анатольевич", "+79011110006", 0, 3, null },
                    { new Guid("c7000000-0000-0000-0000-000000000007"), null, "г. Москва, ул. Тверская, д. 25, кв. 8", null, 0, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), "orlova@email.ru", "Ольга", false, "Орлова", "Сергеевна", "+79011110007", 1, 2, null },
                    { new Guid("c8000000-0000-0000-0000-000000000008"), null, "г. Москва, ул. Арбат, д. 12, кв. 33", null, 0, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "nikolaev@email.ru", "Николай", false, "Николаев", "Павлович", "+79011110008", 4, 0, null },
                    { new Guid("c9000000-0000-0000-0000-000000000009"), null, "г. Москва, ул. Профсоюзная, д. 45, кв. 67", null, 0, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "egorova@email.ru", "Екатерина", false, "Егорова", "Игоревна", "+79011110009", 2, 1, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "IsActive", "LastLogin", "PasswordHash", "Role", "TwoFactorEnabled", "Username" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@lokov.ru", "Администратор Системы", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 0, false, "admin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager1@lokov.ru", "Петров Сергей Иванович", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 1, false, "manager1" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager2@lokov.ru", "Кузнецова Анна Владимировна", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 1, false, "manager2" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "foreman1@lokov.ru", "Смирнов Алексей Дмитриевич", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 2, false, "foreman1" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "foreman2@lokov.ru", "Иванов Дмитрий Сергеевич", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 2, false, "foreman2" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "accountant@lokov.ru", "Соколова Елена Андреевна", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 3, false, "accountant" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "brigadier1@lokov.ru", "Морозов Игорь Петрович", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 4, false, "brigadier1" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "brigadier2@lokov.ru", "Волков Николай Александрович", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 4, false, "brigadier2" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "brigadier3@lokov.ru", "Зайцев Павел Викторович", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 4, false, "brigadier3" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "brigadier4@lokov.ru", "Новиков Роман Геннадьевич", true, null, "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==", 4, false, "brigadier4" }
                });

            migrationBuilder.InsertData(
                table: "ClientInteractions",
                columns: new[] { "Id", "ClientId", "CreatedAt", "CreatedById", "Description", "InteractionDate", "IsDeleted", "Result", "Type" },
                values: new object[,]
                {
                    { new Guid("02000000-0000-0000-0000-000000000001"), new Guid("c1000000-0000-0000-0000-000000000001"), new DateTime(2025, 12, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "Звонок по заявке с сайта", new DateTime(2025, 12, 2, 0, 0, 0, 0, DateTimeKind.Utc), false, "Договорились о встрече", 0 },
                    { new Guid("02000000-0000-0000-0000-000000000002"), new Guid("c1000000-0000-0000-0000-000000000001"), new DateTime(2025, 12, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "Осмотр квартиры для ремонта", new DateTime(2025, 12, 6, 0, 0, 0, 0, DateTimeKind.Utc), false, "Сделали замеры, обсудили пожелания", 1 },
                    { new Guid("02000000-0000-0000-0000-000000000003"), new Guid("c2000000-0000-0000-0000-000000000002"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), "Первичный звонок клиента", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), false, "Нужна замена кровли", 0 },
                    { new Guid("02000000-0000-0000-0000-000000000004"), new Guid("c3000000-0000-0000-0000-000000000003"), new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "Обсуждение комплексного проекта", new DateTime(2025, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), false, "Подписали предварительное соглашение", 1 },
                    { new Guid("02000000-0000-0000-0000-000000000005"), new Guid("c6000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "Повторное обращение по кровле", new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), false, "Согласовали дату осмотра", 0 },
                    { new Guid("02000000-0000-0000-0000-000000000006"), new Guid("c0c00000-0000-0000-0000-00000000000c"), new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), "Встреча с директором ООО СтройИнвест", new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), false, "Обсудили два проекта", 1 },
                    { new Guid("02000000-0000-0000-0000-000000000007"), new Guid("c5000000-0000-0000-0000-000000000005"), new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "Отправлено коммерческое предложение", new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), false, "Клиент согласился", 2 },
                    { new Guid("02000000-0000-0000-0000-000000000008"), new Guid("c8000000-0000-0000-0000-000000000008"), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), "Сообщение в WhatsApp", new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), false, "Интересуется замена проводки", 3 },
                    { new Guid("02000000-0000-0000-0000-000000000009"), new Guid("c0b00000-0000-0000-0000-00000000000b"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "Звонок по рекламе", new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), false, "Нужна облицовка цоколя", 0 },
                    { new Guid("02000000-0000-0000-0000-00000000000a"), new Guid("c0e00000-0000-0000-0000-00000000000e"), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), "Первичное обращение", new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), false, "Интересует ремонт студии", 0 }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "ClientId", "Content", "CreatedAt", "CreatedById", "IsDeleted" },
                values: new object[,]
                {
                    { new Guid("03000000-0000-0000-0000-000000000001"), new Guid("c1000000-0000-0000-0000-000000000001"), "Очень требовательный клиент, важно соблюдать сроки", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false },
                    { new Guid("03000000-0000-0000-0000-000000000002"), new Guid("c3000000-0000-0000-0000-000000000003"), "Постоянный клиент, уже третий проект. Предоставить скидку 5%", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false },
                    { new Guid("03000000-0000-0000-0000-000000000003"), new Guid("c6000000-0000-0000-0000-000000000006"), "Платежеспособный клиент, рекомендовал нас своим знакомым", new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false },
                    { new Guid("03000000-0000-0000-0000-000000000004"), new Guid("c0c00000-0000-0000-0000-00000000000c"), "Крупный корпоративный клиент, перспективный для долгосрочного сотрудничества", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), false },
                    { new Guid("03000000-0000-0000-0000-000000000005"), new Guid("c9000000-0000-0000-0000-000000000009"), "Клиент попросил дополнительную скидку на материалы", new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), false }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "ActualCost", "ActualEndDate", "Address", "BrigadeId", "ClientId", "CreatedAt", "Description", "EstimatedCost", "IsDeleted", "Name", "Number", "PlannedEndDate", "StartDate", "Status", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("d0a00000-0000-0000-0000-00000000000a"), 0m, null, "г. Москва, ул. Бизнес, д. 10", new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new Guid("c0c00000-0000-0000-0000-00000000000c"), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Полная замена кровельного покрытия на площади 1200 м²", 3500000m, false, "Капитальный ремонт кровли бизнес-центра", "ROOF-2026-010", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 2, null },
                    { new Guid("d0a10000-0000-0000-0000-0000000000a1"), 0m, null, "г. Москва, ул. Северная, д. 8", new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new Guid("c0b00000-0000-0000-0000-00000000000b"), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Облицовка цоколя керамогранитом", 120000m, false, "Облицовка цоколя", "FAC-2026-016", new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), 3, 3, null },
                    { new Guid("d0b00000-0000-0000-0000-00000000000b"), 600000m, null, "г. Москва, ул. Торговая, д. 15", new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("c0d00000-0000-0000-0000-00000000000d"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Капитальный ремонт с перепланировкой", 950000m, false, "Ремонт торгового павильона", "KR-2026-011", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 0, null },
                    { new Guid("d0b20000-0000-0000-0000-0000000000b2"), 0m, null, "г. Москва, ул. Садовая, д. 40, кв. 12", new Guid("b4b4b4b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), new Guid("c0e00000-0000-0000-0000-00000000000e"), new DateTime(2026, 5, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Дизайнерский ремонт студии 25 м²", 550000m, false, "Ремонт квартиры студии", "KR-2026-017", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 22, 0, 0, 0, 0, DateTimeKind.Utc), 2, 0, null },
                    { new Guid("d0c00000-0000-0000-0000-00000000000c"), 0m, null, "г. Москва, ул. Бизнес, д. 15", new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new Guid("c0c00000-0000-0000-0000-00000000000c"), new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Система вентилируемого фасада", 2800000m, false, "Утепление фасада офисного здания", "FAC-2026-012", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 31, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3, null },
                    { new Guid("d0c30000-0000-0000-0000-0000000000c3"), 0m, null, "г. Москва, ул. Гагарина, д. 20", new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new Guid("c4000000-0000-0000-0000-000000000004"), new DateTime(2026, 5, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Установка водосточной системы", 65000m, false, "Монтаж водостоков", "ROOF-2026-018", null, null, 0, 2, null },
                    { new Guid("d0d00000-0000-0000-0000-00000000000d"), 220000m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "г. Москва, ул. Тверская, д. 25, кв. 8", new Guid("b4b4b4b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), new Guid("c7000000-0000-0000-0000-000000000007"), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Замена кухонного гарнитура, фартука, освещения", 220000m, false, "Ремонт кухни", "KR-2026-013", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 1, null },
                    { new Guid("d0d40000-0000-0000-0000-0000000000d4"), 0m, null, "г. Москва, ул. Арбат, д. 12, кв. 33", new Guid("b5b5b5b5-b5b5-b5b5-b5b5-b5b5b5b5b5b5"), new Guid("c8000000-0000-0000-0000-000000000008"), new DateTime(2026, 5, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Полная замена проводки в квартире", 85000m, false, "Замена электропроводки", "KR-2026-019", null, null, 2, 1, null },
                    { new Guid("d0e00000-0000-0000-0000-00000000000e"), 0m, null, "г. Москва, ул. Цветочная, д. 3", new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new Guid("c0f00000-0000-0000-0000-00000000000f"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Замена профнастила, ремонт стропильной системы", 95000m, false, "Ремонт кровли гаража", "ROOF-2026-014", new DateTime(2026, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), 6, 2, null },
                    { new Guid("d0e50000-0000-0000-0000-0000000000e5"), 450000m, new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "г. Москва, ул. Складская, д. 5", new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new Guid("c0c00000-0000-0000-0000-00000000000c"), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Подготовка и покраска фасада складского помещения", 450000m, false, "Покраска фасада склада", "FAC-2026-020", new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 3, null },
                    { new Guid("d0f00000-0000-0000-0000-00000000000f"), 740000m, new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "г. Москва, ул. Южная, д. 30, кв. 90", new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("c0a00000-0000-0000-0000-00000000000a"), new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Полный ремонт с заменой всего", 750000m, false, "Капитальный ремонт 1-комнатной квартиры", "KR-2026-015", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 0, null },
                    { new Guid("d1000000-0000-0000-0000-000000000001"), 425000m, null, "г. Москва, ул. Ленина, д. 1, кв. 1", new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("c1000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Полный капитальный ремонт с заменой коммуникаций", 850000m, false, "Капитальный ремонт 2-комнатной квартиры", "KR-2026-001", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 0, null },
                    { new Guid("d2000000-0000-0000-0000-000000000002"), 380000m, null, "г. Москва, ул. Мира, д. 15", new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new Guid("c2000000-0000-0000-0000-000000000002"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Демонтаж старой кровли, монтаж металлочерепицы, утепление", 420000m, false, "Замена кровли частного дома", "ROOF-2026-002", new DateTime(2026, 6, 11, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 2, null },
                    { new Guid("d3000000-0000-0000-0000-000000000003"), 800000m, null, "г. Москва, ул. Пушкина, д. 10", new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new Guid("c3000000-0000-0000-0000-000000000003"), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Утепление фасада минеральной ватой, декоративная штукатурка", 1200000m, false, "Утепление фасада многоквартирного дома", "FAC-2026-003", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), 6, 3, null },
                    { new Guid("d4000000-0000-0000-0000-000000000004"), 0m, null, "г. Москва, ул. Строителей, д. 5, кв. 120", new Guid("b4b4b4b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), new Guid("c5000000-0000-0000-0000-000000000005"), new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Черновая отделка, электрика, сантехника, чистовая отделка", 1500000m, false, "Ремонт 3-комнатной квартиры в новостройке", "KR-2026-004", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 4, 0, null },
                    { new Guid("d5000000-0000-0000-0000-000000000005"), 680000m, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "г. Москва, пр. Вернадского, д. 78", new Guid("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2"), new Guid("c6000000-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Гидроизоляция, ремонт примыканий, установка аэраторов", 680000m, false, "Ремонт плоской кровли офисного здания", "ROOF-2026-005", new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 2, null },
                    { new Guid("d6000000-0000-0000-0000-000000000006"), 270000m, new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), "г. Москва, пр. Вернадского, д. 78, кв. 15", new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("c6000000-0000-0000-0000-000000000006"), new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Замена плитки, сантехники, установка душевой кабины", 280000m, false, "Ремонт ванной комнаты и санузла", "KR-2026-006", new DateTime(2025, 11, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 1, null },
                    { new Guid("d7000000-0000-0000-0000-000000000007"), 150000m, null, "г. Москва, ул. Профсоюзная, д. 45", new Guid("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3"), new Guid("c9000000-0000-0000-0000-000000000009"), new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Очистка фасада, ремонт трещин, покраска", 350000m, false, "Ремонт фасада загородного дома", "FAC-2026-007", new DateTime(2026, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), 6, 3, null },
                    { new Guid("d8000000-0000-0000-0000-000000000008"), 2200000m, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "г. Москва, ул. Пушкина, д. 10", new Guid("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1"), new Guid("c3000000-0000-0000-0000-000000000003"), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ремонт кровли, фасада и внутренних помещений", 2500000m, false, "Комплексный ремонт дома", "KR-2026-008", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 4, null },
                    { new Guid("d9000000-0000-0000-0000-000000000009"), 180000m, new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "г. Москва, ул. Ленина, д. 1, кв. 1", new Guid("b4b4b4b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4"), new Guid("c1000000-0000-0000-0000-000000000001"), new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Демонтаж старых окон, установка ПВХ, отделка откосов", 180000m, false, "Замена окон и балконного остекления", "KR-2026-009", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 8, 1, null }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "Comment", "CreatedAt", "CreatedById", "IsDeleted", "Method", "PaymentDate", "ProjectId", "Type" },
                values: new object[,]
                {
                    { new Guid("f1000000-0000-0000-0000-000000000001"), 425000m, "Аванс 50%", new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 1, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d1000000-0000-0000-0000-000000000001"), 0 },
                    { new Guid("f1000000-0000-0000-0000-000000000002"), 210000m, "Аванс 50%", new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 1, new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d2000000-0000-0000-0000-000000000002"), 0 },
                    { new Guid("f1000000-0000-0000-0000-000000000003"), 170000m, "Промежуточная оплата", new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 1, new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d2000000-0000-0000-0000-000000000002"), 1 },
                    { new Guid("f1000000-0000-0000-0000-000000000004"), 600000m, "Аванс 50%", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), false, 1, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d3000000-0000-0000-0000-000000000003"), 0 },
                    { new Guid("f1000000-0000-0000-0000-000000000005"), 200000m, "Промежуточный платеж", new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), false, 1, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d3000000-0000-0000-0000-000000000003"), 1 },
                    { new Guid("f1000000-0000-0000-0000-000000000006"), 340000m, null, new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 1, new DateTime(2026, 1, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d5000000-0000-0000-0000-000000000005"), 0 },
                    { new Guid("f1000000-0000-0000-0000-000000000007"), 340000m, null, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 1, new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d5000000-0000-0000-0000-000000000005"), 2 },
                    { new Guid("f1000000-0000-0000-0000-000000000008"), 140000m, null, new DateTime(2025, 10, 3, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 0, new DateTime(2025, 10, 3, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d6000000-0000-0000-0000-000000000006"), 0 },
                    { new Guid("f1000000-0000-0000-0000-000000000009"), 130000m, null, new DateTime(2025, 11, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 0, new DateTime(2025, 11, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d6000000-0000-0000-0000-000000000006"), 2 },
                    { new Guid("f1000000-0000-0000-0000-00000000000a"), 1200000m, null, new DateTime(2025, 8, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 1, new DateTime(2025, 8, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d8000000-0000-0000-0000-000000000008"), 0 },
                    { new Guid("f1000000-0000-0000-0000-00000000000b"), 1000000m, null, new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 1, new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d8000000-0000-0000-0000-000000000008"), 2 },
                    { new Guid("f1000000-0000-0000-0000-00000000000c"), 475000m, "Аванс 50%", new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), false, 1, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d0b00000-0000-0000-0000-00000000000b"), 0 },
                    { new Guid("f1000000-0000-0000-0000-00000000000d"), 375000m, null, new DateTime(2025, 4, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 1, new DateTime(2025, 4, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d0f00000-0000-0000-0000-00000000000f"), 0 },
                    { new Guid("f1000000-0000-0000-0000-00000000000e"), 365000m, null, new DateTime(2025, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), false, 1, new DateTime(2025, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d0f00000-0000-0000-0000-00000000000f"), 2 },
                    { new Guid("f1000000-0000-0000-0000-00000000000f"), 450000m, null, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), false, 1, new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("d0e50000-0000-0000-0000-0000000000e5"), 2 }
                });

            migrationBuilder.InsertData(
                table: "ProjectDocuments",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "FilePath", "IsDeleted", "Number", "ProjectId", "Type" },
                values: new object[,]
                {
                    { new Guid("01000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "/documents/КП-2026-001.pdf", false, "КП-2026-001", new Guid("d1000000-0000-0000-0000-000000000001"), 0 },
                    { new Guid("01000000-0000-0000-0000-000000000002"), new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "/documents/Д-2026-001.pdf", false, "Д-2026-001", new Guid("d1000000-0000-0000-0000-000000000001"), 1 },
                    { new Guid("01000000-0000-0000-0000-000000000003"), new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "/documents/Д-2026-002.pdf", false, "Д-2026-002", new Guid("d2000000-0000-0000-0000-000000000002"), 1 },
                    { new Guid("01000000-0000-0000-0000-000000000004"), new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "/documents/А-2026-005.pdf", false, "А-2026-005", new Guid("d5000000-0000-0000-0000-000000000005"), 3 },
                    { new Guid("01000000-0000-0000-0000-000000000005"), new DateTime(2025, 11, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "/documents/А-2026-006.pdf", false, "А-2026-006", new Guid("d6000000-0000-0000-0000-000000000006"), 3 },
                    { new Guid("01000000-0000-0000-0000-000000000006"), new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "/documents/А-2026-008.pdf", false, "А-2026-008", new Guid("d8000000-0000-0000-0000-000000000008"), 3 },
                    { new Guid("01000000-0000-0000-0000-000000000007"), new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), "/documents/Д-2026-003.pdf", false, "Д-2026-003", new Guid("d3000000-0000-0000-0000-000000000003"), 1 },
                    { new Guid("01000000-0000-0000-0000-000000000008"), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333"), "/documents/Д-2026-011.pdf", false, "Д-2026-011", new Guid("d0b00000-0000-0000-0000-00000000000b"), 1 },
                    { new Guid("01000000-0000-0000-0000-000000000009"), new DateTime(2026, 5, 24, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "/documents/КП-2026-004.pdf", false, "КП-2026-004", new Guid("d4000000-0000-0000-0000-000000000004"), 0 },
                    { new Guid("01000000-0000-0000-0000-00000000000a"), new DateTime(2026, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222"), "/documents/Д-2026-004.pdf", false, "Д-2026-004", new Guid("d4000000-0000-0000-0000-000000000004"), 1 }
                });

            migrationBuilder.InsertData(
                table: "ProjectExpenses",
                columns: new[] { "Id", "Amount", "Category", "Date", "Description", "IsDeleted", "Name", "ProjectId" },
                values: new object[,]
                {
                    { new Guid("04000000-0000-0000-0000-000000000001"), 250000m, 0, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Закупка основных материалов", false, "Строительные материалы", new Guid("d1000000-0000-0000-0000-000000000001") },
                    { new Guid("04000000-0000-0000-0000-000000000002"), 120000m, 1, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Оплата рабочим", new Guid("d1000000-0000-0000-0000-000000000001") },
                    { new Guid("04000000-0000-0000-0000-000000000003"), 15000m, 2, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Транспортные расходы", new Guid("d1000000-0000-0000-0000-000000000001") },
                    { new Guid("04000000-0000-0000-0000-000000000004"), 200000m, 0, new DateTime(2026, 5, 7, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Металлочерепица", new Guid("d2000000-0000-0000-0000-000000000002") },
                    { new Guid("04000000-0000-0000-0000-000000000005"), 100000m, 1, new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Оплата кровельщикам", new Guid("d2000000-0000-0000-0000-000000000002") },
                    { new Guid("04000000-0000-0000-0000-000000000006"), 400000m, 0, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Минеральная вата", new Guid("d3000000-0000-0000-0000-000000000003") },
                    { new Guid("04000000-0000-0000-0000-000000000007"), 250000m, 1, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Оплата фасадчикам", new Guid("d3000000-0000-0000-0000-000000000003") },
                    { new Guid("04000000-0000-0000-0000-000000000008"), 1200000m, 0, new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Строительные материалы", new Guid("d8000000-0000-0000-0000-000000000008") },
                    { new Guid("04000000-0000-0000-0000-000000000009"), 700000m, 1, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Оплата работ", new Guid("d8000000-0000-0000-0000-000000000008") },
                    { new Guid("04000000-0000-0000-0000-00000000000a"), 300000m, 0, new DateTime(2026, 5, 12, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Материалы для ремонта", new Guid("d0b00000-0000-0000-0000-00000000000b") }
                });

            migrationBuilder.InsertData(
                table: "ProjectStages",
                columns: new[] { "Id", "ActualDays", "CompletedAt", "CompletionPhoto", "IsDeleted", "Name", "Notes", "Order", "PlannedDays", "ProjectId", "StartedAt", "Status" },
                values: new object[,]
                {
                    { new Guid("e1000000-0000-0000-0000-000000000001"), 1, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Осмотр объекта", null, 1, 1, new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e1000000-0000-0000-0000-000000000002"), 2, new DateTime(2026, 4, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Расчет сметы", null, 2, 3, new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 2, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e1000000-0000-0000-0000-000000000003"), 5, new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Согласование", null, 3, 5, new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 3, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e1000000-0000-0000-0000-000000000004"), 3, new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Договор и предоплата", null, 4, 3, new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 8, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e1000000-0000-0000-0000-000000000005"), 5, new DateTime(2026, 4, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Закупка материалов", null, 5, 7, new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 11, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e1000000-0000-0000-0000-000000000006"), 4, new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Демонтажные работы", null, 6, 5, new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 16, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e1000000-0000-0000-0000-000000000007"), 10, new DateTime(2026, 4, 16, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Черновая отделка", null, 7, 14, new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 6, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e1000000-0000-0000-0000-000000000008"), null, null, null, false, "Чистовая отделка", null, 8, 21, new Guid("d1000000-0000-0000-0000-000000000001"), new DateTime(2026, 4, 16, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { new Guid("e1000000-0000-0000-0000-000000000009"), null, null, null, false, "Сдача объекта", null, 9, 2, new Guid("d1000000-0000-0000-0000-000000000001"), null, 0 },
                    { new Guid("e2000000-0000-0000-0000-000000000001"), 1, new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Осмотр кровли", null, 1, 1, new Guid("d2000000-0000-0000-0000-000000000002"), new DateTime(2026, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e2000000-0000-0000-0000-000000000002"), 2, new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Демонтаж старой кровли", null, 2, 3, new Guid("d2000000-0000-0000-0000-000000000002"), new DateTime(2026, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e2000000-0000-0000-0000-000000000003"), 3, new DateTime(2026, 5, 8, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Монтаж обрешетки", null, 3, 4, new Guid("d2000000-0000-0000-0000-000000000002"), new DateTime(2026, 5, 4, 0, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { new Guid("e2000000-0000-0000-0000-000000000004"), null, null, null, false, "Укладка металлочерепицы", null, 4, 5, new Guid("d2000000-0000-0000-0000-000000000002"), new DateTime(2026, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { new Guid("e2000000-0000-0000-0000-000000000005"), null, null, null, false, "Монтаж водостоков", null, 5, 3, new Guid("d2000000-0000-0000-0000-000000000002"), null, 0 },
                    { new Guid("e2000000-0000-0000-0000-000000000006"), null, null, null, false, "Сдача работы", null, 6, 1, new Guid("d2000000-0000-0000-0000-000000000002"), null, 0 },
                    { new Guid("e5000000-0000-0000-0000-000000000001"), 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Осмотр объекта", null, 1, 1, new Guid("d5000000-0000-0000-0000-000000000005"), null, 2 },
                    { new Guid("e5000000-0000-0000-0000-000000000002"), 13, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Ремонт кровли", null, 2, 15, new Guid("d5000000-0000-0000-0000-000000000005"), null, 2 },
                    { new Guid("e5000000-0000-0000-0000-000000000003"), 1, new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, "Сдача объекта", null, 3, 1, new Guid("d5000000-0000-0000-0000-000000000005"), null, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientInteractions_ClientId",
                table: "ClientInteractions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientInteractions_CreatedById",
                table: "ClientInteractions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Phone",
                table: "Clients",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Status",
                table: "Clients",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ClientId",
                table: "Comments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedById",
                table: "Comments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CreatedById",
                table: "Payments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentDate",
                table: "Payments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ProjectId",
                table: "Payments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDocuments_CreatedById",
                table: "ProjectDocuments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectDocuments_ProjectId",
                table: "ProjectDocuments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectExpenses_ProjectId",
                table: "ProjectExpenses",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BrigadeId",
                table: "Projects",
                column: "BrigadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientId",
                table: "Projects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Number",
                table: "Projects",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Status",
                table: "Projects",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStages_ProjectId",
                table: "ProjectStages",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToId",
                table: "Tasks",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_BrigadeId",
                table: "Tasks",
                column: "BrigadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ClientInteractions");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProjectDocuments");

            migrationBuilder.DropTable(
                name: "ProjectExpenses");

            migrationBuilder.DropTable(
                name: "ProjectStages");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Brigades");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
