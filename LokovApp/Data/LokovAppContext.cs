using LokovApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LokovApp.Data;

public class LokovAppContext : DbContext
{
    public LokovAppContext(DbContextOptions<LokovAppContext> options)
        : base(options) { }

    public DbSet<Client> Clients { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectStage> ProjectStages { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<ProjectExpense> ProjectExpenses { get; set; }
    public DbSet<ProjectDocument> ProjectDocuments { get; set; }
    public DbSet<Brigade> Brigades { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<ClientInteraction> ClientInteractions { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<ProjectPhoto> ProjectPhotos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Подавление предупреждения о PendingModelChangesWarning в development
        // optionsBuilder.ConfigureWarnings(warnings =>
        //     warnings.Log(RelationalEventId.PendingModelChangesWarning)
        // );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Индексы
        modelBuilder.Entity<Client>().HasIndex(c => c.Phone).IsUnique();

        modelBuilder.Entity<Client>().HasIndex(c => c.Email);

        modelBuilder.Entity<Client>().HasIndex(c => c.Status);

        modelBuilder.Entity<Project>().HasIndex(p => p.Number).IsUnique();

        modelBuilder.Entity<Project>().HasIndex(p => p.Status);

        modelBuilder.Entity<Project>().HasIndex(p => p.ClientId);

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        modelBuilder.Entity<Payment>().HasIndex(p => p.ProjectId);

        modelBuilder.Entity<Payment>().HasIndex(p => p.PaymentDate);

        // Глобальный фильтр для мягкого удаления
        modelBuilder.Entity<Client>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Project>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<Payment>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<ProjectExpense>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ProjectDocument>().HasQueryFilter(d => !d.IsDeleted);
        modelBuilder.Entity<ProjectStage>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<ClientInteraction>().HasQueryFilter(i => !i.IsDeleted);
        modelBuilder.Entity<Comment>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<TaskItem>().HasQueryFilter(t => !t.IsDeleted);

        // Добавить индексы для фотографий
        modelBuilder.Entity<ProjectPhoto>().HasIndex(p => p.ProjectId);

        modelBuilder.Entity<ProjectPhoto>().HasIndex(p => p.StageId);

        modelBuilder.Entity<ProjectPhoto>().HasIndex(p => p.Category);

        modelBuilder.Entity<ProjectPhoto>().HasQueryFilter(p => !p.IsDeleted);

        // Добавить связь с этапом
        modelBuilder
            .Entity<ProjectPhoto>()
            .HasOne(p => p.Stage)
            .WithMany()
            .HasForeignKey(p => p.StageId)
            .OnDelete(DeleteBehavior.SetNull);

        // SEED DATA - Все значения статические (hardcoded GUID и DateTime)

        var seedDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        #region Users Seed Data
        var user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111"); // Admin
        var user2Id = Guid.Parse("22222222-2222-2222-2222-222222222222"); // Manager 1
        var user3Id = Guid.Parse("33333333-3333-3333-3333-333333333333"); // Manager 2
        var user4Id = Guid.Parse("44444444-4444-4444-4444-444444444444"); // Foreman 1
        var user5Id = Guid.Parse("55555555-5555-5555-5555-555555555555"); // Foreman 2
        var user6Id = Guid.Parse("66666666-6666-6666-6666-666666666666"); // Accountant
        var user7Id = Guid.Parse("77777777-7777-7777-7777-777777777777"); // Brigadier 1
        var user8Id = Guid.Parse("88888888-8888-8888-8888-888888888888"); // Brigadier 2
        var user9Id = Guid.Parse("99999999-9999-9999-9999-999999999999"); // Brigadier 3
        var user10Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"); // Brigadier 4

        modelBuilder
            .Entity<User>()
            .HasData(
                new User
                {
                    Id = user1Id,
                    Username = "admin",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "admin@lokov.ru",
                    FullName = "Администратор Системы",
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new User
                {
                    Id = user2Id,
                    Username = "manager1",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "manager1@lokov.ru",
                    FullName = "Петров Сергей Иванович",
                    Role = UserRole.Manager,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new User
                {
                    Id = user3Id,
                    Username = "manager2",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "manager2@lokov.ru",
                    FullName = "Кузнецова Анна Владимировна",
                    Role = UserRole.Manager,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new User
                {
                    Id = user4Id,
                    Username = "foreman1",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "foreman1@lokov.ru",
                    FullName = "Смирнов Алексей Дмитриевич",
                    Role = UserRole.Foreman,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new User
                {
                    Id = user5Id,
                    Username = "foreman2",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "foreman2@lokov.ru",
                    FullName = "Иванов Дмитрий Сергеевич",
                    Role = UserRole.Foreman,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new User
                {
                    Id = user6Id,
                    Username = "accountant",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "accountant@lokov.ru",
                    FullName = "Соколова Елена Андреевна",
                    Role = UserRole.Accountant,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new User
                {
                    Id = user7Id,
                    Username = "brigadier1",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "brigadier1@lokov.ru",
                    FullName = "Морозов Игорь Петрович",
                    Role = UserRole.Brigadier,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new User
                {
                    Id = user8Id,
                    Username = "brigadier2",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "brigadier2@lokov.ru",
                    FullName = "Волков Николай Александрович",
                    Role = UserRole.Brigadier,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new User
                {
                    Id = user9Id,
                    Username = "brigadier3",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "brigadier3@lokov.ru",
                    FullName = "Зайцев Павел Викторович",
                    Role = UserRole.Brigadier,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new User
                {
                    Id = user10Id,
                    Username = "brigadier4",
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEJVhxXjNTzlfKCDvAP6GWFDv7XwTklDjLJJyEQvkMfR6RhPBMqcjnWXRiYjrXgzi+Q==",
                    Email = "brigadier4@lokov.ru",
                    FullName = "Новиков Роман Геннадьевич",
                    Role = UserRole.Brigadier,
                    IsActive = true,
                    CreatedAt = seedDate,
                }
            );
        #endregion

        #region Brigades Seed Data
        var brigade1Id = Guid.Parse("b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1");
        var brigade2Id = Guid.Parse("b2b2b2b2-b2b2-b2b2-b2b2-b2b2b2b2b2b2");
        var brigade3Id = Guid.Parse("b3b3b3b3-b3b3-b3b3-b3b3-b3b3b3b3b3b3");
        var brigade4Id = Guid.Parse("b4b4b4b4-b4b4-b4b4-b4b4-b4b4b4b4b4b4");
        var brigade5Id = Guid.Parse("b5b5b5b5-b5b5-b5b5-b5b5-b5b5b5b5b5b5");

        modelBuilder
            .Entity<Brigade>()
            .HasData(
                new Brigade
                {
                    Id = brigade1Id,
                    Name = "Бригада №1 (Универсальная)",
                    ForemanName = "Морозов И.П.",
                    Phone = "+79011112233",
                    WorkersCount = 5,
                    Specialization = BrigadeSpecialization.Universal,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new Brigade
                {
                    Id = brigade2Id,
                    Name = "Бригада №2 (Кровельная)",
                    ForemanName = "Волков Н.А.",
                    Phone = "+79012223344",
                    WorkersCount = 4,
                    Specialization = BrigadeSpecialization.Roofing,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new Brigade
                {
                    Id = brigade3Id,
                    Name = "Бригада №3 (Фасадная)",
                    ForemanName = "Зайцев П.В.",
                    Phone = "+79013334455",
                    WorkersCount = 6,
                    Specialization = BrigadeSpecialization.Facade,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new Brigade
                {
                    Id = brigade4Id,
                    Name = "Бригада №4 (Отделочная)",
                    ForemanName = "Новиков Р.Г.",
                    Phone = "+79014445566",
                    WorkersCount = 4,
                    Specialization = BrigadeSpecialization.InteriorFinishing,
                    IsActive = true,
                    CreatedAt = seedDate,
                },
                new Brigade
                {
                    Id = brigade5Id,
                    Name = "Бригада №5 (Универсальная)",
                    ForemanName = "Федоров А.С.",
                    Phone = "+79015556677",
                    WorkersCount = 5,
                    Specialization = BrigadeSpecialization.Universal,
                    IsActive = true,
                    CreatedAt = seedDate,
                }
            );
        #endregion

        #region Clients Seed Data (15 clients)
        var client1Id = Guid.Parse("c1000000-0000-0000-0000-000000000001");
        var client2Id = Guid.Parse("c2000000-0000-0000-0000-000000000002");
        var client3Id = Guid.Parse("c3000000-0000-0000-0000-000000000003");
        var client4Id = Guid.Parse("c4000000-0000-0000-0000-000000000004");
        var client5Id = Guid.Parse("c5000000-0000-0000-0000-000000000005");
        var client6Id = Guid.Parse("c6000000-0000-0000-0000-000000000006");
        var client7Id = Guid.Parse("c7000000-0000-0000-0000-000000000007");
        var client8Id = Guid.Parse("c8000000-0000-0000-0000-000000000008");
        var client9Id = Guid.Parse("c9000000-0000-0000-0000-000000000009");
        var client10Id = Guid.Parse("c0a00000-0000-0000-0000-00000000000a");
        var client11Id = Guid.Parse("c0b00000-0000-0000-0000-00000000000b");
        var client12Id = Guid.Parse("c0c00000-0000-0000-0000-00000000000c");
        var client13Id = Guid.Parse("c0d00000-0000-0000-0000-00000000000d");
        var client14Id = Guid.Parse("c0e00000-0000-0000-0000-00000000000e");
        var client15Id = Guid.Parse("c0f00000-0000-0000-0000-00000000000f");

        modelBuilder
            .Entity<Client>()
            .HasData(
                new Client
                {
                    Id = client1Id,
                    FirstName = "Иван",
                    LastName = "Иванов",
                    Patronymic = "Иванович",
                    Phone = "+79011110001",
                    Email = "ivanov@email.ru",
                    Address = "г. Москва, ул. Ленина, д. 1, кв. 1",
                    Source = ClientSource.Recommendation,
                    Status = ClientStatus.Active,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-6),
                },
                new Client
                {
                    Id = client2Id,
                    FirstName = "Петр",
                    LastName = "Петров",
                    Patronymic = "Петрович",
                    Phone = "+79011110002",
                    Email = "petrov@email.ru",
                    Address = "г. Москва, ул. Мира, д. 15, кв. 42",
                    Source = ClientSource.Internet,
                    Status = ClientStatus.Active,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-5),
                },
                new Client
                {
                    Id = client3Id,
                    FirstName = "Сергей",
                    LastName = "Сергеев",
                    Patronymic = "Сергеевич",
                    Phone = "+79011110003",
                    Email = "sergeev@email.ru",
                    Address = "г. Москва, ул. Пушкина, д. 10, кв. 5",
                    Source = ClientSource.SocialMedia,
                    Status = ClientStatus.Regular,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-8),
                },
                new Client
                {
                    Id = client4Id,
                    FirstName = "Анна",
                    LastName = "Антонова",
                    Patronymic = "Викторовна",
                    Phone = "+79011110004",
                    Email = "antonova@email.ru",
                    Address = "г. Москва, ул. Гагарина, д. 20, кв. 78",
                    Source = ClientSource.DirectContact,
                    Status = ClientStatus.Potential,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-1),
                },
                new Client
                {
                    Id = client5Id,
                    FirstName = "Мария",
                    LastName = "Маркова",
                    Patronymic = "Александровна",
                    Phone = "+79011110005",
                    Email = "markova@email.ru",
                    Address = "г. Москва, ул. Строителей, д. 5, кв. 120",
                    Source = ClientSource.Advertisement,
                    Status = ClientStatus.Active,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-3),
                },
                new Client
                {
                    Id = client6Id,
                    FirstName = "Дмитрий",
                    LastName = "Дмитриев",
                    Patronymic = "Анатольевич",
                    Phone = "+79011110006",
                    Email = "dmitriev@email.ru",
                    Address = "г. Москва, пр. Вернадского, д. 78, кв. 15",
                    Source = ClientSource.Recommendation,
                    Status = ClientStatus.Regular,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-12),
                },
                new Client
                {
                    Id = client7Id,
                    FirstName = "Ольга",
                    LastName = "Орлова",
                    Patronymic = "Сергеевна",
                    Phone = "+79011110007",
                    Email = "orlova@email.ru",
                    Address = "г. Москва, ул. Тверская, д. 25, кв. 8",
                    Source = ClientSource.Internet,
                    Status = ClientStatus.Inactive,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-9),
                },
                new Client
                {
                    Id = client8Id,
                    FirstName = "Николай",
                    LastName = "Николаев",
                    Patronymic = "Павлович",
                    Phone = "+79011110008",
                    Email = "nikolaev@email.ru",
                    Address = "г. Москва, ул. Арбат, д. 12, кв. 33",
                    Source = ClientSource.DirectContact,
                    Status = ClientStatus.Potential,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-1),
                },
                new Client
                {
                    Id = client9Id,
                    FirstName = "Екатерина",
                    LastName = "Егорова",
                    Patronymic = "Игоревна",
                    Phone = "+79011110009",
                    Email = "egorova@email.ru",
                    Address = "г. Москва, ул. Профсоюзная, д. 45, кв. 67",
                    Source = ClientSource.SocialMedia,
                    Status = ClientStatus.Active,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-4),
                },
                new Client
                {
                    Id = client10Id,
                    FirstName = "Алексей",
                    LastName = "Алексеев",
                    Patronymic = "Михайлович",
                    Phone = "+79011110010",
                    Email = "alekseev@email.ru",
                    Address = "г. Москва, ул. Южная, д. 30, кв. 90",
                    Source = ClientSource.Recommendation,
                    Status = ClientStatus.Regular,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-15),
                },
                new Client
                {
                    Id = client11Id,
                    FirstName = "Татьяна",
                    LastName = "Титова",
                    Patronymic = "Николаевна",
                    Phone = "+79011110011",
                    Email = "titova@email.ru",
                    Address = "г. Москва, ул. Северная, д. 8, кв. 44",
                    Source = ClientSource.Internet,
                    Status = ClientStatus.Potential,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddDays(-15),
                },
                new Client
                {
                    Id = client12Id,
                    FirstName = "ООО",
                    LastName = "СтройИнвест",
                    Patronymic = null,
                    Phone = "+74951112233",
                    Email = "info@stroyinvest.ru",
                    Address = "г. Москва, ул. Бизнес, д. 10, офис 305",
                    Source = ClientSource.DirectContact,
                    Status = ClientStatus.Active,
                    Category = ClientCategory.LegalEntity,
                    CreatedAt = seedDate.AddMonths(-2),
                },
                new Client
                {
                    Id = client13Id,
                    FirstName = "ИП",
                    LastName = "Сидоров А.В.",
                    Patronymic = null,
                    Phone = "+79011110012",
                    Email = "sidorov@biznes.ru",
                    Address = "г. Москва, ул. Торговая, д. 15",
                    Source = ClientSource.Recommendation,
                    Status = ClientStatus.Active,
                    Category = ClientCategory.Entrepreneur,
                    CreatedAt = seedDate.AddMonths(-3),
                },
                new Client
                {
                    Id = client14Id,
                    FirstName = "Владимир",
                    LastName = "Владимиров",
                    Patronymic = "Романович",
                    Phone = "+79011110013",
                    Email = "vladimirov@email.ru",
                    Address = "г. Москва, ул. Садовая, д. 40, кв. 12",
                    Source = ClientSource.Advertisement,
                    Status = ClientStatus.Potential,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddDays(-7),
                },
                new Client
                {
                    Id = client15Id,
                    FirstName = "Наталья",
                    LastName = "Наумова",
                    Patronymic = "Владимировна",
                    Phone = "+79011110014",
                    Email = "naumova@email.ru",
                    Address = "г. Москва, ул. Цветочная, д. 3, кв. 56",
                    Source = ClientSource.SocialMedia,
                    Status = ClientStatus.Inactive,
                    Category = ClientCategory.Individual,
                    CreatedAt = seedDate.AddMonths(-7),
                }
            );
        #endregion

        #region Projects Seed Data (20 projects)
        var project1Id = Guid.Parse("d1000000-0000-0000-0000-000000000001");
        var project2Id = Guid.Parse("d2000000-0000-0000-0000-000000000002");
        var project3Id = Guid.Parse("d3000000-0000-0000-0000-000000000003");
        var project4Id = Guid.Parse("d4000000-0000-0000-0000-000000000004");
        var project5Id = Guid.Parse("d5000000-0000-0000-0000-000000000005");
        var project6Id = Guid.Parse("d6000000-0000-0000-0000-000000000006");
        var project7Id = Guid.Parse("d7000000-0000-0000-0000-000000000007");
        var project8Id = Guid.Parse("d8000000-0000-0000-0000-000000000008");
        var project9Id = Guid.Parse("d9000000-0000-0000-0000-000000000009");
        var project10Id = Guid.Parse("d0a00000-0000-0000-0000-00000000000a");
        var project11Id = Guid.Parse("d0b00000-0000-0000-0000-00000000000b");
        var project12Id = Guid.Parse("d0c00000-0000-0000-0000-00000000000c");
        var project13Id = Guid.Parse("d0d00000-0000-0000-0000-00000000000d");
        var project14Id = Guid.Parse("d0e00000-0000-0000-0000-00000000000e");
        var project15Id = Guid.Parse("d0f00000-0000-0000-0000-00000000000f");
        var project16Id = Guid.Parse("d0a10000-0000-0000-0000-0000000000a1");
        var project17Id = Guid.Parse("d0b20000-0000-0000-0000-0000000000b2");
        var project18Id = Guid.Parse("d0c30000-0000-0000-0000-0000000000c3");
        var project19Id = Guid.Parse("d0d40000-0000-0000-0000-0000000000d4");
        var project20Id = Guid.Parse("d0e50000-0000-0000-0000-0000000000e5");

        modelBuilder
            .Entity<Project>()
            .HasData(
                new Project
                {
                    Id = project1Id,
                    Number = "KR-2026-001",
                    ClientId = client1Id,
                    Type = ProjectType.MajorRepair,
                    Name = "Капитальный ремонт 2-комнатной квартиры",
                    Description = "Полный капитальный ремонт с заменой коммуникаций",
                    Address = "г. Москва, ул. Ленина, д. 1, кв. 1",
                    EstimatedCost = 850000,
                    ActualCost = 425000,
                    StartDate = seedDate.AddMonths(-2),
                    PlannedEndDate = seedDate.AddMonths(1),
                    Status = ProjectStatus.InProgress,
                    BrigadeId = brigade1Id,
                    CreatedAt = seedDate.AddMonths(-2),
                },
                new Project
                {
                    Id = project2Id,
                    Number = "ROOF-2026-002",
                    ClientId = client2Id,
                    Type = ProjectType.RoofWorks,
                    Name = "Замена кровли частного дома",
                    Description = "Демонтаж старой кровли, монтаж металлочерепицы, утепление",
                    Address = "г. Москва, ул. Мира, д. 15",
                    EstimatedCost = 420000,
                    ActualCost = 380000,
                    StartDate = seedDate.AddMonths(-1),
                    PlannedEndDate = seedDate.AddDays(10),
                    Status = ProjectStatus.InProgress,
                    BrigadeId = brigade2Id,
                    CreatedAt = seedDate.AddMonths(-1),
                },
                new Project
                {
                    Id = project3Id,
                    Number = "FAC-2026-003",
                    ClientId = client3Id,
                    Type = ProjectType.FacadeWorks,
                    Name = "Утепление фасада многоквартирного дома",
                    Description = "Утепление фасада минеральной ватой, декоративная штукатурка",
                    Address = "г. Москва, ул. Пушкина, д. 10",
                    EstimatedCost = 1200000,
                    ActualCost = 800000,
                    StartDate = seedDate.AddMonths(-3),
                    PlannedEndDate = seedDate.AddMonths(1),
                    Status = ProjectStatus.InProgress,
                    BrigadeId = brigade3Id,
                    CreatedAt = seedDate.AddMonths(-3),
                },
                new Project
                {
                    Id = project4Id,
                    Number = "KR-2026-004",
                    ClientId = client5Id,
                    Type = ProjectType.MajorRepair,
                    Name = "Ремонт 3-комнатной квартиры в новостройке",
                    Description = "Черновая отделка, электрика, сантехника, чистовая отделка",
                    Address = "г. Москва, ул. Строителей, д. 5, кв. 120",
                    EstimatedCost = 1500000,
                    ActualCost = 0,
                    StartDate = seedDate.AddDays(14),
                    PlannedEndDate = seedDate.AddMonths(3),
                    Status = ProjectStatus.Contract,
                    BrigadeId = brigade4Id,
                    CreatedAt = seedDate.AddDays(-10),
                },
                new Project
                {
                    Id = project5Id,
                    Number = "ROOF-2026-005",
                    ClientId = client6Id,
                    Type = ProjectType.RoofWorks,
                    Name = "Ремонт плоской кровли офисного здания",
                    Description = "Гидроизоляция, ремонт примыканий, установка аэраторов",
                    Address = "г. Москва, пр. Вернадского, д. 78",
                    EstimatedCost = 680000,
                    ActualCost = 680000,
                    StartDate = seedDate.AddMonths(-5),
                    PlannedEndDate = seedDate.AddMonths(-4),
                    ActualEndDate = seedDate.AddMonths(-4),
                    Status = ProjectStatus.Completed,
                    BrigadeId = brigade2Id,
                    CreatedAt = seedDate.AddMonths(-5),
                },
                new Project
                {
                    Id = project6Id,
                    Number = "KR-2026-006",
                    ClientId = client6Id,
                    Type = ProjectType.PartialRepair,
                    Name = "Ремонт ванной комнаты и санузла",
                    Description = "Замена плитки, сантехники, установка душевой кабины",
                    Address = "г. Москва, пр. Вернадского, д. 78, кв. 15",
                    EstimatedCost = 280000,
                    ActualCost = 270000,
                    StartDate = seedDate.AddMonths(-8),
                    PlannedEndDate = seedDate.AddMonths(-7),
                    ActualEndDate = seedDate.AddMonths(-7),
                    Status = ProjectStatus.Completed,
                    BrigadeId = brigade1Id,
                    CreatedAt = seedDate.AddMonths(-8),
                },
                new Project
                {
                    Id = project7Id,
                    Number = "FAC-2026-007",
                    ClientId = client9Id,
                    Type = ProjectType.FacadeWorks,
                    Name = "Ремонт фасада загородного дома",
                    Description = "Очистка фасада, ремонт трещин, покраска",
                    Address = "г. Москва, ул. Профсоюзная, д. 45",
                    EstimatedCost = 350000,
                    ActualCost = 150000,
                    StartDate = seedDate.AddDays(-20),
                    PlannedEndDate = seedDate.AddDays(20),
                    Status = ProjectStatus.InProgress,
                    BrigadeId = brigade3Id,
                    CreatedAt = seedDate.AddDays(-30),
                },
                new Project
                {
                    Id = project8Id,
                    Number = "KR-2026-008",
                    ClientId = client3Id,
                    Type = ProjectType.CombinedWorks,
                    Name = "Комплексный ремонт дома",
                    Description = "Ремонт кровли, фасада и внутренних помещений",
                    Address = "г. Москва, ул. Пушкина, д. 10",
                    EstimatedCost = 2500000,
                    ActualCost = 2200000,
                    StartDate = seedDate.AddMonths(-10),
                    PlannedEndDate = seedDate.AddMonths(-2),
                    ActualEndDate = seedDate.AddMonths(-2),
                    Status = ProjectStatus.Completed,
                    BrigadeId = brigade1Id,
                    CreatedAt = seedDate.AddMonths(-10),
                },
                new Project
                {
                    Id = project9Id,
                    Number = "KR-2026-009",
                    ClientId = client1Id,
                    Type = ProjectType.PartialRepair,
                    Name = "Замена окон и балконного остекления",
                    Description = "Демонтаж старых окон, установка ПВХ, отделка откосов",
                    Address = "г. Москва, ул. Ленина, д. 1, кв. 1",
                    EstimatedCost = 180000,
                    ActualCost = 180000,
                    StartDate = seedDate.AddMonths(-12),
                    PlannedEndDate = seedDate.AddMonths(-11),
                    ActualEndDate = seedDate.AddMonths(-11),
                    Status = ProjectStatus.Completed,
                    BrigadeId = brigade4Id,
                    CreatedAt = seedDate.AddMonths(-12),
                },
                new Project
                {
                    Id = project10Id,
                    Number = "ROOF-2026-010",
                    ClientId = client12Id,
                    Type = ProjectType.RoofWorks,
                    Name = "Капитальный ремонт кровли бизнес-центра",
                    Description = "Полная замена кровельного покрытия на площади 1200 м²",
                    Address = "г. Москва, ул. Бизнес, д. 10",
                    EstimatedCost = 3500000,
                    ActualCost = 0,
                    StartDate = seedDate.AddDays(30),
                    PlannedEndDate = seedDate.AddMonths(3),
                    Status = ProjectStatus.Approval,
                    BrigadeId = brigade2Id,
                    CreatedAt = seedDate.AddDays(-5),
                },
                new Project
                {
                    Id = project11Id,
                    Number = "KR-2026-011",
                    ClientId = client13Id,
                    Type = ProjectType.MajorRepair,
                    Name = "Ремонт торгового павильона",
                    Description = "Капитальный ремонт с перепланировкой",
                    Address = "г. Москва, ул. Торговая, д. 15",
                    EstimatedCost = 950000,
                    ActualCost = 600000,
                    StartDate = seedDate.AddMonths(-1),
                    PlannedEndDate = seedDate.AddMonths(1),
                    Status = ProjectStatus.InProgress,
                    BrigadeId = brigade1Id,
                    CreatedAt = seedDate.AddMonths(-1),
                },
                new Project
                {
                    Id = project12Id,
                    Number = "FAC-2026-012",
                    ClientId = client12Id,
                    Type = ProjectType.FacadeWorks,
                    Name = "Утепление фасада офисного здания",
                    Description = "Система вентилируемого фасада",
                    Address = "г. Москва, ул. Бизнес, д. 15",
                    EstimatedCost = 2800000,
                    ActualCost = 0,
                    StartDate = seedDate.AddDays(60),
                    PlannedEndDate = seedDate.AddMonths(5),
                    Status = ProjectStatus.Estimate,
                    BrigadeId = brigade3Id,
                    CreatedAt = seedDate.AddDays(-3),
                },
                new Project
                {
                    Id = project13Id,
                    Number = "KR-2026-013",
                    ClientId = client7Id,
                    Type = ProjectType.PartialRepair,
                    Name = "Ремонт кухни",
                    Description = "Замена кухонного гарнитура, фартука, освещения",
                    Address = "г. Москва, ул. Тверская, д. 25, кв. 8",
                    EstimatedCost = 220000,
                    ActualCost = 220000,
                    StartDate = seedDate.AddMonths(-6),
                    PlannedEndDate = seedDate.AddMonths(-5),
                    ActualEndDate = seedDate.AddMonths(-5),
                    Status = ProjectStatus.Completed,
                    BrigadeId = brigade4Id,
                    CreatedAt = seedDate.AddMonths(-6),
                },
                new Project
                {
                    Id = project14Id,
                    Number = "ROOF-2026-014",
                    ClientId = client15Id,
                    Type = ProjectType.RoofWorks,
                    Name = "Ремонт кровли гаража",
                    Description = "Замена профнастила, ремонт стропильной системы",
                    Address = "г. Москва, ул. Цветочная, д. 3",
                    EstimatedCost = 95000,
                    ActualCost = 0,
                    StartDate = seedDate.AddDays(-5),
                    PlannedEndDate = seedDate.AddDays(5),
                    Status = ProjectStatus.InProgress,
                    BrigadeId = brigade2Id,
                    CreatedAt = seedDate.AddDays(-10),
                },
                new Project
                {
                    Id = project15Id,
                    Number = "KR-2026-015",
                    ClientId = client10Id,
                    Type = ProjectType.MajorRepair,
                    Name = "Капитальный ремонт 1-комнатной квартиры",
                    Description = "Полный ремонт с заменой всего",
                    Address = "г. Москва, ул. Южная, д. 30, кв. 90",
                    EstimatedCost = 750000,
                    ActualCost = 740000,
                    StartDate = seedDate.AddMonths(-14),
                    PlannedEndDate = seedDate.AddMonths(-12),
                    ActualEndDate = seedDate.AddMonths(-12),
                    Status = ProjectStatus.Completed,
                    BrigadeId = brigade1Id,
                    CreatedAt = seedDate.AddMonths(-14),
                },
                new Project
                {
                    Id = project16Id,
                    Number = "FAC-2026-016",
                    ClientId = client11Id,
                    Type = ProjectType.FacadeWorks,
                    Name = "Облицовка цоколя",
                    Description = "Облицовка цоколя керамогранитом",
                    Address = "г. Москва, ул. Северная, д. 8",
                    EstimatedCost = 120000,
                    ActualCost = 0,
                    StartDate = seedDate.AddDays(10),
                    PlannedEndDate = seedDate.AddDays(20),
                    Status = ProjectStatus.Approval,
                    BrigadeId = brigade3Id,
                    CreatedAt = seedDate.AddDays(-5),
                },
                new Project
                {
                    Id = project17Id,
                    Number = "KR-2026-017",
                    ClientId = client14Id,
                    Type = ProjectType.MajorRepair,
                    Name = "Ремонт квартиры студии",
                    Description = "Дизайнерский ремонт студии 25 м²",
                    Address = "г. Москва, ул. Садовая, д. 40, кв. 12",
                    EstimatedCost = 550000,
                    ActualCost = 0,
                    StartDate = seedDate.AddDays(21),
                    PlannedEndDate = seedDate.AddMonths(2),
                    Status = ProjectStatus.Estimate,
                    BrigadeId = brigade4Id,
                    CreatedAt = seedDate.AddDays(-2),
                },
                new Project
                {
                    Id = project18Id,
                    Number = "ROOF-2026-018",
                    ClientId = client4Id,
                    Type = ProjectType.RoofWorks,
                    Name = "Монтаж водостоков",
                    Description = "Установка водосточной системы",
                    Address = "г. Москва, ул. Гагарина, д. 20",
                    EstimatedCost = 65000,
                    ActualCost = 0,
                    StartDate = null,
                    PlannedEndDate = null,
                    Status = ProjectStatus.New,
                    BrigadeId = brigade2Id,
                    CreatedAt = seedDate.AddDays(-1),
                },
                new Project
                {
                    Id = project19Id,
                    Number = "KR-2026-019",
                    ClientId = client8Id,
                    Type = ProjectType.PartialRepair,
                    Name = "Замена электропроводки",
                    Description = "Полная замена проводки в квартире",
                    Address = "г. Москва, ул. Арбат, д. 12, кв. 33",
                    EstimatedCost = 85000,
                    ActualCost = 0,
                    StartDate = null,
                    PlannedEndDate = null,
                    Status = ProjectStatus.Estimate,
                    BrigadeId = brigade5Id,
                    CreatedAt = seedDate.AddDays(-3),
                },
                new Project
                {
                    Id = project20Id,
                    Number = "FAC-2026-020",
                    ClientId = client12Id,
                    Type = ProjectType.FacadeWorks,
                    Name = "Покраска фасада склада",
                    Description = "Подготовка и покраска фасада складского помещения",
                    Address = "г. Москва, ул. Складская, д. 5",
                    EstimatedCost = 450000,
                    ActualCost = 450000,
                    StartDate = seedDate.AddMonths(-4),
                    PlannedEndDate = seedDate.AddMonths(-3),
                    ActualEndDate = seedDate.AddMonths(-3),
                    Status = ProjectStatus.Completed,
                    BrigadeId = brigade3Id,
                    CreatedAt = seedDate.AddMonths(-4),
                }
            );
        #endregion

        #region ProjectStages Seed Data
        modelBuilder
            .Entity<ProjectStage>()
            .HasData(
                // Stages for project1 (KR-2026-001)
                new ProjectStage
                {
                    Id = Guid.Parse("e1000000-0000-0000-0000-000000000001"),
                    ProjectId = project1Id,
                    Name = "Осмотр объекта",
                    Order = 1,
                    PlannedDays = 1,
                    ActualDays = 1,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-2),
                    StartedAt = seedDate.AddMonths(-2),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e1000000-0000-0000-0000-000000000002"),
                    ProjectId = project1Id,
                    Name = "Расчет сметы",
                    Order = 2,
                    PlannedDays = 3,
                    ActualDays = 2,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-2).AddDays(2),
                    StartedAt = seedDate.AddMonths(-2).AddDays(1),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e1000000-0000-0000-0000-000000000003"),
                    ProjectId = project1Id,
                    Name = "Согласование",
                    Order = 3,
                    PlannedDays = 5,
                    ActualDays = 5,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-2).AddDays(7),
                    StartedAt = seedDate.AddMonths(-2).AddDays(2),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e1000000-0000-0000-0000-000000000004"),
                    ProjectId = project1Id,
                    Name = "Договор и предоплата",
                    Order = 4,
                    PlannedDays = 3,
                    ActualDays = 3,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-2).AddDays(10),
                    StartedAt = seedDate.AddMonths(-2).AddDays(7),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e1000000-0000-0000-0000-000000000005"),
                    ProjectId = project1Id,
                    Name = "Закупка материалов",
                    Order = 5,
                    PlannedDays = 7,
                    ActualDays = 5,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-2).AddDays(15),
                    StartedAt = seedDate.AddMonths(-2).AddDays(10),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e1000000-0000-0000-0000-000000000006"),
                    ProjectId = project1Id,
                    Name = "Демонтажные работы",
                    Order = 6,
                    PlannedDays = 5,
                    ActualDays = 4,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-1).AddDays(-25),
                    StartedAt = seedDate.AddMonths(-2).AddDays(15),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e1000000-0000-0000-0000-000000000007"),
                    ProjectId = project1Id,
                    Name = "Черновая отделка",
                    Order = 7,
                    PlannedDays = 14,
                    ActualDays = 10,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-1).AddDays(-15),
                    StartedAt = seedDate.AddMonths(-1).AddDays(-25),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e1000000-0000-0000-0000-000000000008"),
                    ProjectId = project1Id,
                    Name = "Чистовая отделка",
                    Order = 8,
                    PlannedDays = 21,
                    ActualDays = null,
                    Status = StageStatus.InProgress,
                    StartedAt = seedDate.AddMonths(-1).AddDays(-15),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e1000000-0000-0000-0000-000000000009"),
                    ProjectId = project1Id,
                    Name = "Сдача объекта",
                    Order = 9,
                    PlannedDays = 2,
                    ActualDays = null,
                    Status = StageStatus.Planned,
                },
                // Stages for project2 (ROOF-2026-002)
                new ProjectStage
                {
                    Id = Guid.Parse("e2000000-0000-0000-0000-000000000001"),
                    ProjectId = project2Id,
                    Name = "Осмотр кровли",
                    Order = 1,
                    PlannedDays = 1,
                    ActualDays = 1,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-1),
                    StartedAt = seedDate.AddMonths(-1),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e2000000-0000-0000-0000-000000000002"),
                    ProjectId = project2Id,
                    Name = "Демонтаж старой кровли",
                    Order = 2,
                    PlannedDays = 3,
                    ActualDays = 2,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-1).AddDays(3),
                    StartedAt = seedDate.AddMonths(-1).AddDays(1),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e2000000-0000-0000-0000-000000000003"),
                    ProjectId = project2Id,
                    Name = "Монтаж обрешетки",
                    Order = 3,
                    PlannedDays = 4,
                    ActualDays = 3,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-1).AddDays(7),
                    StartedAt = seedDate.AddMonths(-1).AddDays(3),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e2000000-0000-0000-0000-000000000004"),
                    ProjectId = project2Id,
                    Name = "Укладка металлочерепицы",
                    Order = 4,
                    PlannedDays = 5,
                    ActualDays = null,
                    Status = StageStatus.InProgress,
                    StartedAt = seedDate.AddDays(-15),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e2000000-0000-0000-0000-000000000005"),
                    ProjectId = project2Id,
                    Name = "Монтаж водостоков",
                    Order = 5,
                    PlannedDays = 3,
                    ActualDays = null,
                    Status = StageStatus.Planned,
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e2000000-0000-0000-0000-000000000006"),
                    ProjectId = project2Id,
                    Name = "Сдача работы",
                    Order = 6,
                    PlannedDays = 1,
                    ActualDays = null,
                    Status = StageStatus.Planned,
                },
                // Stages for completed project5 (ROOF-2026-005)
                new ProjectStage
                {
                    Id = Guid.Parse("e5000000-0000-0000-0000-000000000001"),
                    ProjectId = project5Id,
                    Name = "Осмотр объекта",
                    Order = 1,
                    PlannedDays = 1,
                    ActualDays = 1,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-5),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e5000000-0000-0000-0000-000000000002"),
                    ProjectId = project5Id,
                    Name = "Ремонт кровли",
                    Order = 2,
                    PlannedDays = 15,
                    ActualDays = 13,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-4),
                },
                new ProjectStage
                {
                    Id = Guid.Parse("e5000000-0000-0000-0000-000000000003"),
                    ProjectId = project5Id,
                    Name = "Сдача объекта",
                    Order = 3,
                    PlannedDays = 1,
                    ActualDays = 1,
                    Status = StageStatus.Completed,
                    CompletedAt = seedDate.AddMonths(-4),
                }
            );
        #endregion

        #region Payments Seed Data
        modelBuilder
            .Entity<Payment>()
            .HasData(
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-000000000001"),
                    ProjectId = project1Id,
                    Amount = 425000,
                    Type = PaymentType.Prepayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-2).AddDays(10),
                    Comment = "Аванс 50%",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-2).AddDays(10),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-000000000002"),
                    ProjectId = project2Id,
                    Amount = 210000,
                    Type = PaymentType.Prepayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-1).AddDays(1),
                    Comment = "Аванс 50%",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-1).AddDays(1),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-000000000003"),
                    ProjectId = project2Id,
                    Amount = 170000,
                    Type = PaymentType.Intermediate,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddDays(-5),
                    Comment = "Промежуточная оплата",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddDays(-5),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-000000000004"),
                    ProjectId = project3Id,
                    Amount = 600000,
                    Type = PaymentType.Prepayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-3).AddDays(5),
                    Comment = "Аванс 50%",
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddMonths(-3).AddDays(5),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-000000000005"),
                    ProjectId = project3Id,
                    Amount = 200000,
                    Type = PaymentType.Intermediate,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-1),
                    Comment = "Промежуточный платеж",
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddMonths(-1),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-000000000006"),
                    ProjectId = project5Id,
                    Amount = 340000,
                    Type = PaymentType.Prepayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-5).AddDays(3),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-5).AddDays(3),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-000000000007"),
                    ProjectId = project5Id,
                    Amount = 340000,
                    Type = PaymentType.FinalPayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-4).AddDays(5),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-4).AddDays(5),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-000000000008"),
                    ProjectId = project6Id,
                    Amount = 140000,
                    Type = PaymentType.Prepayment,
                    Method = PaymentMethod.Cash,
                    PaymentDate = seedDate.AddMonths(-8).AddDays(2),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-8).AddDays(2),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-000000000009"),
                    ProjectId = project6Id,
                    Amount = 130000,
                    Type = PaymentType.FinalPayment,
                    Method = PaymentMethod.Cash,
                    PaymentDate = seedDate.AddMonths(-7).AddDays(5),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-7).AddDays(5),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-00000000000a"),
                    ProjectId = project8Id,
                    Amount = 1200000,
                    Type = PaymentType.Prepayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-10).AddDays(5),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-10).AddDays(5),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-00000000000b"),
                    ProjectId = project8Id,
                    Amount = 1000000,
                    Type = PaymentType.FinalPayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-2).AddDays(5),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-2).AddDays(5),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-00000000000c"),
                    ProjectId = project11Id,
                    Amount = 475000,
                    Type = PaymentType.Prepayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-1).AddDays(3),
                    Comment = "Аванс 50%",
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddMonths(-1).AddDays(3),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-00000000000d"),
                    ProjectId = project15Id,
                    Amount = 375000,
                    Type = PaymentType.Prepayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-14).AddDays(3),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-14).AddDays(3),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-00000000000e"),
                    ProjectId = project15Id,
                    Amount = 365000,
                    Type = PaymentType.FinalPayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-12).AddDays(5),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-12).AddDays(5),
                },
                new Payment
                {
                    Id = Guid.Parse("f1000000-0000-0000-0000-00000000000f"),
                    ProjectId = project20Id,
                    Amount = 450000,
                    Type = PaymentType.FinalPayment,
                    Method = PaymentMethod.BankTransfer,
                    PaymentDate = seedDate.AddMonths(-3).AddDays(5),
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddMonths(-3).AddDays(5),
                }
            );
        #endregion

        #region ProjectDocuments Seed Data
        modelBuilder
            .Entity<ProjectDocument>()
            .HasData(
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-000000000001"),
                    ProjectId = project1Id,
                    Type = DocumentType.CommercialOffer,
                    Number = "КП-2026-001",
                    FilePath = "/documents/КП-2026-001.pdf",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-2).AddDays(3),
                },
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-000000000002"),
                    ProjectId = project1Id,
                    Type = DocumentType.Contract,
                    Number = "Д-2026-001",
                    FilePath = "/documents/Д-2026-001.pdf",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-2).AddDays(10),
                },
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-000000000003"),
                    ProjectId = project2Id,
                    Type = DocumentType.Contract,
                    Number = "Д-2026-002",
                    FilePath = "/documents/Д-2026-002.pdf",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-1).AddDays(1),
                },
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-000000000004"),
                    ProjectId = project5Id,
                    Type = DocumentType.AcceptanceAct,
                    Number = "А-2026-005",
                    FilePath = "/documents/А-2026-005.pdf",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-4).AddDays(5),
                },
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-000000000005"),
                    ProjectId = project6Id,
                    Type = DocumentType.AcceptanceAct,
                    Number = "А-2026-006",
                    FilePath = "/documents/А-2026-006.pdf",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-7).AddDays(5),
                },
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-000000000006"),
                    ProjectId = project8Id,
                    Type = DocumentType.AcceptanceAct,
                    Number = "А-2026-008",
                    FilePath = "/documents/А-2026-008.pdf",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-2).AddDays(5),
                },
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-000000000007"),
                    ProjectId = project3Id,
                    Type = DocumentType.Contract,
                    Number = "Д-2026-003",
                    FilePath = "/documents/Д-2026-003.pdf",
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddMonths(-3).AddDays(5),
                },
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-000000000008"),
                    ProjectId = project11Id,
                    Type = DocumentType.Contract,
                    Number = "Д-2026-011",
                    FilePath = "/documents/Д-2026-011.pdf",
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddMonths(-1).AddDays(3),
                },
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-000000000009"),
                    ProjectId = project4Id,
                    Type = DocumentType.CommercialOffer,
                    Number = "КП-2026-004",
                    FilePath = "/documents/КП-2026-004.pdf",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddDays(-8),
                },
                new ProjectDocument
                {
                    Id = Guid.Parse("01000000-0000-0000-0000-00000000000a"),
                    ProjectId = project4Id,
                    Type = DocumentType.Contract,
                    Number = "Д-2026-004",
                    FilePath = "/documents/Д-2026-004.pdf",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddDays(-5),
                }
            );
        #endregion

        #region ClientInteractions Seed Data
        modelBuilder
            .Entity<ClientInteraction>()
            .HasData(
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-000000000001"),
                    ClientId = client1Id,
                    Type = InteractionType.Call,
                    Description = "Звонок по заявке с сайта",
                    Result = "Договорились о встрече",
                    InteractionDate = seedDate.AddMonths(-6).AddDays(1),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-6).AddDays(1),
                },
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-000000000002"),
                    ClientId = client1Id,
                    Type = InteractionType.Meeting,
                    Description = "Осмотр квартиры для ремонта",
                    Result = "Сделали замеры, обсудили пожелания",
                    InteractionDate = seedDate.AddMonths(-6).AddDays(5),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-6).AddDays(5),
                },
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-000000000003"),
                    ClientId = client2Id,
                    Type = InteractionType.Call,
                    Description = "Первичный звонок клиента",
                    Result = "Нужна замена кровли",
                    InteractionDate = seedDate.AddMonths(-5).AddDays(1),
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddMonths(-5).AddDays(1),
                },
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-000000000004"),
                    ClientId = client3Id,
                    Type = InteractionType.Meeting,
                    Description = "Обсуждение комплексного проекта",
                    Result = "Подписали предварительное соглашение",
                    InteractionDate = seedDate.AddMonths(-8).AddDays(10),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-8).AddDays(10),
                },
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-000000000005"),
                    ClientId = client6Id,
                    Type = InteractionType.Call,
                    Description = "Повторное обращение по кровле",
                    Result = "Согласовали дату осмотра",
                    InteractionDate = seedDate.AddMonths(-5).AddDays(1),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-5).AddDays(1),
                },
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-000000000006"),
                    ClientId = client12Id,
                    Type = InteractionType.Meeting,
                    Description = "Встреча с директором ООО СтройИнвест",
                    Result = "Обсудили два проекта",
                    InteractionDate = seedDate.AddMonths(-2).AddDays(5),
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddMonths(-2).AddDays(5),
                },
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-000000000007"),
                    ClientId = client5Id,
                    Type = InteractionType.Email,
                    Description = "Отправлено коммерческое предложение",
                    Result = "Клиент согласился",
                    InteractionDate = seedDate.AddDays(-15),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddDays(-15),
                },
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-000000000008"),
                    ClientId = client8Id,
                    Type = InteractionType.Message,
                    Description = "Сообщение в WhatsApp",
                    Result = "Интересуется замена проводки",
                    InteractionDate = seedDate.AddDays(-5),
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddDays(-5),
                },
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-000000000009"),
                    ClientId = client11Id,
                    Type = InteractionType.Call,
                    Description = "Звонок по рекламе",
                    Result = "Нужна облицовка цоколя",
                    InteractionDate = seedDate.AddDays(-10),
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddDays(-10),
                },
                new ClientInteraction
                {
                    Id = Guid.Parse("02000000-0000-0000-0000-00000000000a"),
                    ClientId = client14Id,
                    Type = InteractionType.Call,
                    Description = "Первичное обращение",
                    Result = "Интересует ремонт студии",
                    InteractionDate = seedDate.AddDays(-5),
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddDays(-5),
                }
            );
        #endregion

        #region Comments Seed Data
        modelBuilder
            .Entity<Comment>()
            .HasData(
                new Comment
                {
                    Id = Guid.Parse("03000000-0000-0000-0000-000000000001"),
                    ClientId = client1Id,
                    Content = "Очень требовательный клиент, важно соблюдать сроки",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-2),
                },
                new Comment
                {
                    Id = Guid.Parse("03000000-0000-0000-0000-000000000002"),
                    ClientId = client3Id,
                    Content = "Постоянный клиент, уже третий проект. Предоставить скидку 5%",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-3),
                },
                new Comment
                {
                    Id = Guid.Parse("03000000-0000-0000-0000-000000000003"),
                    ClientId = client6Id,
                    Content = "Платежеспособный клиент, рекомендовал нас своим знакомым",
                    CreatedById = user2Id,
                    CreatedAt = seedDate.AddMonths(-6),
                },
                new Comment
                {
                    Id = Guid.Parse("03000000-0000-0000-0000-000000000004"),
                    ClientId = client12Id,
                    Content =
                        "Крупный корпоративный клиент, перспективный для долгосрочного сотрудничества",
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddMonths(-2),
                },
                new Comment
                {
                    Id = Guid.Parse("03000000-0000-0000-0000-000000000005"),
                    ClientId = client9Id,
                    Content = "Клиент попросил дополнительную скидку на материалы",
                    CreatedById = user3Id,
                    CreatedAt = seedDate.AddDays(-15),
                }
            );
        #endregion

        #region ProjectExpenses Seed Data
        modelBuilder
            .Entity<ProjectExpense>()
            .HasData(
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-000000000001"),
                    ProjectId = project1Id,
                    Name = "Строительные материалы",
                    Category = ExpenseCategory.Materials,
                    Amount = 250000,
                    Date = seedDate.AddMonths(-1).AddDays(-20),
                    Description = "Закупка основных материалов",
                },
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-000000000002"),
                    ProjectId = project1Id,
                    Name = "Оплата рабочим",
                    Category = ExpenseCategory.Work,
                    Amount = 120000,
                    Date = seedDate.AddDays(-15),
                },
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-000000000003"),
                    ProjectId = project1Id,
                    Name = "Транспортные расходы",
                    Category = ExpenseCategory.Transport,
                    Amount = 15000,
                    Date = seedDate.AddDays(-20),
                },
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-000000000004"),
                    ProjectId = project2Id,
                    Name = "Металлочерепица",
                    Category = ExpenseCategory.Materials,
                    Amount = 200000,
                    Date = seedDate.AddDays(-25),
                },
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-000000000005"),
                    ProjectId = project2Id,
                    Name = "Оплата кровельщикам",
                    Category = ExpenseCategory.Work,
                    Amount = 100000,
                    Date = seedDate.AddDays(-15),
                },
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-000000000006"),
                    ProjectId = project3Id,
                    Name = "Минеральная вата",
                    Category = ExpenseCategory.Materials,
                    Amount = 400000,
                    Date = seedDate.AddMonths(-2),
                },
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-000000000007"),
                    ProjectId = project3Id,
                    Name = "Оплата фасадчикам",
                    Category = ExpenseCategory.Work,
                    Amount = 250000,
                    Date = seedDate.AddMonths(-1),
                },
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-000000000008"),
                    ProjectId = project8Id,
                    Name = "Строительные материалы",
                    Category = ExpenseCategory.Materials,
                    Amount = 1200000,
                    Date = seedDate.AddMonths(-9),
                },
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-000000000009"),
                    ProjectId = project8Id,
                    Name = "Оплата работ",
                    Category = ExpenseCategory.Work,
                    Amount = 700000,
                    Date = seedDate.AddMonths(-8),
                },
                new ProjectExpense
                {
                    Id = Guid.Parse("04000000-0000-0000-0000-00000000000a"),
                    ProjectId = project11Id,
                    Name = "Материалы для ремонта",
                    Category = ExpenseCategory.Materials,
                    Amount = 300000,
                    Date = seedDate.AddDays(-20),
                }
            );
        #endregion
    }
}
