using Core.Contracts;
using Core.Entities;
using Core.Enums;
using Core.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Persistence.Validations.EntityValidators;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        public ISubCategoryRepository SubCategoryRepository { get; }
        public IItemRepository ItemRepository { get; }
        public IRentalRepository RentalRepository { get; }
        public IImageRepository ImageRepository { get; }
        public IUserRepository UserRepository { get; }
        public IGeoPostalRepository GeoPostalRepository { get; }

        private readonly ApplicationDbContext _dbContext;

        // Validatoren als private Felder, damit wir sie nicht bei jedem Aufruf neu bauen müssen
        private readonly CategoryValidator _categoryValidator;
        private readonly UserValidator _userValidator;
        private readonly SubCategoryValidator _subCategoryValidator;
        private readonly ImageValidator _imageValidator;

        public UnitOfWork() : this(new ApplicationDbContext()) { }

        public UnitOfWork(ApplicationDbContext context)
        {
            _dbContext = context;

            _categoryValidator = new CategoryValidator(_dbContext);
            _userValidator = new UserValidator(_dbContext);
            _subCategoryValidator = new SubCategoryValidator(_dbContext);
            _imageValidator = new ImageValidator(_dbContext);

            CategoryRepository = new CategoryRepository(_dbContext);
            SubCategoryRepository = new SubCategoryRepository(_dbContext);
            ItemRepository = new ItemRepository(_dbContext);
            RentalRepository = new RentalRepository(_dbContext);
            ImageRepository = new ImageRepository(_dbContext);
            UserRepository = new UserRepository(_dbContext);
            GeoPostalRepository = new GeoPostalRepository(_dbContext);
        }

        public UnitOfWork(IConfiguration configuration) : this(new ApplicationDbContext(configuration)) { }

        public async Task DeleteDatabaseAsync()
        {
            await _dbContext.Database.EnsureDeletedAsync();
        }

        public async Task MigrateDatabaseAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                await _dbContext.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }



        public async Task<int> SaveChangesAsync(bool checkMemory = false)
        {
            var entities = _dbContext.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity)

                .ToArray();

            foreach (var entity in entities)
            {
                await ValidateEntity(entity, checkMemory);
            }

            return await _dbContext.SaveChangesAsync();
        }


        private async Task ValidateEntity(object entity, bool checkMemory = false)
        {
            // 1. Standard DataAnnotations prüfen
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            // Führt die Validierung aller markierten Eigenschaften aus
            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            // 2. Wenn ungültig → detaillierte Fehlermeldung zusammenbauen
            if (!isValid)
            {
                // Name der Entitätsklasse, z. B. "User", "Item"
                string entityId = entity.GetType().GetProperty("Id")?.GetValue(entity)?.ToString() ?? "Unbekannt";
                // Versuche, die Id (sofern vorhanden) zu bekommen
                string entityName = $"{entity.GetType().Name} (Id: {entityId})";

                var detailedErrors = validationResults.Select(result =>
                {
                    // Einzelne Fehlernachrichten (z. B. "Username: ist erforderlich")
                    string properties = string.Join(", ", result.MemberNames);
                    // Wenn keine Property angegeben → nur die Nachricht
                    return string.IsNullOrWhiteSpace(properties)
                        ? result.ErrorMessage
                        : $"{properties}: {result.ErrorMessage}";
                });

                string fullMessage = $"Validierungsfehler in '{entityName}':\n" + string.Join("\n", detailedErrors);
                throw new ValidationException(fullMessage);
            }

            // Zusätzliche Validierungen pro Entität via Strategy Pattern

            switch (entity)
            {
                case Category category:
                    await _categoryValidator.ValidateAsync(category, checkMemory);
                    break;
                case SubCategory subCategory:
                    await _subCategoryValidator.ValidateAsync(subCategory, checkMemory);
                    break;
                case User user:
                    await _userValidator.ValidateAsync(user, checkMemory);
                    break;

                case Image image:
                    await _imageValidator.ValidateAsync(image, checkMemory);
                    break;
                    //    Für andere Entities ggf. erweitern
            }

            //if (entity is Category category)
            //{
            //    bool exists = await _dbContext.Categories
            //        .AnyAsync(c => c.Id != category.Id &&
            //                       EF.Functions.Like(c.Name, category.Name));

            //    if (exists)
            //    {
            //        throw new ValidationException(
            //            new ValidationResult("Category mit gleichem Namen existiert bereits", [nameof(Category.Name)]), null, category);
            //    }
            //}

            //if (entity is SubCategory subCategory)
            //{
            //    bool exists = await _dbContext.SubCategories
            //        .AnyAsync(sc => sc.Id != subCategory.Id &&
            //                        EF.Functions.Like(sc.Name, subCategory.Name));

            //    if (exists)
            //    {
            //        throw new ValidationException(
            //            new ValidationResult("SubCategory mit gleichem Namen existiert bereits", [nameof(SubCategory.Name)]), null, subCategory);
            //    }
            //}

            //if (entity is Image image)
            //{
            //    bool exists = await _dbContext.Images
            //        .AnyAsync(img => img.Id != image.Id &&
            //                         EF.Functions.Like(img.ImageUrl, image.ImageUrl));

            //    if (exists)
            //    {
            //        throw new ValidationException(
            //            new ValidationResult("Image mit der gleichen URL existiert bereits", [nameof(Image.ImageUrl)]), null, image);
            //    }
            //}

            //if (entity is User user)
            //{
            //    bool emailExists = await _dbContext.Users
            //        .AnyAsync(u => u.Id != user.Id &&
            //                       EF.Functions.Like(u.Email, user.Email));

            //    if (emailExists)
            //    {
            //        throw new ValidationException(
            //            new ValidationResult("E-Mail existiert bereits", [nameof(User.Email)]), null, user);
            //    }

            //    bool usernameExists = await _dbContext.Users
            //        .AnyAsync(u => u.Id != user.Id &&
            //                       EF.Functions.Like(u.Username, user.Username));

            //    if (usernameExists)
            //    {
            //        throw new ValidationException(
            //            new ValidationResult("Benutzername existiert bereits", [nameof(User.Username)]), null, user);
            //    }
            //}
        }

        public async Task FillDbAsync()
        {
            await DeleteDatabaseAsync();
            await MigrateDatabaseAsync();

            string[][]? subCategoryCsv = await FillDbHelper.ReadStringMatrixFromCsvAsync("subCategories.csv", true);
            string[][]? itemCsv = await FillDbHelper.ReadStringMatrixFromCsvAsync("items.csv", true);
            string[][]? rentalCsv = await FillDbHelper.ReadStringMatrixFromCsvAsync("rentals.csv", true);
            string[][]? plzCsv = await FillDbHelper.ReadStringMatrixFromCsvAsync("plz.csv", true);
            string[][]? userCsv = await FillDbHelper.ReadStringMatrixFromCsvAsync("users.csv", true);

            var geoPostals = plzCsv
                .Select(g => new GeoPostal()
                {
                    Country = g[0],
                    State = g[1],
                    PostalCode = g[2],
                    Place = g[3]
                })
                .ToList();

            var categories = subCategoryCsv
                .GroupBy(c => c[0])
                .Select(g => new Category { 
                    Name = g.Key,
                })
                .ToList();


            var subCategories = subCategoryCsv
                .Select(sc => new SubCategory
                {
                    Name = sc[1],
                    Category = categories.Single(c => string.Equals(c.Name.Trim(), sc[0].Trim(), StringComparison.OrdinalIgnoreCase)),
                })
                .ToList();

            var users = userCsv.Select(u => new User()
            {
                UserName = u[0],
                PasswordHash = SecurityHelper.HashPassword(u[1]),
                Email = u[2],
                FirstName = u[3],
                LastName = u[4],
                BirthDate = DateTime.ParseExact(u[5], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Role = Enum.Parse<Roles>(u[6]),
                GeoPostal = geoPostals.Single(g => string.Equals(g.Country, u[7], StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(g.State, u[8], StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(g.PostalCode, u[9], StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(g.Place, u[10], StringComparison.OrdinalIgnoreCase)),
                Address = u[11],
                PhoneNumber = u[12],
            })
                .ToList();



            var items = itemCsv.Select(i => new Item
            {
                SubCategory = subCategories.Single(sc => string.Equals(sc.Name.Trim(), i[0].Trim(), StringComparison.OrdinalIgnoreCase)),
                Name = i[1],
                Description = i[2],
                Price = decimal.Parse(i[3]),
                Stock = int.Parse(i[4]),
                RentalType = Enum.Parse<RentalType>(i[5]),
                ItemCondition = Enum.Parse<ItemCondition>(i[6]),
                Deposit = decimal.Parse(i[7]),
                GeoPostal = geoPostals.Single(g=> string.Equals(g.Country, i[8], StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(g.State, i[9], StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(g.PostalCode, i[10], StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(g.Place, i[11], StringComparison.OrdinalIgnoreCase)),
                Address = i[12],
                Owner = users.Single(u => string.Equals(u.UserName, i[13], StringComparison.OrdinalIgnoreCase))
            }).ToList();

            var rentals = rentalCsv
                .GroupBy(r=> new
                {
                    Owner = users.Single(u => string.Equals(u.UserName, r[0], StringComparison.OrdinalIgnoreCase)),
                    Renter = users.Single(u => string.Equals(u.UserName, r[1], StringComparison.OrdinalIgnoreCase)),
                    SubCategoryName = r[2],
                    ItemName = r[3],
                    From = DateTime.Parse(r[4], CultureInfo.InvariantCulture),
                    To = DateTime.Parse(r[5], CultureInfo.InvariantCulture)
                })
                .Select(grp => new Rental
            {
                Owner = grp.Key.Owner,
                Renter = grp.Key.Renter,
                Item = items.Single(i=> string.Equals(i.Name, grp.Key.ItemName, StringComparison.OrdinalIgnoreCase) &&
                                        string.Equals(i.SubCategory!.Name, grp.Key.SubCategoryName, StringComparison.OrdinalIgnoreCase)),
                From = grp.Key.From,
                To = grp.Key.To
            }).ToList();

            _dbContext.AddRange(geoPostals);
            _dbContext.AddRange(categories);
            _dbContext.AddRange(subCategories);
            _dbContext.AddRange(users);
            _dbContext.AddRange(items);
            _dbContext.AddRange(rentals);
            await SaveChangesAsync(checkMemory: true);


            Console.WriteLine("Datenbank wurde erfolgreich gelöscht, migriert und befüllt.");
        }
    }
}
