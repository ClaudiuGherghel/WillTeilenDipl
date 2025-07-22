using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork() : this(new ApplicationDbContext()) { }

        private UnitOfWork(ApplicationDbContext context)
        {
            _dbContext = context;
            CategoryRepository = new CategoryRepository(_dbContext);
            SubCategoryRepository = new SubCategoryRepository(_dbContext);
            ItemRepository = new ItemRepository(_dbContext);
            RentalRepository = new RentalRepository(_dbContext);
            ImageRepository = new ImageRepository(_dbContext);
            UserRepository = new UserRepository(_dbContext);
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


        public async Task<int> SaveChangesAsync()
        {
            var entities = _dbContext!.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Added
                                 || entity.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToArray();  // Geänderte Entities ermitteln

            // Allfällige Validierungen der geänderten Entities durchführen
            foreach (var entity in entities)
            {
                await ValidateEntity(entity);
            }
            return await _dbContext.SaveChangesAsync();
        }

        private async Task ValidateEntity(object entity)
        {
            if (entity is Category category)
            {
                bool exists = await _dbContext.Categories
                    .AnyAsync(a => a.Id != category.Id &&
                                   string.Equals(a.Name, category.Name, StringComparison.OrdinalIgnoreCase));

                if (exists)
                {
                    throw new ValidationException(
                        new ValidationResult("Category mit gleichem Namen existiert bereits", [nameof(Category.Name)]), null, category);
                }
            }


            if (entity is Image image)
            {
                bool exists = await _dbContext.Images.AnyAsync(a => a.Id != image.Id && string.Equals(a.ImageUrl, image.ImageUrl, StringComparison.OrdinalIgnoreCase));
                if (exists)
                {
                    throw new ValidationException(
                        new ValidationResult("Image mit der gleichen URL exisitiert bereits",[nameof(Image.ImageUrl)]), null, image);
                }
            }

            if (entity is SubCategory subCategory)
            {
                bool exists = await _dbContext.SubCategories
                    .AnyAsync(a => a.Id != subCategory.Id &&
                                   string.Equals(a.Name, subCategory.Name, StringComparison.OrdinalIgnoreCase));

                if (exists)
                {
                    throw new ValidationException(
                        new ValidationResult("SubCategory mit gleichem Namen existiert bereits", [nameof(SubCategory.Name)]), null, subCategory);
                }
            }

            if (entity is User user)
            {
                bool exists = await _dbContext.Users
                    .AnyAsync(a => a.Id != user.Id &&
                                   string.Equals(a.Email, user.Email, StringComparison.OrdinalIgnoreCase));

                if (exists)
                {
                    throw new ValidationException(
                        new ValidationResult("E-Mail existiert bereits", [nameof(User.Email)]), null, user);
                }
            }
        }

        public async Task FillDbAsync()
        {
            await DeleteDatabaseAsync();
            await MigrateDatabaseAsync();
            await _dbContext.SaveChangesAsync();
        }

    }
}
