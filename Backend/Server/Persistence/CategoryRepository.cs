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
                .Include(i => i.SubCategories) //ICollection
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await DbContext.Categories
                //.Include(i => i.SubCategories) //ICollection
                .AsNoTracking()
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

    }
}