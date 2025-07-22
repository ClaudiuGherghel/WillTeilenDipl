using Core.Contracts;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    internal class SubCategoryRepository (ApplicationDbContext dbContext) : ISubCategoryRepository
    {

        public ApplicationDbContext DbContext { get; } = dbContext;


        public async Task<int> CountAsync()
        {
            return await DbContext.SubCategories
                .CountAsync();
        }

        public async Task<ICollection<SubCategory>> GetAllAsync()
        {
            return await DbContext.SubCategories
                //.Include(i => i.Category)
                //.Include(i => i.Items) //ICollection
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<SubCategory?> GetByIdAsync(int id)
        {
            return await DbContext.SubCategories
                //.Include(i => i.Category)
                .Include(i => i.Items) //ICollection
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public void Insert(SubCategory subCategoryToPost)
        {
            DbContext.SubCategories.Add(subCategoryToPost);
        }

        public void Update(SubCategory subCategoryToPut)
        {
            DbContext.SubCategories.Update(subCategoryToPut);
        }

        public void Delete(SubCategory subCategoryToRemove)
        {
            DbContext.SubCategories.Remove(subCategoryToRemove);
        }
    }
}