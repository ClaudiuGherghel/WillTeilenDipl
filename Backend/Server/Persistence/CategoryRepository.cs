using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class CategoryRepository(ApplicationDbContext dbContext) : ICategoryRepository
    {
        public ApplicationDbContext DbContext { get; } = dbContext;

        public async Task<int> CountAsync()
        {
            return await DbContext.Categories.CountAsync();
        }

        public async Task<ICollection<Category>> GetAllAsync()
        {
            return await DbContext.Categories
                .AsNoTracking()
                .Include(i => i.SubCategories) //ICollection
                .Where(w=> w.IsDeleted == false)
                .OrderBy(o=> o.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await DbContext.Categories
                .AsNoTracking()
                .Include(i => i.SubCategories) //ICollection
                .Where(w=> w.IsDeleted == false)
                .SingleOrDefaultAsync(s => s.Id == id);
        }
        public void Insert(Category categoryToPost)
        {
            DbContext.Categories.Add(categoryToPost);
        }

        public void Update(Category categoryToPut)
        {
            DbContext.Categories.Update(categoryToPut);
        }

        public void Delete(Category categoryToRemove)
        {
            DbContext.Remove(categoryToRemove);
        }

        public void SoftDelete(int id)
        {
            var category = DbContext.Categories
                   .Include(c => c.SubCategories)
                       .ThenInclude(sc => sc.Items)
                           .ThenInclude(i => i.Images)
                   .Include(c => c.SubCategories)
                       .ThenInclude(sc => sc.Items)
                           .ThenInclude(i => i.Rentals)
                   .FirstOrDefault(c => c.Id == id);

            if (category == null)
                return;

            category.IsDeleted = true;
            category.UpdatedAt = DateTime.UtcNow;

            foreach (var sc in category.SubCategories)
            {
                sc.IsDeleted = true;
                sc.UpdatedAt = DateTime.UtcNow;

                foreach (var item in sc.Items)
                {
                    item.IsDeleted = true;
                    item.UpdatedAt = DateTime.UtcNow;

                    foreach (var img in item.Images)
                    {
                        img.IsDeleted = true;
                        img.UpdatedAt = DateTime.UtcNow;
                    }

                    foreach (var rental in item.Rentals)
                    {
                        rental.IsDeleted = true;
                        rental.UpdatedAt = DateTime.UtcNow;
                    }
                }
            }



        }
    }
}